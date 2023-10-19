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
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, new Color(0.5f, 0f, 0f, 1f), GetClickSprite());
    }

    protected override void OnMouseUp()
    {
        if (GetIsCursorOver())
        {
            if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            {
                GameManager.InformError("버튼 사용 불가 : 현재 모드 확인 요망");
                GetSTRConverter().ConvertSpriteRendererColor(0.3f, Color.red, GetClickSprite());
            }
            else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    GameManager.biliteralsubstitutionpart.OnDecRowDown(6);
                    GetSTRConverter().ConvertSpriteRendererColor(0.3f, Color.white, GetClickSprite());
                }
                else
                {
                    Selected = true;
                    GameManager.biliteralsubstitutionpart.OnDecRowDown(row);
                    GetSTRConverter().ConvertSpriteRendererColor(0.3f, new Color(0f, 0.5f, 0f, 1f), GetClickSprite());
                }
            }
        }
        else
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, Exit, GetClickSprite());
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            GetSTRConverter().ConvertSpriteRendererColor(0f, Color.white, GetClickSprite());
        else if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            GetSTRConverter().ConvertSpriteRendererColor(0f, Color.red, GetClickSprite());
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        if (!Selected)
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, Exit, GetClickSprite());
    }
}
