using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class BiliteralSubstitutionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private int decodeRow;
    private int decodeLine;

    public ElementButton[] elementButtons = new ElementButton[36];
    public RowButton[] rowButtons = new RowButton[6];
    public LineButton[] lineButtons = new LineButton[6];
    private TextMeshPro rowText;
    private TextMeshPro lineText;
    private TextMeshPro arrayNumText;

    private int currentArrayNum;
    private const int ArrayNum_MAX = 4;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        rowText = GetComponentsInChildren<TextMeshPro>()[1];
        lineText = GetComponentsInChildren<TextMeshPro>()[2];
        arrayNumText = GetComponentsInChildren<TextMeshPro>()[3];

        decodeRow = 6;
        decodeLine = 6;

        currentArrayNum = 0;
        UpdateArray(currentArrayNum.ToString());
    }

    private void UpdateArray(string currentArrayNum)//currentArrayNum에 따라서 새로운 ADFGVX배열을 로딩해서 Array를 업데이트
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string value = "";
        FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ArrayTxt/Array_" + currentArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)//FilePath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath);
            value = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");

        //모든 public elementButtons에 접근하면서 버튼의 텍스트를 ADFGVX표의 값대로 변경합니다
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                elementButtons[i * 6 + j].ChangeButtonText(value[i * 6 + j]);                              
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
            adfgvx.UpdateInfoBox("버튼 사용 불가 : 현재 모드 재확인 요망");
        }
    }

    public void OnDecRowDown(int row)//Decoding Mode에서 row의 버튼이 눌렸을 때
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("버튼 사용 불가 : 현재 모드 재확인 요망");
            return;
        }

        decodeRow = row;

        for (int i = 0; i < 6; i++)//현재 선택된 RowButton 전에 선택되었던 RowButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].DisableClickSprite();
            }
        }

        if (decodeLine != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].DisableClickSprite();
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].Selected = false;
                decodeRow = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode에서 line의 버튼이 눌렸을 때
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("버튼 사용 불가 : 현재 모드 재확인 요망");
            return;
        }

        decodeLine = line;

        for (int i = 0; i < 6; i++)//현재 선택된 LineButton 전에 선택되었던 LineButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].DisableClickSprite();
            }
        }
        if (decodeRow != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].DisableClickSprite();
                rowButtons[decodeRow].Selected = false;
                decodeRow = 6;
            }
        }
    }

    public void ArrayPlus()//ADFGVX배열 +1로 전환
    {
        currentArrayNum++;
        currentArrayNum %= ArrayNum_MAX;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateArray(currentArrayNum.ToString());
    }

    public void ArrayMinus()//ADFGVX배열 -1로 전환
    {
        currentArrayNum--;
        if (currentArrayNum < 0)
            currentArrayNum = ArrayNum_MAX - 1;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateArray(currentArrayNum.ToString());
    }

    public void ResponseUseDictionaryADFGVX(int idx , string value)//키보드 직접 입력에 대응
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            int row = 0;
            int line = 0;
            for (int i=0;i<36;i++)
            {
                if(elementButtons[i].GetButtonText() == value)
                {
                    row = i / 6;
                    line = i % 6;
                }
            }
            adfgvx.intermediatepart.AddIntermediateChiper(array[row].ToString() + array[line].ToString() + " ");
        }
        else if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            if (!adfgvx.intermediatepart.isReadyForInput)
                return;

            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            rowText.text = array[decodeRow].ToString();
            lineText.text = array[decodeLine].ToString();

            if(value == "-")
            {
                rowText.text = "-";
                decodeRow = 6;
                lineText.text = "-";
                decodeLine = 6;
            }
            else if (decodeRow == 6)
            {
                rowText.text = array[idx].ToString();
                decodeRow = idx;
            }
            else if (decodeLine == 6)
            {
                lineText.text = array[idx].ToString();
                decodeLine = idx;
            }
            else if (decodeRow != 6 && decodeLine != 6)
            {
                rowText.text = array[idx].ToString();
                decodeRow = idx;
                lineText.text = array[6].ToString();
                decodeLine = 6;
            }
        }
    }

    public void ResponseUseDictionaryEnter()//키보드 엔터 입력에 대응
    {
        if (decodeRow == 6 || decodeLine == 6)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            return;
        OnDecLineDown(decodeLine);
    }

    public void InitializeText()//모드 전환에 따른 이중 문자 치환 파트의 초기화
    {
        rowText.text = "-";
        decodeRow = 6;
        lineText.text = "-";
        decodeLine = 6;
    }

   
}
