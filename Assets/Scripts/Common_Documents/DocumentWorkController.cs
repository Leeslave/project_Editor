using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    /// <summary>두 문서 업무의 씬 진입, 세션, 평판 및 제출 흐름을 공통 처리한다.</summary>
    public abstract class DocumentWorkController : MonoBehaviour
    {
        [SerializeField] private DocumentDesktopHost host;
        [SerializeField] private TextAsset assignmentContent;

        public DocumentSubmission LastSubmission { get; private set; }
        public event Action<DocumentSubmission> Submitted;

        protected DocumentAssignment Assignment { get; private set; }
        protected DocumentDesktop Desktop { get; private set; }
        protected IWorkService WorkService => GameSystem.GetService<IWorkService>();

        protected abstract DocumentMode Mode { get; }
        protected abstract string UnauthorizedViewMessage { get; }

        protected virtual void Start()
        {
            IWorkService workService = WorkService;
            string workCode = Mode == DocumentMode.Forgery ? "SecureDocument" : "Document";
            if (workService == null || workService.CurrentWorkCode != workCode) return;
            if (host == null || assignmentContent == null)
            {
                EditorLogger.LogError($"{Mode} host or content is missing.");
                return;
            }

            int stage = workService.GetStage(workCode);
            Assignment = LoadAssignment(stage);
            if (Assignment == null)
            {
                EditorLogger.LogError($"{Mode} assignment not found: stage {stage}");
                return;
            }
            if (!DocumentRuntimeSession.TryBegin(Assignment, workService, out var state, out string error))
            {
                EditorLogger.LogError(error);
                return;
            }

            host.Show(Assignment, state);
            Desktop = host.Desktop;
            Desktop.DocumentOpened += OnDocumentOpened;
            Desktop.SubmissionRequested += Submit;
            AttachModeHandlers();
            Desktop.RefreshControls();
        }

        private DocumentAssignment LoadAssignment(int stage)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            var assignments = JsonConvert.DeserializeObject<List<DocumentAssignment>>(assignmentContent.text, settings);
            return assignments?.SingleOrDefault(item => item.Mode == Mode && item.Stage == stage);
        }

        private void OnDocumentOpened(ViewRecord view)
        {
            if (!view.Authorized)
            {
                AddEvent(ReputationReason.UnauthorizedView, ReputationTiming.View, ReputationUnit.View,
                    view.Id, view.Id);
                Desktop.ShowStatusMessage(UnauthorizedViewMessage);
            }
            CaptureAndApply();
        }

        protected void AddEvent(ReputationReason reason, ReputationTiming timing, ReputationUnit unit,
            string subjectId, params string[] keyParts)
        {
            DocumentPlayState state = Desktop.GetStateSnapshot();
            var parts = new List<string> { state.WorkInstanceId, reason.ToString() };
            parts.AddRange(keyParts);
            ReputationRule rule = Assignment.Policy.Reputation.FirstOrDefault(item =>
                item.Reason == reason && item.Timing == timing && item.Unit == unit);
            Desktop.AddReputationEvent(new ReputationEvent
            {
                Id = DocumentContract.Key(parts.ToArray()),
                Reason = reason,
                Timing = timing,
                Unit = unit,
                SubjectId = subjectId,
                Delta = rule?.Delta
            });
        }

        protected void CaptureAndApply()
        {
            DocumentRuntimeSession.CaptureAndApply(Desktop.GetStateSnapshot());
        }

        protected void FinishSubmission(DocumentSubmission submission, string title, List<string> resultLines)
        {
            if (!DocumentRuntimeSession.Complete(submission, WorkService, out string error))
            {
                Desktop.ShowStatusMessage(error);
                return;
            }

            LastSubmission = submission;
            DocumentRuntimeSession.ApplyStoryBranch(Assignment.Policy, submission.WorkInstanceId);
            int reputationDelta = DocumentRuntimeSession.ReputationDelta(submission.Snapshot);
            resultLines.Add($"평판 변화: {(reputationDelta >= 0 ? "+" : "")}{reputationDelta}");
            resultLines.Add("업무 제출이 완료되었습니다. 확인하면 업무 화면으로 돌아갑니다.");
            Desktop.ShowSubmissionResult(title, resultLines, DocumentRuntimeSession.ReturnToScreen);
            Submitted?.Invoke(DocumentContract.Copy(submission));
        }

        protected abstract void AttachModeHandlers();
        protected abstract void DetachModeHandlers();
        protected abstract void Submit();

        protected virtual void OnDestroy()
        {
            if (Desktop == null) return;
            DetachModeHandlers();
            Desktop.DocumentOpened -= OnDocumentOpened;
            Desktop.SubmissionRequested -= Submit;
        }
    }
}
