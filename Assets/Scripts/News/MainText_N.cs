using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainText_N : MonoBehaviour
{
    [SerializeField] TextMannager_N TM;
    [SerializeField] TMP_Text Text;
    Outline OL;
    private void Awake()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        OL = GetComponent<Outline>();
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPoint);
        MyUi.AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPoint);
    }
    void OnPoint(PointerEventData Data)
    {
        if (TM.IsDragged)
        {
            TM.InsertIndex = transform.GetSiblingIndex();
            TM.Touched = Text;
            TM.Sub = OL;
            OL.effectColor = TM.ColorT;
        }
    }
    void OutPoint(PointerEventData Data)
    {
        if (TM.IsDragged)
        {
            TM.Touched = null;
            OL.effectColor = TM.ColorN;
        }
    }
}
