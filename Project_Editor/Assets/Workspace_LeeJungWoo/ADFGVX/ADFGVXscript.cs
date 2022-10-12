using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVXscript : MonoBehaviour
{
    private char[,] Array = new char[6, 6];
    private enum Codemode//ADFGVX모드
    { Encoding,Decoding};
    private Codemode CurrentCodemode;//현재 ADFGVX모드

    private struct DecodeElemnt//복호화 엘레멘트
    {
        public int row;
        public int line;
    }
    private DecodeElemnt CurrentDecodeElement;//현재 복호화 엘레멘트

    public ElementButton[] Buttons = new ElementButton[36];
    public Mode ModeBox;
    public InterChiper InterChiperBox;
    public Info InfoBox;

    private void Start()
    {
        UpdateArray("D");
        InterChiperBox.ClearText();
        UpdateInfoBox("환영합니다!");
        UpdateInfoBoxDelay(0.5f, "ADFGVX 테이블을 선택하십시오.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateArray("0");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX 테이블 1번, 적용되었습니다");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateArray("1");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX 테이블 2번, 적용되었습니다");
            InformCurrentMode();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateArray("2");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX 테이블 3번, 적용되었습니다");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateArray("3");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX 테이블 4번, 적용되었습니다");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateInfoBox("중간 암호 용지 비움");
            ClearInterChiperBox();
            InformCurrentMode();
        }
        if(Input.GetKeyDown(KeyCode.M))//M키를 눌러서 복호화 모드, 암호화 모드 전환
        {
            if(CurrentCodemode == Codemode.Encoding)
            {
                CurrentCodemode = Codemode.Decoding;
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
                UpdateInfoBox("모드 전환 : 복호화");
                UpdateModeBox("D", "e");
                ClearInterChiperBox();
            }
            else if(CurrentCodemode == Codemode.Decoding)
            {
                CurrentCodemode = Codemode.Encoding;
                UpdateInfoBox("모드 전환 : 암호화");
                UpdateModeBox("E", "n");
                ClearInterChiperBox();
            }
            InformCurrentMode();
        }

    }

    private void UpdateArray(string ArrayNum)//새로운 ADFGVX표를 로딩해온다
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ADFGVX_Array_Num_" + ArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)
        {
            StreamReader Reader = new StreamReader(FilePath);
            TxtValue = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        char[] Txt = new char[36];
        Txt = TxtValue.ToCharArray();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Array[i, j] = Txt[i * 6 + j];
                Buttons[i * 6 + j].ChangeButtonText(Txt[i * 6 + j]);
            }
        }
    }


    public void OnEncElementDown(int row, int line)//Encoding Mode에서 6x6표의 버튼이 눌렸을 때
    {
        if (CurrentCodemode == Codemode.Encoding)
        {
            char[] ADFGVX = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
            AddInterChiperBox(ADFGVX[row] + "" + ADFGVX[line] + " ");
        }
        else if (CurrentCodemode == Codemode.Decoding)
        {
            UpdateInfoBox("현재 복호화 모드, 모드 재확인 요망");
            InformCurrentMode();
        }
    }
    public void OnDecRowDown(int row)//Decoding Mode에서 row의 버튼이 눌렸을 때
    {
        if (CurrentCodemode == Codemode.Decoding)
        {
            CurrentDecodeElement.row = row;
            if (CurrentDecodeElement.line != 6)
            {
                AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
            }
        }
        else if (CurrentCodemode == Codemode.Encoding)
        {
            UpdateInfoBox("현재 암호화 모드, 모드 재확인 요망");
            InformCurrentMode();
        }
    }
    public void OnDecLineDown(int line)//Decodeing Mode에서 line의 버튼이 눌렸을 때
    {
        if (CurrentCodemode == Codemode.Decoding)
        {
            CurrentDecodeElement.line = line;
            if (CurrentDecodeElement.row != 6)
            {
                AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
            }
        }
        else if (CurrentCodemode == Codemode.Encoding)
        {
            UpdateInfoBox("현재 암호화 모드, 모드 재확인 요망");
            InformCurrentMode();
        }
    }


    private void UpdateModeBox(string Value1, string Value2)
    {
        ModeBox.UpdateText(Value1, Value2);
    }
    private void AddInterChiperBox(string Value)//InterChiperBox의 텍스트에 추가
    {
        InterChiperBox.AddText(Value);
    }
    private void ClearInterChiperBox()//InterChiperBox의 텍스트를 비움
    {
        InterChiperBox.ClearText();
    }
    private void InformCurrentMode()//1초 후에 현재 모드 출력
    {
        if (CurrentCodemode == Codemode.Encoding)
            UpdateInfoBoxDelay(1, "암호화 과정 진행 중...");
        else if (CurrentCodemode == Codemode.Decoding)
            UpdateInfoBoxDelay(1, "복호화 과정 진행 중...");
    }
    private void UpdateInfoBox(string Value)//InfoBox의 텍스트를 Value로 바꾼다
    {
        InfoBox.UpdateText(Value);
    }
    private void UpdateInfoBoxDelay(float Time, string Value)//InfoBox의 텍스트를 Timer초 후에 Value로 바꾼다
    {
        StartCoroutine(UpdateInfoBoxTimer(Time, Value));        
    }
    private IEnumerator UpdateInfoBoxTimer(float Time, string Value)//UpdateInfoBoxDelay 코루틴
    {
        float currenttime = 0.0f;
        while (currenttime < Time)
        {
            yield return new WaitForSeconds(0.01f);
            currenttime += 0.01f;
        }
        UpdateInfoBox(Value);
        yield return null;
    }
}
