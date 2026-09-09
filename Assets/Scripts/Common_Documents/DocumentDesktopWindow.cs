using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorGame.Documents
{
    public sealed class DocumentDesktopWindow : MonoBehaviour, IPointerDownHandler
    {
        private RectTransform bounds;
        private RectTransform rect;
        private Image title;
        private Action<DocumentDesktopWindow> focus;
        public void OnPointerDown(PointerEventData data) { Focus(); }

        internal void Initialize(RectTransform area, Image titleImage, Action<DocumentDesktopWindow> onFocus)
        {
            bounds = area;
            rect = (RectTransform)transform;
            title = titleImage;
            focus = onFocus;
        }

        public void Focus()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            focus?.Invoke(this);
        }

        internal void SetFocused(bool active)
        {
            title.color = active ? DocumentDesktopUI.Navy : Color.gray;
        }

        internal void Move(Vector2 delta)
        {
            // Both pointer coordinates and window position are local to the workspace.
            var position = rect.anchoredPosition + delta;
            position.x = Mathf.Clamp(position.x, 0, Mathf.Max(0, bounds.rect.width - rect.rect.width));
            position.y = Mathf.Clamp(position.y, -Mathf.Max(0, bounds.rect.height - rect.rect.height), 0);
            rect.anchoredPosition = position;
        }
    }
}
