using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    /**
    상호작용 오브젝트
    - 버튼 클릭시 해당 파라미터로 상호작용
    */

    public int param = 0;
    
    public ChatTrigger chatTrigger;


    /// <summary>
    /// NPC 초기화
    /// </summary>
    public void onAwake()
    {
        chatTrigger = gameObject.AddComponent<ChatTrigger>();
        
        foreach (var _data in data.chatData)
        {
            chatTrigger.chatAssets.Add(_data.chat);
        }
        // TODO: 로딩씬으로 빼기, 
        chatTrigger.LoadChatData();
        SetPosition();

        param = 0;
    }

    
    public void OnEnable()
    {
        if (data.chatData[param].onAwake)
        {
            chatTrigger.Init(param);
        }
    }


    public void OnClicked()
    {
        chatTrigger.Init(param);
    }


    private void SetPosition()
    {
        // 위치 설정
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = data.anchor;
        rect.anchorMax = data.anchor;
        
        // 크기 설정
        rect.sizeDelta *= data.size;
        rect.localScale = new Vector3(1f,1f,1f);
    }
}
