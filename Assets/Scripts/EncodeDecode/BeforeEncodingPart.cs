using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeEncodingPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX data;
    private TextField block;
    private TextField primeFactor;
    private Button_ADFGVX_Retranspose reTranspose;

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        data = transform.GetChild(2).GetComponent<InputField_ADFGVX>();
        block = transform.GetChild(3).GetComponent<TextField>();
        primeFactor = transform.GetChild(4).GetComponent<TextField>();
        reTranspose = transform.GetChild(5).GetComponent<Button_ADFGVX_Retranspose>();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        data.gameObject.layer = layer;
        reTranspose.gameObject.layer = layer;
        if(layer==2)
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
        //튜토리얼 관련 코드
        if(adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 1 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.biliteralsubstitutionpart.GetCurrentADFGVXArrayNum() != 0)
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(44, 0f);
                return;
            }
            else
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(2.0f);
        }

        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        int idx_row = 6;
        int idx_line = 6;
        for (int idx = 0; idx < 36; idx++)
        {
            if (adfgvx.biliteralsubstitutionpart.elementButtons[idx].GetButtonText() == value)
            {
                idx_row = idx / 6;
                idx_line = idx % 6;
            }
        }

        if(idx_row == 6 && idx_line == 6)//입력값과 일치하는 테이블 원소가 없다
        {
            adfgvx.InformError("나오면 안되는 오류!?");
            return;
        }

        data.AddInputField(array[idx_row].ToString() + array[idx_line].ToString() + " ");
        UpdateRecommendKeyword();
    }

    public void DeleteInputField_Data()//데이터 입력창에서 삭제
    {        
        data.DeleteInputField(3);
        UpdateRecommendKeyword();
    }

    private void UpdateRecommendKeyword()//추천 전치 키 글자 수 업데이트
    {
        string number = ("오리지널 데이터의 글자 수 : " + data.GetMarkText().Length / 3 * 2).ToString();

        string prime = "추천하는 전치 키의 글자 수 : ";

        if (data.GetMarkText().Length / 3 * 2 == 0)
            prime += "NULL";
        else
        {
            prime += "1";
            int max = ((data.GetMarkText().Length / 3 * 2) < 9) ? (data.GetMarkText().Length / 3 * 2) : 9;
            for (int i = 2; i <= max; i++)
            {
                if ((data.GetMarkText().Length / 3 * 2) % i == 0)
                    prime += ", " + i.ToString();
            }
        }

        block.SetText(number);
        primeFactor.SetText(prime);
    }
}
