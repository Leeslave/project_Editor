using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_VoightKampff_Node : Button_VoightKampff
{
    [Header("������ ����")]
    public int line;

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.OnNodeDown(line);
    }
}
