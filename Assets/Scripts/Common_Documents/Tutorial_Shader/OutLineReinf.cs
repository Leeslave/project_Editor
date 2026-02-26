using UnityEngine;
using UnityEngine.UI;

public class OutLineReinf : MonoBehaviour
{
    RectTransform Rect;
    Image im;
    private void Awake()
    {
        TryGetComponent<RectTransform>(out Rect);
        TryGetComponent<Image>(out im);
    }

    void OnRectTransformDimensionsChange()
    {
        if (Rect == null) return;
        float Ratio = Rect.sizeDelta.y / Rect.sizeDelta.x;
        im.material.SetFloat("_Ratio", Ratio);
    }
}
