using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button_ADFGVX_Element : Button_ADFGVX
{
    public int row;
    public int line;
    public bool clicked;

    public string GetButtonText()
    {
        return GetTMP().text.ToString();
    }

    protected override void OnMouseDown()
    {
        if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, new Color(0f, 0.5f, 0f, 1f), GetClickSprite());
        else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, new Color(0.5f, 0f, 0f, 1f), GetClickSprite());
    }
 
    protected override void OnMouseUp()
    {
        if(GetIsCursorOver())
        {
            if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            {
                GameManager.InformError("버튼 사용 불가 : 현재 모드 확인 요망");
                GetSTRConverter().ConvertSpriteRendererColor(0.3f, Color.red, GetClickSprite());
            }
            else if(GameManager.CurrentMode == ADFGVX.mode.Encoding)
            {
                GameManager.biliteralsubstitutionpart.OnEncElementDown(row, line);
                GetSTRConverter().ConvertSpriteRendererColor(0.3f, Color.white, GetClickSprite());
            }
        }
        else
            GetSTRConverter().ConvertSpriteRendererColor(0.3f, Exit, GetClickSprite());
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (GameManager.CurrentMode == ADFGVX.mode.Encoding)
            GetSTRConverter().ConvertSpriteRendererColor(0f, Color.white, GetClickSprite());
        else if (GameManager.CurrentMode == ADFGVX.mode.Decoding)
            GetSTRConverter().ConvertSpriteRendererColor(0f, Color.red, GetClickSprite());
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        GetSTRConverter().ConvertSpriteRendererColor(0.3f, Exit, GetClickSprite());
    }
}
