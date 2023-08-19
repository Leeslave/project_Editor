using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DumpedIcon : MonoBehaviour
{
    GameObject BIcon;
    Image MyImage;
    TMP_Text MyName;
    int DumpedInd;
    [SerializeField] DumpedIconManager DIM;
    

    private void Awake()
    {
        MyImage = transform.GetChild(0).GetComponent<Image>();
        MyName = transform.GetChild(1).GetComponent<TMP_Text>();
        MyUi.AddEvent(GetComponent<EventTrigger>(),EventTriggerType.PointerClick,Clicked);
    }

    void Clicked(PointerEventData Data)
    {
        if(Data.pointerId == -2)
        {
            DIM.Opt.SetActive(true);
            DIM.Opt.transform.position = transform.position;
            DIM.CurClicked = DumpedInd;
        }
    }

    public void Dumped(GameObject Before,Sprite Image, string Name,int DumpInd)
    {
        BIcon = Before;
        MyImage.sprite = Image;
        MyName.text = Name;
        gameObject.SetActive(true);
        DumpedInd = DumpInd;
    }
    public void DumpedCancle()
    {
        gameObject.SetActive(false);
        BIcon.SetActive(true);
    }
    public void ClearDump()
    {
        BIcon.GetComponent<UIICons>().ClearIcon();
    }
}
