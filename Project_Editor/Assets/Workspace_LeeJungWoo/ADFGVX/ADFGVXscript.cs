using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVXscript : MonoBehaviour
{
    private char[,] Array = new char[6, 6];
    private int ArrayNum = 0;

    public enum Codemode//ADFGVX모드
    { Encoding,Decoding};
    public Codemode CurrentCodemode;//현재 ADFGVX모드

    public struct DecodeElemnt//복호화 엘레멘트
    {
        public int row;
        public int line;
    }
    public DecodeElemnt CurrentDecodeElement;//현재 복호화 엘레멘트


    public ElementButton[] ElementButtons = new ElementButton[36];
    public RowButton[] RowButtons = new RowButton[6];
    public LineButton[] LineButtons = new LineButton[6];


    public Mode ModeBox;
    public InterChiper InterChiperBox;
    public Info InfoBox;
    public ArrayInfo ArrayInfoBox;
    public ArraySelect ArraySelectBox;
   

    private void Start()
    {
        ArrayNum = 0;
        UpdateArray(ArrayNum.ToString());
        CurrentDecodeElement.row = 6;
        CurrentDecodeElement.line = 6;
        InterChiperBox.ClearText();
        UpdateInfoBox("ADFGVX 테이블을 선택하십시오.");
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {

        }
    }
    
    private void UpdateArray(string ArrayNum)//ArrayNum에 따라서 새로운 ADFGVX배열을 로딩해서 Array를 업데이트
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";

        
        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ADFGVXArrayTxt/Array_" + ArrayNum + ".txt";              //ArrayNum에 따라서 각기 다른 표의 FilePath가 저장됩니다
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)                                                                                 //FilePath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath);
            TxtValue = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        
        
        char[] Txt = new char[36];                                                                          
        Txt = TxtValue.ToCharArray();                                                                       //36크기의 문자형 배열에 String형의 ADFGVX표를 전환해서 넣습니다
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Array[i, j] = Txt[i * 6 + j];
                ElementButtons[i * 6 + j].ChangeButtonText(Txt[i * 6 + j]);                                 //모든 public ElementButtons에 접근하면서 버튼의 텍스트를 ADFGVX표의 값대로 변경합니다
            }
        }
    }

    public void OnModeDown()//모드 전환 버튼이 눌렸을 때
    {
        if (CurrentCodemode == Codemode.Encoding)
        {
            CurrentCodemode = Codemode.Decoding;
            UpdateInfoBox("모드 전환 : 복호화");
            UpdateModeBox("D", "e");
            ClearInterChiperBox();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ElementButtons[i * 6 + j].ShiftMode();
                }
            }
        }
        else if (CurrentCodemode == Codemode.Decoding)
        {
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].DisableClickSprite();
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].DisableClickSprite();
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
            CurrentCodemode = Codemode.Encoding;
            UpdateInfoBox("모드 전환 : 암호화");
            UpdateModeBox("E", "n");
            ClearInterChiperBox();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ElementButtons[i * 6 + j].ShiftMode();
                }
            }
        }
        InformCurrentMode();
    }

    public void OnArraySelectDown()//배열 버튼이 눌렸을 때
    {
        ArrayNum++;
        ArrayNum%=4;
        UpdateArray(ArrayNum.ToString());
        ArrayInfoBox.UpdateText("암호배열\n* No." + ArrayNum.ToString() + " *");
    }

    public void OnClearDown()//Clear 버튼이 눌렸을 때
    {
        UpdateInfoBox("중간 암호 용지 비움");
        ClearInterChiperBox();
        InformCurrentMode();
    }

    public void OnDeleteDown()//Delete 버튼이 눌렸을 때
    {
        DeleteInterChiperBox();
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
        CurrentDecodeElement.row = row;

        for (int i = 0; i < 6; i++)                                                                     //현재 선택된 RowButton 전에 선택되었던 RowButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (RowButtons[i].Selected == true && i != row)
            {
                RowButtons[i].Selected = false;
                RowButtons[i].DisableClickSprite();
            }
        }

        if (CurrentDecodeElement.line != 6)                                                             //RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].DisableClickSprite();
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode에서 line의 버튼이 눌렸을 때
    {
        CurrentDecodeElement.line = line;

        for (int i = 0; i < 6; i++)                                                                     //현재 선택된 LineButton 전에 선택되었던 LineButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (LineButtons[i].Selected == true && i != line)
            {
                LineButtons[i].Selected = false;
                LineButtons[i].DisableClickSprite();
            }
        }

        if (CurrentDecodeElement.row != 6)                                                              //RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].DisableClickSprite();
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
        }
    }


    private void UpdateModeBox(string Value1, string Value2)//ModeBox의 텍스트를 En 혹은 De로 바꿀 때 사용한다
    {
        ModeBox.UpdateText(Value1, Value2);
    }

    public void UpdateInfoBox(string Value)//InfoBox의 텍스트를 Value로 바꾼다
    {
        InfoBox.UpdateText(Value);
    }
    
    private void ClearInterChiperBox()//InterChipderBox를 비운다
    {
        InterChiperBox.ClearText();
    }
    
    private void AddInterChiperBox(string Value)//InterChiperBox의 텍스트에 추가
    {
        InterChiperBox.AddText(Value);
    }

    private void DeleteInterChiperBox()//InterChiperBox의 텍스트 하나 삭제
    {
        InterChiperBox.DeleteText();
    }
    
    public void InformCurrentMode()//1초 후에 현재 모드 출력
    {
        if (CurrentCodemode == Codemode.Encoding)
            UpdateInfoBoxDelay(1, "암호화 과정 진행 중...");
        else if (CurrentCodemode == Codemode.Decoding)
            UpdateInfoBoxDelay(1, "복호화 과정 진행 중...");
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
