using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowButton : Button
{
    public int row;
    public bool Selected;

    protected override void OnMouseDown()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            ConvertClickSprite(new Color(0.5f, 0, 0, 1));
    }

    protected override void OnMouseUp()
    {
        if (GetIsCursorOver())
        {
            if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            {
                adfgvx.InformError("버튼 사용 불가 : 현재 모드 재확인 요망");
                ConvertClickSprite(Color.red);
            }
            else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    adfgvx.biliteralsubstitutionpart.OnDecRowDown(6);
                    ConvertClickSprite(Color.white);
                }
                else
                {
                    Selected = true;
                    adfgvx.biliteralsubstitutionpart.OnDecRowDown(row);
                    ConvertClickSprite(new Color(0, 0.5f, 0, 1));
                }
            }
        }
        else
            ConvertClickSprite(Exit);
    }

    protected override void OnMouseEnter()
    {
        SetIsCursorOver(true);
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            SetClickSprite(Color.white);
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            SetClickSprite(Color.red);
    }

    protected override void OnMouseExit()
    {
        SetIsCursorOver(false);
        if (!Selected)
            ConvertClickSprite(Exit);
    }
}
