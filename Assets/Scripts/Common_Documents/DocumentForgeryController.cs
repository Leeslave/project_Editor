using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorGame.Documents
{
    /// <summary>직접 입력한 인물 정보를 판정하고 극비 문서 업무 제출 결과를 만든다.</summary>
    public sealed class DocumentForgeryController : DocumentWorkController
    {
        protected override DocumentMode Mode => DocumentMode.Forgery;
        protected override string UnauthorizedViewMessage => "지정되지 않은 인물 파일을 열람했습니다.";

        protected override void AttachModeHandlers()
        {
            Desktop.ProfileChanged += OnProfileChanged;
        }

        protected override void DetachModeHandlers()
        {
            Desktop.ProfileChanged -= OnProfileChanged;
        }

        private void OnProfileChanged(string personId, ProfileField field, string value)
        {
            CaptureAndApply();
            Desktop.ShowStatusMessage("입력 내용은 창을 닫아도 유지됩니다.");
        }

        protected override void Submit()
        {
            if (LastSubmission != null) return;
            DocumentPlayState state = Desktop.GetStateSnapshot();
            List<FieldResult> fields = EvaluateFields(state);
            CreateSubmissionEvents(fields);
            Desktop.LockSubmission();

            DocumentPlayState snapshot = Desktop.GetStateSnapshot();
            bool successful = fields.All(field =>
                field.Outcome == FieldOutcome.CorrectEdit || field.Outcome == FieldOutcome.Unchanged);
            var submission = new DocumentSubmission
            {
                WorkInstanceId = snapshot.WorkInstanceId,
                AssignmentId = Assignment.Id,
                WorkCode = Assignment.WorkCode,
                Stage = Assignment.Stage,
                TargetDateId = Assignment.TargetDateId,
                Successful = successful,
                Snapshot = snapshot,
                Fields = fields
            };
            Desktop.ShowStatusMessage(successful ? "모든 조작 지시가 정확합니다." : "오타 또는 미수행 지시가 있습니다.");
            FinishSubmission(submission, successful ? "극비 업무 완료" : "극비 업무 결과", ResultLines(fields));
        }

        private List<FieldResult> EvaluateFields(DocumentPlayState state)
        {
            var originalProfiles = Assignment.People.ToDictionary(
                item => item.Id, item => item.Profile, StringComparer.Ordinal);
            var submittedProfiles = state.People.ToDictionary(
                item => item.PersonId, item => item.Values, StringComparer.Ordinal);
            var editsByField = Assignment.Edits.ToDictionary(
                item => DocumentContract.Key(item.PersonId, item.Field.ToString()), item => item, StringComparer.Ordinal);
            var results = new List<FieldResult>();
            foreach (string personId in Assignment.TargetPersonIds)
            {
                foreach (ProfileField field in Enum.GetValues(typeof(ProfileField)))
                {
                    string key = DocumentContract.Key(personId, field.ToString());
                    bool instructed = editsByField.TryGetValue(key, out EditInstruction instruction);
                    string original = originalProfiles[personId].Get(field);
                    string submittedValue = submittedProfiles[personId].Get(field);
                    string expected = instructed ? instruction.TargetValue : original;
                    results.Add(new FieldResult
                    {
                        PersonId = personId,
                        Field = field,
                        Original = original,
                        Submitted = submittedValue,
                        Expected = expected,
                        Outcome = DocumentContract.ClassifyField(original, submittedValue, expected, instructed)
                    });
                }
            }
            return results;
        }

        private void CreateSubmissionEvents(List<FieldResult> fields)
        {
            var instructions = new HashSet<string>(Assignment.Edits.Select(item =>
                DocumentContract.Key(item.PersonId, item.Field.ToString())), StringComparer.Ordinal);
            foreach (string personId in Assignment.TargetPersonIds)
            {
                List<FieldResult> personFields = fields.Where(item => item.PersonId == personId).ToList();
                if (personFields.All(item => DocumentContract.SameText(item.Original, item.Submitted)))
                {
                    AddEvent(ReputationReason.UnchangedTarget, ReputationTiming.Submission,
                        ReputationUnit.Person, personId, personId);
                    continue;
                }

                foreach (FieldResult field in personFields)
                    AddFieldReputationEvent(personId, field, instructions);
            }
        }

        private void AddFieldReputationEvent(string personId, FieldResult field, HashSet<string> instructions)
        {
            string fieldKey = DocumentContract.Key(personId, field.Field.ToString());
            ReputationReason reason;
            switch (field.Outcome)
            {
                case FieldOutcome.CorrectEdit: reason = ReputationReason.CorrectEdit; break;
                case FieldOutcome.IncorrectEdit: reason = ReputationReason.IncorrectEdit; break;
                case FieldOutcome.UnchangedTarget when instructions.Contains(fieldKey):
                    reason = ReputationReason.UnchangedTarget;
                    break;
                case FieldOutcome.UnrequestedEdit: reason = ReputationReason.UnrequestedEdit; break;
                default: return;
            }
            AddEvent(reason, ReputationTiming.Submission, ReputationUnit.Field,
                fieldKey, personId, field.Field.ToString());
        }

        private List<string> ResultLines(List<FieldResult> fields)
        {
            var names = Assignment.People.ToDictionary(item => item.Id, item => item.Profile.Name, StringComparer.Ordinal);
            var result = fields
                .Where(item => item.Outcome != FieldOutcome.Unchanged)
                .Select(field => $"[{OutcomeName(field.Outcome)}] {names[field.PersonId]} · {field.Field}\n" +
                    $"기존: {field.Original}\n입력: {field.Submitted}\n목표: {field.Expected}")
                .ToList();
            if (result.Count == 0) result.Add("수정된 정보가 없습니다.");
            result.Add("최종 제출되어 수정할 수 없습니다.");
            return result;
        }

        private static string OutcomeName(FieldOutcome outcome)
        {
            switch (outcome)
            {
                case FieldOutcome.CorrectEdit: return "정확";
                case FieldOutcome.IncorrectEdit: return "오답";
                case FieldOutcome.UnchangedTarget: return "미수정";
                case FieldOutcome.UnrequestedEdit: return "비지정 변경";
                default: return "유지";
            }
        }

    }
}
