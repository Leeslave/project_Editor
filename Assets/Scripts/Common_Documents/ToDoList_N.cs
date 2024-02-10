using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class ToDoList_N : MonoBehaviour
{
    [SerializeField] DB_M DB;
    [SerializeField] TMP_Text[] Texts;
    [SerializeField] TextMannager_N TextM;
    [SerializeField] InfChange InfM;

    string NRed = "#FF0000";
    string DRed = "#800000";
    string NWhite = "#FFFFFF";
    string DWhite = "#808080";

    int ToDoCount = 0;

    class ToDoIndex
    {
        public int line;
        public int type;
        public bool IsClear;
        public string line1;
        public string line2;
        public int goalLine;
        public string goal;
        public ToDoIndex(int line, int type, string line1, string line2,int goalLine,string goal)
        {
            this.line = line;
            this.type = type;
            this.IsClear = false;
            this.line1 = line1;
            this.line2 = line2;
            this.goalLine = goalLine;
            this.goal = goal;
        }
    }

    // 2: Info, 1 : Docs, 0 : News
    List<List<ToDoIndex>> ToDoIndexes;
    string[] sub = { "국적", "직업", "얼굴" };
    string[] sub2 = { "추가", "삭제", "변경" };
    private void Start()
    {
        ToDoIndexes = new List<List<ToDoIndex>>(3);
        for (int i = 0; i < 3; i++) ToDoIndexes.Add(new List<ToDoIndex>());
        
        string cnt = "";
        if(DB.Instructions.InfoInst!=null)
            foreach(var i in DB.Instructions.InfoInst)
            {
                Texts[ToDoCount].text = $"<color={NRed}>Info</color> {i.Target} {sub[i.ToDo]} 변경";
                if(i.ToDo != 2) cnt = $"{DB.InfSub[i.ToDo][i.Before].Trim('\r')} > {DB.InfSub[i.ToDo][i.After].Trim('\r')}";
                else cnt = $"Face{i.Before} > Face{i.After}";

                Texts[ToDoCount].text += $"<size=20>\n\n{cnt}</size>";

                InfM.PeopleCorrect.Add(new Tuple<string,int,int,int>(i.Target, i.ToDo,i.After, ToDoCount));
                ToDoIndexes[2].Add(new ToDoIndex(0, i.ToDo,$"{i.Target} {sub[i.ToDo]}", cnt,0,$"{i.After}"));
                Texts[ToDoCount].gameObject.SetActive(true);
                ToDoCount++;
            }
        
        if(DB.Instructions.NewsInst!=null)
            foreach (var i in DB.Instructions.NewsInst)
            {
                Texts[ToDoCount].text =
                    $"<color=#FF0000>News</color> {i.Line+1}번째 줄";
                if (i.ToDo == 0)
                {
                    Texts[ToDoCount].text += $" 추가<size=20>\n\n {i.Goal}</size>";
                    cnt = $"{i.Goal}";
                }
                else if (i.ToDo == 1)
                {
                    Texts[ToDoCount].text += $" 삭제";
                    cnt = "";
                }
                else
                {
                    Texts[ToDoCount].text += $" 변경<size=20>\n\n {i.Normal} > {i.Revise}</size>";
                    cnt = $"{i.Normal} > {i.Revise}";
                }
                TextM.NewsAnsLine[i.Line] = true;
                TextM.NewsChange[i.Line] = i.Goal;
                ToDoIndexes[0].Add(new ToDoIndex(ToDoCount,i.ToDo, $"{i.Line+1}번째 줄 {sub2[i.ToDo]}",cnt,i.Line,i.Goal));
                Texts[ToDoCount].gameObject.SetActive(true);
                ToDoCount++;
            }
        if(DB.Instructions.DocsInst != null)
            foreach (var i in DB.Instructions.DocsInst)
            {
                Texts[ToDoCount].text =
                    $"<color=#FF0000>Docs</color> {i.Line+1}번째 줄";
                if (i.ToDo == 0)
                {
                    Texts[ToDoCount].text += $" 추가<size=20>\n\n {i.Goal}</size>";
                    cnt = $"{i.Goal}";
                }
                else if (i.ToDo == 1)
                {
                    Texts[ToDoCount].text += $" 삭제";
                    cnt = "";
                }
                else
                {
                    Texts[ToDoCount].text += $" 변경<size=20>\n\n {i.Normal} > {i.Revise}</size>";
                    cnt = $"{i.Normal} > {i.Revise}";
                }
                TextM.DocsAnsLine[i.Line] = true;
                TextM.DocsChange[i.Line] = i.Goal;
                ToDoIndexes[1].Add(new ToDoIndex(ToDoCount, i.ToDo, $"{i.Line+1}번째 줄 {sub2[i.ToDo]}", cnt,i.Line,i.Goal));
                Texts[ToDoCount].gameObject.SetActive(true);
                ToDoCount++;
            }
        //gameObject.SetActive(false);
    }

    string[] sub3 = { "News", "Docs", "Info" };

    public void CheckList(int type, int line, bool Clear, int DoLine = -1)
    {
        print(Clear);
        ToDoIndex k = null;
        if (DoLine == -1) { foreach (var s in ToDoIndexes[type]) if (s.goalLine == line) k = s; }
        else k = ToDoIndexes[type][DoLine];
        if (Clear)
            Texts[k.line].text = $"<s><color={DRed}>{sub3[type]}</color><color={DWhite}> {k.line1}<size=20>\n\n {k.line2}</color></size></s>";
        else
            Texts[k.line].text = $"<color={NRed}>{sub3[type]}</color><color={NWhite}> {k.line1}<size=20>\n\n {k.line2}</color></size>";
    }
}
