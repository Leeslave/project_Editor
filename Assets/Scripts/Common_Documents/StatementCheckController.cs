using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorGame.Documents
{
    /// <summary>진술·행동 기록을 판정하고 일반 문서 업무 제출 결과를 만든다.</summary>
    public sealed class StatementCheckController : DocumentWorkController
    {
        protected override DocumentMode Mode => DocumentMode.StatementCheck;
        protected override string UnauthorizedViewMessage => "지정되지 않은 문서를 열람했습니다.";

        private string statementId;
        private string actionId;
        private Dictionary<string, DocumentContent> documentsByLine;

        protected override void AttachModeHandlers()
        {
            Desktop.PairSelectionChanged += OnPairSelectionChanged;
            Desktop.RecordRequested += RecordSelection;
            Desktop.InvestigationCompletionRequested += CompleteInvestigation;
            documentsByLine = Assignment.Documents
                .SelectMany(document => document.Lines.Select(line => new { line.Id, Document = document }))
                .ToDictionary(item => item.Id, item => item.Document, StringComparer.Ordinal);
        }

        protected override void DetachModeHandlers()
        {
            Desktop.PairSelectionChanged -= OnPairSelectionChanged;
            Desktop.RecordRequested -= RecordSelection;
            Desktop.InvestigationCompletionRequested -= CompleteInvestigation;
        }

        private void OnPairSelectionChanged(string statement, string action)
        {
            statementId = statement;
            actionId = action;
        }

        private void RecordSelection()
        {
            if (statementId == null || actionId == null) return;
            DocumentPlayState state = Desktop.GetStateSnapshot();
            if (!documentsByLine.TryGetValue(statementId, out var statement) ||
                !documentsByLine.TryGetValue(actionId, out var action) || statement.PersonId != action.PersonId)
                return;

            InvestigationState investigation = state.Investigations.Single(item => item.PersonId == statement.PersonId);
            if (investigation.Completed) return;
            if (investigation.Records.Any(item => item.StatementLineId == statementId && item.ActionLineId == actionId))
            {
                Desktop.ClearSelection();
                Desktop.ShowStatusMessage("이미 기록한 조합입니다.");
                return;
            }

            AnswerPair answer = Assignment.Answers.SingleOrDefault(item =>
                item.PersonId == statement.PersonId && item.StatementLineId == statementId && item.ActionLineId == actionId);
            bool correct = answer != null;
            investigation.Records.Add(new PairRecord
            {
                Id = DocumentContract.Key(state.WorkInstanceId, "record", statementId, actionId),
                StatementLineId = statementId,
                ActionLineId = actionId,
                MatchedAnswerId = answer?.Id,
                Correct = correct
            });
            AddEvent(correct ? ReputationReason.CorrectPair : ReputationReason.IncorrectPair,
                ReputationTiming.Record, ReputationUnit.Pair,
                DocumentContract.Key(statementId, actionId), statementId, actionId);
            Desktop.UpdateInvestigation(investigation);
            CaptureAndApply();
            Desktop.ClearSelection();
            Desktop.ShowStatusMessage(correct ? "불일치를 기록했습니다." : "일치하지 않는 기록을 제출했습니다.");
        }

        private void CompleteInvestigation(string personId)
        {
            DocumentPlayState state = Desktop.GetStateSnapshot();
            InvestigationState investigation = state.Investigations.Single(item => item.PersonId == personId);
            if (investigation.Completed || investigation.Records.Count == 0) return;

            var found = new HashSet<string>(investigation.Records
                .Where(item => item.Correct && item.MatchedAnswerId != null)
                .Select(item => item.MatchedAnswerId), StringComparer.Ordinal);
            investigation.MissingAnswerIds = Assignment.Answers
                .Where(item => item.PersonId == personId && !found.Contains(item.Id))
                .Select(item => item.Id).ToList();
            investigation.Completed = true;
            foreach (string answerId in investigation.MissingAnswerIds)
                AddEvent(ReputationReason.MissingPair, ReputationTiming.InvestigationComplete,
                    ReputationUnit.Pair, answerId, answerId);
            Desktop.UpdateInvestigation(investigation);
            CaptureAndApply();
            Desktop.ShowStatusMessage(investigation.MissingAnswerIds.Count == 0
                ? "조사를 완료했습니다." : $"조사를 완료했습니다. 누락 {investigation.MissingAnswerIds.Count}건");
        }

        protected override void Submit()
        {
            if (LastSubmission != null) return;
            Desktop.LockSubmission();
            DocumentPlayState snapshot = Desktop.GetStateSnapshot();
            bool successful = snapshot.Investigations.All(item =>
                item.MissingAnswerIds.Count == 0 && item.Records.All(record => record.Correct));
            var submission = new DocumentSubmission
            {
                WorkInstanceId = snapshot.WorkInstanceId,
                AssignmentId = Assignment.Id,
                WorkCode = Assignment.WorkCode,
                Stage = Assignment.Stage,
                TargetDateId = Assignment.TargetDateId,
                Successful = successful,
                Snapshot = snapshot
            };
            Desktop.ShowStatusMessage(successful ? "모든 기록이 정확합니다." : "오판 또는 누락 기록이 있습니다.");
            FinishSubmission(submission, successful ? "검증 완료" : "검증 결과", SubmissionResults(snapshot));
        }

        private List<string> SubmissionResults(DocumentPlayState state)
        {
            var names = Assignment.People.ToDictionary(item => item.Id, item => item.Profile.Name, StringComparer.Ordinal);
            var lines = Assignment.Documents.SelectMany(document => document.Lines)
                .ToDictionary(item => item.Id, item => item, StringComparer.Ordinal);
            var answers = Assignment.Answers.ToDictionary(item => item.Id, item => item, StringComparer.Ordinal);
            var result = new List<string>();
            foreach (InvestigationState investigation in state.Investigations)
            {
                foreach (PairRecord record in investigation.Records.Where(item => !item.Correct))
                    result.Add("[오판] " + names[investigation.PersonId] + "\n진술: " + Line(lines, record.StatementLineId) +
                        "\n행동: " + Line(lines, record.ActionLineId));
                foreach (string answerId in investigation.MissingAnswerIds)
                {
                    AnswerPair answer = answers[answerId];
                    result.Add("[누락] " + names[investigation.PersonId] + "\n진술: " + Line(lines, answer.StatementLineId) +
                        "\n행동: " + Line(lines, answer.ActionLineId));
                }
            }
            if (result.Count == 0) result.Add("모든 불일치를 정확히 기록했습니다.");
            result.Add("최종 제출되어 수정할 수 없습니다.");
            return result;
        }

        private static string Line(IReadOnlyDictionary<string, DocumentLine> lines, string id)
        {
            DocumentLine line = lines[id];
            return line.Time + " " + line.Text;
        }
    }
}
