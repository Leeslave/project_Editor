using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private ADFGVX adfgvx;
   
    Dictionary<KeyCode, Action> keyDictionary;
    private string lastInputKeyCode;                //저번 프레임에서의 입력 키
    private bool isKeyHoldingDown;                  //키 홀드 다운 여부
    private int holdingDownCounter;                 //홀드 다운 카운터
    private int ContinuousInputCounter;             //연속 입력 카운터

    [Header("오디오 소스 컴포넌트")]
    public AudioSource audioSource;
    [Header("키보드 클릭 오디오 재생 여부")]
    public bool PlayKeyboardClickAudio;
    [Header("키보드 클릭 오디오 클립 - 랜덤 재생")]
    public AudioClip[] KeyboardClickAudioClips;
    [Header("마우스 클릭 오디오 재생 여부")]
    public bool PlayMouseClickAudio;
    [Header("마우스 클릭 오디오 클립 - 랜덤 재생")]
    public AudioClip[] MouseClickAudioClips;

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

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

        //키 홀드 관련 변수 초기화
        lastInputKeyCode = "";
        ContinuousInputCounter = 0;
        holdingDownCounter = 0;
    }

    void Update()
    {
        foreach (var dic in keyDictionary)
        {
            //키 딕셔너리에 있는 키보드 버튼 다운
            if (Input.GetKeyDown(dic.Key))
            {
                //키보드 버튼 다운 오디오 재생
                if(PlayKeyboardClickAudio)
                {
                    audioSource.clip = KeyboardClickAudioClips[UnityEngine.Random.Range(0, KeyboardClickAudioClips.Length - 1)];
                    audioSource.Play();
                }

                //키코드에 따른 실행
                dic.Value();

                //이번 키코드가 저번 키코드와 다름 
                if (dic.Key.ToString() != lastInputKeyCode)
                {
                    //키 홀드 관련 변수 초기화
                    holdingDownCounter = 0;
                    ContinuousInputCounter = 0;
                }

                lastInputKeyCode = dic.Key.ToString();
            }

            //키보드 버튼 홀드
            if(Input.GetKey(dic.Key))
            {
                isKeyHoldingDown = true;
            }
        }

        //키보드 버튼 업
        if (!Input.anyKey && isKeyHoldingDown)
        {
            //키홀드 관련 변수 초기화
            holdingDownCounter = 0;
            ContinuousInputCounter = 0;
            isKeyHoldingDown = false;
        }

        //마우스 버튼 다운 오디오 재생
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && PlayMouseClickAudio)
        {
            audioSource.clip = MouseClickAudioClips[UnityEngine.Random.Range(0, MouseClickAudioClips.Length - 1)];
            audioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        foreach (var dic in keyDictionary)
        {
            //키 딕셔너리에 있는 키보드 버튼 홀드
            if (Input.GetKey(dic.Key))
            {
                //저번 키코드와 같은 키코드 값을 홀드하는 동안
                if (dic.Key.ToString() == lastInputKeyCode)
                    holdingDownCounter++;

                //연속 입력 카운터 횟수가 3회를 넘기면 연속 입력 간격이 10 픽스드 프레임에서 2 픽스드 프레임으로 바뀐다
                float countinuousInputDelay = (ContinuousInputCounter > 3) ? 2 : 10;

                //연속 입력 간격을 홀드 다운 카운터가 넘기면
                if (holdingDownCounter > countinuousInputDelay)
                {
                    //홀드 다운 카운터를 초기화하고 연속 입력 카운터 획수를 늘린다
                    holdingDownCounter = 0;
                    ContinuousInputCounter++;

                    //키코드에 따른 실행
                    dic.Value();
                }
            }
        }
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

    private void AddInputField(string value)
    {
        if (adfgvx.transpositionpart.GetInputField_Keyword().GetIsReadyForInput())
            adfgvx.transpositionpart.AddKeyword(value);

        if (adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (adfgvx.encodeDataLoadPart.GetInputField_FilePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.GetInputField_FilePath().AddInputField(value);

            if (adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.afterDecodingPart.AddInputField_Data(value);
        }
        else
        {
            if (adfgvx.beforeEncodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.beforeEncodingPart.AddInputField_Data(value);

            if (adfgvx.encodeDataSavePart.GetInputField_Title().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Title().AddInputField(value);

            if (adfgvx.encodeDataSavePart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Data().AddInputField(value + " ");
        }
    }

    private void KeyDown_Backspace()
    {
        if (adfgvx.transpositionpart.GetInputField_Keyword().GetIsReadyForInput())
            adfgvx.transpositionpart.DeleteKeyword();

        if (adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (adfgvx.encodeDataLoadPart.GetInputField_FilePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.GetInputField_FilePath().DeleteInputField(1);

            if (adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.afterDecodingPart.GetInputField_Data().DeleteInputField(2);
        }
        else
        {
            if (adfgvx.beforeEncodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.beforeEncodingPart.DeleteInputField_Data();

            if (adfgvx.encodeDataSavePart.GetInputField_Title().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Title().DeleteInputField(1);

            if (adfgvx.encodeDataSavePart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Data().DeleteInputField(2);
        }
    }

    private void KeyDown_Return()
    {
        if (adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if(adfgvx.transpositionpart.GetInputField_Keyword().GetIsReadyForInput())
                adfgvx.transpositionpart.OnTransposeDown();

            if (adfgvx.encodeDataLoadPart.GetInputField_FilePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.LoadEncodeData();
    
            if(adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.afterDecodingPart.ReturnInputField();
        }
        else if(adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.transpositionpart.GetInputField_Keyword().GetIsReadyForInput())
                adfgvx.transpositionpart.OnTransposeReverseDown();
         
            if(adfgvx.beforeEncodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.beforeEncodingPart.GetInputField_Data().ExitInputField();

            if (adfgvx.encodeDataSavePart.GetInputField_Title().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Title().ExitInputField();

            if (adfgvx.encodeDataSavePart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Data().ExitInputField();
        }
    }
}