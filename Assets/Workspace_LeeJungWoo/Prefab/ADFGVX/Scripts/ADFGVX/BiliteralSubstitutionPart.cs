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

    private void Start()
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

    public void SetLayer(int layer)//���̾� ����
    {
        this.gameObject.layer = layer;
        GameObject.Find("ArrayKeyboard").layer = layer;
        GameObject.Find("Delete").layer = layer;
        GameObject.Find("ArrayMinus").layer = layer;
        GameObject.Find("ArrayPlus").layer = layer;
    }

    public TextMeshPro GetRowText()//�Է� ��� ���� �� ����
    {
        return rowText;
    }

    public TextMeshPro GetLineText()//�Է� ��� ���� �� ����
    {
        return lineText;
    }

    private void UpdateArray(string currentArrayNum)//currentArrayNum�� ���� ���ο� ADFGVX�迭�� �ε��ؼ� Array�� ������Ʈ
    {
        string FilePath = "Assets/Workspace_LeeJungWoo/Prefab/ADFGVX/ArrayTxt/Array_" + currentArrayNum + ".txt";
        FileInfo TxtFile = new FileInfo(FilePath);
        string value = "";

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
        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        adfgvx.beforeEncodingPart.AddInputField_DataByButton(array[row] + "" + array[line] + " ");
    }

    public void OnDecRowDown(int row)//Decoding Mode���� row�� ��ư�� ������ ��
    {
        decodeRow = row;

        for (int i = 0; i < 6; i++)//���� ���õ� RowButton ���� ���õǾ��� RowButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].ConvertClickSprite(rowButtons[i].Exit);
            }
        }

        if (decodeLine != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.afterDecodingPart.GetInputField_Data().AddInputFieldByButton(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].ConvertClickSprite(lineButtons[decodeLine].Exit);
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
        decodeLine = line;

        for (int i = 0; i < 6; i++)//���� ���õ� LineButton ���� ���õǾ��� LineButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (lineButtons[i].Selected == true && i != line)
            {
                lineButtons[i].Selected = false;
                lineButtons[i].ConvertClickSprite(lineButtons[i].Exit);
            }
        }
        if (decodeRow != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            adfgvx.afterDecodingPart.GetInputField_Data().AddInputFieldByButton(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            if (decodeLine != 6)
            {
                lineButtons[decodeLine].Selected = false;
                decodeLine = 6;
            }
            if (decodeRow != 6)
            {
                rowButtons[decodeRow].ConvertClickSprite(rowButtons[decodeRow].Exit);
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
}
