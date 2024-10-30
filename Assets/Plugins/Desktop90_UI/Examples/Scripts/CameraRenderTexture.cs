using UnityEngine;
using UnityEngine.UI;

namespace float_oat.Desktop90.Examples
{
    [RequireComponent(typeof(RawImage))]
    public class CameraRenderTexture : MonoBehaviour
    {
        [SerializeField] private Camera cameraToDisplay = default;
        void Start()
        {
            var rectTransform = GetComponent<RectTransform>();
            var renderTexture = new RenderTexture((int)rectTransform.rect.width, (int)rectTransform.rect.height, 16);
            cameraToDisplay.targetTexture = renderTexture;
            GetComponent<RawImage>().texture = renderTexture;
        }
    }
}
