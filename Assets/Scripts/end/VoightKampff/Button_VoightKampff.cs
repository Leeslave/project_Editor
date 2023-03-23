using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_VoightKampff : Button
{
    protected GameManager_VoightKampff GameManager;

    protected override void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager_VoightKampff>();
        Init();
    }
}
