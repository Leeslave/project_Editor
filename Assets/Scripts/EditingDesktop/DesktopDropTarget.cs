using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopDropTarget : MonoBehaviour
{

    protected bool CanAttach = true;

    // 포인터 진입과 이탈에 드롭 대상 등록을 연결한다.
    private void Awake()
    {
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger eT = GetComponent<EventTrigger>();
        UiEventUtility.AddEvent(eT,EventTriggerType.PointerEnter,(PointerEventData data) => EditingDesktopController.Instance.DragState.DropAction = HandleDrop);
        UiEventUtility.AddEvent(eT, EventTriggerType.PointerExit, OutPointer);
    }

    // 포인터가 벗어나면 등록한 드롭 동작을 해제한다.
    private void OutPointer(PointerEventData data)
    {
        if (EditingDesktopController.Instance.DragState.DropAction == this.HandleDrop) EditingDesktopController.Instance.DragState.DropAction = null;
    }

    // 드래그한 파일 종류에 맞는 편집 화면을 연다.
    private void HandleDrop()
    {
        if (EditingDesktopController.Instance.DragState.FileType >=0)
        {
            if (!CanAttach) return;
            CanAttach = false;
            switch (EditingDesktopController.Instance.DragState.FileType)
            {
                case 1: StartCoroutine(LoadProfileFile()); break;
                case 2: StartCoroutine(LoadNewsFile()); break;
                default: CanAttach = true; ShowDropError(); break;
            }
        }
        else
        {
            ShowDropError();
        }
    }
    protected virtual IEnumerator LoadProfileFile() { yield return null; }
    protected virtual IEnumerator LoadNewsFile() { yield return null; }

    protected virtual void ShowDropError() { }

}
