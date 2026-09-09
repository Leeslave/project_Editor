using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorGame.Documents
{
    public sealed class DocumentWindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private DocumentDesktopWindow window;
        private RectTransform bounds;
        private Vector2 previous;
        private bool hasPoint;

        internal void Initialize(DocumentDesktopWindow target, RectTransform area)
        {
            window = target;
            bounds = area;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            window.Focus();
            hasPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(bounds, data.position,
                data.pressEventCamera, out previous);
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 point;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bounds, data.position,
                data.pressEventCamera, out point)) return;
            if (hasPoint) window.Move(point - previous);
            previous = point;
            hasPoint = true;
        }
    }
}
