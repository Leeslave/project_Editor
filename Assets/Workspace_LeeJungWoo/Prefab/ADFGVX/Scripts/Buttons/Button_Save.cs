using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Save : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.verificationpart.StartVerificationSequence();
    }
}
