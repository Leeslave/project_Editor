using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GetOptionFile_D : BatchField_D
{

    [SerializeField] GameObject ToDoList;

    [FormerlySerializedAs("Text")]
    [SerializeField] private TMP_Text statusText;

    [SerializeField] public Tabs_D[] Tabs;
    [SerializeField] TMP_Text[] TabsText;
    [SerializeField] GameObject[] Processes;
    [SerializeField] GameObject Folders;
    [SerializeField] GameObject image;


    [NonSerialized] private int currentTabIndex;
    [NonSerialized] public int CurType = 0;
    private const string IdleMessage = "\n\n\n\nDrag Option File Here!";
    private const string ErrorMessage = "<size=40><color=#FF0000>404 Not Found</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
    private readonly string[] loadingSteps =
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

    private const float LoadingStepDelay = 0.01f;
    private const float LoadingFinishDelay = 0.1f;
    // Manipulation
    protected override IEnumerator BatchType1()
    {
        bool isAllowed = false;
        foreach (var person in DB_M.DB_Docs.PersonDataManager.PeopleCorrect)
        {
            if (person.Item1 == DB_M.DB_Docs.CntFileForAttach.IconName)
                isAllowed = true;
        }

        // 금일 ToDoList에 있는 인물에만 접근 가능
        if (isAllowed)
        {
            DB_M.DB_Docs.PersonDataManager.PeopleName = DB_M.DB_Docs.CntFileForAttach.IconName;
            CurType = 1;
            CommonBatch();
            image.SetActive(false);
            yield return ShowLoading();

            // 인물 정보 수정용 Process 활성화
            ActivateProcess(0);
        }
        else
        {
            statusText.text = "<size=40><color=#FF0000>401 Not Unauthorized</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
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
        yield return ShowLoading();

        // News에 Text 추가
        Title.text = CurNews.Title;
        Date.text = CurNews.Date;
        Reporter.text = CurNews.Reporter;
        for (int i = 0; i < CurNews.Main.Count; i++)
        {
            DB_M.DB_Docs.NewsManager.ActiveText(CurNews.Main[i]);
        }

        // News 수정용 Process 활성화
        ActivateProcess(1);
    }

    // Docs
    [SerializeField] TextMannager_D Docs_Record;            // 문서_녹취록
    [SerializeField] TextMannager_D Docs_Act;               // 문서_행동 목록
    [SerializeField] TMP_Text Recorder;
    [SerializeField] TMP_Text Subject;

    protected override IEnumerator BatchType3()
    {
        bool isAllowed = false;
        foreach (var task in DB_M.DB_Docs.ToDoList.ToDoIndexes[1])
        {
            if (task.line1 != DB_M.DB_Docs.CntFileForAttach.IconName) continue;
            isAllowed = true;
            break;
        }

        if (isAllowed)
        {
            Docs CurDocs = DB_M.DB_Docs.FindDocs(DB_M.DB_Docs.CntFileForAttach.IconName);
            if (CurDocs.IsDone)
            {
                statusText.text = "<size=40><color=#FF0000>Already handled task</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Other Option File Here!";
                AttatchAble = true;
            }
            else
            {
                CommonBatch();
                CurType = 2;
                image.SetActive(false);


                yield return ShowLoading();

                PopulateDocumentComparison(CurDocs);

                // 문서 수정용 Process 활성화
                ActivateProcess(2);
                ActivateProcess(3);
                ActivateProcess(4);
            }
        }
        else
        {
            statusText.text = "<size=40><color=#FF0000>401 Not Unauthorized</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
            AttatchAble = true;
        }
    }

    private IEnumerator ShowLoading()
    {
        string completedSteps = "";
        WaitForSeconds stepDelay = new WaitForSeconds(LoadingStepDelay);
        foreach (string step in loadingSteps)
        {
            completedSteps += step;
            for (int progress = 0; progress <= 10; progress++)
            {
                statusText.text = completedSteps + $"<size=20>{progress * 10}% </size>";
                yield return stepDelay;
            }
            completedSteps += " <size=20>Complete!\n</size>";
        }
        statusText.text = completedSteps + "\n\nEnd!\n\n Wait a little...";
        yield return new WaitForSeconds(LoadingFinishDelay);

        statusText.text = IdleMessage;
        image.SetActive(true);
        AttatchAble = true;
    }

    private void ActivateProcess(int index)
    {
        GameObject process = Processes[index];
        process.SetActive(true);
        process.transform.SetAsLastSibling();
        process.transform.position = Vector3.zero;
    }

    private void PopulateDocumentComparison(Docs document)
    {
        Recorder.text = $"Recorder : {document.Recorder}";
        Subject.text = $"Subject : {document.Subject}";
        int recorderIndex = 0;
        int subjectIndex = 0;

        for (int lineIndex = 0; lineIndex < document.RecorderTexts.Count + document.SubjectTexts.Count; lineIndex++)
        {
            if (recorderIndex < document.RecorderTexts.Count && document.RecorderTextInd[recorderIndex] == lineIndex)
            {
                Docs_Record.AddText(document.RecorderTexts[recorderIndex++], new Color(0, 0.5f, 0, 1),
                    IsTouchAble: false);
            }
            else
            {
                Docs_Record.AddText(document.SubjectTexts[subjectIndex++], new Color(0.5f, 0, 0),
                    TextAlignmentOptions.Right);
            }
        }
        foreach (string action in document.Time_Action)
            Docs_Act.AddText(action, Color.black);

        Docs_Record.MyAns = document.SubjectAns[0];
        Docs_Act.MyAns = document.ActionAns[0];
        Docs_Record.CurDocs = document;
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
        statusText.text = IdleMessage;
        image.SetActive(true);
    }

    protected override void BatchFail()
    {
        statusText.text = ErrorMessage;
    }

    public void ChangeTab(int index)
    {
        Tabs[currentTabIndex].CloseTab();
        currentTabIndex = index;
    }

    private void OnDisable()
    {
        AttatchAble = true;
    }
}
