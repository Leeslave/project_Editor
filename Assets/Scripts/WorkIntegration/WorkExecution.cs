using System;
using System.Collections.Generic;

namespace EditorGame.WorkIntegration
{
    public enum WorkKind { Document, ADFGVX }
    public enum WorkSecurity { Normal, Confidential }
    public enum ExecutionPhase { Idle, AuthenticationRequired, Authenticating, AuthenticationFailed, Working, Completed }

    [Serializable]
    public sealed class WorkAssignment
    {
        public string Id;
        public string Title;
        public WorkKind Kind;
        public WorkSecurity Security;
        public int Stage;
        public int MazeLayout;
        public string WorkCode => Kind == WorkKind.Document
            ? Security == WorkSecurity.Confidential ? "SecureDocument" : "Document"
            : Security == WorkSecurity.Confidential ? "SecureADFGVX" : "ADFGVX";
        public string SceneName => Kind == WorkKind.Document ? "Document_TEST" : "ADFGVX_TEST";
    }

    /// <summary>씬이나 저장 서비스에 의존하지 않는 업무별 인증·완료 상태.</summary>
    public sealed class WorkExecution
    {
        private readonly HashSet<string> completed = new HashSet<string>(StringComparer.Ordinal);
        public WorkAssignment Current { get; private set; }
        public string ExecutionId { get; private set; }
        public ExecutionPhase Phase { get; private set; }
        public bool IsCompleted(string id) => completed.Contains(id);

        public bool Begin(WorkAssignment assignment)
        {
            if (Phase != ExecutionPhase.Idle || assignment == null || string.IsNullOrWhiteSpace(assignment.Id) ||
                assignment.Stage < 0 || completed.Contains(assignment.Id)) return false;
            Current = assignment;
            ExecutionId = Guid.NewGuid().ToString("N");
            Phase = assignment.Security == WorkSecurity.Confidential
                ? ExecutionPhase.AuthenticationRequired : ExecutionPhase.Working;
            return true;
        }

        public bool StartAuthentication()
        {
            if (Phase != ExecutionPhase.AuthenticationRequired && Phase != ExecutionPhase.AuthenticationFailed) return false;
            Phase = ExecutionPhase.Authenticating;
            return true;
        }

        public bool FinishAuthentication(string executionId, bool success)
        {
            if (Phase != ExecutionPhase.Authenticating || executionId != ExecutionId) return false;
            Phase = success ? ExecutionPhase.Working : ExecutionPhase.AuthenticationFailed;
            return true;
        }

        public bool HasAccess(string code) => Phase == ExecutionPhase.Working && Current?.WorkCode == code;

        public bool Complete(string executionId, string code)
        {
            if (executionId != ExecutionId || !HasAccess(code)) return false;
            completed.Add(Current.Id);
            Phase = ExecutionPhase.Completed;
            return true;
        }

        public void Return()
        {
            Current = null;
            ExecutionId = null;
            Phase = ExecutionPhase.Idle;
        }
    }
}
