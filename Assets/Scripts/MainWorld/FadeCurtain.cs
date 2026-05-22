using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utility;


public enum FadeMode
{
    /// <summary>
    /// 점점 밝아지기 효과
    /// </summary>
    In,
    
    /// <summary>
    /// 점점 어두워지기 효과
    /// </summary>
    Out
}

public class FadeCurtain : MonoBehaviour
{
    public float delay;
    [SerializeField] private Image image;
    
    public bool isLoading = false;
    
    /// <summary>
    /// 화면 전환 효과
    /// </summary>
    public void Fade(FadeMode mode)
    {
        if (mode == FadeMode.In)
        {
            StartCoroutine(IE_FadeIn());
        }
        else if (mode == FadeMode.Out)
        {
            StartCoroutine(IE_FadeOut());
        }
    }
    
    private IEnumerator IE_FadeIn()
    {
        isLoading = true;
        
        float elapsedTime = 0f;
        
        // 점점 밝아지기
        while (elapsedTime < delay)
        {
            image.color = Color.Lerp(Color.black, Color.clear, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isLoading = false;
    }

    private IEnumerator IE_FadeOut()
    {
        isLoading = true;
        
        float elapsedTime = 0f;
        
        // 점점 어두워지기
        while (elapsedTime < delay)
        {
            image.color = Color.Lerp(Color.clear, Color.black, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isLoading = false;
    }
}
