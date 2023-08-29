using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messenger_M : MonoBehaviour
{
    [SerializeField] Windows_M WM;
    [SerializeField] MessengerIcon MI;
    [SerializeField] MessengerTab[] Tabs;
    // Ȱ��ȭ �Ǿ� �ִ� Tab�� ��
    int ActiveTab = 0;
    // �� �޼����� ������ ��� Class��
    Tab[] TabInf;

    // �޽����� ������ ���� â ����
    [SerializeField] GameObject MessengerText;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Sender;
    [SerializeField] TMP_Text Attatch;
    [SerializeField] GameObject[] Attatchs;
    [SerializeField] TMP_Text[] AttatchName;
    [SerializeField] Image[] AttatchImage;
    [SerializeField] TMP_Text Main;


    /// <summary>
    /// �� �޼����� ������ ��� Class
    /// �۽���, ����, ����, ÷������ ���� ������ ������ �ִ�.
    /// </summary>
    class Tab
    {
        public string from;
        public string title;
        public string main;
        public GameObject[] processes;
        public Image[] Images;
        public string[] includesname;
        public Tab(string from, string title, string main, GameObject[] processes, Image[] Images, string[] includesname)
        {
            this.from = from;
            this.title = title;
            this.main = main;
            this.processes = processes;
            this.Images = Images;
            this.includesname = includesname;
        }
    }

    int CurOpened;

    private void Awake()
    {
        TabInf = new Tab[Tabs.Length];
    }

    private void Start()
    {
        StartCoroutine(Tester());
    }

   IEnumerator Tester()
    {
        for(int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            Test1();
        }
    }
    [Header("ForTest")]
    [SerializeField] Sprite[] TestSprite;
    [SerializeField] string[] TestName;
    void Test1()
    {
        int a = Random.Range(0,TestSprite.Length);
        GameObject cnt = WM.NewIcon(true, null, TestName[a], TestSprite[a]);
        cnt.SetActive(false);
        NewMessage(TestName[a], "Test","Test", new GameObject[] {null}, new Image[] { cnt.transform.GetChild(0).GetComponent<Image>() }, new string[] { TestName[a] }) ;
    }

    /// <summary>
    /// ���ο� �޼��� Tab�� �߰��Ѵ�.
    /// </summary>
    /// <param name="from"> �۽��� </param>
    /// <param name="title"> ���� </param>
    /// <param name="main"> ���� ���� </param>
    /// <param name="processes"> ÷�ε� ������ ���� �ϴ� ���α׷��� </param>
    /// <param name="Images"> ÷�ε� ������ �̹����� </param>
    /// <param name="includesname"> ÷�ε� ������ �̸��� </param>

    public void NewMessage(string from, string title, string main, GameObject[] processes,Image[] Images, string[] includesname)
    {
        Tabs[ActiveTab].NewTab(from,title);
        TabInf[ActiveTab++] = new Tab(from, title, main, processes, Images, includesname);
        MI.ChangeCount(1);
    }

    /// <summary>
    /// �޽��� ����â�� ����.
    /// MessengerTab.cs���� ȣ��ȴ�.
    /// </summary>
    /// <param name="ind"> ���� Tab�� Index </param>
    public void OpenMessage(int ind)
    {
        CurOpened = ind;
        if (Tabs[ind].IsOpened == false) MI.ChangeCount(-1);
        for (int i = 0; i < TabInf[ind].processes.Length; i++)
        {
            Attatchs[i].SetActive(true);
            AttatchImage[i].sprite = TabInf[ind].Images[i].sprite;
            AttatchName[i].text = TabInf[ind].includesname[i];
        }
        for (int i = TabInf[ind].processes.Length; i < Attatchs.Length; i++) Attatchs[i].SetActive(false);
        Title.text = TabInf[ind].title;
        Sender.text = TabInf[ind].from;
        Attatch.text = $"÷�� {TabInf[ind].processes.Length}��";
        Main.text = TabInf[ind].main;
        StartCoroutine(UpdateCanvas());
    }

    /// <summary>
    /// ÷�� ����(Icon)�� Window�� �߰��Ѵ�.
    /// �޼��� ����â�� ÷�� �ʵ��� ��ư�� ����Ǿ� ����Ѵ�.
    /// </summary>
    /// <param name="ind"> �� ��° ÷�� �������� </param>
    public void AddIncludes(int ind)
    {
        WM.NewIcon(
            true,
            TabInf[CurOpened].processes[ind],
            TabInf[CurOpened].includesname[ind],
            TabInf[CurOpened].Images[ind].sprite
            );
    }

    WaitForSeconds wfs = new WaitForSeconds(0.01f);

    /// <summary>
    /// ��� LayOut ��ȭ ������ ���� Object�� �ѹ� ���� Ų��.
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateCanvas()
    {
        yield return wfs;
        MessengerText.SetActive(true);
        MessengerText.SetActive(false);
        MessengerText.SetActive(true);
    }

    bool IsOpen = true;
    [SerializeField] Transform Messenger;
    [SerializeField] TMP_Text ButtonText;

    /// <summary>
    /// ��ư Ŭ�� ��, �޽��� �ּ�ȭ, �ִ�ȭ.
    /// �ٸ� ��ư�� ������Ѽ� ����Ѵ�.
    /// </summary>
    public void ChangeMessenger()
    {
        if (IsOpen)StartCoroutine(MoveMessenger(-780,false));
        else StartCoroutine(MoveMessenger(-180,true));
        IsOpen = IsOpen == false;
    }
    IEnumerator MoveMessenger(float Goal,bool dy)
    {
        if (dy)
        {
            for (; Messenger.position.y < -180; Messenger.Translate(0, 100, 0)) yield return new WaitForSeconds(0.01f);
            ButtonText.text = "��";
        }
        else
        {
            for (; Messenger.position.y > -780; Messenger.Translate(0, -100, 0)) yield return new WaitForSeconds(0.01f);
            ButtonText.text = "��";
        }
            
    }
}
