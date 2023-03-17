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

    [Header("ADFGVX 테이블 원소")]
    public Button_ADFGVX_Element[] elementButtons = new Button_ADFGVX_Element[36];
    [Header("ADFGVX 테이블 행")]
    public Button_ADFGVX_Row[] rowButtons = new Button_ADFGVX_Row[6];
    [Header("ADFGVX 테이블 열")]
    public Button_ADFGVX_Line[] lineButtons = new Button_ADFGVX_Line[6];

    private TextMeshPro rowText;
    private TextMeshPro lineText;
    private TextMeshPro arrayNumText;

    private int currentADFGVXArrayNum;
    private const int ArrayNum_MAX = 4;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        rowText = GetComponentsInChildren<TextMeshPro>()[1];
        lineText = GetComponentsInChildren<TextMeshPro>()[2];
        arrayNumText = GetComponentsInChildren<TextMeshPro>()[3];

        decodeRow = 6;
        decodeLine = 6;

        currentADFGVXArrayNum = 0;
        UpdateADFGVXArray();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        GameObject arrayKeyboard = transform.Find("ArrayKeyboard").gameObject;
        for(int i=0;i<6;i++)
        {
            arrayKeyboard.transform.Find("Line (" + i.ToString() + ")").gameObject.layer = layer;
            arrayKeyboard.transform.Find("Row (" + i.ToString() + ")").gameObject.layer = layer;
        }
        for(int i=0;i<36;i++)
        {
            arrayKeyboard.transform.Find("Element (" + i.ToString() + ")").gameObject.layer = layer;
        }
        transform.Find("Delete").gameObject.layer = layer;
        transform.Find("Clear").gameObject.layer = layer;
        transform.Find("ArrayMinus").gameObject.layer = layer;
        transform.Find("ArrayPlus").gameObject.layer = layer;
    }

    public TextMeshPro GetRowText()//행의 입력값을 반환
    {
        return rowText;
    }

    public TextMeshPro GetLineText()//열의 입력값을 반환
    {
        return lineText;
    }

    public int GetCurrentADFGVXArrayNum()//현재 ADFGVX 테이블 번호 반환
    {
        return currentADFGVXArrayNum;
    }
    
    public void OnEncElementDown(int row, int line)//복호화 모드에서 테이블 원소 클릭
    {
        //튜토리얼 관련 코드
        if(adfgvx.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.biliteralsubstitutionpart.currentADFGVXArrayNum != 0)
            {
                adfgvx.DisplayTutorialDialog(44, 0f);
                return;
            }
            else
                adfgvx.MoveToNextTutorialPhase(2.0f);
        }

        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        adfgvx.beforeEncodingPart.GetInputField_Data().AddInputField(array[row].ToString() + array[line].ToString() + " ");
    }

    public void OnDecRowDown(int row)//복호화 모드에서 행 버튼 클릭
    {
        decodeRow = row;

        for (int i = 0; i < 6; i++)//새롭게 눌린 행 버튼을 제외한 나머지 행 버튼의 선택 상태를 해제한다
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].ConvertClickSpriteColor(rowButtons[i].Exit);
            }
        }

        if (decodeLine != 6)//이미 열 버튼이 선택된 상태, 두 버튼이 전투 선택되었다
        {
            rowButtons[decodeRow].Selected = false;
            lineButtons[decodeLine].Selected = false;
            lineButtons[decodeLine].ConvertClickSpriteColor(lineButtons[decodeLine].Exit);

            //튜토리얼 관련 코드
            if (adfgvx.GetCurrentTutorialPhase() == 8 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.DisplayTutorialDialog(144, 0f);
                decodeRow = 6;
                decodeLine = 6;
                return;
            }
            if (adfgvx.GetCurrentTutorialPhase() == 7 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 1 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(133, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 6 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 2 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(127, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 5 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 5 && decodeLine == 4 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(121, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }

            adfgvx.afterDecodingPart.GetInputField_Data().AddInputField(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            decodeRow = 6;
            decodeLine = 6;
        }
    }

    public void OnDecLineDown(int line)//복호화 모드에서 열 버튼 클릭
    {
        decodeLine = line;

        for (int i = 0; i < 6; i++)//새롭게 눌린 열 버튼을 제외한 나머지 열 버튼의 선택 상태를 해제한다
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].ConvertClickSpriteColor(lineButtons[i].Exit);
            }
        }

        if (decodeRow != 6)//이미 행 버튼이 선택된 상태, 두 버튼이 전투 선택되었다
        {
            lineButtons[decodeLine].Selected = false;
            rowButtons[decodeRow].Selected = false;
            rowButtons[decodeRow].ConvertClickSpriteColor(rowButtons[decodeRow].Exit);

            //튜토리얼 관련 코드
            if (adfgvx.GetCurrentTutorialPhase() == 8 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.DisplayTutorialDialog(144, 0f);
                decodeRow = 6;
                decodeLine = 6;
                return;
            }
            if (adfgvx.GetCurrentTutorialPhase() == 7 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 1 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(133, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 6 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 2 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(127, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 5 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 5 && decodeLine == 4 && currentADFGVXArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentADFGVXArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(121, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }

            adfgvx.afterDecodingPart.GetInputField_Data().AddInputField(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            decodeLine = 6;
            decodeRow = 6;
        }
    }

    private void UpdateADFGVXArray()//currentADFGVXArrayNum에 따라서 ADFGVX 테이블 원소의 텍스트를 변경한다
    {
        string FilePath = "Assets/Resources/Text/Array_" + currentADFGVXArrayNum + ".txt";
        FileInfo TxtFile = new FileInfo(FilePath);
        string value = "";

        if (TxtFile.Exists)
        {
            StreamReader Reader = new StreamReader(FilePath);
            value = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");

        //ADFGVX 테이블 원소들을 순회하면서 텍스트 값을 변경한다
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                elementButtons[i * 6 + j].SetMarkText(value[i * 6 + j].ToString());
            }
        }
    }

    public void ArrayPlus()//ADFGVX 테이블 번호 +1
    {
        if (adfgvx.GetCurrentTutorialPhase()==4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentADFGVXArrayNum++;
        currentADFGVXArrayNum %= ArrayNum_MAX;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentADFGVXArrayNum.ToString();
        UpdateADFGVXArray();
    }

    public void ArrayMinus()//ADFGVX 테이블 번호 -1
    {
        if (adfgvx.GetCurrentTutorialPhase() == 4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentADFGVXArrayNum--;
        if (currentADFGVXArrayNum < 0)
            currentADFGVXArrayNum = ArrayNum_MAX - 1;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentADFGVXArrayNum.ToString();
        UpdateADFGVXArray();
    }
}
