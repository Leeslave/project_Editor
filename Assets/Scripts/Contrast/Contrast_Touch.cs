using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Contrast_Touch : MonoBehaviour
{
    //-------------------Variable-------------------------------
    //Unity
    public GameObject ReportProcess;
    public GameObject CardProcess;
    public GameObject WorkProcess;
    public GameObject MapProcess;
    public GameObject Fog;

    GraphicRaycaster gr;
    GameObject CurText = null;
    GameObject CurCardText = null;
    GameObject CurWorkText = null;
    GameObject CurBar = null;
    Vector3 AnchorGap;

    //ForTest
    GameObject a1 = null;
    void LineTest(GameObject cnt)
    {
        if (a1 == null) a1 = cnt;
        else
        {
            GetComponent<MakeLine>().DrawDotLine(a1, cnt);
            a1 = null;
        }
    }


    //STL
    Dictionary<String, GameObject> ProcessList;

    //ETC
    const float DoubleLag = 0.5f;
    bool Draging = false;
    public bool TouchAble = true;
    bool JGError = false;
    bool JGDouble = false;

    //Func

    public void ChangeJGError()
    {
        Fog.SetActive(!JGError);
        JGError = !JGError;
    }
    public void ChangeTouchAble() { TouchAble = TouchAble == false; }
    void DoubleCheck() { JGDouble = false; }
    void DragSetting(GameObject cnt) { Draging = true; CurBar = cnt; AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - cnt.transform.position; }
    //-------------------------------------------------------------

    private void Start()
    {
        gr = GetComponent<GraphicRaycaster>();
        ProcessList = new Dictionary<String, GameObject>()
        { 
            { "Card",CardProcess } ,
            { "Work",WorkProcess },
            { "Map", MapProcess }
        };
    }

    private void Update()
    {
        if (!TouchAble) return;
        if (Input.GetMouseButtonDown(0))
        {
            GameObject ClickedObject = MyUi.GRay(gr); if (ClickedObject == null) return;
            /*LineTest(ClickedObject);*/

            if (JGError) JudgeTouchOption(ClickedObject);
            else EtcTouchOption(ClickedObject);

            JGDouble = true; Invoke("DoubleCheck",DoubleLag);
        }
        if (Input.GetMouseButtonUp(0)) Draging = false;
        if (Draging) MyUi.DragUI(CurBar,AnchorGap);
    }



    void JudgeTouchOption(GameObject _ClickedObject)
    {
        switch (_ClickedObject.tag)
        {
            case "ReportText":
                if (CurText != null) CurText.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 1);
                _ClickedObject.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
                CurText = _ClickedObject;
                LineTest(CurText);
                break;
            case "CardText":
                if (CurCardText != null) ChangeAllChild(CurCardText.transform, Color.black);
                CurCardText = _ClickedObject.transform.parent.gameObject;
                ChangeAllChild(CurCardText.transform, Color.red);
                LineTest(CurCardText);
                break;
            case "WorkText":
                if (CurWorkText != null) ChangeAllChild(CurWorkText.transform, Color.black);
                CurWorkText = _ClickedObject.transform.parent.gameObject;
                ChangeAllChild(CurWorkText.transform, Color.red);
                LineTest(CurWorkText);
                break;
        }
    }

    void ChangeAllChild(Transform cnt, Color ChangeColor)
    {
        for (int i = 0; i < cnt.childCount; i++) cnt.GetChild(i).GetComponent<TMP_Text>().color = ChangeColor;
    }

    void EtcTouchOption(GameObject _ClickedObject)
    {
        switch (_ClickedObject.tag)
        {
            case "Icon":
                if (JGDouble) OpenProcess(_ClickedObject);
                break;
            case "ProcessBar":
                DragSetting(_ClickedObject.transform.parent.gameObject);
                break;
            case "ExitButton":
                _ClickedObject.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    void OpenProcess(GameObject _ClickedObject)
    {
        ProcessList[_ClickedObject.name].SetActive(true);
        ProcessList[_ClickedObject.name].transform.position = _ClickedObject.transform.position;
    }


}
