using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ReTrans : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.transpositionpart.OnTransposeReverseDownByButton();
    }
}
