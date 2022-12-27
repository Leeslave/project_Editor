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
        this.GetComponentInChildren<TextMeshPro>().text = value.ToString();
    }

    public string GetButtonText()
    {
        return this.GetComponentInChildren<TextMeshPro>().text;
    }

    protected override void OnMouseDown()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            GetClickSprite().color = new Color(0, 0.5f, 0, 1);
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            GetClickSprite().color = new Color(0.5f, 0, 0, 1);
    }

    protected override void OnMouseUp()
    {
        if(GetIsOver())
        {
            adfgvx.biliteralsubstitutionpart.OnEncElementDown(row, line);
            if (adfgvx.currentmode == ADFGVX.mode.Encoding)
                GetClickSprite().color = new Color(1, 1, 1, 1);
            else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
                GetClickSprite().color = new Color(1, 0, 0, 1);
        }
        else
            DisableClickSprite();
    }

    protected override void OnMouseEnter()
    {
        SetIsOver(true);
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            GetClickSprite().color = new Color(1, 1, 1, 1);
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
            GetClickSprite().color = new Color(1, 0, 0, 1);
    }

    protected override void OnMouseExit()
    {
        SetIsOver(false);
        DisableClickSprite();
    }
}
