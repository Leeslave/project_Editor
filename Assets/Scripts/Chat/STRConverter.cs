using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

///<summary> STRConverter는 SpriteRenderer, TextMeshPro 및 TextMeshProUGUI, RectTransform 등의 값을 시간에 흐름에 따라서 제어하는 함수를 제공합니다. </summary>
///<remarks> 씬에 배치하고 FindObjectOfType를 통해서 각 함수를 호출합니다. 딕셔너리를 이용해서 각기 다른 요청을 구별합니다. </remarks>
public class STRConverter : MonoBehaviour
{
    public static STRConverter instance = null;

    ///<summary> SpriteRenderer 색 </summary>
    private Dictionary<SpriteRenderer, Coroutine> m_SpriteRendererColorCoroutines = new Dictionary<SpriteRenderer, Coroutine>();
    ///<summary> SpriteRenderer 크기 </summary>
    private Dictionary<SpriteRenderer, Coroutine> m_SpriteRendererSizeCoroutines = new Dictionary<SpriteRenderer, Coroutine>();
    ///<summary> SpriteRenderer 스프라이트 </summary>
    private Dictionary<SpriteRenderer, Coroutine> m_SpriteRendererSpriteCoroutines = new Dictionary<SpriteRenderer, Coroutine>();
    
    ///<summary> TextMeshPro 색 </summary>
    private Dictionary<TextMeshPro, Coroutine> m_TMPColorCoroutines = new Dictionary<TextMeshPro, Coroutine>();
    ///<summary> TextMeshPro 폰트 크기 </summary>
    private Dictionary<TextMeshPro, Coroutine> m_TMPFontSizeCoroutines = new Dictionary<TextMeshPro, Coroutine>();
    ///<summary> TextMeshPro 텍스트 출력 </summary>
    private Dictionary<TextMeshPro, Coroutine> m_TMPPrintCoroutines = new Dictionary<TextMeshPro, Coroutine>();

    ///<summary> TextMeshProUGUI 색 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> m_TMPUGUIColorCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    ///<summary> TextMeshProUGUI 폰트 크기 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> m_TMPUGUIFontSizeCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    ///<summary> TextMeshProUGUI 텍스트 출력 </summary>
    private Dictionary<TextMeshProUGUI, Coroutine> m_TMPUGUIPrintCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();

    ///<summary> RectTransform 위치 </summary>
    private Dictionary<RectTransform, Coroutine> m_RectTransformPosCoroutines = new Dictionary<RectTransform, Coroutine>();
    ///<summary> RectTransform 크기 </summary>
    private Dictionary<RectTransform, Coroutine> m_RectTransformSizeCoroutines = new Dictionary<RectTransform, Coroutine>();

    ///<summary> Transform 위치 </summary>
    private Dictionary<Transform, Coroutine> m_TransformPosCoroutines = new Dictionary<Transform, Coroutine>();
    ///<summary> Transform 회전 </summary>
    private Dictionary<Transform, Coroutine> m_TransformRotationCoroutines = new Dictionary<Transform, Coroutine>();
    ///<summary> Transform 크기 </summary>
    private Dictionary<Transform, Coroutine> m_TransformScaleCoroutines = new Dictionary<Transform, Coroutine>();


    ///<summary> 현재 시간
    private float currentTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        currentTime = 0;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }



    ///<summary> SpriteRenderer의 color를 duration동안 endColor로 전환 </summary>
    public void ConvertSpriteRendererColor(float duration, Color endColor, SpriteRenderer targetSpriteRenderer)
    {

        if(m_SpriteRendererColorCoroutines.ContainsKey(targetSpriteRenderer))
        {
            if(m_SpriteRendererColorCoroutines[targetSpriteRenderer] != null)
                StopCoroutine(m_SpriteRendererColorCoroutines[targetSpriteRenderer]);
            m_SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_IE(duration, currentTime, endColor, targetSpriteRenderer.color, targetSpriteRenderer));
        }
        else
        {
            m_SpriteRendererColorCoroutines.Add(targetSpriteRenderer, StartCoroutine(ConvertSpriteRendererColor_IE(duration, currentTime, endColor, targetSpriteRenderer.color, targetSpriteRenderer)));
        }
    }

    private IEnumerator ConvertSpriteRendererColor_IE(float duration, float startTime, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer)
    {
        if(duration == 0f)
        {
            targetSpriteRenderer.color = endColor;
            m_SpriteRendererColorCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }

        //작업 개시로부터 지난 시간
        float pastDeltaTime = currentTime - startTime;
        
        //타임 아웃
        if (pastDeltaTime > duration)
        {
            targetSpriteRenderer.color = endColor;
            m_SpriteRendererColorCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }   

        targetSpriteRenderer.color = Color.Lerp(startColor, endColor, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_SpriteRendererColorCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererColor_IE(duration, startTime, endColor, startColor, targetSpriteRenderer));
    }

    ///<summary> SpriteRenderer의 size를 duration동안 endSize로 전환 </summary>
    public void ConvertSpriteRendererSize(float duration, Vector2 endSize, SpriteRenderer targetSpriteRenderer)
    {
        if(m_SpriteRendererSizeCoroutines.ContainsKey(targetSpriteRenderer))
        {
            if(m_SpriteRendererSizeCoroutines[targetSpriteRenderer] != null)
                StopCoroutine(m_SpriteRendererSizeCoroutines[targetSpriteRenderer]);
            m_SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_IE(duration, currentTime, endSize, targetSpriteRenderer.size, targetSpriteRenderer));
        }
        else
        {
            m_SpriteRendererSizeCoroutines.Add(targetSpriteRenderer, StartCoroutine(ConvertSpriteRendererSize_IE(duration, currentTime, endSize, targetSpriteRenderer.size, targetSpriteRenderer)));
        }
    }

    private IEnumerator ConvertSpriteRendererSize_IE(float duration, float startTime, Vector2 endSize, Vector2 startSize, SpriteRenderer targetSpriteRenderer)
    {
        if(duration == 0f)
        {
            targetSpriteRenderer.size = endSize;
            m_SpriteRendererSizeCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;
        
        if(pastDeltaTime > duration)
        {
            targetSpriteRenderer.size = endSize;
            m_SpriteRendererSizeCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }
    
        targetSpriteRenderer.size = Vector2.Lerp(startSize, endSize, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_SpriteRendererSizeCoroutines[targetSpriteRenderer] = StartCoroutine(ConvertSpriteRendererSize_IE(duration, startTime, endSize, startSize, targetSpriteRenderer));
    }

    ///<summary> SpriteRenderer의 sprite를 duration동안 페이드아웃하고 newSprite로 전환 </summary>
    public void SwitchSpriteRendererSprite(float duration, Sprite newSprite, Color endColor, SpriteRenderer targetSpriteRenderer)
    {
        if(m_SpriteRendererSpriteCoroutines.ContainsKey(targetSpriteRenderer))
        {
            if(m_SpriteRendererSpriteCoroutines[targetSpriteRenderer] != null)
                StopCoroutine(m_SpriteRendererSpriteCoroutines[targetSpriteRenderer]);
            m_SpriteRendererSpriteCoroutines[targetSpriteRenderer] = StartCoroutine(SwitchSpriteRendererSprite_IE(duration, currentTime, newSprite, endColor, targetSpriteRenderer.color, targetSpriteRenderer));
        }
        else
        {
            m_SpriteRendererSpriteCoroutines.Add(targetSpriteRenderer, StartCoroutine(SwitchSpriteRendererSprite_IE(duration, currentTime, newSprite, endColor, targetSpriteRenderer.color, targetSpriteRenderer)));
        }
    }

    private IEnumerator SwitchSpriteRendererSprite_IE(float duration, float startTime, Sprite newSprite, Color endColor, Color startColor, SpriteRenderer targetSpriteRenderer)
    {
        if(duration == 0f)
        {
            targetSpriteRenderer.color = Color.white;
            targetSpriteRenderer.sprite = newSprite;
            m_SpriteRendererSpriteCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }

        //작업 개시로부터 지난 시간
        float pastDeltaTime = currentTime - startTime;
        
        //타임 아웃
        if (pastDeltaTime > duration)
        {
            targetSpriteRenderer.color = Color.white;
            targetSpriteRenderer.sprite = newSprite;
            m_SpriteRendererSpriteCoroutines.Remove(targetSpriteRenderer);
            yield break;
        }

        targetSpriteRenderer.color = Color.Lerp(startColor, Color.clear, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_SpriteRendererSpriteCoroutines[targetSpriteRenderer] = StartCoroutine(SwitchSpriteRendererSprite_IE(duration, startTime, newSprite, endColor, startColor, targetSpriteRenderer));
    }



    ///<summary> TextMeshPro의 color를 duration동안 endColor로 전환 </summary>
    public void ConvertTMPColor(float duration, Color endColor, TextMeshPro targetTMP)
    {
        if(m_TMPColorCoroutines.ContainsKey(targetTMP))
        {
            if(m_TMPColorCoroutines[targetTMP] != null)
                StopCoroutine(m_TMPColorCoroutines[targetTMP]);
            m_TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_IE(duration, currentTime, endColor, targetTMP.color, targetTMP));
        }
        else
        {
            m_TMPColorCoroutines.Add(targetTMP, StartCoroutine(ConvertTMPColor_IE(duration, currentTime, endColor, targetTMP.color, targetTMP)));
        }
    }

    private IEnumerator ConvertTMPColor_IE(float duration, float startTime, Color endColor, Color startColor, TextMeshPro targetTMP)
    {
        if(duration == 0f)
        {
            targetTMP.color = endColor;
            m_TMPColorCoroutines.Remove(targetTMP);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if (pastDeltaTime > duration)
        {
            targetTMP.color = endColor;
            m_TMPColorCoroutines.Remove(targetTMP);
            yield break;
        }

        targetTMP.color = Color.Lerp(startColor, endColor, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TMPColorCoroutines[targetTMP] = StartCoroutine(ConvertTMPColor_IE(duration, startTime, endColor, startColor, targetTMP));
    }

    ///<summary> TextMeshPro의 fontSize를 duration동안 endSize로 전환 </summary>
    public void ConvertTMPFontSize(float duration, float endSize, TextMeshPro targetTMP)
    {
        if(m_TMPFontSizeCoroutines.ContainsKey(targetTMP))
        {
            if(m_TMPFontSizeCoroutines[targetTMP] != null)
                StopCoroutine(m_TMPFontSizeCoroutines[targetTMP]);
            m_TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_IE(duration, currentTime, endSize, targetTMP.fontSize, targetTMP));
        }
        else
        {
            m_TMPFontSizeCoroutines.Add(targetTMP, StartCoroutine(ConvertTMPFontSize_IE(duration, currentTime, endSize, targetTMP.fontSize, targetTMP)));
        }
    }

    private IEnumerator ConvertTMPFontSize_IE(float duration, float startTime, float endSize, float startSize, TextMeshPro targetTMP)
    {
        if(duration == 0f)
        {
            targetTMP.fontSize = endSize;
            m_TMPFontSizeCoroutines.Remove(targetTMP);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if(pastDeltaTime > duration)
        {
            targetTMP.fontSize = endSize;
            m_TMPFontSizeCoroutines.Remove(targetTMP);
            yield break;
        }

        targetTMP.fontSize = Mathf.Lerp(startSize, endSize, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TMPFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPFontSize_IE(duration, startTime, endSize, startSize, targetTMP));
    }

    ///<summary> duration에 따라서 targetTMP에 value를 출력합니다 </summary>
    public void PrintTMPByDuration(float duration, string value, TextMeshPro targetTMP)
    {
        if(m_TMPPrintCoroutines.ContainsKey(targetTMP))
        {
            if(m_TMPPrintCoroutines[targetTMP] != null)
                StopCoroutine(m_TMPPrintCoroutines[targetTMP]);
            targetTMP.text = "";
            m_TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_IE(duration, 0, value, targetTMP));
        }
        else
        {
            targetTMP.text = "";
            m_TMPPrintCoroutines.Add(targetTMP, StartCoroutine(PrintTMPByDuration_IE(duration, 0, value, targetTMP)));
        }
    }

    private IEnumerator PrintTMPByDuration_IE(float duration, int idx, string value, TextMeshPro targetTMP)
    {
        if(duration == 0f)
        {
            targetTMP.text = value;
            m_TMPPrintCoroutines.Remove(targetTMP);
            yield break;
        }

        if(idx >= value.Length)
        {
            m_TMPPrintCoroutines.Remove(targetTMP);
            yield break;
        }

        targetTMP.text += value[idx];

        yield return new WaitForSeconds(duration/value.Length);
        m_TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDuration_IE(duration, idx + 1, value, targetTMP));
    }

    ///<summary> delay에 따라서 targetTMP에 value를 출력합니다 </summary>
    public void PrintTMPByDelay(float delay, string value, TextMeshPro targetTMP)
    {
        if(m_TMPPrintCoroutines.ContainsKey(targetTMP))
        {
            if(m_TMPPrintCoroutines[targetTMP] != null)
                StopCoroutine(m_TMPPrintCoroutines[targetTMP]);
            targetTMP.text = "";
            m_TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_IE(delay, 0, value, targetTMP));
        }
        else
        {
            targetTMP.text = "";
            m_TMPPrintCoroutines.Add(targetTMP, StartCoroutine(PrintTMPByDelay_IE(delay, 0, value, targetTMP)));
        }
    }

    private IEnumerator PrintTMPByDelay_IE(float delay, int idx, string value, TextMeshPro targetTMP)
    {
        if(delay == 0f)
        {
            targetTMP.text = value;
            m_TMPPrintCoroutines.Remove(targetTMP);
            yield break;
        }

        if(idx >= value.Length)
        {
            m_TMPPrintCoroutines.Remove(targetTMP);
            yield break;
        }

        targetTMP.text += value[idx];

        yield return new WaitForSeconds(delay);
        m_TMPPrintCoroutines[targetTMP] = StartCoroutine(PrintTMPByDelay_IE(delay, idx + 1, value, targetTMP));
    }

    ///<summary> targetTMP의 출력을 강제 종료합니다 </summary>
    public void StopPrintingTMP(TextMeshPro targetTMP)
    {
        if(!m_TMPPrintCoroutines.ContainsKey(targetTMP))
            return;
        StopCoroutine(m_TMPPrintCoroutines[targetTMP]);
        m_TMPPrintCoroutines.Remove(targetTMP);
        return;
    }

    ///<summary> targetTMP의 출력 여부를 반환합니다 </summary>
    public bool GetIsPrintingTMP(TextMeshPro targetTMP)
    {
        if(targetTMP == null)
            return false;
        else
            return m_TMPPrintCoroutines.ContainsKey(targetTMP);
    }



    ///<summary> TextMeshProUGUI의 color를 duration동안 endColor로 전환 </summary>
    public void ConvertTMPUGUIColor(float duration, Color endColor, TextMeshProUGUI targetTMPUGUI)
    {
        if(m_TMPUGUIColorCoroutines.ContainsKey(targetTMPUGUI))
        {
            if(m_TMPUGUIColorCoroutines[targetTMPUGUI] != null)
                StopCoroutine(m_TMPUGUIColorCoroutines[targetTMPUGUI]);
            m_TMPUGUIColorCoroutines[targetTMPUGUI] = StartCoroutine(ConvertTMPUGUIColor_IE(duration, currentTime, endColor, targetTMPUGUI.color, targetTMPUGUI));
        }
        else
        {
            m_TMPUGUIColorCoroutines.Add(targetTMPUGUI, StartCoroutine(ConvertTMPUGUIColor_IE(duration, currentTime, endColor, targetTMPUGUI.color, targetTMPUGUI)));
        }
    }

    private IEnumerator ConvertTMPUGUIColor_IE(float duration, float startTime, Color endColor, Color startColor, TextMeshProUGUI targetTMPUGUI)
    {
        if(duration == 0f)
        {
            targetTMPUGUI.color = endColor;
            m_TMPUGUIColorCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if (pastDeltaTime > duration)
        {
            targetTMPUGUI.color = endColor;
            m_TMPUGUIColorCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        targetTMPUGUI.color = Color.Lerp(startColor, endColor, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TMPUGUIColorCoroutines[targetTMPUGUI] = StartCoroutine(ConvertTMPUGUIColor_IE(duration, startTime, endColor, startColor, targetTMPUGUI));
    }

    ///<summary> TextMeshProUGUI의 fontSize를 duration동안 endSize로 전환 </summary>
    public void ConvertTMPUGUIFontSize(float duration, float endSize, TextMeshProUGUI targetTMPUGUI)
    {
        if(m_TMPUGUIFontSizeCoroutines.ContainsKey(targetTMPUGUI))
        {
            if(m_TMPUGUIFontSizeCoroutines[targetTMPUGUI] != null)
                StopCoroutine(m_TMPUGUIFontSizeCoroutines[targetTMPUGUI]);
            m_TMPUGUIFontSizeCoroutines[targetTMPUGUI] = StartCoroutine(ConvertTMPUGUIFontSize_IE(duration, currentTime, endSize, targetTMPUGUI.fontSize, targetTMPUGUI));
        }
        else
        {
            m_TMPUGUIFontSizeCoroutines.Add(targetTMPUGUI, StartCoroutine(ConvertTMPUGUIFontSize_IE(duration, currentTime, endSize, targetTMPUGUI.fontSize, targetTMPUGUI)));
        }
    }
    
    private IEnumerator ConvertTMPUGUIFontSize_IE(float duration, float startTime, float endSize, float startSize, TextMeshProUGUI targetTMP)
    {
        if(duration == 0f)
        {
            targetTMP.fontSize = endSize;
            m_TMPUGUIFontSizeCoroutines.Remove(targetTMP);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if(pastDeltaTime > duration)
        {
            targetTMP.fontSize = endSize;
            m_TMPUGUIFontSizeCoroutines.Remove(targetTMP);
            yield break;
        }

        targetTMP.fontSize = Mathf.Lerp(startSize, endSize, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TMPUGUIFontSizeCoroutines[targetTMP] = StartCoroutine(ConvertTMPUGUIFontSize_IE(duration, startTime, endSize, startSize, targetTMP));
    }
    
    ///<summary> duration에 따라서 targetTMPUGUI에 value를 출력합니다 </summary>
    public void PrintTMPUGUIByDuration(float duration, string value, TextMeshProUGUI targetTMPUGUI)
    {
         if(m_TMPUGUIPrintCoroutines.ContainsKey(targetTMPUGUI))
        {
            if(m_TMPUGUIPrintCoroutines[targetTMPUGUI] != null)
                StopCoroutine(m_TMPUGUIPrintCoroutines[targetTMPUGUI]);
            targetTMPUGUI.text = "";
            m_TMPUGUIPrintCoroutines[targetTMPUGUI] = StartCoroutine(PrintTMPUGUIByDuration_IE(duration, 0, value, targetTMPUGUI));
        }
        else
        {
            targetTMPUGUI.text = "";
            m_TMPUGUIPrintCoroutines.Add(targetTMPUGUI, StartCoroutine(PrintTMPUGUIByDuration_IE(duration, 0, value, targetTMPUGUI)));
        }
    }

    private IEnumerator PrintTMPUGUIByDuration_IE(float duration, int idx, string value, TextMeshProUGUI targetTMPUGUI)
    {
        if(duration == 0f)
        {
            targetTMPUGUI.text = value;
            m_TMPUGUIPrintCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        if(idx >= value.Length)
        {
            m_TMPUGUIPrintCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        targetTMPUGUI.text += value[idx];

        yield return new WaitForSeconds(duration/value.Length);
        m_TMPUGUIPrintCoroutines[targetTMPUGUI] = StartCoroutine(PrintTMPUGUIByDuration_IE(duration, idx + 1, value, targetTMPUGUI));
    }

    ///<summary> delay에 따라서 targetTMPUGUI에 value를 출력합니다 </summary>
    public void PrintTMPUGUIByDelay(float delay, string value, TextMeshProUGUI targetTMPUGUI)
    {
        if(m_TMPUGUIPrintCoroutines.ContainsKey(targetTMPUGUI))
        {
            if(m_TMPUGUIPrintCoroutines[targetTMPUGUI] != null)
                StopCoroutine(m_TMPUGUIPrintCoroutines[targetTMPUGUI]);
            targetTMPUGUI.text = "";
            m_TMPUGUIPrintCoroutines[targetTMPUGUI] = StartCoroutine(PrintTMPUGUIByDelay_IE(delay, 0, value, targetTMPUGUI));
        }
        else
        {
            targetTMPUGUI.text = "";
            m_TMPUGUIPrintCoroutines.Add(targetTMPUGUI, StartCoroutine(PrintTMPUGUIByDelay_IE(delay, 0, value, targetTMPUGUI)));
        }
    }

    private IEnumerator PrintTMPUGUIByDelay_IE(float delay, int idx, string value, TextMeshProUGUI targetTMPUGUI)
    {
        if(delay == 0f)
        {
            targetTMPUGUI.text = value;
            m_TMPUGUIPrintCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        if(idx >= value.Length)
        {
            m_TMPUGUIPrintCoroutines.Remove(targetTMPUGUI);
            yield break;
        }

        targetTMPUGUI.text += value[idx];

        yield return new WaitForSeconds(delay);
        m_TMPUGUIPrintCoroutines[targetTMPUGUI] = StartCoroutine(PrintTMPUGUIByDelay_IE(delay, idx + 1, value, targetTMPUGUI));
    }

    ///<summary> targetTMPUGUI의 출력을 강제 종료합니다 </summary>
    public void StopPrintingTMPUGUI(TextMeshProUGUI targetTMPUGUI)
    {
        if(!m_TMPUGUIPrintCoroutines.ContainsKey(targetTMPUGUI))
            return;
        StopCoroutine(m_TMPUGUIPrintCoroutines[targetTMPUGUI]);
        m_TMPUGUIPrintCoroutines.Remove(targetTMPUGUI);
        return;
    }

    ///<summary> targetTMPUGUI의 출력 여부를 반환합니다 </summary>
    public bool GetIsPrintingTMPUGUI(TextMeshProUGUI targetTMPUGUI)
    {
        if(m_TMPUGUIPrintCoroutines.ContainsKey(targetTMPUGUI))
        {
            if(m_TMPUGUIPrintCoroutines[targetTMPUGUI] != null)
                return true;
            else
                return false;
        }
        else
            return false;
    }



    ///<summary> RectTransform의 localPosition을 duration동안 endPos로 전환 </summary>
    public void ConvertRectTransformPos(float duration, Vector3 endPos, RectTransform targetRectTransform)
    {
        if(m_RectTransformPosCoroutines.ContainsKey(targetRectTransform))
        {
            if(m_RectTransformPosCoroutines[targetRectTransform] != null)
                StopCoroutine(m_RectTransformPosCoroutines[targetRectTransform]);
            m_RectTransformPosCoroutines[targetRectTransform] = StartCoroutine(CovertRectTransformPos_IE(duration, currentTime, endPos, targetRectTransform.localPosition, targetRectTransform));
        }
        else
        {
            m_RectTransformPosCoroutines.Add(targetRectTransform, StartCoroutine(CovertRectTransformPos_IE(duration, currentTime, endPos, targetRectTransform.localPosition, targetRectTransform)));
        }
    }

    private IEnumerator CovertRectTransformPos_IE(float duration, float startTime, Vector3 endPos, Vector3 startPos, RectTransform targetRectTransform)
    {
        if(duration == 0f)
        {
            targetRectTransform.localPosition = endPos;
            m_RectTransformPosCoroutines.Remove(targetRectTransform);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if(pastDeltaTime > duration)
        {
            targetRectTransform.localPosition = endPos;
            m_RectTransformPosCoroutines.Remove(targetRectTransform);
            yield break;
        }

        targetRectTransform.localPosition = Vector3.Lerp(startPos, endPos, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_RectTransformPosCoroutines[targetRectTransform] = StartCoroutine(CovertRectTransformPos_IE(duration, startTime, endPos, startPos, targetRectTransform));
    }



    ///<summary> Transform의 localPosition을 duration동안 endPos로 전환 </summary>
    public void ConvertTransformPos(float duration, Vector3 endPos, Transform targetTransform)
    {
        if(m_TransformPosCoroutines.ContainsKey(targetTransform))
        {
            if(m_TransformPosCoroutines[targetTransform] != null)
                StopCoroutine(m_TransformPosCoroutines[targetTransform]);
            m_TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_IE(duration, currentTime, endPos, targetTransform.localPosition, targetTransform));
        }
        else
        {
            m_TransformPosCoroutines.Add(targetTransform, StartCoroutine(ConvertTransformPos_IE(duration, currentTime, endPos, targetTransform.localPosition, targetTransform)));
        }
    }

    private IEnumerator ConvertTransformPos_IE(float duration, float startTime, Vector3 endPos, Vector3 startPos, Transform targetTransform)
    {
        if(duration == 0f)
        {
            targetTransform.localPosition = endPos;
            m_TransformPosCoroutines.Remove(targetTransform);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;

        if(pastDeltaTime > duration)
        {
            targetTransform.localPosition = endPos;
            m_TransformPosCoroutines.Remove(targetTransform);
            yield break;
        }

        targetTransform.localPosition = Vector3.Lerp(startPos, endPos, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TransformPosCoroutines[targetTransform] = StartCoroutine(ConvertTransformPos_IE(duration, startTime, endPos, startPos, targetTransform));
    }

    ///<summary> Transform의 localRotation을 duration동안 endRotation로 전환 </summary>
    public void ConvertTransformRotation(float duration, Vector3 endRotation, Transform targetTransform)
    {
        if(m_TransformRotationCoroutines.ContainsKey(targetTransform))
        {
            if(m_TransformRotationCoroutines[targetTransform] != null)
                StopCoroutine(m_TransformRotationCoroutines[targetTransform]);
            m_TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRotation_IE(duration, currentTime, endRotation, targetTransform.localRotation.eulerAngles, targetTransform));
        }
        else
        {
            m_TransformRotationCoroutines.Add(targetTransform, StartCoroutine(ConvertTransformRotation_IE(duration, currentTime, endRotation, targetTransform.localRotation.eulerAngles, targetTransform)));
        }
    }

    private IEnumerator ConvertTransformRotation_IE(float duration, float startTime, Vector3 endRotation, Vector3 startRotation, Transform targetTransform)
    {
        if(duration == 0f)
        {
            targetTransform.localRotation = Quaternion.Euler(endRotation);
            m_TransformRotationCoroutines.Remove(targetTransform);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;
        
        if(pastDeltaTime > duration)
        {
            targetTransform.localRotation = Quaternion.Euler(endRotation);
            m_TransformRotationCoroutines.Remove(targetTransform);
            yield break;
        }

        targetTransform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, pastDeltaTime / duration));

        yield return new WaitForSeconds(0.01f);
        m_TransformRotationCoroutines[targetTransform] = StartCoroutine(ConvertTransformRotation_IE(duration, startTime, endRotation, startRotation, targetTransform));
    }

    ///<summary> Transform의 localScale을 duration동안 endScale로 전환 </summary>
    public void ConvertTransformScale(float duration, Vector3 endScale, Transform targetTransform)
    {
        if(m_TransformScaleCoroutines.ContainsKey(targetTransform))
        {
            if(m_TransformRotationCoroutines[targetTransform] != null)
                StopCoroutine(m_TransformScaleCoroutines[targetTransform]);
            m_TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_IE(duration, currentTime, endScale, targetTransform.localScale, targetTransform));
        }
        else
        {
            m_TransformScaleCoroutines.Add(targetTransform, StartCoroutine(ConvertTransformScale_IE(duration, currentTime, endScale, targetTransform.localScale, targetTransform)));
        }
    }

    private IEnumerator ConvertTransformScale_IE(float duration, float startTime, Vector3 endScale, Vector3 startScale, Transform targetTransform)
    {
        if(duration == 0f)
        {
            targetTransform.localScale = endScale;
            m_TransformScaleCoroutines.Remove(targetTransform);
            yield break;
        }

        float pastDeltaTime = currentTime - startTime;
        
        if(pastDeltaTime > duration)
        {
            targetTransform.localScale = endScale;
            m_TransformScaleCoroutines.Remove(targetTransform);
            yield break;
        }

        targetTransform.localScale = Vector3.Lerp(startScale, endScale, pastDeltaTime / duration);

        yield return new WaitForSeconds(0.01f);
        m_TransformScaleCoroutines[targetTransform] = StartCoroutine(ConvertTransformScale_IE(duration, startTime, endScale, startScale, targetTransform));
    }

    public void FillPercentage(float endTime, TextMeshPro targetTMP)
    {
        StartCoroutine(FillPercentage_IE(endTime, 0, targetTMP));
    }

    private IEnumerator FillPercentage_IE(float endTime, float currentendTime, TextMeshPro targetTMP)
    {
        currentendTime += endTime / 100;
        if (currentendTime > endTime)
            yield break;

        targetTMP.text = Mathf.CeilToInt(currentendTime / endTime * 100).ToString() + "%";

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(FillPercentage_IE(endTime, currentendTime, targetTMP));
    }
}
