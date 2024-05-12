using System.Collections.Generic;
using UnityEngine;

public class BasicKeyboard : MonoBehaviour
{
    /// <summary> 
    /// 싱글톤 패턴 - Singleton pattern 
    /// </summary>
    public static BasicKeyboard Instance { get; private set; } = null;

    /// <summary>
    /// 이 딕셔너리에는 키 코드에 따른 메소드가 바인드되어 있음 - There is a method matches with keycode in this dictionary
    /// </summary>
    private Dictionary<KeyCode, System.Action> KeyDictionary { get; set; }
    
    public BasicInputField ConnectedInputField { get; set; }

    private string LastInputKeyCode { get; set; } = "";
    private bool IsKeyHoldingDown { get; set; } = false;
    private int HoldingDownCounter { get; set; } = 0;
    private int ContinuousInputCounter { get; set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }

        KeyDictionary = new Dictionary<KeyCode, System.Action>
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
            { KeyCode.Minus, KeyDown_Minus },
            { KeyCode.Backspace, DeleteInputField },
            { KeyCode.Return, ReturnInputField }
        };
    }

    void Update()
    {
        foreach (var dic in KeyDictionary)
        {
            //키 다운
            if (Input.GetKeyDown(dic.Key))
            {
                //키 다운 사운드
                //if (PlayKeyboardClickAudio)
                //{
                //    audioSource.clip = KeyboardClickAudioClips[UnityEngine.Random.Range(0, KeyboardClickAudioClips.Length - 1)];
                //    audioSource.Play();
                //}

                //키 값에 따른 실행
                dic.Value();

                //키 홀드 확인
                if (dic.Key.ToString() != LastInputKeyCode)
                {
                    HoldingDownCounter = 0;
                    ContinuousInputCounter = 0;
                }

                //키 홀드 확인을 위한 업데이트
                LastInputKeyCode = dic.Key.ToString();
                IsKeyHoldingDown = true;
            }
        }

        //키 홀드 종료
        if (!Input.anyKey && IsKeyHoldingDown)
        {
            HoldingDownCounter = 0;
            ContinuousInputCounter = 0;
            IsKeyHoldingDown = false;
        }

        //마우스 다운 사운드
        //if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && PlayMouseClickAudio)
        //{
        //    audioSource.clip = MouseClickAudioClips[UnityEngine.Random.Range(0, MouseClickAudioClips.Length - 1)];
        //    audioSource.Play();
        //}
    }

    private void FixedUpdate()
    {
        foreach (var dic in KeyDictionary)
        {
            //0.02초마다 키 입력을 확인한다
            if (Input.GetKey(dic.Key))
            {
                //홀드 중인지 확인한다
                if (dic.Key.ToString() == LastInputKeyCode)
                    HoldingDownCounter++;

                //인풋 카운터가 3을 넘어가면 인풋 딜레이가 짧아진다
                float continuousInputDelay = (ContinuousInputCounter > 3) ? 2 : 10;

                //홀드가 인풋 딜레이를 넘어섰다면
                if (HoldingDownCounter > continuousInputDelay)
                {
                    //홀드 카운트를 0으로 초기화하고 인풋 카운터가 증가한다
                    HoldingDownCounter = 0;
                    ContinuousInputCounter++;

                    //키 입력한다
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
    private void KeyDown_Minus()
    {
        AddInputField("-");
    }

    private void AddInputField(string value)
    {
        if (ConnectedInputField == null)
            return;
        ConnectedInputField.AddInputField(value);
    }

    private void DeleteInputField()
    {
        if (ConnectedInputField == null)
            return;
        ConnectedInputField.DeleteInputField();
    }

    private void ReturnInputField()
    {
        if(ConnectedInputField == null)
            return;
        ConnectedInputField.ReturnInputField();
    }


}