using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MyUi
{
     public static Vector3 UIPosition(GameObject a) { return a.GetComponent<RectTransform>().anchoredPosition; }    // transform.position -> RectTransform.position
     public static Vector3 UISize(GameObject a) { return a.GetComponent<RectTransform>().sizeDelta; }     // UI 기준의 Size 반환
     public static void ChangeUIPosition(ref GameObject a, Vector3 l) { a.GetComponent<RectTransform>().anchoredPosition = l; }
     public static GameObject GRay(GraphicRaycaster gr)     // Graphic Raycast
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
    public static int StringToInt(string cnt) { int a = 0; foreach (char b in cnt) a = a * 10 + (b - '0'); return a; }
    public static void AddEvent(EventTrigger eventTrigger, EventTriggerType Type, Action<PointerEventData> Event)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = Type;
        entry.callback.AddListener((data) => { Event((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    public static void ButtonInit(EventTrigger eventTrigger, Action<PointerEventData> OnPointer, Action<PointerEventData> OutPointer, Action<PointerEventData> ClickPointer)
    {
        //data.pointerId : left -> -1, right -> -2, Wheel -> -3;
        if (OnPointer != null) AddEvent(eventTrigger, EventTriggerType.PointerEnter, OnPointer);

        if (OutPointer != null) AddEvent(eventTrigger, EventTriggerType.PointerExit, OutPointer);

        if (ClickPointer != null) AddEvent(eventTrigger, EventTriggerType.PointerClick, ClickPointer);
    }

    public static class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }
}
