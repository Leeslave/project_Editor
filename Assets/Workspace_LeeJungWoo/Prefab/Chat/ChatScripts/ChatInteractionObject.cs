using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInteractionObject : MonoBehaviour
{
    private Chat chat;
    [Header("��ǥ CSV ����")]
    public int line;

    private void Start()
    {
        chat = GameObject.FindObjectOfType<Chat>();
    }

    private void OnMouseDown()
    {
        chat.OnCharOrObjDown(line);
    }
}
