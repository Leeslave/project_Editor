using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_EncodeSave : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.verificationpart.StartEncodeVerifiaction();
    }
}
