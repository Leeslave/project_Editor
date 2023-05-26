using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button_Game : MonoBehaviour
{
    private TextMeshPro markText;
    private SpriteRenderer guideSprite;
    private SpriteRenderer clickSprite;
    
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

    //버튼 색 전환 코루틴 - 색 전환 중단 용
    private Coroutine colorConvertCoroutine;

    protected virtual void Awake()
    {
        Init();
    }

    protected void Init()
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();
        if (transform.Find("GuideSprite") != null)
            guideSprite = transform.Find("GuideSprite").GetComponent<SpriteRenderer>();
        if (transform.Find("ClickSprite") != null)
            clickSprite = transform.Find("ClickSprite").GetComponent<SpriteRenderer>();

        SetClickSpriteColor(Exit);
        SetIsCursorOver(false);
    }

    protected void SetLayer(int layer)//입력 제어
    {
        this.gameObject.layer = layer;
    }

    protected virtual void OnMouseDown()//버튼 다운
    {
        if (GetIsCursorOver())
            ConvertClickSpriteColor(Down);
        else
            ConvertClickSpriteColor(Exit);
    }

    protected virtual void OnMouseUp()//버튼 업
    {
        if (GetIsCursorOver())
            ConvertClickSpriteColor(Up);
        else
            ConvertClickSpriteColor(Exit);
    }

    protected virtual void OnMouseEnter()//마우스 엔터
    {
        SetIsCursorOver(true);
        SetClickSpriteColor(Enter);
    }

    protected virtual void OnMouseExit()//마우스 엑시트
    {
        SetIsCursorOver(false);
        ConvertColorSprite(clickSprite, 1, Exit);
    }

    public void ConvertClickSpriteColor(Color value)
    {
        ConvertColorSprite(clickSprite, 1, value);
    }

    public void SetMarkText(string value)//MarkText값 설정
    {
        markText.text = value;
    }

    public string GetMarkText()//MarkText값 반환
    {
        return markText.text;
    }

    public void SetClickSpriteColor(Color value)//
    {
        if (colorConvertCoroutine != null)
            StopCoroutine(colorConvertCoroutine);
        clickSprite.color = value;
    }

    public bool GetIsCursorOver()//IsCursorOver값 반환
    {
        return isCursorOver;
    }

    public void SetIsCursorOver(bool value)//IsCursorOver값 설정
    {
        isCursorOver = value;
    }

    private void ConvertColorSprite(SpriteRenderer target, float endTime, Color targetValue)
    {
        if(colorConvertCoroutine!=null)
           StopCoroutine(colorConvertCoroutine);
        colorConvertCoroutine =  StartCoroutine(ConvertColorSprite_IE(target, endTime, 0, targetValue));
    }

    private IEnumerator ConvertColorSprite_IE(SpriteRenderer target, float endTime, float currentTime, Color targetValue)
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float target_r = target.color.r + (targetValue.r - target.color.r) * (currentTime / endTime);
        float target_g = target.color.g + (targetValue.g - target.color.g) * (currentTime / endTime);
        float target_b = target.color.b + (targetValue.b - target.color.b) * (currentTime / endTime);
        float target_a = target.color.a + (targetValue.a - target.color.a) * (currentTime / endTime);
        target.color = new Color(target_r, target_g, target_b, target_a);

        yield return new WaitForSeconds(endTime / 100);
        colorConvertCoroutine = StartCoroutine(ConvertColorSprite_IE(target, endTime, currentTime, targetValue));
    }
}
