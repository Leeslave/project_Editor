using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainText_Back : Buttons_M
{

    [SerializeField] TextMannager_N TMN;
    [SerializeField] TextMannager_D TMD;
    GameObject Field;
    [SerializeField] GameObject Text;
    [SerializeField] Color AfColor;
    [SerializeField] int MyInd;
    

    protected override void Awake()
    {
        base.Awake();
        Field = transform.GetChild(0).gameObject;
        MyInd = transform.GetSiblingIndex() - 4;
    }

    protected override void Click(PointerEventData Data)
    {
        Field.SetActive(true);
        image.color = BfColor;
        Text.SetActive(false);
        if (TMN != null) TMN.OpenText(MyInd);
        //else
    }
    protected override void OnPointer(PointerEventData data)
    {
        if (Field.activeSelf) return;
        image.color = AfColor;
    }
    protected override void OutPointer(PointerEventData data)
    {
        image.color = BfColor;
    }
}
