using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_ADFGVX : Chat
{
    [Header("���� �Ŵ���")]
    public ADFGVX GameManager;

    protected override void SetLayerDefault()
    {
        GameManager.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }
}
