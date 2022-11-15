using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class BiliteralSubstitutionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private int decoderow;
    private int decodeline;

    public ElementButton[] elementbuttons = new ElementButton[36];
    public RowButton[] rowbuttons = new RowButton[6];
    public LineButton[] linebuttons = new LineButton[6];
    private TextMeshPro rowtext;
    private TextMeshPro linetext;

    private int arraynum;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        rowtext = GetComponentsInChildren<TextMeshPro>()[1];
        linetext = GetComponentsInChildren<TextMeshPro>()[2];

        decoderow = 6;
        decodeline = 6;

        arraynum = 0;
        UpdateArray("0");
    }

    private void Update()
    {

    }

    private void UpdateArray(string ArrayNum)//ArrayNum�� ���� ���ο� ADFGVX�迭�� �ε��ؼ� Array�� ������Ʈ
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string value = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ArrayTxt/Array_" + ArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)//FilePath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath);
            value = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");

        //��� public ElementButtons�� �����ϸ鼭 ��ư�� �ؽ�Ʈ�� ADFGVXǥ�� ����� �����մϴ�
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                elementbuttons[i * 6 + j].ChangeButtonText(value[i * 6 + j]);                              
            }
        }
    }
    
    public void OnEncElementDown(int row, int line)//Encoding Mode���� 6x6ǥ�� ��ư�� ������ ��
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
            adfgvx.intermediatepart.AddIntermediateChiper(array[row] + "" + array[line] + " ");
        }
        else if (adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
        }
    }

    public void OnDecRowDown(int row)//Decoding Mode���� row�� ��ư�� ������ ��
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }

        decoderow = row;

        for (int i = 0; i < 6; i++)//���� ���õ� RowButton ���� ���õǾ��� RowButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (rowbuttons[i].Selected == true && i != row)
            {
                rowbuttons[i].Selected = false;
                rowbuttons[i].DisableClickSprite();
            }
        }

        if (decodeline != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementbuttons[decoderow * 6 + decodeline].GetButtonText() + " ");
            if (decodeline != 6)
            {
                linebuttons[decodeline].DisableClickSprite();
                linebuttons[decodeline].Selected = false;
                decodeline = 6;
            }
            if (decoderow != 6)
            {
                rowbuttons[decoderow].Selected = false;
                decoderow = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode���� line�� ��ư�� ������ ��
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("���� ��ȣȭ ���, ��� ��Ȯ�� ���");
            adfgvx.InformCurrentMode();
            return;
        }

        decodeline = line;

        for (int i = 0; i < 6; i++)//���� ���õ� LineButton ���� ���õǾ��� LineButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (linebuttons[i].Selected == true && i != line)
            {
                linebuttons[i].Selected = false;
                linebuttons[i].DisableClickSprite();
            }
        }
        if (decoderow != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementbuttons[decoderow * 6 + decodeline].GetButtonText() + " ");
            if (decodeline != 6)
            {
                linebuttons[decodeline].Selected = false;
                decodeline = 6;
            }
            if (decoderow != 6)
            {
                rowbuttons[decoderow].DisableClickSprite();
                rowbuttons[decoderow].Selected = false;
                decoderow = 6;
            }
        }
    }

    public void ShiftArray()//ADFGVX�迭 ��ȯ
    {
        arraynum++;
        arraynum %= 4;
        UpdateArray(arraynum.ToString());
    }

    public void ResponseUseDictionaryADFGVX(int idx , string value)//Ű���� ���� �Է¿� ����
    {
        if (!adfgvx.intermediatepart.isreadyforinput)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            int row = 0;
            int line = 0;
            for (int i=0;i<36;i++)
            {
                if(elementbuttons[i].GetButtonText() == value)
                {
                    row = i / 6;
                    line = i % 6;
                }
            }
            adfgvx.intermediatepart.AddIntermediateChiper(array[row].ToString() + array[line].ToString() + " ");
        }
        else if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            rowtext.text = array[decoderow].ToString();
            linetext.text = array[decodeline].ToString();

            if(value == "-")
            {
                rowtext.text = "-";
                decoderow = 6;
                linetext.text = "-";
                decodeline = 6;
            }
            else if (decoderow == 6)
            {
                rowtext.text = array[idx].ToString();
                decoderow = idx;
            }
            else if (decodeline == 6)
            {
                linetext.text = array[idx].ToString();

                decodeline = idx;
            }
            else if (decoderow != 6 && decodeline != 6)
            {
                rowtext.text = array[idx].ToString();
                decoderow = idx;
                linetext.text = array[6].ToString();
                decodeline = 6;
            }
        }
    }

    public void ResponseUseDictionaryEnter()//Ű���� ���� �Է¿� ����
    {
        if (decoderow == 6 || decodeline == 6)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            return;
        OnDecLineDown(decodeline);
    }

    public void InitializeText()//��� ��ȯ�� ���� ���� ���� ġȯ ��Ʈ�� �ʱ�ȭ
    {
        rowtext.text = "-";
        decoderow = 6;
        linetext.text = "-";
        decodeline = 6;
    }

   
}
