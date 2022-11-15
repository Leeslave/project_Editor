using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Delete : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.intermediatepart.DeleteText();
    }
}
