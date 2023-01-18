using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Load : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.encodeDataLoadPart.LoadEncodeDataByButton();
    }
}
