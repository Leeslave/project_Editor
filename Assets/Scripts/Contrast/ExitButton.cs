using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


// Button Must Be TMP_Text
public class ExitButton : MonoBehaviour
{
    public GameObject contrastmanager;
    ContrastManager CM;

    Color BfColor;
    TMP_Text text;
    private void Start()
    {
        CM = contrastmanager.GetComponent<ContrastManager>();
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, Click);
        text = GetComponent<TMP_Text>();
        BfColor = text.color;
    }

    void OnPointer(PointerEventData data)
    {
        if(!CM.JudgeTime)text.color = Color.red;
    }

    void OutPointer(PointerEventData data)
    {
        if(!CM.JudgeTime)text.color = BfColor;
    }

    void Click(PointerEventData Data)
    {
        if (CM.JudgeTime) return;
        CM.InputCall();

        text.color = BfColor;
        transform.parent.gameObject.SetActive(false);
    }
}
