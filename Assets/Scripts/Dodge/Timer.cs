using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
public class Timer : MonoBehaviour
{
    public PatternManager PM;
    public float MaxTime;           // 현재 최대 살아남아야 하는 시간
    public bool IsTimeFlow = true;  // 현재 시간이 흐르는지 여부
    public float time = 0;          // 현재 시간
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
            // 현재 버텨야 하는 시간 만큼 버텼을 경우, 시간의 흐름을 멈추고 2페이즈를 생성
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
