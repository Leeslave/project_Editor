using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartIntro : MonoBehaviour
{
    public int footCount;
    public Image left;
    public Image right;
    public float footDelay;
    public Image Logo;
    public float logoDuration;

    public void Awake()
    {
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(0.4f);
        for(int i = 0; i < footCount; i++)
        {
            yield return FadeOut(left,footDelay);
            yield return FadeOut(right,footDelay);
        }
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        yield return FadeIn(Logo, logoDuration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOut(Image image, float delay)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < delay)
        {
            image.color = Color.Lerp(Color.white, Color.black, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(Image image, float delay)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < delay)
        {
            image.color = Color.Lerp(Color.black, Color.white, elapsedTime / delay);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
