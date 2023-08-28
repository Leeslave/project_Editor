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
    int ActiveTab = 0;
    List<Tab> TabInf;

    // 메신저를 열었을 때의 창 관련
    [SerializeField] GameObject MessengerText;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Sender;
    [SerializeField] TMP_Text Attatch;
    [SerializeField] GameObject[] Attatchs;
    [SerializeField] TMP_Text[] AttatchName;
    [SerializeField] Image[] AttatchImage;
    [SerializeField] TMP_Text Main;

    class Tab
    {
        public string from;
        public string title;
        public string main;
        public GameObject[] processes;
        public Image[] Images;
        public string[] includesname;
        public void SetValues(string from, string title, string main, GameObject[] processes, Image[] Images, string[] includesname)
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
        TabInf = new List<Tab>(Tabs.Length);
    }

    public void NewMessage(string from, string title, string main, GameObject[] processes,Image[] Images, string[] includesname)
    {
        Tabs[ActiveTab].NewTab(from,title);
        TabInf[ActiveTab].SetValues(from, title, main, processes, Images, includesname);
        MI.ChangeCount(1);
    }

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
    IEnumerator UpdateCanvas()
    {
        yield return wfs;
        MessengerText.SetActive(true);
        MessengerText.SetActive(false);
        MessengerText.SetActive(true);
    }

}
