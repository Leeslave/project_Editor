using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Quit : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        Debug.Log("ADFGVX를 종료합니다");
    }
}
