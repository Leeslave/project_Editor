using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    Color BfColor = new Color(0.8f,0.8f,0.8f,1);
    public Color AfColor;
    OptionManager Manager;
    public bool TouchAble = false;
    public Image CurColor;

    void Start()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, ClickPointer);
    }

    void OnPointer(PointerEventData data)
    {
        if(TouchAble)CurColor.color = AfColor;
    }
    void OutPointer(PointerEventData data)
    {
        if(TouchAble)CurColor.color = BfColor;
    }
    void ClickPointer(PointerEventData data)
    {
        if (!TouchAble) return;
        GetComponent<Image>().color = BfColor;
        switch (name)
        {
            case "Edit":
                Debug.Log("±â´Þ·Á");
                break;
            case "Delete":
                Manager.ChangedList.Push(new Tuple<GameObject, string>(Manager.CurObject,""));
                Manager.CurObject.SetActive(false);
                break;
            case "RollBack":
                Tuple<GameObject,string> cnt = Manager.ChangedList.Pop();
                if (cnt.Item2 != "") cnt.Item1.GetComponent<TMP_Text>().text = cnt.Item2;
                else cnt.Item1.SetActive(true);
                break;
        }
        transform.parent.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Manager = transform.parent.GetComponent<OptionManager>();
        CurColor = GetComponent<Image>();
        CurColor.color = AfColor;
    }
    private void OnDisable()
    {
        TouchAble = false;
    }
}
