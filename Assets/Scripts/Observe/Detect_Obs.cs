using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Detect_Obs : MonoBehaviour
{
    [SerializeField] string Target;
    [SerializeField] GameObject ErrorMessage;
    [SerializeField] TMP_Text[] LogTexts;
    [SerializeField] Timer_Obs Timer;
    [SerializeField] RectTransform LogTrans;
    [SerializeField] public TMP_Text NameMessage;
    [SerializeField] GameObject[] Chat;
    RectTransform[] ChatForm;
    TMP_Text[] ChatText;
    int TextsInd = 0;
    [NonSerialized] public bool OnEnter = false;
    [NonSerialized] public string[] Chats;
    [NonSerialized] public string[] Actions;
    [NonSerialized] public string Name;
    [NonSerialized] public Transform CurTarget;
    float Hor;
    float Ver;
    private void Awake()
    {
        ChatForm = new RectTransform[Chat.Length];
        ChatText = new TMP_Text[Chat.Length];
        for(int i = 0; i < Chat.Length; i++)
        {
            ChatForm[i] = Chat[i].GetComponent<RectTransform>();
            ChatText[i] = ChatForm[i].GetChild(0).GetComponent<TMP_Text>();
        }
    }

    void Update()
    {
        Hor = Input.GetAxisRaw("Horizontal");
        Ver = Input.GetAxisRaw("Vertical");
        if (Hor * transform.position.x > 520) Hor = 0;
        if (Ver * (transform.position.y-30) > 340) Ver = 0;


        transform.Translate(new Vector3(Hor*1.5f,Ver*1.5f,0));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(OnEnter)
            {
                if (Name == Target)
                {
                    LogTexts[TextsInd].text = Timer.ReturnTime();
                    LogTexts[TextsInd++].gameObject.SetActive(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(LogTrans);
                }
                else
                {
                    if (ErrorMessage.activeSelf) ErrorMessage.SetActive(false);
                    ErrorMessage.SetActive(true);
                }
            }
        }
    }


    Vector3 CntPos = new Vector3(0, 1.5f, 0);
    public void MakeNoise()
    {
        if (!OnEnter) return;
        if (CurTarget.childCount == 0) for (int i = 0; i < Chat.Length; i++){ if (!Chat[i].activeSelf)
                {
                    Chat[i].SetActive(true);
                    Chat[i].transform.SetParent(CurTarget);
                    ChatForm[i].anchoredPosition = CntPos;
                    ChatText[i].text = Chats[0];
                    break;
                }}
        else for (int i = 0; i < Chat.Length; i++) {  if (Chat[i] == CurTarget.GetChild(0).gameObject)
                {
                    Chat[i].SetActive(false);
                    Chat[i].SetActive(true);
                    ChatText[i].text = Chats[0];
                    break;
                }}
    }
}
