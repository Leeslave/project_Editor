using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameEditor
{
    public class LocationButton : Button
    {
        public WorldVector location;
        public GameObject selectedIcon;
        
        [System.Serializable] public class DataClickEvent : UnityEvent<WorldVector> { }
        
        public DataClickEvent OnLocationClick;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (IsActive() && IsInteractable())
            {
                OnLocationClick?.Invoke(location);
            }
        }

        public void OnSelect(bool isSelected = true)
        {
            if (selectedIcon != null)
            {
                selectedIcon.gameObject.SetActive(isSelected);
            }
        }
    }
}