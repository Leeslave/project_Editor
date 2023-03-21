using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class ReportChnageBt : MonoBehaviour
{
    public Color BfColor;
    public Color AfColor;
    public TMP_Text Time;
    public TMP_Text Place;
    public TMP_Text Action;
    public GameObject ForRepChange;
    bool highlighted = false;

    void Start()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, ClickPointer);
    }

    public void OnPointer(PointerEventData data)
    {
        if (!highlighted) GetComponent<Image>().color = AfColor;
    }
    public void OutPointer(PointerEventData data)
    {
        if (!highlighted) GetComponent<Image>().color = BfColor;
    }
    public void ClickPointer(PointerEventData data)
    {
        GetComponent<Image>().color = BfColor;
        ForRepChange.GetComponent<ReportChanger>().InsertNewReport(new List<string> {Time.text.Substring(0,Time.text.Length - 1),Place.text,Action.text} );
        transform.parent.gameObject.SetActive(false);
    }
}
