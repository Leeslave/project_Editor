using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextChanger_M : MonoBehaviour
{
    RectTransform s;
    RectTransform S;
    TMP_Text j;


    private void Awake()
    {
        S = GetComponent<RectTransform>();
        s = transform.GetChild(0).GetComponent<RectTransform>();
        j = transform.GetChild(0).GetComponent<TMP_Text>();
        
    }

    public void Changer(string a)
    {
        j.text = a;
        Invoke("SizeChange", 0.05f);
    }

    void SizeChange()
    {
        S.sizeDelta = s.sizeDelta;
    }
}
