using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeEncodingPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX data;
    private TextField block;
    private TextField primeFactor;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        data = transform.Find("Data").GetComponent<InputField_ADFGVX>();
        block = transform.Find("Block").GetComponent<TextField>();
        primeFactor = transform.Find("PrimeFactor").GetComponent<TextField>();
    }

    public void SetLayer(int layer)//�Է� ����
    {
        this.gameObject.layer = layer;
        data.gameObject.layer = layer;
    }

    public void VisiblePart()//��Ʈ ���� ���
    {
        this.transform.localPosition = new Vector3(96.3f, -19f, 0);
    }

    public void UnvisiblePart()//��Ʈ �񰡽� ���
    {
        this.transform.localPosition = new Vector3(96.3f, 200, 0);
    }

    public InputField_ADFGVX GetInputField_Data()//������ ��ǲ �ʵ� ��ȯ
    {
        return data;
    }

    public void AddInputFieldByKeyboard(string value)//keyboard �Է¿� ���� data�� value�� �����ϴ� ���� �Է�
    {
        if (!data.GetIsReadyForInput())
            return;

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

        if(idx_row == 6 && idx_line == 6)//�ش��ϴ� ������Ʈ�� ã�� ���߽��ϴ�
        {
            adfgvx.InformError("ADFGVX�迭�� �������� �ʴ� �Է� : �Է� �Ұ�");
            return;
        }
        
        data.AddInputFieldByKeyboard(array[idx_row].ToString() + array[idx_line].ToString() + " ");
        UpdateRecommendKeyword();
    }

    public void AddInputField_DataByButton(string value)//button �Է¿� ���� data�� value�� �����ϴ� ���� �Է�
    {
        data.AddInputFieldByButton(value);
        UpdateRecommendKeyword();
    }

    public void DeleteInputField_DataByKeyboard()//keyboard �Է��� ���� data�� ����� ����
    {
        if (!data.GetIsReadyForInput())
            return;

        data.DeleteInputFieldByKeyboard(3);
        UpdateRecommendKeyword();
    }

    public void DeleteInputField_DataByButton()//button �Է¿� ���� data�� ����� ����
    {
        data.DeleteInputFieldByButton(3);
        UpdateRecommendKeyword();
    }

    private void UpdateRecommendKeyword()//��õ Ű���� ������Ʈ
    {
        string number = ("��ȣȭ ��� �� ���� �� : " + data.GetMarkText().Length / 3 * 2).ToString();

        string prime = "��õ ��ȣ Ű ���� �� : ";

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
