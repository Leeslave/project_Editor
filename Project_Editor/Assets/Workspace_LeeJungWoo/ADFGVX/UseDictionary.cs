using System;
using System.Collections.Generic;
using UnityEngine;

public class UseDictionary : MonoBehaviour
{
    TranspositionPart TranspositionPart;
    Dictionary<KeyCode, Action> keyDictionary;

    void Awake()
    {
        TranspositionPart = GameObject.Find("TranspositionPart").GetComponent<TranspositionPart>();
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            { KeyCode.A, KeyDown_A },
            { KeyCode.B, KeyDown_B },
            { KeyCode.C, KeyDown_C },
            { KeyCode.D, KeyDown_D },
            { KeyCode.E, KeyDown_E },
            { KeyCode.F, KeyDown_F },
            { KeyCode.G, KeyDown_G },
            { KeyCode.H, KeyDown_H },
            { KeyCode.I, KeyDown_I },
            { KeyCode.J, KeyDown_J },
            { KeyCode.K, KeyDown_K },
            { KeyCode.L, KeyDown_L },
            { KeyCode.M, KeyDown_M },
            { KeyCode.N, KeyDown_N },
            { KeyCode.O, KeyDown_O },
            { KeyCode.P, KeyDown_P },
            { KeyCode.Q, KeyDown_Q },
            { KeyCode.R, KeyDown_R },
            { KeyCode.S, KeyDown_S },
            { KeyCode.T, KeyDown_T },
            { KeyCode.U, KeyDown_U },
            { KeyCode.W, KeyDown_W },
            { KeyCode.V, KeyDown_V },
            { KeyCode.X, KeyDown_X },
            { KeyCode.Y, KeyDown_Y },
            { KeyCode.Z, KeyDown_Z },
            { KeyCode.Backspace, KeyDown_Backspace }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
            foreach (var dic in keyDictionary)
                if (Input.GetKeyDown(dic.Key))
                    dic.Value();
         
    }

    private void KeyDown_A()
    {
        TranspositionPart.AddText("A");
    }
    private void KeyDown_B()
    {
        TranspositionPart.AddText("B");
    }
    private void KeyDown_C()
    {
        TranspositionPart.AddText("C");
    }
    private void KeyDown_D()
    {
        TranspositionPart.AddText("D");
    }
    private void KeyDown_E()
    {
        TranspositionPart.AddText("E");
    }
    private void KeyDown_F()
    {
        TranspositionPart.AddText("F");
    }
    private void KeyDown_G()
    {
        TranspositionPart.AddText("G");
    }
    private void KeyDown_H()
    {
        TranspositionPart.AddText("H");
    }
    private void KeyDown_I()
    {
        TranspositionPart.AddText("I");
    }
    private void KeyDown_J()
    {
        TranspositionPart.AddText("J");
    }
    private void KeyDown_K()
    {
        TranspositionPart.AddText("K");
    }
    private void KeyDown_L()
    {
        TranspositionPart.AddText("L");
    }
    private void KeyDown_M()
    {
        TranspositionPart.AddText("M");
    }
    private void KeyDown_N()
    {
        TranspositionPart.AddText("N");
    }
    private void KeyDown_O()
    {
        TranspositionPart.AddText("O");
    }
    private void KeyDown_P()
    {
        TranspositionPart.AddText("P");
    }
    private void KeyDown_Q()
    {
        TranspositionPart.AddText("Q");
    }
    private void KeyDown_R()
    {
        TranspositionPart.AddText("R");
    }
    private void KeyDown_S()
    {
        TranspositionPart.AddText("S");
    }
    private void KeyDown_T()
    {
        TranspositionPart.AddText("T");
    }
    private void KeyDown_U()
    {
        TranspositionPart.AddText("U");
    }
    private void KeyDown_W()
    {
        TranspositionPart.AddText("W");
    }
    private void KeyDown_V()
    {
        TranspositionPart.AddText("V");
    }
    private void KeyDown_X()
    {
        TranspositionPart.AddText("X");
    }
    private void KeyDown_Y()
    {
        TranspositionPart.AddText("Y");
    }
    private void KeyDown_Z()
    {
        TranspositionPart.AddText("Z");
    }
    private void KeyDown_Backspace()
    {
        TranspositionPart.DeleteText();
    }
}