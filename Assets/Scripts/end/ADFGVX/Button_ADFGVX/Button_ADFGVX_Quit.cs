using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Quit : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        Debug.Log("ADFGVX 미니게임 종료");
    }
}
