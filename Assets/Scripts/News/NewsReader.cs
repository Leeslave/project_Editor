using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NewsReader : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Date;
    [SerializeField] TMP_Text Reporter;
    [SerializeField] List<TMP_Text> Mains;
    [SerializeField] List<GameObject> MainsBack;
    [SerializeField] List<TMP_Text> Revises;
    [SerializeField] List<GameObject> RevisesBack;
/*
    struct News
    {
        public string Title;
        public string Date;
        public string Reporter;
        public string[] Main; *//*= new string[50];*//*
        public int CountM; *//*= 0;*//*
        public string[] Revise; *//*= new string[4];*//*
        public int CountR; *//*= 0;*//*
        public List<int> Errors; *//*= new List<int>(4);*//*
    }

    private void Awake()
    {
        var Files = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\Assets\\Resources\\News");
        News[] news = new News[Files.Length];

        for (int i = 0; i < Files.Length; i++)
        {
            news[i].CountM = 0;
            news[i].CountR = 0;
            news[i].Main = new string[50];
            news[i].Revise = new string[4];
            news[i].Errors = new List<int>(4);
            ReadMain(news[i], Files[i] + "\\Main.txt");
            ReadRevise(news[i], Files[i] + "\\Revise.txt");
        }
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
            //Mains[CountM].text = line;
            //MainsBack[CountM].SetActive(true);
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
            //Revises[CountR].text = line;
            //RevisesBack[CountR].SetActive(true);
            N.Revise[N.CountR++] = line;
        }
        reader.Close();
    }*/
}

