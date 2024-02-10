using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_N : MonoBehaviour
{
    [SerializeField] GameObject[] Texts;
    [SerializeField] MainText_N[] MainTexts;
    [SerializeField] public ToDoList_N TDN;

    [NonSerialized] public News AnsNews;
    //[NonSerialized] public Docs DocsAns;

    [NonSerialized] public bool[] NewsAnsLine = new bool[20];
    [NonSerialized] public string[] NewsChange = new string[20];

    [NonSerialized] public bool[] DocsAnsLine = new bool[20];
    [NonSerialized] public string[] DocsChange = new string[20];

    bool IsClear = false;

    int ActivateText = 0;

    private void Start()
    {
        foreach (var k in Texts) if (k.activeSelf) ActivateText++;
    }

    int CurOpen = 0;
    public void OpenText(int Ind)
    {
        if(Ind != CurOpen)
        {
            MainTexts[CurOpen].gameObject.SetActive(false);
            CurOpen = Ind;
        }
    }

    public void ActiveText(string Text,int ind = -1)
    {
        for(int i = 0; i < Texts.Length; i++)
        {
            if (!Texts[i].activeSelf)
            {
                if(ind != -1) Texts[i].transform.SetSiblingIndex(ind+5);
                Texts[i].SetActive(true);
                MainTexts[i].AddLine(Text,ActivateText++);
                break;
            }
        }
        ResetIndex();
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
        for(int i = 0; i < Texts.Length; i++)
        {
            if (Texts[i].activeSelf)
            {
                MainTexts[i].MyInd = Texts[i].transform.GetSiblingIndex() - 4;
            }
        }
    }

    public void ValidText(bool IsNews, int line, string text)
    {
        if (IsNews)
        {
            if (NewsAnsLine[line])
            {
                if (text.Equals(NewsChange[line])) TDN.CheckList(0, line, true);
                else TDN.CheckList(0, line, false);
            }
            else if (!text.Equals(AnsNews.Main[line])) for (int i = 0; i < AnsNews.CountM; i++) if (NewsAnsLine[i]) MainTexts[i].CheckMyText();
        }
        else
        {
            if (DocsAnsLine[line])
            {
                if (text.Equals(DocsChange[line])) TDN.CheckList(1, line, true);
                else TDN.CheckList(1, line, false);
            }
            //else if (!text.Equals(AnsDocs.Main[line])) for (int i = 0; i < AnsDocs.CountM; i++) if (DocsAnsLine[i]) MainTexts[i].CheckMyText();
        }
    }
}
