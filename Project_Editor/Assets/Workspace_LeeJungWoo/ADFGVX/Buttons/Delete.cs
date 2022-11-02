using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GetADFGVX().OnDeleteDown();
    }
}
