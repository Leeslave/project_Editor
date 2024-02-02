using System;
using System.Collections.Generic;
using System.Linq.Expressions; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_N : MonoBehaviour
{
    [SerializeField] GameObject[] Texts;
    [SerializeField] MainText_N[] MainTexts;
    [SerializeField] public ToDoList_N TDN;

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
}
