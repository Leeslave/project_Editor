using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private ADFGVX adfgvx;
    
    
    Dictionary<KeyCode, Action> keyDictionary;
    private string lastKeyCode;
    private bool holdingDown;
    private int count;
    private int down;

    public AudioSource audioSource;

    [Header("키보드 클릭 시 오디오")]
    public AudioClip[] keyboardClickAudioClips;
    [Header("마우스 클릭 시 오디오")]
    public AudioClip[] mouseClickAudioClips;

    private void Start()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

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
            { KeyCode.Space, KeyDown_Space },
            { KeyCode.Backspace, KeyDown_Backspace },
            { KeyCode.Return, KeyDown_Return }
        };

        lastKeyCode = null;
        count = 0;
        down = 0;
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            foreach (var dic in keyDictionary)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    audioSource.clip = keyboardClickAudioClips[UnityEngine.Random.Range(0, keyboardClickAudioClips.Length - 1)];
                    audioSource.Play();

                    dic.Value();
                    if (dic.Key.ToString() != lastKeyCode)
                    {
                        down = 0;
                        count = 0;
                    }
                    lastKeyCode = dic.Key.ToString();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            audioSource.clip = mouseClickAudioClips[UnityEngine.Random.Range(0, mouseClickAudioClips.Length - 1)];
            audioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        if(Input.anyKey)
        {
            foreach(var dic in keyDictionary)
            {
                if(Input.GetKey(dic.Key))
                {
                    if (dic.Key.ToString() == lastKeyCode)
                    {
                        down++;
                    }

                    float delay = (count > 3) ? 3 : 15 ;

                    if (down > delay)
                    {
                        down = 0;
                        count++;

                        dic.Value();
                    }
                }
            }
            holdingDown = true;
        }

        if(!Input.anyKey && holdingDown)
        {
            down = 0;
            count = 0;
            holdingDown = false;
        }
    }


    private void AddInputField(string value)
    {
        adfgvx.transpositionpart.AddKeyword(value);

        adfgvx.afterDecodingPart.AddInputFieldByKeyboard(value);

        adfgvx.beforeEncodingPart.AddInputFieldByKeyboard(value);

        adfgvx.encodeDataLoadPart.GetInputField_filePath().AddInputFieldByKeyboard(value);

        adfgvx.encodeDataSavePart.GetInputField_Title().AddInputFieldByKeyboard(value);
        adfgvx.encodeDataSavePart.AddInputField_DataByKeyboard(value);
    }

    private void KeyDown_0()
    {
        AddInputField("0");
    }
    private void KeyDown_1()
    {
        AddInputField("1");
    }
    private void KeyDown_2()
    {
        AddInputField("2");
    }
    private void KeyDown_3()
    {
        AddInputField("3");
    }
    private void KeyDown_4()
    {
        AddInputField("4");
    }
    private void KeyDown_5()
    {
        AddInputField("5");
    }
    private void KeyDown_6()
    {
        AddInputField("6");
    }
    private void KeyDown_7()
    {
        AddInputField("7");
    }
    private void KeyDown_8()
    {
        AddInputField("8");
    }
    private void KeyDown_9()
    {
        AddInputField("9");
    }
    private void KeyDown_A()
    {
        AddInputField("A");
    }
    private void KeyDown_B()
    {
        AddInputField("B");
    }
    private void KeyDown_C()
    {
        AddInputField("C");
    }
    private void KeyDown_D()
    {
        AddInputField("D");
    }
    private void KeyDown_E()
    {
        AddInputField("E");
    }
    private void KeyDown_F()
    {
        AddInputField("F");
    }
    private void KeyDown_G()
    {
        AddInputField("G");
    }
    private void KeyDown_H()
    {
        AddInputField("H");
    }
    private void KeyDown_I()
    {
        AddInputField("I");
    }
    private void KeyDown_J()
    {
        AddInputField("J");
    }
    private void KeyDown_K()
    {
        AddInputField("K");
    }
    private void KeyDown_L()
    {
        AddInputField("L");
    }
    private void KeyDown_M()
    {
        AddInputField("M");
    }
    private void KeyDown_N()
    {
        AddInputField("N");
    }
    private void KeyDown_O()
    {
        AddInputField("O");
    }
    private void KeyDown_P()
    {
        AddInputField("P");
    }
    private void KeyDown_Q()
    {
        AddInputField("Q");
    }
    private void KeyDown_R()
    {
        AddInputField("R");
    }
    private void KeyDown_S()
    {
        AddInputField("S");
    }
    private void KeyDown_T()
    {
        AddInputField("T");
    }
    private void KeyDown_U()
    {
        AddInputField("U");
    }
    private void KeyDown_W()
    {
        AddInputField("W");
    }
    private void KeyDown_V()
    {
        AddInputField("V");
    }
    private void KeyDown_X()
    {
        AddInputField("X");
    }
    private void KeyDown_Y()
    {
        AddInputField("Y");
    }
    private void KeyDown_Z()
    {
        AddInputField("Z");
    }
    private void KeyDown_Slash()
    {
    }
    private void KeyDown_Minus()
    {
        AddInputField("-");
    }
    private void KeyDown_Space()
    {
        AddInputField(" ");
    }
    private void KeyDown_Backspace()
    {
        adfgvx.transpositionpart.DeleteKeyword();
        adfgvx.encodeDataLoadPart.GetInputField_filePath().DeleteInputFieldByKeyboard(1);

        adfgvx.afterDecodingPart.GetInputField_Data().DeleteInputFieldByKeyboard(2);

        adfgvx.beforeEncodingPart.DeleteInputField_DataByKeyboard();

        adfgvx.encodeDataSavePart.GetInputField_Title().DeleteInputFieldByKeyboard(1);
        adfgvx.encodeDataSavePart.GetInputField_Data().DeleteInputFieldByKeyboard(2);
    }
    private void KeyDown_Return()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.transpositionpart.OnTransposeDownByKeyboard();
            adfgvx.encodeDataLoadPart.LoadEncodeDataByKeyboard();
            adfgvx.afterDecodingPart.ReturnInputFieldByKeyboard();
        }
        else if(adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.transpositionpart.OnTransposeReverseDownByKeyboard();
            adfgvx.beforeEncodingPart.GetInputField_Data().ReturnInputFieldByKeyboard();
        }

        adfgvx.encodeDataSavePart.GetInputField_Title().ReturnInputFieldByKeyboard();
        adfgvx.encodeDataSavePart.GetInputField_Data().ReturnInputFieldByKeyboard();
    }
}