using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NewsReader : MonoBehaviour
{
    [SerializeField] int Cur = 1;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Date;
    [SerializeField] TMP_Text Reporter;
    [SerializeField] List<TMP_Text> Mains;
    [SerializeField] List<GameObject> MainsBack;
    [SerializeField] List<TMP_Text> Revises;
    [SerializeField] List<GameObject> RevisesBack;

    string[] Main = new string[50];
    int CountM = 0;
    string[] Revise = new string[4];
    int CountR = 0;
    [NonSerialized] public List<int> Errors = new List<int>(4);

    private void Awake()
    {
        ReadRevise();
        ReadMain(); 
    }
    void ReadMain()
    {
        TextAsset textFile = Resources.Load($"News/{Cur}/Main") as TextAsset;
        if (textFile == null)
        {
            return;
        }
        StringReader stringReader = new StringReader(textFile.text);
        Title.text = stringReader.ReadLine();
        Date.text = stringReader.ReadLine();
        Reporter.text = stringReader.ReadLine();
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null) break;
            if (line.Contains("[/]"))
            {
                line = line.Replace("[/]", "");
                Errors.Add(CountM);
            }
            Mains[CountM].text = line;
            MainsBack[CountM].SetActive(true);
            Main[CountM] = line;
            CountM++;
        }
        stringReader.Close();
    }

    void ReadRevise()
    {
        TextAsset textFile = Resources.Load($"News/{Cur}/Revise") as TextAsset;
        if (textFile == null)
        {
            return;
        }
        StringReader stringReader = new StringReader(textFile.text);
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null) break;
            Revises[CountR].text = line;
            RevisesBack[CountR].SetActive(true);
            Revise[CountR] = line;
            CountR++;
        }
        stringReader.Close();
    }
}

