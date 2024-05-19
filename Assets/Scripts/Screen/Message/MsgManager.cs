using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MsgManager : Singleton<MsgManager>
{
    [Header("사용자 정보")]
    private int currentMessage;

    private string currentUser = "클레이튼";
    [Header("메시지 리스트")]
    private List<MessageData> messageDatas = new();

    public int panelSize;
    public RectTransform MsgListPanel;
    public GameObject MsgPrefab;

    [Header("메시지")]
    public List<Paragraph> msgs = new();
    public RectTransform MsgPanel;
    public GameObject msgPrefab_L;
    public GameObject msgPrefab_R;
    private int messageIndex;
    public GameObject BackButton;
    
    
    public new void Awake()
    {
        base.Awake();
        GetMessages();

        // 메시지 패널 생성
        for(int i = 0; i < messageDatas.Count; i++)
        {
            Debug.Log(messageDatas.Count);
            GameObject newPanel = Instantiate(MsgPrefab, MsgListPanel);
            newPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = messageDatas[i].name;
            MsgListPanel.sizeDelta += new Vector2(0, panelSize);
            newPanel.GetComponent<MessagePanel>().count = i;
        }
    }


    /// <summary>
    /// 새 메시지 패널 대화시작
    /// </summary>
    /// <param name="idx"></param>
    public void SetMessagePanel(int idx)
    {
        ClearMessage();
        messageIndex = 0;
        currentMessage = idx;

        msgs = DataLoader.GetChatData(messageDatas[currentMessage].awakeParam);
        
        NextMessage();
    }

    
    /// <summary>
    /// 다음 메시지 출력
    /// </summary>
    public void NextMessage()
    {
        // 마지막 메시지 확인
        if (messageIndex >= msgs.Count)
        {
            FinishMessage();
            return;
        }
        
        if (msgs[messageIndex] is not TalkParagraph)
            return;
        // 메시지 오브젝트 생성
        GameObject newMsg;
        TalkParagraph newParagraph = msgs[messageIndex] as TalkParagraph;
        if (newParagraph.talker == currentUser)
        {
            newMsg = Instantiate(msgPrefab_R, MsgPanel);
        }
        else
        {
            newMsg = Instantiate(msgPrefab_L, MsgPanel);
        }
        MsgPanel.sizeDelta += new Vector2(0, panelSize);
        
        // 메시지 오브젝트 실행
        MessageBlock newBlock = newMsg.GetComponent<MessageBlock>();
        newBlock.data = newParagraph;
        
        if (newParagraph.talker != currentUser)
        {
            newBlock.SetText();
        }

        messageIndex++;
    }

    
    /// <summary>
    /// 메시지 완료
    /// </summary>
    public void FinishMessage()
    {
        BackButton.SetActive(true);
    }

    
    /// <summary>
    /// 메시지 패널 초기화
    /// </summary>
    private void ClearMessage()
    {
        for (int i = 0; i < MsgPanel.childCount; i++)
        {
            Destroy(MsgPanel.GetChild(i).gameObject);
        }

        MsgPanel.sizeDelta -= new Vector2(0, MsgPanel.sizeDelta.y);
        MsgPanel.position = new Vector3(MsgPanel.position.x, 0, MsgPanel.position.z);
    }
    
    
    /// <summary>
    /// 메시지 불러오기
    /// </summary>
    public void GetMessages()
    {
        messageDatas.Clear();

        messageDatas = ObjectDatabase.MessageList;
    }
}
