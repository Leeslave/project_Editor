using UnityEngine;
using UnityEngine.UI;

public class TutorialAspectRatio : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Image _image;
    // 비율 계산에 필요한 UI 컴포넌트를 보관한다.
    private void Awake()
    {
        TryGetComponent<RectTransform>(out _rectTransform);
        TryGetComponent<Image>(out _image);
        OnRectTransformDimensionsChange();
    }

    // UI 비율 변경을 조명 머티리얼에 반영한다.
    private void OnRectTransformDimensionsChange()
    {
        if (_rectTransform == null || _image == null || _rectTransform.rect.width <= 0) return;
        float ratio = _rectTransform.rect.height / _rectTransform.rect.width;
        _image.material.SetFloat("_Ratio", ratio);
    }
}
