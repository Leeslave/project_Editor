using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour, IChatList
{
    /**
    상호작용 오브젝트
    - 버튼 클릭시 해당 파라미터로 상호작용
    */
    public List<(WorldVector worldVector, Anchor anchor)> positions;
    public List<(string chat, bool onAwake)> chatAssets = new();
    
    public int positionParam;
    public int chatParam;
    
    private List<ChatTrigger> _chats = new();
    private IChatList _chatListImplementation;


    /// <summary>
    /// NPC 초기화
    /// </summary>
    /// <remarks>로딩씬에 포함</remarks>
    public void OnAwake()
    {
        // Trigger들 초기화
        foreach (ChatTrigger chat in _chats)
        {
            Destroy(chat);
        }
        
        foreach (var asset in chatAssets)
        {
            ChatTrigger newTrigger = gameObject.AddComponent<ChatTrigger>();
            newTrigger.chatAsset = asset.chat;
            _chats.Add(newTrigger);
            
            // TODO: 대사 로드 비동기 처리
            newTrigger.LoadChatData();
        }
        
        SetAnchor();

        positionParam = 0;
        chatParam = 0;
    }
    
    public void SwapIndex(int idx)
    {
        chatParam = idx;
    }
    
    public void StartChat()
    {
        _chats[chatParam].StartChat();
    }

    
    // Awake 대사시, 실행
    public void OnEnable()
    {
        if (chatAssets[chatParam].onAwake)
        {
            StartChat();
        }
    }


    // 현재 대사 실행
    public void OnClicked()
    {
        StartChat();
    }


    /// <summary>
    /// 위치값에 맞춰 앵커 설정
    /// </summary>
    private void SetAnchor()
    {
        var _data = positions[positionParam];
        
        // 위치 설정
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = _data.anchor.GetVector();
        rect.anchorMax = _data.anchor.GetVector();
        
        // 크기 설정
        rect.sizeDelta *= _data.anchor.size;
        rect.localScale = new Vector3(1f,1f,1f);
    }

}
