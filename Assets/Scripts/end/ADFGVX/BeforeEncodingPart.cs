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

    public void SetLayer(int layer)//�� ���ӿ�����Ʈ ���� ����� ���̾� ����
    {
        transform.Find("Data").gameObject.layer = layer;
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

    public void AddInputField_Data(string value)//data�� value�� �����ϴ� ���� �Է�
    {
        //Ʃ�丮�� ���� �ڵ�
        if(adfgvx.GetCurrentTutorialPhase() == 1 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.biliteralsubstitutionpart.GetCurrentArrayNum() != 0)
            {
                adfgvx.DisplayTutorialDialog(44, 0f);
                return;
            }
            else
                adfgvx.MoveToNextTutorialPhase(2.0f);
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

        if(idx_row == 6 && idx_line == 6)//�ش��ϴ� ������Ʈ�� ã�� ���߽��ϴ�
        {
            adfgvx.InformError("ADFGVX�迭�� �������� �ʴ� �Է� : �Է� �Ұ�");
            return;
        }

        data.AddInputField(array[idx_row].ToString() + array[idx_line].ToString() + " ");
        UpdateRecommendKeyword();
    }

    public void DeleteInputField_Data()//keyboard �Է��� ���� data�� ����� ����
    {        
        data.DeleteInputField(3);
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
