using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnOffButton_Obs : Buttons_Obs
{
    Color BfColor;
    Color AfColor;
    Image image;
    [SerializeField] GameObject Target;
    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        BfColor = image.color;
        AfColor = BfColor - new Color(0.2f,0.2f,0.2f,0);
    }

    protected override void Click(PointerEventData Data)
    {
        Target.SetActive(Target.activeSelf == false);
    }
    protected override void OnPointer(PointerEventData data)
    {
        image.color = AfColor;
    }
    protected override void OutPointer(PointerEventData data)
    {
        image.color = BfColor;
    }
}
