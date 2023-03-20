using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EncodeDataLoadPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro partTitle;                  //��Ʈ Ÿ��Ʋ

    private InputField_ADFGVX filePath;

    private TextField securityLevel;
    private TextField title;
    private TextField data;
    private TextField sender;
    private TextField senderUI;
    private TextField date;
    private TextField dateUI;

    private string DecodedChiper = "";           //��ȣȭ�� ������ ��

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        partTitle = GetComponentsInChildren<TextMeshPro>()[0];

        filePath = transform.Find("FilePath").GetComponent<InputField_ADFGVX>();

        securityLevel = transform.Find("SecurityLevel").GetComponent<TextField>();
        title = transform.Find("Title").GetComponent<TextField>();
        data = transform.Find("Data").GetComponent<TextField>();
        sender = transform.Find("Sender").GetComponent<TextField>();
        senderUI = transform.Find("SenderUI").GetComponent<TextField>();
        date = transform.Find("Date").GetComponent<TextField>();
        dateUI = transform.Find("DateUI").GetComponent<TextField>();
    }

    public void SetLayer(int layer)//�� ���ӿ�����Ʈ ���� ����� ���̾� ����
    {
        transform.Find("FilePath").gameObject.layer = layer;
        transform.Find("Load").gameObject.layer = layer;
    }

    public void UnvisiblePart()//��ȣ ��Ʈ ����
    {
        this.transform.localPosition = new Vector3(102.3f, -200f, 0);
    }

    public void VisiblePart()//��ȣ ��Ʈ �񰡽�
    {
        this.transform.localPosition = new Vector3(102.3f, -68.2f, 0);
    }

    public InputField_ADFGVX GetInputField_filePath()//���� ��� ��ǲ �ʵ� ��ȯ
    {
        return filePath;
    }

    public TextField GetTextField_SecurityLevel()//���� ��� ��ǲ �ʵ� ��ȯ
    {
        return securityLevel;
    }

    public void LoadEncodeData()//inputstring�� ���� ��ȣ���� �ҷ��´�
    {
        if (date.GetIsNowFlowText() || sender.GetIsNowFlowText())//���� ���� ���ɹ��� ���� �ҷ����� �۾��� ������ ����
        {
            adfgvx.InformError("���� �ҷ����� �Ұ� : �۾� ���� ��");
            return;
        }

        //return Ű�� �������� �˻�â ���ð� �������� ��Ȱ��ȭ�Ѵ�
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

        //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����ȴ�
        FilePath = "Assets/Resources/Chiper/" + filePath.GetInputString() + ".txt";
        TxtFile = new FileInfo(FilePath);

        if (!TxtFile.Exists)//Filepath�� ��ȿ���� �ʴٸ�
        {
            //Ʃ�丮�� ���� �ڵ�
            if (adfgvx.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.DisplayTutorialDialog(11, 0f);
            }

            adfgvx.InformError("'" + filePath.GetInputString() + "' " + "������ ���� : ��ȿ���� ���� ���");
            filePath.DisplayErrorInInputField("���� ���� �Ұ�!");
            return;
        }

        //Ʃ�丮�� ���� �ڵ�
        if (adfgvx.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (filePath.GetInputString() == "SI-XI-I")//Ʃ�丮���� ������ ������
                adfgvx.MoveToNextTutorialPhase(3f);
            else
                adfgvx.DisplayTutorialDialog(14, 3f);
        }
        else
        {
            adfgvx.SetPartLayerWaitForSec(3f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
            
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

        //��� ��Ʈ �Է� ����
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        adfgvx.InformUpdate("'" + filePath.GetInputString() + "' " + "������ ���� : ���� �ð� 1ms ����");

        //�帧 ��� ����
        ClearChiperAll();
        securityLevel.SetText(SecurityLevel);
        title.FlowText(Title, 3.0f);
        data.FlowText(Data, 3.0f);
        dateUI.SetText(SendingDateUI);
        date.FlowText(SendingDate, 3.0f);
        senderUI.SetText(SenderUI);
        sender.FlowText(Sender, 3.0f);

        //intermediatechiper���� �帧 ��� ����
        adfgvx.afterDecodingPart.GetSecurityLevel().SetText(SecurityLevel);
        adfgvx.afterDecodingPart.GetTitle().FlowText(Title, 3.0f);
        adfgvx.afterDecodingPart.GetDateUI().SetText(ReceptionDateUI);
        adfgvx.afterDecodingPart.GetDate().FlowText(ReceptionDate, 3.0f);
        adfgvx.afterDecodingPart.GetSenderUI().SetText(ReceiverUI);
        adfgvx.afterDecodingPart.GetSender().FlowText(Receiver, 3.0f);

        //���� ���
        adfgvx.soundFlow(30, 3f);

        //��ž��ġ ����
        adfgvx.StartStopWatch();
    }

    public void ClearChiperAll()//�ҷ����� �ִ� ��ȣ���� ����
    {
        securityLevel.SetText("");
        title.SetText("");
        data.SetText("");
        dateUI.SetText("");
        date.SetText("");
        senderUI.SetText("");
        sender.SetText("");
    }

    public string GetData()//�ҷ����ִ� ��ȣ���� ��ȯ�Ѵ�
    {
        return data.GetText();
    }

    public string GetDecodedChiper()//��ȣȭ�� ��ȣ���� ��ȯ�Ѵ�
    {
        return DecodedChiper;
    }
}