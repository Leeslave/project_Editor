using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

public class MakeReport_Proof : MonoBehaviour
{
    public int Difficulty;
    public string Name;
    public TMP_Text ReportTop;
    public TMP_Text TextSample;
    public GameObject CardText;
    public GameObject WorkText;
    public GameObject Report;
    public GameObject CardProcess;
    public GameObject WorkProcess;
    public GameObject ChangeBT;
    

    GameObject WorkTextSub = null;
    ReportChangeButton ForRepChange;

    string[] Date;
    CultureInfo culture = new CultureInfo("Ko-KR");

    private void Start()
    {
        ForRepChange = ChangeBT.GetComponent<ReportChangeButton>();
        MakingReport_Proof();
    }

    void MakingReport_Proof()
    {
        string[] a = { "2020", "12", "18" };
        MakingReport_Proof(a);
    }

    void MakingReport_Proof(string[] _Date)
    {
        Date = _Date;
        ReportTop.text =  $" 제국력 {Date[0]}년 {Date[1]}월 {Date[2]}일\n";
        ReportTop.text += $" 진술자 : {Name}\n";
        ReportTop.text += " 진술 내용 :\n";
        List<Dictionary<string, object>> Data = CSVReader.Read($"Csv/Report_Proof/{Name}/{Date[1] + Date[2]}");
        
        foreach(var a in Data)
        {
            MakingText(a);
            MakingProof(a);
        }

    }

    void MakingText(Dictionary<string, object> Data)
    {
        TMP_Text CntText = Instantiate(TextSample);
        CntText.transform.SetParent(Report.transform);
        CntText.name = "";
        ReportText tmp = CntText.GetComponent<ReportText>();
        tmp.Time = Data["Time"].ToString();
        ForRepChange.TimeList.Add(tmp.Time);
        tmp.Place = Data["Place"].ToString();
        tmp.Action = Data["Action"].ToString();
        tmp.ChangeText();
        CntText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    void MakingProof(Dictionary<string, object> Data)
    {
        switch (Data["Type"])
        {
            case "Card":
                Making_Card(Data);
                break;
            case "Work":
                Making_Work(Data["Time"].ToString());
                break;
        }
    }

    void Making_Card(Dictionary<string, object> Data)
    {
        GameObject CntText = Instantiate(CardText);
        CntText.transform.SetParent(CardProcess.transform);
        CntText.transform.position = new Vector3(CntText.transform.position.x + MyUi.UISize(CntText).x,CntText.transform.position.y,CntText.transform.position.z);
        CntText.name = "";
        CntText.transform.GetChild(0).GetComponent<TMP_Text>().text = Data["Time"].ToString();
        CntText.transform.GetChild(1).GetComponent<TMP_Text>().text = Data["Place"].ToString();
        CntText.transform.GetChild(2).GetComponent<TMP_Text>().text = Data["Bill"].ToString();
        CntText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    void Making_Work(string Time)
    {
        if (WorkTextSub == null)
        {
            WorkTextSub = Instantiate(WorkText);
            WorkTextSub.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{Date[1]}/{Date[2]}";
            WorkTextSub.GetComponent<WorkText>().ChangeText(Time);
            int CurMonth = MyUi.StringToInt(Date[1]);
            int CurDay = MyUi.StringToInt(Date[2]);
            int EndOfMonth = 32; if ((CurMonth % 7) % 2 == 0) EndOfMonth = 31; if (CurMonth == 2) EndOfMonth = 29;
            int MakingTextNum = Random.Range(CurDay, EndOfMonth);

            int[][] TimeCnt = { new int[] { 700, 759 }, new int[] { 1140, 1150 }, new int[] { 1200, 1259 }, new int[] { 1800, 1815 } };

            for (int i = 1; i < MakingTextNum; i++)
            {
                if (CurDay == i) WorkTextSub.transform.SetParent(WorkProcess.transform);
                else
                {
                    string DayCnt = Convert.ToDateTime($"{Date[1]}/{i}/{Date[0]}").ToString("ddd");
                    if (DayCnt == "토" || DayCnt == "일") continue;
                    GameObject CntText = Instantiate(WorkText, WorkProcess.transform);
                    CntText.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = $"{Date[1]}/{i}";
                    int tmp = 0;
                    for (int x = 1; x <= 4; x++)
                    {
                        int CCC = Random.Range(TimeCnt[x-1][0], TimeCnt[x-1][1]);
                        CntText.transform.GetChild(x).gameObject.GetComponent<TMP_Text>().text = (CCC / 100).ToString() + ":" + ((CCC % 100).ToString()).PadLeft(2,'0');
                        if (x == 4) tmp = CCC;
                    }
                    if (CurDay != i - 1)
                    {
                        if (Random.Range(0, 4) == 2)
                        {
                            CntText.transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = (tmp / 100).ToString() + ":" + (tmp % 100).ToString().PadLeft(2, '0'); ;
                            CntText.transform.GetChild(6).gameObject.GetComponent<TMP_Text>().text = (tmp / 100 + Random.Range(2, 7)).ToString() + ":" + (tmp % 100).ToString().PadLeft(2, '0');
                        }
                    }
                    CntText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            WorkTextSub.GetComponent<WorkText>().ChangeText(Time);
            WorkTextSub.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    int StrTmToIntTm(string a)
    {
        string[] cnt = a.Split(":");
        return MyUi.StringToInt(cnt[0]) * 100 + MyUi.StringToInt(cnt[1]);
    }

    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }
}
