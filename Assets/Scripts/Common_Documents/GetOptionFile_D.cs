using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetOptionFile_D : BatchField_D
{

    [SerializeField] GameObject ToDoList;

    [SerializeField] TMP_Text Text;

    [SerializeField] public Tabs_D[] Tabs;
    [SerializeField] TMP_Text[] TabsText;
    [SerializeField] GameObject[] Processes;
    [SerializeField] GameObject Folders;
    [SerializeField] GameObject image;


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
        bool Go = false;
        foreach (var k in DB_M.DB_Docs.PersonDataManager.PeopleCorrect) if (k.Item1 == DB_M.DB_Docs.CntFileForAttach.IconName) Go = true;

        // 금일 ToDoList에 있는 인물에만 접근 가능
        if (Go)
        {
            DB_M.DB_Docs.PersonDataManager.PeopleName = DB_M.DB_Docs.CntFileForAttach.IconName;
            CurType = 1;
            CommonBatch();
            image.SetActive(false);
            string cnt = "";

            // Loading 연출 시작
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

            // Loading 연출 끝
            Text.text = Normal;
            image.SetActive(true);
            AttatchAble = true;

            // 인물 정보 수정용 Process 활성화
            Processes[0].SetActive(true);
            Processes[0].transform.SetAsLastSibling();
            Processes[0].transform.position = Vector3.zero;
        }
        else
        {
            Text.text = "<size=40><color=#FF0000>401 Not Unauthorized</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
            AttatchAble = true;
        }
    }


    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Date;
    [SerializeField] TMP_Text Reporter;

    // News
    protected override IEnumerator BatchType2()
    {
        CommonBatch();
        CurType = 2;
        image.SetActive(false);
        News CurNews = DB_M.DB_Docs.FindNews(DB_M.DB_Docs.CntFileForAttach.IconName);
        DB_M.DB_Docs.NewsManager.CurNews = CurNews;
        string cnt = "";

        // Loading 연출 시작
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

        // Loading 연출 끝
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;

        // News에 Text 추가
        Title.text = CurNews.Title;
        Date.text = CurNews.Date;
        Reporter.text = CurNews.Reporter;
        for (int i = 0; i < CurNews.Main.Count; i++)
        {
            DB_M.DB_Docs.NewsManager.ActiveText(CurNews.Main[i]);
        }

        // News 수정용 Process 활성화
        Processes[1].SetActive(true);
        Processes[1].transform.SetAsLastSibling();
        Processes[1].transform.position = Vector3.zero;
    }

    // Docs
    [SerializeField] TextMannager_D Docs_Record;            // 문서_녹취록
    [SerializeField] TextMannager_D Docs_Act;               // 문서_행동 목록
    [SerializeField] TMP_Text Recorder;
    [SerializeField] TMP_Text Subject;

    protected override IEnumerator BatchType3()
    {
        CommonBatch();
        CurType = 2;
        image.SetActive(false);
        Docs CurDocs = DB_M.DB_Docs.FindDocs(DB_M.DB_Docs.CntFileForAttach.IconName);
        string cnt = "";

        // Loading 연출 시작
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

        // Loading 연출 끝
        Text.text = Normal;
        image.SetActive(true);
        AttatchAble = true;

        // 문서 대조에 사용될 문서 생성
        Recorder.text = $"Recorder : {CurDocs.Recorder}";
        Subject.text = $"Subject : {CurDocs.Subject}";
        int RC = 0;         // 현재까지 Docs_Record에 기록된 녹취자의 Text수를 새기 위해 사용
        int SC = 0;         // 현재까지 Docs_Record에 기록된 대상자의 Text수를 새기 위해 사용

        /*
         * 현재 줄이 CurDocs의 RecorderTextInd[RC]와 동일하면 녹취자의 Text로 기록(왼쪽, 초록색, 상호작용 불가)
         * 위의 조건에 해당하지 않으면 대상자의 Text로 기록(오른쪽, 빨간색, 상호작용 가능)
         */
        for (int i = 0; i < CurDocs.RecorderTexts.Count + CurDocs.SubjectTexts.Count; i++)
        {
            if (RC < CurDocs.RecorderTexts.Count)
            {
                if (CurDocs.RecorderTextInd[RC] == i) Docs_Record.AddText(CurDocs.RecorderTexts[RC++], new Color(0, 0.5f, 0, 1), IsTouchAble: false);
                else Docs_Record.AddText(CurDocs.SubjectTexts[SC++], new Color(0.5f, 0, 0), TextAlignmentOptions.Right);
            }
            else Docs_Record.AddText(CurDocs.SubjectTexts[SC++], new Color(0.5f, 0, 0), TextAlignmentOptions.Right);
        }
        foreach (var k in CurDocs.Time_Action) Docs_Act.AddText(k, Color.black);

        // 정답 Index 저장
        Docs_Record.MyAns = CurDocs.SubjectAns[0];
        Docs_Act.MyAns = CurDocs.ActionAns[0];

        // 문서 수정용 Process 활성화
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

    protected override IEnumerator BatchETC()
    {
        return base.BatchETC();
    }

    public void CommonBatch()
    {
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
