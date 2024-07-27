using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToDoList_N : MonoBehaviour
{
    [SerializeField] TMP_Text[] Texts;

    string NRed = "#FF0000";
    string DRed = "#800000";
    string NWhite = "#FFFFFF";
    string DWhite = "#808080";

    // 현재 지시사항의 수
    int ToDoCount = 0;

    public class ToDoIndex
    {
        public int line;        // 현재 ToDoList에서 몇번째 줄인지
        public int type;        //
        public bool IsClear;    // 수행된 지시사항인지
        public string line1;    // 대략적인 지시사항 (ex) Clover 수정 > 국적)
        public string line2;    // 세부적인 지시사항 (ex) 아슬라니아 -> 주렌)
        public int goalLine;    // 정답의 줄 (News에서만 사용)
        public string goal;     // 정답 String (News에서만 사용)
        public ToDoIndex(int line, int type, string line1, string line2, int goalLine, string goal)
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
    [HideInInspector] public List<List<ToDoIndex>> ToDoIndexes;
    // ToDoList 문장 구성용 임시 배열
    string[] sub = { "국적", "직업", "부서", "소속", "얼굴" };
    string[] sub2 = { "추가", "삭제", "변경" };
    private void Start()
    {
        ToDoIndexes = new List<List<ToDoIndex>>(3);
        for (int i = 0; i < 3; i++) ToDoIndexes.Add(new List<ToDoIndex>());

        string cnt = "";

        // 인물 정보 수정 지시사항 ToDoList 추가
        foreach (var i in DB_M.DB_Docs.Instructions.InfoInst)
        {
            Texts[ToDoCount].text = $"<color={NRed}>Info</color> {i.Target} {sub[i.ToDo]} 변경";
            if (i.ToDo != 4) cnt = $"{DB_M.DB_Docs.InfSub[i.ToDo][i.Before].Trim('\r')} > {DB_M.DB_Docs.InfSub[i.ToDo][i.After].Trim('\r')}";
            else cnt = $"Face{i.Before} > Face{i.After}";

            Texts[ToDoCount].text += $"<size=20>\n\n{cnt}</size>";

            DB_M.DB_Docs.PersonDataManager.PeopleCorrect.Add(new Tuple<string, int, int, int>(i.Target, i.ToDo, i.After, ToDoCount));
            ToDoIndexes[2].Add(new ToDoIndex(ToDoCount, i.ToDo, $"{i.Target} {sub[i.ToDo]} 변경", cnt, 0, $"{i.After}"));
            Texts[ToDoCount].gameObject.SetActive(true);
            ToDoCount++;
        }

        // 뉴스 수정 지시사항 ToDoList 추가
        foreach (var i in DB_M.DB_Docs.Instructions.NewsInst)
        {
            Texts[ToDoCount].text =
                $"<color=#FF0000>News</color> {i.Line + 1}번째 줄";
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
            DB_M.DB_Docs.NewsManager.NewsAnsLine[i.Line] = true;
            DB_M.DB_Docs.NewsManager.NewsChange[i.Line] = i.Goal;
            ToDoIndexes[0].Add(new ToDoIndex(ToDoCount, i.ToDo, $"{i.Line + 1}번째 줄 {sub2[i.ToDo]}", cnt, i.Line, i.Goal));
            Texts[ToDoCount].gameObject.SetActive(true);
            ToDoCount++;
        }

        // 문서 수정 지시사항 ToDoList 추가
        foreach (var i in DB_M.DB_Docs.Instructions.DocsInst)
        {
            Texts[ToDoCount].text = $"<color=#FF0000>Docs</color> {i.Name}의 취조록 조사";
            ToDoIndexes[1].Add(new ToDoIndex(ToDoCount, 0, i.Name, "", 0, ""));
            Texts[ToDoCount].gameObject.SetActive(true);
            ToDoCount++;
        }

        gameObject.SetActive(false);
    }


    // ToDoList 문장 구성용 임시 배열
    string[] sub3 = { "News", "Docs", "Info" };

    /// <summary>
    /// 업무 수행 사항에 따라 CheckList내 문장 수정
    /// </summary>
    /// <param name="type">0 : 뉴스, 1 : 문서, 2 : 인물</param>
    /// <param name="line">해당 수행 사항이 적용된 줄(News만 사용)</param>
    /// <param name="Clear">클리어 여부</param>
    /// <param name="DoLine">인물 정보 수정에서 해당 인물의 몇 번째 수정 사항인지</param>
    public void CheckList(int type, int line, bool Clear, int DoLine = -1)
    {
        ToDoIndex k = null;
        if (DoLine == -1) { foreach (var s in ToDoIndexes[type]) if (s.goalLine == line) k = s; }
        else k = ToDoIndexes[type][DoLine];
        if (type != 1)
        {
            if (Clear)
                Texts[k.line].text = $"<s><color={DRed}>{sub3[type]}</color><color={DWhite}> {k.line1}<size=20>\n\n {k.line2}</color></size></s>";
            else
                Texts[k.line].text = $"<color={NRed}>{sub3[type]}</color><color={NWhite}> {k.line1}<size=20>\n\n {k.line2}</color></size>";
        }
        else
        {
            if (Clear)
                Texts[k.line].text = $"<s><color={DRed}>{sub3[type]}</color><color={DWhite}> {k.line1}의 취조록 조사</color></s>";
            else
                Texts[k.line].text = $"<color={NRed}>{sub3[type]}</color><color={NWhite}> {k.line1}의 취조록 조사</color></size>";
        }
    }
}
