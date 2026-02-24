using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TutorialDimHoleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image overlayImage;

    [Header("Defaults (px)")]
    [SerializeField] private float outlineThicknessPx = 2f;
    [SerializeField] private float featherPx = 2f;

    [Header("Pop Animation")]
    [SerializeField] private float popDuration = 1f;
    [SerializeField] private float popOvershootRatio = 0.25f; // 강조 효과를 위해서 목표로하는 도형의 크기보다 몇 % 크게 할 것인지 (Default : 25%)

    private Material runtimeMat;
    private Coroutine popCo;        // 강조 애니메이션을 보여줄 코루틴 


    private static readonly int ShapeTypeId = Shader.PropertyToID("_ShapeType");
    private static readonly int HoleCenterId = Shader.PropertyToID("_HoleCenter");
    private static readonly int HoleSizeId = Shader.PropertyToID("_HoleSize");
    private static readonly int CornerRadiusId = Shader.PropertyToID("_CornerRadius");
    private static readonly int OutlineThicknessId = Shader.PropertyToID("_OutlineThickness");
    private static readonly int FeatherId = Shader.PropertyToID("_Feather");


    private void Reset()
    {
        overlayImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (null == overlayImage) { overlayImage = GetComponent<Image>(); }

        runtimeMat = Instantiate(overlayImage.material);
        overlayImage.material = runtimeMat;
    }

    private void OnDestroy()
    {
        if (null != runtimeMat)
        {
            Destroy(runtimeMat);
            runtimeMat = null;
        }
    }


    // Public API

    /// <summary>
    /// 화면 픽셀 좌표로 중심/사이즈를 받아서 UV로 변환해 적용.
    /// </summary>
    public void ShowRectPx(Vector2 centerPx, Vector2 sizePx, float cornerRadiusPx = 0f, bool playPop = true)
    {
        ShowRectUv(centerPx, sizePx, cornerRadiusPx, playPop);
    }

    public void ShowCirclePx(Vector2 centerPx, float radiusPx, bool playPop = true)
    {
        ShowCircleUv(centerPx, radiusPx, playPop);
    }

    public void ShowRectUv(Vector2 centerUv, Vector2 sizeUv, float cornerRadiusUv = 0f, bool playPop = true)
    {
        Debug.Log(centerUv);
        ApplyCommon(HighlightShape.Spuare, centerUv);

        runtimeMat.SetFloat(CornerRadiusId, Mathf.Max(0f, cornerRadiusUv));

        if (playPop)
        {
            StartPop(sizeUv);
        }
        else
        {
            runtimeMat.SetVector(HoleSizeId, new Vector4(sizeUv.x, sizeUv.y, 0, 0));
        }

        overlayImage.enabled = true;
        gameObject.SetActive(true);
    }

    public void ShowCircleUv(Vector2 centerUv, float radiusUv, bool playPop = true)
    {
        ApplyCommon(HighlightShape.Circle, centerUv);

        if (playPop)
        {
            // circle은 _HoleSize.x만 사용(= radius). y는 무시
            StartPop(new Vector2(radiusUv, 0f), isCircle: true);
        }
        else
        {
            runtimeMat.SetVector(HoleSizeId, new Vector4(radiusUv, 0f, 0, 0));
        }

        overlayImage.enabled = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        StopPop();
        overlayImage.enabled = false;
    }

    /// <summary>
    /// 딤 오버레이의 Raycast 차단 여부를 설정합니다.
    /// false로 설정하면 IsPointerOverGameObject()가 false를 반환해 물리 기반 클릭이 통과됩니다.
    /// </summary>
    public void SetRaycastEnabled(bool enabled)
    {
        overlayImage.raycastTarget = enabled;
    }

    // -------------------------
    // Internals
    // -------------------------

    private void ApplyCommon(HighlightShape type, Vector2 centerUv)
    {
        runtimeMat.SetFloat(ShapeTypeId, (type == HighlightShape.Circle) ? 0f : 1f);
        Debug.Log(centerUv);
        runtimeMat.SetVector(HoleCenterId, new Vector4(centerUv.x, centerUv.y, 0, 0));

        // px -> uv 변환해서 넣기 (두께/페더 픽셀 고정)
        runtimeMat.SetFloat(OutlineThicknessId, PxToUvY(outlineThicknessPx));
        runtimeMat.SetFloat(FeatherId, PxToUvY(featherPx));
    }

    private void StartPop(Vector2 targetSizeUv, bool isCircle = false)
    {
        StopPop();
        popCo = StartCoroutine(CoPop(targetSizeUv, isCircle));
    }

    private void StopPop()
    {
        if (popCo != null)
        {
            StopCoroutine(popCo);
            popCo = null;
        }
    }

    private IEnumerator CoPop(Vector2 targetUv, bool isCircle)
    {
        float dur = Mathf.Max(0.01f, popDuration);
        float overshoot = Mathf.Max(0f, popOvershootRatio);

        if (isCircle)
        {
            float targetR = Mathf.Max(0f, targetUv.x);
            float startR = targetR * (1f + overshoot);

            float t = 0f;
            runtimeMat.SetVector(HoleSizeId, new Vector4(startR, 0f, 0, 0));

            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / dur);
                float eased = 1f - Mathf.Pow(1f - a, 3f); // easeOutCubic

                float curR = Mathf.LerpUnclamped(startR, targetR, eased);
                runtimeMat.SetVector(HoleSizeId, new Vector4(curR, 0f, 0, 0));

                yield return null;
            }

            runtimeMat.SetVector(HoleSizeId, new Vector4(targetR, 0f, 0, 0));
        }
        else
        {
            Vector2 safeTarget = new Vector2(Mathf.Max(0f, targetUv.x), Mathf.Max(0f, targetUv.y));
            Vector2 startUv = safeTarget * (1f + overshoot);

            float t = 0f;
            runtimeMat.SetVector(HoleSizeId, new Vector4(startUv.x, startUv.y, 0, 0));

            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / dur);
                float eased = 1f - Mathf.Pow(1f - a, 3f); // easeOutCubic

                Vector2 cur = Vector2.LerpUnclamped(startUv, safeTarget, eased);
                runtimeMat.SetVector(HoleSizeId, new Vector4(cur.x, cur.y, 0, 0));

                yield return null;
            }

            runtimeMat.SetVector(HoleSizeId, new Vector4(safeTarget.x, safeTarget.y, 0, 0));
        }

        popCo = null;
    }

    // -------------------------
    // UV Utils
    // -------------------------

    private static Vector2 ScreenPxToUv(Vector2 screenPx)
    {
        float w = Mathf.Max(1f, Screen.width);
        float h = Mathf.Max(1f, Screen.height);
        return new Vector2(screenPx.x / w, screenPx.y / h);
    }

    private static float PxToUvY(float px)
    {
        float h = Mathf.Max(1f, Screen.height);
        return px;
    }

    public void SetCenterPx(Vector2 centerPx)
    {
        Vector2 centerUv = ScreenPxToUv(centerPx);
        SetCenterUv(centerUv);
    }

    public void SetCenterUv(Vector2 centerUv)
    {
        if (runtimeMat == null) return;
        runtimeMat.SetVector(HoleCenterId, new Vector4(centerUv.x, centerUv.y, 0, 0));
    }
}