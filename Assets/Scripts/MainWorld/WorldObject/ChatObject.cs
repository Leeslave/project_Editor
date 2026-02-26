using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class ChatObject : WorldObject, IChatList, IPointerClickHandler
{
    // 현재 대사 인덱스
    public int ChatIndex { get; set; }
    // 각 대사들 모음
    public List<(string chat, bool onAwake)> chatAssets { get; set; }
    
    // 대사 트리거들
    private readonly List<ChatTrigger> _triggers = new();
    
    public ChatTrigger Trigger {
        get
        {
            if (ChatIndex < 0 || ChatIndex > _triggers.Count)
            {
                return null;
            }
            return _triggers[ChatIndex];
        }
    }
    
    
    
    /// <summary>
    /// 초기화 시
    /// </summary>
    /// <remarks>각 트리거들 초기화, 초기 index 설정</remarks>
    public override void Init()
    {
        base.Init();
        
        // Trigger들 초기화
        foreach (ChatTrigger chat in _triggers)
        {
            Destroy(chat);
        }

        // 대사 생성
        ChatIndex = -1;
        if (chatAssets.Count > 0)
        {
            foreach (var asset in chatAssets)
            {
                ChatTrigger newTrigger = gameObject.AddComponent<ChatTrigger>();
                newTrigger.chatAsset = asset.chat;
                _triggers.Add(newTrigger);
            
                // TODO: 대사 로드 비동기 처리
                newTrigger.LoadChatData();
            }
            
            ChatIndex = 0;
        }
    }
    
    /// <summary>
    /// 활성화 시
    /// </summary>
    /// <remarks>현재 대사가 onAwake일시 실행</remarks>
    public override void OnBecameVisible()
    {
        if (ChatIndex >= 0)
        {
            if (chatAssets[ChatIndex].onAwake)
            {
                StartChat();
            }
        }
        
    }


    /// <summary>
    /// 클릭 시
    /// </summary>
    /// <remarks>현재 대사 실행</remarks>
    public void OnPointerClick(PointerEventData eventData)
    {
        StartChat();
    }

    public void SwapIndex(int idx)
    {
        ChatIndex = idx;
    }
    
    public void StartChat()
    {
        if (!Trigger)
        {
            EditorLogger.Log("No Trigger");
            return;
        }
        
        Trigger.StartChat();
    }
}
