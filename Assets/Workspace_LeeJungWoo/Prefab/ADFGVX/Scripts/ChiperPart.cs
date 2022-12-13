using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ChiperPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro partTitle;              //��Ʈ Ÿ��Ʋ
    private TextMeshPro chiperUI;
    private TextMeshPro chiperTitle;            //��ȣ�� ����
    private TextMeshPro chiper;                 //��ȣ�� ����
    private TextMeshPro inputField;             //��ȣ�� �˻�â
    private TextMeshPro dateUI;                 //��¥ ����
    private TextMeshPro date;                   //��¥
    private TextMeshPro senderUI;               //�ۼ��� ����
    private TextMeshPro sender;                 //�ۼ���

    private SpriteRenderer inputFieldColor;     //�˻�â ��� ��������Ʈ
    private string inputString;                 //�÷��̾ �˻�â�� �Է��� ����
    public bool isReadyForInput;                //�˻� �غ� �Ǿ� �ִ°�?
    private bool iaCursorOverInputField;        //�˻�â�� Ŀ���� �ö󰬴°�?
    private bool isOnPrintFlow;
    private const int InpuField_MAX = 18;

    private bool isFlash;                       //������
    private bool skipOneFlash;                  //true�� ������ �ѹ� �ǳʶڴ�

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        partTitle = GetComponentsInChildren<TextMeshPro>()[0];
        chiperUI = GetComponentsInChildren<TextMeshPro>()[1];
        chiperTitle = GetComponentsInChildren<TextMeshPro>()[2];
        chiper = GetComponentsInChildren<TextMeshPro>()[3];
        inputField = GetComponentsInChildren<TextMeshPro>()[4];
        dateUI = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderUI = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputFieldColor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputFieldColor.color = new Color(0, 1, 0, 0);
        inputString = "";
        isFlash = false;

        ClearChiperAll();
        InitializeChiperAll();
        StartCoroutine("FlashinputField");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iaCursorOverInputField && !isReadyForInput)
            {
                inputFieldColor.color = new Color(0, 1, 0, 0);
                inputField.text = inputString;
                isReadyForInput = true;
                isFlash = true;
            }
            else if (iaCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                if (inputString == "")
                    inputField.text = "Ŭ���Ͽ� �Է¡�";
                isReadyForInput = false;
                isFlash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if(!isReadyForInput)
            inputFieldColor.color = new Color(0, 1, 0, 0.15f);
        iaCursorOverInputField = true;
    }

    private void OnMouseExit()
    {
        inputFieldColor.color = new Color(0, 1, 0, 0);
        iaCursorOverInputField = false;
    }

    IEnumerator FlashinputField()//�˻�â�� �����̰� �����
    {
        if (inputString.Length <= InpuField_MAX && isReadyForInput && !skipOneFlash)//�Է�â ���̸� �ѱ�ų�, �Է� ���� �ƴϰų�, ��ŵ ����� �ִٸ� �ǳʶڴ�
        {
            if (isFlash)
            {
                inputField.text = inputString;
                isFlash = false;
            }
            else
            {
                inputField.text = inputString + "��";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")//�Է� ���� �ƴϳ� ��ĭ�� �ƴ϶��, �Է� ���¸� �����Ѵ�
            inputField.text = inputString;

        skipOneFlash = false;//�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashinputField");
    }

    private void DelayFlashinputField()//�������� 0.5�� ���´�
    {
        skipOneFlash = true;
    }

    public void AddInputField(string value)//�Է�â�� �� �ܾ� �߰��Ѵ�
    {
        if (!isReadyForInput)
            return;
        else if (inputString.Length > InpuField_MAX)
        {
            adfgvx.InformError("���� ��� �Է� �ִ� : �Է� �Ұ�");
            return;
        }
        DelayFlashinputField();
        inputString = inputString + value;
        inputField.text = inputString;
    }

    public void DeleteInputField()//�Է�â���� �� �ܾ� �����
    {
        if (!isReadyForInput)
            return;
        else if(inputString.Length < 1)
        {
            adfgvx.InformError("���� ��� �Է� �ּ� : ���� �Ұ�");
            return;
        }
        DelayFlashinputField();
        inputString = inputString.Substring(0, inputString.Length - 1);
        inputField.text = inputString;
    }

    public void UpdateChiperTitleAndText()
    {
        if (isOnPrintFlow)          //���� ���� ��ɹ��� ���� �ҷ����� �۾��� ������ ����
        {
            adfgvx.InformError("���� �ҷ����� �Ұ� : �۾� ���� ��");
            return;
        }
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.InformError("���� �ҷ����� �Ұ�: ���� ��� ��ȣȭ");
            return;
        }

        //return Ű�� �������� �˻�â ���ð� �������� ��Ȱ��ȭ�Ѵ�
        isReadyForInput = false;
        isFlash = false;

        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtchiperUI = "";
        string TxtchiperTitle = "";
        string Txtchiper = "";
        string TxtdateUI = "";
        string Txtdate = "";
        string TxtsenderUI = "";
        string Txtsender = "";

        //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����ȴ�
        FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ChipersTxt/" + inputString + ".txt";                  
        TxtFile = new FileInfo(FilePath);
        
        if (TxtFile.Exists)//Filepath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            TxtchiperUI = Reader.ReadLine();
            TxtchiperTitle = Reader.ReadLine();
            Txtchiper = Reader.ReadLine();
            TxtdateUI = Reader.ReadLine();
            Txtdate = Reader.ReadLine();
            TxtsenderUI = Reader.ReadLine();
            Txtsender = Reader.ReadLine();
            Reader.Close();
        }
        else//Filepath�� ��ȿ���� �ʴٸ�
        {
            adfgvx.InformError("'" + inputString + "' " + "���� ���� : ��ȿ���� ���� ���");
            DisplayErrorInInputField("���� ���� �Ұ�!");
            return;
        }

        adfgvx.UpdateInfoBox("'" + inputString + "' " + "���� ���� : ���� �ð� 1ms ����");

        //���ο� ��ȣ���� �ҷ����⿡ �ռ� �̹� �ҷ����� �ִ� ���� ����
        ClearChiperAll();

        //1���� �帧 ��� ����
        StartCoroutine(printFlow(chiperUI, TxtchiperUI, 0, 3.0f));
        StartCoroutine(printFlow(chiperTitle, TxtchiperTitle, 0, 3.0f));
        StartCoroutine(printFlow(chiper, Txtchiper, 0, 3.0f));
        StartCoroutine(printFlow(dateUI, TxtdateUI, 0, 3.0f));
        StartCoroutine(printFlow(date, Txtdate, 0, 3.0f));
        StartCoroutine(printFlow(senderUI, TxtsenderUI, 0, 3.0f));
        StartCoroutine(printFlow(sender, Txtsender, 0, 3.0f));

        //�۾� ���� �ñ��� ������, ���ο� �۾� ����
        isOnPrintFlow = true;
        Invoke("SetisOnPrintFlowFalse", 3.0f);

        adfgvx.intermediatepart.ClearIntermediateChiperAll();

        //intermediatechiper���� �帧 ��� ����
        StartCoroutine(printFlow(adfgvx.intermediatepart.chiperUI, TxtchiperUI, 0, 3.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.chiperTitle, TxtchiperTitle, 0, 3.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.dateUI, TxtdateUI, 0, 3.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.date, Txtdate, 0, 3.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.senderUI, TxtsenderUI, 0, 3.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.sender, Txtsender, 0, 3.0f));

        adfgvx.soundFlow(30, 0, 3.0f);
    }

    private void DisplayErrorInInputField(string value)//�˻�â�� ���� �޼����� ����
    {
        inputField.text = value;
        inputString = "";
        isReadyForInput = false;
        isFlash = false;
    }

    public void ClearChiperAll()//�ҷ����� �ִ� ��ȣ���� ����
    {
        chiperUI.text = "";
        chiperTitle.text = "";
        chiper.text = "";
        dateUI.text = "";
        date.text = "";
        senderUI.text = "";
        sender.text = "";
    }

    public string GetChiperText()//�Ҷ���� �ִ� ��ȣ���� ��ȯ�Ѵ�
    {
        return chiper.text;
    }

    public void InitializeChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            partTitle.text = "��ȣȭ ������ ����";
            if (chiperUI.text == "")
                chiperUI.text = "[���� ���]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "[������ ����]";
            if (dateUI.text == "")
                dateUI.text = "[�ۼ���]";
            if (date.text == "")
                date.text = "��";
            if (senderUI.text == "")
                senderUI.text = "[�ۼ���]";
            if (sender.text == "")
                sender.text = "��";
        }
        else
        {
            partTitle.text = "��ȣȭ ������ �ε�";
            if (chiperUI.text == "")
                chiperUI.text = "[���� ���]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "[������ ����]";
            if (dateUI.text == "")
                dateUI.text = "[�ۼ���]";
            if (date.text == "")
                date.text = "��";
            if (senderUI.text == "")
                senderUI.text = "[�ۼ���]";
            if (sender.text == "")
                sender.text = "��";
        }
    }

    private IEnumerator printFlow(TextMeshPro target, string value, int idx, float endTime)//tartget�� value�� endTime�ȿ� ���������� ä�� �ִ´�
    {
        if (idx >= value.Length)
            yield break;
        target.text += value.Substring(idx,1);
        yield return new WaitForSeconds(endTime/value.Length);
        StartCoroutine(printFlow(target,value,idx+1,endTime));
    }

    private void SetisOnPrintFlowFalse()//isOnPrintFlow������ �������� �Ѵ�_Invoke��
    {
        isOnPrintFlow = false;
    }

    public void ReturnEncodingResult()//�̹� ��ȣȭ ������� �����մϴ�
    {
        if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.InformError("���� �����ϱ� �Ұ�: ���� ��� ��ȣȭ");
            return;
        }
        Debug.Log("��ȣȭ ������� �����սô�!");
    }
}
