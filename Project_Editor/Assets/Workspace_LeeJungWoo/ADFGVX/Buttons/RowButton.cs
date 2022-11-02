using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowButton : Button
{
    public int row;
    public bool Selected;

    private void FixedUpdate()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding && Selected)
            GetClickSprite().color = new Color(0, 0.5f, 0);
    }

    protected override void OnMouseDown()
    {
        if (GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Encoding)
        {
            GetADFGVX().UpdateInfoBox("현재 암호화 모드, 모드 재확인 요망");
            GetADFGVX().InformCurrentMode();
            return;
        }
        else if(GetADFGVX().CurrentCodemode == ADFGVXscript.Codemode.Decoding)
        {
            if (Selected)
            {
                Selected = false;
                GetADFGVX().OnDecRowDown(6);
            }
            else
            {
                Selected = true;
                GetADFGVX().OnDecRowDown(row);
            }
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
