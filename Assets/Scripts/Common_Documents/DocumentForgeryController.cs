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

        // 프로필 변경 후 진행 상태를 보관하고 안내를 표시한다.
        private void OnProfileChanged(string personId, ProfileField field, string value)
        {
            CaptureState();
            Desktop.ShowStatusMessage("입력 내용은 창을 닫아도 유지됩니다.");
        }

        protected override void Submit()
        {
            if (LastSubmission != null) return;
            DocumentPlayState state = Desktop.GetStateSnapshot();
            List<FieldResult> fields = EvaluateFields(state);
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
                Fields = fields,
                IncorrectCount = fields.Count(item => item.Outcome == FieldOutcome.IncorrectEdit ||
                    item.Outcome == FieldOutcome.UnchangedTarget || item.Outcome == FieldOutcome.UnrequestedEdit),
                CaughtCount = snapshot.Views.Count(item => !item.Authorized) +
                    fields.Count(item => item.Outcome == FieldOutcome.UnrequestedEdit)
            };
            Desktop.ShowStatusMessage(successful ? "모든 조작 지시가 정확합니다." : "오타 또는 미수행 지시가 있습니다.");
            FinishSubmission(submission, successful ? "극비 업무 완료" : "극비 업무 결과", ResultLines(fields));
        }

        // 원본과 제출값을 지시된 목표값과 비교한다.
        private List<FieldResult> EvaluateFields(DocumentPlayState state)
        {
            var people = ContentIndex.PeopleById;
            var submittedProfiles = state.People.ToDictionary(
                item => item.PersonId, item => item.Values, StringComparer.Ordinal);
            var editsByField = ContentIndex.EditsByField;
            var results = new List<FieldResult>();
            foreach (string personId in Assignment.TargetPersonIds)
            {
                foreach (ProfileField field in Enum.GetValues(typeof(ProfileField)))
                {
                    string key = DocumentContract.Key(personId, field.ToString());
                    bool instructed = editsByField.TryGetValue(key, out EditInstruction instruction);
                    string original = people[personId].Profile.Get(field);
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

        // 프로필 판정 결과를 제출 화면의 문장으로 변환한다.
        private List<string> ResultLines(List<FieldResult> fields)
        {
            var people = ContentIndex.PeopleById;
            var result = fields
                .Where(item => item.Outcome != FieldOutcome.Unchanged)
                .Select(field => $"[{OutcomeName(field.Outcome)}] {people[field.PersonId].Profile.Name} · {field.Field}\n" +
                    $"기존: {field.Original}\n입력: {field.Submitted}\n목표: {field.Expected}")
                .ToList();
            if (result.Count == 0) result.Add("수정된 정보가 없습니다.");
            result.Add("최종 제출되어 수정할 수 없습니다.");
            return result;
        }

        // 프로필 판정 결과의 한국어 이름을 반환한다.
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
