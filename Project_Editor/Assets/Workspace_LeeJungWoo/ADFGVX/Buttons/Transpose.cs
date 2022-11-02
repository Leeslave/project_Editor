using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transpose : Button
{
    TranspositionPart TranspositionPart;
    protected override void Awake()
    {
        base.Awake();
        TranspositionPart = GameObject.Find("TranspositionPart").GetComponent<TranspositionPart>();
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        TranspositionPart.OnTransposeDown();
    }
}
