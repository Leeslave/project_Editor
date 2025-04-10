using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TargetAreaPopUpV3 : MonoBehaviour
{
    private Image Background { get; set; }
    private TMP_Text[] Instruction { get; set; }

    private void Awake()
    {
        Background = transform.GetChild(0).GetComponent<Image>();
        Instruction = transform.GetChild(2).GetComponentsInChildren<TMP_Text>();
        
        //비가시
        Background.color = Color.clear;
        foreach(TMP_Text i in Instruction)
            i.color = Color.clear;
    }

    private void OnEnable()
    {
        //가시
        LJWConverter.Instance.GradientImageColor(false, 0f, 1f, new Color(0f, 0f, 0f, 0.95f), Background);
        foreach(TMP_Text i in Instruction)
            LJWConverter.Instance.GradientTMPColor(false, 1f, 1f, Color.white, i);
    }
}
