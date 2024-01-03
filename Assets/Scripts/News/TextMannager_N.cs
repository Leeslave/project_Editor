using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMannager_N : MonoBehaviour
{
    [SerializeField] DB_M DB;
    [SerializeField] public RectTransform CntSize;
    [SerializeField] public TMP_Text CntText;
    [SerializeField] TMP_Text Insertion;
    [SerializeField] List<GameObject> ForInsert;
    [SerializeField] List<TMP_Text> ForInsertText;
    [SerializeField] Transform TextField;
    [NonSerialized] int InsertUsing = 0;
    [NonSerialized] public int InsertIndex = 0;
    [NonSerialized] public int TouchedIndex = 0;
    [NonSerialized] public Color ColorT = new Color(1, 0, 0, 1);
    [NonSerialized] public Color ColorN = new Color(1, 0, 0, 0);
    [NonSerialized] public bool IsDragged = false;
    [NonSerialized] public TMP_Text Touched = null;
    [NonSerialized] public Outline Sub = null;
    [NonSerialized] public bool IsRevise = true;
    [NonSerialized] public int TryCount = 0;
    [NonSerialized] public int Health = 0;
    [NonSerialized] public int MaxHealth = 0;
    [NonSerialized] public int CurNews;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (IsRevise == true) Insertion.text = "모드 : 삽입";
            else Insertion.text = "모드 : 수정";
            IsRevise = IsRevise == false;
        }
    }

    public bool EndDrag(int ind)
    {
        if (Touched == null) return false;
        else
        {
            TryCount--;
            Sub.effectColor = ColorN;
            if (IsRevise)
            {
/*                print(Touched.text);
                print(CntText.text);*/
                Touched.text = CntText.text;
            }
            else
            {
                bool sub;
                TMP_Text cnt = null;
                Transform cnt2 = null;
                try
                {
                    cnt2 = TextField.GetChild(InsertIndex - 1);
                    cnt = cnt2.GetChild(0).GetComponent<TMP_Text>();
                    sub = cnt.text == "";
                }
                catch { sub = false; }

                if (sub)
                {
                    cnt.text = CntText.text;
                }
                else
                {
                    ForInsert[InsertUsing].transform.SetSiblingIndex(InsertIndex);
                    ForInsertText[InsertUsing].text = CntText.text;
                    ForInsert[InsertUsing++].SetActive(true);
                    Health--;
                }
            }
            return true;
        }
    }
}
