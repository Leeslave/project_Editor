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
    public GameObject Option;
    public GameObject Fog;

    GraphicRaycaster gr;
    GameObject CurReportText = null;
    GameObject CurContrastText = null;
    GameObject CurBar = null;
    Vector3 AnchorGap;

    //ForTest
    void LineTest()
    {
        if (CurContrastText != null && CurReportText != null) GetComponent<MakeLine>().DrawDotLine(CurContrastText, CurReportText);
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
    void DragSetting(GameObject cnt) { Draging = true; CurBar = cnt; AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - cnt.transform.position; AnchorGap.z = 0; }
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
            if (ClickedObject.layer != 5) return;    // Not Need Ray -> Return

            Debug.Log(ClickedObject);
            if (JGError) JudgeTouchOption(ClickedObject);
            else EtcTouchOption(ClickedObject);

            JGDouble = true; Invoke("DoubleCheck",DoubleLag);
        }
        if (Input.GetMouseButtonDown(1))
        {
            GameObject ClickedObject = MyUi.GRay(gr); if (ClickedObject == null) return;
            if (Option.activeSelf) Option.SetActive(false);
            Option.SetActive(true);
            Option.GetComponent<OptionManager>().OptionInit(ClickedObject);
            Vector3 CurCamPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); CurCamPos.z = 0;
            Option.transform.position = CurCamPos;
        }
        if (Input.GetMouseButtonUp(0)) Draging = false;
        if (Draging) MyUi.DragUI(CurBar,AnchorGap);
    }



    void JudgeTouchOption(GameObject _ClickedObject)
    {
        if (_ClickedObject == CurReportText || _ClickedObject == CurContrastText) return;
        switch (_ClickedObject.tag)
        {
            case "ReportText":
                if (CurReportText != null) CurReportText.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 1);
                _ClickedObject.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
                CurReportText = _ClickedObject;
                LineTest();
                break;
            case "DistanceText":
                if (CurContrastText != null) ChangeContrastTextColor(CurContrastText);
                _ClickedObject.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
                CurContrastText = _ClickedObject;
                LineTest();
                break;
            case "CardText":
                JudgeSub(_ClickedObject);
                break;
            case "WorkText":
                JudgeSub(_ClickedObject);
                break;
        }
    }
    void JudgeSub(GameObject _ClickedObject)
    {
        if (CurContrastText != null) ChangeContrastTextColor(CurContrastText);
        CurContrastText = _ClickedObject.transform.parent.gameObject;
        ChangeAllChild(CurContrastText.transform, Color.red);
        LineTest();
    }

    void ChangeAllChild(Transform cnt, Color ChangeColor)
    {
        for (int i = 0; i < cnt.childCount; i++) cnt.GetChild(i).GetComponent<TMP_Text>().color = ChangeColor;
    }
    void ChangeContrastTextColor(GameObject cnt)
    {
        if (cnt.tag == "MapText") cnt.GetComponent<TMP_Text>().color = new Color(0.9254902f, 0.9019608f, 0.7372549f, 1);
        else ChangeAllChild(cnt.transform, Color.black);
    }

    void EtcTouchOption(GameObject _ClickedObject)
    {
        switch (_ClickedObject.tag)
        {
            case "Icon":
                if (JGDouble) OpenProcess(_ClickedObject);
                break;
            case "ProcessBar":
                GameObject CurParent = _ClickedObject.transform.parent.gameObject;
                if (_ClickedObject.name == "Top" && CurParent != CurBar) 
                    CurParent.transform.SetSiblingIndex(CurParent.transform.parent.childCount-3);
                DragSetting(CurParent);            
                break;
            case "ExitButton":
                _ClickedObject.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
    void ResetJudge()
    {
        CurReportText = null;
        CurContrastText = null;
        TouchAble = true;
        ChangeJGError();
    }

    void OpenProcess(GameObject _ClickedObject)
    {
        ProcessList[_ClickedObject.name].SetActive(true);
        ProcessList[_ClickedObject.name].transform.position = _ClickedObject.transform.position;
    }
}
