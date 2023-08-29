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
    // 활성화 되어 있는 Tab의 수
    int ActiveTab = 0;
    // 각 메세지의 정보를 담는 Class들
    Tab[] TabInf;

    // 메신저를 열었을 때의 창 관련
    [SerializeField] GameObject MessengerText;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Sender;
    [SerializeField] TMP_Text Attatch;
    [SerializeField] GameObject[] Attatchs;
    [SerializeField] TMP_Text[] AttatchName;
    [SerializeField] Image[] AttatchImage;
    [SerializeField] TMP_Text Main;


    /// <summary>
    /// 각 메세지의 정보를 담는 Class
    /// 송신인, 제목, 본문, 첨부파일 관련 정보를 가지고 있다.
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
    /// 새로운 메세지 Tab을 추가한다.
    /// </summary>
    /// <param name="from"> 송신인 </param>
    /// <param name="title"> 제목 </param>
    /// <param name="main"> 본문 내용 </param>
    /// <param name="processes"> 첨부된 파일이 실행 하는 프로그램들 </param>
    /// <param name="Images"> 첨부된 파일의 이미지들 </param>
    /// <param name="includesname"> 첨부된 파일의 이름들 </param>

    public void NewMessage(string from, string title, string main, GameObject[] processes,Image[] Images, string[] includesname)
    {
        Tabs[ActiveTab].NewTab(from,title);
        TabInf[ActiveTab++] = new Tab(from, title, main, processes, Images, includesname);
        MI.ChangeCount(1);
    }

    /// <summary>
    /// 메신저 세부창을 연다.
    /// MessengerTab.cs에서 호출된다.
    /// </summary>
    /// <param name="ind"> 현재 Tab의 Index </param>
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
        Attatch.text = $"첨부 {TabInf[ind].processes.Length}개";
        Main.text = TabInf[ind].main;
        StartCoroutine(UpdateCanvas());
    }

    /// <summary>
    /// 첨부 파일(Icon)을 Window에 추가한다.
    /// 메세지 세부창의 첨부 필드의 버튼에 연결되어 사용한다.
    /// </summary>
    /// <param name="ind"> 몇 번째 첨부 파일인지 </param>
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
    /// 모든 LayOut 변화 적용을 위해 Object를 한번 껏다 킨다.
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
    /// 버튼 클릭 시, 메신저 최소화, 최대화.
    /// 다른 버튼에 연결시켜서 사용한다.
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
            ButtonText.text = "▽";
        }
        else
        {
            for (; Messenger.position.y > -780; Messenger.Translate(0, -100, 0)) yield return new WaitForSeconds(0.01f);
            ButtonText.text = "△";
        }
            
    }
}
