using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElementButton : Button
{
    public int row;
    public int line;

    public void ChangeButtonText(char value)
    {
        this.GetComponentInChildren<TextMeshPro>().text = value.ToString();
    }

    public void ShiftMode()
    {
        GetClickSprite().color = new Color(0, 0, 0);
    }

    protected override void OnMouseDown()
    {
        GetADFGVX().OnEncElementDown(row, line);
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(0, 0.5f, 0);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            GetClickSprite().color = new Color(0.5f, 0, 0);
    }

    protected override void OnMouseUp()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseEnter()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseExit()
    {
        DisableClickSprite();
    }
}
