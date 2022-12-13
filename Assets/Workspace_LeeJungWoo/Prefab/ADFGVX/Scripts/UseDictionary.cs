using System;
using System.Collections.Generic;
using UnityEngine;

public class UseDictionary : MonoBehaviour
{
    private ADFGVX adfgvx;
    Dictionary<KeyCode, Action> keyDictionary;
    
    public AudioClip[] clickAudioClip;
    private AudioSource audioSource;

    void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();
        audioSource = GetComponent<AudioSource>();

        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Alpha0, KeyDown_0 },
            {KeyCode.Alpha1, KeyDown_1 },
            {KeyCode.Alpha2, KeyDown_2 },
            {KeyCode.Alpha3, KeyDown_3 },
            {KeyCode.Alpha4, KeyDown_4 },
            {KeyCode.Alpha5, KeyDown_5 },
            {KeyCode.Alpha6, KeyDown_6 },
            {KeyCode.Alpha7, KeyDown_7 },
            {KeyCode.Alpha8, KeyDown_8 },
            {KeyCode.Alpha9, KeyDown_9 },
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
            { KeyCode.Slash, KeyDown_Slash },
            { KeyCode.Minus, KeyDown_Minus },
            { KeyCode.Backspace, KeyDown_Backspace },
            { KeyCode.Return, KeyDown_Return }
        };
    }

    void Update()
    {
        
        if (Input.anyKeyDown)
            foreach (var dic in keyDictionary)
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value();

                    //키보드 소리 중 랜덤으로 재생
                    audioSource.clip = clickAudioClip[UnityEngine.Random.Range(0, clickAudioClip.Length-1)];
                    audioSource.Play();
                }
    }
    private void KeyDown_0()
    {
        adfgvx.chiperpart.AddInputField("0");
    }
    private void KeyDown_1()
    {
        adfgvx.chiperpart.AddInputField("1");
    }
    private void KeyDown_2()
    {
        adfgvx.chiperpart.AddInputField("2");
    }
    private void KeyDown_3()
    {
        adfgvx.chiperpart.AddInputField("3");
    }
    private void KeyDown_4()
    {
        adfgvx.chiperpart.AddInputField("4");
    }
    private void KeyDown_5()
    {
        adfgvx.chiperpart.AddInputField("5");
    }
    private void KeyDown_6()
    {
        adfgvx.chiperpart.AddInputField("6");
    }
    private void KeyDown_7()
    {
        adfgvx.chiperpart.AddInputField("7");
    }
    private void KeyDown_8()
    {
        adfgvx.chiperpart.AddInputField("8");
    }
    private void KeyDown_9()
    {
        adfgvx.chiperpart.AddInputField("9");
    }
    private void KeyDown_A()
    {
        adfgvx.transpositionpart.AddKeyWord("A");
        adfgvx.chiperpart.AddInputField("A");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(0 , "A");
    }
    private void KeyDown_B()
    {
        adfgvx.transpositionpart.AddKeyWord("B");
        adfgvx.chiperpart.AddInputField("B");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "B");
    }
    private void KeyDown_C()
    {
        adfgvx.transpositionpart.AddKeyWord("C");
        adfgvx.chiperpart.AddInputField("C");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "C");
    }
    private void KeyDown_D()
    {
        adfgvx.transpositionpart.AddKeyWord("D");
        adfgvx.chiperpart.AddInputField("D");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(1 , "D");
    }
    private void KeyDown_E()
    {
        adfgvx.transpositionpart.AddKeyWord("E");
        adfgvx.chiperpart.AddInputField("E");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "E");
    }
    private void KeyDown_F()
    {
        adfgvx.transpositionpart.AddKeyWord("F");
        adfgvx.chiperpart.AddInputField("F");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(2 , "F");
    }
    private void KeyDown_G()
    {
        adfgvx.transpositionpart.AddKeyWord("G");
        adfgvx.chiperpart.AddInputField("G");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(3, "G");
    }
    private void KeyDown_H()
    {
        adfgvx.transpositionpart.AddKeyWord("H");
        adfgvx.chiperpart.AddInputField("H");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "H");
    }
    private void KeyDown_I()
    {
        adfgvx.transpositionpart.AddKeyWord("I");
        adfgvx.chiperpart.AddInputField("I");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "I");
    }
    private void KeyDown_J()
    {
        adfgvx.transpositionpart.AddKeyWord("J");
        adfgvx.chiperpart.AddInputField("J");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "J");
    }
    private void KeyDown_K()
    {
        adfgvx.transpositionpart.AddKeyWord("K");
        adfgvx.chiperpart.AddInputField("K");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "K");
    }
    private void KeyDown_L()
    {
        adfgvx.transpositionpart.AddKeyWord("L");
        adfgvx.chiperpart.AddInputField("L");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "L");
    }
    private void KeyDown_M()
    {
        adfgvx.transpositionpart.AddKeyWord("M");
        adfgvx.chiperpart.AddInputField("M");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "M");
    }
    private void KeyDown_N()
    {
        adfgvx.transpositionpart.AddKeyWord("N");
        adfgvx.chiperpart.AddInputField("N");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "N");
    }
    private void KeyDown_O()
    {
        adfgvx.transpositionpart.AddKeyWord("O");
        adfgvx.chiperpart.AddInputField("O");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "O");
    }
    private void KeyDown_P()
    {
        adfgvx.transpositionpart.AddKeyWord("P");
        adfgvx.chiperpart.AddInputField("P");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "P");
    }
    private void KeyDown_Q()
    {
        adfgvx.transpositionpart.AddKeyWord("Q");
        adfgvx.chiperpart.AddInputField("Q");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "Q");
    }
    private void KeyDown_R()
    {
        adfgvx.transpositionpart.AddKeyWord("R");
        adfgvx.chiperpart.AddInputField("R");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "R");
    }
    private void KeyDown_S()
    {
        adfgvx.transpositionpart.AddKeyWord("S");
        adfgvx.chiperpart.AddInputField("S");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "S");
    }
    private void KeyDown_T()
    {
        adfgvx.transpositionpart.AddKeyWord("T");
        adfgvx.chiperpart.AddInputField("T");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "T");
    }
    private void KeyDown_U()
    {
        adfgvx.transpositionpart.AddKeyWord("U");
        adfgvx.chiperpart.AddInputField("U");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "U");
    }
    private void KeyDown_W()
    {
        adfgvx.transpositionpart.AddKeyWord("W");
        adfgvx.chiperpart.AddInputField("W");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "W");
    }
    private void KeyDown_V()
    {
        adfgvx.transpositionpart.AddKeyWord("V");
        adfgvx.chiperpart.AddInputField("V");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(4, "V");
    }
    private void KeyDown_X()
    {
        adfgvx.transpositionpart.AddKeyWord("X");
        adfgvx.chiperpart.AddInputField("X");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(5, "X");
    }
    private void KeyDown_Y()
    {
        adfgvx.transpositionpart.AddKeyWord("Y");
        adfgvx.chiperpart.AddInputField("Y");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "Y");
    }
    private void KeyDown_Z()
    {
        adfgvx.transpositionpart.AddKeyWord("Z");
        adfgvx.chiperpart.AddInputField("Z");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "Z");
    }
    private void KeyDown_Slash()
    {
        adfgvx.chiperpart.AddInputField("/");
    }
    private void KeyDown_Minus()
    {
        adfgvx.chiperpart.AddInputField("-");
        adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryADFGVX(6, "-");
    }
    private void KeyDown_Backspace()
    {
        adfgvx.transpositionpart.DeleteKeyWord();
        adfgvx.chiperpart.DeleteInputField();
        adfgvx.intermediatepart.DeleteIntermediateChiper();
    }
    private void KeyDown_Return()
    {
    
        if(adfgvx.transpositionpart.isReadyForInput)
        {
            if (adfgvx.currentmode == ADFGVX.mode.Encoding)
                adfgvx.transpositionpart.OnTransposeReverseDown();
            else
                adfgvx.transpositionpart.OnTransposeDown();
        }

        if(adfgvx.chiperpart.isReadyForInput)
            adfgvx.chiperpart.UpdateChiperTitleAndText();

        if (adfgvx.intermediatepart.isReadyForInput)
            adfgvx.biliteralsubstitutionpart.ResponseUseDictionaryEnter();
    }
}