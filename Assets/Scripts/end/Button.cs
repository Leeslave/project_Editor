using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
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

    //커서 오버 확인 변수
    private bool isCursorOver;

    //색을 변경하는 코루틴, 중도 파기 및 추가에 대응
    private Coroutine colorConvertCoroutine;

    protected virtual void Start()
    {
        InitButton();
    }

    protected void SetLayer(int layer)//입력 제어
    {
        this.gameObject.layer = layer;
    }

    protected void InitButton()//버튼 초기화
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();
        if (transform.Find("MarkSprite") != null)
            markSprite = transform.Find("MarkSprite").GetComponent<SpriteRenderer>();
        if (transform.Find("GuideSprite") != null)
            guideSprite = transform.Find("GuideSprite").GetComponent<SpriteRenderer>();
        if (transform.Find("ClickSprite") != null)
            clickSprite = transform.Find("ClickSprite").GetComponent<SpriteRenderer>();

        SetClickSpriteColor(Exit);

        SetIsCursorOver(false);
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

    protected virtual void OnMouseEnter()//버튼 엔터
    {
        SetIsCursorOver(true);
        SetClickSpriteColor(Enter);
    }

    protected virtual void OnMouseExit()//버튼 엑시트
    {
        SetIsCursorOver(false);
        ConvertColorSprite(clickSprite, 1, Exit);
    }

    public void ConvertClickSpriteColor(Color value)
    {
        ConvertColorSprite(clickSprite, 1, value);
    }

    public void SetMarkText(string value)//MarkText 값 설정
    {
        markText.text = value;
    }

    public string GetMarkText()//MarkText 값 반환
    {
        return markText.text;
    }

    public void SetClickSpriteColor(Color value)//클릭 스프라이트 색 설정
    {
        if (colorConvertCoroutine != null)
            StopCoroutine(colorConvertCoroutine);
        clickSprite.color = value;
    }

    public bool GetIsCursorOver()//IsCursorOver 반환
    {
        return isCursorOver;
    }

    public void SetIsCursorOver(bool value)//IsCursorOver 설정
    {
        isCursorOver = value;
    }

    private void ConvertColorSprite(SpriteRenderer target, float endTime, Color targetValue)//스프라이트 색 변환
    {
        if(colorConvertCoroutine!=null)
           StopCoroutine(colorConvertCoroutine);
        colorConvertCoroutine =  StartCoroutine(ConvertColorSpriteIEnumerator(target, endTime, 0, targetValue));
    }

    private IEnumerator ConvertColorSpriteIEnumerator(SpriteRenderer target, float endTime, float currentTime, Color targetValue)//스프라이트 색 변환 재귀
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
        colorConvertCoroutine = StartCoroutine(ConvertColorSpriteIEnumerator(target, endTime, currentTime, targetValue));
    }
}
