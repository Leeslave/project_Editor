using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_ADFGVX : Chat
{
    [Header("게임 매니저")]
    public ADFGVX GameManager;

    protected override void SetLayerDefault()
    {
        GameManager.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }
}
