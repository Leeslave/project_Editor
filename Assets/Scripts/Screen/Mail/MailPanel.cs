using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MailPanel : MonoBehaviour
{
    public string title;
    public TMP_Text titleUI;
    public bool isActive;

    public void Set(string _title)
    {
        title = _title;
        titleUI.text = _title;
        isActive = false;
    }

    public void OnClick()
    {
        if (!isActive)
        {
            MailManager.Instance.ActiveMail(title);
        }
        else
        {
            MailManager.Instance.OffMail();
        }
    }
}
