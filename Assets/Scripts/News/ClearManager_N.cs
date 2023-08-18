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
            AF.AttatchFail("���� ����","�������� �ʴ� �ּ�");
            return;
        }
        if (Title.text != "���� ����")
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "���� ���� ���� : -5";
            HP -= 5;
        }
        if (Main.text != "���� ���� �����Դϴ�.")
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "���� ���� ���� : -5";
            HP -= 5;
        }
        if (IsGoalAttatched != 1)
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = "���� ÷�� ���� ���� : -5";
            HP -= 5;
        }
        if (IsGoalAttatched == 0)ReviseScore = MN.MaxHealth;
        else ReviseScore = MN.TryCount - MN.Health;
        if (ReviseScore != 0) 
        {
            Reports[ErrorCount].SetActive(true);
            ReportsText[ErrorCount++].text = $"���� ������ : -{10 * ReviseScore}";
            HP -= 10 * ReviseScore;
            StartCoroutine(SendReport(Title.text, -1));
        }
        else
        {
            StartCoroutine(SendReport(Title.text, ErrorCount));
        }
        Reports[ErrorCount].SetActive(true);
        ReportsText[ErrorCount].text = $"�� : {HP}";

        
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReportRect);
    }

    [SerializeField] GameObject test1;
    [SerializeField] GameObject test2;
    [SerializeField] GameObject test3;

    IEnumerator GS()
    {
        yield return new WaitForSeconds(5);
        MM.NewMessage(
            "������", "���� ���� ���û���",
            $"���� ���� ���û����Դϴ�.\n���� 16�ñ��� ó�� �ٶ��ϴ�.\n{Email}",
            new GameObject[] { test1, test2 },
            new string[] { "����", "���� ����" }
            );
    }

    IEnumerator SendReport(string cnt,int ErrorCount)
    {
        yield return new WaitForSeconds(5);
        if (ErrorCount < 0)
        {
            MM.NewMessage("������", $"Re:{cnt}", "���û��׿� ���� ���� ������ �ٶ��ϴ�.",
                new GameObject[] { test3 },
                new string[] { "�����" }
                );
        }
        else if(ErrorCount > 0)
        {
            MM.NewMessage("������", $"Re:{cnt}", "�����ϼ̽��ϴ�.\n�������� ���� �ؼ� �ٶ��ϴ�.",
                new GameObject[] { test3 },
                new string[] { "�����" }
                );
        }
        else
        {
            MM.NewMessage("������", $"Re:{cnt}", "�����ϼ̽��ϴ�.",
                new GameObject[0],
                new string[0]
                );
        }
    }
}
