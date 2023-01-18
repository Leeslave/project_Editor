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

    private void Start()
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

    public void SetLayer(int layer)//레이어 변경
    {
        this.gameObject.layer = layer;
        GameObject.Find("ArrayKeyboard").layer = layer;
        GameObject.Find("Delete").layer = layer;
        GameObject.Find("ArrayMinus").layer = layer;
        GameObject.Find("ArrayPlus").layer = layer;
    }

    public TextMeshPro GetRowText()//입력 대기 중인 오 문자
    {
        return rowText;
    }

    public TextMeshPro GetLineText()//입력 대기 중인 열 문자
    {
        return lineText;
    }

    private void UpdateArray(string currentArrayNum)//currentArrayNum에 따라서 새로운 ADFGVX배열을 로딩해서 Array를 업데이트
    {
        string FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ArrayTxt/Array_" + currentArrayNum + ".txt";
        FileInfo TxtFile = new FileInfo(FilePath);
        string value = "";

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
        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        adfgvx.beforeEncodingPart.AddInputField_DataByButton(array[row] + "" + array[line] + " ");
    }

    public void OnDecRowDown(int row)//Decoding Mode에서 row의 버튼이 눌렸을 때
    {
        decodeRow = row;

        for (int i = 0; i < 6; i++)//현재 선택된 RowButton 전에 선택되었던 RowButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].ConvertClickSprite(rowButtons[i].Exit);
            }
        }

        if (decodeLine != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.afterDecodingPart.GetInputField_Data().AddInputFieldByButton(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].ConvertClickSprite(lineButtons[decodeLine].Exit);
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
        decodeLine = line;

        for (int i = 0; i < 6; i++)//현재 선택된 LineButton 전에 선택되었던 LineButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].ConvertClickSprite(lineButtons[i].Exit);
            }
        }
        if (decodeRow != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
        {
            adfgvx.afterDecodingPart.GetInputField_Data().AddInputFieldByButton(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].ConvertClickSprite(rowButtons[decodeRow].Exit);
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
}
