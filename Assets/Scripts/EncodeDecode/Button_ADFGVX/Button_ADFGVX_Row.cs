using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_Row : Button_ADFGVX
{
    public int row;
    public bool Selected;

    protected override void OnMouseDown()
    {
        if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            ConvertClickSpriteColor(new Color(0.5f, 0, 0, 1));
    }

    protected override void OnMouseUp()
    {
        if (GetIsCursorOver())
        {
            if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            {
                GameManager.InformError("버튼 사용 불가 : 현재 모드 확인 요망");
                ConvertClickSpriteColor(Color.red);
            }
            else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    GameManager.biliteralsubstitutionpart.OnDecRowDown(6);
                    ConvertClickSpriteColor(Color.white);
                }
                else
                {
                    Selected = true;
                    GameManager.biliteralsubstitutionpart.OnDecRowDown(row);
                    ConvertClickSpriteColor(new Color(0, 0.5f, 0, 1));
                }
            }
        }
        else
            ConvertClickSpriteColor(Exit);
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            SetClickSpriteColor(Color.white);
        else if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            SetClickSpriteColor(Color.red);
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        if (!Selected)
            ConvertClickSpriteColor(Exit);
    }
}
