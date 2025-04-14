using System.Collections.Generic;

public class ChatObject : WorldObject, IChatList
{
    // 현재 대사 인덱스
    public int ChatIndex { get; set; }
    // 각 대사들 모음
    public List<(string chat, bool onAwake)> chatAssets { get; set; }
    // 대사 트리거들
    public List<ChatTrigger> Triggers { get; set; }
    
    
    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>각 트리거들 초기화, 초기 index 설정</remarks>
    public override void OnAwake()
    {
        base.OnAwake();
        
        // Trigger들 초기화
        foreach (ChatTrigger chat in Triggers)
        {
            Destroy(chat);
        }
        
        foreach (var asset in chatAssets)
        {
            ChatTrigger newTrigger = gameObject.AddComponent<ChatTrigger>();
            newTrigger.chatAsset = asset.chat;
            Triggers.Add(newTrigger);
            
            // TODO: 대사 로드 비동기 처리
            newTrigger.LoadChatData();
        }
        
        ChatIndex = 0;
    }
    
    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>현재 대사가 onAwake일시 실행</remarks>
    public override void OnEnable()
    {
        if (chatAssets[ChatIndex].onAwake)
        {
            StartChat();
        }
    }


    /// <summary>
    /// 클릭 시
    /// </summary>
    /// <remarks>현재 대사 실행</remarks>
    public override void OnClick()
    {
        StartChat();
    }

    public void SwapIndex(int idx)
    {
        ChatIndex = idx;
    }
    
    public void StartChat()
    {
        Triggers[ChatIndex].StartChat();
    }
}
