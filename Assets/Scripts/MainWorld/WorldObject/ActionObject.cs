
using UnityEngine;
using GameAction;

public class ActionObject : WorldObject
{
    public string actionName;
    public string actionParam;
    private IGameAction _gameAction;
    public bool loop;
    
    void Awake()
    {
        Init();
    }
    
    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>Action 초기화</remarks>
    public override void Init()
    {
        base.Init();
        
        _gameAction = ActionHandler.Create(actionName, actionParam);
    }

    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>액션 실행 및 삭제</remarks>
    public override void OnBecameVisible()
    {
        if(_gameAction?.Invoke() ?? false)
        {
            if (!loop)
            {
                Destroy(gameObject);
            }
        }
    }
}
