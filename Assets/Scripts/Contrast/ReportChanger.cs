using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportChanger : MonoBehaviour
{
    public List<string> TimeList = new List<string>();
    public GameObject ReportText;


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

    public void InsertNewReport(List<string> Data)
    {
        int ind = FindInsertInd(Data[0]);
        if (ind == -1) { Debug.Log("ºÒ°¡´É"); return; }
        else
        {
            GameObject cnt = Instantiate(ReportText, transform);
            ReportText cntt = cnt.GetComponent<ReportText>();
            cnt.transform.SetSiblingIndex(ind+1);
            cntt.Time = Data[0]; cntt.Place = Data[1]; cntt.Action = Data[2];
            cntt.ChangeText();
        }
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
