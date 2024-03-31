using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCData : WorldObjectData
{
    [HideInInspector]
    public string clickParam = "";   // Click 함수 매개변수
    [HideInInspector]
    public Vector2 anchor;  // 위치
    public float size;    // 크기
    public List<Paragraph> awakeTalk = new();    // 자동 대사 데이터
    public List<Paragraph> clickTalk = new();    // 터치 대사 데이터 (배열로 해서 여러 데이터)
    [HideInInspector]
    public int talkCount = 0;   // 대화 횟수 카운트
}
