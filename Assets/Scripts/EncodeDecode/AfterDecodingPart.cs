using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AfterDecodingPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro m_FilePath;
    private TextMeshPro m_FileTitle;
    private TextMeshPro m_SecurityLevel;
    private TextMeshPro m_ReceptionDateUI;
    private TextMeshPro m_ReceptionDate;
    private TextMeshPro m_ReceiverUI;
    private TextMeshPro m_Receiver;

    private InputField_ADFGVX data;


    private GameObject button_DecodeSave;

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        m_FilePath = transform.GetChild(0).GetComponent<TextMeshPro>();
        m_FileTitle = transform.GetChild(1).GetComponent<TextMeshPro>();
        m_SecurityLevel = transform.GetChild(2).GetComponent<TextMeshPro>();
        m_ReceptionDateUI = transform.GetChild(3).GetComponent<TextMeshPro>();
        m_ReceptionDate = transform.GetChild(4).GetComponent<TextMeshPro>();
        m_ReceiverUI = transform.GetChild(5).GetComponent<TextMeshPro>();
        m_Receiver = transform.GetChild(6).GetComponent<TextMeshPro>();


        data = transform.Find("Data").GetComponent<InputField_ADFGVX>();

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

    public TextMeshPro GetFilePath()
    {
        return m_FilePath;
    }

    public TextMeshPro GetFileTitle()
    {
        return m_FileTitle;
    }

    public TextMeshPro GetSecurityLevel()
    {
        return m_SecurityLevel;
    }

    public TextMeshPro GetReceptionDateUI()
    {
        return m_ReceptionDateUI;
    }

    public TextMeshPro GetReceptionDate()
    {
        return m_ReceptionDate;
    }

    public TextMeshPro GetReceiverUI()
    {
        return m_ReceiverUI;
    }

    public TextMeshPro GetReceiver()
    {
        return m_Receiver;
    }
}
