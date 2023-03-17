using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextField : MonoBehaviour
{

    private TextMeshPro markText;
    private SpriteRenderer guideSprite;

    private bool isNowFlowText;

    private void Start()
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();

        if (transform.Find("GuideText") != null)
            guideSprite = transform.Find("GuideText").GetComponent<SpriteRenderer>();
    }

    public string GetText()
    {
        return markText.text;
    }

    public void SetText(string value)
    {
        markText.text = value;
    }

    public bool GetIsNowFlowText()
    {
        return isNowFlowText;
    }

    public void FlowText(string value, float endTime)
    {
        markText.text = "";
        isNowFlowText = true;
        StartCoroutine(FlowText_IE(value, 0, endTime));
    }

    private IEnumerator FlowText_IE(string value, int idx, float endTime)
    {
        if (idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);
        
        yield return new WaitForSeconds(endTime / value.Length);
        StartCoroutine(FlowText_IE(value, idx + 1, endTime));
    }

    public void SetTextColor(Color targetValue)
    {
        markText.color = targetValue;
    }

    public void ConvertColorTextOnly(float time, Color targetValue)
    {
        ConvertColorText(markText, time, targetValue);
    }

    private void ConvertColorText(TextMeshPro target, float endTime, Color targetColor)
    {
        StartCoroutine(ConvertColorText_IE(target.color, target, endTime, 0, targetColor));
    }

    private IEnumerator ConvertColorText_IE(Color currentColor, TextMeshPro target, float endTime, float currentTime, Color targetColor)
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float targetColorR = currentColor.r + ((targetColor.r - currentColor.r) * (currentTime / endTime));
        float targetColorG = currentColor.g + ((targetColor.g - currentColor.g) * (currentTime / endTime));
        float targetColorB = currentColor.b + ((targetColor.b - currentColor.b) * (currentTime / endTime));
        float targetColorA = currentColor.a + ((targetColor.a - currentColor.a) * (currentTime / endTime));
        target.color = new Color(targetColorR, targetColorG, targetColorB, targetColorA);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertColorText_IE(currentColor, target, endTime, currentTime, targetColor));
    }

    public void ConvertSizeTextOnly(Vector2 targetSize, float endTime)
    {
        ConvertSizeText(markText, targetSize, endTime);
    }

    public void SetSizeTextOnly(Vector2 targetSize)
    {
        markText.transform.localScale = new Vector2(targetSize.x, targetSize.y);
    }

    private void ConvertSizeText(TextMeshPro target, Vector2 targetSize, float endTime)
    {
        StartCoroutine(ConvertSizeText_IE(target.transform.localScale, target, targetSize, endTime, 0));
    }

    private IEnumerator ConvertSizeText_IE(Vector2 currentSize, TextMeshPro target, Vector2 targetSize, float endTime, float currentTime)
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float newSizeX = currentSize.x + (targetSize.x - currentSize.x) * (currentTime / endTime);
        float newSizeY = currentSize.y + (targetSize.y - currentSize.y) * (currentTime / endTime);
        target.transform.localScale = new Vector3(newSizeX, newSizeY, 1);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSizeText_IE(currentSize, target, targetSize, endTime, currentTime));
    }

    public void FillPercentage(float endTime)
    {
        StartCoroutine(FillPercentage_IE(endTime, 0));
    }

    private IEnumerator FillPercentage_IE(float endTime, float currentendTime)
    {
        currentendTime += endTime / 100;
        if (currentendTime > endTime)
            yield break;

        markText.text = Mathf.CeilToInt(currentendTime / endTime * 100).ToString() + "%";

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(FillPercentage_IE(endTime, currentendTime));
    }
}
