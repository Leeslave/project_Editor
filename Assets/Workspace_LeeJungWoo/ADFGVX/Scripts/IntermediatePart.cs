using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntermediatePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro parttitle;
    private TextMeshPro fileroute;
    public TextMeshPro chiperui;
    public TextMeshPro chipertitle;
    private TextMeshPro chiper;
    public TextMeshPro dateui;
    public TextMeshPro date;
    public TextMeshPro senderui;
    public TextMeshPro sender;

    private SpriteRenderer inputfieldcolor;
    private string inputstring;
    private bool iscursoroverinputfield;
    public bool isreadyforinput;
    private bool isflash;
    private bool skiponeflash;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        parttitle = GetComponentsInChildren<TextMeshPro>()[0];
        fileroute = GetComponentsInChildren<TextMeshPro>()[1];
        chiperui = GetComponentsInChildren<TextMeshPro>()[2];
        chipertitle = GetComponentsInChildren<TextMeshPro>()[3];
        chiper = GetComponentsInChildren<TextMeshPro>()[4];
        dateui = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderui = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputfieldcolor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        inputstring = "";
        isflash = false;

        ClearIntermediateChiperAll();
        InitializeIntermediateChiperAll();
        StartCoroutine("FlashInputField");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                inputfieldcolor.color = new Color(0, 1, 0, 0);
                chiper.text = inputstring + "��";
                isreadyforinput = true;
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (inputstring == "")
                    chiper.text = "Ŭ���Ͽ� �Է¡�";
                isreadyforinput = false;
                isflash = false;
            }
        }

        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            fileroute.text = "���: ��ȣȭ ������";
        else
            fileroute.text = "���: ��ȣȭ ������";
    }

    private void OnMouseEnter()
    {
        if (!isreadyforinput)
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
                chiper.text = inputstring;
                isflash = false;
            }
            else
            {
                chiper.text = inputstring + "��";
                isflash = true;
            }
        }
        else if (!isreadyforinput && inputstring != "")                             //�Է� ���� �ƴϳ� ��ĭ�� �ƴ϶��, �Է� ���¸� �����Ѵ�
            chiper.text = inputstring;

        skiponeflash = !skiponeflash ? false : false;                               //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField");
    }

    private void DelayFlashInputField()//�������� 0.5�� ���´�
    {
        skiponeflash = true;
    }

    public void AddIntermediateChiper(string value)
    {
        inputstring += value;
        chiper.text = inputstring;
        skiponeflash = true;
        return;
    }

    public void ClearIntermediateChiper()
    {
        chiper.text = "";
        return;
    }

    public void DeleteText()
    {
        if (!isreadyforinput)
            return;

        string text = inputstring;
        int DeleteLength = adfgvx.currentmode == ADFGVX.mode.Decoding ? 2 : 3;
        if (text.Length >= DeleteLength)
        {
            inputstring = text.Substring(0, text.Length - DeleteLength);
            chiper.text = inputstring;
            skiponeflash = true;
        }
        else
        {
            adfgvx.UpdateInfoBox("��ȣ�� ���� �Ұ� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
        }
    }

    public string GetIntermediateChiper()
    {
        return inputstring;
    }

    public void ClearIntermediateChiperAll()
    {
        inputstring = "";
        chiperui.text = "";
        chipertitle.text = "";
        chiper.text = "";
        dateui.text = "";
        date.text = "";
        senderui.text = "";
        sender.text = "";
    }

    public void InitializeIntermediateChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            parttitle.text = "��ȣȭ ���÷���";
            if (chiperui.text == "")
                chiperui.text = "[���� ���]";
            if (chipertitle.text == "")
                chipertitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "[��ȣȭ ����]";
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
            parttitle.text = "��ȣȭ ���÷���";
            if (chiperui.text == "")
                chiperui.text = "[���� ���]";
            if (chipertitle.text == "")
                chipertitle.text = "[������ ����]";
            if (chiper.text == "")
                chiper.text = "Ŭ���Ͽ� �Է¡�";
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
}
