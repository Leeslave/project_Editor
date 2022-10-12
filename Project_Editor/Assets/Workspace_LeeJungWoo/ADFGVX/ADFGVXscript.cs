using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVXscript : MonoBehaviour
{
    private char[,] Array = new char[6, 6];
    private enum Codemode//ADFGVX���
    { Encoding,Decoding};
    private Codemode CurrentCodemode;//���� ADFGVX���

    private struct DecodeElemnt//��ȣȭ ������Ʈ
    {
        public int row;
        public int line;
    }
    private DecodeElemnt CurrentDecodeElement;//���� ��ȣȭ ������Ʈ

    public ElementButton[] Buttons = new ElementButton[36];
    public Mode ModeBox;
    public InterChiper InterChiperBox;
    public Info InfoBox;

    private void Start()
    {
        UpdateArray("D");
        InterChiperBox.ClearText();
        UpdateInfoBox("ȯ���մϴ�!");
        UpdateInfoBoxDelay(0.5f, "ADFGVX ���̺��� �����Ͻʽÿ�.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateArray("0");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX ���̺� 1��, ����Ǿ����ϴ�");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateArray("1");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX ���̺� 2��, ����Ǿ����ϴ�");
            InformCurrentMode();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateArray("2");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX ���̺� 3��, ����Ǿ����ϴ�");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateArray("3");
            ClearInterChiperBox();
            UpdateInfoBox("ADFGVX ���̺� 4��, ����Ǿ����ϴ�");
            InformCurrentMode();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateInfoBox("�߰� ��ȣ ���� ���");
            ClearInterChiperBox();
            InformCurrentMode();
        }
        if(Input.GetKeyDown(KeyCode.M))//MŰ�� ������ ��ȣȭ ���, ��ȣȭ ��� ��ȯ
        {
            if(CurrentCodemode == Codemode.Encoding)
            {
                CurrentCodemode = Codemode.Decoding;
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
                UpdateInfoBox("��� ��ȯ : ��ȣȭ");
                UpdateModeBox("D", "e");
                ClearInterChiperBox();
            }
            else if(CurrentCodemode == Codemode.Decoding)
            {
                CurrentCodemode = Codemode.Encoding;
                UpdateInfoBox("��� ��ȯ : ��ȣȭ");
                UpdateModeBox("E", "n");
                ClearInterChiperBox();
            }
            InformCurrentMode();
        }

    }

    private void UpdateArray(string ArrayNum)//���ο� ADFGVXǥ�� �ε��ؿ´�
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ADFGVX_Array_Num_" + ArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)
        {
            StreamReader Reader = new StreamReader(FilePath);
            TxtValue = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        char[] Txt = new char[36];
        Txt = TxtValue.ToCharArray();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Array[i, j] = Txt[i * 6 + j];
                Buttons[i * 6 + j].ChangeButtonText(Txt[i * 6 + j]);
            }
        }
    }


    public void OnEncElementDown(int row, int line)//Encoding Mode���� 6x6ǥ�� ��ư�� ������ ��
    {
        if (CurrentCodemode == Codemode.Encoding)
        {
            char[] ADFGVX = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
            AddInterChiperBox(ADFGVX[row] + "" + ADFGVX[line] + " ");
        }
        else if (CurrentCodemode == Codemode.Decoding)
        {
            UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            InformCurrentMode();
        }
    }
    public void OnDecRowDown(int row)//Decoding Mode���� row�� ��ư�� ������ ��
    {
        if (CurrentCodemode == Codemode.Decoding)
        {
            CurrentDecodeElement.row = row;
            if (CurrentDecodeElement.line != 6)
            {
                AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
            }
        }
        else if (CurrentCodemode == Codemode.Encoding)
        {
            UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            InformCurrentMode();
        }
    }
    public void OnDecLineDown(int line)//Decodeing Mode���� line�� ��ư�� ������ ��
    {
        if (CurrentCodemode == Codemode.Decoding)
        {
            CurrentDecodeElement.line = line;
            if (CurrentDecodeElement.row != 6)
            {
                AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
                CurrentDecodeElement.row = 6;
                CurrentDecodeElement.line = 6;
            }
        }
        else if (CurrentCodemode == Codemode.Encoding)
        {
            UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            InformCurrentMode();
        }
    }


    private void UpdateModeBox(string Value1, string Value2)
    {
        ModeBox.UpdateText(Value1, Value2);
    }
    private void AddInterChiperBox(string Value)//InterChiperBox�� �ؽ�Ʈ�� �߰�
    {
        InterChiperBox.AddText(Value);
    }
    private void ClearInterChiperBox()//InterChiperBox�� �ؽ�Ʈ�� ���
    {
        InterChiperBox.ClearText();
    }
    private void InformCurrentMode()//1�� �Ŀ� ���� ��� ���
    {
        if (CurrentCodemode == Codemode.Encoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
        else if (CurrentCodemode == Codemode.Decoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
    }
    private void UpdateInfoBox(string Value)//InfoBox�� �ؽ�Ʈ�� Value�� �ٲ۴�
    {
        InfoBox.UpdateText(Value);
    }
    private void UpdateInfoBoxDelay(float Time, string Value)//InfoBox�� �ؽ�Ʈ�� Timer�� �Ŀ� Value�� �ٲ۴�
    {
        StartCoroutine(UpdateInfoBoxTimer(Time, Value));        
    }
    private IEnumerator UpdateInfoBoxTimer(float Time, string Value)//UpdateInfoBoxDelay �ڷ�ƾ
    {
        float currenttime = 0.0f;
        while (currenttime < Time)
        {
            yield return new WaitForSeconds(0.01f);
            currenttime += 0.01f;
        }
        UpdateInfoBox(Value);
        yield return null;
    }
}
