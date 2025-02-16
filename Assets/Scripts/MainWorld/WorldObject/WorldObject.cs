using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldObject : MonoBehaviour
{
    /**
    상호작용 오브젝트
    - 버튼 클릭시 해당 파라미터로 상호작용
    */
    public List<(WorldVector worldVector, Anchor anchor)> positions;
    public List<(string chat, bool onAwake)> chatAssets;
    
    public int positionParam;
    public int chatParam;
    
    public ChatTrigger chatTrigger;
    

    /// <summary>
    /// NPC 초기화
    /// </summary>
    /// <remarks>로딩씬에 포함</remarks>
    public void OnAwake()
    {
        gameObject.AddComponent<ChatTrigger>();
        
        foreach (var asset in chatAssets)
        {
            chatTrigger.chatAssets.Add(asset.chat);
        }
        chatTrigger.LoadChatData();
        
        SetAnchor();

        positionParam = 0;
        chatParam = 0;
    }

    
    // Awake 대사시, 실행
    public void OnEnable()
    {
        if (chatAssets[chatParam].onAwake)
        {
            chatTrigger.Init(chatParam);
        }
    }


    // 현재 대사 실행
    public void OnClicked()
    {
        chatTrigger.Init(chatParam);
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
