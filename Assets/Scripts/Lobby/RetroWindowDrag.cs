using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Title-bar dragging, clamped to the desktop canvas.</summary>
public sealed class RetroWindowDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform window;
    private Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (window == null) return;
        window.SetAsLastSibling();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)window.parent,
                eventData.position, eventData.pressEventCamera, out Vector2 point))
            offset = window.anchoredPosition - TopLeftPoint(point);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (window == null) return;
        var parent = (RectTransform)window.parent;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
                eventData.position, eventData.pressEventCamera, out Vector2 point)) return;
        Vector2 target = TopLeftPoint(point) + offset;
        target.x = Mathf.Clamp(target.x, 0, Mathf.Max(0, parent.rect.width - window.rect.width));
        target.y = Mathf.Clamp(target.y, -Mathf.Max(0, parent.rect.height - window.rect.height - 48), 0);
        window.anchoredPosition = target;
    }

    private Vector2 TopLeftPoint(Vector2 point)
    {
        Rect bounds = ((RectTransform)window.parent).rect;
        return new Vector2(point.x - bounds.xMin, point.y - bounds.yMax);
    }
}
