using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Icon : MonoBehaviour
{
    public GameObject contrastmanager;
    ContrastManager CM;
    public GameObject Process;

    bool DoubleClick = false;

    private void Start()
    {
        CM = contrastmanager.GetComponent<ContrastManager>();
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerClick, Click);
    }

    void Click(PointerEventData Data)
    {
        if (Data.pointerId == -2) return;
        if (CM.JudgeTime) return;
        CM.InputCall();
        if (DoubleClick)
        {
            if (!Process.activeSelf) Process.SetActive(true);

            Process.transform.SetAsLastSibling();
            Process.transform.position = transform.position;
        }
        else
        {
            DoubleClick = true;
            Invoke("DoubleCheck", 0.5f);
        }
    }
    void DoubleCheck() { DoubleClick = false; }
}
