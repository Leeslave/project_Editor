using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetOptionFile_D : BatchField_D
{
    [SerializeField] TMP_Text Text;
    
    [SerializeField] Tabs_D[] Tabs;
    [SerializeField] TMP_Text[] TabsText;
    [SerializeField] GameObject[] Processes;
    [SerializeField] GameObject[] MPSub;
    [SerializeField] GameObject[] NewsSub;
    [SerializeField] GameObject image;

    [SerializeField] InfChange IC;

    [NonSerialized] int CurOpen = 0;
    string Normal = "\n\n\n\nDrag Option File Here!";
    string Error = "<size=40><color=#FF0000>404 Not Found</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";

    // Manipulation
    protected override IEnumerator BatchType1()
    {
        CommonBatch();
        image.SetActive(false);
        IC.PeopleName = AN.IconName;
        string[] texts =
        {
            "Decoding File...",
            "Identifying File Type...",
            "Collecting Information...",
            "Configuring UI..."
        };

        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        foreach (string s in texts)
        {
            cnt += s;
            
            for (int i = 0; i <= 10; i++)
            {
                Text.text = cnt + $"<size=20>{i * 10}% </size>";
                yield return wfs;
            }
            cnt += " <size=20>Complete!\n</size>";
        }
        Text.text = cnt + "End!\n Wait a little...";
        yield return new WaitForSeconds(1);
        Processes[0].SetActive(true);
        Processes[0].transform.SetAsLastSibling();
        Tabs[1].gameObject.SetActive(true);
        TabsText[1].text = "Change Option";
        Tabs[1].Subs.AddRange(MPSub);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;
    }

    // News
    protected override IEnumerator BatchType2()
    {
        return null;
    }
    protected override IEnumerator BatchType3()
    {
        return base.BatchType3();
    }

    public void CommonBatch()
    {
        if(CurOpen!=0) Tabs[0].OpenTab();
        for (int i = 1; i < Tabs.Length; i++)
        {
            Tabs[i].Subs.Clear();
            Tabs[i].gameObject.SetActive(false);
        }
        foreach (GameObject s in Processes) s.SetActive(false);
        Text.text = Normal;
        image.SetActive(true);
    }
    protected override void BatchFail()
    {
        Text.text = Error;
    }
    public void ChangeTab(int index)
    {
        Tabs[CurOpen].CloseTab();
        CurOpen = index;
    }
}
