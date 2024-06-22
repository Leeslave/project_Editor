using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// 해당 게임의 모든 I/O 입출력은 해당 코드를 통해 이루어짐
public class DB_M : MonoBehaviour
{
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
    [SerializeField] GameObject Secret;         // Secret 폴더
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
    private int stageInt = 0;
    void Awake()
    {
        try
        {
            stageInt = GameSystem.Instance.GetTask("Document");
            Debug.Log(stageInt);
            Day += stageInt;
        }
        catch
        {
            stageInt = 0;
        }
        
        if (DB_Docs != null) { Destroy(gameObject); return; }
        DB_Docs = this;

        // Read Manipulation Data
        InfSub.Add(Enum.GetNames(typeof(Country)));
        InfSub.Add(Enum.GetNames(typeof(Job)));
        InfSub.Add(Enum.GetNames(typeof(Belonging)));
        InfSub.Add(Enum.GetNames(typeof(Part)));
        for (int i = 0; i < PeopleList.Count - 1; i++) DBFolder.NewIcon(PeopleList[i].name_e, spr, 1);
        DBFolder.gameObject.SetActive(false);
        
        //Read News Data
        for (int i = 0; i < NewsList.Length - 1; i++)
        {
            NewsFolder.NewIcon(NewsList[i].publishDay, spr, 2);
            var j = NewsList[i].Main[0].Split('\n');
            if (j.Length != 1) NewsList[i].Main = j.ToList();
        }
        NewsFolder.gameObject.SetActive(false);
        //Read Docs Data
        foreach (var k in DocsList) DocsFolder.NewIcon(name: k.Subject, Image: spr, 3);
        DocsFolder.gameObject.SetActive(false);

        // Set Instruction
        foreach(var k in InstructionList)
        {
            if (k.Month == Month && k.date == Day) { Instructions = k;
                // 정답용 Data에 등록된 인물의 이름을 저장(중복 생성 방지)
                List<string> TargetSub = new List<string>();

                // Instruction과 기존 DB의 인물 정보를 조합하여 정답용 Data를 생성
                foreach (var j in k.InfoInst)
                {
                    PeopleIndex s = new PeopleIndex();
                    if (!TargetSub.Contains(j.Target))          // 정답용 Data에 해당 인물이 없을 경우 s에 해당 인물 정보 할당
                    {
                        s = new PeopleIndex(FindPeople(j.Target));
                        k.Peoples.Add(s);
                        TargetSub.Add(j.Target);
                    }
                    else foreach(var i in k.Peoples)if(i.name_e == j.Target) { s = i; break; }      // 이미 있을 경우, 정답용 Data에서 해당 인물의 정보를 가져온 뒤 s에 할당
  
                    switch (j.ToDo)
                    {
                        case 0:
                            s.country = (Country)j.After;
                            break;
                        case 1:
                            s.job = (Job)j.After;
                            break;
                        case 4:
                            s.curFace = j.After;
                            break;
                        case 2:
                            s.belong = (Belonging)j.After;
                            break;
                        case 3:
                            s.part = (Part)j.After;
                            break;
                    }
                }

                // Instruction과 기존 DB의 뉴스 정보를 조합하여 정답용 Data를 생성
                News NewsSub = FindNews($"{Month}/" + Day.ToString("D2"));
                if (NewsSub != null)
                {
                    // Calculate Cur News's Maximum Line(mx)
                    int mx = -1; foreach (var j in k.NewsInst) if (j.Line > mx) mx = j.Line;
                    // Add Over Line & Add News Main To Evaluate News Main
                    k.NewsMain = new List<string>(); foreach (var i in NewsSub.Main) k.NewsMain.Add(i); for (int I = 0; I < NewsSub.Main.Count + 1 - mx; I++) k.NewsMain.Add("");
                    // Apply Changes At Evaluate News
                    foreach (var i in k.NewsInst) k.NewsMain[i.Line] = i.Goal;
                }
                break; 
            }
        }
        Secret.gameObject.SetActive(false);
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
        // Evaluate Info
        int Score_Info = 0;
        for(int i = 0; i < Instructions.InfoInst.Length; i++) Score_Info += Instructions.Peoples[i].Evaluate(FindPeople(Instructions.InfoInst[i].Target));

        // Evaluate News
        int Score_News = 0;
        var EvalNews = FindNews($"{Month}/" + Day.ToString("D2"));
        if (EvalNews != null)
        {
            Score_News -= Mathf.Abs(EvalNews.Main.Count - Instructions.NewsMain.Count);
            int l = Mathf.Min(EvalNews.Main.Count, Instructions.NewsMain.Count);
        
            for(int i = 0; i < l; i++)
            {
                if (EvalNews.Main[i].TrimEnd('\n','\r') != Instructions.NewsMain[i].TrimEnd('\n', '\r')) Score_News--;
            }
        }
        

        // Evaluate Docs
        int Score_Docs = 0;
        // 추후 추가

        Score[0] = Score_Info; Score[1] = Score_News; Score[2] = Score_Docs;
    }
}
