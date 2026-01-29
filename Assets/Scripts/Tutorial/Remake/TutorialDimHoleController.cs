using UnityEngine;
using UnityEngine.UI;

public class TutorialDimHoleController : MonoBehaviour
{
    [Header("Camera & Canvas")]
    [SerializeField] private Camera worldCam; // 주로 mainCamera
    [SerializeField] private RectTransform canvasRect;

    [Header("Dim Image")]
    [SerializeField] private Image dimImage;

    [Header("Visual")]
    [SerializeField] private float featherPx = 10f;
    [SerializeField, Range(0f, 1f)] private float dimAlpha = 0.75f;

    private Material runtimeMat;
    private Transform targetTf;
    private TutorialHighlightConfig config;
    private bool active;

    private void Awake()
    {
        runtimeMat = Instantiate(dimImage.material);
        dimImage.material = runtimeMat;
    }

    public void Begin(Transform target, TutorialHighlightConfig cfg)
    {
        targetTf = target;
        config = cfg;
        active = true;

        dimImage.enabled = true;
        gameObject.SetActive(true);
    }


    public void End()
    {
        active = false;
        targetTf = null;
        config = null;

        dimImage.enabled = false;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!active || targetTf == null || config == null) { return; }

        Vector2 screenPos = TutorialCoordUtil.WorldToScreenInCameraRect(worldCam, targetTf.position);

        Vector2 centerUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        HighlightShape shape = config.GetShape();

        float radiusUV = config.cRadius / Mathf.Min(Screen.width, Screen.height);

        Vector2 rectHalfUV = new Vector2(config.sWidth / Screen.width, config.sHeight / Screen.height) * 0.5f;

        float featherUV = featherPx / Mathf.Min(Screen.width, Screen.height);

        runtimeMat.SetVector("_Center", centerUV);
        runtimeMat.SetFloat("_Shape", shape == HighlightShape.Circle ? 0f : 1f);
        runtimeMat.SetFloat("_Radius", radiusUV);
        runtimeMat.SetVector("_RectHalfSize", rectHalfUV);
        runtimeMat.SetFloat("_Feather", featherUV);
        runtimeMat.SetFloat("_DimAlpha", dimAlpha);
    }

}
