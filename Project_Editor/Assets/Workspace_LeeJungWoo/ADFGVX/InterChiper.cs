using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InterChiper : MonoBehaviour
{
    private ADFGVXscript ADFGVX;

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
    }

    public void AddText(string value)
    {
        GetComponentInChildren<TextMeshPro>().text += value;
        return;
    }

    public void ClearText()
    {
        GetComponentInChildren<TextMeshPro>().text = "";
        return;
    }

    public void DeleteText()
    {
        string text = GetComponentInChildren<TextMeshPro>().text;
        int DeleteLength = ADFGVX.CurrentCodemode == ADFGVXscript.Codemode.Decoding ? 2 : 3;
        if (text.Length >= DeleteLength)
            GetComponentInChildren<TextMeshPro>().text = text.Substring(0, text.Length - DeleteLength);
        else
        {
            ADFGVX.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            ADFGVX.InformCurrentMode();
        }
            
    }
}
