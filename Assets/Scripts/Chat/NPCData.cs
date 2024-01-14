using System;
using System.Collections.Generic;
using UnityEngine;

/// JSON 파싱 클래스 : NPCData 기본값
[Serializable]
public class NPCWrapper
{
    public string name;
    public string image = null;
    public int position = 0;
    public Vector2 anchor = new Vector2(0,0);
    public Vector2 size = new Vector2(100f,200f);
    public string triggerType = "Click";
    public int count = 0;
    public List<Paragraph> chatList = new();    // 대화
    public string action = null;
    public string actionParam = null;
}

public class NPCData
{
    public string name;     // 이름
    public string image;    // 오브젝트 이미지
    public string triggerType;  // NPC 타입
    public int count;   // 대화 가능 횟수
    public Vector2 size;          // 오브젝트 크기
    public int position;   // 지역 내 장소
    public Vector2 anchor;    // 장소 내 위치
    public List<Paragraph> chatList;    // 대화
    public ChatAction action;   // 반응


    public NPCData(NPCWrapper wrapper)
    {
        name = wrapper.name;
        chatList = wrapper.chatList;
        action = Chat.GetAction(wrapper.action, wrapper.actionParam);
        triggerType = wrapper.triggerType;
        count = wrapper.count;
        image = wrapper.image;
        // 다이얼로그 대화 시 종료
        if (image == null)
        {
            return;
        }

        // 오브젝트 장소 추가
        position = wrapper.position;
        size = wrapper.size;
    }
}