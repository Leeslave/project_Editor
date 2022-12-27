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
            GetClickSprite().color = new Color(0.5f, 0, 0);     
    }

    protected override void OnMouseUp()
    {
        if (GetIsOver())
        {
            if (adfgvx.currentmode == ADFGVX.mode.Encoding)
                GetClickSprite().color = new Color(1, 0, 0);
            else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    adfgvx.biliteralsubstitutionpart.OnDecRowDown(6);
                    GetClickSprite().color = new Color(1, 1, 1);
                }
                else
                {
                    Selected = true;
                    adfgvx.biliteralsubstitutionpart.OnDecRowDown(row);
                    GetClickSprite().color = new Color(0, 0.5f, 0);
                }
            }
        }
        else
            DisableClickSprite();
    }

    protected override void OnMouseEnter()
    {
        SetIsOver(true);
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseExit()
    {
        SetIsOver(false);
        if (!Selected)
            DisableClickSprite();
    }
}
