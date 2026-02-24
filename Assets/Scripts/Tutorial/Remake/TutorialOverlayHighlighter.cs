using UnityEngine;
using UnityEngine.U2D;

public class TutorialOverlayHighlighter : MonoBehaviour
{
    [SerializeField] private Camera worldCam;
    [SerializeField] private RectTransform canvasRect;

    [Header("Modules")]
    [SerializeField] private TutorialHighlightConfig config;
    [SerializeField] private TutorialDimHoleController dimHole;
    //[SerializeField] private TutorialRingController ring;

    private Transform targetTf;
    private bool active;

    public void Begin(Transform target)
    {
        targetTf = target;
        active = true;

        Vector2 screenPos = TutorialCoordUtil.WorldToScreenInCameraRect(worldCam,targetTf.position);


        if (config.isCircle) { dimHole.ShowCirclePx(targetTf.position, config.cRadius, playPop:true); }
        if (config.isSquare) { dimHole.ShowRectPx(targetTf.position, new Vector2(config.sWidth, config.sHeight), cornerRadiusPx:0f, playPop:true); }
    }

    public void End()
    {
        active = false;
        targetTf = null;


        dimHole.Hide();
    }

    private void LateUpdate()
    {
        if (!active || targetTf == null || worldCam == null || canvasRect == null || config == null)
            return;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCam, targetTf.position);
        //dimHole.SetCenterPx(targetTf.position);
    }
}
