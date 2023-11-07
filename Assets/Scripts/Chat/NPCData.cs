using System;
using System.Collections.Generic;
using UnityEngine;

/// JSON 파싱 클래스 : NPCData 기본값
[Serializable]
public class NPCWrapper
{
    public string name;
    public string image = null;
    public string location = null;
    public int locationIndex = 0;
    public Vector2 position = new Vector2(0,0);
    public float size = 1f;
    public string triggerType = "OnClick";
    public List<Paragraph> chatList = new();    // 대화
    public string action = null;
    public string actionParam = null;
}

public class NPCData
{
    // NPC 대화 타입
    public enum TriggerType {   
        OnClick,    // 버튼 누를 시
        OnStart,    // 활성화시 자동 1회
        EveryStart, // 매번 활성화마다
        Once,       // 한번 실행 후 삭제
        GlobalChat, // NPC 없이 배경 한번 실행 후 삭제
    }

    public string name;     // 이름
    public string image;    // 오브젝트 이미지
    public TriggerType triggerType;  // NPC 타입
    public float size;          // 오브젝트 크기
    public World location;  // 월드 내 지역
    public int locationIndex;   // 지역 내 장소
    public Vector2 position;    // 장소 내 위치
    public List<Paragraph> chatList;    // 대화
    public ChatAction action;   // 반응


    public NPCData(NPCWrapper wrapper)
    {
        // 기본 NPC 이름
        name = wrapper.name;  
        // 대화 
        chatList = wrapper.chatList;    
        // 반응함수
        action = Chat.GetAction(wrapper.action, wrapper.actionParam);    

        try
        {
            // NPC 대화 타입 설정
            triggerType = Enum.Parse<TriggerType>(wrapper.triggerType);   // string을 TriggerType로 할당
        }
        catch(ArgumentException)
        {
            // 대화 타입 오류 시 예외처리
            triggerType = TriggerType.OnClick;
        }   
        // 다이얼로그 대화 시 종료
        if (triggerType == TriggerType.GlobalChat)
        {
            return;
        }

        // 오브젝트 장소 추가
        image = wrapper.image;
        locationIndex = wrapper.locationIndex;
        position = wrapper.position;
        size = wrapper.size;
        try
        {
            // 지역, 장소, 위치 설정
            location = Enum.Parse<World>(wrapper.location);   // string을 World로 할당
        }
        catch(ArgumentException)
        {
            // 시작 위치값 오류 시 예외처리
            location = World.Street;
            size = 0f;
        }
    }
}