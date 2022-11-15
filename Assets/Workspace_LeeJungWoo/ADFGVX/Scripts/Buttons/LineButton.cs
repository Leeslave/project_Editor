using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : Button
{
    public int line;
    public bool Selected;

    protected override void OnMouseDown()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            GetClickSprite().color = new Color(0.5f, 0, 0);
    }

    protected override void OnMouseUp()
    {
        if (IsOver)
        {
            if (adfgvx.currentmode == ADFGVX.mode.Encoding)
                GetClickSprite().color = new Color(1, 0, 0);
            else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            {
                if (Selected)
                {
                    Selected = false;
                    adfgvx.biliteralsubstitutionpart.OnDecLineDown(6);
                    GetClickSprite().color = new Color(1, 1, 1);
                }
                else
                {
                    Selected = true;
                    adfgvx.biliteralsubstitutionpart.OnDecLineDown(line);
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
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            GetClickSprite().color = new Color(1, 1, 1);
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            GetClickSprite().color = new Color(1, 0, 0);
    }

    protected override void OnMouseExit()
    {
        IsOver = false;
        if (!Selected)
            DisableClickSprite();
    }
}

