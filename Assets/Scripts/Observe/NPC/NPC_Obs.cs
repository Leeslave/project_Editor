using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Obs : MonoBehaviour
{
    [SerializeField]
    Detect_Obs Manager;
    [SerializeField]
    public string[] Chats;
    [SerializeField]
    public string[] Actions;
    [SerializeField]
    public string Name;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Manager.Chats = Chats;
        Manager.Actions = Actions;
        Manager.Name = Name;
        Manager.OnEnter = true;
        Manager.NameMessage.text = Name;
        Manager.CurTarget = transform;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Manager.OnEnter = false;
        Manager.NameMessage.text = "";
    }
}
