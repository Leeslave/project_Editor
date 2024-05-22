using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 해당 게임의 모든 I/O 입출력은 해당 코드를 통해 이루어짐
public class DB_M : MonoBehaviour
{
    public static DB_M DB_Docs;

    [SerializeField] Windows_M DBFolder;
    [SerializeField] Windows_M NewsFolder;
    [SerializeField] Windows_M DocsFolder;
    [SerializeField] GameObject Secret;
    [SerializeField] Sprite spr;

    public int Month;
    public int Day;

    // 0 : 국, 1 : 부, 2 : 소, 3 ; 직
    public List<string[]> InfSub = new List<string[]>(4);
    public List<PeopleIndex> PeopleList;
    public News[] NewsList;
    public List<Instruction> InstructionList;
    [NonSerialized] public Instruction Instructions;

    public Docs[] DocsList;
    private int stageInt;
    void Awake()
    {
        try { stageInt = GameSystem.Instance.GetTask("Document"); } catch (Exception e) { }
        if (DB_Docs != null) { Destroy(gameObject); return; }
        DB_Docs = this;
        // Read Manipulation Data
        InfSub.Add(Enum.GetNames(typeof(Country)));
        InfSub.Add(Enum.GetNames(typeof(Belonging)));
        InfSub.Add(Enum.GetNames(typeof(Part)));
        InfSub.Add(Enum.GetNames(typeof(Job)));

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
                List<string> TargetSub = new List<string>();
                foreach (var j in k.InfoInst)
                {
                    PeopleIndex s = new PeopleIndex();
                    if (!TargetSub.Contains(j.Target))
                    {
                        s = new PeopleIndex(FindPeople(j.Target));
                        k.Peoples.Add(s);
                        TargetSub.Add(j.Target);
                    }
                    else foreach(var i in k.Peoples)if(i.name_e == j.Target) { s = i; break; }

                    switch (j.ToDo)
                    {
                        case 0:
                            s.country = (Country)j.After;
                            break;
                        case 1:
                            s.job = (Job)j.After;
                            break;
                        case 2:
                            s.curFace = j.After;
                            break;
                        case 3:
                            s.belong = (Belonging)j.After;
                            break;
                        case 4:
                            s.part = (Part)j.After;
                            break;
                    }
                }
                

                News NewsSub = FindNews($"{Month}/" + Day.ToString("D2"));
                if (NewsSub != null)
                {
                    // Calculate Cur News's Maximum Line 
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

    public News FindNews(string Date)
    {
        foreach(News a in NewsList)
        {
            if (a.publishDay == Date) return a;
        }
        return null;
    }

    public Docs FindDocs(string Name)
    {
        foreach(Docs a in DocsList)
        {
            if (a.Subject == Name) return a;
        }
        return null;
    }

    public void EvaluateWork(ref int[] Score)
    {
        // Evaluate Info
        int Score_Info = 0;
        for(int i = 0; i < Instructions.InfoInst.Length; i++) Score_Info += Instructions.Peoples[i].Evaluate(FindPeople(Instructions.InfoInst[i].Target));

        // Evaluate News
        int Score_News = 0;
        var EvalNews = FindNews($"{Month}/" + Day.ToString("D2"));
        Score_News -= Mathf.Abs(EvalNews.Main.Count - Instructions.NewsMain.Count);
        int l = Mathf.Min(EvalNews.Main.Count, Instructions.NewsMain.Count);
        
        for(int i = 0; i < l; i++)
        {
            if (EvalNews.Main[i].TrimEnd('\n','\r') != Instructions.NewsMain[i].TrimEnd('\n', '\r')) Score_News--;
        }

        // Evaluate Docs
        int Score_Docs = 0;
        // 추후 추가

        Score[0] = Score_Info; Score[1] = Score_News; Score[2] = Score_Docs;
    }
}
