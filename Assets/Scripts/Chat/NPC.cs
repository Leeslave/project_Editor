using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine;

public class NPC : MonoBehaviour
{   
    /**
    대사 실행 오브젝트
    - 타입에 따라 타이밍 맞춰 대사 실행
    */

    public NPCData data;    // NPC 데이터
    public int talkCount = 0;   // 대화 횟수 카운트

    void OnEnable()
    {
        if (data == null)
        {
            return;
        }
        
        // 활성화마다 실행
        if (data.triggerType == "Auto")
        {
            StartChat();
        }
    }
    
    /// <summary>
    /// 대화 시작
    /// </summary>
    public void StartChat()
    {
        if (data == null)
        {
            Debug.Log($"NPC DATA CANNOT FOUND");
            return;
        }

        // 무제한 or 대화 횟수 제한
        if (data.count > 0 && talkCount >= data.count)
            return;
        
        Debug.Log($"Start Chat : {data.name}");
        Chat.Instance.StartChat(data.chatList);
        talkCount++;
    }
}
