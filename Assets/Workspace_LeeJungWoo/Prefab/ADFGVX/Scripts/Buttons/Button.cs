using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
    protected ADFGVX adfgvx;
    protected TextMeshPro buttonText;
    protected SpriteRenderer buttonGuideLine;
    private SpriteRenderer clickSprite;
    protected bool IsOver;

    protected virtual void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();
        buttonText = GetComponentInChildren<TextMeshPro>();
        buttonGuideLine = GetComponentsInChildren<SpriteRenderer>()[0];
        clickSprite = GetComponentsInChildren<SpriteRenderer>()[1];

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
        return clickSprite;
    }

    public void DisableClickSprite()
    {
        clickSprite.color = new Color(0, 0, 0, 0);
    }

    public void EnableClickSprite()
    {
        clickSprite.color = new Color(1, 1, 1, 1);
    }
}
