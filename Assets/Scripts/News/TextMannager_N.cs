using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_N : MonoBehaviour
{

    [SerializeField] public ToDoList_N TDN;


    [SerializeField] GameObject TextsPref;

    [NonSerialized] List<GameObject> Texts;
    [NonSerialized] List<MainText_N> MainTexts;

    [NonSerialized] public News AnsNews;
    //[NonSerialized] public Docs DocsAns;

    [NonSerialized] public bool[] NewsAnsLine = new bool[20];
    [NonSerialized] public string[] NewsChange = new string[20];

    [NonSerialized] public bool[] DocsAnsLine = new bool[20];
    [NonSerialized] public string[] DocsChange = new string[20];

    bool IsClear = false;

    int ActivateText = 0;

    private void Awake()
    {
        int i;
        Texts = new List<GameObject> { TextsPref }; for (i = 0; i < 10; i++) Texts.Add(Instantiate(TextsPref, TextsPref.transform.parent));
        MainTexts = new List<MainText_N>();
        i = 0;
        foreach (var k in Texts)
        {
            k.SetActive(false);
            MainTexts.Add(k.transform.GetChild(0).GetComponent<MainText_N>());
            MainTexts[i].MyInd = i++;
        }
    }

    int CurOpen = 0;
    public void OpenText(int Ind)
    {
        if (Ind != CurOpen)
        {
            MainTexts[CurOpen].gameObject.SetActive(false);
            CurOpen = Ind;
        }
    }

    public void ActiveText(string Text, int ind = -1)
    {
        for (int i = 0; i < Texts.Count; i++)
        {
            if (!Texts[i].activeSelf)
            {
                if (ind != -1) Texts[i].transform.SetSiblingIndex(ind + 5);
                Texts[i].SetActive(true);
                MainTexts[i].AddLine(Text, ActivateText++);
                break;
            }
        }
        if (ind != -1) ResetIndex();
    }

    public void RemoveText(int Ind)
    {
        Texts[Ind].SetActive(false);
        MainTexts[Ind].DelLine();
        Texts[Ind].transform.SetAsLastSibling();
        ActivateText--;
        ResetIndex();
    }

    public void ResetIndex()
    {
        for (int i = 0; i < Texts.Count; i++)
        {
            if (Texts[i].activeSelf)
            {
                MainTexts[i].MyInd = Texts[i].transform.GetSiblingIndex() - 4;
            }
        }
        for (int i = 0; i < AnsNews.Main.Length; i++) if (NewsAnsLine[i]) MainTexts[i].CheckMyText();
    }

    public void ValidText(bool IsNews, int line, string text)
    {
        text = text.TrimEnd('\r', '\n');
        if (NewsAnsLine[line])
        {
            if (text.Equals(NewsChange[line])) TDN.CheckList(0, line, true);
            else TDN.CheckList(0, line, false);
        }
        else if (!text.Equals(AnsNews.Main[line])) for (int i = 0; i < AnsNews.Main.Length; i++) if (NewsAnsLine[i]) MainTexts[i].CheckMyText();
    }
}
