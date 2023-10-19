using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButton_ExecuteSkip : MonoBehaviour
{
    private Chat chat;

    private void Awake()
    {
        chat = GameObject.FindObjectOfType<Chat>();
    }

    public void OnExecuteSkipDown()
    {
        chat.ExecuteSkip();
    }
}
