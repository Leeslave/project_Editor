namespace GameService
{
    /// <summary>인증을 포함하는 업무 실행 호스트. 기존 독립 업무 서비스에는 요구하지 않는다.</summary>
    public interface IWorkExecutionService : IWorkService
    {
        string CurrentExecutionId { get; }
        bool IsWorkAuthorized(string workCode);
        void ReturnToWorkList();
    }
}
