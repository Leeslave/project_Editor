using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSub : MonoBehaviour
{
    public string[] CityList;
    public GameObject CitySub;

    private void Start()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { Clicked((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerEnter);
    }
    private void Clicked(PointerEventData data)
    {
        if (!CitySub.activeSelf) CitySub.SetActive(true);
    }
}
