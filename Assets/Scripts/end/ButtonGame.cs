using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button_Game : MonoBehaviour
{
    private STRConverter m_STRConverter;
    private TextMeshPro m_TMP;
    private SpriteRenderer guideSprite;
    private SpriteRenderer m_ClickSpriteRenderer;
    
    [Header("Enter Color")]
    public Color Enter;
    [Header("Exit Color")]
    public Color Exit;
    [Header("Down Color")]
    public Color Down;
    [Header("Up Color")]
    public Color Up;

    //커서 오버 여부
    private bool isCursorOver;

    protected virtual void Awake()
    {
        m_STRConverter = FindObjectOfType<STRConverter>();

        m_TMP = transform.GetChild(0).GetComponent<TextMeshPro>();
        guideSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        m_ClickSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();

        m_ClickSpriteRenderer.color = Exit;
        SetIsCursorOver(false);
    }

    protected void SetLayer(int layer)//입력 제어
    {
        this.gameObject.layer = layer;
    }

    protected virtual void OnMouseDown()//버튼 다운
    {
        if (GetIsCursorOver())
            m_STRConverter.ConvertSpriteRendererColor(0f, Down, m_ClickSpriteRenderer);
        else
            m_STRConverter.ConvertSpriteRendererColor(0.2f, Exit, m_ClickSpriteRenderer);
    }

    protected virtual void OnMouseUp()//버튼 업
    {
        if (GetIsCursorOver())
            m_STRConverter.ConvertSpriteRendererColor(0.2f, Up, m_ClickSpriteRenderer);
        else
            m_STRConverter.ConvertSpriteRendererColor(0.2f, Exit, m_ClickSpriteRenderer);
    }

    protected virtual void OnMouseEnter()//마우스 엔터
    {
        SetIsCursorOver(true);
        m_STRConverter.ConvertSpriteRendererColor(0f, Enter, m_ClickSpriteRenderer);
    }

    protected virtual void OnMouseExit()//마우스 엑시트
    {
        SetIsCursorOver(false);
        m_STRConverter.ConvertSpriteRendererColor(0.2f, Exit, m_ClickSpriteRenderer);
    }

    public SpriteRenderer GetClickSprite()
    {
        return m_ClickSpriteRenderer;
    }

    protected STRConverter GetSTRConverter()
    {
        return m_STRConverter;
    }

    public TextMeshPro GetTMP()
    {
        return m_TMP;
    }

    public bool GetIsCursorOver()//IsCursorOver값 반환
    {
        return isCursorOver;
    }

    public void SetIsCursorOver(bool value)//IsCursorOver값 설정
    {
        isCursorOver = value;
    }
}
