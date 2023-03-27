using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_ADFGVX : Chat
{
    [Header("게임 매니저")]
    public ADFGVX GameManager;

    public override void LoadLine(int line)
    {
        GameManager.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);
        base.LoadLine(line);
    }

    protected override void SetLayerDefault()
    {
        GameManager.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }
}
