using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private ADFGVXscript ADFGVX;
    private SpriteRenderer ClickSprite;

    protected virtual void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();
        ClickSprite = GetComponentsInChildren<SpriteRenderer>()[1];
        DisableClickSprite();
    }

    protected virtual void OnMouseDown()
    {
        GetClickSprite().color = new Color(0, 0.5f, 0);
    }

    protected virtual void OnMouseUp()
    {
        GetClickSprite().color = new Color(1,1,1);
    }

    protected virtual void OnMouseEnter()
    {
        EnableClickSprite();
    }

    protected virtual void OnMouseExit()
    {
        DisableClickSprite();
    }

    protected ADFGVXscript GetADFGVX()
    {
        return ADFGVX;
    }

    protected SpriteRenderer GetClickSprite()
    {
        return ClickSprite;
    }
    public void DisableClickSprite()
    {
        GetClickSprite().color = new Color(0, 0, 0);
    }
    public void EnableClickSprite()
    {
        GetClickSprite().color = new Color(1, 1, 1);
    }
}
