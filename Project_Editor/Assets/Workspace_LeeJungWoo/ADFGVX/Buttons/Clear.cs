using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetADFGVX().OnClearDown();
    }
}
