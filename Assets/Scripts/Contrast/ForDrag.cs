using UnityEngine;
using UnityEngine.EventSystems;

public class ForDrag : UIDragger
{
    public GameObject contrastmanager;
    ContrastManager CM;

    private void Start()
    {
        CM = contrastmanager.GetComponent<ContrastManager>();
    }

    protected override void Click(PointerEventData Data)
    {
        if (CM.JudgeTime) return;
        Dragged.SetAsLastSibling();
        CM.InputCall();
    }

    protected override void DragOn(PointerEventData Data)
    {
        if (!CM.JudgeTime)DragSetting();
    }

    protected override void DragPointer(PointerEventData data)
    {
        if(!CM.JudgeTime)MyUi.DragUI(Dragged.gameObject, AnchorGap);
    }
}
