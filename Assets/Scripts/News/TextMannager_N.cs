using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_N : MonoBehaviour
{
    [SerializeField] public RectTransform CntSize;
    [SerializeField] public TMP_Text CntText;
    [SerializeField] TMP_Text Insertion;
    [SerializeField] List<GameObject> ForInsert;
    [SerializeField] List<TMP_Text> ForInsertText;
    [NonSerialized] int InsertUsing = 0;
    [NonSerialized] public int InsertIndex = 0;
    [NonSerialized] public Color ColorT = new Color(1, 0, 0, 1);
    [NonSerialized] public Color ColorN = new Color(1, 0, 0, 0);
    [NonSerialized] public bool IsDragged = false;
    [NonSerialized] public TMP_Text Touched = null;
    [NonSerialized] public Outline Sub = null;
    [NonSerialized] public bool IsRevise = true;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (IsRevise == true) Insertion.text = "모드 : 삽입";
            else Insertion.text = "모드 : 수정";
            IsRevise = IsRevise == false;
        }
    }

    public bool EndDrag()
    {
        if (Touched == null) return false;
        else
        {
            Sub.effectColor = ColorN;
            if (IsRevise)
            {
                Touched.text = CntText.text;
            }
            else
            {
                ForInsert[InsertUsing].transform.SetSiblingIndex(InsertIndex);
                ForInsertText[InsertUsing].text = CntText.text;
                ForInsert[InsertUsing++].SetActive(true);
            }
            return true;
        }
    }
}
