using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EncodeDataLoadPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro m_SecurityLevel;
    private TextMeshPro m_FileTitle;
    private TextMeshPro m_EncodeData;
    private TextMeshPro m_Sender;
    private TextMeshPro m_SenderUI;
    private TextMeshPro m_SendingDate;
    private TextMeshPro m_SendingDateUI;
    private InputField_ADFGVX m_FilePath;
    private Button_ADFGVX_Load load;

    private string DecodedChiper = "";  //복호화 정답

    private void Awake()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        m_SecurityLevel = transform.GetChild(0).GetComponent<TextMeshPro>();
        m_FileTitle = transform.GetChild(1).GetComponent<TextMeshPro>();
        m_EncodeData = transform.GetChild(2).GetComponent<TextMeshPro>();
        m_SendingDateUI = transform.GetChild(3).GetComponent<TextMeshPro>();
        m_SendingDate = transform.GetChild(4).GetComponent<TextMeshPro>();
        m_SenderUI = transform.GetChild(5).GetComponent<TextMeshPro>();
        m_Sender = transform.GetChild(6).GetComponent<TextMeshPro>();
        m_FilePath = transform.GetChild(17).GetComponent<InputField_ADFGVX>();
        load = transform.GetChild(18).GetComponent<Button_ADFGVX_Load>();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        m_FilePath.gameObject.layer = layer;
        load.gameObject.layer = layer;
        if(layer == 2)
        {
            m_FilePath.SetIsReadyForInput(false);
            m_FilePath.SetIsFlash(false);
        }
    }

    public void LoadEncodeData()//암호화된 데이터를 로드
    {
        if(adfgvx.GetSTRConverter().GetIsPrintingTMP(m_SendingDate) || adfgvx.GetSTRConverter().GetIsPrintingTMP(m_Sender))
        {
            adfgvx.InformError("현재 암호화 데이터 로드 중 : 종료 시까지 대기 요망");
            return;
        }

        //깜박임 종료
        m_FilePath.StopFlashInputField();
        

        string FilePath = "";
        FileInfo TxtFile = null;
        string SecurityLevel = "";
        string FileTitle = "";
        string Data = "";
        string SendingDateUI = "";
        string SendingDate = "";
        string SenderUI = "";
        string Sender = "";
        string ReceptionDateUI = "";
        string ReceptionDate = "";
        string ReceiverUI = "";
        string Receiver = "";

        FilePath = "Assets/Resources/Text/EncodeDecode/Key/" + m_FilePath.GetInputString() + ".txt";
        TxtFile = new FileInfo(FilePath);

        if (!TxtFile.Exists)
        {
            //튜토리얼 관련 코드
            if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(11, 0f);
            }

            adfgvx.InformError("'" + m_FilePath.GetInputString() + "' " + "로드 실패 : 유효하지 않은 파일 경로");
            m_FilePath.DisplayErrorInInputField("파일 경로 오류!");
            return;
        }

        //튜토리얼 관련 코드
        if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (m_FilePath.GetInputString() == "SI-XI-I")
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(3f);
            else
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(14, 3.2f);
        }
            
        StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
        SecurityLevel = Reader.ReadLine();
        FileTitle = Reader.ReadLine();
        Data = Reader.ReadLine();
        SendingDateUI = Reader.ReadLine();
        SendingDate = Reader.ReadLine();
        SenderUI = Reader.ReadLine();
        Sender = Reader.ReadLine();
        ReceptionDateUI = Reader.ReadLine();
        ReceptionDate = Reader.ReadLine();
        ReceiverUI = Reader.ReadLine();
        Receiver = Reader.ReadLine();
        DecodedChiper = Reader.ReadLine();
        Reader.Close();

        //입력 차단
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //입력 회복
        if(!adfgvx.chat_ADFGVX.PlayAsTutorial)
            adfgvx.SetPartLayerWaitForSec(3f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        adfgvx.InformUpdate("'" + m_FilePath.GetInputString() + "' " + "로드 성공 : 총 작업 시간 1ms 이하");

        //암호화 데이터 로드 파트 업데이트
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, SecurityLevel, m_SecurityLevel);
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, FileTitle, m_FileTitle);
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, Data, m_EncodeData);
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, SendingDateUI, m_SendingDateUI);
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, SendingDate, m_SendingDate);
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, SenderUI, m_SenderUI);
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, Sender, m_Sender);

        //복호화 후 파트 업데이트
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, SecurityLevel, adfgvx.afterDecodingPart.GetSecurityLevel());
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, FileTitle, adfgvx.afterDecodingPart.GetFileTitle());
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, ReceptionDateUI, adfgvx.afterDecodingPart.GetReceptionDateUI());
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, ReceptionDate, adfgvx.afterDecodingPart.GetReceptionDate());
        adfgvx.GetSTRConverter().PrintTMPByDuration(0f, ReceiverUI, adfgvx.afterDecodingPart.GetReceiverUI());
        adfgvx.GetSTRConverter().PrintTMPByDuration(3.0f, Receiver, adfgvx.afterDecodingPart.GetReceiver());

        //오디오 재생
        adfgvx.SoundFlow(30, 3f);

        //시간 측정 개시
        adfgvx.StartStopWatch();
    }

    public TextMeshPro GetEncodeData()
    {
        return m_EncodeData;
    }

    public TextMeshPro GetSecurityLevel()
    {
        return m_SecurityLevel;
    }

    public InputField_ADFGVX GetInputField_FilePath()
    {
        return m_FilePath;
    }

    public string GetDecodedChiper()//복호화 정답 반환
    {
        return DecodedChiper;
    }
}
