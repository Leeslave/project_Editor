using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ChiperPart : MonoBehaviour
{
    private TextMeshPro parttitle;              //��Ʈ Ÿ��Ʋ
    private TextMeshPro chipertitle;            //��ȣ�� ����
    private TextMeshPro chipertext;             //��ȣ�� ����
 
    
    private TextMeshPro inputfieldtext;         //��ȣ�� �˻�â
    private string inputstring;                 //�÷��̾ �˻�â�� �Է��� ����
    private SpriteRenderer inputfieldcolor;     //�˻�â ��� ��������Ʈ
    private bool isreadyforinput;               //�˻� �غ� �Ǿ� �ִ°�?
    private bool iscursoroverinput;             //�˻�â�� Ŀ���� �ö󰬴°�?

    private bool isflash;                       //������
 
    
    private void Awake()
    {
        parttitle = GetComponentsInChildren<TextMeshPro>()[0];
        chipertitle = GetComponentsInChildren<TextMeshPro>()[1];
        chipertext = GetComponentsInChildren<TextMeshPro>()[2];
        inputfieldtext = GetComponentsInChildren<TextMeshPro>()[3];
        inputfieldcolor = GetComponentsInChildren<SpriteRenderer>()[3];
        inputstring = "";
        ClearChiperTitleAndText();

        isflash = false;
        StartCoroutine("FlashInputField", 0.5f);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinput && !isreadyforinput)
            {
                isreadyforinput = true;
                inputfieldcolor.color = new Color(0, 1, 0, 0);
                inputfieldtext.text = inputstring + "_";
                isflash = true;
            }
            else if (iscursoroverinput && isreadyforinput)
                isreadyforinput = true;
            else
            {
                isreadyforinput = false;
                inputfieldtext.text = inputstring;
                isflash = false;
            }
                
        }
    }

    private void OnMouseEnter()
    {
        if(!isreadyforinput)
            inputfieldcolor.color = new Color(0, 1, 0, 0.1f);
        iscursoroverinput = true;
    }

    private void OnMouseExit()
    {
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        iscursoroverinput = false;
    }

    IEnumerator FlashInputField()//�˻�â�� �����̰� �����
    {
        if (inputstring.Length <= 16 && isreadyforinput)
        {
            if (isflash)
            {
                inputfieldtext.text = inputstring;
                isflash = false;
            }
            else
            {
                inputfieldtext.text = inputstring + "_";
                isflash = true;
            }
        }
        else if (inputstring == "")
            inputfieldtext.text = "Ŭ���Ͽ� �Է�...";
        else if (!isreadyforinput)
            inputfieldtext.text = inputstring;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField", 0.5f);
    }

    public void AddInputField(string value)//Ű���忡 �� �ܾ� �߰��Ѵ�
    {
        if (!isreadyforinput)
            return;

        inputstring = inputstring + value;
        inputfieldtext.text = inputstring;
    }

    public void DeleteInputField()//Ű���忡�� �� �ܾ� �����
    {
        if (!isreadyforinput)
            return;

        inputstring = inputstring.Substring(0, inputstring.Length - 1);
        inputfieldtext.text = inputstring;
    }

    public void UpdateChiperTitleAndText(int ChiperNum)
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";
        string TxtTitle = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ChipersTxt/Chiper_" + ChiperNum + ".txt";                //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)                                                                                     //FilePath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            TxtTitle = Reader.ReadLine();
            TxtValue = Reader.ReadToEnd();
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        chipertitle.text = TxtTitle;
        chipertext.text = TxtValue;
    }

    public void ClearChiperTitleAndText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
        GetComponentsInChildren<TextMeshPro>()[1].text = "";
    }
}
