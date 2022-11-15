using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class BiliteralSubstitutionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private int decoderow;
    private int decodeline;

    public ElementButton[] elementbuttons = new ElementButton[36];
    public RowButton[] rowbuttons = new RowButton[6];
    public LineButton[] linebuttons = new LineButton[6];
    private TextMeshPro rowtext;
    private TextMeshPro linetext;

    private int arraynum;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        rowtext = GetComponentsInChildren<TextMeshPro>()[1];
        linetext = GetComponentsInChildren<TextMeshPro>()[2];

        decoderow = 6;
        decodeline = 6;

        arraynum = 0;
        UpdateArray("0");
    }

    private void Update()
    {

    }

    private void UpdateArray(string ArrayNum)//ArrayNum에 따라서 새로운 ADFGVX배열을 로딩해서 Array를 업데이트
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string value = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ArrayTxt/Array_" + ArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)//FilePath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath);
            value = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");

        //모든 public ElementButtons에 접근하면서 버튼의 텍스트를 ADFGVX표의 값대로 변경합니다
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                elementbuttons[i * 6 + j].ChangeButtonText(value[i * 6 + j]);                              
            }
        }
    }
    
    public void OnEncElementDown(int row, int line)//Encoding Mode에서 6x6표의 버튼이 눌렸을 때
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
            adfgvx.intermediatepart.AddIntermediateChiper(array[row] + "" + array[line] + " ");
        }
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.UpdateInfoBox("현재 복호화 모드, 모드 재확인 요망");
            adfgvx.InformCurrentMode();
        }
    }

    public void OnDecRowDown(int row)//Decoding Mode에서 row의 버튼이 눌렸을 때
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("현재 암호화 모드, 모드 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }

        decoderow = row;

        for (int i = 0; i < 6; i++)//현재 선택된 RowButton 전에 선택되었던 RowButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (rowbuttons[i].Selected == true && i != row)
            {
                rowbuttons[i].Selected = false;
                rowbuttons[i].DisableClickSprite();
            }
        }

        if (decodeline != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementbuttons[decoderow * 6 + decodeline].GetButtonText() + " ");
            if (decodeline != 6)
            {
                linebuttons[decodeline].DisableClickSprite();
                linebuttons[decodeline].Selected = false;
                decodeline = 6;
            }
            if (decoderow != 6)
            {
                rowbuttons[decoderow].Selected = false;
                decoderow = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode에서 line의 버튼이 눌렸을 때
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("현재 암호화 모드, 모드 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }

        decodeline = line;

        for (int i = 0; i < 6; i++)//현재 선택된 LineButton 전에 선택되었던 LineButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (linebuttons[i].Selected == true && i != line)
            {
                linebuttons[i].Selected = false;
                linebuttons[i].DisableClickSprite();
            }
        }
        if (decoderow != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementbuttons[decoderow * 6 + decodeline].GetButtonText() + " ");
            if (decodeline != 6)
            {
                linebuttons[decodeline].Selected = false;
                decodeline = 6;
            }
            if (decoderow != 6)
            {
                rowbuttons[decoderow].DisableClickSprite();
                rowbuttons[decoderow].Selected = false;
                decoderow = 6;
            }
        }
    }

    public void ShiftArray()//ADFGVX배열 전환
    {
        arraynum++;
        arraynum %= 4;
        UpdateArray(arraynum.ToString());
    }

    public void ResponseUseDictionaryADFGVX(int idx , string value)//키보드 직접 입력에 대응
    {
        if (!adfgvx.intermediatepart.isreadyforinput)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            int row = 0;
            int line = 0;
            for (int i=0;i<36;i++)
            {
                if(elementbuttons[i].GetButtonText() == value)
                {
                    row = i / 6;
                    line = i % 6;
                }
            }
            adfgvx.intermediatepart.AddIntermediateChiper(array[row].ToString() + array[line].ToString() + " ");
        }
        else if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            rowtext.text = array[decoderow].ToString();
            linetext.text = array[decodeline].ToString();

            if(value == "-")
            {
                rowtext.text = "-";
                decoderow = 6;
                linetext.text = "-";
                decodeline = 6;
            }
            else if (decoderow == 6)
            {
                rowtext.text = array[idx].ToString();
                decoderow = idx;
            }
            else if (decodeline == 6)
            {
                linetext.text = array[idx].ToString();

                decodeline = idx;
            }
            else if (decoderow != 6 && decodeline != 6)
            {
                rowtext.text = array[idx].ToString();
                decoderow = idx;
                linetext.text = array[6].ToString();
                decodeline = 6;
            }
        }
    }

    public void ResponseUseDictionaryEnter()//키보드 엔터 입력에 대응
    {
        if (decoderow == 6 || decodeline == 6)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            return;
        OnDecLineDown(decodeline);
    }

    public void InitializeText()//모드 전환에 따른 이중 문자 치환 파트의 초기화
    {
        rowtext.text = "-";
        decoderow = 6;
        linetext.text = "-";
        decodeline = 6;
    }

   
}
