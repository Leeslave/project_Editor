using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Clear : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            GameManager.afterDecodingPart.GetInputField_Data().ClearInputField();
        else if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            GameManager.beforeEncodingPart.GetInputField_Data().ClearInputField();
    }
}
