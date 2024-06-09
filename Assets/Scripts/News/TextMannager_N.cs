using System;
using System.Collections.Generic;
using UnityEngine;

public class TextMannager_N : MonoBehaviour
{
    [SerializeField] GameObject TextsPref;

    [NonSerialized] List<GameObject> Texts;
    [NonSerialized] List<MainText_N> MainTexts;
    [NonSerialized] List<MainText_Back> Backs = new List<MainText_Back>();

    [NonSerialized] public News CurNews;

    [NonSerialized] public bool[] NewsAnsLine = new bool[20];
    [NonSerialized] public string[] NewsChange = new string[20];

    [NonSerialized] public bool[] DocsAnsLine = new bool[20];
    [NonSerialized] public string[] DocsChange = new string[20];

    /// <summary>
    /// Tuple : 수정 전, 수정이 이뤄진 줄, (0 : 변경, 1 : 추가, 2 : 삭제)
    /// </summary>
    [NonSerialized] public List<Tuple<string, int, int>> Commands_Back = new List<Tuple<string, int, int>>();      // CTRL + Z
    // 넣어만 둠 [NonSerialized] public List<Tuple<string, int, int>> Commands_Go = new List<Tuple<string, int, int>>();        // CTRL + Y

    int ActivateText = 0;

    private void Update()
    {
        // 작업 취소 임시 기능
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (MainTexts[CurOpen].gameObject.activeSelf) return;
            if (Input.GetKeyDown(KeyCode.I) && Commands_Back.Count != 0)
            {
                var command = Commands_Back[0]; Commands_Back.RemoveAt(0);
                switch (command.Item3)
                {
                    case 0:     // 변경 취소
                        foreach (var k in MainTexts) if (k.MyInd == command.Item2) 
                            {
                                k.Text.text = command.Item1; k.ReBuildRect(); break;
                            }
                        break;
                    case 1:     // 추가 취소
                        foreach (var k in MainTexts) if (k.MyInd == command.Item2) { k.DelSelf(true); break; }
                        break;
                    case 2:     // 삭제 취소
                        ActiveText(command.Item1, command.Item2-1,true);
                        break;
                }
            }
        }
    }

    private void Awake()
    {
        int i;
        Texts = new List<GameObject> { TextsPref }; for (i = 0; i < 11; i++) Texts.Add(Instantiate(TextsPref, TextsPref.transform.parent));
        MainTexts = new List<MainText_N>();
        i = 0;
        foreach (var k in Texts)
        {
            Backs.Add(k.GetComponent<MainText_Back>());
            MainTexts.Add(k.transform.GetChild(0).GetComponent<MainText_N>());
            MainTexts[i].MyInd = i++;
            k.gameObject.SetActive(false);
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

    public void ActiveText(string Text, int ind = -2,bool IsRollBack = false)
    {
        for (int i = 0; i < Texts.Count; i++)
        {
            if (!Texts[i].activeSelf)
            {
                if (ind != -2)
                {
                    Texts[i].transform.SetSiblingIndex(ind + 5);
                    if (!IsRollBack) Commands_Back.Insert(0,new Tuple<string, int, int>("", ind+1, 1));
                }
                Texts[i].SetActive(true);
                MainTexts[i].AddLine(Text, ActivateText++);
                break;
            }
        }
        if (ind != -1) ResetIndex();
    }

    public void RemoveText(int Ind,string LastText,bool IsRollBack = false)
    {
        if(!IsRollBack) Commands_Back.Insert(0,new Tuple<string, int, int>(LastText, Ind, 2));
        Texts[Ind].SetActive(false);
        MainTexts[Ind].DelLine();
        Texts[Ind].transform.SetAsLastSibling();
        ActivateText--;
        ResetIndex();
    }


    // Reset Cur Index Of Texts
    public void ResetIndex()
    {
        for (int i = 0; i < Texts.Count; i++) MainTexts[i].MyInd = Texts[i].transform.GetSiblingIndex() - 4;
        for (int i = 0; i < MainTexts.Count; i++) if (NewsAnsLine[MainTexts[i].MyInd])
                MainTexts[i].CheckMyText();
    }

    public void ValidText(bool IsNews, int line, string text)
    {
        text = text.TrimEnd('\r', '\n');
        if (NewsAnsLine[line])
        {
            if (text.Equals(NewsChange[line]) && Texts[line].gameObject.activeSelf) DB_M.DB_Docs.ToDoList.CheckList(0, line, true);
            else DB_M.DB_Docs.ToDoList.CheckList(0, line, false);
        }
        else foreach(var k in MainTexts)
            {
                if (NewsAnsLine[k.MyInd]) ValidText(true, k.MyInd, k.Text.text);
            }
    }

    // Reset All Texts
    private void OnDisable()
    {
        foreach(var k in MainTexts)
        {
            if (k.transform.parent.gameObject.activeSelf)
            {
                while (k.MyInd >= CurNews.Main.Count) CurNews.Main.Add("");
                CurNews.Main[k.MyInd] = k.Text.text;
            }
        }
        foreach (var k in Backs)
        {
            k.transform.SetSiblingIndex(k.MyInd + 4);
            k.gameObject.SetActive(false);
        }
        ActivateText = 0;
    }
}
