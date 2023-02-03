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
        if (GameManager.currentmode == ADFGVX.mode.Encoding)
            ConvertClickSpriteColor(new Color(0, 0.5f, 0, 1));
        else if (GameManager.currentmode == ADFGVX.mode.Decoding)
            ConvertClickSpriteColor(new Color(0.5f, 0, 0, 1));
    }
 
    protected override void OnMouseUp()
    {
        if(GetIsCursorOver())
        {
            if (GameManager.currentmode == ADFGVX.mode.Decoding)
            {
                GameManager.InformError("��ư ��� �Ұ� : ���� ��� ��Ȯ�� ���");
                ConvertClickSpriteColor(Color.red);
            }
            else if(GameManager.currentmode == ADFGVX.mode.Encoding)
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
        if (GameManager.currentmode == ADFGVX.mode.Encoding)
            SetClickSpriteColor(Color.white);
        else if (GameManager.currentmode == ADFGVX.mode.Decoding)
            SetClickSpriteColor(Color.red);
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        ConvertClickSpriteColor(Exit);
    }
}
