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
    
    [Header("Enter ��")]
    public Color Enter;
    [Header("Exit ��")]
    public Color Exit;
    [Header("Down ��")]
    public Color Down;
    [Header("Up ��")]
    public Color Up;

    //Ŀ�� ���� Ȯ�� ����
    private bool isCursorOver;

    //���� �����ϴ� �ڷ�ƾ, �ߵ� �ı� �� �߰��� ����
    private Coroutine colorConvertCoroutine;

    protected virtual void Start()
    {
        InitButton();
    }

    protected void SetLayer(int layer)//�Է� ����
    {
        this.gameObject.layer = layer;
    }

    protected void InitButton()//��ư �ʱ�ȭ
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

    protected virtual void OnMouseDown()//��ư �ٿ�
    {
        if (GetIsCursorOver())
            ConvertClickSpriteColor(Down);
        else
            ConvertClickSpriteColor(Exit);
    }

    protected virtual void OnMouseUp()//��ư ��
    {
        if (GetIsCursorOver())
            ConvertClickSpriteColor(Up);
        else
            ConvertClickSpriteColor(Exit);
    }

    protected virtual void OnMouseEnter()//��ư ����
    {
        SetIsCursorOver(true);
        SetClickSpriteColor(Enter);
    }

    protected virtual void OnMouseExit()//��ư ����Ʈ
    {
        SetIsCursorOver(false);
        ConvertColorSprite(clickSprite, 1, Exit);
    }

    public void ConvertClickSpriteColor(Color value)
    {
        ConvertColorSprite(clickSprite, 1, value);
    }

    public void SetMarkText(string value)//MarkText �� ����
    {
        markText.text = value;
    }

    public string GetMarkText()//MarkText �� ��ȯ
    {
        return markText.text;
    }

    public void SetClickSpriteColor(Color value)//Ŭ�� ��������Ʈ �� ����
    {
        if (colorConvertCoroutine != null)
            StopCoroutine(colorConvertCoroutine);
        clickSprite.color = value;
    }

    public bool GetIsCursorOver()//IsCursorOver ��ȯ
    {
        return isCursorOver;
    }

    public void SetIsCursorOver(bool value)//IsCursorOver ����
    {
        isCursorOver = value;
    }

    private void ConvertColorSprite(SpriteRenderer target, float endTime, Color targetValue)//��������Ʈ �� ��ȯ
    {
        if(colorConvertCoroutine!=null)
           StopCoroutine(colorConvertCoroutine);
        colorConvertCoroutine =  StartCoroutine(ConvertColorSpriteIEnumerator(target, endTime, 0, targetValue));
    }

    private IEnumerator ConvertColorSpriteIEnumerator(SpriteRenderer target, float endTime, float currentTime, Color targetValue)//��������Ʈ �� ��ȯ ���
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
