// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// public class TutorialBorder : MonoBehaviour
// {
//     [SerializeField] bool IsRect;
//     [SerializeField] Material Mat;
//     [SerializeField] RectTransform TextBox;
//     [SerializeField] RectTransform CanvasRect;
//     [SerializeField] GameObject Target;
//     [SerializeField] private Camera uiCam; // Canvas가 Overlay면 null, Camera면 해당 카메라
//     TMP_Text TextDetail;
//     RectTransform SelfRect;
//     Image image;
//     EventTrigger trigger;

//     Vector2 Pos;
//     Vector2 ScreenSize;
//     Vector2 ObjSize;

//     bool IsScreen = false;

//     float MaxSize;

//     float CamSize = 1200;
//     float ReverseCamSize;

//     private void Awake()
//     {
//         SelfRect = GetComponent<RectTransform>();
//         image = GetComponent<Image>();
//         Mat = image.material;
//         trigger = GetComponent<EventTrigger>();
//         TextDetail = TextBox.GetComponentInChildren<TMP_Text>();
//         gameObject.SetActive(false);
//         ReverseCamSize = 1 / CamSize;
//         uiCam = Camera.main;
//     }

//     public float Init(GameObject Target, string text, bool IsHighlight, bool KeepEvent)
//     {
//         if (!KeepEvent)
//         {
//             try
//             {
//                 trigger.triggers.Clear();
//             }
//             catch (System.Exception e)
//             {
//                 print($"{e} At Clear");
//             }
//         }

//         TutorialSetting.instance.CurActive = this;
//         this.Target = Target;

//         if (Target == null)
//         {
//             ObjSize = Vector2.zero;
//             StartCoroutine(Test(text, IsHighlight));
//             return 0;
//         }

//         ObjSize = Vector2.one;

//         Transform ObjTrans = Target.transform;

//         IsScreen = false;
//         while (ObjTrans != null)
//         {
//             ObjSize *= ObjTrans.localScale;
//             if (ObjTrans.TryGetComponent(out Canvas cv)) IsScreen = cv.renderMode == RenderMode.ScreenSpaceOverlay;
//             ObjTrans = ObjTrans.parent;
//         }


//         if (!IsScreen) ObjSize *= (500 / Camera.main.orthographicSize);


//         if (Target.TryGetComponent(out RectTransform Rect))
//         {
//             LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

//             // RectTransform의 실제 "스크린 픽셀" 크기 구하기
//             Vector3[] corners = new Vector3[4];
//             Rect.GetWorldCorners(corners);

//             Vector2 bl = RectTransformUtility.WorldToScreenPoint(uiCam, corners[0]);
//             Vector2 tr = RectTransformUtility.WorldToScreenPoint(uiCam, corners[2]);

//             ObjSize = new Vector2(Mathf.Abs(tr.x - bl.x), Mathf.Abs(tr.y - bl.y));
//             IsScreen = true; // 이미 화면 기준 픽셀로 잡았으니 Screen로 취급
//         }
//         else
//         {
//             if (Target.TryGetComponent(out SpriteRenderer Sprite)) ObjSize *= Sprite.sprite.bounds.size;
//         }

//         if (!IsRect) ObjSize *= 1.2f;

//         MaxSize = Mathf.Max(ObjSize.x * ReverseCamSize, ObjSize.y * ReverseCamSize);

//         CalcPosition();

//         if (IsRect)
//         {
//             float nx = Pos.x / Screen.width;
//             float ny = Pos.y / Screen.height;
//             Mat.SetVector("_MaskCenter", new Vector4(nx, ny, 0, 0));
//             //Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
//             Mat.SetFloat("_X", ObjSize.x * 0.9f / CamSize);
//             Mat.SetFloat("_Y", ObjSize.y * 0.9f / CamSize);
//         }
//         else
//         {
//             float nx = Pos.x / Screen.width;
//             float ny = Pos.y / Screen.height;
//             Mat.SetVector("_MaskCenter", new Vector4(nx, ny, 0, 0));
//             //Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
//             Mat.SetFloat("_Radius", MaxSize);
//         }

//         gameObject.SetActive(true);

//         StartCoroutine(Test(text, IsHighlight));

//         return Mathf.Min(ObjSize.x, ObjSize.y);
//     }

//     private void Update()
//     {
//         CalcPosition();

//         if (IsRect)
//         {
//             //Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
//             float nx = Pos.x / Screen.width;
//             float ny = Pos.y / Screen.height;
//             Mat.SetVector("_MaskCenter", new Vector4(nx, ny, 0, 0));
//             //Mat.SetFloat("_X", ObjSize.x * 0.9f * ReverseCamSize);
//             //Mat.SetFloat("_Y", ObjSize.y * 0.9f * ReverseCamSize);
//         }
//         else
//         {
//             float nx = Pos.x / Screen.width;
//             float ny = Pos.y / Screen.height;
//             Mat.SetVector("_MaskCenter", new Vector4(nx, ny, 0, 0));
//             //Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
//             Mat.SetFloat("_Radius", MaxSize);
//         }
//     }

//     IEnumerator Test(string text, bool IsHighlight)
//     {
//         TextDetail.text = "";
//         TextBox.gameObject.SetActive(false);
//         WaitForSecondsRealtime ts = new WaitForSecondsRealtime(0.01f);

//         if (IsHighlight)
//         {
//             if (IsRect)
//             {
//                 Vector2 Gap = (ObjSize - new Vector2(1200, 1200)) * 0.05f;
//                 ObjSize.x = ObjSize.y = 1200;

//                 for (int i = 0; i < 20; i++)
//                 {
//                     yield return ts;
//                     ObjSize += Gap;
//                 }
//             }
//             else
//             {
//                 float Gap = (MaxSize - 1) * 0.05f;
//                 MaxSize = 1;

//                 for (int i = 0; i < 20; i++)
//                 {
//                     yield return ts;
//                     MaxSize += Gap;
//                 }
//             }
//             yield return new WaitForSecondsRealtime(0.2f);
//         }

//         if (text.Length != 0)
//         {
//             TextDetail.text = text;
//             TextBox.gameObject.SetActive(true);
//             LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox);
//             TextDetail.color = new Color(1, 1, 1, 0);
//             Color ColorCnt = new Color(0, 0, 0, 0.1f);

//             for (int i = 0; i < 10; i++)
//             {
//                 TextDetail.color += ColorCnt;
//                 yield return ts;
//             }
//         }

//         TutorialSetting.instance.SetEvent(trigger);
//     }

//     public void ChangeTarget(GameObject Target)
//     {
//         this.Target = Target;
//     }

//     Vector2 TextPos = Vector2.zero;
//     // void CalcPosition()
//     // {
//     //     if (Target == null)
//     //     {
//     //         TextBox.localPosition = Vector2.zero;
//     //     }
//     //     else
//     //     {
//     //         if (IsScreen) Pos = Target.transform.position;
//     //         else //Pos = Camera.main.WorldToScreenPoint(Target.transform.position);
//     //         {
//     //             Pos = TutorialSetting.WorldToScreenInCameraRect(Camera.main, Target.transform.position);
//     //         }

//     //         if (IsRect) TextPos = new Vector2(Pos.x * 1200 / Screen.width - 600, Pos.y * 900 / Screen.height - 450 - ObjSize.y * 0.5f - TextBox.rect.height * 0.5f);
//     //         else TextPos = new Vector2(Pos.x * 1200 / Screen.width - 600, Pos.y * 900 / Screen.height - 450 - ObjSize.y * 0.8f - TextBox.rect.height * 0.5f);

//     //         if (TextPos.y - TextBox.rect.height * 0.5f < -450)
//     //         {
//     //             if (IsRect) TextPos.y += ObjSize.y + TextBox.rect.height;
//     //             else TextPos.y += ObjSize.y * 1.6f + TextBox.rect.height;
//     //         }

//     //         if (TextPos.x > 600 - TextBox.rect.width * 0.5f) TextPos.x = 600 - TextBox.rect.width * 0.5f;
//     //         else if (TextPos.x < -600 + TextBox.rect.width * 0.5f) TextPos.x = -600 + TextBox.rect.width * 0.5f;

//     //         if (TextPos.y > 450 - TextBox.rect.height * 0.5f) TextPos.y = 450 - TextBox.rect.height * 0.5f;
//     //         else if (TextPos.y < -450 + TextBox.rect.height * 0.5f) TextPos.y = -450 + TextBox.rect.height * 0.5f;

//     //         TextBox.localPosition = TextPos;
//     //     }
//     // }
//     private Vector2 GetTargetScreenCenter()
//     {
//         if (Target == null) return Vector2.zero;

//         // UI(RectTransform)면 transform.position 대신 "월드 코너 중심"을 쓴다.
//         if (Target.TryGetComponent(out RectTransform rt))
//         {
//             Vector3[] corners = new Vector3[4];
//             rt.GetWorldCorners(corners); // 0:BL, 2:TR
//             Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;

//             // Screen Space - Camera 기준: uiCam으로 스크린 픽셀 좌표로 변환
//             // (Overlay면 uiCam=null이어도 동작)
//             return RectTransformUtility.WorldToScreenPoint(uiCam, worldCenter);
//         }

//         // 월드 오브젝트(스프라이트 등)
//         return TutorialSetting.WorldToScreenInCameraRect(Camera.main, Target.transform.position);
//     }

//     void CalcPosition()
//     {
//         if (Target == null)
//         {
//             TextBox.anchoredPosition = Vector2.zero;
//             return;
//         }

//         // 1) 월드 -> "카메라 rect 보정된" 스크린 픽셀 좌표
//         // if (IsScreen) Pos = Target.transform.position;
//         // else Pos = TutorialSetting.WorldToScreenInCameraRect(Camera.main, Target.transform.position);
//         Pos = GetTargetScreenCenter();
//         // 2) 스크린 픽셀 -> Canvas 로컬(anchoredPosition)
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             CanvasRect,
//             Pos,
//             uiCam, // Overlay면 null
//             out Vector2 localPoint
//         );

//         // 3) 기존 offset 로직은 localPoint 기준으로 적용
//         float yOffset = IsRect ? (ObjSize.y * 0.5f + TextBox.rect.height * 0.5f)
//                                : (ObjSize.y * 0.8f + TextBox.rect.height * 0.5f);

//         TextPos = localPoint + Vector2.down * yOffset;

//         TextBox.anchoredPosition = TextPos;
//     }





//     public void InActiveRay()
//     {
//         image.raycastTarget = false;
//     }

//     public void ActiveRay()
//     {
//         image.raycastTarget = true;
//     }
// }


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialBorder : MonoBehaviour
{
    [SerializeField] private bool IsRect;
    [SerializeField] private Material Mat;

    [SerializeField] private RectTransform TextBox;
    [SerializeField] private RectTransform CanvasRect;
    [SerializeField] private GameObject Target;

    [Header("Cameras")]
    [SerializeField] private Camera worldCam; // 월드 -> 스크린 변환용
    [SerializeField] private Camera uiCam;    // Canvas가 ScreenSpace-Camera면 RenderCamera, Overlay면 null

    private TMP_Text TextDetail;
    private RectTransform SelfRect;
    private Image image;
    private EventTrigger trigger;

    private Vector2 screenPos;     // Target 중심의 스크린 픽셀 좌표
    private Vector2 objSizePx;     // Target의 스크린 픽셀 크기 (가로/세로)
    private Vector2 maskSizePx;    // 마스크 이미지(SelfRect)의 스크린 픽셀 크기

    private float radiusParam;     // CircleTuto.shader는 dist*2 <= _Radius 로 discard => _Radius = uvRadius * 2

    private void Awake()
    {
        SelfRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        trigger = GetComponent<EventTrigger>();

        Mat = image.material;
        TextDetail = TextBox.GetComponentInChildren<TMP_Text>();

        gameObject.SetActive(false);

        if (worldCam == null) worldCam = Camera.main;
        // uiCam은 인스펙터에서 Canvas Render Camera를 넣는 게 정석.
        // Overlay 캔버스면 null이어도 정상.
    }

    public float Init(GameObject Target, string text, bool IsHighlight, bool KeepEvent)
    {
        if (!KeepEvent)
        {
            try { trigger.triggers.Clear(); }
            catch (System.Exception e) { Debug.Log($"{e} At Clear"); }
        }

        TutorialSetting.instance.CurActive = this;
        this.Target = Target;

        if (Target == null)
        {
            objSizePx = Vector2.zero;
            StartCoroutine(Test(text, IsHighlight));
            return 0;
        }

        EnsureCameras();
        if (worldCam == null) return 0;

        // 1) Target screenPos / objSizePx 계산
        screenPos = GetTargetScreenCenterPx();
        objSizePx = GetTargetScreenSizePx();

        // 2) 마스크(SelfRect)의 screen size 계산
        maskSizePx = GetRectScreenSizePx(SelfRect);

        // 3) 마스크 파라미터 세팅
        ApplyMaskParams();

        gameObject.SetActive(true);
        StartCoroutine(Test(text, IsHighlight));

        return Mathf.Min(objSizePx.x, objSizePx.y);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        if (Target == null) return;

        EnsureCameras();
        if (worldCam == null) return;

        screenPos = GetTargetScreenCenterPx();
        objSizePx = GetTargetScreenSizePx();
        maskSizePx = GetRectScreenSizePx(SelfRect);

        ApplyMaskParams();
    }

    private void EnsureCameras()
    {
        // Destroy된 카메라 참조 복구 (MissingReferenceException 방지)
        if (worldCam == null) worldCam = Camera.main;
        // Overlay 캔버스면 uiCam은 null이 “정상”
        // ScreenSpace-Camera면 인스펙터에서 넣는 게 정석이지만, 비어있으면 worldCam으로 fallback
        if (uiCam == null) uiCam = worldCam;
    }

    // private void ApplyMaskParams()
    // {
    //     // 핵심: screenPos -> SelfRect 로컬 -> UV(0~1)
    //     if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         SelfRect, screenPos, uiCam, out Vector2 localInMask))
    //     {
    //         return;
    //     }

    //     Rect r = SelfRect.rect;
    //     float u = (localInMask.x - r.xMin) / r.width;
    //     float v = (localInMask.y - r.yMin) / r.height;

    //     // 쉐이더는 IN.uv_MainTex(0~1) 기준
    //     Mat.SetVector("_MaskCenter", new Vector4(u, v, 0, 0));

    //     // Target 크기(px) -> 마스크 UV 크기
    //     // BoxTuto.shader: distx < _X && disty < _Y discard => _X/_Y는 "반폭/반높이" (UV)
    //     float halfUvX = (maskSizePx.x <= 0.0001f) ? 0.1f : (objSizePx.x * 0.5f / maskSizePx.x);
    //     float halfUvY = (maskSizePx.y <= 0.0001f) ? 0.1f : (objSizePx.y * 0.5f / maskSizePx.y);

    //     if (IsRect)
    //     {
    //         Mat.SetFloat("_X", halfUvX * 1.75f);
    //         Mat.SetFloat("_Y", halfUvY * 1.5f);
    //     }
    //     else
    //     {
    //         // CircleTuto.shader: dist = distance(uv, center) * 2
    //         // discard if dist <= _Radius  => distance <= _Radius/2
    //         float uvRadius = Mathf.Max(halfUvX, halfUvY);
    //         radiusParam = uvRadius * 2f; // ✅ 중요
    //         Mat.SetFloat("_Radius", radiusParam);
    //     }

    //     // TextBox는 CanvasRect 기준으로 기존처럼 로컬 좌표로 배치
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         CanvasRect, screenPos, uiCam, out Vector2 canvasLocal);

    //     float canvasScaleY = CanvasRect.rect.height / Screen.height;
    //     float objYCanvas = objSizePx.y * canvasScaleY;

    //     float yOffset = IsRect
    //         ? (objYCanvas * 0.5f + TextBox.rect.height * 0.5f)
    //         : (objYCanvas * 0.8f + TextBox.rect.height * 0.5f);

    //     //TextBox.anchoredPosition = canvasLocal + Vector2.down * yOffset;
    //     //TextBox.anchoredPosition = Vector2.zero;


    //     // 1) screenPos(px) -> CanvasRect local point
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, screenPos, uiCam, out Vector2 canvasLocal);

    //     // 2) CanvasRect를 "가상 1200x900" 좌표로 변환
    //     // (CanvasRect 크기가 얼마든지 대응됨)
    //     float halfW = CanvasRect.rect.width * 0.5f;
    //     float halfH = CanvasRect.rect.height * 0.5f;

    //     // canvasLocal은 [-halfW, halfW], [-halfH, halfH] 범위
    //     // 예전 1200x900 기준 좌표로 맵핑
    //     Vector2 virtualPos = new Vector2(
    //         canvasLocal.x * (1200f / (halfW * 2f)),
    //         canvasLocal.y * (900f / (halfH * 2f))
    //     );

    //     // 3) 예전처럼 오프셋 계산(단, ObjSize는 "canvas 기준"으로 바꿔줘야 함)
    //     float objYVirtual = (objSizePx.y / Screen.height) * 900f; // px -> 900기준

    //     float yOffset = IsRect
    //         ? (objYVirtual * 0.5f + TextBox.rect.height * 0.5f)
    //         : (objYVirtual * 0.8f + TextBox.rect.height * 0.5f);

    //     Vector2 textPos = virtualPos + Vector2.down * yOffset;

    //     // 4) 화면 밖으로 나가지 않도록 clamp (예전 로직 그대로)
    //     float clampX = 600f - TextBox.rect.width * 0.5f;
    //     float clampY = 450f - TextBox.rect.height * 0.5f;

    //     textPos.x = Mathf.Clamp(textPos.x, -clampX, clampX);
    //     textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);

    //     // 5) 만약 아래로 내려가서 잘리면 위로 올리기(예전 로직)
    //     if (textPos.y - TextBox.rect.height * 0.5f < -450f)
    //     {
    //         if (IsRect) textPos.y += objYVirtual + TextBox.rect.height;
    //         else textPos.y += objYVirtual * 1.6f + TextBox.rect.height;

    //         // 위로 올린 뒤에도 다시 clamp
    //         textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);
    //     }

    //     // 6) "가상 1200x900" 좌표를 CanvasRect 실제 anchoredPosition으로 변환
    //     Vector2 finalAnchored = new Vector2(
    //         textPos.x * (halfW * 2f / 1200f),
    //         textPos.y * (halfH * 2f / 900f)
    //     );

    //     TextBox.anchoredPosition = finalAnchored;
    // }

    private Vector2 GetTargetScreenCenterPx()
    {
        // UI 타겟(RectTransform)이면 코너 중심
        if (Target.TryGetComponent(out RectTransform rt))
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;
            return RectTransformUtility.WorldToScreenPoint(uiCam, worldCenter);
        }

        // 월드 타겟
        return TutorialSetting.WorldToScreenInCameraRect(worldCam, Target.transform.position);
    }

    private void ApplyMaskParams()
    {
        // =========================
        // 1. 마스크 UV 계산
        // =========================
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            SelfRect, screenPos, uiCam, out Vector2 localInMask))
            return;

        Rect maskRect = SelfRect.rect;

        float u = (localInMask.x - maskRect.xMin) / maskRect.width;
        float v = (localInMask.y - maskRect.yMin) / maskRect.height;

        Mat.SetVector("_MaskCenter", new Vector4(u, v, 0, 0));

        // Target 크기(px) → 마스크 UV 크기
        float halfUvX = (maskSizePx.x <= 0.0001f) ? 0.1f : (objSizePx.x * 0.5f / maskSizePx.x);
        float halfUvY = (maskSizePx.y <= 0.0001f) ? 0.1f : (objSizePx.y * 0.5f / maskSizePx.y);

        if (IsRect)
        {
            // 살짝 여유 주기
            Mat.SetFloat("_X", halfUvX * 1.75f);
            Mat.SetFloat("_Y", halfUvY * 1.5f);
        }
        else
        {
            float uvRadius = Mathf.Max(halfUvX, halfUvY);
            Mat.SetFloat("_Radius", uvRadius * 2f);
        }

        // =========================
        // 2. TextBox 위치 계산
        // =========================

        // screenPos(px) → CanvasRect local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CanvasRect, screenPos, uiCam, out Vector2 canvasLocal);

        // Canvas 실제 크기
        float halfW = CanvasRect.rect.width * 0.5f;
        float halfH = CanvasRect.rect.height * 0.5f;

        // Canvas 좌표 → 가상 1200x900 좌표
        Vector2 virtualPos = new Vector2(
            canvasLocal.x * (1200f / (halfW * 2f)),
            canvasLocal.y * (900f / (halfH * 2f))
        );

        // Target 높이(px) → 가상좌표
        float objYVirtual = (objSizePx.y / Screen.height) * 900f;

        float yOffset = IsRect
            ? (objYVirtual * 0.5f + TextBox.rect.height * 0.5f)
            : (objYVirtual * 0.8f + TextBox.rect.height * 0.5f);

        Vector2 textPos = virtualPos + Vector2.down * yOffset;

        // 화면 밖 clamp
        float clampX = 600f - TextBox.rect.width * 0.5f;
        float clampY = 450f - TextBox.rect.height * 0.5f;

        textPos.x = Mathf.Clamp(textPos.x, -clampX, clampX);
        textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);

        // 아래로 삐져나가면 위로 올리기
        if (textPos.y - TextBox.rect.height * 0.5f < -450f)
        {
            if (IsRect) textPos.y += objYVirtual + TextBox.rect.height;
            else textPos.y += objYVirtual * 1.6f + TextBox.rect.height;

            textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);
        }

        // 가상좌표 → Canvas 실제 anchoredPosition
        Vector2 finalAnchored = new Vector2(
            textPos.x * (halfW * 2f / 1200f),
            textPos.y * (halfH * 2f / 900f)
        );

        TextBox.anchoredPosition = finalAnchored;
    }

    private Vector2 GetTargetScreenSizePx()
    {
        // UI(RectTransform) 크기
        if (Target.TryGetComponent(out RectTransform rt))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            return GetRectScreenSizePx(rt);
        }

        // SpriteRenderer 크기(좀 더 정확하게 bounds를 투영)
        if (Target.TryGetComponent(out SpriteRenderer sr))
        {
            Bounds b = sr.bounds;
            Vector3 c = b.center;
            Vector3 e = b.extents;

            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            for (int xi = -1; xi <= 1; xi += 2)
                for (int yi = -1; yi <= 1; yi += 2)
                    for (int zi = -1; zi <= 1; zi += 2)
                    {
                        Vector3 p = c + Vector3.Scale(e, new Vector3(xi, yi, zi));
                        Vector2 sp = TutorialSetting.WorldToScreenInCameraRect(worldCam, p);
                        min = Vector2.Min(min, sp);
                        max = Vector2.Max(max, sp);
                    }

            return new Vector2(max.x - min.x, max.y - min.y);
        }

        // fallback
        return new Vector2(120, 120);
    }

    private Vector2 GetRectScreenSizePx(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector2 bl = RectTransformUtility.WorldToScreenPoint(uiCam, corners[0]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(uiCam, corners[2]);

        return new Vector2(Mathf.Abs(tr.x - bl.x), Mathf.Abs(tr.y - bl.y));
    }

    private IEnumerator Test(string text, bool IsHighlight)
    {
        TextDetail.text = "";
        TextBox.gameObject.SetActive(false);
        WaitForSecondsRealtime ts = new WaitForSecondsRealtime(0.01f);

        if (IsHighlight)
        {
            // 기존 연출은 유지. (필요하면 여기 연출값 따로 조절)
            yield return new WaitForSecondsRealtime(0.2f);
        }

        if (!string.IsNullOrEmpty(text))
        {
            TextDetail.text = text;
            TextBox.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox);

            TextDetail.color = new Color(1, 1, 1, 0);
            Color colorCnt = new Color(0, 0, 0, 0.1f);

            for (int i = 0; i < 10; i++)
            {
                TextDetail.color += colorCnt;
                yield return ts;
            }
        }

        TutorialSetting.instance.SetEvent(trigger);
    }

    public void ChangeTarget(GameObject target)
    {
        Target = target;

        if (!gameObject.activeSelf) return;
        if (Target == null) return;

        // 카메라 복구/갱신
        // (네 코드에 EnsureCameras() 있으면 그거 호출)
        // EnsureCameras();
        if (worldCam == null) worldCam = Camera.main;
        if (uiCam == null) uiCam = worldCam;

        // 즉시 재계산
        screenPos = GetTargetScreenCenterPx();
        objSizePx = GetTargetScreenSizePx();
        maskSizePx = GetRectScreenSizePx(SelfRect);

        ApplyMaskParams();
    }

    public void InActiveRay() => image.raycastTarget = false;
    public void ActiveRay() => image.raycastTarget = true;
}
