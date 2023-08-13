using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessengerTab : MonoBehaviour
{
    [SerializeField] Messenger_M MM;
    [SerializeField] TMP_Text From;
    [SerializeField] TMP_Text Title;
    [SerializeField] MessengerText MT;
    Image image;
    string from;
    string main;
    string title;
    GameObject[] includes;
    string[] includesname;
    bool IsOpen;
    WaitForSeconds wfs = new WaitForSeconds(0.01f);

    Color N = new Color(0.8f,0.8f,0.8f);
    Color Y = new Color(0.95f,0.95f,0.95f);

    private void Awake()
    {
        image = GetComponent<Image>();
        MyUi.AddEvent(GetComponent<EventTrigger>(),EventTriggerType.PointerClick,Click);
    }

    public void NewTab(string from, string title, string main, GameObject[] includes, string[] includesname)
    {
        gameObject.SetActive(true);
        this.from = from;
        this.title = title;
        this.main = main;
        this.includes = includes;
        this.includesname = includesname;
        From.text = from;
        Title.text = title;
    }

    void Click(PointerEventData Data)
    {
        if (Data.clickCount == 2)
        {
            MM.CloseMessage();
            MT.gameObject.SetActive(false);
            MT.gameObject.SetActive(true);
            MT.OpenContent(from,title,main,includes,includesname);
            StartCoroutine(UpdateCanvas());
            image.color = Y;
        }
    }

    IEnumerator UpdateCanvas()
    {
        yield return wfs;
        MT.gameObject.SetActive(false);
        MT.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        image.color = N;
        IsOpen = false;
    }

}
