using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX : Button
{
    protected ADFGVX GameManager;

    protected override void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<ADFGVX>();
        Init();
    }
}
