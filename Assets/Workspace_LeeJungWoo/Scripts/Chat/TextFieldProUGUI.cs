using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFieldProUGUI : MonoBehaviour
{
    private TextMeshProUGUI markText;
    private SpriteRenderer guideSprite;

    private Coroutine flowTextWithEndTimeCoroutine;
    private Coroutine flowTextWithDelayCoroutine;
    private bool isNowFlowText;

    private void Start()
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshProUGUI>();

        if (transform.Find("GuideText") != null)
            guideSprite = transform.Find("GuideText").GetComponent<SpriteRenderer>();
    }

    public string GetText()//markText�� ��ȯ�Ѵ�
    {
        return markText.text;
    }

    public void SetText(string value)//markText�� value�� �����Ѵ�
    {
        markText.text = value;
    }

    public bool GetIsNowFlowText()//�帧 ��� ���� ���� ��ȯ
    {
        return isNowFlowText;
    }

    public void FlowTextWithEndTime(string value, float endTime)//value�� endTime�ȿ� markText�� ���������� ä�� �ִ´�
    {
        markText.text = "";
        isNowFlowText = true;
        flowTextWithEndTimeCoroutine = StartCoroutine(FlowTextWithEndTimeIEnumerator(value, 0, endTime));
    }

    private IEnumerator FlowTextWithEndTimeIEnumerator(string value, int idx, float endTime)//FlowText ���
    {
        if (idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);

        yield return new WaitForSeconds(endTime / value.Length);
        flowTextWithEndTimeCoroutine = StartCoroutine(FlowTextWithEndTimeIEnumerator(value, idx + 1, endTime));
    }

    public void FlowTextWithDelay(string value, float delay)//value�� �� ���ھ� delay�� �ָ鼭 markText�� ä�� �ִ´�
    {
        markText.text = "";
        isNowFlowText = true;
        flowTextWithDelayCoroutine = StartCoroutine(FlowTextWithDelayIEnumerator(value, 0, delay));
    }

    private IEnumerator FlowTextWithDelayIEnumerator(string value, int idx, float delay)//FlowTextWithDelay ���
    {
        if(idx >= value.Length)
        {
            isNowFlowText = false;
            yield break;
        }

        markText.text += value.Substring(idx, 1);

        yield return new WaitForSeconds(delay);
        flowTextWithDelayCoroutine = StartCoroutine(FlowTextWithDelayIEnumerator(value, idx + 1, delay));
    }

    public void StopCoroutineFlowTextWithEndTime()//endTime �帧 ��� �ڷ�ƾ ����
    {
        StopCoroutine(flowTextWithEndTimeCoroutine);
        isNowFlowText = false;
    }

    public void StopCoroutineFlowTextWithDelay()//delay �帧 ��� �ڷ�ƾ ����
    {
        StopCoroutine(flowTextWithDelayCoroutine);
        isNowFlowText = false;
    }

    public void HideTextOnly(float time)//markText ����
    {
        ConvertColorText(markText, time, Color.clear);
    }

    public void SetColorText(Color targetValue)//martText �� ����
    {
        markText.color = targetValue;
    }

    private void ConvertColorText(TextMeshProUGUI target, float time, Color targetValue)//markText �� ��ȯ ����
    {
        StartCoroutine(ConvertColorTextIEnumerator(target.color, target, time, 0, targetValue));
    }

    private IEnumerator ConvertColorTextIEnumerator(Color currentValue, TextMeshProUGUI target, float time, float currentTime, Color targetValue)//markText �� ��ȯ ���
    {
        currentTime += time / 100;
        if (currentTime > time)
            yield break;

        float target_r = currentValue.r + ((targetValue.r - currentValue.r) * (currentTime / time));
        float target_g = currentValue.g + ((targetValue.g - currentValue.g) * (currentTime / time));
        float target_b = currentValue.b + ((targetValue.b - currentValue.b) * (currentTime / time));
        float target_a = currentValue.a + ((targetValue.a - currentValue.a) * (currentTime / time));
        target.color = new Color(target_r, target_g, target_b, target_a);

        yield return new WaitForSeconds(time / 100);
        StartCoroutine(ConvertColorTextIEnumerator(currentValue, target, time, currentTime, targetValue));
    }

    private void ConvertSizeText(TextMeshProUGUI target, float targetSizeX, float targetSizeY, float time)//markText ������ ��ȯ ����
    {
        StartCoroutine(ConvertSizeTextIEnumerator(target.transform.localScale.x, target.transform.localScale.y, target, targetSizeX, targetSizeY, time, 0));
    }

    private IEnumerator ConvertSizeTextIEnumerator(float currentSizeX, float currentSizeY, TextMeshProUGUI target, float targetSizeX, float targetSizeY, float time, float currentTime)//markText ������ ��ȯ ���
    {
        currentTime += time / 100;
        if (currentTime > time)
            yield break;

        float newSizeX = currentSizeX + (targetSizeX - currentSizeX) * (currentTime / time);
        float newSizeY = currentSizeY + (targetSizeY - currentSizeY) * (currentTime / time);
        target.transform.localScale = new Vector3(newSizeX, newSizeY, 1);

        yield return new WaitForSeconds(time / 100);
        StartCoroutine(ConvertSizeTextIEnumerator(currentSizeX, currentSizeY, target, targetSizeX, targetSizeY, time, currentTime));
    }
}
