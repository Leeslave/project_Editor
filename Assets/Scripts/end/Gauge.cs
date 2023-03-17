using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    private SpriteRenderer guideSprite;
    private SpriteRenderer barSprite;

    private void Start()
    {
        if (transform.Find("GuideSprite").GetComponent<SpriteRenderer>() != null)
            guideSprite = transform.Find("GuideSprite").GetComponent<SpriteRenderer>();

        if (transform.Find("BarSprite").GetComponent<SpriteRenderer>() != null)
            barSprite = transform.Find("BarSprite").GetComponent<SpriteRenderer>();
    }

    public SpriteRenderer GetGuideSprite()
    {
        return guideSprite;
    }

    public SpriteRenderer GetBarSprite()
    {
        return barSprite;
    }

    public void FillGaugeBar(float time, Color barColor)
    {
        barSprite.size = new Vector2(0.01f, barSprite.size.y);
        barSprite.color = barColor;
        StartCoroutine(FillGaugeBarIEnumerator(time, 0));
    }

    private IEnumerator FillGaugeBarIEnumerator(float endTime, float currentendTime)
    {
        currentendTime += endTime / 100;
        if (currentendTime > endTime)
            yield break;

        barSprite.size = new Vector2(47.6f * (currentendTime / endTime), barSprite.size.y);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(FillGaugeBarIEnumerator(endTime, currentendTime));
    }

    public void UnvisibleGauge(float endTime)
    {
        ConvertColorSprite(barSprite, endTime, Color.clear);
        ConvertColorSprite(guideSprite, endTime, Color.clear);
    }

    public void VisibleGaugeImediately()
    {
        barSprite.color = new Color(0.13f, 0.67f, 0.28f, 1);
        guideSprite.color = Color.white;
    }

    private void ConvertColorSprite(SpriteRenderer target, float endTime, Color targetValue)
    {
        StartCoroutine(ConvertColorSprite_IE(target, endTime, 0, targetValue));
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
        StartCoroutine(ConvertColorSprite_IE(target, endTime, currentTime, targetValue));
    }
}
