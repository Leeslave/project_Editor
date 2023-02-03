using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Delete : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (GameManager.currentmode == ADFGVX.mode.Decoding)
            GameManager.afterDecodingPart.GetInputField_Data().DeleteInputFieldByButton(2);
        else if (GameManager.currentmode == ADFGVX.mode.Encoding)
            GameManager.beforeEncodingPart.DeleteInputField_DataByButton();
    }
}
