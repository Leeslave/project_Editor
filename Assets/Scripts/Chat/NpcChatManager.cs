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
    private string chatFile;
    private Chat chatObject;

    private void Awake() {
        chatObject = GameObject.FindObjectOfType<Chat>();
    }

    /// 일반 대화 함수
    public void OnNormalChat()
    {
        OnChatStart();
    }

    /// 시간/날짜 넘김 함수
    public void OnTimeChat()
    {
        
    }

    /// 대화 연동 함수
    private void OnChatStart(Action CallBackFunc = null) {
        // 날짜/NPC이름_시간.csv
        chatFile = $"{PlayerDataManager.Instance.playerData.index.ToString()}/{NPCName}_{PlayerDataManager.Instance.playerData.time.ToString()}";
        Debug.Log($"Chat Start: {chatFile}");
        if (chatFile != "")
        {
            chatObject.LoadData(chatFile);
            chatObject.LoadLine(1);
        }
    }
}
