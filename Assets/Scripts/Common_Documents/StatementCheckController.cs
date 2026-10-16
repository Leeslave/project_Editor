using System.Collections.Generic;
using System.Linq;

namespace EditorGame.Documents
{
    /// <summary>진술·행동 기록을 판정하고 일반 문서 업무 제출 결과를 만든다.</summary>
    public sealed class StatementCheckController : DocumentWorkController
    {
        protected override DocumentMode Mode => DocumentMode.StatementCheck;
        protected override string UnauthorizedViewMessage => "지정되지 않은 문서를 열람했습니다.";

        private string _selectedStatementLineId;
        private string _selectedActionLineId;

        protected override void AttachModeHandlers()
        {
            Desktop.PairSelectionChanged += OnPairSelectionChanged;
            Desktop.RecordRequested += RecordSelection;
            Desktop.InvestigationCompletionRequested += CompleteInvestigation;
        }

        protected override void DetachModeHandlers()
        {
            Desktop.PairSelectionChanged -= OnPairSelectionChanged;
            Desktop.RecordRequested -= RecordSelection;
            Desktop.InvestigationCompletionRequested -= CompleteInvestigation;
        }

        // 판정에 사용할 진술 및 행동 줄 선택을 보관한다.
        private void OnPairSelectionChanged(string statement, string action)
        {
            _selectedStatementLineId = statement;
            _selectedActionLineId = action;
        }

        // 선택한 조합의 중복 여부와 정답을 판정해 기록한다.
        private void RecordSelection()
        {
            if (_selectedStatementLineId == null || _selectedActionLineId == null) return;
            DocumentPlayState state = Desktop.GetStateSnapshot();
            if (!ContentIndex.DocumentsByLineId.TryGetValue(_selectedStatementLineId, out var statement) ||
                !ContentIndex.DocumentsByLineId.TryGetValue(_selectedActionLineId, out var action) || statement.PersonId != action.PersonId)
                return;

            InvestigationState investigation = state.Investigations.Single(item => item.PersonId == statement.PersonId);
            if (investigation.Completed) return;
            if (investigation.Records.Any(item => item.StatementLineId == _selectedStatementLineId &&
                item.ActionLineId == _selectedActionLineId))
            {
                Desktop.ClearSelection();
                Desktop.ShowStatusMessage("이미 기록한 조합입니다.");
                return;
            }

            ContentIndex.AnswersByPair.TryGetValue(
                DocumentContract.Key(_selectedStatementLineId, _selectedActionLineId), out AnswerPair answer);
            bool correct = answer != null;
            investigation.Records.Add(new PairRecord
            {
                Id = DocumentContract.Key(state.WorkInstanceId, "record", _selectedStatementLineId, _selectedActionLineId),
                StatementLineId = _selectedStatementLineId,
                ActionLineId = _selectedActionLineId,
                MatchedAnswerId = answer?.Id,
                Correct = correct
            });
            Desktop.UpdateInvestigation(investigation);
            CaptureState();
            Desktop.ClearSelection();
            Desktop.ShowStatusMessage("선택한 진술과 행동을 기록했습니다.");
        }

        // 미발견 정답을 계산하고 해당 인물의 조사를 완료한다.
        private void CompleteInvestigation(string personId)
        {
            DocumentPlayState state = Desktop.GetStateSnapshot();
            InvestigationState investigation = state.Investigations.Single(item => item.PersonId == personId);
            if (investigation.Completed || investigation.Records.Count == 0) return;

            var found = new HashSet<string>(investigation.Records
                .Where(item => item.Correct && item.MatchedAnswerId != null)
                .Select(item => item.MatchedAnswerId), System.StringComparer.Ordinal);
            investigation.MissingAnswerIds = Assignment.Answers
                .Where(item => item.PersonId == personId && !found.Contains(item.Id))
                .Select(item => item.Id).ToList();
            investigation.Completed = true;
            Desktop.UpdateInvestigation(investigation);
            CaptureState();
            Desktop.ShowStatusMessage("조사를 완료했습니다. 정확도는 최종 제출 후 공개됩니다.");
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
                Snapshot = snapshot,
                IncorrectCount = snapshot.Investigations.Sum(item =>
                    item.Records.Count(record => !record.Correct) + item.MissingAnswerIds.Count),
                CaughtCount = snapshot.Views.Count(item => !item.Authorized)
            };
            Desktop.ShowStatusMessage(successful ? "모든 기록이 정확합니다." : "오판 또는 누락 기록이 있습니다.");
            FinishSubmission(submission, successful ? "검증 완료" : "검증 결과", SubmissionResults(snapshot));
        }

        // 오판과 누락을 최종 제출 결과 문장으로 만든다.
        private List<string> SubmissionResults(DocumentPlayState state)
        {
            var people = ContentIndex.PeopleById;
            var lines = ContentIndex.LinesById;
            var answers = ContentIndex.AnswersById;
            var result = new List<string>();
            foreach (InvestigationState investigation in state.Investigations)
            {
                foreach (PairRecord record in investigation.Records.Where(item => !item.Correct))
                    result.Add("[오판] " + people[investigation.PersonId].Profile.Name + "\n진술: " + Line(lines, record.StatementLineId) +
                        "\n행동: " + Line(lines, record.ActionLineId));
                foreach (string answerId in investigation.MissingAnswerIds)
                {
                    AnswerPair answer = answers[answerId];
                    result.Add("[누락] " + people[investigation.PersonId].Profile.Name + "\n진술: " + Line(lines, answer.StatementLineId) +
                        "\n행동: " + Line(lines, answer.ActionLineId));
                }
            }
            if (result.Count == 0) result.Add("모든 불일치를 정확히 기록했습니다.");
            result.Add("최종 제출되어 수정할 수 없습니다.");
            return result;
        }

        // ID로 조회한 기록 줄의 시간과 내용을 반환한다.
        private static string Line(IReadOnlyDictionary<string, DocumentLine> lines, string id)
        {
            DocumentLine line = lines[id];
            return line.Time + " " + line.Text;
        }
    }
}
