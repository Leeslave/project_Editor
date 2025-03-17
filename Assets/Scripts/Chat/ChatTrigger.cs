using System.Collections.Generic;
using UnityEngine;


public interface IChatList
{
    public void SwapIndex(int idx);
    public void StartChat();
}

public class ChatTrigger : MonoBehaviour
{
    /**
     * 대사를 출력하는 컴포넌트
     * - 필요한 대사 목록으로 데이터를 미리 로드
     * - index를 통해 대사를 선택하여 출력 가능
     */
    
    public string chatAsset;

    [SerializeField] 
    private List<TalkParagraph> chatText;
    private List<Paragraph> chatData = new();
    
    
    public void Awake()
    {
        if (chatText is null || chatText.Count == 0)
        {
            return;
        }
        chatData.AddRange(chatText);
    }
    
    
    /// <summary>
    /// 대화 파일 읽어오기
    /// </summary>
    public void LoadChatData()
    {
        chatData = new();
        chatData = DataLoader.GetChatData(chatAsset);
    }


    /// <summary>
    /// Start Chat
    /// </summary>
    /// <param name="idx">chat data index</param>
    public void StartChat()
    {
        if (chatData is null)
        {
            Debug.LogWarning("Failed to Load Chat Data");
            return;
        }
              
        Chat.Instance.StartChat(gameObject, chatData);
    }
}
