using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContrastText : MonoBehaviour
{
    ContrastManager CM;

    public string Time;
    public string Place;
    public string Action;
    public string ErrorType = "";


    private void Start()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        CM = transform.parent.GetComponent<MakingContrast>().CM;

        MyUi.AddEvent(eventTrigger,EventTriggerType.PointerClick,Click);
    }

    void Click(PointerEventData data)
    {
        CM.ContrastClick(gameObject);
    }
}
