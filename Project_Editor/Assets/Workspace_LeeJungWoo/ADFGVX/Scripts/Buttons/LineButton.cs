using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : Button
{
    public int line;
    public bool Selected;

    protected override void OnMouseDown()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(0.5f, 0, 0);
    }

    protected override void OnMouseUp()
    {
        if (IsOver)
        {
            if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
                GetClickSprite().color = new Color(1, 0, 0);
            else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    GetADFGVX().OnDecLineDown(6);
                    GetClickSprite().color = new Color(1, 1, 1);
                }
                else
                {
                    Selected = true;
                    GetADFGVX().OnDecLineDown(line);
                    GetClickSprite().color = new Color(0, 0.5f, 0);
                }
            }
        }
        else
            DisableClickSprite();
    }

    protected override void OnMouseEnter()
    {
        IsOver = true;
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseExit()
    {
        IsOver = false;
        if (!Selected)
            DisableClickSprite();
    }
}

