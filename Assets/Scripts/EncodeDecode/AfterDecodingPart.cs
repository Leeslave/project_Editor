using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AfterDecodingPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextField title;
    private InputField_ADFGVX data;
    private TextField securityLevel;
    private TextField dateUI;
    private TextField date;
    private TextField senderUI;
    private TextField sender;
    private GameObject button_DecodeSave;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        title = transform.Find("Title").GetComponent<TextField>();
        data = transform.Find("Data").GetComponent<InputField_ADFGVX>();
        securityLevel = transform.Find("SecurityLevel").GetComponent<TextField>();
        dateUI = transform.Find("DateUI").GetComponent<TextField>();
        date = transform.Find("Date").GetComponent<TextField>();
        senderUI = transform.Find("SenderUI").GetComponent<TextField>();
        sender = transform.Find("Sender").GetComponent<TextField>();
        button_DecodeSave = GameObject.Find("DecodeSave");
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        button_DecodeSave.gameObject.layer = layer;
        data.gameObject.layer = layer;
        if(layer == 2)
        {
            data.SetIsReadyForInput(false);
            data.SetIsFlash(false);
        }
    }
    
    public InputField_ADFGVX GetInputField_Data()//데이터 입력창 반환
    {
        return data;
    }

    public void AddInputField_Data(string value)//데이터 입력창에 value 추가
    {
        TextMeshPro row = adfgvx.biliteralsubstitutionpart.GetRowText();
        TextMeshPro line = adfgvx.biliteralsubstitutionpart.GetLineText();

        if (value == "A" || value == "D" || value == "F" || value == "G" || value == "V" || value == "X")
        {
            if (row.text == "-")
                row.text = value;
            else if (line.text == "-")
                line.text = value;
            else if (row.text != "-" && line.text != "-")
            {
                row.text = value;
                line.text = "-";
            }
        }
        else
        {
            adfgvx.InformError("ADFGVX 테이블에 해당하는 원소 입력 요망");
            return;
        }
    }

    public void ReturnInputField()//데이터 입력창의 값을 토대로 변환
    {
        TextMeshPro row = adfgvx.biliteralsubstitutionpart.GetRowText();
        TextMeshPro line = adfgvx.biliteralsubstitutionpart.GetLineText();

        //튜토리얼 관련 코드
        if(adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 8 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (row.text == "F" && line.text == "G" && adfgvx.biliteralsubstitutionpart.GetCurrentADFGVXArrayNum() == 0)
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(2.0f);
            else if(adfgvx.biliteralsubstitutionpart.GetCurrentADFGVXArrayNum() != 0)
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(175, 0f);
                row.text = "-";
                line.text = "-";
                return;
            }
            else
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(141, 0f);
                row.text = "-";
                line.text = "-";
                return;
            }
        }

        if (row.text == "-" || line.text == "-")
            return;

        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        int idx_row;
        int idx_line;
        for (idx_row = 0; idx_row < array.Length; idx_row++)
        {
            if (row.text == array[idx_row].ToString())
                break;
        }
        for (idx_line = 0; idx_line < array.Length; idx_line++)
        {
            if (line.text == array[idx_line].ToString())
                break;
        }
        if(adfgvx.afterDecodingPart.GetInputField_Data().GetIsReadyForInput())
            adfgvx.afterDecodingPart.GetInputField_Data().AddInputField(adfgvx.biliteralsubstitutionpart.elementButtons[idx_row * 6 + idx_line].GetButtonText() + " ");
        row.text = "-";
        line.text = "-";
    }

    public TextField GetSecurityLevel()
    {
        return securityLevel;
    }

    public TextField GetTitle()
    {
        return title;
    }

    public TextField GetDateUI()
    {
        return dateUI;
    }

    public TextField GetDate()
    {
        return date;
    }

    public TextField GetSenderUI()
    {
        return senderUI;
    }

    public TextField GetSender()
    {
        return sender;
    }
}
