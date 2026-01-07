using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeCurtain : MonoBehaviour
{
    public float delay;
    [SerializeField] private Image image;
    
    /// <summary>
    /// 화면 전환 효과
    /// </summary>
    public void Fade()
    {
        StartCoroutine(FadeInOut());
    }
    
    
    private IEnumerator FadeInOut()
    {
        float elapsedTime = 0f;
        
        // 점점 밝아지기
        while (elapsedTime < delay)
        {
            image.color = Color.Lerp(Color.black, Color.clear, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
