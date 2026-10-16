using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(EventTrigger))]
public class TutorialBorder : MonoBehaviour
{
    [SerializeField] private bool _isRect;
    [SerializeField] private Material _maskMaterial;
    [SerializeField] private RectTransform _textBox;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private GameObject _target;
    [Header("Cameras")]
    [SerializeField] private Camera _worldCamera; // 월드 -> 스크린 변환용
    [SerializeField] private Camera _uiCamera;    // Canvas가 ScreenSpace-Camera면 RenderCamera, Overlay면 null
    private TMP_Text _detailText;
    private RectTransform _rectTransform;
    private Image _image;
    private EventTrigger _trigger;
    private Vector2 _screenPosition;     // Target 중심의 스크린 픽셀 좌표
    private Vector2 _targetSizePixels;     // Target의 스크린 픽셀 크기 (가로/세로)
    private Vector2 _maskSizePixels;    // 마스크 이미지(SelfRect)의 스크린 픽셀 크기
    private readonly Vector3[] _worldCorners = new Vector3[4];
    // 조명 머티리얼과 설명창 및 카메라를 준비한다.
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _trigger = GetComponent<EventTrigger>();
        _maskMaterial = new Material(_image.material);
        _image.material = _maskMaterial;
        _detailText = _textBox.GetComponentInChildren<TMP_Text>();
        gameObject.SetActive(false);
        if (_worldCamera == null) _worldCamera = Camera.main;
        // Overlay 캔버스면 null이어도 정상.
    }
    public float Init(GameObject target, string text, bool isHighlight, bool keepEvent)
    {
        if (!keepEvent) _trigger.triggers.Clear();
        TutorialController.Instance.CurrentBorder = this;
        _target = target;
        gameObject.SetActive(true);
        if (target == null)
        {
            _targetSizePixels = Vector2.zero;
            StartCoroutine(ShowDescription(text, isHighlight));
            return 0;
        }
        EnsureCameras();
        if (_worldCamera == null) return 0;
        // 1) Target screenPos / objSizePx 계산
        _screenPosition = GetTargetScreenCenterPx();
        _targetSizePixels = GetTargetScreenSizePx();
        // 2) 마스크(SelfRect)의 screen size 계산
        _maskSizePixels = GetRectScreenSizePx(_rectTransform);
        // 3) 마스크 파라미터 세팅
        ApplyMaskParams();
        gameObject.SetActive(true);
        StartCoroutine(ShowDescription(text, isHighlight));
        return Mathf.Min(_targetSizePixels.x, _targetSizePixels.y);
    }
    // 대상의 화면 위치와 크기를 조명 영역에 반영한다.
    private void Update()
    {
        if (!gameObject.activeSelf) return;
        if (_target == null) return;
        EnsureCameras();
        if (_worldCamera == null) return;
        _screenPosition = GetTargetScreenCenterPx();
        _targetSizePixels = GetTargetScreenSizePx();
        _maskSizePixels = GetRectScreenSizePx(_rectTransform);
        ApplyMaskParams();
    }
    // 씬 변경으로 소실된 카메라 참조를 복구한다.
    private void EnsureCameras()
    {
        // Destroy된 카메라 참조 복구 (MissingReferenceException 방지)
        if (_worldCamera == null) _worldCamera = Camera.main;
        // Overlay 캔버스면 uiCam은 null이 “정상”
        if (_uiCamera == null) _uiCamera = _worldCamera;
    }
    // UI 또는 월드 대상의 화면 중심을 계산한다.
    private Vector2 GetTargetScreenCenterPx()
    {
        // UI 타겟(RectTransform)이면 코너 중심
        if (_target.TryGetComponent(out RectTransform rt))
        {
            rt.GetWorldCorners(_worldCorners);
            Vector3 worldCenter = (_worldCorners[0] + _worldCorners[2]) * 0.5f;
            return RectTransformUtility.WorldToScreenPoint(_uiCamera, worldCenter);
        }
        // 월드 타겟
        return TutorialController.WorldToScreenInCameraRect(_worldCamera, _target.transform.position);
    }
    // 대상 크기와 위치를 조명 머티리얼 및 설명창에 적용한다.
    private void ApplyMaskParams()
    {
        // =========================
        // 1. 마스크 UV 계산
        // =========================
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, _screenPosition, _uiCamera, out Vector2 localInMask))
            return;
        Rect maskRect = _rectTransform.rect;
        float u = (localInMask.x - maskRect.xMin) / maskRect.width;
        float v = (localInMask.y - maskRect.yMin) / maskRect.height;
        _maskMaterial.SetVector("_MaskCenter", new Vector4(u, v, 0, 0));
        // Target 크기(px) → 마스크 UV 크기
        float halfUvX = (_maskSizePixels.x <= 0.0001f) ? 0.1f : (_targetSizePixels.x * 0.5f / _maskSizePixels.x);
        float halfUvY = (_maskSizePixels.y <= 0.0001f) ? 0.1f : (_targetSizePixels.y * 0.5f / _maskSizePixels.y);
        if (_isRect)
        {
            // 살짝 여유 주기
            _maskMaterial.SetFloat("_X", halfUvX * 1.75f);
            _maskMaterial.SetFloat("_Y", halfUvY * 1.5f);
        }
        else
        {
            float uvRadius = Mathf.Max(halfUvX, halfUvY);
            _maskMaterial.SetFloat("_Radius", uvRadius * 2f);
        }
        // =========================
        // 2. TextBox 위치 계산
        // =========================
        // screenPos(px) → CanvasRect local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, _screenPosition, _uiCamera, out Vector2 canvasLocal);
        // Canvas 실제 크기
        float halfW = _canvasRect.rect.width * 0.5f;
        float halfH = _canvasRect.rect.height * 0.5f;
        // Canvas 좌표 → 가상 1200x900 좌표
        Vector2 virtualPos = new Vector2(
            canvasLocal.x * (1200f / (halfW * 2f)),
            canvasLocal.y * (900f / (halfH * 2f))
        );
        // Target 높이(px) → 가상좌표
        float objYVirtual = (_targetSizePixels.y / Screen.height) * 900f;
        float yOffset = _isRect
            ? (objYVirtual * 0.5f + _textBox.rect.height * 0.5f)
            : (objYVirtual * 0.8f + _textBox.rect.height * 0.5f);
        Vector2 textPos = virtualPos + Vector2.down * yOffset;
        // 화면 밖 clamp
        float clampX = 600f - _textBox.rect.width * 0.5f;
        float clampY = 450f - _textBox.rect.height * 0.5f;
        textPos.x = Mathf.Clamp(textPos.x, -clampX, clampX);
        textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);
        // 아래로 삐져나가면 위로 올리기
        if (textPos.y - _textBox.rect.height * 0.5f < -450f)
        {
            if (_isRect) textPos.y += objYVirtual + _textBox.rect.height;
            else textPos.y += objYVirtual * 1.6f + _textBox.rect.height;
            textPos.y = Mathf.Clamp(textPos.y, -clampY, clampY);
        }
        // 가상좌표 → Canvas 실제 anchoredPosition
        Vector2 finalAnchored = new Vector2(
            textPos.x * (halfW * 2f / 1200f),
            textPos.y * (halfH * 2f / 900f)
        );
        _textBox.anchoredPosition = finalAnchored;
    }
    // UI 또는 스프라이트 대상의 화면 크기를 계산한다.
    private Vector2 GetTargetScreenSizePx()
    {
        // UI(RectTransform) 크기
        if (_target.TryGetComponent(out RectTransform rt))
        {
            return GetRectScreenSizePx(rt);
        }
        // SpriteRenderer 크기(좀 더 정확하게 bounds를 투영)
        if (_target.TryGetComponent(out SpriteRenderer sr))
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
                        Vector2 sp = TutorialController.WorldToScreenInCameraRect(_worldCamera, p);
                        min = Vector2.Min(min, sp);
                        max = Vector2.Max(max, sp);
                    }
            return new Vector2(max.x - min.x, max.y - min.y);
        }
        // fallback
        return new Vector2(120, 120);
    }
    // UI 사각형의 모서리를 화면 좌표로 변환해 크기를 계산한다.
    private Vector2 GetRectScreenSizePx(RectTransform rt)
    {
        rt.GetWorldCorners(_worldCorners);
        Vector2 bl = RectTransformUtility.WorldToScreenPoint(_uiCamera, _worldCorners[0]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(_uiCamera, _worldCorners[2]);
        return new Vector2(Mathf.Abs(tr.x - bl.x), Mathf.Abs(tr.y - bl.y));
    }
    // 조명 설명을 표시하고 튜토리얼 입력 이벤트를 연결한다.
    private IEnumerator ShowDescription(string text, bool isHighlight)
    {
        _detailText.text = "";
        _textBox.gameObject.SetActive(false);
        WaitForSecondsRealtime ts = new WaitForSecondsRealtime(0.01f);
        if (isHighlight)
        {
            // 기존 연출은 유지. (필요하면 여기 연출값 따로 조절)
            yield return new WaitForSecondsRealtime(0.2f);
        }
        if (!string.IsNullOrEmpty(text))
        {
            _detailText.text = text;
            _textBox.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_textBox);
            _detailText.color = new Color(1, 1, 1, 0);
            Color colorCnt = new Color(0, 0, 0, 0.1f);
            for (int i = 0; i < 10; i++)
            {
                _detailText.color += colorCnt;
                yield return ts;
            }
        }
        TutorialController.Instance.SetEvent(_trigger);
    }
    public void ChangeTarget(GameObject target)
    {
        _target = target;
        if (!gameObject.activeSelf) return;
        if (_target == null) return;
        // 카메라 복구/갱신
        if (_worldCamera == null) _worldCamera = Camera.main;
        if (_uiCamera == null) _uiCamera = _worldCamera;
        // 즉시 재계산
        _screenPosition = GetTargetScreenCenterPx();
        _targetSizePixels = GetTargetScreenSizePx();
        _maskSizePixels = GetRectScreenSizePx(_rectTransform);
        ApplyMaskParams();
    }
    public void DisableRaycast() => _image.raycastTarget = false;
    public void EnableRaycast() => _image.raycastTarget = true;

    // 조명 영역이 소유한 머티리얼 인스턴스를 해제한다.
    private void OnDestroy()
    {
        if (_maskMaterial != null) Destroy(_maskMaterial);
    }
}
