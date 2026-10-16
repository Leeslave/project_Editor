using System;
using UnityEngine;

public class DesktopDragState : MonoBehaviour
{
    // 드래그 중인지
    [NonSerialized] public bool IsDragged = false;
    // 현재 드래그 중인 Icon
    [NonSerialized] public GameObject DraggedObject;

    [NonSerialized] public int FileType = -1;

    [NonSerialized] public Action DropAction;

    [NonSerialized] public string IconName = null;
    // 드래그 중인 파일과 드롭 콜백을 초기화한다.
    private void OnDisable()
    {
        FileType = -1;
        DropAction = null;
        IsDragged = false;
        DraggedObject = null;
        IconName = null;
    }
}
