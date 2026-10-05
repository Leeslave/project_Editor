using GameService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

// 해당 게임의 모든 I/O 입출력은 해당 코드를 통해 이루어짐
public class DB_M : MonoBehaviour
{
    private IWorkService WorkService => GameSystem.GetService<IWorkService>();
    
    [HideInInspector] public static DB_M DB_Docs;
    public AttatchFile_N CntFileForAttach;
    public ToDoList_N ToDoList;
    public InfChange PersonDataManager;
    public TextMannager_N NewsManager;
    public TextMannager_D Docs_Record;
    public GetOptionFile_D GetOption;


    [SerializeField] Windows_M DBFolder;
    [SerializeField] Windows_M NewsFolder;
    [SerializeField] Windows_M DocsFolder;
    [SerializeField] GameObject secretInfo;         // secretInfo 폴더
    [SerializeField] private GameObject normalInfo;
    [SerializeField] Sprite spr;                // 아이콘 생성(폴더 내 하위 오브젝트)에 사용되는 임시 스프라이트

    public int Month;
    public int Day;

    // ETC.cs의 인물 정보를 나타내는 열거형 정보를 string으로 변환하여 저장해 둠
    // 0 : 국,  1 ; 직, 2 : 부, 3 : 소
    [HideInInspector] public List<string[]> InfSub = new List<string[]>(4);

    public List<PeopleIndex> PeopleList;
    public News[] NewsList;
    public List<Instruction> InstructionList;
    
    // 현재 요일에 사용될 지시 사항
    [HideInInspector] public Instruction Instructions;

    public Docs[] DocsList;
    [SerializeField]private int stageInt = 0;

    private string clearGameName = "Document";

    /// <summary>
    /// 현재 문서 모드에 대응하는 완료 처리용 업무 코드
    /// </summary>
    public string WorkCode => clearGameName;
    void Start()
    {
        normalInfo.SetActive(false);
        secretInfo.SetActive(false);
        // 실행 정보 없이 직접 진입한 경우 일반 문서 업무로 처리
        if (WorkService.CurrentWorkCode != "SecureDocument")
        {
            // 일반 Document의 경우를 뜻함
            clearGameName = "Document";
            normalInfo.SetActive(true);
        }
        else // 명시적으로 선택된 극비 문서 업무
        {
            clearGameName = "SecureDocument";
            secretInfo.SetActive(true);
        }
        
        stageInt = WorkService.GetStage(clearGameName);
        if (DB_Docs != null) { Destroy(gameObject); return; }
        DB_Docs = this;

        InitializeInfoCategories();
        CreateFolderIcons();
        PrepareInstructions();

        if (stageInt == 1)
            StartCoroutine(TutoTest());
    }

    private void InitializeInfoCategories()
    {
        InfSub.Add(Enum.GetNames(typeof(Country)));
        InfSub.Add(Enum.GetNames(typeof(Job)));
        InfSub.Add(Enum.GetNames(typeof(Belonging)));
        InfSub.Add(Enum.GetNames(typeof(Part)));
    }

    private void CreateFolderIcons()
    {
        for (int i = 0; i < PeopleList.Count - 1; i++)
            DBFolder.NewIcon(PeopleList[i].name_e, spr, 1);
        DBFolder.gameObject.SetActive(false);

        for (int i = 0; i < NewsList.Length - 1; i++)
        {
            NewsFolder.NewIcon(NewsList[i].publishDay, spr, 2);
            string[] newsLines = NewsList[i].Main[0].Split('\n');
            if (newsLines.Length != 1) NewsList[i].Main = newsLines.ToList();
        }
        NewsFolder.gameObject.SetActive(false);

        foreach (Docs document in DocsList)
            DocsFolder.NewIcon(name: document.Subject, Image: spr, 3);
        DocsFolder.gameObject.SetActive(false);
    }

    private void PrepareInstructions()
    {
        foreach (Instruction instruction in InstructionList)
        {
            EditorLogger.Log("k.stageInt: " + instruction.stageInt);
            EditorLogger.Log("now stageInte: " + stageInt);
            if (instruction.stageInt == stageInt)
            {
                EditorLogger.Log("stageInt 대조");
                Instructions = instruction;
                PreparePeopleAnswers(instruction);
                PrepareNewsAnswers(instruction);
                break;
            }
        }
    }

    private void PreparePeopleAnswers(Instruction instruction)
    {
        var targetNames = new List<string>();
        foreach (var edit in instruction.InfoInst)
        {
            PeopleIndex person = new PeopleIndex();
            if (!targetNames.Contains(edit.Target))
            {
                person = new PeopleIndex(FindPeople(edit.Target));
                instruction.Peoples.Add(person);
                targetNames.Add(edit.Target);
            }
            else
            {
                foreach (PeopleIndex existing in instruction.Peoples)
                {
                    if (existing.name_e != edit.Target) continue;
                    person = existing;
                    break;
                }
            }

            switch (edit.ToDo)
            {
                case 0: person.country = (Country)edit.After; break;
                case 1: person.job = (Job)edit.After; break;
                case 2: person.belong = (Belonging)edit.After; break;
                case 3: person.part = (Part)edit.After; break;
                case 4: person.curFace = edit.After; break;
            }
        }
    }

    private void PrepareNewsAnswers(Instruction instruction)
    {
        News source = FindNews($"{Month}/" + Day.ToString("D2"));
        if (source == null) return;

        int maximumLine = source.Main.Count;
        foreach (var edit in instruction.NewsInst)
            if (edit.Line > maximumLine) maximumLine = edit.Line;

        instruction.NewsMain = new List<string>(source.Main);
        for (int index = source.Main.Count; index < maximumLine; index++)
            instruction.NewsMain.Add("");
        foreach (var edit in instruction.NewsInst)
            instruction.NewsMain[edit.Line] = edit.Goal;
    }

    IEnumerator TutoTest()
    {
        yield return new WaitForSeconds(0.2f);

        TutorialSetting.instance.ActiveTutorial();
    }

    /// <summary>
    /// 찾는 인물의 정보를 반환
    /// </summary>
    /// <param name="name"> 찾을 인물의 이름(영어) </param>
    /// <returns>인물(찾는 인물이 없는 경우 null)</returns>
    public PeopleIndex FindPeople(string name)
    {
        foreach(PeopleIndex a in PeopleList)
        {
            if ((a.name_e.ToLower() == name.ToLower() || a.name_k == name)) return a;
        }
        return null;
    }

    /*public void ChangeInfo(string name,string Country,string Job, int Face)
    {
        foreach(PeopleIndex a in PeopleList.PL)
        {
            if(a.name_e == name)
            {
                a.country = Country;
                a.job = Job;
                a.face = Face;
                break;
            }
        }
    }*/

    /// <summary>
    /// 찾는 뉴스의 정보를 반환
    /// </summary>
    /// <param name="Date"> 찾을 뉴스의 발간일 </param>
    /// <returns>뉴스(찾는 뉴스가 없는 경우 null)</returns>
    public News FindNews(string Date)
    {
        foreach(News a in NewsList)
        {
            if (a.publishDay == Date) return a;
        }
        return null;
    }

    /// <summary>
    /// 찾는 인물의 문서 정보를 반환
    /// </summary>
    /// <param name="Name"> 찾을 인물의 이름(한국어) </param>
    /// <returns>문서 정보(찾는 인물이 없는 경우 null)</returns>
    public Docs FindDocs(string Name)
    {
        foreach(Docs a in DocsList)
        {
            if (a.Subject == Name) return a;
        }
        return null;
    }

    /// <summary>
    /// 업무를 평가. 지시하지 않은 사항을 수행하거나 지시 사항을 수행하지 않았을 경우 각 부분의 Score가 +됨
    /// </summary>
    /// <param name="Score">{인물 종합 점수, 뉴스 종합 점수, 문서 종합 점수}</param>
    public void EvaluateWork(ref int[] Score)
    {
        Score[0] = EvaluatePeople();
        Score[1] = EvaluateNews();
        Score[2] = EvaluateDocuments();
    }

    private int EvaluatePeople()
    {
        int score = 0;
        for (int i = 0; i < Instructions.Peoples.Count; i++)
            score += Instructions.Peoples[i].Evaluate(FindPeople(Instructions.InfoInst[i].Target));
        return score;
    }

    private int EvaluateNews()
    {
        int score = 0;
        News currentNews = FindNews($"{Month}/" + Day.ToString("D2"));
        if (currentNews != null)
        {
            score -= Mathf.Abs(currentNews.Main.Count - Instructions.NewsMain.Count);
            int sharedLineCount = Mathf.Min(currentNews.Main.Count, Instructions.NewsMain.Count);
            for (int index = 0; index < sharedLineCount; index++)
                if (currentNews.Main[index].TrimEnd('\n', '\r') != Instructions.NewsMain[index].TrimEnd('\n', '\r'))
                    score--;
        }
        return score;
    }

    private int EvaluateDocuments()
    {
        int score = 0;
        foreach (var instruction in Instructions.DocsInst)
        {
            Docs document = FindDocs(instruction.Name);
            if (document.IsAbnormalFinded != document.IsWrongDocs)
                score--;
        }
        return score;
    }
}
