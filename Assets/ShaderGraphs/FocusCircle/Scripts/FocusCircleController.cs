using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FocusCircleController : MonoBehaviour, IMaterialInstantiate
{
    #region 머터리얼 관련
    [SerializeField] private RectTransform focusTarget;
    [SerializeField] private Material baseMaterial;

    private static readonly int Color = Shader.PropertyToID("_Color");
    private static readonly int RendererRatio = Shader.PropertyToID("_RendererRatio");
    private static readonly int PointerRelativePos = Shader.PropertyToID("_PointerRelativePos");
    private static readonly int PointerRelativeSize = Shader.PropertyToID("_PointerRelativeSize");
    private static readonly int Strength = Shader.PropertyToID("_Strength");

    private Material ImageMatForRendering { get; set; }

    public void AssignInstanceMaterial()
    {
        if (baseMaterial == null)
        {
            Debug.LogWarning("베이스 머터리얼이 설정되어 있지 않음.");
            return;
        }

        var component = transform.GetComponent<Image>();
        if (component == null)
        {
            Debug.LogWarning("머터리얼 인스턴스를 할당할 이미지 컴포넌트 없음.");
            return;
        }

        var objName = gameObject.name;
        var objId = gameObject.GetInstanceID();
        if (component.material.name.Contains($"{baseMaterial.name} [Instance:{objName}_{objId}]"))
        {
            Debug.LogWarning("이미 베이스 머터리얼의 인스턴스가 할당되어 있음.");
            return;
        }

        var inst = new Material(baseMaterial)
        {
            name = $"{baseMaterial.name} [Instance:{objName}_{objId}]"
        };
        component.material = inst;
        Debug.Log("이미지에 인스턴스 머터리얼 할당 완료.");
    }

    #endregion

    public Image Image { get; set; }

    [SerializeField] float strength;

    private void Awake()
    {
        Image = transform.GetComponent<Image>();

        AssignInstanceMaterial();

        ImageMatForRendering = Image.materialForRendering;
    }

    private void OnEnable()
    {
        Image.color = UnityEngine.Color.clear;

        if(LJWConverter.Instance != null)
            LJWConverter.Instance.GradientImageColor(false, 1f, 1f, new UnityEngine.Color(0.99f, 0.0f, 0.0f, 0.2f), Image);
    }

    private void LateUpdate()
    {
        if (!focusTarget)
            return;

        //상대 위치
        Vector2 posDelta = focusTarget.anchoredPosition - Image.rectTransform.anchoredPosition;
        float x = posDelta.x / Image.rectTransform.rect.width;
        float y = posDelta.y / Image.rectTransform.rect.height;

        //랜더러 비율
        float ratio = Image.rectTransform.rect.width / Image.rectTransform.rect.height;

        //상대 크기
        float z = focusTarget.rect.width / Image.rectTransform.rect.width;
        float w = focusTarget.rect.height / Image.rectTransform.rect.height;

        //전달
        ImageMatForRendering.SetColor(Color, Image.color);
        ImageMatForRendering.SetVector(RendererRatio, new Vector2(1, ratio));
        ImageMatForRendering.SetVector(PointerRelativePos, new Vector2(x, y));
        ImageMatForRendering.SetVector(PointerRelativeSize, new Vector2(z, w));
        ImageMatForRendering.SetFloat(Strength, strength);
    }

}
