using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasicText : MonoBehaviour
{
    public TextMeshPro TextTMP { get; set; }
    public SpriteRenderer TextFrame { get; set; }
    public SpriteRenderer TextFill { get; set; }

    protected virtual void Awake()
    {
        TextTMP = transform.GetChild(0).GetComponent<TextMeshPro>();
        TextFrame = transform.GetChild(1).GetComponent<SpriteRenderer>();
        TextFill = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }
}
