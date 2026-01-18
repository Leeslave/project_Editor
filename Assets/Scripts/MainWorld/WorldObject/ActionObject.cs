
using UnityEngine;

public class ActionObject : WorldObject
{
    public string actionName;
    public string actionParam;
    private IGameAction _gameAction;
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
        
        _gameAction = ActionHandler.GetAction(actionName, actionParam);
        Debug.Log(_gameAction);
    }

    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>액션 실행 및 삭제</remarks>
    public override void OnEnable()
    {
        base.OnEnable();
        
        if(_gameAction?.Invoke() ?? false)
        {
            if (!loop)
            {
                Destroy(gameObject);
            }
        }
        
        Debug.Log($"Action Invoke! : {actionName}-{actionParam}");
    }
}
