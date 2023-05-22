using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabManager_M : MonoBehaviour
{
    public GameObject[] Tabs;
    public TMP_Text[] Texts;
    public GameObject Adder;

    Tab_M[] Tabs_M = new Tab_M[4];
    int ActiveTab = 1;
    int CurTab = 0;

    private void Awake()
    {
        for(int i = 0; i < 4; i++) Tabs_M[i] = Tabs[i].GetComponent<Tab_M>();
    }

    public bool AddTab()
    {
        Texts[ActiveTab].text = "Main";
        Tabs[ActiveTab++].SetActive(true);
        return ActiveTab == 4;
    }

    public void DeleteTab(int TabInd)
    {
        for(int i = TabInd+1; i < ActiveTab-1; i++)
        {
            Texts[i].text = Texts[i + 1].text;
        }
        Tabs[--ActiveTab].SetActive(false);
        if (!Adder.activeSelf) Adder.SetActive(true);
    }
    public void ChangeTab(int ChangeNum)
    {
        Tabs_M[CurTab].Deselect();
        CurTab = ChangeNum;
    }
    public void ChangeFolder(HighLighter_M cnt,string name)
    {
        Texts[CurTab].text = name;
        Tabs_M[CurTab].ChangeFolder(cnt);
    }
}
