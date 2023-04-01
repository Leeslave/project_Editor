using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameManager GM;
    public float MaxTime;
    public bool IsTimeFlow = false;
    public float time = 0;

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
            }
        }
    }
}
