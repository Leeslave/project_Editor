using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreAnim : AnimBase
{
    public float WaitSecond = 0.5f;

    public override IEnumerator Play()
    {
       
        var c = GUITextCtrl.color;
        GUITextCtrl.color = new Color(c.r, c.g, c.b, 0);
        yield return new WaitForSeconds(WaitSecond);
        Debug.Log("INFO PLAY2");
        GUITextCtrl.color = new Color(c.r, c.g, c.b, 255);
    }
}
