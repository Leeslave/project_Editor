using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class EyePart : MonoBehaviour
{
    private TextMeshPro time;

    private void Start()
    {
        time = transform.Find("Time").GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        time.text = DateTime.Now.ToString("HH:mm:ss");
    }
}
