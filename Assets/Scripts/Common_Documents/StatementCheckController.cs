using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    /// <summary>Evaluates statement/action pairs and publishes a locked submission snapshot.</summary>
    public sealed class StatementCheckController : MonoBehaviour
    {
        [SerializeField] private DocumentDesktopHost host;
        [SerializeField] private TextAsset assignmentContent;

        public DocumentSubmission LastSubmission { get; private set; }
        public event Action<DocumentSubmission> Submitted;

        private IWorkService WorkService => GameSystem.GetService<IWorkService>();
        private DocumentAssignment assignment;
        private DocumentDesktop desktop;
        private string statementId;
        private string actionId;

        private void Start()
        {
            if (host == null || assignmentContent == null)
            {
                EditorLogger.LogError("Statement check host or content is missing.");
                return;
            }
            if (WorkService != null && WorkService.CurrentWorkCode == "SecureDocument") return;

            int stage = WorkService == null ? 0 : WorkService.GetStage("Document");
            if (stage < 0) stage = 0; // Direct scene entry uses the previewable first assignment.
            assignment = LoadAssignment(stage);
            if (assignment == null)
            {
                EditorLogger.LogError($"Statement assignment not found: stage {stage}");
                return;
            }

            string instanceId = DocumentContract.Key("statement-check", assignment.Id, Guid.NewGuid().ToString("N"));
            host.Show(assignment, DocumentContract.Begin(assignment, instanceId));
            desktop = host.Desktop;
            desktop.DocumentOpened += OnDocumentOpened;
            desktop.PairSelectionChanged += OnPairSelectionChanged;
            desktop.RecordRequested += RecordSelection;
            desktop.InvestigationCompletionRequested += CompleteInvestigation;
            desktop.SubmissionRequested += Submit;
        }

        private DocumentAssignment LoadAssignment(int stage)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            var contents = JsonConvert.DeserializeObject<List<DocumentAssignment>>(assignmentContent.text, settings);
            return contents?.SingleOrDefault(item => item.Mode == DocumentMode.StatementCheck && item.Stage == stage);
        }

        private void OnDocumentOpened(ViewRecord view)
        {
            if (view.Authorized) return;
            AddEvent(ReputationReason.UnauthorizedView, ReputationTiming.View, ReputationUnit.View,
                view.Id, view.Id);
            desktop.ShowStatusMessage("지정되지 않은 문서를 열람했습니다.");
        }

        private void OnPairSelectionChanged(string statement, string action)
        {
            statementId = statement;
            actionId = action;
        }

        private void RecordSelection()
        {
            if (statementId == null || actionId == null) return;
            var state = desktop.GetStateSnapshot();
            var documentByLine = assignment.Documents
                .SelectMany(document => document.Lines.Select(line => new { line.Id, Document = document }))
                .ToDictionary(item => item.Id, item => item.Document, StringComparer.Ordinal);
            if (!documentByLine.TryGetValue(statementId, out var statement) ||
                !documentByLine.TryGetValue(actionId, out var action) || statement.PersonId != action.PersonId)
                return;

            var investigation = state.Investigations.Single(item => item.PersonId == statement.PersonId);
            if (investigation.Completed) return;
            if (investigation.Records.Any(item => item.StatementLineId == statementId && item.ActionLineId == actionId))
            {
                desktop.ClearSelection();
                desktop.ShowStatusMessage("이미 기록한 조합입니다.");
                return;
            }

            var answer = assignment.Answers.SingleOrDefault(item =>
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
            var reason = correct ? ReputationReason.CorrectPair : ReputationReason.IncorrectPair;
            AddEvent(reason, ReputationTiming.Record, ReputationUnit.Pair,
                DocumentContract.Key(statementId, actionId), statementId, actionId);
            desktop.UpdateInvestigation(investigation);
            desktop.ClearSelection();
            desktop.ShowStatusMessage(correct ? "불일치를 기록했습니다." : "일치하지 않는 기록을 제출했습니다.");
        }

        private void CompleteInvestigation(string personId)
        {
            var state = desktop.GetStateSnapshot();
            var investigation = state.Investigations.Single(item => item.PersonId == personId);
            if (investigation.Completed || investigation.Records.Count == 0) return;

            var found = new HashSet<string>(investigation.Records
                .Where(item => item.Correct && item.MatchedAnswerId != null)
                .Select(item => item.MatchedAnswerId), StringComparer.Ordinal);
            investigation.MissingAnswerIds = assignment.Answers
                .Where(item => item.PersonId == personId && !found.Contains(item.Id))
                .Select(item => item.Id).ToList();
            investigation.Completed = true;
            foreach (string answerId in investigation.MissingAnswerIds)
                AddEvent(ReputationReason.MissingPair, ReputationTiming.InvestigationComplete,
                    ReputationUnit.Pair, answerId, answerId);
            desktop.UpdateInvestigation(investigation);
            desktop.ShowStatusMessage(investigation.MissingAnswerIds.Count == 0
                ? "조사를 완료했습니다." : $"조사를 완료했습니다. 누락 {investigation.MissingAnswerIds.Count}건");
        }

        private void Submit()
        {
            if (LastSubmission != null) return;
            desktop.LockSubmission();
            var snapshot = desktop.GetStateSnapshot();
            bool successful = snapshot.Investigations.All(item =>
                item.MissingAnswerIds.Count == 0 && item.Records.All(record => record.Correct));
            LastSubmission = new DocumentSubmission
            {
                WorkInstanceId = snapshot.WorkInstanceId,
                AssignmentId = assignment.Id,
                WorkCode = assignment.WorkCode,
                Stage = assignment.Stage,
                TargetDateId = assignment.TargetDateId,
                Successful = successful,
                Snapshot = snapshot
            };
            var resultLines = SubmissionResults(snapshot);
            desktop.ShowStatusMessage(successful ? "모든 기록이 정확합니다." : "오판 또는 누락 기록이 있습니다.");
            desktop.ShowSubmissionResult(successful ? "검증 완료" : "검증 결과", resultLines);
            Submitted?.Invoke(DocumentContract.Copy(LastSubmission));
        }

        private List<string> SubmissionResults(DocumentPlayState state)
        {
            var people = assignment.People.ToDictionary(item => item.Id, item => item.Profile.Name, StringComparer.Ordinal);
            var lines = assignment.Documents.SelectMany(document => document.Lines)
                .ToDictionary(item => item.Id, item => item, StringComparer.Ordinal);
            var answers = assignment.Answers.ToDictionary(item => item.Id, item => item, StringComparer.Ordinal);
            var result = new List<string>();
            foreach (var investigation in state.Investigations)
            {
                foreach (var record in investigation.Records.Where(item => !item.Correct))
                    result.Add("[오판] " + people[investigation.PersonId] + "\n진술: " + Line(lines, record.StatementLineId) +
                        "\n행동: " + Line(lines, record.ActionLineId));
                foreach (string answerId in investigation.MissingAnswerIds)
                {
                    var answer = answers[answerId];
                    result.Add("[누락] " + people[investigation.PersonId] + "\n진술: " + Line(lines, answer.StatementLineId) +
                        "\n행동: " + Line(lines, answer.ActionLineId));
                }
            }
            if (result.Count == 0) result.Add("모든 불일치를 정확히 기록했습니다.");
            result.Add("최종 제출되어 수정할 수 없습니다.");
            return result;
        }

        private static string Line(IReadOnlyDictionary<string, DocumentLine> lines, string id)
        {
            var line = lines[id];
            return line.Time + " " + line.Text;
        }

        private void AddEvent(ReputationReason reason, ReputationTiming timing, ReputationUnit unit,
            string subjectId, params string[] keyParts)
        {
            var state = desktop.GetStateSnapshot();
            var parts = new List<string> { state.WorkInstanceId, reason.ToString() };
            parts.AddRange(keyParts);
            var rule = assignment.Policy.Reputation.FirstOrDefault(item =>
                item.Reason == reason && item.Timing == timing && item.Unit == unit);
            desktop.AddReputationEvent(new ReputationEvent
            {
                Id = DocumentContract.Key(parts.ToArray()),
                Reason = reason,
                Timing = timing,
                Unit = unit,
                SubjectId = subjectId,
                Delta = rule?.Delta
            });
        }

        private void OnDestroy()
        {
            if (desktop == null) return;
            desktop.DocumentOpened -= OnDocumentOpened;
            desktop.PairSelectionChanged -= OnPairSelectionChanged;
            desktop.RecordRequested -= RecordSelection;
            desktop.InvestigationCompletionRequested -= CompleteInvestigation;
            desktop.SubmissionRequested -= Submit;
        }
    }
}
