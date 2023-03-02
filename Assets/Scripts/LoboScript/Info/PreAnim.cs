using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreAnim : AnimBase
{
    public float WaitSecond = 0.5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator PlayAnim()
    {
       
        var c = GUITextCtrl.color;
        GUITextCtrl.color = new Color(c.r, c.g, c.b, 0);
        yield return new WaitForSeconds(WaitSecond);
        Debug.Log("INFO PLAY2");
        GUITextCtrl.color = new Color(c.r, c.g, c.b, 255);
    }
}
