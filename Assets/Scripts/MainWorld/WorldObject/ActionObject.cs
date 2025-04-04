
public class ActionObject : WorldObject
{
    public string actionName;
    public string actionParam;
    private Action action;
    
    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>Action 초기화</remarks>
    public override void OnAwake()
    {
        base.OnAwake();
        
        action = ActionHandler.GetAction(actionName, actionParam);
    }

    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>액션 실행 및 삭제</remarks>
    public override void OnEnable()
    {
        base.OnEnable();
        
        action.Invoke();
        Destroy(gameObject);
    }
}
