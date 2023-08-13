using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class Messenger_M : MonoBehaviour
{
    [SerializeField] MessengerIcon MI;
    [SerializeField] MessengerTab[] Tabs;
    int ActiveTab = 0;

    private void Awake()
    {
        Invoke("Test1",1);
        Invoke("Test", 5);
    }
    public void NewMessage(string from, string title, string main, GameObject[] includes, string[] includesname)
    {
        Tabs[ActiveTab++].NewTab(from,title,main,includes,includesname);
        MI.ChangeCount(1);
    }
    public void CloseMessage()
    {
        MI.ChangeCount(-1);
    }

    [SerializeField] GameObject test1;
    [SerializeField] GameObject test2;

    void Test1()
    {
        NewMessage(
            "������","���� ���� ���û���",
            "���� ���� ���û����Դϴ�.\n���� 16�ñ��� ó�� �ٶ��ϴ�.",
            new GameObject[] { test1, test2 },
            new string[] {"�λ���Ʈ ����","���� ����"}
            );
    }

    void Test()
    {
        if (ActiveTab < Tabs.Length) Invoke("Test", Random.Range(1, 10));
        NewMessage($"{Random.Range(1,10)}", "!", "??", new GameObject[2], new string[] { "Test1", "Test2"});
    }
}
