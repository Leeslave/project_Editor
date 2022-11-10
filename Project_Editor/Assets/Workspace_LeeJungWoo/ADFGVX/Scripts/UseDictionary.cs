using System;
using System.Collections.Generic;
using UnityEngine;

public class UseDictionary : MonoBehaviour
{
    TranspositionPart transpositionpart;
    ChiperPart chiperpart;

    Dictionary<KeyCode, Action> keyDictionary;

    void Awake()
    {
        transpositionpart = GameObject.Find("TranspositionPart").GetComponent<TranspositionPart>();
        chiperpart = GameObject.Find("ChiperPart").GetComponent<ChiperPart>();
        
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
            { KeyCode.Backspace, KeyDown_Backspace },
            { KeyCode.Slash, KeyDown_Slash }
        };
    }

    void Update()
    {
        if (Input.anyKeyDown)
            foreach (var dic in keyDictionary)
                if (Input.GetKeyDown(dic.Key))
                    dic.Value();
    }

    private void KeyDown_A()
    {
        transpositionpart.AddKeyWord("A");
        chiperpart.AddInputField("A");
    }
    private void KeyDown_B()
    {
        transpositionpart.AddKeyWord("B");
        chiperpart.AddInputField("B");
    }
    private void KeyDown_C()
    {
        transpositionpart.AddKeyWord("C");
        chiperpart.AddInputField("C");
    }
    private void KeyDown_D()
    {
        transpositionpart.AddKeyWord("D");
        chiperpart.AddInputField("D");
    }
    private void KeyDown_E()
    {
        transpositionpart.AddKeyWord("E");
        chiperpart.AddInputField("E");
    }
    private void KeyDown_F()
    {
        transpositionpart.AddKeyWord("F");
        chiperpart.AddInputField("F");
    }
    private void KeyDown_G()
    {
        transpositionpart.AddKeyWord("G");
        chiperpart.AddInputField("G");
    }
    private void KeyDown_H()
    {
        transpositionpart.AddKeyWord("H");
        chiperpart.AddInputField("H");
    }
    private void KeyDown_I()
    {
        transpositionpart.AddKeyWord("I");
        chiperpart.AddInputField("I");
    }
    private void KeyDown_J()
    {
        transpositionpart.AddKeyWord("J");
        chiperpart.AddInputField("J");
    }
    private void KeyDown_K()
    {
        transpositionpart.AddKeyWord("K");
        chiperpart.AddInputField("K");
    }
    private void KeyDown_L()
    {
        transpositionpart.AddKeyWord("L");
        chiperpart.AddInputField("L");
    }
    private void KeyDown_M()
    {
        transpositionpart.AddKeyWord("M");
        chiperpart.AddInputField("M");
    }
    private void KeyDown_N()
    {
        transpositionpart.AddKeyWord("N");
        chiperpart.AddInputField("N");
    }
    private void KeyDown_O()
    {
        transpositionpart.AddKeyWord("O");
        chiperpart.AddInputField("O");
    }
    private void KeyDown_P()
    {
        transpositionpart.AddKeyWord("P");
        chiperpart.AddInputField("P");
    }
    private void KeyDown_Q()
    {
        transpositionpart.AddKeyWord("Q");
        chiperpart.AddInputField("Q");
    }
    private void KeyDown_R()
    {
        transpositionpart.AddKeyWord("R");
        chiperpart.AddInputField("R");
    }
    private void KeyDown_S()
    {
        transpositionpart.AddKeyWord("S");
        chiperpart.AddInputField("S");
    }
    private void KeyDown_T()
    {
        transpositionpart.AddKeyWord("T");
        chiperpart.AddInputField("T");
    }
    private void KeyDown_U()
    {
        transpositionpart.AddKeyWord("U");
        chiperpart.AddInputField("U");
    }
    private void KeyDown_W()
    {
        transpositionpart.AddKeyWord("W");
        chiperpart.AddInputField("W");
    }
    private void KeyDown_V()
    {
        transpositionpart.AddKeyWord("V");
        chiperpart.AddInputField("V");
    }
    private void KeyDown_X()
    {
        transpositionpart.AddKeyWord("X");
        chiperpart.AddInputField("X");
    }
    private void KeyDown_Y()
    {
        transpositionpart.AddKeyWord("Y");
        chiperpart.AddInputField("Y");
    }
    private void KeyDown_Z()
    {
        transpositionpart.AddKeyWord("Z");
        chiperpart.AddInputField("Z");
    }
    private void KeyDown_Slash()
    {
        chiperpart.AddInputField("/");
    }
    private void KeyDown_Backspace()
    {
        transpositionpart.DeleteKeyWord();
        chiperpart.DeleteInputField();
    }
}