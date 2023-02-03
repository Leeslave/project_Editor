using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_QuitVerificationPanel : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.verificationpart.UnvisiblePart();
        GameManager.SetPartLayer(0, 0, 0, 0, 0, 0, 0, 0);
    }
}
