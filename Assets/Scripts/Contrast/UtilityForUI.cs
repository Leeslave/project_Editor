using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MyUi
{
     public static Vector3 UIPosition(GameObject a) { return a.GetComponent<RectTransform>().anchoredPosition; }
     public static Vector3 UISize(GameObject a) { return a.GetComponent<RectTransform>().sizeDelta; }
     public static void ChangeUIPosition(ref GameObject a, Vector3 l) { a.GetComponent<RectTransform>().anchoredPosition = l; }
     public static GameObject GRay(GraphicRaycaster gr)
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (gr == null) return null;
        gr.Raycast(ped, results);

        if (results.Count <= 0) return null;
        return results[0].gameObject;
    }
    public static void DragUI(GameObject DragingObject, Vector3 AnchorGap) { Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0; DragingObject.transform.position = mousePos - AnchorGap; }
}
