using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetOptionFile_D : BatchField_D
{
    [SerializeField] DB_M DB;

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
    string[] Waittexts =
        {
            "Decoding File...",
            "Identifying File Type...",
            "Collecting Information...",
            "Configuring UI..."
        };

    [SerializeField] GameObject GrandParrent;
    void Start()
    {
        Mains = new List<TMP_Text>(MainsBack.Count);
        foreach(GameObject s in MainsBack) Mains.Add(s.transform.GetChild(0).GetComponent<TMP_Text>());
        GrandParrent.SetActive(false);
    }
    // Manipulation
    protected override IEnumerator BatchType1()
    {
        IC.PeopleName = AN.IconName;

        CommonBatch();
        image.SetActive(false);
        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        foreach (string s in Waittexts)
        {
            cnt += s;
            
            for (int i = 0; i <= 10; i++)
            {
                Text.text = cnt + $"<size=20>{i * 10}% </size>";
                yield return wfs;
            }
            cnt += " <size=20>Complete!\n</size>";
        }
        Text.text = cnt + "\n\nEnd!\n\n Wait a little...";
        yield return new WaitForSeconds(1);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;

        Processes[0].SetActive(true);
        Processes[0].transform.SetAsLastSibling();

        Tabs[1].gameObject.SetActive(true);
        TabsText[1].text = "Change Option";
        Tabs[1].Subs.AddRange(MPSub);
    }


    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Date;
    [SerializeField] TMP_Text Reporter;
    List<TMP_Text> Mains;
    [SerializeField] List<GameObject> MainsBack;
    
    // News
    protected override IEnumerator BatchType2()
    {
        CommonBatch();
        image.SetActive(false);
        News CurNews = DB.FindNews(AN.IconName);
        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        foreach (string s in Waittexts)
        {
            cnt += s;

            for (int i = 0; i <= 10; i++)
            {
                Text.text = cnt + $"<size=20>{i * 10}% </size>";
                yield return wfs;
            }
            cnt += " <size=20>Complete!\n</size>";
        }
        Text.text = cnt + "\n\nEnd!\n\n Wait a little...";
        yield return new WaitForSeconds(1f);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;

        Title.text = CurNews.Title;
        Date.text = CurNews.Date;
        Reporter.text = CurNews.Reporter;
        for(int i = 0; i < CurNews.CountM; i++)
        {
            Mains[i].text = CurNews.Main[i];
            MainsBack[i].SetActive(true);
        }

        Processes[1].SetActive(true);
        Processes[1].transform.SetAsLastSibling();
        Processes[1].transform.position = Vector3.zero;
        Processes[2].SetActive(true);
        Processes[2].transform.SetAsLastSibling();
        Processes[2].transform.position = Vector3.zero;

        Tabs[1].gameObject.SetActive(true);
        TabsText[1].text = "Add Text";
        Tabs[1].Subs.AddRange(NewsSub);
    }
    protected override IEnumerator BatchType3()
    {
        return base.BatchType3();
    }
    protected override IEnumerator BatchType4()
    {
        return base.BatchType4();
    }

    // 아마 이걸 
    protected override IEnumerator BatchETC()
    {
        return base.BatchETC();
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
