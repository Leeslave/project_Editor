using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX : Button_Game
{
    protected ADFGVX GameManager;

    protected override void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<ADFGVX>();
        Init();
    }
}
