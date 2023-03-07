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

    public void SetLayer(int layer)//이 게임오브젝트 하위 요소의 레이어 제어
    {
        transform.Find("DecodeSave").gameObject.layer = layer;
        transform.Find("Data").gameObject.layer = layer;
    }

    public void UnvisiblePart()//파트 비가시 모드
    {
        this.transform.localPosition = new Vector3(66.9f, 150, 0);
    }

    public void VisiblePart()//파트 가시 모드
    {
        this.transform.localPosition = new Vector3(66.9f, 14.6f, 0);
    }

    public InputField_ADFGVX GetInputField_Data()//데이터 인풋 필드 반환
    {
        return data;
    }

    public void AddInputField(string value)//키보드 입력
    {
        if (value.ToCharArray()[0] >= '0' && value.ToCharArray()[0] <= '9')
        {
            adfgvx.InformError("유효하지 않은 입력 : 입력 불가");
            return;
        }

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
            adfgvx.InformError("ADFGVX 원소를 키보드로 입력하십시오");
            return;
        }
    }

    public void ReturnInputField()//키보드 엔터
    {
        TextMeshPro row = adfgvx.biliteralsubstitutionpart.GetRowText();
        TextMeshPro line = adfgvx.biliteralsubstitutionpart.GetLineText();

        //튜토리얼 관련 코드
        if(adfgvx.GetCurrentTutorialPhase() == 8)
        {
            if (row.text == "F" && line.text == "G" && adfgvx.biliteralsubstitutionpart.GetCurrentArrayNum() == 0)
                adfgvx.MoveToNextTutorialPhase(2.0f);
            else if(adfgvx.biliteralsubstitutionpart.GetCurrentArrayNum() != 0)
            {
                adfgvx.DisplayTutorialDialog(100, 0f);
                row.text = "-";
                line.text = "-";
                return;
            }
            else
            {
                adfgvx.DisplayTutorialDialog(94, 0f);
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
