using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_D : MonoBehaviour
{
    [SerializeField] public ToDoList_N TDN;

    [SerializeField] bool IsRecord;

    [SerializeField] GameObject TextsPref;

    [NonSerialized] List<GameObject> TextsObj;
    [NonSerialized] List<MainText_D> Texts;

    bool IsClear = false;

    int ActivateText = 0;

    private void Awake()
    {
        TextsObj = new List<GameObject>(); TextsObj.Add(TextsPref);
        for (int i = 0; i < 9; i++)
        {
            TextsObj.Add(Instantiate(TextsPref, TextsPref.transform.parent));
        }
        foreach (var k in TextsObj)
        {
            Texts.Add(k.GetComponentInChildren<MainText_D>());
        }
    }

    int CurOpen = 0;
}
