using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class BiliteralSubstitutionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private int decodeRow;
    private int decodeLine;

    public ElementButton[] elementButtons = new ElementButton[36];
    public RowButton[] rowButtons = new RowButton[6];
    public LineButton[] lineButtons = new LineButton[6];
    private TextMeshPro rowText;
    private TextMeshPro lineText;
    private TextMeshPro arrayNumText;

    private int currentArrayNum;
    private const int ArrayNum_MAX = 4;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        rowText = GetComponentsInChildren<TextMeshPro>()[1];
        lineText = GetComponentsInChildren<TextMeshPro>()[2];
        arrayNumText = GetComponentsInChildren<TextMeshPro>()[3];

        decodeRow = 6;
        decodeLine = 6;

        currentArrayNum = 0;
        UpdateArray(currentArrayNum.ToString());
    }

    private void UpdateArray(string currentArrayNum)//currentArrayNum�� ���� ���ο� ADFGVX�迭�� �ε��ؼ� Array�� ������Ʈ
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string value = "";
        FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ArrayTxt/Array_" + currentArrayNum + ".txt";
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)//FilePath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath);
            value = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");

        //��� public elementButtons�� �����ϸ鼭 ��ư�� �ؽ�Ʈ�� ADFGVXǥ�� ����� �����մϴ�
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                elementButtons[i * 6 + j].ChangeButtonText(value[i * 6 + j]);                              
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
            adfgvx.UpdateInfoBox("��ư ��� �Ұ� : ���� ��� ��Ȯ�� ���");
        }
    }

    public void OnDecRowDown(int row)//Decoding Mode���� row�� ��ư�� ������ ��
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("��ư ��� �Ұ� : ���� ��� ��Ȯ�� ���");
            return;
        }

        decodeRow = row;

        for (int i = 0; i < 6; i++)//���� ���õ� RowButton ���� ���õǾ��� RowButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].DisableClickSprite();
            }
        }

        if (decodeLine != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].DisableClickSprite();
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].Selected = false;
                decodeRow = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode���� line�� ��ư�� ������ ��
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("��ư ��� �Ұ� : ���� ��� ��Ȯ�� ���");
            return;
        }

        decodeLine = line;

        for (int i = 0; i < 6; i++)//���� ���õ� LineButton ���� ���õǾ��� LineButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].DisableClickSprite();
            }
        }
        if (decodeRow != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.intermediatepart.AddIntermediateChiper(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].DisableClickSprite();
                rowButtons[decodeRow].Selected = false;
                decodeRow = 6;
            }
        }
    }

    public void ArrayPlus()//ADFGVX�迭 +1�� ��ȯ
    {
        currentArrayNum++;
        currentArrayNum %= ArrayNum_MAX;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateArray(currentArrayNum.ToString());
    }

    public void ArrayMinus()//ADFGVX�迭 -1�� ��ȯ
    {
        currentArrayNum--;
        if (currentArrayNum < 0)
            currentArrayNum = ArrayNum_MAX - 1;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateArray(currentArrayNum.ToString());
    }

    public void ResponseUseDictionaryADFGVX(int idx , string value)//Ű���� ���� �Է¿� ����
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            int row = 0;
            int line = 0;
            for (int i=0;i<36;i++)
            {
                if(elementButtons[i].GetButtonText() == value)
                {
                    row = i / 6;
                    line = i % 6;
                }
            }
            adfgvx.intermediatepart.AddIntermediateChiper(array[row].ToString() + array[line].ToString() + " ");
        }
        else if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            if (!adfgvx.intermediatepart.isReadyForInput)
                return;

            char[] array = new char[7] { 'A', 'D', 'F', 'G', 'V', 'X', '-' };
            rowText.text = array[decodeRow].ToString();
            lineText.text = array[decodeLine].ToString();

            if(value == "-")
            {
                rowText.text = "-";
                decodeRow = 6;
                lineText.text = "-";
                decodeLine = 6;
            }
            else if (decodeRow == 6)
            {
                rowText.text = array[idx].ToString();
                decodeRow = idx;
            }
            else if (decodeLine == 6)
            {
                lineText.text = array[idx].ToString();
                decodeLine = idx;
            }
            else if (decodeRow != 6 && decodeLine != 6)
            {
                rowText.text = array[idx].ToString();
                decodeRow = idx;
                lineText.text = array[6].ToString();
                decodeLine = 6;
            }
        }
    }

    public void ResponseUseDictionaryEnter()//Ű���� ���� �Է¿� ����
    {
        if (decodeRow == 6 || decodeLine == 6)
            return;
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            return;
        OnDecLineDown(decodeLine);
    }

    public void InitializeText()//��� ��ȯ�� ���� ���� ���� ġȯ ��Ʈ�� �ʱ�ȭ
    {
        rowText.text = "-";
        decodeRow = 6;
        lineText.text = "-";
        decodeLine = 6;
    }

   
}
