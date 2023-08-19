using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    /**
    대사 실행 코드
    - 타입에 따라 타이밍 맞춰 대사 실행
    - 
    */
    public string chatName; // 대화 이름

    public ChatTriggerType triggerType; // 대화 타입
    public int talkCount = 0;   // 대화 횟수 카운트

    void Start()
    {
        if (triggerType == ChatTriggerType.EveryStart)
        {
            StartChat();
        }
        else if (triggerType == ChatTriggerType.OnStart)
        {
            if (talkCount == 0)
            {
                StartChat();
            }
        }
    }

    void OnEnable()
    {
        if (triggerType == ChatTriggerType.EveryStart)
        {
            StartChat();
        }
    }

    /// <summary>
    /// 대화 시작
    /// </summary>
    public void StartChat()
    {
        Chat.Instance.StartChat(chatName);
        talkCount++;
    }
}
