using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer_Obs : MonoBehaviour
{
    TMP_Text Text;
    int Hour = 8;
    float Minute = 0;
    private void Awake()
    {
        Text = GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        Minute += Time.deltaTime;
        if (Minute >= 60) { Hour += 1; Minute = 0; }
        Text.text = $"{Hour}".PadLeft(2,'0') + ":" + $"{(int)Minute}".PadLeft(2,'0');
    }
    public string ReturnTime()
    {
        return $"{Hour}".PadLeft(2, '0') + ":" + $"{(int)Minute}".PadLeft(2, '0');
    }
}
