using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public GameObject CurObject;
    public Stack<Tuple<GameObject, string>> ChangedList = new Stack<Tuple<GameObject, string>>();

    public void OptionInit(GameObject ClickedObject)
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
        gameObject.SetActive(true);
        CurObject = ClickedObject;
        Vector3 CurCamPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); CurCamPos.z = 0;
        transform.position = CurCamPos;
        gameObject.transform.SetAsLastSibling();

        ChildInit(transform.GetChild(0).GetComponent<OptionButton>());
        if(ChangedList.Count != 0) ChildInit(transform.GetChild(3).GetComponent<OptionButton>());
        if (ClickedObject.tag == "ReportText")
        {
            ChildInit(transform.GetChild(1).GetComponent<OptionButton>());
            ChildInit(transform.GetChild(2).GetComponent<OptionButton>());
        }
        if(ChangedList.Count!=0) ChildInit(transform.GetChild(3).GetComponent<OptionButton>());
    }

    void ChildInit(OptionButton A)
    {
        A.TouchAble = true;
        A.CurColor.color = new Color(0.8f, 0.8f, 0.8f, 1);
    }
    private void OnDisable()
    {
        CurObject = null;
    }
}
