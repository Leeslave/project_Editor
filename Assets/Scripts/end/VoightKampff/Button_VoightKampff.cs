using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_VoightKampff : Button_Game
{
    protected GameManager_VoightKampff GameManager;

    protected override void Awake()
    {
        base.Awake();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager_VoightKampff>();
    }
}
