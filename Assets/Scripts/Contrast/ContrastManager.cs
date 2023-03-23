using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContrastManager : MonoBehaviour
{
    public bool JudgeTime;
    public GameObject Option;
    public GameObject CurReport = null;
    public GameObject CurContrast = null;

    public GameObject Report;
    public GameObject Card;
    public GameObject Work;
    public GameObject Map;

    public List<Dictionary<string, object>> Data;

    public List<Tuple<int, int>> WorkTime = new List<Tuple<int, int>>
    {
        new Tuple<int,int>(600,659),
        new Tuple<int,int>(700,759),
        new Tuple<int,int>(1155,1159),
        new Tuple<int,int>(1255,1259),
        new Tuple<int,int>(1800,1805),
        new Tuple<int,int>(1900,1905),
        new Tuple<int,int>(2000,2400)
    };        // 출근 전, 출근, 점심 시작, 점심 끝, 퇴근, 잔업 시작, 잔업 끝 or 퇴근 후

    int[] Date;

    private void Start()
    {
        GetExternalData(new int[] { 2020, 12, 18 }, "People1");
        foreach(var a in Data)
        {
            Report.GetComponent<MakingReport>().Making(a);
            switch (a["Type"])
            {
                case "Card":
                    Card.GetComponent<MakingContrast>().Making(a);
                    break;
                case "Work":
                    Work.GetComponent<MakingContrast>().Making(a["Time"].ToString(),Date);
                    break;
                case "Map":
                    break;
            }
        }
    }

    public void GetExternalData(int[] _Date, string Name)
    {
        Date = _Date;
        Data = MyUi.CSVReader.Read($"Csv/Report_Proof/{Name}/{Date[1]}{Date[2]}");
    }

    public void InputCall()
    {
        OptionClose();
    }

    public void OptionClose() { if(Option.activeSelf)Option.SetActive(false); }
    public void ReportClick(GameObject Clicked)
    {
        InputCall();
        if (CurReport != null)
        {
            if (CurReport.name == "Report") CurReport.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 0);
            else CurReport.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 1);
        }
        if (Clicked.name == "Report") Clicked.GetComponent<Outline>().effectColor = new Color(1, 0, 0, 1);
        else Clicked.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
        CurReport = Clicked;

        if (CurContrast != null && CurReport != null) GetComponent<MakeLine>().DrawDotLine(CurContrast, CurReport);
    }
    public void ContrastClick(GameObject Clicked)
    {
        if (CurContrast != null) ChangeContrastTextColor(CurContrast);
        if (Clicked.tag == "DistanceText")
        {
            Clicked.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
            CurContrast = Clicked;
        }
        else
        {
            CurContrast = Clicked;
            ChangeAllChild(Clicked.transform, Color.red);
        }
        
        if (CurContrast != null && CurReport != null) GetComponent<MakeLine>().DrawDotLine(CurContrast, CurReport);
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
}
