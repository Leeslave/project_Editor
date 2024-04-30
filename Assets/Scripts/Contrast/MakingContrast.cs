using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MakingContrast : MonoBehaviour         // Manager 겸용
{
    public GameObject Contrast;

    public ContrastManager CM;

    public GameObject Text;

    GameObject ForWork = null;
    int CurIndex = 1;

    private void Start()
    {
        CM = Contrast.GetComponent<ContrastManager>();
    }

    public void Making(Dictionary<string, object> Data)         // Card
    {
        GameObject CntText = Instantiate(Text);
        CntText.transform.SetParent(transform);
        CntText.name = "";      // 혹시 모를 꼬임 방지

        CntText.transform.GetChild(0).GetComponent<TMP_Text>().text = Data["Time"].ToString();
        CntText.transform.GetChild(1).GetComponent<TMP_Text>().text = Data["Place"].ToString();
        CntText.transform.GetChild(2).GetComponent<TMP_Text>().text = Data["Bill"].ToString();

        CntText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);    // 계속 Scale 커지는 버그 방지
    }


    public void Making(string _Time,int[] date)      // Work
    {
        int CurTime;
        int lastday = UnityEngine.Random.Range(date[2],DateTime.DaysInMonth(date[0], date[1])+1);

        if (ForWork != null)
        {
            ForWork.transform.GetChild(CurIndex++).GetComponent<TMP_Text>().text = _Time;
        }
        else
        {
            for (int i = 1; i <= lastday; i++)
            {
                if (i == date[2])
                {
                    ForWork = Instantiate(Text, transform);
                    ForWork.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);    // 계속 Scale 커지는 버그 방지
                    ForWork.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{date[1]}/{date[2]}";
                    ForWork.transform.GetChild(CurIndex++).GetComponent<TMP_Text>().text = _Time;
                }
                else
                {
                    DateTime tmp = new DateTime(date[0], date[1], i);
                    if (tmp.DayOfWeek == DayOfWeek.Sunday || tmp.DayOfWeek == DayOfWeek.Saturday) if (UnityEngine.Random.Range(0, 3) < 2) continue;
                    GameObject CntText = Instantiate(Text, transform); 
                    CntText.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{date[1]}/{i.ToString().PadLeft(2,'0')}";
                    for (int x = 1; x <= 4; x++)
                    {
                        CurTime = UnityEngine.Random.Range(CM.WorkTime[x].Item1, CM.WorkTime[x].Item2);
                        CntText.transform.GetChild(x).GetComponent<TMP_Text>().text = $"{CurTime / 100}:{((CurTime % 100)%60).ToString().PadLeft(2,'0')}";
                    }
                    if (UnityEngine.Random.Range(0, 4) < 3) continue;
                    CurTime = UnityEngine.Random.Range(CM.WorkTime[5].Item1, CM.WorkTime[5].Item2);
                    CntText.transform.GetChild(5).GetComponent<TMP_Text>().text = $"{CurTime / 100}:{((CurTime % 100) % 60).ToString().PadLeft(2, '0')}";
                    CurTime = UnityEngine.Random.Range(CM.WorkTime[6].Item1, CM.WorkTime[6].Item2);
                    CntText.transform.GetChild(6).GetComponent<TMP_Text>().text = $"{CurTime / 100}:{((CurTime % 100) % 60).ToString().PadLeft(2, '0')}";
                }
            }
        }
    }
}
