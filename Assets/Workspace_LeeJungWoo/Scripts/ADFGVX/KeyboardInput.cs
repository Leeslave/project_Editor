using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private ADFGVX adfgvx;
   
    Dictionary<KeyCode, Action> keyDictionary;
    private string lastInputKeyCode;                //������ �Էµ� Ű�ڵ� �� ����
    private bool isKeyHoldingDown;                  //Ű�ٿ� ���� ����
    private int holdingDownCounter;                 //Ű�ٿ� ���ķ� �󸶳� ���� FixedFrame�� ���������� ī��Ʈ
    private int ContinuousInputCounter;             //���� �Է��� Ƚ�� - ���� Ƚ���� �ѱ�� ���� �Է� �ӵ��� ��������

    [Header("����� �ҽ� ������Ʈ")]
    public AudioSource audioSource;
    [Header("Ű���� Ŭ�� �� ����� ��� ����")]
    public bool PlayKeyboardAudio;
    [Header("Ű���� Ŭ�� �� ����� Ŭ�� - ���� ���")]
    public AudioClip[] KeyboardClickAudioClips;
    [Header("���콺 Ŭ�� �� ����� ��� ����")]
    public bool PlayMouseAudio;
    [Header("���콺 Ŭ�� �� ����� Ŭ�� - ���� ���")]
    public AudioClip[] MouseClickAudioClips;

    private void Start()
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

        //���� �Է� ���� ���� �ʱ�ȭ
        lastInputKeyCode = "";
        ContinuousInputCounter = 0;
        holdingDownCounter = 0;
    }

    void Update()
    {
        foreach (var dic in keyDictionary)
        {
            //��ųʸ��� �ִ� Ű�ڵ� ���� Ű�ٿ�� ����
            if (Input.GetKeyDown(dic.Key))
            {
                if(PlayKeyboardAudio)
                {
                    //Ű���� �Է� ���带 �������� ����Ѵ�
                    audioSource.clip = KeyboardClickAudioClips[UnityEngine.Random.Range(0, KeyboardClickAudioClips.Length - 1)];
                    audioSource.Play();
                }

                //�Է°��� Ű�� ��ųʸ����� �׼��� �����Ѵ�
                dic.Value();

                //�̹��� �Է��� Ű�ڵ尡 ������ �Է��� Ű�ڵ�� �ٸ��ٸ�
                if (dic.Key.ToString() != lastInputKeyCode)
                {
                    holdingDownCounter = 0;
                    ContinuousInputCounter = 0;
                }

                //lastInputKeyCode�� ���� �̹��� �Է��� Ű�ڵ��� ������ ������
                lastInputKeyCode = dic.Key.ToString();
            }

            //��ųʸ��� �ִ� Ű�ڵ� ���� Ű�ٿ�ǰ� �ִ� ����
            if(Input.GetKey(dic.Key))
            {
                //Ű�ٿ� ���� �������� �˸�
                isKeyHoldingDown = true;
            }
        }

        //� Ű�ڵ�� Ű�ٿ��� ���� ����
        if (!Input.anyKey && isKeyHoldingDown)
        {
            //���� �Է� ���� ������ �� �ʱ�ȭ
            holdingDownCounter = 0;
            ContinuousInputCounter = 0;
            isKeyHoldingDown = false;
        }

        //���콺 ��ư�ٿ�� ����
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && PlayMouseAudio)
        {
            //���콺 Ŭ�� ���带 �������� ���
            audioSource.clip = MouseClickAudioClips[UnityEngine.Random.Range(0, MouseClickAudioClips.Length - 1)];
            audioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        foreach (var dic in keyDictionary)
        {
            //��ųʸ��� �ִ� Ű�ڵ� ���� Ű�ٿ��� ����
            if (Input.GetKey(dic.Key))
            {
                //���� �Է��� �� FixedFrame���� �����Ǿ����� holdingDownCounter�� ���
                if (dic.Key.ToString() == lastInputKeyCode)
                    holdingDownCounter++;

                //���� �Է��� �Ͼ Ƚ���� 3ȸ �̻��̸� ���� �Է� �����̰� 10 FixedFrame���� 2 FixedFrame���� ��ȯ
                float countinuousInputDelay = (ContinuousInputCounter > 3) ? 2 : 10;

                //���� �Է��� ���� �Է� �����̸� ��ȸ�ϸ� 
                if (holdingDownCounter > countinuousInputDelay)
                {
                    //���� �Է� ī���͸� �ʱ�ȭ�ϰ�, ���� �Է��� �Ͼ Ƚ���� �ø��� 
                    holdingDownCounter = 0;
                    ContinuousInputCounter++;

                    //�Է°��� Ű�� ��ųʸ����� �׼��� �����Ѵ�
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
        if (adfgvx.transpositionpart.GetInputField_keyword().GetIsReadyForInput())
            adfgvx.transpositionpart.AddKeyword(value);

        if (adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (adfgvx.encodeDataLoadPart.GetInputField_filePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.GetInputField_filePath().AddInputField(value);

            if (adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.afterDecodingPart.AddInputField(value);
        }
        else
        {
            if (adfgvx.beforeEncodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.beforeEncodingPart.AddInputField(value);

            if (adfgvx.encodeDataSavePart.GetInputField_Title().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Title().AddInputField(value);

            if (adfgvx.encodeDataSavePart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Data().AddInputField(value + " ");
        }
    }

    private void KeyDown_Backspace()
    {
        if (adfgvx.transpositionpart.GetInputField_keyword().GetIsReadyForInput())
            adfgvx.transpositionpart.DeleteKeyword();

        if (adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (adfgvx.encodeDataLoadPart.GetInputField_filePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.GetInputField_filePath().DeleteInputField(1);

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
            if(adfgvx.transpositionpart.GetInputField_keyword().GetIsReadyForInput())
                adfgvx.transpositionpart.OnTransposeDown();

            if (adfgvx.encodeDataLoadPart.GetInputField_filePath().GetIsReadyForInput())
                adfgvx.encodeDataLoadPart.LoadEncodeData();
    
            if(adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.afterDecodingPart.ReturnInputField();
        }
        else if(adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.transpositionpart.GetInputField_keyword().GetIsReadyForInput())
                adfgvx.transpositionpart.OnTransposeReverseDown();
         
            if(adfgvx.beforeEncodingPart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.beforeEncodingPart.GetInputField_Data().ReturnInputField();

            if (adfgvx.encodeDataSavePart.GetInputField_Title().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Title().ReturnInputField();

            if (adfgvx.encodeDataSavePart.GetInputField_Data().GetIsReadyForInput())
                adfgvx.encodeDataSavePart.GetInputField_Data().ReturnInputField();
        }
    }
}