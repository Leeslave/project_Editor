using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ReportText : MonoBehaviour
{
    ContrastManager CM;

    public string Time;
    public string Place;
    public string Action;
    public string ErrorType = "";

    public void ChangeText()
    {
        gameObject.GetComponent<TMP_Text>().text = $"{Time} {Place}¿¡¼­ {Action}";
    }

    private void Start()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        if (tag == "ReportText") CM = transform.parent.GetComponent<MakingReport>().CM;
        else CM = transform.GetChild(0).GetComponent<MakingReport>().CM;

        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerClick, Click);
    }

    void Click(PointerEventData data)
    {
        if (data.pointerId == -2) CM.Option.GetComponent<OptionManager>().OptionInit(gameObject);
        else CM.ReportClick(gameObject);
    }
}
