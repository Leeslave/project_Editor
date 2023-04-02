using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public PatternManager PM;
    public float MaxTime;
    public bool IsTimeFlow = true;
    public float time = 0;
    [Range(5, 30)]
    public float TimeToSurvive;     // How Long Player Have To Survie

    private void Awake()
    {
        MaxTime = TimeToSurvive;
    }

    private void Update()
    {
        if (IsTimeFlow)
        {
            time += Time.deltaTime;
            GetComponent<TMP_Text>().text = string.Format("{0:0.00}", time);
            if (time >= MaxTime)
            {
                time = MaxTime;
                GetComponent<TMP_Text>().text = string.Format("{0:0.00}", time);
                IsTimeFlow = false;
                PM.StartPT(1);
            }
        }
    }
}
