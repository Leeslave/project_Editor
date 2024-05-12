using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 해당 게임의 모든 I/O 입출력은 해당 코드를 통해 이루어짐
public class DB_M : MonoBehaviour
{
    [SerializeField] Windows_M DBFolder;
    [SerializeField] Windows_M NewsFolder;
    [SerializeField] Windows_M DocsFolder;
    [SerializeField] GameObject Secret;
    [SerializeField] Sprite spr;

    public int Month;
    public int Day;

    public List<string[]> InfSub = new List<string[]>(2);
    public List<PeopleIndex> PeopleList;
    public News[] NewsList;
    public List<Instruction> InstructionList;
    [NonSerialized] public Instruction Instructions;

    public Docs[] DocsList;

    void Awake()
    {
        string CurPath = Directory.GetCurrentDirectory() + "\\Assets\\Resources\\GameData";
        // Read Manipulation Data
        InfSub.Add(File.ReadAllText(CurPath + $"\\Manipulation\\Countries.txt").Split('\n'));
        InfSub.Add(File.ReadAllText(CurPath + $"\\Manipulation\\Jobs.txt").Split('\n'));

        for (int i = 0; i < PeopleList.Count - 1; i++) DBFolder.NewIcon(PeopleList[i].name_e, spr, 1);
        DBFolder.gameObject.SetActive(false);
        //Read News Data

        for (int i = 0; i < NewsList.Length - 1; i++)
        {
            NewsFolder.NewIcon(NewsList[i].publishDay, spr, 2);
            var j = NewsList[i].Main[0].Split('\n');
            if (j.Length != 1) NewsList[i].Main = j;
        }
        NewsFolder.gameObject.SetActive(false);
        //Read Docs Data
        foreach (var k in DocsList) DocsFolder.NewIcon(name: k.Subject, Image: spr, 3);
        DocsFolder.gameObject.SetActive(false);

        foreach(var k in InstructionList)
        {
            if (k.Month == Month && k.date == Day) { Instructions = k; break; }
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

  /*  void ReadMain(News N, string path)
    {
        StreamReader reader = new StreamReader(path);
        N.Title = reader.ReadLine();
        N.Date = reader.ReadLine();
        N.Reporter = reader.ReadLine();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            N.Main[N.CountM++] = line;
        }
        reader.Close();
    }*/
}
