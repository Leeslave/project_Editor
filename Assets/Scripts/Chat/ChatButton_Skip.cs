using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButton_Skip : MonoBehaviour
{
    private Chat chat;

    private void Awake()
    {
        chat = GameObject.FindObjectOfType<Chat>();
    }

    public void OnSkipDown()
    {
        chat.OnSkipDown();
    }
}
