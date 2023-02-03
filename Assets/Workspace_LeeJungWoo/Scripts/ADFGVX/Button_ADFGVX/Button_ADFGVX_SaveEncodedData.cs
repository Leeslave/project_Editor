using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_SaveEncodedData : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.verificationpart.StartEncodeVerifiaction();
    }
}
