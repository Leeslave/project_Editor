
using GameAction;
using UnityEngine.EventSystems;

public class ActionButton : WorldObject, IPointerClickHandler
{
    public string actionName;
    public string actionParam;
    private IGameAction _action;
    public bool loop;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        
        _action = ActionHandler.Create(actionName, actionParam);
    }

    public override void OnBecameVisible()
    {
        
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_action?.Invoke() ?? false)
        {
            if (!loop)
            {
                Destroy(gameObject);
            }
        }
    }
}
    
