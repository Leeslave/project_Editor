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

    [Header("ADFGVX ������Ʈ ��ư")]
    public Button_ADFGVX_Element[] elementButtons = new Button_ADFGVX_Element[36];
    [Header("ADFGVX �� ��ư")]
    public Button_ADFGVX_Row[] rowButtons = new Button_ADFGVX_Row[6];
    [Header("ADFGVX �� ��ư")]
    public Button_ADFGVX_Line[] lineButtons = new Button_ADFGVX_Line[6];

    private TextMeshPro rowText;
    private TextMeshPro lineText;
    private TextMeshPro arrayNumText;

    private int currentArrayNum;
    private const int ArrayNum_MAX = 4;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        rowText = GetComponentsInChildren<TextMeshPro>()[1];
        lineText = GetComponentsInChildren<TextMeshPro>()[2];
        arrayNumText = GetComponentsInChildren<TextMeshPro>()[3];

        decodeRow = 6;
        decodeLine = 6;

        currentArrayNum = 0;
        UpdateADFGVXArray();
    }

    public void SetLayer(int layer)//�� ���ӿ�����Ʈ ���� ����� ���̾� ����
    {
        GameObject arrayKeyboard = transform.Find("ArrayKeyboard").gameObject;
        for(int i=0;i<6;i++)
        {
            arrayKeyboard.transform.Find("Line (" + i.ToString() + ")").gameObject.layer = layer;
            arrayKeyboard.transform.Find("Row (" + i.ToString() + ")").gameObject.layer = layer;
        }
        for(int i=0;i<36;i++)
        {
            arrayKeyboard.transform.Find("Element (" + i.ToString() + ")").gameObject.layer = layer;
        }
        transform.Find("Delete").gameObject.layer = layer;
        transform.Find("Clear").gameObject.layer = layer;
        transform.Find("ArrayMinus").gameObject.layer = layer;
        transform.Find("ArrayPlus").gameObject.layer = layer;
    }

    public TextMeshPro GetRowText()//�Է� ��� ���� �� ����
    {
        return rowText;
    }

    public TextMeshPro GetLineText()//�Է� ��� ���� �� ����
    {
        return lineText;
    }

    public int GetCurrentArrayNum()//���� ADFGVX �迭 ��ȣ ��ȯ
    {
        return currentArrayNum;
    }
    
    public void OnEncElementDown(int row, int line)//Encoding Mode���� 6x6ǥ�� ��ư�� ������ ��
    {
        //Ʃ�丮�� ���� �ڵ�
        if(adfgvx.GetCurrentTutorialPhase() == 0 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.biliteralsubstitutionpart.currentArrayNum != 0)
            {
                adfgvx.DisplayTutorialDialog(44, 0f);
                return;
            }
            else
                adfgvx.MoveToNextTutorialPhase(2.0f);
        }

        char[] array = new char[6] { 'A', 'D', 'F', 'G', 'V', 'X' };
        adfgvx.beforeEncodingPart.GetInputField_Data().AddInputField(array[row].ToString() + array[line].ToString() + " ");
    }

    public void OnDecRowDown(int row)//Decoding Mode���� row�� ��ư�� ������ ��
    {
        decodeRow = row;

        for (int i = 0; i < 6; i++)//���� ���õ� RowButton ���� ���õǾ��� RowButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (rowButtons[i].Selected == true && i != row)
            {
                rowButtons[i].Selected = false;
                rowButtons[i].ConvertClickSpriteColor(rowButtons[i].Exit);
            }
        }

        if (decodeLine != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            rowButtons[decodeRow].Selected = false;
            lineButtons[decodeLine].Selected = false;
            lineButtons[decodeLine].ConvertClickSpriteColor(lineButtons[decodeLine].Exit);

            //Ʃ�丮�� ���� �ڵ�
            if (adfgvx.GetCurrentTutorialPhase() == 8 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.DisplayTutorialDialog(144, 0f);
                decodeRow = 6;
                decodeLine = 6;
                return;
            }
            if (adfgvx.GetCurrentTutorialPhase() == 7 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 1 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(133, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 6 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 2 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(127, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 5 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 5 && decodeLine == 4 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(121, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }

            adfgvx.afterDecodingPart.GetInputField_Data().AddInputField(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            decodeRow = 6;
            decodeLine = 6;
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
                lineButtons[i].ConvertClickSpriteColor(lineButtons[i].Exit);
            }
        }

        if (decodeRow != 6)//RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            lineButtons[decodeLine].Selected = false;
            rowButtons[decodeRow].Selected = false;
            rowButtons[decodeRow].ConvertClickSpriteColor(rowButtons[decodeRow].Exit);

            //Ʃ�丮�� ���� �ڵ�
            if (adfgvx.GetCurrentTutorialPhase() == 8 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                adfgvx.DisplayTutorialDialog(144, 0f);
                decodeRow = 6;
                decodeLine = 6;
                return;
            }
            if (adfgvx.GetCurrentTutorialPhase() == 7 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 1 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(133, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 6 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 0 && decodeLine == 2 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(127, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }
            if (adfgvx.GetCurrentTutorialPhase() == 5 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (decodeRow == 5 && decodeLine == 4 && currentArrayNum == 0)
                    adfgvx.MoveToNextTutorialPhase(2.0f);
                else if (currentArrayNum != 0)
                {
                    adfgvx.DisplayTutorialDialog(147, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
                else
                {
                    adfgvx.DisplayTutorialDialog(121, 0f);
                    decodeRow = 6;
                    decodeLine = 6;
                    return;
                }
            }

            adfgvx.afterDecodingPart.GetInputField_Data().AddInputField(elementButtons[decodeRow * 6 + decodeLine].GetButtonText() + " ");
            decodeLine = 6;
            decodeRow = 6;
        }
    }

    private void UpdateADFGVXArray()//currentArrayNum�� ���� ���ο� ADFGVX �迭�� �ε��ؼ� Array�� ������Ʈ
    {
        string FilePath = "Assets/Resources/Text/Array_" + currentArrayNum + ".txt";
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

    public void ArrayPlus()//ADFGVX�迭 +1�� ��ȯ
    {
        //Ʃ�丮�� ���� �ڵ�
        if (adfgvx.GetCurrentTutorialPhase()==4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentArrayNum++;
        currentArrayNum %= ArrayNum_MAX;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateADFGVXArray();
    }

    public void ArrayMinus()//ADFGVX�迭 -1�� ��ȯ
    {
        //Ʃ�丮�� ���� �ڵ�
        if (adfgvx.GetCurrentTutorialPhase() == 4 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            adfgvx.MoveToNextTutorialPhase(3.0f);
        }

        currentArrayNum--;
        if (currentArrayNum < 0)
            currentArrayNum = ArrayNum_MAX - 1;
        arrayNumText.text = "ADFGVX\nARRAY\nNo." + currentArrayNum.ToString();
        UpdateADFGVXArray();
    }
}
