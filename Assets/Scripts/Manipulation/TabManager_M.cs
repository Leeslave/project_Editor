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

    private void OnEnable()
    {
        for(int i = 3; i >= 1; i++)
        {
            if (Tabs[i].activeSelf) DeleteTab(i);
        }
        if(Texts[0].text != "Main")
        {
            Texts[0].text = "Main";
            Tabs_M[0].ChangeFolder(null);
            Tabs_M[0].ClickByOther();
        }

    }

    public bool AddTab()
    {
        Texts[ActiveTab].text = "Main";
        Tabs[ActiveTab++].SetActive(true);
        return ActiveTab == 4;
    }

    public void DeleteTab(int TabInd)
    {
        for (int i = TabInd; i < ActiveTab - 1; i++)
        {
            Texts[i].text = Texts[i + 1].text;
            Tabs_M[i].ChangeFolder(Tabs_M[i + 1].ReturnMyFolder());
        }
        Tabs[--ActiveTab].SetActive(false);
        if (!Adder.activeSelf) Adder.SetActive(true);
        if (TabInd == CurTab)
        {
            if (ActiveTab == TabInd) CurTab = TabInd - 1;
            else CurTab = TabInd;
            Tabs_M[CurTab].ClickByOther();
        }
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
