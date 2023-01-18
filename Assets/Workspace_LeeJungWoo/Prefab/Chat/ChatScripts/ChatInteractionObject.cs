using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInteractionObject : MonoBehaviour
{
    private Chat chat;
    [Header("목표 CSV 라인")]
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
