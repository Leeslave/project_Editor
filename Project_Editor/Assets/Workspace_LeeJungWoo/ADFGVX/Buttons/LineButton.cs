using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : Button
{
    public int line;
    public bool Selected;

    private void FixedUpdate()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding && Selected)
            GetClickSprite().color = new Color(0, 0.5f, 0);
    }

    protected override void OnMouseDown()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(0.5f, 0, 0);
        if (Selected)
        {
            Selected = GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding ? false : true;
            GetADFGVX().OnDecLineDown(6);
        }
        else
        {
            Selected = GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding ? true : false;
            GetADFGVX().OnDecLineDown(line);
        }
    }

    protected override void OnMouseUp()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(1, 0, 0);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding && !Selected)
            GetClickSprite().color = new Color(1, 1, 1);
    }

    protected override void OnMouseEnter()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseExit()
    {
        if (!Selected)
            DisableClickSprite();
    }
}

