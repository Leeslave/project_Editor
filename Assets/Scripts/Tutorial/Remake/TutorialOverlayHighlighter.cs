using UnityEngine;

public class TutorialOverlayHighlighter : MonoBehaviour
{
    [SerializeField] private Camera worldCam;
    [SerializeField] private RectTransform canvasRect;

    [Header("Modules")]
    [SerializeField] private TutorialHighlightConfig config;
    [SerializeField] private TutorialDimHoleController dimHole;
    [SerializeField] private TutorialRingController ring;

    private Transform targetTf;
    private bool active;

    public void Begin(Transform target)
    {
        targetTf = target;
        active = true;

        dimHole.Begin(target, config);
        gameObject.SetActive(true);
    }

    public void End()
    {
        active = false;
        targetTf = null;

        dimHole.End();
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!active || targetTf == null || worldCam == null || canvasRect == null || config == null)
            return;

        // 링은 anchoredPosition이 필요하니까: 월드 -> Overlay Canvas local
        if (TutorialCoordUtil.WorldToOverlayCanvasLocal(worldCam, canvasRect, targetTf.position, out Vector2 canvasPos, true))
        {
            HighlightShape shape = config.GetShape();
            ring.Apply(shape, canvasPos, config);
        }
    }
}
