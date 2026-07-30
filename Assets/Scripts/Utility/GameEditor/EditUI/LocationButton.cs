using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
    public class LocationButton : MonoBehaviour
    {
        [SerializeField] public WorldVector location;
        [SerializeField] Image selectedIcon;
        public StartLocationEditUI editManager;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (editManager != null)
            {
                button.onClick.AddListener(() => editManager.ChangeLocation(location));
                button.onClick.AddListener(OnSelect);
            }
        }

        private void OnSelect()
        {
            if (selectedIcon != null)
            {
                selectedIcon.sprite = GetComponent<Image>().sprite;
            }
        }
    }
}