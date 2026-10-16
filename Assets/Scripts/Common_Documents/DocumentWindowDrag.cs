using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorGame.Documents
{
    public sealed class DocumentWindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private DocumentDesktopWindow _window;
        private RectTransform _dragBounds;
        private Vector2 _previousPoint;
        private bool _hasPreviousPoint;

        internal void Initialize(DocumentDesktopWindow target, RectTransform area)
        {
            _window = target;
            _dragBounds = area;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            _window.Focus();
            _hasPreviousPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragBounds, data.position,
                data.pressEventCamera, out _previousPoint);
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 point;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragBounds, data.position,
                data.pressEventCamera, out point)) return;
            if (_hasPreviousPoint) _window.Move(point - _previousPoint);
            _previousPoint = point;
            _hasPreviousPoint = true;
        }
    }
}
