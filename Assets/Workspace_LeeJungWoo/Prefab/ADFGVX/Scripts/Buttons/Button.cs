using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
    protected ADFGVX adfgvx;

    private TextMeshPro markText;
    private SpriteRenderer markSprite;
    private SpriteRenderer guideSprite;
    private SpriteRenderer clickSprite;
    
    [Header("Enter 색")]
    public Color Enter;
    [Header("Exit 색")]
    public Color Exit;
    [Header("Down 색")]
    public Color Down;
    [Header("Up 색")]
    public Color Up;

    private bool isCursorOver;

    //색을 변경하는 코루틴 관리
    private Coroutine colorConvertCoroutine;

    protected virtual void Start()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();

        if (transform.Find("MarkSprite") != null)
            markSprite = transform.Find("MarkSprite").GetComponent<SpriteRenderer>();

        if (transform.Find("GuideSprite") != null)
            guideSprite = transform.Find("GuideSprite").GetComponent<SpriteRenderer>();

        if (transform.Find("ClickSprite") != null)
            clickSprite = transform.Find("ClickSprite").GetComponent<SpriteRenderer>();

        SetClickSprite(Exit);

        SetIsCursorOver(false);
    }

    protected virtual void OnMouseDown()
    {
        if (GetIsCursorOver())
            ConvertClickSprite(Down);
        else
            ConvertClickSprite(Exit);
    }

    protected virtual void OnMouseUp()
    {
        if (GetIsCursorOver())
            ConvertClickSprite(Up);
        else
            ConvertClickSprite(Exit);
    }

    protected virtual void OnMouseEnter()
    {
        SetIsCursorOver(true);
        SetClickSprite(Enter);
    }

    protected virtual void OnMouseExit()
    {
        SetIsCursorOver(false);
        ConvertColorSprite(clickSprite, 1, Exit);
    }

    public void ConvertClickSprite(Color value)
    {
        ConvertColorSprite(clickSprite, 1, value);
    }

    public void SetMarkText(string value)
    {
        markText.text = value;
    }

    public string GetMarkText()
    {
        return markText.text;
    }

    public void SetClickSprite(Color value)
    {
        if (colorConvertCoroutine != null)
            StopCoroutine(colorConvertCoroutine);
        clickSprite.color = value;
    }

    public bool GetIsCursorOver()
    {
        return isCursorOver;
    }

    public void SetIsCursorOver(bool value)
    {
        isCursorOver = value;
    }

    private void ConvertColorSprite(SpriteRenderer target, float time, Color targetValue)//스프라이트 색 변환
    {
        if(colorConvertCoroutine!=null)
           StopCoroutine(colorConvertCoroutine);
        colorConvertCoroutine =  StartCoroutine(ConvertColorSpriteIEnumerator(target, time, 0, targetValue));
    }

    private IEnumerator ConvertColorSpriteIEnumerator(SpriteRenderer target, float time, float currentTime, Color targetValue)//스프라이트 색 변환 재귀
    {
        currentTime += time / 100;
        if (currentTime > time)
            yield break;

        float target_r = target.color.r + (targetValue.r - target.color.r) * (currentTime / time);
        float target_g = target.color.g + (targetValue.g - target.color.g) * (currentTime / time);
        float target_b = target.color.b + (targetValue.b - target.color.b) * (currentTime / time);
        float target_a = target.color.a + (targetValue.a - target.color.a) * (currentTime / time);
        target.color = new Color(target_r, target_g, target_b, target_a);

        yield return new WaitForSeconds(time / 100);
        colorConvertCoroutine = StartCoroutine(ConvertColorSpriteIEnumerator(target, time, currentTime, targetValue));
    }
}
