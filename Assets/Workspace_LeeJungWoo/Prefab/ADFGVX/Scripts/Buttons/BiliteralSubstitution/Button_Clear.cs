using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Clear : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            adfgvx.afterDecodingPart.GetInputField_Data().ClearInputFieldByButton();
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            adfgvx.beforeEncodingPart.GetInputField_Data().ClearInputFieldByButton();
    }
}
