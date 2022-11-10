using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mode : Button
{
    public void UpdateText(string value1, string value2)
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = value1;
        GetComponentsInChildren<TextMeshPro>()[1].text = value2;
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetADFGVX().OnModeDown();
    }
}
