using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_VerificationPanelX : Button
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        adfgvx.verificationpart.UnvisiblePart();
        adfgvx.SetPartLayer(0, 0, 0, 0, 0, 0, 0, 0);
    }
}
