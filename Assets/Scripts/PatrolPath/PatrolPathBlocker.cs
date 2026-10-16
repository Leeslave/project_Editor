using System;
using UnityEngine;

public class PatrolPathBlocker : MonoBehaviour
{
    [SerializeField] private PatrolPathBlockerController pathBlockerController;
    private void Start() => pathBlockerController = transform.GetComponentInParent<PatrolPathBlockerController>();
    private void OnMouseDown() => pathBlockerController.OnMouseDownEvent();
    private void OnMouseDrag() => pathBlockerController.OnMouseDragEvent();
    private void OnMouseUp() => pathBlockerController.OnMouseUpEvent();
}
