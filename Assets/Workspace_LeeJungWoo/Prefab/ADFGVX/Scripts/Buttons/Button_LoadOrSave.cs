using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_LoadOrSave : Button
{
    private void Update()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            buttonText.text = "Load";
        else
            buttonText.text = "Save";
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.chiperpart.UpdateChiperTitleAndText();
        }
        else
        {
            adfgvx.chiperpart.ReturnEncodingResult();
        }
    }
}
