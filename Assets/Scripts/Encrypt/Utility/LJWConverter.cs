using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 
/// STRConverter는 SpriteRenderer, TextMeshPro 및 TextMeshProUGUI, RectTransform, Transform 등의 컴포넌트의 값을 시간에 흐름에 따라서 제어하는 함수를 제공합니다. 
/// </summary>
/// <remarks> 
/// 씬이 바뀌더라도 유지되며 싱글톤을 이용하여 각 함수를 호출할 수 있습니다.
/// </remarks>
public class LJWConverter : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 패턴
    /// </summary>
    public static LJWConverter Instance { get; private set; }



    ///<summary> SpriteRenderer 색 </summary>
    private Dictionary<SpriteRenderer, Coroutine> SpriteRendererColorCoroutines { get; set; } = new Dictionary<SpriteRenderer, Coroutine>();
    ///<summary> SpriteRenderer 크기 </summary>
    private Dictionary<SpriteRenderer, Coroutine> SpriteRendererSizeCoroutines { get; set; } = new Dictionary<SpriteRenderer, Coroutine>();

    
    
    /// <summary> Image 색 </summary>
    private Dictionary<Image, Coroutine> ImageColorCoroutines { get; set; } = new Dictionary<Image, Coroutine>();
    
    

    ///<summary> TextMeshPro 색 </summary>
    private Dictionary<TextMeshPro, Coroutine> TMPColorCoroutines { get; set; } = new Dictionary<TextMeshPro, Coroutine>();
    ///<summary> TextMeshPro 폰트 크기 </summary>
    private Dictionary<TextMeshPro, Coroutine> TMPFontSizeCoroutines { get; set; } = new Dictionary<TextMeshPro, Coroutine>();
    ///<summary> TextMeshPro 텍스트 출력 </summary>
    private Dictionary<TextMeshPro, Coroutine> TMPPrintCoroutines { get; set; } = new Dictionary<TextMeshPro, Coroutine>();



    ///<summary> TextMeshProUGUI 색 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> UGUIColorCoroutine { get; set; } = new Dictionary<TextMeshProUGUI, Coroutine>();
    ///<summary> TextMeshProUGUI 폰트 크기 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> UGUIFontSizeCoroutines { get; set; } = new Dictionary<TextMeshProUGUI, Coroutine>();
    ///<summary> TextMeshProUGUI 텍스트 출력 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> UGUIPrintCoroutines { get; set; } = new Dictionary<TextMeshProUGUI, Coroutine>();

    
    
    ///<summary> RectTransform 크기 </summary>
    private Dictionary<RectTransform, Coroutine> RectTransformSizeDeltaCoroutines { get; set; } = new Dictionary<RectTransform, Coroutine>();

    
    private Dictionary<Transform, Coroutine> PeriodicXTransformPosCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> PeriodicYTransformPosCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> PeriodicZTransformPosCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();
    

    ///<summary> Transform 위치 </summary>
    private Dictionary<Transform, Coroutine> TransformPosCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();
    ///<summary> Transform 회전 </summary>
    private Dictionary<Transform, Coroutine> TransformRotationCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();
    ///<summary> Transform 크기 </summary>
    private Dictionary<Transform, Coroutine> TransformScaleCoroutines { get; set; } = new Dictionary<Transform, Coroutine>();



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
    
    
    
    /// <summary>
    /// 제네릭 키에 대응하는 코루틴 밸류가 딕셔너리에 있는지 확인하고, 없으면 새로운 코루틴을 추가하고, 있다면 기존 코루틴을 종료시키고 갱신
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="newCoroutine"></param>
    /// <typeparam name="T"></typeparam>
    private void CheckAndStartCoroutine<T>(IDictionary<T, Coroutine> dictionary, T key, Coroutine newCoroutine)
    {
        if (dictionary.ContainsKey(key))
        {
            StopCoroutine(dictionary[key]);
            dictionary[key] = newCoroutine;
        }
        else
            dictionary.Add(key, newCoroutine);
    }



    /// <summary>
    /// SpriteRenderer의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetSpriteRenderer"> 변환을 가할 SpriteRenderer </param>
    public void ConvertSpriteRendererColor(bool unscaledTime, float wait, float duration, Color endColor, SpriteRenderer targetSpriteRenderer)
    {
        CheckAndStartCoroutine(SpriteRendererColorCoroutines, targetSpriteRenderer,
            unscaledTime
                ? StartCoroutine(ConvertSpriteRendererColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetSpriteRenderer.color, targetSpriteRenderer))
                : StartCoroutine(ConvertSpriteRendererColor_ScaledTime(wait, duration, Time.time, endColor, targetSpriteRenderer.color, targetSpriteRenderer)));
    }
    private IEnumerator ConvertSpriteRendererColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (pastDeltaTime > duration + wait)//타임 아웃
        {
            targetSpriteRenderer.color = endColor;
            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererColorCoroutines.Remove(targetSpriteRenderer);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetSpriteRenderer.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer));
        }
    }
    private IEnumerator ConvertSpriteRendererColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (pastDeltaTime > duration + wait)//타임 아웃
        {
            targetSpriteRenderer.color = endColor;
            yield return new WaitForSeconds(0.02f);
            SpriteRendererColorCoroutines.Remove(targetSpriteRenderer);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetSpriteRenderer.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer));
        }
    }
    
    
    
    /// <summary>
    /// SpriteRenderer의 size를 wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 사이즈 </param>
    /// <param name="targetSpriteRenderer"> 변환을 가할 SpriteRenderer </param>
    public void ConvertSpriteRendererSize(bool unscaledTime, float wait, float duration, Vector2 endSize, SpriteRenderer targetSpriteRenderer)
    {
        CheckAndStartCoroutine(SpriteRendererSizeCoroutines, targetSpriteRenderer,
            unscaledTime
                ? StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetSpriteRenderer.size, targetSpriteRenderer))
                : StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, Time.time, endSize, targetSpriteRenderer.size, targetSpriteRenderer)));
    }
    private IEnumerator ConvertSpriteRendererSize_UnscaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, SpriteRenderer targetSpriteRenderer)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
    
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetSpriteRenderer.size = endSize;
            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererSizeCoroutines.Remove(targetSpriteRenderer);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetSpriteRenderer.size = Vector2.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer));
        }
    }
    private IEnumerator ConvertSpriteRendererSize_ScaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, SpriteRenderer targetSpriteRenderer)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
    
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetSpriteRenderer.size = endSize;
            yield return new WaitForSeconds(0.02f);
            SpriteRendererSizeCoroutines.Remove(targetSpriteRenderer);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetSpriteRenderer.size = Vector2.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer));
        }
    }


    
    /// <summary>
    /// Image의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"></param>
    /// <param name="wait"></param>
    /// <param name="duration"></param>
    /// <param name="endColor"></param>
    /// <param name="targetImage"></param>
    public void ConvertImageColor(bool unscaledTime, float wait, float duration, Color endColor, Image targetImage)
    {
        CheckAndStartCoroutine(ImageColorCoroutines, targetImage,
            unscaledTime
                ? StartCoroutine(ConvertImageColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetImage.color, targetImage))
                : StartCoroutine(ConvertImageColor_ScaledTime(wait, duration, Time.time, endColor, targetImage.color, targetImage)));
    }
    private IEnumerator ConvertImageColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, Image targetImage)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
    
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetImage.color = endColor;
            yield return new WaitForSecondsRealtime(0.02f);
            ImageColorCoroutines.Remove(targetImage);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(ConvertImageColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetImage));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetImage.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(ConvertImageColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetImage));
        }
    }
    private IEnumerator ConvertImageColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, Image targetImage)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
    
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetImage.color = endColor;
            yield return new WaitForSeconds(0.02f);
            ImageColorCoroutines.Remove(targetImage);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(ConvertImageColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetImage));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetImage.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(ConvertImageColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetImage));
        }
    }


    
    /// <summary>
    /// TextMeshPro의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetTMP"> 변환을 가할 TMP </param>
    public void ConvertTMPColor(bool unscaledTime, float wait, float duration, Color endColor, TextMeshPro targetTMP)
    {
        CheckAndStartCoroutine(TMPColorCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(ConvertTMPColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetTMP.color, targetTMP))
                : StartCoroutine(ConvertTMPColor_ScaledTime(wait, duration, Time.time, endColor, targetTMP.color, targetTMP)));
    }
    private IEnumerator ConvertTMPColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTMP.color = endColor;
            yield return new WaitForSecondsRealtime(0.02f);
            TMPColorCoroutines.Remove(targetTMP);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetTMP));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTMP.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetTMP));
        }
    }
    private IEnumerator ConvertTMPColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTMP.color = endColor;
            yield return new WaitForSeconds(0.02f);
            TMPColorCoroutines.Remove(targetTMP);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetTMP));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTMP.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetTMP));
        }
    }
    
    
    
    /// <summary>
    /// TextMeshPro의 fontSize를 wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 색상 </param>
    /// <param name="targetTMP"> 변환을 가할 TMP </param>
    public void ConvertTMPFontSize(bool unscaledTime, float wait, float duration, float endSize, TextMeshPro targetTMP)
    {
        CheckAndStartCoroutine(TMPFontSizeCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetTMP.fontSize, targetTMP))
                : StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, Time.time, endSize, targetTMP.fontSize, targetTMP)));
    }
    private IEnumerator ConvertTMPFontSize_UnscaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTMP.fontSize = endSize;
            yield return new WaitForSecondsRealtime(0.02f);
            TMPFontSizeCoroutines.Remove(targetTMP);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetTMP));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTMP.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetTMP));
        }
    }
    private IEnumerator ConvertTMPFontSize_ScaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        float pastDeltaTime = Time.time - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTMP.fontSize = endSize;
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines.Remove(targetTMP);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetTMP));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTMP.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetTMP));
        }
    }
    
    
    
    /// <summary>
    /// TextMeshPro의 text에 wait동안 대기한 후에 duration동안 value를 순차적으로 입력
    /// </summary>
    /// <param name="unscaledTime"></param>
    /// <param name="wait"></param>
    /// <param name="duration"></param>
    /// <param name="value"></param>
    /// <param name="clear"></param>
    /// <param name="targetTMP"></param>
    public void PrintTMPByDuration(bool unscaledTime, float wait, float duration, string value, bool clear, TextMeshPro targetTMP)
    {
        if(clear)
            targetTMP.text = "";
        
        CheckAndStartCoroutine(TMPPrintCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(PrintTMPByDuration_UnscaledTime(wait, duration, Time.unscaledTime, 0, value, targetTMP))
                : StartCoroutine(PrintTMPByDuration_ScaledTime(wait, duration, Time.time, 0, value, targetTMP)));
    }
    private IEnumerator PrintTMPByDuration_UnscaledTime(float wait, float duration, float startTime, int idx, string value, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPPrintCoroutines.Remove(targetTMP);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_UnscaledTime(wait, duration, startTime, idx, value, targetTMP));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetTMP.text += value[idx];
            yield return new WaitForSecondsRealtime(duration/value.Length);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_UnscaledTime(wait, duration, startTime, idx + 1, value, targetTMP));   
        }
    }
    private IEnumerator PrintTMPByDuration_ScaledTime(float wait, float duration, float startTime, int idx, string value, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSeconds(0.02f);
            TMPPrintCoroutines.Remove(targetTMP);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_ScaledTime(wait, duration, startTime, idx, value, targetTMP));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            targetTMP.text += value[idx];
            yield return new WaitForSeconds(duration/value.Length);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_ScaledTime(wait, duration, startTime, idx + 1, value, targetTMP));   
        }
    }

    
    
    /// <summary>
    /// TextMeshPro의 text에 wait동안 대기한 후에 delay간격으로 value를 순차적으로 입력
    /// </summary>
    /// <param name="unscaledTime"></param>
    /// <param name="wait"></param>
    /// <param name="delay"></param>
    /// <param name="value"></param>
    /// <param name="clear"></param>
    /// <param name="targetTMP"></param>
    public void PrintTMPByDelay(bool unscaledTime, float wait, float delay, string value, bool clear, TextMeshPro targetTMP)
    {
        if(clear)
            targetTMP.text = "";
        
        CheckAndStartCoroutine(TMPPrintCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(PrintTMPByDelay_UnscaledTime(wait, delay, Time.unscaledTime, 0, value, targetTMP))
                : StartCoroutine(PrintTMPByDelay_UnscaledTime(wait, delay, Time.time, 0, value, targetTMP)));
    }
    private IEnumerator PrintTMPByDelay_UnscaledTime(float wait, float delay, float startTime, int idx, string value, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPPrintCoroutines.Remove(targetTMP);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_UnscaledTime(wait, delay, Time.unscaledTime, idx, value, targetTMP));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetTMP.text += value[idx];
            yield return new WaitForSecondsRealtime(delay);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_ScaledTime(wait, delay, Time.unscaledTime, idx + 1, value, targetTMP));
        }
    }
    private IEnumerator PrintTMPByDelay_ScaledTime(float wait, float delay, float startTime, int idx, string value, TextMeshPro targetTMP)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSeconds(0.02f);
            TMPPrintCoroutines.Remove(targetTMP);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_ScaledTime(wait, delay, Time.time, idx, value, targetTMP));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetTMP.text += value[idx];
            yield return new WaitForSeconds(delay);
            TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_ScaledTime(wait, delay, Time.time, idx + 1, value, targetTMP));
        }
    }
    
    
    
    /// <summary>
    /// TextMeshPro에 진행 중인 출력 작업 정지 
    /// </summary>
    /// <param name="targetTMP"></param>
    public void StopPrintingTMP(TextMeshPro targetTMP)
    {
        if(!TMPPrintCoroutines.ContainsKey(targetTMP))
            return;
    
        StopCoroutine(TMPPrintCoroutines[targetTMP]);
        TMPPrintCoroutines.Remove(targetTMP);
    }
    
    
    
    /// <summary>
    /// TextMeshPro에 진행 중인 출력 작업 유무
    /// </summary>
    /// <param name="targetTMP"></param>
    /// <returns></returns>
    public bool GetIsPrintingTMP(TextMeshPro targetTMP)
    {
        return TMPPrintCoroutines.ContainsKey(targetTMP);
    }



    /// <summary>
    /// TextMeshProUGUI의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetUGUI"> 변환을 가할 TextMeshProUGUI </param>
    public void ConvertUGUIColor(bool unscaledTime, float wait, float duration, Color endColor, TextMeshProUGUI targetUGUI)
    {
        CheckAndStartCoroutine(UGUIColorCoroutine, targetUGUI,
            unscaledTime
                ? StartCoroutine(ConvertUGUIColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetUGUI.color, targetUGUI))
                : StartCoroutine(ConvertUGUIColor_ScaledTime(wait, duration, Time.time, endColor, targetUGUI.color, targetUGUI)));
    }
    private IEnumerator ConvertUGUIColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (pastDeltaTime > wait + duration)//타임 아웃
        {
            targetUGUI.color = endColor;
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIColorCoroutine.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(ConvertUGUIColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetUGUI));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            targetUGUI.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(ConvertUGUIColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetUGUI));
        }
    }
    private IEnumerator ConvertUGUIColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (pastDeltaTime > wait + duration)//타임 아웃
        {
            targetUGUI.color = endColor;
            yield return new WaitForSeconds(0.02f);
            UGUIColorCoroutine.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(ConvertUGUIColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetUGUI));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            targetUGUI.color = Color.Lerp(startColor, endColor, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            yield return new WaitForSeconds(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(ConvertUGUIColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetUGUI));
        }
    }

    
    
    /// <summary>
    /// TextMeshProUGUI의 fontSize wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 색상 </param>
    /// <param name="targetUGUI"> 변환을 가할 TextMeshProUGUI </param>
    public void ConvertUGUIFontSize(bool unscaledTime, float wait, float duration, float endSize, TextMeshProUGUI targetUGUI)
    {
        CheckAndStartCoroutine(UGUIFontSizeCoroutines, targetUGUI,
            unscaledTime
                ? StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetUGUI.fontSize, targetUGUI))
                : StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, Time.time, endSize, targetUGUI.fontSize, targetUGUI)));
    }
    private IEnumerator ConvertUGUIFontSize_UnscaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (pastDeltaTime > wait + duration)//타임 아웃
        {
            targetUGUI.fontSize = endSize;
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIFontSizeCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetUGUI));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            targetUGUI.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetUGUI));
        }
    }
    private IEnumerator ConvertUGUIFontSize_ScaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (pastDeltaTime > wait + duration)//타임 아웃
        {
            targetUGUI.fontSize = endSize;
            yield return new WaitForSeconds(0.02f);
            UGUIFontSizeCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetUGUI));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            targetUGUI.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            yield return new WaitForSeconds(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetUGUI));
        }
    }
    
    
    
    /// <summary>
    /// TextMeshProUGUI의 text에 wait동안 대기한 후에 duration동안 value를 순차적으로 입력
    /// </summary>
    /// <param name="unscaledTime"></param>
    /// <param name="wait"></param>
    /// <param name="duration"></param>
    /// <param name="value"></param>
    /// <param name="clear"></param>
    /// <param name="targetUGUI"></param>
    public void PrintUGUIByDuration(bool unscaledTime, float wait, float duration, string value, bool clear, TextMeshProUGUI targetUGUI)
    {
        if(clear)
           targetUGUI.text = "";
        
        CheckAndStartCoroutine(UGUIPrintCoroutines, targetUGUI,
            unscaledTime
                ? StartCoroutine(PrintUGUIByDuration_UnscaledTime(wait, duration, Time.unscaledTime, 0, value, targetUGUI))
                : StartCoroutine(PrintUGUIByDuration_ScaledTime(wait, duration, Time.time, 0, value, targetUGUI)));
    }
    private IEnumerator PrintUGUIByDuration_UnscaledTime(float wait, float duration, float startTime, int idx, string value, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIPrintCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDuration_UnscaledTime(wait, duration, startTime, idx, value, targetUGUI));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetUGUI.text += value[idx];
            yield return new WaitForSecondsRealtime(duration/value.Length);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDuration_UnscaledTime(wait, duration, startTime, idx + 1, value, targetUGUI));   
        }
    }
    private IEnumerator PrintUGUIByDuration_ScaledTime(float wait, float duration, float startTime, int idx, string value, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSeconds(0.02f);
            UGUIPrintCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDuration_ScaledTime(wait, duration, startTime, idx, value, targetUGUI));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetUGUI.text += value[idx];
            yield return new WaitForSeconds(duration/value.Length);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDuration_ScaledTime(wait, duration, startTime, idx + 1, value, targetUGUI));   
        }
    }

    
    
    /// <summary>
    /// TextMeshProUGUI의 text에 wait동안 대기한 후에 delay간격으로 value를 순차적으로 입력
    /// </summary>
    /// <param name="unscaledTime"></param>
    /// <param name="wait"></param>
    /// <param name="delay"></param>
    /// <param name="value"></param>
    /// <param name="clear"></param>
    /// <param name="targetUGUI"></param>
    public void PrintUGUIByDelay(bool unscaledTime, float wait, float delay, string value, bool clear, TextMeshProUGUI targetUGUI)
    {
        if(clear)
            targetUGUI.text = "";
        
        CheckAndStartCoroutine(UGUIPrintCoroutines, targetUGUI,
            unscaledTime
                ? StartCoroutine(PrintUGUIByDelay_UnscaledTime(wait, delay, Time.unscaledTime, 0, value, targetUGUI))
                : StartCoroutine(PrintUGUIByDelay_ScaledTime(wait, delay, Time.time, 0, value, targetUGUI)));
    }
    private IEnumerator PrintUGUIByDelay_UnscaledTime(float wait, float delay, float startTime, int idx, string value, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIPrintCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDelay_UnscaledTime(wait, delay, Time.unscaledTime, idx, value, targetUGUI));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetUGUI.text += value[idx];
            yield return new WaitForSecondsRealtime(delay);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDelay_UnscaledTime(wait, delay, Time.unscaledTime, idx + 1, value, targetUGUI));
        }
    }
    private IEnumerator PrintUGUIByDelay_ScaledTime(float wait, float delay, float startTime, int idx, string value, TextMeshProUGUI targetUGUI)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (idx == value.Length)//출력 종료
        {
            yield return new WaitForSeconds(0.02f);
            UGUIPrintCoroutines.Remove(targetUGUI);
        }
        else if (pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDelay_ScaledTime(wait, delay, Time.time, idx, value, targetUGUI));
        }
        else if (pastDeltaTime >= wait) //액티브
        {
            targetUGUI.text += value[idx];
            yield return new WaitForSeconds(delay);
            UGUIPrintCoroutines[targetUGUI] = StartCoroutine(PrintUGUIByDelay_ScaledTime(wait, delay, Time.time, idx + 1, value, targetUGUI));
        }
    }
    
    
    
    /// <summary>
    /// TextMeshProUGUI에 진행 중인 출력 작업 정지 
    /// </summary>
    /// <param name="targetUGUI"></param>
    public void StopPrintingUGUI(TextMeshProUGUI targetUGUI)
    {
        if(!UGUIPrintCoroutines.ContainsKey(targetUGUI))
            return;
    
        StopCoroutine(UGUIPrintCoroutines[targetUGUI]);
        UGUIPrintCoroutines.Remove(targetUGUI);
    }
    
    
    
    /// <summary>
    /// TextMeshProUGUI에 진행 중인 출력 작업 유무
    /// </summary>
    /// <param name="targetUGUI"></param>
    /// <returns></returns>
    public bool GetIsPrintingUGUI(TextMeshProUGUI targetUGUI)
    {
        return UGUIPrintCoroutines.ContainsKey(targetUGUI);
    }
    
    
    
    /// <summary>
    /// RectTransform의 sizeDelta를 wait동안 대기한 후에 duration동안 endSize로 선형 변환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 사이즈 </param>
    /// <param name="targetRectTransform"> 변환을 가할 RectTransform </param>
    public void ConvertRectTransformSize(bool unscaledTime, float wait, float duration, Vector2 endSize, RectTransform targetRectTransform)
    {
        CheckAndStartCoroutine(RectTransformSizeDeltaCoroutines, targetRectTransform,
            unscaledTime
                ? StartCoroutine(ConvertRectTransformSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetRectTransform.sizeDelta, targetRectTransform))
                : StartCoroutine(ConvertRectTransformSize_ScaledTime(wait, duration, Time.time, endSize, targetRectTransform.sizeDelta, targetRectTransform)));
    }
    private IEnumerator ConvertRectTransformSize_UnscaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, RectTransform targetRectTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetRectTransform.sizeDelta = endSize;
            yield return new WaitForSecondsRealtime(0.02f);
            RectTransformSizeDeltaCoroutines.Remove(targetRectTransform);
            yield break;
        }
        if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(ConvertRectTransformSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform));
            yield break;
        }
        if(pastDeltaTime >= wait)//액티브
        {
            targetRectTransform.sizeDelta = Vector2.Lerp(new Vector2(startSize.x, startSize.y), new Vector2(endSize.x, endSize.y), Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(ConvertRectTransformSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform));
        }
    }
    private IEnumerator ConvertRectTransformSize_ScaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, RectTransform targetRectTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetRectTransform.sizeDelta = endSize;
            yield return new WaitForSeconds(0.02f);
            RectTransformSizeDeltaCoroutines.Remove(targetRectTransform);
            yield break;
        }
        if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(ConvertRectTransformSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform));
            yield break;
        }
        if(pastDeltaTime >= wait)//액티브
        {
            targetRectTransform.sizeDelta = Vector2.Lerp(new Vector2(startSize.x, startSize.y), new Vector2(endSize.x, endSize.y), Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(ConvertRectTransformSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform));
        }
    }



    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 endPos로 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endPos"> 목표로 하는 종료 위치 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void ConvertTransformPos(bool unscaledTime, float wait, float duration, Vector3 endPos, Transform targetTransform)
    {
        CheckAndStartCoroutine(TransformPosCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(ConvertTransformPos_UnscaledTime(wait, duration, Time.unscaledTime, endPos, targetTransform.localPosition, targetTransform))
                : StartCoroutine(ConvertTransformPos_ScaledTime(wait, duration, Time.time, endPos, targetTransform.localPosition, targetTransform)));    
    }
    private IEnumerator ConvertTransformPos_UnscaledTime(float wait, float duration, float startTime, Vector3 endPos, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = endPos;
            yield return new WaitForSecondsRealtime(0.02f);
            TransformPosCoroutines.Remove(targetTransform);
            yield break;
        }

        if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_UnscaledTime(wait, duration, startTime, endPos, startPos, targetTransform));
            yield break;
        }

        if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSecondsRealtime(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_UnscaledTime(wait, duration, startTime, endPos, startPos, targetTransform));
        }
    }
    private IEnumerator ConvertTransformPos_ScaledTime(float wait, float duration, float startTime, Vector3 endPos, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = endPos;
            yield return new WaitForSeconds(0.02f);
            TransformPosCoroutines.Remove(targetTransform);
            yield break;
        }

        if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_ScaledTime(wait, duration, startTime, endPos, startPos, targetTransform));
            yield break;
        }

        if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);

            yield return new WaitForSeconds(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_ScaledTime(wait, duration, startTime, endPos, startPos, targetTransform));
        }
    }

    
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 Y축에 대하여 주기 운동한다 - 흔든다
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="periods"> 실행 시간 동안 몇 주기를 반복하는가 - 사인 함수의 주기와 주파수를 결정 </param>
    /// <param name="amplitude"> 사인 함수의 진폭 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void MovePeriodicXTransformPos(bool unscaledTime, float wait, float duration, int periods, float amplitude, Transform targetTransform)
    {
        if (PeriodicXTransformPosCoroutines.ContainsKey(targetTransform))
            return;
        
        CheckAndStartCoroutine(PeriodicXTransformPosCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(MovePeriodicXTransformPos_UnscaledTime(wait, duration, Time.unscaledTime, periods, amplitude, targetTransform.localPosition, targetTransform))
                : StartCoroutine(MovePeriodicXTransformPos_ScaledTime(wait, duration, Time.time, periods, amplitude, targetTransform.localPosition, targetTransform)));   
    }
    private IEnumerator MovePeriodicXTransformPos_UnscaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicXTransformPosCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicXTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicXTransformPos_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration), 0f, 0f);

            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicXTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicXTransformPos_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    private IEnumerator MovePeriodicXTransformPos_ScaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSeconds(0.02f);
            PeriodicXTransformPosCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            PeriodicXTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicXTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration), 0f, 0f);

            yield return new WaitForSeconds(0.02f);
            PeriodicXTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicXTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 X축에 대하여 주기 운동한다 - 흔든다
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="periods"> 실행 시간 동안 몇 주기를 반복하는가 - 사인 함수의 주기와 주파수를 결정 </param>
    /// <param name="amplitude"> 사인 함수의 진폭 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void MovePeriodicYTransformPos(bool unscaledTime, float wait, float duration, int periods, float amplitude, Transform targetTransform)
    {
        if (PeriodicYTransformPosCoroutines.ContainsKey(targetTransform))
            return;
        
        CheckAndStartCoroutine(PeriodicYTransformPosCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(MovePeriodicYTransform_UnscaledTime(wait, duration, Time.unscaledTime, periods, amplitude, targetTransform.localPosition, targetTransform))
                : StartCoroutine(MovePeriodicYTransformPos_ScaledTime(wait, duration, Time.time, periods, amplitude, targetTransform.localPosition, targetTransform)));
    }
    private IEnumerator MovePeriodicYTransform_UnscaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicYTransformPosCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicYTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicYTransform_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration), 0f);

            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicYTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicYTransform_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    private IEnumerator MovePeriodicYTransformPos_ScaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSeconds(0.02f);
            PeriodicYTransformPosCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            PeriodicYTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicYTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration), 0f);

            yield return new WaitForSeconds(0.02f);
            PeriodicYTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicYTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 Z축에 대하여 주기 운동한다 - 흔든다
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="periods"> 실행 시간 동안 몇 주기를 반복하는가 - 사인 함수의 주기와 주파수를 결정 </param>
    /// <param name="amplitude"> 사인 함수의 진폭 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void MovePeriodicZTransformPos(bool unscaledTime, float wait, float duration, int periods, float amplitude, Transform targetTransform)
    {
        if (PeriodicZTransformPosCoroutines.ContainsKey(targetTransform))
            return;
        
        CheckAndStartCoroutine(PeriodicZTransformPosCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(MovePeriodicZTransformPos_UnscaledTime(wait, duration, Time.unscaledTime, periods, amplitude, targetTransform.localPosition, targetTransform))
                : StartCoroutine(MovePeriodicZTransformPos_ScaledTime(wait, duration, Time.time, periods, amplitude, targetTransform.localPosition, targetTransform)));
    }
    private IEnumerator MovePeriodicZTransformPos_UnscaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicZTransformPosCoroutines.Remove(targetTransform);
        }
        else if (pastDeltaTime < wait) //대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicZTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicZTransformPos_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(0f, 0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration));

            yield return new WaitForSecondsRealtime(0.02f);
            PeriodicZTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicZTransformPos_UnscaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    private IEnumerator MovePeriodicZTransformPos_ScaledTime(float wait, float duration, float startTime, int periods, float amplitude, Vector3 startPos, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTransform.localPosition = startPos;
            yield return new WaitForSeconds(0.02f);
            PeriodicZTransformPosCoroutines.Remove(targetTransform);
        }
        else if (pastDeltaTime < wait) //대기
        {
            yield return new WaitForSeconds(0.02f);
            PeriodicZTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicZTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localPosition = startPos + new Vector3(0f, 0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * Mathf.Max(0.01f, pastDeltaTime - wait) / duration));

            yield return new WaitForSeconds(0.02f);
            PeriodicZTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicZTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    
    
    /// <summary>
    /// Transform의 localRotation을 wait동안 대기한 후에 duration동안 endRot로 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endRot"> 목표로 하는 종료 각도 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void ConvertTransformRot(bool unscaledTime, float wait, float duration, Vector3 endRot, Transform targetTransform)
    {
        CheckAndStartCoroutine(TransformRotationCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(ConvertTransformRot_UnscaledTime(wait, duration, Time.unscaledTime, endRot, targetTransform.localEulerAngles, targetTransform))
                : StartCoroutine(ConvertTransformRot_ScaledTime(wait, duration, Time.time, endRot, targetTransform.localEulerAngles, targetTransform)));
    }
    private IEnumerator ConvertTransformRot_UnscaledTime(float wait, float duration, float startTime, Vector3 endRot, Vector3 startRot, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (pastDeltaTime > duration + wait) //타임 아웃
        {
            targetTransform.localRotation = Quaternion.Euler(endRot);
            yield return new WaitForSecondsRealtime(0.02f);
            TransformRotationCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRot_UnscaledTime(wait, duration, startTime, endRot, startRot, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRot_UnscaledTime(wait, duration, startTime, endRot, startRot, targetTransform));
        }
    }
    private IEnumerator ConvertTransformRot_ScaledTime(float wait, float duration, float startTime, Vector3 endRot, Vector3 startRot, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (pastDeltaTime > duration + wait) //타임 아웃
        {
            targetTransform.localRotation = Quaternion.Euler(endRot);
            yield return new WaitForSeconds(0.02f);
            TransformRotationCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRot_ScaledTime(wait, duration, startTime, endRot, startRot, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSeconds(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRot_ScaledTime(wait, duration, startTime, endRot, startRot, targetTransform));
        }
    }

    
    
    /// <summary>
    /// Transform의 localScale을 wait동안 대기한 후에 duration동안 endScale로 전환
    /// </summary>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endScale"> 목표로 하는 종료 크기 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    public void ConvertTransformScale(bool unscaledTime, float wait, float duration, Vector3 endScale, Transform targetTransform)
    {
        CheckAndStartCoroutine(TransformScaleCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(ConvertTransformScale_UnscaledTime(wait, duration, Time.unscaledTime, endScale, targetTransform.localScale, targetTransform))
                : StartCoroutine(ConvertTransformScale_ScaledTime(wait, duration, Time.time, endScale, targetTransform.localScale, targetTransform)));    
    }
    private IEnumerator ConvertTransformScale_UnscaledTime(float wait, float duration, float startTime, Vector3 endScale, Vector3 startScale, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.unscaledTime - startTime;

        if (pastDeltaTime > duration + wait) //타임 아웃
        {
            targetTransform.localScale = endScale;
            yield return new WaitForSecondsRealtime(0.02f);
            TransformScaleCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_UnscaledTime(wait, duration, startTime, endScale, startScale, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, pastDeltaTime / duration);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_UnscaledTime(wait, duration, startTime, endScale, startScale, targetTransform));
        }
    }
    private IEnumerator ConvertTransformScale_ScaledTime(float wait, float duration, float startTime, Vector3 endScale, Vector3 startScale, Transform targetTransform)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;

        if (pastDeltaTime > duration + wait) //타임 아웃
        {
            targetTransform.localScale = endScale;
            yield return new WaitForSeconds(0.02f);
            TransformScaleCoroutines.Remove(targetTransform);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_ScaledTime(wait, duration, startTime, endScale, startScale, targetTransform));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, pastDeltaTime / duration);
            
            yield return new WaitForSeconds(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_ScaledTime(wait, duration, startTime, endScale, startScale, targetTransform));
        }
    }
    
    
    
}