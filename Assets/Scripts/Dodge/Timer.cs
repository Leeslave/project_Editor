using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
public class Timer : MonoBehaviour
{
    public PatternManager PM;
    public float MaxTime;           // ���� �ִ� ��Ƴ��ƾ� �ϴ� �ð�
    public bool IsTimeFlow = true;  // ���� �ð��� �帣���� ����
    public float time = 0;          // ���� �ð�
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
            // ���� ���߾� �ϴ� �ð� ��ŭ ������ ���, �ð��� �帧�� ���߰� 2����� ����
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
