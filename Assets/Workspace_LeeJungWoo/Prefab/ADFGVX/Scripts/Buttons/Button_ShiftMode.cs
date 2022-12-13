using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button_ShiftMode : Button
{
    private void Update()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            UpdateButtonText("E", "n");
        else
            UpdateButtonText("D", "e");
    }

    public void UpdateButtonText(string value1, string value2)
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = value1;
        GetComponentsInChildren<TextMeshPro>()[1].text = value2;
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.OnModeDown();
    }
}
