using System;
using System.Collections.Generic;
using GameService;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    /// <summary>두 문서 업무의 씬 진입, 세션, 평판 및 제출 흐름을 공통 처리한다.</summary>
    public abstract class DocumentWorkController : MonoBehaviour
    {
        [SerializeField] private DocumentDesktopHost host;

        public DocumentSubmission LastSubmission { get; private set; }
        public event Action<DocumentSubmission> Submitted;

        protected DocumentAssignment Assignment { get; private set; }
        protected DocumentStageData StageData { get; private set; }
        protected DocumentDesktop Desktop { get; private set; }
        protected IWorkService WorkService => GameSystem.GetService<IWorkService>();

        protected abstract DocumentMode Mode { get; }
        protected abstract string UnauthorizedViewMessage { get; }

        protected virtual void Start()
        {
            IWorkService workService = WorkService;
            string workCode = GetWorkCode(Mode);
            if (workService == null || workService.CurrentWorkCode != workCode) return;
            if (host == null)
            {
                EditorLogger.LogError($"{Mode} host or content is missing.");
                return;
            }

            int stage = workService.GetStage(workCode);
            StageData = DocumentStageRepository.Load(workCode, stage);
            Assignment = StageData == null ? null : StageData.Assignment;
            if (StageData == null || Assignment == null)
            {
                EditorLogger.LogError($"{Mode} assignment not found: stage {stage}");
                return;
            }
            if (!DocumentRuntimeSession.TryBegin(StageData, workService, out var state, out string error))
            {
                EditorLogger.LogError(error);
                return;
            }

            OpenDesktop(state);
        }

        private static string GetWorkCode(DocumentMode mode)
        {
            return mode == DocumentMode.Forgery ? "SecureDocument" : "Document";
        }

        private void OpenDesktop(DocumentPlayState state)
        {
            host.Show(Assignment, state, LoadPortraits());
            Desktop = host.Desktop;
            Desktop.DocumentOpened += OnDocumentOpened;
            Desktop.SubmissionRequested += Submit;
            AttachModeHandlers();
            Desktop.RefreshControls();
        }

        private Dictionary<string, Sprite> LoadPortraits()
        {
            var portraits = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (PersonContent person in Assignment.People)
            {
                if (string.IsNullOrWhiteSpace(person.PortraitKey)) continue;
                Sprite portrait = Resources.Load<Sprite>("Image/" + person.PortraitKey);
                if (portrait != null) portraits[person.PortraitKey] = portrait;
                else EditorLogger.LogWarning("Document portrait not found: " + person.PortraitKey);
            }
            return portraits;
        }

        private void OnDocumentOpened(ViewRecord view)
        {
            if (!view.Authorized)
            {
                Desktop.ShowStatusMessage(UnauthorizedViewMessage);
            }
            CaptureState();
        }

        protected void CaptureState()
        {
            DocumentRuntimeSession.Capture(Desktop.GetStateSnapshot());
        }

        protected void FinishSubmission(DocumentSubmission submission, string title, List<string> resultLines)
        {
            int delta = StageData.Settlement.Calculate(submission.IncorrectCount, submission.CaughtCount,
                StageData.IsConfidential);
            submission.ReputationDelta = delta;
            if (!DocumentRuntimeSession.Complete(submission, WorkService, delta, out string error))
            {
                Desktop.ShowStatusMessage(error);
                return;
            }

            LastSubmission = submission;
            resultLines.Add($"오류 {submission.IncorrectCount}건 · 발각 {submission.CaughtCount}건");
            resultLines.Add($"평판 변화: {(delta >= 0 ? "+" : "")}{delta}");
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
