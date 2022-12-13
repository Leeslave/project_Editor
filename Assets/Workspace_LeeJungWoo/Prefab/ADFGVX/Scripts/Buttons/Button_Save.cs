using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Save : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        Debug.Log("복호화 데이터를 저장합시다!");
    }
}
