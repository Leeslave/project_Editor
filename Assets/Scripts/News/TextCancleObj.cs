using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCancleObj : MonoBehaviour
{
    public KeyCode Resetkey;

    bool OnInputGap = true;
    // 튜토때매 분리함
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(Resetkey) && OnInputGap)
        {
            DB_M.DB_Docs.NewsManager.CancleWork();
            OnInputGap = false;
            Invoke("CanInput", 0.1f);
        }
    }

    void CanInput()
    {
        OnInputGap = true;
    }
}
