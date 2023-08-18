using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class ClearManager_N : MonoBehaviour
{
    [SerializeField] TextMannager_N MN;
    [SerializeField] Messenger_M MM;
    [SerializeField] AttatchFile_F AF;
    [SerializeField] TMP_InputField Receive;
    [SerializeField] TMP_InputField Title;
    [SerializeField] TMP_InputField Main;
    [SerializeField] List<GameObject> Reports;
    [SerializeField] List<TMP_Text> ReportsText;
    [SerializeField] RectTransform ReportRect;
    [NonSerialized] public string Email;
    [NonSerialized] public int IsGoalAttatched;
    int ReviseScore = 0;
    int HP = 0;

    private void Awake()
    {
        Email = "";
        int l = Random.Range(7, 10);
        for (int i = 0; i < l; i++) 
        {
            if(Random.Range(0,2) == 0) Email += (char)('a' + Random.Range(0, 26));
            else Email += (char)('A' + Random.Range(0, 26));
        }
        print(Email);
    }

    private void Start()
    {
        StartCoroutine(GS());
    }

    public void JudgeClear()
    {
        int ErrorCount = 0;
        for (int i = 0; i < 5; i++) Reports[i].SetActive(false);

        if(Receive.text != Email + "@aslania.goal")
        {
            AF.AttatchFail("전송 실패","존재하지 않는 주소");
            return;
        }
        if (Title.text != "업무 보고")
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "제목 형식 위반 : -5";
            HP -= 5;
        }
        if (Main.text != "문서 수정 보고입니다.")
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "본문 형식 위반 : -5";
            HP -= 5;
        }
        if (IsGoalAttatched != 1)
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "파일 첨부 형식 위반 : -5";
            HP -= 5;
        }
        if (IsGoalAttatched == 0)ReviseScore = MN.MaxHealth;
        else ReviseScore = MN.TryCount - MN.Health;
        if (ReviseScore != 0) 
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = $"지시 불이행 : -{10 * ReviseScore}";
            HP -= 10 * ReviseScore;
            StartCoroutine(SendReport(Title.text, -1));
        }
        else
        {
            StartCoroutine(SendReport(Title.text, ErrorCount));
        }
        Reports[ErrorCount].SetActive(true);
        ReportsText[ErrorCount].text = $"총 : {HP}";

        
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReportRect);
    }

    [SerializeField] GameObject test1;
    [SerializeField] GameObject test2;
    [SerializeField] GameObject test3;

    IEnumerator GS()
    {
        yield return new WaitForSeconds(5);
        MM.NewMessage(
            "정보부", "금일 업무 지시사항",
            $"금일 업무 지시사항입니다.\n금일 16시까지 처리 바랍니다.\n{Email}",
            new GameObject[] { test1, test2 },
            new string[] { "뉴스", "수정 사항" }
            );
    }

    IEnumerator SendReport(string cnt,int ErrorCount)
    {
        yield return new WaitForSeconds(5);
        if (ErrorCount < 0)
        {
            MM.NewMessage("정보부", $"Re:{cnt}", "지시사항에 맞춰 업무 재이행 바랍니다.",
                new GameObject[] { test3 },
                new string[] { "경고장" }
                );
        }
        else if(ErrorCount > 0)
        {
            MM.NewMessage("정보부", $"Re:{cnt}", "수고하셨습니다.\n다음부턴 형식 준수 바랍니다.",
                new GameObject[] { test3 },
                new string[] { "경고장" }
                );
        }
        else
        {
            MM.NewMessage("정보부", $"Re:{cnt}", "수고하셨습니다.",
                new GameObject[0],
                new string[0]
                );
        }
    }
}
