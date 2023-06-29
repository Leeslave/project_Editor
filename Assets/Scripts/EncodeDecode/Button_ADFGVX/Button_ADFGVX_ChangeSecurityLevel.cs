using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_ChangeSecurityLevel : Button_ADFGVX
{
    private int securityLevel;

    protected override void Awake()
    {
        base.Awake();
        securityLevel = 3;
        SetSecurityLevel(securityLevel);
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        securityLevel--;
        if (securityLevel < 0)
            securityLevel = 3;
        SetSecurityLevel(securityLevel);
    }

    private void SetSecurityLevel(int level)
    {
        string[] korean = { "[특급 기밀]", "[1급 기밀]", "[2급 기밀]", "[3급 기밀]"};
        GetTMP().text = korean[level];
    }
}
