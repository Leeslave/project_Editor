using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVXscript : MonoBehaviour
{
    private char[,] Array = new char[6, 6];
    private int ArrayNum = 0;

    public enum Codemode//ADFGVX���
    { Encoding,Decoding};
    public Codemode CurrentCodemode;//���� ADFGVX���

    public struct DecodeElemnt//��ȣȭ ������Ʈ
    {
        public int row;
        public int line;
    }
    public DecodeElemnt CurrentDecodeElement;//���� ��ȣȭ ������Ʈ


    public ElementButton[] ElementButtons = new ElementButton[36];
    public RowButton[] RowButtons = new RowButton[6];
    public LineButton[] LineButtons = new LineButton[6];


    public Mode ModeBox;
    public InterChiper InterChiperBox;
    public Info InfoBox;
    public ArrayInfo ArrayInfoBox;
    public ArraySelect ArraySelectBox;
   

    private void Start()
    {
        ArrayNum = 0;
        UpdateArray(ArrayNum.ToString());
        CurrentDecodeElement.row = 6;
        CurrentDecodeElement.line = 6;
        InterChiperBox.ClearText();
        UpdateInfoBox("ADFGVX ���̺��� �����Ͻʽÿ�.");
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {

        }
    }
    
    private void UpdateArray(string ArrayNum)//ArrayNum�� ���� ���ο� ADFGVX�迭�� �ε��ؼ� Array�� ������Ʈ
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";

        
        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ADFGVXArrayTxt/Array_" + ArrayNum + ".txt";              //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����˴ϴ�
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)                                                                                 //FilePath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath);
            TxtValue = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        
        
        char[] Txt = new char[36];                                                                          
        Txt = TxtValue.ToCharArray();                                                                       //36ũ���� ������ �迭�� String���� ADFGVXǥ�� ��ȯ�ؼ� �ֽ��ϴ�
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Array[i, j] = Txt[i * 6 + j];
                ElementButtons[i * 6 + j].ChangeButtonText(Txt[i * 6 + j]);                                 //��� public ElementButtons�� �����ϸ鼭 ��ư�� �ؽ�Ʈ�� ADFGVXǥ�� ����� �����մϴ�
            }
        }
    }

    public void OnModeDown()//��� ��ȯ ��ư�� ������ ��
    {
        if (CurrentCodemode == Codemode.Encoding)
        {
            CurrentCodemode = Codemode.Decoding;
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
            UpdateModeBox("D", "e");
            ClearInterChiperBox();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ElementButtons[i * 6 + j].ShiftMode();
                }
            }
        }
        else if (CurrentCodemode == Codemode.Decoding)
        {
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].DisableClickSprite();
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].DisableClickSprite();
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
            CurrentCodemode = Codemode.Encoding;
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
            UpdateModeBox("E", "n");
            ClearInterChiperBox();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ElementButtons[i * 6 + j].ShiftMode();
                }
            }
        }
        InformCurrentMode();
    }

    public void OnArraySelectDown()//�迭 ��ư�� ������ ��
    {
        ArrayNum++;
        ArrayNum%=4;
        UpdateArray(ArrayNum.ToString());
        ArrayInfoBox.UpdateText("��ȣ�迭\n* No." + ArrayNum.ToString() + " *");
    }

    public void OnClearDown()//Clear ��ư�� ������ ��
    {
        UpdateInfoBox("�߰� ��ȣ ���� ���");
        ClearInterChiperBox();
        InformCurrentMode();
    }

    public void OnDeleteDown()//Delete ��ư�� ������ ��
    {
        DeleteInterChiperBox();
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
        CurrentDecodeElement.row = row;

        for (int i = 0; i < 6; i++)                                                                     //���� ���õ� RowButton ���� ���õǾ��� RowButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (RowButtons[i].Selected == true && i != row)
            {
                RowButtons[i].Selected = false;
                RowButtons[i].DisableClickSprite();
            }
        }

        if (CurrentDecodeElement.line != 6)                                                             //RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].DisableClickSprite();
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
        }
    }

    public void OnDecLineDown(int line)//Decodeing Mode���� line�� ��ư�� ������ ��
    {
        CurrentDecodeElement.line = line;

        for (int i = 0; i < 6; i++)                                                                     //���� ���õ� LineButton ���� ���õǾ��� LineButton�� ã�Ƽ� ClickSprite�� ��Ȱ��ȭ�մϴ�
        {
            if (LineButtons[i].Selected == true && i != line)
            {
                LineButtons[i].Selected = false;
                LineButtons[i].DisableClickSprite();
            }
        }

        if (CurrentDecodeElement.row != 6)                                                              //RowButton�� LineButton �� �� ���� �Ǿ����Ƿ�, Decoding ������ �����մϴ�
        {
            AddInterChiperBox(Array[CurrentDecodeElement.row, CurrentDecodeElement.line] + " ");
            if (CurrentDecodeElement.line != 6)
            {
                LineButtons[CurrentDecodeElement.line].Selected = false;
                CurrentDecodeElement.line = 6;
            }
            if (CurrentDecodeElement.row != 6)
            {
                RowButtons[CurrentDecodeElement.row].DisableClickSprite();
                RowButtons[CurrentDecodeElement.row].Selected = false;
                CurrentDecodeElement.row = 6;
            }
        }
    }


    private void UpdateModeBox(string Value1, string Value2)//ModeBox�� �ؽ�Ʈ�� En Ȥ�� De�� �ٲ� �� ����Ѵ�
    {
        ModeBox.UpdateText(Value1, Value2);
    }

    public void UpdateInfoBox(string Value)//InfoBox�� �ؽ�Ʈ�� Value�� �ٲ۴�
    {
        InfoBox.UpdateText(Value);
    }
    
    private void ClearInterChiperBox()//InterChipderBox�� ����
    {
        InterChiperBox.ClearText();
    }
    
    private void AddInterChiperBox(string Value)//InterChiperBox�� �ؽ�Ʈ�� �߰�
    {
        InterChiperBox.AddText(Value);
    }

    private void DeleteInterChiperBox()//InterChiperBox�� �ؽ�Ʈ �ϳ� ����
    {
        InterChiperBox.DeleteText();
    }
    
    public void InformCurrentMode()//1�� �Ŀ� ���� ��� ���
    {
        if (CurrentCodemode == Codemode.Encoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
        else if (CurrentCodemode == Codemode.Decoding)
            UpdateInfoBoxDelay(1, "��ȣȭ ���� ���� ��...");
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
