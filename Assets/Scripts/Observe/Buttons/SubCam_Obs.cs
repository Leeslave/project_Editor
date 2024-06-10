using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubCam_Obs : Buttons_Obs
{
    Color BfColor;
    Color AfColor;
    Image image;
    [SerializeField] int Area;
    [SerializeField] int Sub;
    [SerializeField] GameObject GG;
    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        BfColor = image.color;
        AfColor = BfColor - new Color(0.2f, 0.2f, 0.2f, 0);
    }

    protected override void Click(PointerEventData Data)
    {
        StageDB_Obs.DB.StageChanger.StageChanger(Area, Sub);
        GG.SetActive(false);
        image.color = BfColor;
        transform.parent.gameObject.SetActive(false);
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
