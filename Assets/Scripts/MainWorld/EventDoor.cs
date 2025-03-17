using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : Door
{
    public string actionType;
    public string actionParam;
    private Action _action = null;

    public override void OnClick()
    {
        base.OnClick();
        
        // 반응 무시 아니면 지역 이동
        if (Interactable is false)
        {
            return;
        }
        
        // 반응 획득
        _action = ActionHandler.GetAction(actionType, actionParam);
        _action.Invoke();
    }
}
