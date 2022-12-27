using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntermediatePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro partTitle;
    private TextMeshPro fileRoute;
    public TextMeshPro chiperUI;
    public TextMeshPro chiperTitle;
    private TextMeshPro chiper;
    public TextMeshPro dateUI;
    public TextMeshPro date;
    public TextMeshPro senderUI;
    public TextMeshPro sender;

    private SpriteRenderer inputFieldColor;
    private string inputString;
    private bool isCursorOverInputField;
    public bool isReadyForInput;
    private bool isFlash;
    private bool skipOneFlash;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        partTitle = GetComponentsInChildren<TextMeshPro>()[0];
        fileRoute = GetComponentsInChildren<TextMeshPro>()[1];
        chiperUI = GetComponentsInChildren<TextMeshPro>()[2];
        chiperTitle = GetComponentsInChildren<TextMeshPro>()[3];
        chiper = GetComponentsInChildren<TextMeshPro>()[4];
        dateUI = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderUI = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputFieldColor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputFieldColor.color = new Color(0, 1, 0, 0);
        inputString = "";
        isFlash = false;

        ClearIntermediateChiper();
        ClearIntermediateChiperAll();
        InitializeIntermediateChiperAll();
        StartCoroutine(FlashInputField());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isCursorOverInputField && !isReadyForInput)
            {
                inputFieldColor.color = new Color(0, 1, 0, 0);
                chiper.text = inputString + "��";
                isReadyForInput = true;
                isFlash = true;
            }
            else if (isCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                InitializeIntermediateChiperAll();
                isReadyForInput = false;
                isFlash = false;
            }
        }

        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            fileRoute.text = "���: ��ȣȭ ������";
        else
            fileRoute.text = "���: ��ȣȭ ������";
    }

    private void OnMouseEnter()
    {
        if (!isReadyForInput)
            inputFieldColor.color = new Color(0, 1, 0, 0.15f);
        isCursorOverInputField = true;
    }

    private void OnMouseExit()
    {
        inputFieldColor.color = new Color(0, 1, 0, 0);
        isCursorOverInputField = false;
    }


    public void SetLayer(int layer)//��� �Է� ����
    {
        this.gameObject.layer = layer;
        GameObject.Find("Save").layer = layer;
    }

    IEnumerator FlashInputField()//�˻�â�� �����̰� �����
    {
        if (inputString.Length <= 192 && isReadyForInput && !skipOneFlash)//�Է�â ���̸� �ѱ�ų�, �Է� ���� �ƴϰų�, ��ŵ ����� �ִٸ� �ǳʶڴ�
        {
            if (isFlash)
            {
                chiper.text = inputString;
                isFlash = false;
            }
            else
            {
                chiper.text = inputString + "��";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && inputString != "")//�Է� ���� �ƴϳ� ��ĭ�� �ƴ϶��, �Է� ���¸� �����Ѵ�
            chiper.text = inputString;

        //�̹� �Ͽ� ��ŵ������ ���� ������ ���ڿ��� �Ѵ�
        skipOneFlash = !skipOneFlash ? false : false;                               
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FlashInputField());
    }

    private void DelayFlashInputField()//�������� 0.5�� ���´�
    {
        skipOneFlash = true;
        return;
    }

    public void AddIntermediateChiper(string value)
    {
        if (!isReadyForInput)
            return;

        if(inputString.Length > 192)
        {
            adfgvx.InformError("��ȣ�� �ִ� �Է� : �Է� �Ұ�");
            return;
        }

        inputString += value;
        chiper.text = inputString;
        skipOneFlash = true;

        //�Է��� ���
        adfgvx.PlayAudioSource(1);
        
        return;
    }

    public void DeleteIntermediateChiper()
    {
        if (!isReadyForInput)
            return;

        string text = inputString;
        int DeleteLength = adfgvx.currentmode == ADFGVX.mode.Decoding ? 2 : 3;
        
        if (text.Length < DeleteLength)
        {
            adfgvx.InformError("��ȣ�� �ּ� �Է� : ���� �Ұ�");
            return;
        }
        
        inputString = text.Substring(0, text.Length - DeleteLength);
        chiper.text = inputString;
        skipOneFlash = true;

        //������ ���
        adfgvx.PlayAudioSource(2);

        return;
    }

    public void ClearIntermediateChiper()
    {
        chiper.text = "";
        return;
    }

    public string GetIntermediateChiper()
    {
        return inputString;
    }

    public void ClearIntermediateChiperAll()
    {
        inputString = "";
        chiperUI.text = "";
        chiperTitle.text = "";
        dateUI.text = "";
        date.text = "";
        senderUI.text = "";
        sender.text = "";
    }

    public void InitializeIntermediateChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            partTitle.text = "��ȣȭ ���÷���";
            if (chiperUI.text == "")
                chiperUI.text = "[���� ���]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[������ ����]";
            if (inputString == "")
                chiper.text = "[��ȣȭ ����]";
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
            partTitle.text = "��ȣȭ ���÷���";
            if (chiperUI.text == "")
                chiperUI.text = "[���� ���]";
            if (chiperTitle.text == "")
                chiperTitle.text = "[������ ����]";
            if (inputString == "")
                chiper.text = "Ŭ���Ͽ� �Է¡�";
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
}
