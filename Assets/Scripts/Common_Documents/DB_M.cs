using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DB_M : MonoBehaviour
{
    [SerializeField] Windows_M DBFolder;
    [SerializeField] Windows_M NewsFolder;
    [SerializeField] Sprite spr;

    public Peoples PeopleList;
    public Dictionary<string, Sprite[]> FaceImages = new Dictionary<string, Sprite[]>();
    public News[] NewsList;
    void Start()
    {
        string CurPath = Directory.GetCurrentDirectory() + "\\Assets\\Resources";
        // Read Manipulation Data
        PeopleList = JsonUtility.FromJson<Peoples>(File.ReadAllText(CurPath + "\\Manipulation\\People.json"));
        foreach (PeopleIndex a in PeopleList.PL)
        {
            DBFolder.NewIcon(a.name_e, spr, 1);
            FaceImages.Add(a.name_e, Resources.LoadAll<Sprite>("Manipulation/" + a.name_e));
        }
        DBFolder.gameObject.SetActive(false);
        //Read News Data
        var Files = Directory.GetDirectories(CurPath + "\\News");
        NewsList = new News[Files.Length];

        for (int i = 0; i < Files.Length; i++)
        {
            string cnt = Files[i][(CurPath.Length + 6)..].Replace('_','/');
            NewsFolder.NewIcon(name: cnt, Image: spr,2);
            NewsList[i] = new News();
            NewsList[i].publishDay = cnt;
            ReadMain(NewsList[i], Files[i] + "\\Main.txt");
            ReadRevise(NewsList[i], Files[i] + "\\Revise.txt");
        }
        NewsFolder.gameObject.SetActive(false);
    }

    public PeopleIndex FindPeople(string name)
    {
        foreach(PeopleIndex a in PeopleList.PL)
        {
            if ((a.name_e.ToLower() == name.ToLower() || a.name_k == name)) return a;
        }
        return null;
    }

    public void ChangeInfo(string name,string Country,string Job, int Face)
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
    }

    public News FindNews(string Date)
    {
        foreach(News a in NewsList)
        {
            print(a.publishDay);
            print(Date);
            if (a.publishDay == Date) return a;
        }
        return null;
    }

    void ReadMain(News N, string path)
    {
        StreamReader reader = new StreamReader(path);
        N.Title = reader.ReadLine();
        N.Date = reader.ReadLine();
        N.Reporter = reader.ReadLine();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Contains("[/]"))
            {
                line = line.Replace("[/]", "");
                N.Errors.Add(N.CountM);
            }
            N.Main[N.CountM++] = line;
        }
        reader.Close();
    }

    void ReadRevise(News N, string path)
    {
        StreamReader reader = new StreamReader(path);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            
            N.Revise[N.CountR++] = line;
        }
        reader.Close();
    }
}
