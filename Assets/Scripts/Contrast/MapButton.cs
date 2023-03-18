using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapButton : MonoBehaviour
{
    public GameObject CitySub;
    public GameObject DistanceResult;
    public Color BfColor;
    public Color AfColor;
    MapButtonManager Manager;
    bool highlighted = false;

    private void Start()
    {
        if (tag == "MapButton_Top") Manager = transform.parent.GetComponent<MapButtonManager>();
        else Manager = transform.parent.transform.parent.transform.parent.GetComponent<MapButtonManager>();
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        MyUi.ButtonInit(GetComponent<EventTrigger>(), OnPointer, OutPointer, ClickPointer);
    }
    public void OnPointer(PointerEventData data) 
    {
        if (!highlighted) GetComponent<Image>().color = AfColor;
    }
    public void OutPointer(PointerEventData data)
    {
        if (!highlighted) GetComponent<Image>().color = BfColor;
    }
    public void ClickPointer(PointerEventData data) 
    { 
        if (tag == "MapButton_Top")
        {
            if (Manager.CurButton != null) Manager.CurButton.SetActive(false);
            highlighted = true;
            GetComponent<Image>().color = BfColor;
            
            CitySub.SetActive(true);
            Manager.CurButton = CitySub;
            Vector3 CurCamPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); CurCamPos.z = 0;
            CitySub.transform.position = CurCamPos;
        }
        else if (tag == "MapButton_Sub")
        {
            Manager.CurButton = null;
            transform.parent.transform.parent.GetComponent<MapButton>().highlighted = false;
            DistanceResult.GetComponent<MakingMapResult>().ClickedReact(transform.GetChild(0).GetComponent<TMP_Text>().text);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
