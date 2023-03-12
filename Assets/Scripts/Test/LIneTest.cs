using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LIneTest : MonoBehaviour
{
    GraphicRaycaster gr;
    GameObject a1 = null;
    public GameObject Line;

    private void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject CurRay = GRay();
            if (CurRay == null) return;
            Debug.Log(CurRay.name);
            if (a1 == null) a1 = CurRay;
            else
            {
                Vector3 middle = (a1.transform.position + CurRay.transform.position) / 2;
                GameObject c1 = Line_Init(a1.transform.position);
                Vector3[] cnt = { new Vector3(middle.x, a1.transform.position.y, 0), new Vector3(middle.x, CurRay.transform.position.y,0),CurRay.transform.position};
                DrawLine(c1.GetComponent<LineRenderer>(),cnt);

            }
        }
        
    }

    void DrawLine(LineRenderer a, Vector3[] aa)
    {
        a.enabled = true;
        a.SetPositions(aa);
    }

    GameObject Line_Init(Vector3 aa)
    {
        GameObject lb = Instantiate(Line);
        lb.transform.position = aa;
        LineRenderer cnt = lb.GetComponent<LineRenderer>();
        cnt.enabled = false;
        cnt.material.color = Color.red;
        cnt.widthMultiplier = 1f;

        return lb;
    }

    GameObject GRay()
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (gr == null) return null;
        gr.Raycast(ped, results);

        if (results.Count <= 0) return null;
        return results[0].gameObject;
    }
}
