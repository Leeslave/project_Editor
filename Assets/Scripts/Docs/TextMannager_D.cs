using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMannager_D : MonoBehaviour
{
    [SerializeField] public ToDoList_N TDN;

    [SerializeField] bool IsRecord;

    [SerializeField] GameObject TextsPref;
    [SerializeField] GameObject DotPref;
    [SerializeField] EventTrigger NotTouch;


    [NonSerialized] List<GameObject> Dots;
    [NonSerialized] List<GameObject> TextsObj;
    [NonSerialized] List<Docs_Back> Texts;

    public TextMannager_D OtherMannager;

    bool IsClear = false;

    int ActivateText = 0;

    public Transform CurSelection;
    int CurOpen = -1;

    public int Errors = 0;

    public int MyAns = -1;

    private void Start()
    {
        TextsObj = new List<GameObject> { TextsPref };
        Dots = new List<GameObject>();

        for (int i = 0; i < 25; i++) Dots.Add(Instantiate(DotPref, transform));
        for (int i = 0; i < 15; i++) TextsObj.Add(Instantiate(TextsPref, TextsPref.transform.parent));
        Texts = new List<Docs_Back>();
        foreach (var k in TextsObj)
        {
            Texts.Add(k.GetComponent<Docs_Back>());
            k.SetActive(false);
        }
    }

    private void OnDisable()
    {
        CurOpen = -1;
        foreach (var k in TextsObj) k.SetActive(false); ActivateText = 0;
    }

    public void AddText(string text, Color color, TextAlignmentOptions align = TextAlignmentOptions.Left, bool IsTouchAble = true)
    {
        TextsObj[ActivateText].SetActive(true);
        Texts[ActivateText++].AddTexts(text, color, this,align, IsTouchAble); 
    }

    public void Clicked(int Ind, Transform tr)
    {
        if (CurOpen != -1) Texts[CurOpen].UnSelect();
        CurOpen = Ind; CurSelection = tr;
    }

    public void JudgeStart()
    {
        if(OtherMannager.CurOpen != -1)
        {
            NotTouch.gameObject.SetActive(true);
            Vector3 Gap = (OtherMannager.CurSelection.position - CurSelection.position) * 0.5f;
            int x = Mathf.FloorToInt(Mathf.Abs(Gap.x / 30));
            int y = Mathf.FloorToInt(Mathf.Abs(Gap.y / 30));
            Vector3 xGap = x == 0 ? Vector3.zero : Gap / x; xGap.y = 0;
            Vector3 yGap = y == 0 ? Vector3.zero : Gap / y; yGap.x = 0;

            Vector3 StartPos = CurSelection.position;

            int l = 0;
            for (int i = 0; i < x; i++) { Dots[l++].transform.position = StartPos+ xGap * i;}
            StartPos = StartPos + xGap * (x - 1);
            for(int i = 0; i <= y; i++) { Dots[l++].transform.position = StartPos + yGap * i;}
            StartCoroutine(DotAct(l));
        }

    }

    [SerializeField] TMP_Text Message;
    IEnumerator DotAct(int i)
    {
        for(int x = 0; x < i-1; x++)
        {
            Dots[x].SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        if (IsRecord)
        {
            Message.transform.position = Dots[i-1].transform.position; Message.gameObject.SetActive(true);
            if (IsCor() && OtherMannager.IsCor())
            {
                Message.text = $"Abnormal Detection!";
                TDN.CheckList(1, 0, true);
            }
            else Message.text = $"No Abnormal";
            MyUi.AddEvent(NotTouch, EventTriggerType.PointerClick,
                (PointerEventData DT) =>
                {
                    RemoveDot(); OtherMannager.RemoveDot(); Message.gameObject.SetActive(false); NotTouch.gameObject.SetActive(false);NotTouch.triggers.Clear();
                }
            );
        }
    }

    public bool IsCor() 
    {
        if (MyAns == CurOpen) Texts[CurOpen].GetCorrected();
        return MyAns == CurOpen; 
    }

    public void RemoveDot() { foreach (var k in Dots) k.SetActive(false); }
}
