using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_DecodeSave : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.verificationpart.StartDecodeVerification();
    }
}
