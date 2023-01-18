using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EncodeDataLoadPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro partTitle;                  //파트 타이틀


    private Button_Load button_Load;
    private InputField filePath;

    private TextField securityLevel;
    private TextField title;
    private TextField data;
    private TextField sender;
    private TextField senderUI;
    private TextField date;
    private TextField dateUI;

    private string DecodedChiper = "";           //복호화된 데이터 값

    private void Start()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        partTitle = GetComponentsInChildren<TextMeshPro>()[0];

        button_Load = transform.Find("Load").GetComponent<Button_Load>();
        filePath = transform.Find("FilePath").GetComponent<InputField>();

        securityLevel = transform.Find("SecurityLevel").GetComponent<TextField>();
        title = transform.Find("Title").GetComponent<TextField>();
        data = transform.Find("Data").GetComponent<TextField>();
        sender = transform.Find("Sender").GetComponent<TextField>();
        senderUI = transform.Find("SenderUI").GetComponent<TextField>();
        date = transform.Find("Date").GetComponent<TextField>();
        dateUI = transform.Find("DateUI").GetComponent<TextField>();

    }

    public void SetLayer(int layer)//모든 입력 제어
    {
        this.gameObject.layer = layer;
        filePath.gameObject.layer = layer;
        button_Load.gameObject.layer = layer;
    }

    public void UnvisiblePart()//암호 파트 가시
    {
        this.transform.localPosition = new Vector3(102.3f, -200f, 0);
    }

    public void VisiblePart()//암호 파트 비가시
    {
        this.transform.localPosition = new Vector3(102.3f, -68.2f, 0);
    }

    public InputField GetInputField_filePath()//파일 경로 인풋 필드 반환
    {
        return filePath;
    }

    public TextField GetTextField_SecurityLevel()//보안 등급 인풋 필드 반환
    {
        return securityLevel;
    }

    public void LoadEncodeDataByKeyboard()//inputstring에 따라서 암호문을 불러온다
    {
        if (!filePath.GetIsReadyForInput())
            return;

        if (date.GetIsNowFlowText() || sender.GetIsNowFlowText())//아직 전에 명령받은 파일 불러오기 작업이 끝나지 않음
        {
            adfgvx.InformError("파일 불러오기 불가 : 작업 진행 중");
            return;
        }

        //return 키를 눌렀으니 검색창 선택과 깜박임을 비활성화한다
        filePath.StopFlashInputField();

        string FilePath = "";
        FileInfo TxtFile = null;
        string SecurityLevel = "";
        string Title = "";
        string Data = "";
        string SendingDateUI = "";
        string SendingDate = "";
        string SenderUI = "";
        string Sender = "";
        string ReceptionDateUI = "";
        string ReceptionDate = "";
        string ReceiverUI = "";
        string Receiver = "";

        //ArrayNum에 따라서 각기 다른 표의 FilePath가 저장된다
        FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ChipersTxt/" + filePath.GetInputString() + ".txt";
        TxtFile = new FileInfo(FilePath);

        if (TxtFile.Exists)//Filepath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            SecurityLevel = Reader.ReadLine();
            Title = Reader.ReadLine();
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
        }
        else//Filepath가 유효하지 않다면
        {
            adfgvx.InformError("'" + filePath.GetInputString() + "' " + "엑세스 실패 : 유효하지 않은 경로");
            filePath.DisplayErrorInInputField("파일 접근 불가!");
            Debug.Log("Unexist FilePath Error!");
            return;
        }

        //모든 파트 입력 차단
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2);
        //모든 파트 입력 회복 예약
        adfgvx.SetPartLayerWaitForSec(3f, 0, 0, 0, 0, 0, 0, 0, 0);

        adfgvx.InformUpdate("'" + filePath.GetInputString() + "' " + "엑세스 성공 : 도달 시간 1ms 이하");

        //흐름 출력 시작
        ClearChiperAll();

        securityLevel.SetText(SecurityLevel);
        title.FlowText(Title, 3.0f);
        data.FlowText(Data, 3.0f);
        dateUI.SetText(SendingDateUI);
        date.FlowText(SendingDate, 3.0f);
        senderUI.SetText(SenderUI);
        sender.FlowText(Sender, 3.0f);

        //intermediatechiper에도 흐름 출력 시작
        adfgvx.afterDecodingPart.GetSecurityLevel().SetText(SecurityLevel);
        adfgvx.afterDecodingPart.GetTitle().FlowText(Title, 3.0f);
        adfgvx.afterDecodingPart.GetDateUI().SetText(ReceptionDateUI);
        adfgvx.afterDecodingPart.GetDate().FlowText(ReceptionDate, 3.0f);
        adfgvx.afterDecodingPart.GetSenderUI().SetText(ReceiverUI);
        adfgvx.afterDecodingPart.GetSender().FlowText(Receiver, 3.0f);

        //사운드 재생
        adfgvx.soundFlow(30, 3f);

        //스탑워치 시작
        adfgvx.StartStopWatch();
    }

    public void LoadEncodeDataByButton()
    {
        if (date.GetIsNowFlowText() || sender.GetIsNowFlowText())//아직 전에 명령받은 파일 불러오기 작업이 끝나지 않음
        {
            adfgvx.InformError("파일 불러오기 불가 : 작업 진행 중");
            return;
        }

        //return 키를 눌렀으니 검색창 선택과 깜박임을 비활성화한다
        filePath.StopFlashInputField();

        string FilePath = "";
        FileInfo TxtFile = null;
        string SecurityLevel = "";
        string Title = "";
        string Data = "";
        string SendingDateUI = "";
        string SendingDate = "";
        string SenderUI = "";
        string Sender = "";
        string ReceptionDateUI = "";
        string ReceptionDate = "";
        string ReceiverUI = "";
        string Receiver = "";

        //ArrayNum에 따라서 각기 다른 표의 FilePath가 저장된다
        FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ChipersTxt/" + filePath.GetInputString() + ".txt";
        TxtFile = new FileInfo(FilePath);

        if (TxtFile.Exists)//Filepath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            SecurityLevel = Reader.ReadLine();
            Title = Reader.ReadLine();
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
        }
        else//Filepath가 유효하지 않다면
        {
            adfgvx.InformError("'" + filePath.GetInputString() + "' " + "엑세스 실패 : 유효하지 않은 경로");
            filePath.DisplayErrorInInputField("파일 접근 불가!");
            Debug.Log("Unexist FilePath Error!");
            return;
        }

        //모든 파트 입력 차단
        adfgvx.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2);
        //모든 파트 입력 회복 예약
        adfgvx.SetPartLayerWaitForSec(3f, 0, 0, 0, 0, 0, 0, 0, 0);

        adfgvx.InformUpdate("'" + filePath.GetInputString() + "' " + "엑세스 성공 : 도달 시간 1ms 이하");

        //흐름 출력 시작
        ClearChiperAll();

        securityLevel.SetText(SecurityLevel);
        title.FlowText(Title, 3.0f);
        data.FlowText(Data, 3.0f);
        dateUI.SetText(SendingDateUI);
        date.FlowText(SendingDate, 3.0f);
        senderUI.SetText(SenderUI);
        sender.FlowText(Sender, 3.0f);

        //intermediatechiper에도 흐름 출력 시작
        adfgvx.afterDecodingPart.GetSecurityLevel().SetText(SecurityLevel);
        adfgvx.afterDecodingPart.GetTitle().FlowText(Title, 3.0f);
        adfgvx.afterDecodingPart.GetDateUI().SetText(ReceptionDateUI);
        adfgvx.afterDecodingPart.GetDate().FlowText(ReceptionDate, 3.0f);
        adfgvx.afterDecodingPart.GetSenderUI().SetText(ReceiverUI);
        adfgvx.afterDecodingPart.GetSender().FlowText(Receiver, 3.0f);

        //사운드 재생
        adfgvx.soundFlow(30, 3f);

        //스탑워치 시작
        adfgvx.StartStopWatch();

        //모든 파트 입력 회복
    }

    public void ClearChiperAll()//불러와져 있던 암호문을 비운다
    {
        securityLevel.SetText("");
        title.SetText("");
        data.SetText("");
        dateUI.SetText("");
        date.SetText("");
        senderUI.SetText("");
        sender.SetText("");
    }

    public string GetData()//불러와있던 암호문을 반환한다
    {
        return data.GetText();
    }

    public string GetDecodedChiper()//복호화된 암호문을 반환한다
    {
        return DecodedChiper;
    }
}
