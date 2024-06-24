using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowEffect : MonoBehaviour
{
    private Image texture;
    public float Delay;
    public bool Loop;
    private float originAlpha = 1;

    void Awake()
    {
        if (TryGetComponent<Image>(out texture))
        {
            originAlpha = texture.color.a;
        }
        else
        {
            Debug.LogWarning($"Warning: Glow Effect on the NOT Image Object");
            Destroy(this);
        }
    }
    
    
    void OnEnable()
    {
        SetAlpha(originAlpha);
        StartCoroutine(GlowAnimationLoop());
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void SetAlpha(float alpha)
    {
        if (!texture)
        {
            return;
        }
        
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, alpha);
    }


    /// <summary>
    /// 깜빡임 효과
    /// </summary>
    public IEnumerator GlowAnimationLoop()
    {
        yield return StartCoroutine(FadeEffect());

        if (Loop)
        {
            StartCoroutine(GlowAnimationLoop());
        }
    }


    /// <summary>
    /// 깜빡임 효과
    /// </summary>
    private IEnumerator FadeEffect()
    {
        float elapsedTime = 0f;
        Color origin = texture.color;

        // 점점 투명해지기
        while (elapsedTime < Delay)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originAlpha, 0f, elapsedTime / Delay);
            SetAlpha(alpha);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        
        // 점점 돌아오기
        elapsedTime = 0f;
        while (elapsedTime < Delay)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originAlpha, elapsedTime / Delay);
            SetAlpha(alpha);
            yield return null;
        }
    }
}
