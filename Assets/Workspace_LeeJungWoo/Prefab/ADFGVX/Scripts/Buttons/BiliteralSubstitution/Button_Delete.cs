using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Delete : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            adfgvx.afterDecodingPart.GetInputField_Data().DeleteInputFieldByButton(2);
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            adfgvx.beforeEncodingPart.DeleteInputField_DataByButton();
    }
}
