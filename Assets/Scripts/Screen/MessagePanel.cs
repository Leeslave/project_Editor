using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    public int count;
    public bool isRead;

    public void Awake()
    {
        isRead = false;
    }

    public void Onclicked()
    {
        isRead = true;
        MsgManager.Instance.MsgPanel.parent.parent.gameObject.SetActive(true);
        MsgManager.Instance.SetMessagePanel(count);
    }
}
