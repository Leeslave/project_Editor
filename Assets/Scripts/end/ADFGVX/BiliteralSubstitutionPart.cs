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

    [Header("ADFGVX 엘레멘트 버튼")]
    public Button_ADFGVX_Element[] elementButtons = new Button_ADFGVX_Element[36];
    [Header("ADFGVX 행 버튼")]
    public Button_ADFGVX_Row[] rowButtons = new Button_ADFGVX_Row[6];
    [Header("ADFGVX 열 버튼")]
    public Button_ADFGVX_Line[] lineButtons = new Button_ADFGVX_Line[6];

    private TextMeshPro rowText;
    private TextMeshPro lineText;
    private TextMeshPro arrayNumText;

    private int currentArrayNum;
    private const int ArrayNum_MAX = 4;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        rowText = GetComponentsInChildren<TextMeshPro>()[1];
        lineText = GetComponentsInChildren<TextMeshPro>()[2];
        arrayNumText = GetComponentsInChildren<TextMeshPro>()[3];

        decodeRow = 6;
        decodeLine = 6;

        currentArrayNum = 0;
        UpdateADFGVXArray();
    }

    public void SetLayer(int layer)//이 게임오브젝트 하위 요소의 레이어 제어
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

    public TextMeshPro GetRowText()//입력 대기 중인 오 문자
    {
        return rowText;
    }

    public TextMeshPro GetLineText()//입력 대기 중인 열 문자
    {
        return lineText;
    }

    public int GetCurrentArrayNum()//현재 ADFGVX 배열 번호 반환
    {
        return currentArrayNum;
    }
    
    public void OnEncElementDown(int row, int line)//Encoding Mode에서 6x6표의 버튼이 눌렸을 때
    {
        //튜토리얼 관련 코드
        if(adfgvx.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.biliteralsubstitutionpart.currentArrayNum != 0)
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

    public void OnDecRowDown(int row)//Decoding Mode에서 row의 버튼이 눌렸을 때
    {
        decodeRow = row;

        for (int i = 0; i < 6; i++)//현재 선택된 RowButton 전에 선택되었던 RowButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].ConvertClickSpriteColor(rowButtons[i].Exit);
            }
        }

        if (decodeLine != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
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
                if (decodeRow == 0 && decodeLine == 1 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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
                if (decodeRow == 0 && decodeLine == 2 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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
                if (decodeRow == 5 && decodeLine == 4 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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

    public void OnDecLineDown(int line)//Decodeing Mode에서 line의 버튼이 눌렸을 때
    {
        decodeLine = line;

        for (int i = 0; i < 6; i++)//현재 선택된 LineButton 전에 선택되었던 LineButton을 찾아서 ClickSprite를 비활성화합니다
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].ConvertClickSpriteColor(lineButtons[i].Exit);
            }
        }

        if (decodeRow != 6)//RowButton과 LineButton 둘 다 선택 되었으므로, Decoding 과정을 진행합니다
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
                if (decodeRow == 0 && decodeLine == 1 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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
                if (decodeRow == 0 && decodeLine == 2 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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
                if (decodeRow == 5 && decodeLine == 4 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
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

    private void UpdateADFGVXArray()//currentArrayNum에 따라서 새로운 ADFGVX 배열을 로딩해서 Array를 업데이트
    {
        string FilePath = "Assets/Resources/Text/Array_" + currentArrayNum + ".txt";
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

    public void ArrayPlus()//ADFGVX배열 +1로 전환
    {
        //튜토리얼 관련 코드
        if (adfgvx.GetCurrentTutorialPhase()==4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentArrayNum++;
        currentArrayNum %= ArrayNum_MAX;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateADFGVXArray();
    }

    public void ArrayMinus()//ADFGVX배열 -1로 전환
    {
        //튜토리얼 관련 코드
        if (adfgvx.GetCurrentTutorialPhase() == 4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentArrayNum--;
        if (currentArrayNum < 0)
            currentArrayNum = ArrayNum_MAX - 1;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateADFGVXArray();
    }
}
