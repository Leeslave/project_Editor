using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Clear : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (GameManager.currentmode == ADFGVX.mode.Decoding)
            GameManager.afterDecodingPart.GetInputField_Data().ClearInputFieldByButton();
        else if (GameManager.currentmode == ADFGVX.mode.Encoding)
            GameManager.beforeEncodingPart.GetInputField_Data().ClearInputFieldByButton();
    }
}
