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

    public string GetText()//markText를 반환한다
    {
        return markText.text;
    }

    public void SetText(string value)//markText를 value로 설정한다
    {
        markText.text = value;
    }

    public bool GetIsNowFlowText()//흐름 출력 진행 여부 반환
    {
        return isNowFlowText;
    }

    public void FlowText(string value, float endTime)//value를 endTime안에 markText에 순차적으로 채워 넣는다
    {
        markText.text = "";
        isNowFlowText = true;
        StartCoroutine(FlowTextIEnumerator(value, 0, endTime));
    }

    private IEnumerator FlowTextIEnumerator(string value, int idx, float endTime)//FlowText 재귀
    {
        if (idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);
        
        yield return new WaitForSeconds(endTime / value.Length);
        StartCoroutine(FlowTextIEnumerator(value, idx + 1, endTime));
    }



    public void SetColorText(Color targetValue)//martText 색 설정
    {
        markText.color = targetValue;
    }

    public void ConvertColorTextOnly(float time, Color targetValue)//martText 색 변환
    {
        ConvertColorText(markText, time, targetValue);
    }

    private void ConvertColorText(TextMeshPro target, float endTime, Color targetColor)//markText 색 변환 시작
    {
        StartCoroutine(ConvertColorTextIEnumerator(target.color, target, endTime, 0, targetColor));
    }

    private IEnumerator ConvertColorTextIEnumerator(Color currentColor, TextMeshPro target, float endTime, float currentTime, Color targetColor)//markText 색 변환 재귀
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
        StartCoroutine(ConvertColorTextIEnumerator(currentColor, target, endTime, currentTime, targetColor));
    }



    public void ConvertSizeTextOnly(Vector2 targetSize, float time)//markText 사이즈 전환
    {
        ConvertSizeText(markText, targetSize, time);
    }

    public void SetSizeTextOnly(Vector2 targetSize)
    {
        markText.transform.localScale = new Vector2(targetSize.x, targetSize.y);
    }

    private void ConvertSizeText(TextMeshPro target, Vector2 targetSize, float endTime)//markText 사이즈 전환 시작
    {
        StartCoroutine(ConvertSizeTextIEnumerator(target.transform.localScale, target, targetSize, endTime, 0));
    }

    private IEnumerator ConvertSizeTextIEnumerator(Vector2 currentSize, TextMeshPro target, Vector2 targetSize, float endTime, float currentTime)//markText 사이즈 전환 재귀
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float newSizeX = currentSize.x + (targetSize.x - currentSize.x) * (currentTime / endTime);
        float newSizeY = currentSize.y + (targetSize.y - currentSize.y) * (currentTime / endTime);
        target.transform.localScale = new Vector3(newSizeX, newSizeY, 1);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSizeTextIEnumerator(currentSize, target, targetSize, endTime, currentTime));
    }



    public void FillPercentage(float time)
    {
        StartCoroutine(FillPercentageIEnumerator(time, 0));
    }

    private IEnumerator FillPercentageIEnumerator(float time, float currentTime)
    {
        currentTime += time / 100;
        if (currentTime >= time)
            yield break;

        markText.text = Mathf.CeilToInt(currentTime / time * 100).ToString() + "%";

        yield return new WaitForSeconds(time / 100);
        StartCoroutine(FillPercentageIEnumerator(time, currentTime));
    }
}
