using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorGame.Documents
{
    public sealed class DocumentDesktopWindow : MonoBehaviour, IPointerDownHandler
    {
        private RectTransform _dragBounds;
        private RectTransform _rectTransform;
        private Image _titleImage;
        private Action<DocumentDesktopWindow> _onFocus;
        public void OnPointerDown(PointerEventData data) { Focus(); }

        internal void Initialize(RectTransform area, Image titleImage, Action<DocumentDesktopWindow> onFocus)
        {
            _dragBounds = area;
            _rectTransform = (RectTransform)transform;
            _titleImage = titleImage;
            _onFocus = onFocus;
        }

        public void Focus()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _onFocus?.Invoke(this);
        }

        internal void SetFocused(bool active)
        {
            _titleImage.color = active ? DocumentDesktopUI.Navy : Color.gray;
        }

        internal void Move(Vector2 delta)
        {
            // 포인터와 창 위치는 같은 작업 영역 로컬 좌표를 사용한다.
            var position = _rectTransform.anchoredPosition + delta;
            position.x = Mathf.Clamp(position.x, 0, Mathf.Max(0, _dragBounds.rect.width - _rectTransform.rect.width));
            position.y = Mathf.Clamp(position.y, -Mathf.Max(0, _dragBounds.rect.height - _rectTransform.rect.height), 0);
            _rectTransform.anchoredPosition = position;
        }
    }
}
