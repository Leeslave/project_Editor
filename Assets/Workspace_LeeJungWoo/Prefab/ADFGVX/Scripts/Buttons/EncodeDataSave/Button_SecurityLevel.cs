using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SecurityLevel : Button
{
    private int securityLevel;

    protected override void Start()
    {
        base.Start();
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
        string[] korean = { "[Ư�� ���]", "[1�� ���]", "[2�� ���]", "[3�� ���]"};
        SetMarkText(korean[level]);
    }
}
