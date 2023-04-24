using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcChatManager : MonoBehaviour
{
    /**
    *   NPC 대화 매니저
    *   - 대화csv 파일명 + 날짜로 해당 대화 데이터 불러오기
    *   - 버튼을 눌러 대화 활성화
    *   - 대화 횟수 or 대화 여부 저장
    */

    public string NPCName = "";
    public int talkCount = 0;
    private string chatFileName;
    private Chat chatObject;

    private void Awake() {
        chatObject = GameObject.FindObjectOfType<Chat>();
    }

    /// <summary>
    /// 대화 시작 이벤트 함수
    /// </summary>
    public void OnChatStart(Action CallBackFunc = null) {
        // 대화 
        if (chatFileName != "")
            chatObject.LoadData(chatFileName);
        chatObject.LoadLine(1);
    }
}
