using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlowEffect : MonoBehaviour
{
    private Image texture;
    public float Delay;
    public bool Loop;
    private float originAlpha = 1;
    public float minimumAlpha = 0;

    private void Awake()
    {
        if (TryGetComponent(out texture))
        {
            originAlpha = texture.color.a;
        }
        else
        {
            Debug.LogWarning($"Warning: Glow Effect on the NOT Image Object");
            Destroy(this);
        }
    }
    
    
    private void OnEnable()
    {
        SetAlpha(originAlpha);
        StartCoroutine(GlowAnimationLoop());
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private void SetAlpha(float alpha)
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
    private IEnumerator GlowAnimationLoop()
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

        // 점점 투명해지기
        while (elapsedTime < Delay)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originAlpha, minimumAlpha, elapsedTime / Delay);
            SetAlpha(alpha);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        
        // 점점 돌아오기
        elapsedTime = 0f;
        while (elapsedTime < Delay)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(minimumAlpha, originAlpha, elapsedTime / Delay);
            SetAlpha(alpha);
            yield return null;
        }
    }
}
