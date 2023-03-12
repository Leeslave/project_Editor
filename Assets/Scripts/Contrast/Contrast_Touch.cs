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

    GraphicRaycaster gr;
    GameObject CurText = null;
    GameObject CurCardText = null;
    GameObject CurBar = null;
    Vector3 AnchorGap;

    //ForTest
    /*GameObject a1 = null;
    GameObject a2 = null;
    void LineTest(GameObject cnt)
    {
        if (a1 == null) a1 = cnt;
        else
        {
            GetComponent<MakeLine>().DrawDotLine(a1, cnt);
            a1 = null;
        }
    }
    */

    //STL
    Dictionary<String, GameObject> ProcessList;

    //ETC
    const float DoubleLag = 0.5f;
    bool Draging = false;
    bool TouchAble = true;
    bool JGError = false;
    bool JGDouble = false;

    //Func

    void DoubleCheck() { JGDouble = false; }
    void DragSetting(GameObject cnt) { Draging = true; CurBar = cnt; AnchorGap = Camera.main.ScreenToWorldPoint(Input.mousePosition) - cnt.transform.position; }
    //-------------------------------------------------------------

    private void Start()
    {
        gr = GetComponent<GraphicRaycaster>();
        ProcessList = new Dictionary<String, GameObject>()
        { 
            { "Card",CardProcess } ,
            { "Work",WorkProcess }
        };
    }

    private void Update()
    {
        if (!TouchAble)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<MakeLine>().EndLine();
                TouchAble = true;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameObject ClickedObject = MyUi.GRay(gr); if (ClickedObject == null) return;

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
                break;
            case "CardText":
                if (CurCardText != null) CurCardText.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 1);
                _ClickedObject.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
                CurCardText = _ClickedObject;
                break;
        }
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
            case "ErrorButton":
                JGError = true;
                break;
        }
    }

    void OpenProcess(GameObject _ClickedObject)
    {
        Debug.Log("!");
        ProcessList[_ClickedObject.name].SetActive(true);
        ProcessList[_ClickedObject.name].transform.position = _ClickedObject.transform.position;
    }


}
