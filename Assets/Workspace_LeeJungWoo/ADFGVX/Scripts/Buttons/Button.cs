using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
    protected ADFGVX adfgvx;
    protected TextMeshPro buttontext;
    protected SpriteRenderer buttonguideline;

    private SpriteRenderer ClickSprite;
    protected bool IsOver;

    protected virtual void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();
        buttontext = GetComponentInChildren<TextMeshPro>();
        buttonguideline = GetComponentsInChildren<SpriteRenderer>()[0];


        ClickSprite = GetComponentsInChildren<SpriteRenderer>()[1];
        DisableClickSprite();
    }

    protected virtual void OnMouseDown()
    {
        GetClickSprite().color = new Color(0, 0.5f, 0, 1);
    }

    protected virtual void OnMouseUp()
    {
        GetClickSprite().color = new Color(1, 1, 1, 1);
    }

    protected virtual void OnMouseEnter()
    {
        IsOver = true;
        EnableClickSprite();
    }

    protected virtual void OnMouseExit()
    {
        IsOver = false;
        DisableClickSprite();
    }

    protected SpriteRenderer GetClickSprite()
    {
        return ClickSprite;
    }

    public void DisableClickSprite()
    {
        GetClickSprite().color = new Color(0, 0, 0, 0);
    }

    public void EnableClickSprite()
    {
        GetClickSprite().color = new Color(1, 1, 1, 1);
    }
}
