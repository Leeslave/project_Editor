using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class MakeReport_Proof : MonoBehaviour
{
    public int Difficulty;
    public string Name;
    public TMP_Text ReportTop;
    public TMP_Text TextSample;
    public GameObject Report;
    public GameObject CardProcess;

    private void Awake()
    {
        MakingReport_Proof();
    }

    void MakingReport_Proof()
    {
        string[] a = { "1720", "12", "18" };
        MakingReport_Proof("People1", a);
    }

    void MakingReport_Proof(string Name, string[] Date)
    {
        ReportTop.text =  $" 제국력 {Date[0]}년 {Date[1]}월 {Date[2]}일\n";
        ReportTop.text += $" 진술자 : {name}\n";
        ReportTop.text += " 진술 내용 :\n";

        List<Dictionary<string, object>> Data = CSVReader.Read($"Csv/Report_Proof/{Name}/{Date[1] + Date[2]}");
        foreach(var a in Data)
        {
            MakingText(a);
        }

    }

    void MakingText(Dictionary<string, object> Data)
    {
        TMP_Text CntText = Instantiate(TextSample);
        CntText.transform.SetParent(Report.transform);
        CntText.name = "";
        ReportText tmp = CntText.GetComponent<ReportText>();
        tmp.Time = Data["Time"].ToString();
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
                break;
        }
    }

    void Making_Card(Dictionary<string, object> Data)
    {
        TMP_Text CntText = Instantiate(TextSample);
        CntText.transform.SetParent(CardProcess.transform);
        CntText.name = "";
        ReportText tmp = CntText.GetComponent<ReportText>();
        tmp.Time = Data["Time"].ToString();
        tmp.Place = Data["Place"].ToString();
        tmp.Action = Data["Action"].ToString();
        tmp.ChangeCard();
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
