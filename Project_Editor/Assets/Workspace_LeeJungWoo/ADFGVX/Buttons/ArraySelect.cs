using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArraySelect : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetADFGVX().OnArraySelectDown();
    }
}
