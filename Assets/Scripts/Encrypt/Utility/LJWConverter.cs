using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 
/// LJWConverter는 SpriteRenderer, TextMeshPro 및 TextMeshProUGUI, RectTransform, Transform 등의 컴포넌트의 값을 시간에 흐름에 따라서 제어하는 함수를 제공합니다. 
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
        if (dictionary.TryAdd(key, newCoroutine)) 
            return;
        StopCoroutine(dictionary[key]);
        dictionary[key] = newCoroutine;
    }
    
    /// <summary>
    /// 제네릭 키에 대응하는 코루틴 밸류가 딕셔너리에 있는지 확인하고, 있다면 기존 코루틴을 종료시키고 딕셔너리에서 삭제한다
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    private void CheckAndStopCoroutine<T>(IDictionary<T, Coroutine> dictionary, T key)
    {
        if (!dictionary.TryGetValue(key, out var value)) 
            return;
        
        StopCoroutine(value);
        dictionary.Remove(key);
    }

    #region 1) SpriteRenderer 관련 함수

    /// <summary>
    /// SpriteRenderer의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetSpriteRenderer"> 목표 SpriteRenderer 컴포넌트 </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void GradientSpriteRendererColor(bool unscaledTime, float wait, float duration, Color endColor, SpriteRenderer targetSpriteRenderer, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(SpriteRendererColorCoroutines, targetSpriteRenderer,
            unscaledTime
                ? StartCoroutine(GradientSpriteRendererColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetSpriteRenderer.color, targetSpriteRenderer, curve))
                : StartCoroutine(GradientSpriteRendererColor_ScaledTime(wait, duration, Time.time, endColor, targetSpriteRenderer.color, targetSpriteRenderer, curve)));
    }
    private IEnumerator GradientSpriteRendererColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer, AnimationCurve curve)
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
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(GradientSpriteRendererColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetSpriteRenderer.color = Color.Lerp(startColor, endColor, evaluation);

            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(GradientSpriteRendererColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer, curve));
        }
    }
    private IEnumerator GradientSpriteRendererColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer, AnimationCurve curve)
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
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(GradientSpriteRendererColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetSpriteRenderer.color = Color.Lerp(startColor, endColor, evaluation);
            
            yield return new WaitForSeconds(0.02f);
            SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(GradientSpriteRendererColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetSpriteRenderer, curve));
        }
    }

    /// <summary>
    /// SpriteRenderer의 size를 wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 사이즈 </param>
    /// <param name="targetSpriteRenderer"> 목표 SpriteRenderer 컴포넌트 </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void ConvertSpriteRendererSize(bool unscaledTime, float wait, float duration, Vector2 endSize, SpriteRenderer targetSpriteRenderer, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(SpriteRendererSizeCoroutines, targetSpriteRenderer,
            unscaledTime
                ? StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetSpriteRenderer.size, targetSpriteRenderer, curve))
                : StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, Time.time, endSize, targetSpriteRenderer.size, targetSpriteRenderer, curve)));
    }
    private IEnumerator ConvertSpriteRendererSize_UnscaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, SpriteRenderer targetSpriteRenderer, AnimationCurve curve)
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
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetSpriteRenderer.size = Vector2.Lerp(startSize, endSize, evaluation);

            yield return new WaitForSecondsRealtime(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer, curve));
        }
    }
    private IEnumerator ConvertSpriteRendererSize_ScaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, SpriteRenderer targetSpriteRenderer, AnimationCurve curve)
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
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetSpriteRenderer.size = Vector2.Lerp(startSize, endSize, evaluation);

            yield return new WaitForSeconds(0.02f);
            SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetSpriteRenderer, curve));
        }
    }

    #endregion
    
    #region 2) Image 관련 함수
    
    /// <summary>
    /// Image의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetImage"> 목표 이미지 컴포넌트 </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void GradientImageColor(bool unscaledTime, float wait, float duration, Color endColor, Image targetImage, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(ImageColorCoroutines, targetImage,
            unscaledTime
                ? StartCoroutine(GradientImageColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetImage.color, targetImage, curve))
                : StartCoroutine(GradientImageColor_ScaledTime(wait, duration, Time.time, endColor, targetImage.color, targetImage, curve)));
    }
    private IEnumerator GradientImageColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, Image targetImage, AnimationCurve curve)
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
            ImageColorCoroutines[targetImage] = StartCoroutine(GradientImageColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetImage, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetImage.color = Color.Lerp(startColor, endColor, evaluation);
            
            yield return new WaitForSecondsRealtime(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(GradientImageColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetImage, curve));
        }
    }
    private IEnumerator GradientImageColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, Image targetImage, AnimationCurve curve)
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
            ImageColorCoroutines[targetImage] = StartCoroutine(GradientImageColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetImage, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetImage.color = Color.Lerp(startColor, endColor, evaluation);

            yield return new WaitForSeconds(0.02f);
            ImageColorCoroutines[targetImage] = StartCoroutine(GradientImageColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetImage, curve));
        }
    }

    #endregion
    
    #region 3) TextMeshPro 관련 함수

    /// <summary>
    /// TextMeshPro의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetTMP"> 목표 TMP 컴포넌트 </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void GradientTMPColor(bool unscaledTime, float wait, float duration, Color endColor, TextMeshPro targetTMP, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(TMPColorCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(GradientTMPColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetTMP.color, targetTMP, curve))
                : StartCoroutine(GradientTMPColor_ScaledTime(wait, duration, Time.time, endColor, targetTMP.color, targetTMP, curve)));
    }
    private IEnumerator GradientTMPColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshPro targetTMP, AnimationCurve curve)
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
            TMPColorCoroutines[targetTMP] = StartCoroutine(GradientTMPColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetTMP, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTMP.color = Color.Lerp(startColor, endColor, evaluation);

            yield return new WaitForSecondsRealtime(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(GradientTMPColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetTMP, curve));
        }
    }
    private IEnumerator GradientTMPColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshPro targetTMP, AnimationCurve curve)
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
            TMPColorCoroutines[targetTMP] = StartCoroutine(GradientTMPColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetTMP, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTMP.color = Color.Lerp(startColor, endColor, evaluation);

            yield return new WaitForSeconds(0.02f);
            TMPColorCoroutines[targetTMP] = StartCoroutine(GradientTMPColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetTMP, curve));
        }
    }

    /// <summary>
    /// TextMeshPro의 fontSize를 wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 색상 </param>
    /// <param name="targetTMP"> 변환을 가할 TMP </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void ConvertTMPFontSize(bool unscaledTime, float wait, float duration, float endSize, TextMeshPro targetTMP, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(TMPFontSizeCoroutines, targetTMP,
            unscaledTime
                ? StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetTMP.fontSize, targetTMP, curve))
                : StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, Time.time, endSize, targetTMP.fontSize, targetTMP, curve)));
    }
    private IEnumerator ConvertTMPFontSize_UnscaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshPro targetTMP, AnimationCurve curve)
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
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetTMP, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTMP.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetTMP, curve));
        }
    }
    private IEnumerator ConvertTMPFontSize_ScaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshPro targetTMP, AnimationCurve curve)
    {
        //작업 개시로부터 지난 시간
        var pastDeltaTime = Time.time - startTime;
        
        if(pastDeltaTime > duration + wait)//타임 아웃
        {
            targetTMP.fontSize = endSize;
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines.Remove(targetTMP);
        }
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetTMP, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTMP.fontSize = Mathf.Lerp(startSize, endSize, Mathf.Max(0.01f, pastDeltaTime - wait) / duration);
            
            yield return new WaitForSeconds(0.02f);
            TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetTMP, curve));
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
        else if (pastDeltaTime >= wait) //액티브
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

    #endregion

    #region 4) TextMeshProUGUI 관련 함수

    /// <summary>
    /// TextMeshProUGUI의 color를 wait동안 대기한 후에 duration동안 endColor로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endColor"> 목표로 하는 종료 색상 </param>
    /// <param name="targetUGUI"> 변환을 가할 TextMeshProUGUI </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void GradientUGUIColor(bool unscaledTime, float wait, float duration, Color endColor, TextMeshProUGUI targetUGUI, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(UGUIColorCoroutine, targetUGUI,
            unscaledTime
                ? StartCoroutine(GradientUGUIColor_UnscaledTime(wait, duration, Time.unscaledTime, endColor, targetUGUI.color, targetUGUI, curve))
                : StartCoroutine(GradientUGUIColor_ScaledTime(wait, duration, Time.time, endColor, targetUGUI.color, targetUGUI, curve)));
    }
    private IEnumerator GradientUGUIColor_UnscaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshProUGUI targetUGUI, AnimationCurve curve)
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
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(GradientUGUIColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetUGUI, curve));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetUGUI.color = Color.Lerp(startColor, endColor, evaluation);
            
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(GradientUGUIColor_UnscaledTime(wait, duration, startTime, endColor, startColor, targetUGUI, curve));
        }
    }
    private IEnumerator GradientUGUIColor_ScaledTime(float wait, float duration, float startTime, Color endColor, Color startColor, TextMeshProUGUI targetUGUI, AnimationCurve curve)
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
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(GradientUGUIColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetUGUI, curve));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetUGUI.color = Color.Lerp(startColor, endColor, evaluation);
            
            yield return new WaitForSeconds(0.02f);
            UGUIColorCoroutine[targetUGUI] = StartCoroutine(GradientUGUIColor_ScaledTime(wait, duration, startTime, endColor, startColor, targetUGUI, curve));
        }
    }

    /// <summary>
    /// TextMeshProUGUI의 fontSize wait동안 대기한 후에 duration동안 endSize로 선형 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 색상 </param>
    /// <param name="targetUGUI"> 변환을 가할 TextMeshProUGUI </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void ConvertUGUIFontSize(bool unscaledTime, float wait, float duration, float endSize, TextMeshProUGUI targetUGUI, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(UGUIFontSizeCoroutines, targetUGUI,
            unscaledTime
                ? StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetUGUI.fontSize, targetUGUI, curve))
                : StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, Time.time, endSize, targetUGUI.fontSize, targetUGUI, curve)));
    }
    private IEnumerator ConvertUGUIFontSize_UnscaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshProUGUI targetUGUI, AnimationCurve curve)
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
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetUGUI, curve));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetUGUI.fontSize = Mathf.Lerp(startSize, endSize, evaluation);
            
            yield return new WaitForSecondsRealtime(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_UnscaledTime(wait, duration, startTime, endSize, startSize, targetUGUI, curve));
        }
    }
    private IEnumerator ConvertUGUIFontSize_ScaledTime(float wait, float duration, float startTime, float endSize, float startSize, TextMeshProUGUI targetUGUI, AnimationCurve curve)
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
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetUGUI, curve));
        }
        else if (pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetUGUI.fontSize = Mathf.Lerp(startSize, endSize, evaluation);
            
            yield return new WaitForSeconds(0.02f);
            UGUIFontSizeCoroutines[targetUGUI] = StartCoroutine(ConvertUGUIFontSize_ScaledTime(wait, duration, startTime, endSize, startSize, targetUGUI, curve));
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
    
    #endregion

    #region 5) RectTransform 관련 함수

    /// <summary>
    /// RectTransform의 sizeDelta를 wait동안 대기한 후에 duration동안 endSize로 선형 변환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endSize"> 목표로 하는 종료 사이즈 </param>
    /// <param name="targetRectTransform"> 변환을 가할 RectTransform </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void SizeRectTransform(bool unscaledTime, float wait, float duration, Vector2 endSize, RectTransform targetRectTransform, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(RectTransformSizeDeltaCoroutines, targetRectTransform,
            unscaledTime
                ? StartCoroutine(SizeRectTransform_UnscaledTime(wait, duration, Time.unscaledTime, endSize, targetRectTransform.sizeDelta, targetRectTransform, curve))
                : StartCoroutine(SizeRectTransform_ScaledTime(wait, duration, Time.time, endSize, targetRectTransform.sizeDelta, targetRectTransform, curve)));
    }
    private IEnumerator SizeRectTransform_UnscaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, RectTransform targetRectTransform, AnimationCurve curve)
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
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(SizeRectTransform_UnscaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform, curve));
            yield break;
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetRectTransform.sizeDelta = Vector2.Lerp(new Vector2(startSize.x, startSize.y), new Vector2(endSize.x, endSize.y), evaluation);

            yield return new WaitForSecondsRealtime(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(SizeRectTransform_UnscaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform, curve));
        }
    }
    private IEnumerator SizeRectTransform_ScaledTime(float wait, float duration, float startTime, Vector2 endSize, Vector2 startSize, RectTransform targetRectTransform, AnimationCurve curve)
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
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(SizeRectTransform_ScaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform, curve));
            yield break;
        }
        else if(pastDeltaTime >= wait)//액티브
        {            
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetRectTransform.sizeDelta = Vector2.Lerp(new Vector2(startSize.x, startSize.y), new Vector2(endSize.x, endSize.y), evaluation);

            yield return new WaitForSeconds(0.02f);
            RectTransformSizeDeltaCoroutines[targetRectTransform] = StartCoroutine(SizeRectTransform_ScaledTime(wait, duration, startTime, endSize, startSize, targetRectTransform, curve));
        }
    }

    #endregion

    #region Transform 관련 변수

    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 endPos로 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endPos"> 목표로 하는 종료 위치 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void PositionTransform(bool unscaledTime, float wait, float duration, Vector3 endPos, Transform targetTransform, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(TransformPosCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(PositionTransform_UnscaledTime(wait, duration, Time.unscaledTime, endPos, targetTransform.localPosition, targetTransform, curve))
                : StartCoroutine(PositionTransform_ScaledTime(wait, duration, Time.time, endPos, targetTransform.localPosition, targetTransform, curve)));    
    }
    private IEnumerator PositionTransform_UnscaledTime(float wait, float duration, float startTime, Vector3 endPos, Vector3 startPos, Transform targetTransform, AnimationCurve curve)
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
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSecondsRealtime(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(PositionTransform_UnscaledTime(wait, duration, startTime, endPos, startPos, targetTransform, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, evaluation);

            yield return new WaitForSecondsRealtime(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(PositionTransform_UnscaledTime(wait, duration, startTime, endPos, startPos, targetTransform, curve));
        }
    }
    private IEnumerator PositionTransform_ScaledTime(float wait, float duration, float startTime, Vector3 endPos, Vector3 startPos, Transform targetTransform, AnimationCurve curve)
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
        else if(pastDeltaTime < wait)//대기
        {
            yield return new WaitForSeconds(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(PositionTransform_ScaledTime(wait, duration, startTime, endPos, startPos, targetTransform, curve));
            yield break;
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, evaluation);

            yield return new WaitForSeconds(0.02f);
            TransformPosCoroutines[targetTransform] = StartCoroutine(PositionTransform_ScaledTime(wait, duration, startTime, endPos, startPos, targetTransform, curve));
        }
    }
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 X축에 대하여 주기 운동한다
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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per), 0f, 0f);

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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per), 0f, 0f);

            yield return new WaitForSeconds(0.02f);
            PeriodicXTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicXTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 Y축에 대하여 주기 운동한다
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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per), 0f);

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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per), 0f);

            yield return new WaitForSeconds(0.02f);
            PeriodicYTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicYTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    /// <summary>
    /// Transform의 localPosition을 wait동안 대기한 후에 duration동안 duration / periods 주기로 사인 함수를 이용하여 Z축에 대하여 주기 운동한다
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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(0f, 0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per));

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
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            targetTransform.localPosition = startPos + new Vector3(0f, 0f, amplitude * Mathf.Sin(2 * Mathf.PI * periods / duration * per));

            yield return new WaitForSeconds(0.02f);
            PeriodicZTransformPosCoroutines[targetTransform] = StartCoroutine(MovePeriodicZTransformPos_ScaledTime(wait, duration, startTime, periods, amplitude, startPos, targetTransform));
        }
    }
    
    /// <summary>
    /// Transform의 localRotation을 wait동안 대기한 후에 duration동안 endRot로 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endRot"> 목표로 하는 종료 각도 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void RotateTransform(bool unscaledTime, float wait, float duration, Vector3 endRot, Transform targetTransform, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(TransformRotationCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(RotateTransform_UnscaledTime(wait, duration, Time.unscaledTime, endRot, targetTransform.localEulerAngles, targetTransform, curve))
                : StartCoroutine(RotateTransform_ScaledTime(wait, duration, Time.time, endRot, targetTransform.localEulerAngles, targetTransform, curve)));
    }
    private IEnumerator RotateTransform_UnscaledTime(float wait, float duration, float startTime, Vector3 endRot, Vector3 startRot, Transform targetTransform, AnimationCurve curve)
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
            TransformRotationCoroutines[targetTransform] = StartCoroutine(RotateTransform_UnscaledTime(wait, duration, startTime, endRot, startRot, targetTransform, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), evaluation);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(RotateTransform_UnscaledTime(wait, duration, startTime, endRot, startRot, targetTransform, curve));
        }
    }
    private IEnumerator RotateTransform_ScaledTime(float wait, float duration, float startTime, Vector3 endRot, Vector3 startRot, Transform targetTransform, AnimationCurve curve)
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
            TransformRotationCoroutines[targetTransform] = StartCoroutine(RotateTransform_ScaledTime(wait, duration, startTime, endRot, startRot, targetTransform, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), evaluation);
            
            yield return new WaitForSeconds(0.02f);
            TransformRotationCoroutines[targetTransform] = StartCoroutine(RotateTransform_ScaledTime(wait, duration, startTime, endRot, startRot, targetTransform, curve));
        }
    }

    /// <summary>
    /// Transform의 localScale을 wait동안 대기한 후에 duration동안 endScale로 전환
    /// </summary>
    /// <remarks>
    /// 애니메이션 커브를 인자로 받아서 비선형 전환이 가능
    /// </remarks>
    /// <param name="unscaledTime"> 스케일 여부 </param>
    /// <param name="wait"> 대기 시간 </param>
    /// <param name="duration"> 실행 시간 </param>
    /// <param name="endScale"> 목표로 하는 종료 크기 </param>
    /// <param name="targetTransform"> 변환을 가할 Transform </param>
    /// <param name="curve"> 적용할 애니메이션 커브 </param>
    public void ScaleTransform(bool unscaledTime, float wait, float duration, Vector3 endScale, Transform targetTransform, AnimationCurve curve = null)
    {
        CheckAndStartCoroutine(TransformScaleCoroutines, targetTransform,
            unscaledTime
                ? StartCoroutine(ScaleTransform_UnscaledTime(wait, duration, Time.unscaledTime, endScale, targetTransform.localScale, targetTransform, curve))
                : StartCoroutine(ScaleTransform_ScaledTime(wait, duration, Time.time, endScale, targetTransform.localScale, targetTransform, curve)));    
    }
    private IEnumerator ScaleTransform_UnscaledTime(float wait, float duration, float startTime, Vector3 endScale, Vector3 startScale, Transform targetTransform, AnimationCurve curve)
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
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ScaleTransform_UnscaledTime(wait, duration, startTime, endScale, startScale, targetTransform, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, evaluation);
            
            yield return new WaitForSecondsRealtime(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ScaleTransform_UnscaledTime(wait, duration, startTime, endScale, startScale, targetTransform, curve));
        }
    }
    private IEnumerator ScaleTransform_ScaledTime(float wait, float duration, float startTime, Vector3 endScale, Vector3 startScale, Transform targetTransform, AnimationCurve curve)
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
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ScaleTransform_ScaledTime(wait, duration, startTime, endScale, startScale, targetTransform, curve));
        }
        else if(pastDeltaTime >= wait)//액티브
        {
            var per = Mathf.Max(0.001f, pastDeltaTime - wait) / duration;
            var evaluation = curve?.Evaluate(per) ?? per;
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, evaluation);
            
            yield return new WaitForSeconds(0.02f);
            TransformScaleCoroutines[targetTransform] = StartCoroutine(ScaleTransform_ScaledTime(wait, duration, startTime, endScale, startScale, targetTransform, curve));
        }
    }
    
    #endregion
    
}