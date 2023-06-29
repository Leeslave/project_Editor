using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButton_Choice : MonoBehaviour
{
    private Chat chat;
    public int choice;

    private void Awake()
    {
        chat = GameObject.FindObjectOfType<Chat>();
    }

    public void OnChoiceDown()
    {
        chat.OnChoiceDown(choice);
    }
}
