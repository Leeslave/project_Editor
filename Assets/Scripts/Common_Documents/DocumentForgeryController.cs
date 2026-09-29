using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    /// <summary>Evaluates typed profile edits and publishes a locked confidential-work submission.</summary>
    public sealed class DocumentForgeryController : MonoBehaviour
    {
        [SerializeField] private DocumentDesktopHost host;
        [SerializeField] private TextAsset assignmentContent;

        public DocumentSubmission LastSubmission { get; private set; }
        public event Action<DocumentSubmission> Submitted;

        private IWorkService WorkService => GameSystem.GetService<IWorkService>();
        private DocumentAssignment assignment;
        private DocumentDesktop desktop;

        private void Start()
        {
            if (WorkService == null || WorkService.CurrentWorkCode != "SecureDocument") return;
            if (host == null || assignmentContent == null)
            {
                EditorLogger.LogError("Document forgery host or content is missing.");
                return;
            }

            int stage = WorkService.GetStage("SecureDocument");
            assignment = LoadAssignment(stage);
            if (assignment == null)
            {
                EditorLogger.LogError($"Document forgery assignment not found: stage {stage}");
                return;
            }

            if (!DocumentRuntimeSession.TryBegin(assignment, WorkService, out var state, out string error))
            {
                EditorLogger.LogError(error);
                return;
            }
            host.Show(assignment, state);
            desktop = host.Desktop;
            desktop.DocumentOpened += OnDocumentOpened;
            desktop.ProfileChanged += OnProfileChanged;
            desktop.SubmissionRequested += Submit;
        }

        private DocumentAssignment LoadAssignment(int stage)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            var contents = JsonConvert.DeserializeObject<List<DocumentAssignment>>(assignmentContent.text, settings);
            return contents?.SingleOrDefault(item => item.Mode == DocumentMode.Forgery && item.Stage == stage);
        }

        private void OnDocumentOpened(ViewRecord view)
        {
            if (!view.Authorized)
            {
                AddEvent(ReputationReason.UnauthorizedView, ReputationTiming.View, ReputationUnit.View,
                    view.Id, view.Id);
                desktop.ShowStatusMessage("지정되지 않은 인물 파일을 열람했습니다.");
            }
            CaptureAndApply();
        }

        private void OnProfileChanged(string personId, ProfileField field, string value)
        {
            CaptureAndApply();
            desktop.ShowStatusMessage("입력 내용은 창을 닫아도 유지됩니다.");
        }

        private void Submit()
        {
            if (LastSubmission != null) return;
            DocumentPlayState state = desktop.GetStateSnapshot();
            List<FieldResult> fields = EvaluateFields(state);
            CreateSubmissionEvents(fields);

            desktop.LockSubmission();
            DocumentPlayState snapshot = desktop.GetStateSnapshot();
            bool successful = fields.All(field =>
                field.Outcome == FieldOutcome.CorrectEdit || field.Outcome == FieldOutcome.Unchanged);
            LastSubmission = new DocumentSubmission
            {
                WorkInstanceId = snapshot.WorkInstanceId,
                AssignmentId = assignment.Id,
                WorkCode = assignment.WorkCode,
                Stage = assignment.Stage,
                TargetDateId = assignment.TargetDateId,
                Successful = successful,
                Snapshot = snapshot,
                Fields = fields
            };
            desktop.ShowStatusMessage(successful ? "모든 조작 지시가 정확합니다." : "오타 또는 미수행 지시가 있습니다.");
            if (!DocumentRuntimeSession.Complete(LastSubmission, WorkService, out string error))
            {
                desktop.ShowStatusMessage(error);
                return;
            }
            DocumentRuntimeSession.ApplyStoryBranch(assignment.Policy, LastSubmission.WorkInstanceId);
            List<string> resultLines = ResultLines(fields);
            int reputationDelta = DocumentRuntimeSession.ReputationDelta(snapshot);
            resultLines.Add($"평판 변화: {(reputationDelta >= 0 ? "+" : "")}{reputationDelta}");
            resultLines.Add("업무 제출이 완료되었습니다. 확인하면 업무 화면으로 돌아갑니다.");
            desktop.ShowSubmissionResult(successful ? "극비 업무 완료" : "극비 업무 결과", resultLines,
                DocumentRuntimeSession.ReturnToScreen);
            Submitted?.Invoke(DocumentContract.Copy(LastSubmission));
        }

        private List<FieldResult> EvaluateFields(DocumentPlayState state)
        {
            var originals = assignment.People.ToDictionary(item => item.Id, item => item.Profile, StringComparer.Ordinal);
            var submitted = state.People.ToDictionary(item => item.PersonId, item => item.Values, StringComparer.Ordinal);
            var instructions = assignment.Edits.ToDictionary(
                item => DocumentContract.Key(item.PersonId, item.Field.ToString()), item => item, StringComparer.Ordinal);
            var results = new List<FieldResult>();
            foreach (string personId in assignment.TargetPersonIds)
                foreach (ProfileField field in Enum.GetValues(typeof(ProfileField)))
                {
                    string key = DocumentContract.Key(personId, field.ToString());
                    bool instructed = instructions.TryGetValue(key, out var instruction);
                    string original = GetField(originals[personId], field);
                    string submittedValue = GetField(submitted[personId], field);
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
            return results;
        }

        private void CreateSubmissionEvents(List<FieldResult> fields)
        {
            var instructions = new HashSet<string>(assignment.Edits.Select(item =>
                DocumentContract.Key(item.PersonId, item.Field.ToString())), StringComparer.Ordinal);
            foreach (string personId in assignment.TargetPersonIds)
            {
                List<FieldResult> personFields = fields.Where(item => item.PersonId == personId).ToList();
                bool unchangedPerson = personFields.All(item => DocumentContract.SameText(item.Original, item.Submitted));
                if (unchangedPerson)
                {
                    AddEvent(ReputationReason.UnchangedTarget, ReputationTiming.Submission,
                        ReputationUnit.Person, personId, personId);
                    continue;
                }

                foreach (FieldResult field in personFields)
                {
                    string fieldKey = DocumentContract.Key(personId, field.Field.ToString());
                    bool instructed = instructions.Contains(fieldKey);
                    switch (field.Outcome)
                    {
                        case FieldOutcome.CorrectEdit:
                            AddEvent(ReputationReason.CorrectEdit, ReputationTiming.Submission,
                                ReputationUnit.Field, fieldKey, personId, field.Field.ToString());
                            break;
                        case FieldOutcome.IncorrectEdit:
                            AddEvent(ReputationReason.IncorrectEdit, ReputationTiming.Submission,
                                ReputationUnit.Field, fieldKey, personId, field.Field.ToString());
                            break;
                        case FieldOutcome.UnchangedTarget when instructed:
                            AddEvent(ReputationReason.UnchangedTarget, ReputationTiming.Submission,
                                ReputationUnit.Field, fieldKey, personId, field.Field.ToString());
                            break;
                        case FieldOutcome.UnrequestedEdit:
                            AddEvent(ReputationReason.UnrequestedEdit, ReputationTiming.Submission,
                                ReputationUnit.Field, fieldKey, personId, field.Field.ToString());
                            break;
                    }
                }
            }
        }

        private List<string> ResultLines(List<FieldResult> fields)
        {
            var names = assignment.People.ToDictionary(item => item.Id, item => item.Profile.Name, StringComparer.Ordinal);
            var result = new List<string>();
            foreach (FieldResult field in fields.Where(item => item.Outcome != FieldOutcome.Unchanged))
                result.Add($"[{OutcomeName(field.Outcome)}] {names[field.PersonId]} · {field.Field}\n" +
                    $"기존: {field.Original}\n입력: {field.Submitted}\n목표: {field.Expected}");
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

        private void AddEvent(ReputationReason reason, ReputationTiming timing, ReputationUnit unit,
            string subjectId, params string[] keyParts)
        {
            DocumentPlayState state = desktop.GetStateSnapshot();
            var parts = new List<string> { state.WorkInstanceId, reason.ToString() };
            parts.AddRange(keyParts);
            ReputationRule rule = assignment.Policy.Reputation.FirstOrDefault(item =>
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

        private static string GetField(ProfileValues values, ProfileField field)
        {
            switch (field)
            {
                case ProfileField.Name: return values.Name;
                case ProfileField.Age: return values.Age;
                case ProfileField.Sex: return values.Sex;
                case ProfileField.Country: return values.Country;
                default: return values.Job;
            }
        }

        private void CaptureAndApply()
        {
            DocumentRuntimeSession.CaptureAndApply(desktop.GetStateSnapshot());
        }

        private void OnDestroy()
        {
            if (desktop == null) return;
            desktop.DocumentOpened -= OnDocumentOpened;
            desktop.ProfileChanged -= OnProfileChanged;
            desktop.SubmissionRequested -= Submit;
        }
    }
}
