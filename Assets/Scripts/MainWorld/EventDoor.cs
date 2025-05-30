using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : Door
{
    public string actionType;
    public string actionParam;
    private Action action = null;

    public override void OnClick()
    {
        // 반응 무시 아니면 지역 이동
        base.OnClick();
        
        // 반응 획득
        GetAction();
        action.Invoke();
    }

    private void GetAction()
    {
        switch(actionType)
        {
            case "HardDayChange" :
                action = new HardDayChangeAction();
                action.param = actionParam;
                break;
            case "DayChange" :
                action = new DayChangeAction();
                action.param = actionParam;
                break;
            case "TimeChange" :
                action = new TimeChangeAction();
                action.param = actionParam;
                break;

            default:
                action = null;
                break;
        }
    }
}
