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

    public void FillGaugeBar(float time, Color barColor)//게이지 바 채움
    {
        barSprite.size = new Vector2(0.01f, barSprite.size.y);
        barSprite.color = barColor;
        StartCoroutine(FillGaugeBarIEnumerator(time, 0));
    }

    private IEnumerator FillGaugeBarIEnumerator(float time, float currentTime)//게이지 바 채움 재귀
    {
        currentTime += time / 100;
        if (currentTime > time)
            yield break;

        barSprite.size = new Vector2(47.6f * (currentTime / time), barSprite.size.y);

        yield return new WaitForSeconds(time / 100);
        StartCoroutine(FillGaugeBarIEnumerator(time, currentTime));
    }

    public void HideGauge(float time)//게이지 바 숨김
    {
        ConvertColorSprite(barSprite, time, Color.clear);
        ConvertColorSprite(guideSprite, time, Color.clear);
    }

    public void ShowGauge()
    {
        barSprite.color = new Color(0.13f, 0.67f, 0.28f, 1);
        guideSprite.color = Color.white;
    }

    private void ConvertColorSprite(SpriteRenderer target, float time, Color targetValue)//스프라이트 색 변환
    {
        StartCoroutine(ConvertColorSpriteIEnumerator(target, time, 0, targetValue));
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
        StartCoroutine(ConvertColorSpriteIEnumerator(target, time, currentTime, targetValue));
    }
}
