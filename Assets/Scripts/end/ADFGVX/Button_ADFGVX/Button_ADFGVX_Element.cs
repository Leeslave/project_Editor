using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button_ADFGVX_Element : Button_ADFGVX
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
        if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            ConvertClickSpriteColor(new Color(0, 0.5f, 0, 1));
        else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            ConvertClickSpriteColor(new Color(0.5f, 0, 0, 1));
    }
 
    protected override void OnMouseUp()
    {
        if(GetIsCursorOver())
        {
            if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            {
                GameManager.InformError("버튼 사용 불가 : 현재 모드 재확인 요망");
                ConvertClickSpriteColor(Color.red);
            }
            else if(GameManager.CurrentMode == ADFGVX.mode.Encoding)
            {
                GameManager.biliteralsubstitutionpart.OnEncElementDown(row, line);
                ConvertClickSpriteColor(Color.white);
            }
        }
        else
            ConvertClickSpriteColor(Exit);
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            SetClickSpriteColor(Color.white);
        else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            SetClickSpriteColor(Color.red);
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        ConvertClickSpriteColor(Exit);
    }
}
