using System;
using System.Collections.Generic;
using GameAction;
using GameService;
using UnityEngine;
using Utility;

namespace EditorGame.Documents
{
    /// <summary>씬 재진입용 업무 상태를 보관하고 각 결과를 한 번만 적용한다.</summary>
    public static class DocumentRuntimeSession
    {
        private sealed class RuntimeState
        {
            public DocumentPlayState PlayState;
            public readonly HashSet<string> AppliedEventIds = new HashSet<string>(StringComparer.Ordinal);
            public bool WorkApplied;
            public bool StoryApplied;
        }

        private static readonly Dictionary<string, RuntimeState> Sessions =
            new Dictionary<string, RuntimeState>(StringComparer.Ordinal);

        public static bool TryBegin(DocumentAssignment assignment, IWorkService workService,
            out DocumentPlayState state, out string error)
        {
            state = null;
            error = null;
            if (assignment == null || workService == null)
            {
                error = "문서 업무 실행 정보가 없습니다.";
                return false;
            }

            try { DocumentContract.ValidateForExecution(assignment); }
            catch (Exception exception)
            {
                error = "문서 업무 정책이 완성되지 않았습니다: " + exception.Message;
                return false;
            }

            if (assignment.Mode == DocumentMode.Forgery && !PrerequisitesMet(assignment.Policy, workService))
            {
                error = "극비 업무의 선행 업무가 완료되지 않았습니다.";
                return false;
            }

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

        public static void CaptureAndApply(DocumentPlayState state)
        {
            if (state == null || string.IsNullOrWhiteSpace(state.WorkInstanceId)) return;
            RuntimeState runtime = GetRuntime(state.WorkInstanceId);
            runtime.PlayState = DocumentContract.CopyState(state);
            ApplyNewReputationEvents(runtime, state.Events);
        }

        private static void ApplyNewReputationEvents(RuntimeState runtime,
            IEnumerable<ReputationEvent> reputationEvents)
        {
            foreach (ReputationEvent reputationEvent in reputationEvents)
            {
                if (runtime.AppliedEventIds.Contains(reputationEvent.Id)) continue;
                if (!reputationEvent.Delta.HasValue)
                    throw new InvalidOperationException("Unconfigured reputation event: " + reputationEvent.Id);
                if (GameSystem.SaveService == null)
                    throw new InvalidOperationException("Save service is required to apply reputation.");
                new SetRenownAction(reputationEvent.Delta.Value).Invoke();
                runtime.AppliedEventIds.Add(reputationEvent.Id);
            }
        }

        public static bool Complete(DocumentSubmission submission, IWorkService workService, out string error)
        {
            error = null;
            if (submission == null || submission.Snapshot == null || workService == null)
            {
                error = "제출 결과 또는 업무 서비스가 없습니다.";
                return false;
            }

            CaptureAndApply(submission.Snapshot);
            RuntimeState runtime = GetRuntime(submission.WorkInstanceId);
            if (runtime.WorkApplied) return true;
            if (workService.CurrentWorkCode != submission.WorkCode)
            {
                error = "실행 중인 업무와 제출 업무가 일치하지 않습니다.";
                return false;
            }
            if (!workService.ClearWork(submission.WorkCode))
            {
                // 반환값은 하루 전체 완료 여부이므로 현재 업무 상태를 별도로 확인한다.
                if (!workService.IsWorkClear(submission.WorkCode))
                {
                    error = "업무 완료 상태를 반영하지 못했습니다.";
                    return false;
                }
            }

            DocumentPlayState applied = DocumentContract.CopyState(submission.Snapshot);
            applied.Phase = SubmissionPhase.Applied;
            runtime.PlayState = applied;
            runtime.WorkApplied = true;
            return true;
        }

        public static void ApplyStoryBranch(DocumentPolicy policy, string workInstanceId)
        {
            if (policy == null || !policy.StoryThreshold.HasValue ||
                string.IsNullOrWhiteSpace(policy.StoryBranchId) || GameSystem.SaveService == null) return;
            RuntimeState runtime = GetRuntime(workInstanceId);
            if (runtime.StoryApplied) return;
            if (!GameSystem.SaveService.CheckRenown(policy.StoryThreshold.Value)) return;
            if (int.TryParse(policy.StoryBranchId, out int branch))
            {
                GameSystem.SaveService.SwitchBranch(branch);
                runtime.StoryApplied = true;
            }
            else EditorLogger.LogError("Document story branch ID must be an integer: " + policy.StoryBranchId);
        }

        public static int ReputationDelta(DocumentPlayState state)
        {
            int total = 0;
            if (state == null || state.Events == null) return total;
            foreach (ReputationEvent reputationEvent in state.Events)
                if (reputationEvent.Delta.HasValue) total += reputationEvent.Delta.Value;
            return total;
        }

        public static void ReturnToScreen()
        {
            if (GameSystem.Instance != null && !GameSystem.Instance.IsLoading)
                GameSystem.Instance.EnterScene("Screen");
        }

        private static bool PrerequisitesMet(DocumentPolicy policy, IWorkService workService)
        {
            if (policy.UnlockScope != UnlockScope.SameDay) return false;
            if (policy.UnlockCombination == UnlockCombination.All)
                return policy.PrerequisiteWorkCodes.TrueForAll(workService.IsWorkClear);
            if (policy.UnlockCombination == UnlockCombination.Any)
                return policy.PrerequisiteWorkCodes.Exists(workService.IsWorkClear);
            return false;
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
