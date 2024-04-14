using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetOptionFile_D : BatchField_D
{
    [SerializeField] DB_M DB;

    [SerializeField] GameObject ToDoList;

    [SerializeField] TMP_Text Text;
    
    [SerializeField] public Tabs_D[] Tabs;
    [SerializeField] TMP_Text[] TabsText;
    [SerializeField] GameObject[] Processes;
    [SerializeField] GameObject Folders;
    [SerializeField] GameObject image;

    [SerializeField] public InfChange IC;

    [NonSerialized] int CurOpen = 0;
    [NonSerialized] public int CurType = 0;
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
        GrandParrent.SetActive(false);
    }

    float LoadingTime1 = 0.01f;
    float LoadingTime2 = 0.1f;
    // Manipulation
    protected override IEnumerator BatchType1()
    {
        IC.PeopleName = AN.IconName;
        CurType = 1;
        CommonBatch();
        image.SetActive(false);
        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(LoadingTime1);
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
        yield return new WaitForSeconds(LoadingTime2);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;

        Processes[0].SetActive(true);
        Processes[0].transform.SetAsLastSibling();

        Tabs[2].gameObject.SetActive(true);
    }


    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Date;
    [SerializeField] TMP_Text Reporter;
    [SerializeField] TextMannager_N TMN;
    
    // News
    protected override IEnumerator BatchType2()
    {
        CommonBatch();
        CurType = 2;
        image.SetActive(false);
        News CurNews = DB.FindNews(AN.IconName);
        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(LoadingTime1);
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
        yield return new WaitForSeconds(LoadingTime2);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;
        Title.text = CurNews.Title;
        Date.text = CurNews.Date;
        Reporter.text = CurNews.Reporter;
        for (int i = 0; i < CurNews.Main.Length; i++)
        {
            TMN.ActiveText(CurNews.Main[i]);
        }
        TMN.AnsNews = CurNews;

        Processes[1].SetActive(true);
        Processes[1].transform.SetAsLastSibling();
        Processes[1].transform.position = Vector3.zero;
    }

    // Docs
    [SerializeField] TextMannager_D Docs_Record;
    [SerializeField] TextMannager_D Docs_Act;
    [SerializeField] TMP_Text Recorder;
    [SerializeField] TMP_Text Subject;

    protected override IEnumerator BatchType3()
    {
        CommonBatch();
        CurType = 2;
        image.SetActive(false);
        Docs CurDocs = DB.FindDocs(AN.IconName);
        string cnt = "";
        WaitForSeconds wfs = new WaitForSeconds(LoadingTime1);
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
        yield return new WaitForSeconds(LoadingTime2);
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;
        Recorder.text = $"Recorder : {CurDocs.Recorder}";
        Subject.text = $"Subject : {CurDocs.Subject}";

        int RC = 0;
        int SC = 0;
        for(int i = 0; i < CurDocs.RecorderTexts.Count + CurDocs.SubjectTexts.Count; i++)
        {
            if (RC < CurDocs.RecorderTexts.Count) 
            {
                if (CurDocs.RecorderTextInd[RC] == i) Docs_Record.AddText(CurDocs.RecorderTexts[RC++], new Color(0, 0.5f, 0, 1), IsTouchAble: false);
                else Docs_Record.AddText(CurDocs.SubjectTexts[SC++], new Color(0.5f, 0, 0),TextAlignmentOptions.Right);
            }
            else Docs_Record.AddText(CurDocs.SubjectTexts[SC++], new Color(0.5f, 0, 0), TextAlignmentOptions.Right);
        }
        foreach (var k in CurDocs.Time_Action) Docs_Act.AddText(k, Color.black);


        Docs_Record.MyAns = CurDocs.SubjectAns[0];
        Docs_Act.MyAns = CurDocs.ActionAns[0];

        Processes[2].SetActive(true);
        Processes[2].transform.SetAsLastSibling();
        Processes[2].transform.position = Vector3.zero;

        Processes[3].SetActive(true);
        Processes[3].transform.SetAsLastSibling();
        Processes[3].transform.position = Vector3.zero;

        Processes[4].SetActive(true);
        Processes[4].transform.SetAsLastSibling();
        Processes[4].transform.position = Vector3.zero;
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
        if (Tabs[2].gameObject.activeSelf)
        {
            IC.CloseFolder();
            Tabs[2].gameObject.SetActive(false);
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

    private void OnDisable()
    {
        AttatchAble = true;
    }
}
