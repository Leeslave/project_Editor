using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInteractionObject : MonoBehaviour
{
    private ChatUI chatUI;
    public int chatLine;

    private void Start()
    {
        chatUI = GameObject.FindObjectOfType<ChatUI>();
    }

    private void OnMouseDown()
    {
        chatUI.OnMoveChatLine(chatLine);
    }
}
