using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFieldProUGUI : MonoBehaviour
{
    [Header("표시 텍스트")]
    public TextMeshProUGUI markText;

    private float width;
    private float height;

    private Coroutine flowTextWithEndTimeCoroutine;
    private Coroutine flowTextWithDelayCoroutine;
    private bool isNowFlowText;

    private void Awake()
    {

    }

    public string GetText()//markText에 설정된 값을 반환
    {
        return markText.text;
    }

    public void SetText(string value)//value에 따라서 markText를 설정하고 캔버스를 업데이트, ContentSizeFitter에 의해 변경된 Rect의 정보를 저장
    {
        markText.text = value;
        Canvas.ForceUpdateCanvases();
        width = this.GetComponentInChildren<RectTransform>().rect.width;
        height = this.GetComponentInChildren<RectTransform>().rect.height;
    }

    public float GetWidth()//ContentSizeFitter에 따라 조정된 Rect의 너비 반환
    {
        return width;
    }

    public float GetHeight()//ContentSizeFitter에 따라 조정된 Rect의 높이 반환
    {
        return height;
    }

    public bool GetIsNowFlowText()//현재 흐름 출력 여부를 반환한다
    {
        return isNowFlowText;
    }

    public void FlowTextWithEndTime(string value, float endTime)//endTime에 따라 흐름 출력한다
    {
        markText.text = "";
        isNowFlowText = true;
        flowTextWithEndTimeCoroutine = StartCoroutine(FlowTextWithEndTime_IE(value, 0, endTime));
    }

    private IEnumerator FlowTextWithEndTime_IE(string value, int idx, float endTime)
    {
        if (idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);

        yield return new WaitForSeconds(endTime / value.Length);
        flowTextWithEndTimeCoroutine = StartCoroutine(FlowTextWithEndTime_IE(value, idx + 1, endTime));
    }

    public void FlowTextWithDelay(string value, float delay)//delay에 따라 흐름 출력한다
    {
        markText.text = "";
        isNowFlowText = true;
        flowTextWithDelayCoroutine = StartCoroutine(FlowTextWithDelay_IE(value, 0, delay));
    }

    private IEnumerator FlowTextWithDelay_IE(string value, int idx, float delay)
    {
        if(idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);

        yield return new WaitForSeconds(delay);
        flowTextWithDelayCoroutine = StartCoroutine(FlowTextWithDelay_IE(value, idx + 1, delay));
    }

    public void StopCoroutineFlowTextWithEndTime()//endTime 흐름 출력을 강제 종료한다
    {
        StopCoroutine(flowTextWithEndTimeCoroutine);
        isNowFlowText = false;
    }

    public void StopCoroutineFlowTextWithDelay()//delay 흐름 출력을 강제 종료한다
    {
        if(flowTextWithDelayCoroutine!=null)
           StopCoroutine(flowTextWithDelayCoroutine);
        isNowFlowText = false;
    }

    public void HideTextOnly(float time)//텍스트의 색깔을 투명으로 바꿔 숨긴다
    {
        ConvertColorText(markText, time, Color.clear);
    }

    public void SetColorText(Color targetValue)//targetValue로 텍스트의 색깔을 설정한다
    {
        markText.color = targetValue;
    }

    private void ConvertColorText(TextMeshProUGUI target, float endTime, Color targetValue)
    {
        StartCoroutine(ConvertColorText_IE(target.color, target, endTime, 0, targetValue));
    }

    private IEnumerator ConvertColorText_IE(Color currentValue, TextMeshProUGUI target, float endTime, float currentTime, Color targetValue)//endTime에 따라 targetValue로 텍스트의 색깔을 설정한다
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float target_r = currentValue.r + ((targetValue.r - currentValue.r) * (currentTime / endTime));
        float target_g = currentValue.g + ((targetValue.g - currentValue.g) * (currentTime / endTime));
        float target_b = currentValue.b + ((targetValue.b - currentValue.b) * (currentTime / endTime));
        float target_a = currentValue.a + ((targetValue.a - currentValue.a) * (currentTime / endTime));
        target.color = new Color(target_r, target_g, target_b, target_a);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertColorText_IE(currentValue, target, endTime, currentTime, targetValue));
    }

    private void ConvertSizeText(TextMeshProUGUI target, float targetSizeX, float targetSizeY, float endTime)
    {
        StartCoroutine(ConvertSizeText_IE(target.transform.localScale.x, target.transform.localScale.y, target, targetSizeX, targetSizeY, endTime, 0));
    }

    private IEnumerator ConvertSizeText_IE(float currentSizeX, float currentSizeY, TextMeshProUGUI target, float targetSizeX, float targetSizeY, float endTime, float currentTime)//endTime에 따라 targetSize로 텍스트의 크기를 설정한다
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float newSizeX = currentSizeX + (targetSizeX - currentSizeX) * (currentTime / endTime);
        float newSizeY = currentSizeY + (targetSizeY - currentSizeY) * (currentTime / endTime);
        target.transform.localScale = new Vector3(newSizeX, newSizeY, 1);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSizeText_IE(currentSizeX, currentSizeY, target, targetSizeX, targetSizeY, endTime, currentTime));
    }
}
