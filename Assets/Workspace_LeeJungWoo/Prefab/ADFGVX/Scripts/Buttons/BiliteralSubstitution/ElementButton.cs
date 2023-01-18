using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElementButton : Button
{
    public int row;
    public int line;
    public bool clicked;

    public void ChangeButtonText(char value)
    {
        SetMarkText(value.ToString());
    }

    public string GetButtonText()
    {
        return GetMarkText();
    }

    protected override void OnMouseDown()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            ConvertClickSprite(new Color(0, 0.5f, 0, 1));
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            ConvertClickSprite(new Color(0.5f, 0, 0, 1));
    }
 
    protected override void OnMouseUp()
    {
        if(GetIsCursorOver())
        {
            if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            {
                adfgvx.InformError("버튼 사용 불가 : 현재 모드 재확인 요망");
                ConvertClickSprite(Color.red);
            }
            else if(adfgvx.currentmode == ADFGVX.mode.Encoding)
            {
                adfgvx.biliteralsubstitutionpart.OnEncElementDown(row, line);
                ConvertClickSprite(Color.white);
            }
        }
        else
            ConvertClickSprite(Exit);
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            SetClickSprite(Color.white);
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            SetClickSprite(Color.red);
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        ConvertClickSprite(Exit);
    }
}
