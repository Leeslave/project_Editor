using System;
using System.Collections.Generic;
using GameService;

namespace EditorGame.Documents
{
    /// <summary>문서 업무의 씬 재진입 상태와 최종 제출을 관리한다.</summary>
    public static class DocumentRuntimeSession
    {
        private sealed class RuntimeState
        {
            public DocumentPlayState PlayState;
            public bool WorkApplied;
        }

        private static readonly Dictionary<string, RuntimeState> Sessions =
            new Dictionary<string, RuntimeState>(StringComparer.Ordinal);

        public static bool TryBegin(DocumentStageData stageData, IWorkService workService,
            out DocumentPlayState state, out string error)
        {
            state = null;
            error = null;
            if (stageData == null || workService == null)
            {
                error = "문서 업무 실행 정보가 없습니다.";
                return false;
            }

            try { stageData.ValidateForExecution(); }
            catch (Exception exception)
            {
                error = "문서 스테이지 데이터가 올바르지 않습니다: " + exception.Message;
                return false;
            }
            if (!stageData.IsUnlocked(GameSystem.SaveService))
            {
                error = "극비 업무의 선행 업무가 완료되지 않았습니다.";
                return false;
            }

            DocumentAssignment assignment = stageData.Assignment;
            int saveId = GameSystem.SaveService == null ? -1 : GameSystem.SaveService.CurrentSaveId;
            string workInstanceId = DocumentContract.Key("document-work", saveId.ToString(),
                assignment.WorkCode, assignment.Stage.ToString(), assignment.Id);
            if (!Sessions.TryGetValue(workInstanceId, out RuntimeState runtime))
            {
                state = DocumentContract.Begin(assignment, workInstanceId);
                Sessions.Add(workInstanceId, new RuntimeState { PlayState = DocumentContract.CopyState(state) });
            }
            else state = DocumentContract.CopyState(runtime.PlayState);
            return true;
        }

        public static void Capture(DocumentPlayState state)
        {
            if (state == null || string.IsNullOrWhiteSpace(state.WorkInstanceId)) return;
            GetRuntime(state.WorkInstanceId).PlayState = DocumentContract.CopyState(state);
        }

        public static bool Complete(DocumentSubmission submission, IWorkService workService,
            int reputationDelta, out string error)
        {
            error = null;
            if (submission == null || submission.Snapshot == null || workService == null)
            {
                error = "제출 결과 또는 업무 서비스가 없습니다.";
                return false;
            }
            if (workService.CurrentWorkCode != submission.WorkCode)
            {
                error = "실행 중인 업무와 제출 업무가 일치하지 않습니다.";
                return false;
            }

            Capture(submission.Snapshot);
            RuntimeState runtime = GetRuntime(submission.WorkInstanceId);
            if (runtime.WorkApplied) return true;
            if (GameSystem.SaveService == null)
            {
                error = "평판 정산에 필요한 저장 서비스가 없습니다.";
                return false;
            }

            GameSystem.SaveService.TryApplyRenownSettlement(submission.WorkInstanceId, reputationDelta);
            bool allWorkCleared = workService.ClearWork(submission.WorkCode);
            if (!allWorkCleared && !workService.IsWorkClear(submission.WorkCode))
            {
                error = "업무 완료 상태를 반영하지 못했습니다.";
                return false;
            }

            DocumentPlayState applied = DocumentContract.CopyState(submission.Snapshot);
            applied.Phase = SubmissionPhase.Applied;
            runtime.PlayState = applied;
            runtime.WorkApplied = true;
            return true;
        }

        public static void ReturnToScreen()
        {
            if (GameSystem.Instance != null && !GameSystem.Instance.IsLoading)
                GameSystem.Instance.EnterScene("Screen");
        }

        private static RuntimeState GetRuntime(string workInstanceId)
        {
            if (!Sessions.TryGetValue(workInstanceId, out RuntimeState runtime))
            {
                runtime = new RuntimeState();
                Sessions.Add(workInstanceId, runtime);
            }
            return runtime;
        }
    }
}
