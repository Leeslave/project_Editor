using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ChiperPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro parttitle;              //��Ʈ Ÿ��Ʋ
    private TextMeshPro chiperui;
    private TextMeshPro chipertitle;            //��ȣ�� ����
    private TextMeshPro chiper;                 //��ȣ�� ����
    private TextMeshPro inputfield;             //��ȣ�� �˻�â
    private TextMeshPro dateui;                 //��¥ ����
    private TextMeshPro date;                   //��¥
    private TextMeshPro senderui;               //�ۼ��� ����
    private TextMeshPro sender;                 //�ۼ���

    private SpriteRenderer inputfieldcolor;     //�˻�â ��� ��������Ʈ

    private string inputstring;                 //�÷��̾ �˻�â�� �Է��� ����
    public bool isreadyforinput;                //�˻� �غ� �Ǿ� �ִ°�?
    private bool iscursoroverinputfield;        //�˻�â�� Ŀ���� �ö󰬴°�?
    private bool isonprintflow;

    private bool isflash;                       //������
    private bool skiponeflash;                  //true�� ������ �ѹ� �ǳʶڴ�

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        parttitle = GetComponentsInChildren<TextMeshPro>()[0];
        chiperui = GetComponentsInChildren<TextMeshPro>()[1];
        chipertitle = GetComponentsInChildren<TextMeshPro>()[2];
        chiper = GetComponentsInChildren<TextMeshPro>()[3];
        inputfield = GetComponentsInChildren<TextMeshPro>()[4];
        dateui = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderui = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputfieldcolor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        inputstring = "";
        isflash = false;

        ClearChiperAll();
        InitializeChiperAll();
        StartCoroutine("FlashInputField");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                inputfieldcolor.color = new Color(0, 1, 0, 0);
                inputfield.text = inputstring + "��";
                isreadyforinput = true;
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (inputstring == "")
                    inputfield.text = "Ŭ���Ͽ� �Է¡�";
                isreadyforinput = false;
                isflash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if(!isreadyforinput)
            inputfieldcolor.color = new Color(0, 1, 0, 0.15f);
        iscursoroverinputfield = true;
    }

    private void OnMouseExit()
    {
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        iscursoroverinputfield = false;
    }

    IEnumerator FlashInputField()//�˻�â�� �����̰� �����
    {
        if (inputstring.Length <= 16 && isreadyforinput && !skiponeflash)           //�Է�â ���̸� �ѱ�ų�, �Է� ���� �ƴϰų�, ��ŵ ����� �ִٸ� �ǳʶڴ�
        {
            if (isflash)
            {
                inputfield.text = inputstring;
                isflash = false;
            }
            else
            {
                inputfield.text = inputstring + "��";
                isflash = true;
            }
        }
        else if (!isreadyforinput && inputstring != "")                             //�Է� ���� �ƴϳ� ��ĭ�� �ƴ϶��, �Է� ���¸� �����Ѵ�
            inputfield.text = inputstring;

        skiponeflash = !skiponeflash ? false : false;                               //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField");
    }

    private void DelayFlashInputField()//�������� 0.5�� ���´�
    {
        skiponeflash = true;
    }

    public void AddInputField(string value)//�Է�â�� �� �ܾ� �߰��Ѵ�
    {
        if (!isreadyforinput)
            return;
        else if (inputstring.Length > 16)
        {
            adfgvx.UpdateInfoBox("���� ��� �ִ� �Է� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }
        DelayFlashInputField();
        inputstring = inputstring + value;
        inputfield.text = inputstring;
    }

    public void DeleteInputField()//�Է�â���� �� �ܾ� �����
    {
        if (!isreadyforinput)
            return;
        else if(inputstring.Length < 1)
        {
            adfgvx.UpdateInfoBox("���� ��� ���� �Ұ� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }
        DelayFlashInputField();
        inputstring = inputstring.Substring(0, inputstring.Length - 1);
        inputfield.text = inputstring;
    }

    public void UpdateChiperTitleAndText()
    {
        if (isonprintflow)          //���� ���� ��ɹ��� ���� �ҷ����� �۾��� ������ ����
        {
            adfgvx.UpdateInfoBox("���� �ҷ����� �Ұ�: �۾� ���� ��");
            return;
        }
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("���� �ҷ����� �Ұ�: ���� ��� ��ȣȭ");
            return;
        }

        //return Ű�� �������� �˻�â ���ð� �������� ��Ȱ��ȭ�Ѵ�
        isreadyforinput = false;
        isflash = false;

        string FilePath = "";
        FileInfo TxtFile = null;
        string Txtchiperui = "";
        string Txtchipertitle = "";
        string Txtchiper = "";
        string Txtdateui = "";
        string Txtdate = "";
        string Txtsenderui = "";
        string Txtsender = "";

        //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����ȴ�
        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ChipersTxt/" + inputstring + ".txt";                  
        TxtFile = new FileInfo(FilePath);
        
        if (TxtFile.Exists)//Filepath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            Txtchiperui = Reader.ReadLine();
            Txtchipertitle = Reader.ReadLine();
            Txtchiper = Reader.ReadLine();
            Txtdateui = Reader.ReadLine();
            Txtdate = Reader.ReadLine();
            Txtsenderui = Reader.ReadLine();
            Txtsender = Reader.ReadLine();
            Reader.Close();
        }
        else//Filepath�� ��ȿ���� �ʴٸ�
        {
            adfgvx.UpdateInfoBox("���� �ҷ����� �Ұ�: ��ȿ���� ���� ���");
            DisplayError("���� ���� �Ұ�!");
            return;
        }

        adfgvx.UpdateInfoBox("���� �ҷ����� ����");

        //���ο� ��ȣ���� �ҷ����⿡ �ռ� �̹� �ҷ����� �ִ� ���� ����
        ClearChiperAll();

        //1���� �帧 ��� ����
        StartCoroutine(printFlow(chiperui, Txtchiperui, 0, 2.0f));
        StartCoroutine(printFlow(chipertitle, Txtchipertitle, 0, 2.0f));
        StartCoroutine(printFlow(chiper, Txtchiper, 0, 2.0f));
        StartCoroutine(printFlow(dateui, Txtdateui, 0, 2.0f));
        StartCoroutine(printFlow(date, Txtdate, 0, 2.0f));
        StartCoroutine(printFlow(senderui, Txtsenderui, 0, 2.0f));
        StartCoroutine(printFlow(sender, Txtsender, 0, 2.0f));

        //�۾� ���� �ñ��� ������, ���ο� �۾� ����
        isonprintflow = true;
        Invoke("SetisonprintflowFalse", 2.0f);

        adfgvx.intermediatepart.ClearIntermediateChiperAll();

        //intermediatechiper�� ����
        StartCoroutine(printFlow(adfgvx.intermediatepart.chiperui, Txtchiperui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.chipertitle, Txtchipertitle, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.dateui, Txtdateui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.date, Txtdate, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.senderui, Txtsenderui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.sender, Txtsender, 0, 2.0f));
    }

    private void DisplayError(string value)//�˻�â�� ���� �޼����� ����
    {
        inputfield.text = value;
        inputstring = "";
        isreadyforinput = false;
        isflash = false;
    }

    public void ClearChiperAll()//�ҷ����� �ִ� ��ȣ���� ����
    {
        chiperui.text = "";
        chipertitle.text = "";
        chiper.text = "";
        dateui.text = "";
        date.text = "";
        senderui.text = "";
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
            parttitle.text = "��ȣȭ ������ ����";
            if (chiperui.text == "")
                chiperui.text = "[���� ���]";
            if (chipertitle.text == "")
                chipertitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "[������ ����]";
            if (dateui.text == "")
                dateui.text = "[�ۼ���]";
            if (date.text == "")
                date.text = "��";
            if (senderui.text == "")
                senderui.text = "[�ۼ���]";
            if (sender.text == "")
                sender.text = "��";
        }
        else
        {
            parttitle.text = "��ȣȭ ������ �ε�";
            if (chiperui.text == "")
                chiperui.text = "[���� ���]";
            if (chipertitle.text == "")
                chipertitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "[������ ����]";
            if (dateui.text == "")
                dateui.text = "[�ۼ���]";
            if (date.text == "")
                date.text = "��";
            if (senderui.text == "")
                senderui.text = "[�ۼ���]";
            if (sender.text == "")
                sender.text = "��";
        }
    }

    private IEnumerator printFlow(TextMeshPro target, string value, int idx, float endtime)//tartget�� value�� endtime�ȿ� ���������� ä�� �ִ´�
    {
        if (idx >= value.Length)
            yield break;
        target.text += value.Substring(idx,1);
        yield return new WaitForSeconds(endtime/value.Length);
        StartCoroutine(printFlow(target,value,idx+1,endtime));
    }

    private void SetisonprintflowFalse()//isonprintflow������ �������� �Ѵ�_Invoke��
    {
        isonprintflow = false;
    }

    public void ReturnEncodingResult()//�̹� ��ȣȭ ������� �����մϴ�
    {
        if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.UpdateInfoBox("���� �����ϱ� �Ұ�: ���� ��� ��ȣȭ");
            return;
        }
        Debug.Log("��ȣȭ ������� �����սô�!");
    }
}
