using UnityEngine;
using UnityEngine.UI;

namespace float_oat.Desktop90.Examples
{
    public class WindowSettingsWindow : MonoBehaviour
    {
        [SerializeField] private DraggableTopbar draggableTopbar = default;
        [SerializeField] private WindowController windowController = default;

        [Header("Color sliders")]
        [SerializeField] private Slider RedSlider = default;
        [SerializeField] private Slider GreenSlider = default;
        [SerializeField] private Slider BlueSlider = default;

        private Image topbarImage;

        private void Start()
        {
            topbarImage = draggableTopbar.GetComponent<Image>();
        }

        public void SetDragableOutOfScreen(bool value)
        {
            draggableTopbar.KeepWithinScreen = value;
        }

        public void SetFadeInTime(float time)
        {
            windowController.FadeInTime = time;
        }

        public void SetFadeOutTime(float time)
        {
            windowController.FadeOutTime = time;
        }

        public void DoFadeInAndOut()
        {
            windowController.Close();
            windowController.Invoke("Open", windowController.FadeOutTime + 0.5f);
        }

        public void OnColorChange()
        {
            topbarImage.color = new Color(RedSlider.value, GreenSlider.value, BlueSlider.value);
        }
    }
}
