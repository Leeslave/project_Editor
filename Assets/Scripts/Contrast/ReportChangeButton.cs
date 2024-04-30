using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using System;

public class ReportChangeButton : MonoBehaviour
{
    public Color BfColor;
    public Color AfColor;
    public TMP_Text TimeDrop;
    public TMP_Text Time;
    public TMP_Text Place;
    public TMP_Text Action;
    public GameObject Report;
    public GameObject ReportText;
    public GameObject ErrorLog;
    public GameObject Option;

    public List<string> TimeList = new List<string>();
    public bool IsCreate = true;

    Dictionary<string, int[]> AbleTime = new Dictionary<string, int[]>() 
    { {"출근 전", new int[] { 600, 659 } },
      {"출근",  new int[] { 700, 759 }} ,
      {"오전 업무", new int[] {800,1159 } },
      {"점심 시간", new int[] { 1200, 1259 } },
      {"업무 복귀",new int[]{ 1300,1305 } },
      {"오후 업무",new int[]{ 1305,1759 } },
      {"퇴근", new int[] {1800, 1805 } },
      {"퇴근 후", new int[] { 1805, 2200 } }
    };
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
        if (!JudgeTime()) return;
        if (IsCreate)
        {
            InsertNewReport();
        }
        else
        {
            
        }
        transform.parent.gameObject.SetActive(false);
    }
    public void InsertNewReport()
    {
        int ind = FindInsertInd(Time.text.Substring(0, Time.text.Length - 1));
        GameObject cnt = Instantiate(ReportText, Report.transform);
        ReportText cntt = cnt.GetComponent<ReportText>();
        cnt.transform.SetSiblingIndex(ind + 1);
        cntt.Time = Time.text.Substring(0, Time.text.Length - 1); cntt.Place = Place.text; cntt.Action = Action.text;
        cntt.ChangeText();
        Option.GetComponent<OptionManager>().ChangedList.Push(new Tuple<GameObject,string>(cnt,"Create"));
    }
    bool JudgeTime()
    {
        return true;
    }
    int FindInsertInd(string a)
    {
        int ind = 0;
        for (; ind < TimeList.Count; ind++)
        {
            int j = TimeCompare(TimeList[ind], a);
            if (j == 1) break;
            else if (j == 0) return -1;
        }
        return ind;
    }
    int TimeCompare(string a, string b)
    {
        int aa = MyUi.StringToInt(a);
        int bb = MyUi.StringToInt(b);
        if (aa > bb) return 1;
        else if (aa == bb) return 0;
        else return -1;
    }
}
