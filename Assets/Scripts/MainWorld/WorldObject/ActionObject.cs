
using UnityEngine;

public class ActionObject : WorldObject
{
    public string actionName;
    public string actionParam;
    private Action action = null;
    public bool loop = false;
    
    
    void Awake()
    {
        OnAwake();
    }
    
    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>Action 초기화</remarks>
    public override void OnAwake()
    {
        base.OnAwake();
        
        action = ActionHandler.GetAction(actionName, actionParam);
        Debug.Log(action);
    }

    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>액션 실행 및 삭제</remarks>
    public override void OnEnable()
    {
        base.OnEnable();
        
        action?.Invoke();
        if (!loop)
        {
            Destroy(gameObject);
        }
        
        Debug.Log($"Action Invoke! : {actionName}-{actionParam}");
    }
}
