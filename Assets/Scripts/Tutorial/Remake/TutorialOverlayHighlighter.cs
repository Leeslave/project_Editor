using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MessageBoxAnchor
{
    TopLeft, TopCenter, TopRight,
    MiddleLeft, MiddleCenter, MiddleRight,
    BottomLeft, BottomCenter, BottomRight,
    Auto  // 자동 배치 (빈 공간 탐색)
}

[System.Serializable]
public class TutorialComponent
{
    public GameObject target;
    public bool autoSkip;           // true = 클릭으로 넘어감, false = 타이핑 + Enter
    public string message;
    public MessageBoxAnchor msgAnchor = MessageBoxAnchor.Auto;
    public Vector2 msgOffset;       // 미세 조정용 추가 오프셋
    public string targetText;       // autoSkip = false 전용: 정답 텍스트 (비워두면 Enter만으로 진행)
}

public class TutorialOverlayHighlighter : MonoBehaviour
{
    [SerializeField] private Camera worldCam;

    [Header("Modules")]
    [SerializeField] private TutorialHighlightConfig config;
    [SerializeField] private TutorialDimHoleController dimHole;
    [SerializeField] private TutorialMessageBox messageBox;
    [SerializeField] private RectTransform canvasRect; // 루트 Canvas의 RectTransform

    [Header("Steps")]
    [SerializeField] private TutorialComponent[] steps;

    [Header("Settings")]
    [SerializeField] private float messageDelay = 1f;
    [SerializeField] private float msgPadding = 20f;

    private int currentIndex;
    private Coroutine stepCoroutine;
    private Coroutine errorCoroutine;
    private Rect currentTargetRect;

    private static readonly WaitForSecondsRealtime WaitErrorMsg = new WaitForSecondsRealtime(1.5f);

    private static readonly Vector2[] Directions =
    {
        new Vector2( 1,  0), // Right
        new Vector2(-1,  0), // Left
        new Vector2( 0,  1), // Top
        new Vector2( 0, -1), // Bottom
        new Vector2( 1,  1), // Top-Right
        new Vector2(-1,  1), // Top-Left
        new Vector2( 1, -1), // Bottom-Right
        new Vector2(-1, -1), // Bottom-Left
    };

    void Start()
    {
        HideMessage();
    }

    // ── Public API ───────────────────────────────────────

    public void Begin()
    {
        currentIndex = 0;
        RunStep(currentIndex);
    }

    public void End()
    {
        if (stepCoroutine != null) StopCoroutine(stepCoroutine);
        dimHole.Hide();
        HideMessage();
    }

    // ── 스텝 진행 ────────────────────────────────────────

    private void RunStep(int index)
    {
        if (index >= steps.Length) { End(); return; }
        if (stepCoroutine != null) StopCoroutine(stepCoroutine);
        stepCoroutine = StartCoroutine(CoStep(steps[index]));
    }

    private IEnumerator CoStep(TutorialComponent step)
    {
        // 1. 하이라이트
        HighlightTarget(step.target.transform);

        // 2. n초 대기 후 메시지 표시 (unscaledTime: 게임 일시정지 상태에서도 동작)
        yield return new WaitForSecondsRealtime(messageDelay);
        ActiveMessage(step.message);

        // 3. 진행 조건 대기
        if (step.autoSkip)
            yield return CoWaitForClick(step.target);
        else
            yield return CoWaitForTypeAndEnter(step.target, step.targetText, step.message);

        // 4. 다음 스텝
        HideMessage();
        currentIndex++;
        RunStep(currentIndex);
    }

    // ── 하이라이트 ───────────────────────────────────────

    private void HighlightTarget(Transform target)
    {
        Vector2 center;
        Vector2 size;

        if (target.TryGetComponent<RectTransform>(out var rectTf))
        {
            Vector3[] corners = new Vector3[4];
            rectTf.GetWorldCorners(corners);
            // corners[0] = bottom-left, [1] = top-left, [2] = top-right, [3] = bottom-right
            center = (corners[0] + corners[2]) / 2f;
            size = new Vector2(
                Vector3.Distance(corners[0], corners[3]),
                Vector3.Distance(corners[0], corners[1])
            );
        }
        else if (target.TryGetComponent<Renderer>(out var rend))
        {
            Bounds bounds = rend.bounds;
            Vector3 ext = bounds.extents;
            Vector3 bc = bounds.center;

            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            for (int sx = -1; sx <= 1; sx += 2)
                for (int sy = -1; sy <= 1; sy += 2)
                    for (int sz = -1; sz <= 1; sz += 2)
                    {
                        Vector2 sp = worldCam.WorldToScreenPoint(bc + new Vector3(ext.x * sx, ext.y * sy, ext.z * sz));
                        minX = Mathf.Min(minX, sp.x); maxX = Mathf.Max(maxX, sp.x);
                        minY = Mathf.Min(minY, sp.y); maxY = Mathf.Max(maxY, sp.y);
                    }

            Vector2 screenOffset = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f) - screenOffset;
            size = new Vector2(maxX - minX, maxY - minY);
        }
        else
        {
            Vector2 screenOffset = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            center = (Vector2)worldCam.WorldToScreenPoint(target.position) - screenOffset;
            size = new Vector2(config.sWidth, config.sHeight);
        }

        if (config.isCircle)
            dimHole.ShowCirclePx(center, Mathf.Min(size.x, size.y) * 0.5f, playPop: true);
        else if (config.isSquare)
            dimHole.ShowRectPx(center, size, cornerRadiusPx: 0f, playPop: true);

        currentTargetRect = new Rect(center.x - size.x * 0.5f, center.y - size.y * 0.5f, size.x, size.y);
    }

    // ── 입력 대기 ────────────────────────────────────────

    // autoSkip = true: 하이라이트된 오브젝트 클릭 시 진행
    private IEnumerator CoWaitForClick(GameObject target)
    {
        var rectTf = target.GetComponent<RectTransform>();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool hit = false;

                if (rectTf != null)
                {
                    // UI 오브젝트: 오버레이 레이캐스트 차단과 무관하게 직접 영역 체크
                    hit = RectTransformUtility.RectangleContainsScreenPoint(rectTf, Input.mousePosition, worldCam);
                }
                else
                {
                    // 일반 오브젝트: EventSystem 레이캐스트
                    var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                    var results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, results);

                    foreach (var r in results)
                    {
                        if (r.gameObject == target || r.gameObject.transform.IsChildOf(target.transform))
                        {
                            hit = true;
                            break;
                        }
                    }
                }

                if (hit) yield break;
            }
            yield return null;
        }
    }
    [SerializeField] RectTransform rectTf;
    [SerializeField] TMP_InputField tmpInput;
    
    [SerializeField] InputField legInput;


    // autoSkip = false: 딤 Raycast 해제 → 물리 클릭 허용 + 텍스트 검증 + Enter 키 입력 시 진행
    private IEnumerator CoWaitForTypeAndEnter(GameObject target, string targetText, string originalMessage)
    {
        var basicInput = target.GetComponentInParent<BasicInputField>();

        // 딤 오버레이 Raycast 해제: IsPointerOverGameObject()가 false가 되어 BasicInputField 클릭 허용
        dimHole.SetRaycastEnabled(false);

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // targetText가 비어있으면 텍스트 검증 없이 바로 진행
                if (string.IsNullOrEmpty(targetText))
                    break;

                // BasicInputField → StringBuffer, 아니면 TMP/Legacy InputField 순서로 읽기
                string inputText = basicInput != null ? basicInput.StringBuffer
                                 : tmpInput  != null ? tmpInput.text
                                 : legInput  != null ? legInput.text
                                 : "";

                if (inputText == targetText)
                    break;

                // 오답: 에러 메시지 표시 후 원래 메시지 복원
                if (errorCoroutine != null) StopCoroutine(errorCoroutine);
                errorCoroutine = StartCoroutine(CoShowErrorMessage(originalMessage));
            }

            yield return null;
        }

        // 스텝 완료 시 Raycast 복원
        dimHole.SetRaycastEnabled(true);
    }

    private IEnumerator CoShowErrorMessage(string originalMessage)
    {
        ActiveMessage("잘못 입력하셨습니다.");
        yield return WaitErrorMsg;
        ActiveMessage(originalMessage);
        errorCoroutine = null;
    }

    // ── 메시지 박스 (추후 구현) ──────────────────────────

    private void ActiveMessage(string message)
    {
        messageBox.Show(message);
        Vector2 msgSize = messageBox.GetSize();
        TutorialComponent step = steps[currentIndex];
        Vector2 pos = CalcMessagePosition(currentTargetRect, msgSize, step.msgAnchor, step.msgOffset);
        messageBox.SetPosition(pos);
    }

    private void HideMessage()
    {
        messageBox.Hide();
    }

    // ── 메시지 박스 위치 계산 ────────────────────────────

    private Vector2 CalcMessagePosition(Rect targetRect, Vector2 msgSize, MessageBoxAnchor anchor, Vector2 offset)
    {
        if (anchor == MessageBoxAnchor.Auto)
            return FindBestPosition(targetRect, msgSize, msgPadding) + offset;

        float tx = targetRect.center.x, ty = targetRect.center.y;
        float tw = targetRect.width * 0.5f, th = targetRect.height * 0.5f;
        float mw = msgSize.x * 0.5f, mh = msgSize.y * 0.5f;

        Vector2 pos = anchor switch
        {
            MessageBoxAnchor.TopLeft => new Vector2(tx - tw - mw, ty + th + mh),
            MessageBoxAnchor.TopCenter => new Vector2(tx, ty + th + mh),
            MessageBoxAnchor.TopRight => new Vector2(tx + tw + mw, ty + th + mh),
            MessageBoxAnchor.MiddleLeft => new Vector2(tx - tw - mw, ty),
            MessageBoxAnchor.MiddleCenter => new Vector2(tx, ty),
            MessageBoxAnchor.MiddleRight => new Vector2(tx + tw + mw, ty),
            MessageBoxAnchor.BottomLeft => new Vector2(tx - tw - mw, ty - th - mh),
            MessageBoxAnchor.BottomCenter => new Vector2(tx, ty - th - mh),
            MessageBoxAnchor.BottomRight => new Vector2(tx + tw + mw, ty - th - mh),
            _ => FindBestPosition(targetRect, msgSize, msgPadding)
        };

        return pos + offset;
    }

    private Vector2 FindBestPosition(Rect targetRect, Vector2 msgSize, float padding)
    {
        foreach (var dir in Directions)
        {
            Vector2 candidate = new Vector2(
                targetRect.center.x + dir.x * (targetRect.width * 0.5f + msgSize.x * 0.5f + padding),
                targetRect.center.y + dir.y * (targetRect.height * 0.5f + msgSize.y * 0.5f + padding)
            );
            Rect msgRect = new Rect(candidate.x - msgSize.x * 0.5f, candidate.y - msgSize.y * 0.5f, msgSize.x, msgSize.y);
            if (IsInsideScreen(msgRect)) return candidate;
        }

        // 카메라 가시 영역 기준으로 클램프
        Rect camRect = GetCameraWorldRect();
        float halfW = msgSize.x * 0.5f;
        float halfH = msgSize.y * 0.5f;
        return new Vector2(
            Mathf.Clamp(targetRect.center.x, camRect.xMin + halfW, camRect.xMax - halfW),
            Mathf.Clamp(targetRect.center.y, camRect.yMin + halfH, camRect.yMax - halfH)
        );
    }

    private bool IsInsideScreen(Rect rect)
    {
        Rect camRect = GetCameraWorldRect();
        return rect.xMin >= camRect.xMin && rect.xMax <= camRect.xMax
            && rect.yMin >= camRect.yMin && rect.yMax <= camRect.yMax;
    }

    // 카메라가 실제로 보는 월드 영역 (Orthographic 전용)
    private Rect GetCameraWorldRect()
    {
        float dist = Mathf.Abs(worldCam.transform.position.z);
        Vector3 bl = worldCam.ViewportToWorldPoint(new Vector3(0, 0, dist));
        Vector3 tr = worldCam.ViewportToWorldPoint(new Vector3(1, 1, dist));
        return Rect.MinMaxRect(bl.x, bl.y, tr.x, tr.y);
    }
}
