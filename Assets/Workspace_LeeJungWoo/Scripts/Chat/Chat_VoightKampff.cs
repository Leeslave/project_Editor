using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_VoightKampff : Chat
{
    public GameManager_VoightKampff GameManager;

    protected override void SetLayerDefault()
    {
        GameManager.SetLayer(0, 0, 0, 0, 0);
    }
}
