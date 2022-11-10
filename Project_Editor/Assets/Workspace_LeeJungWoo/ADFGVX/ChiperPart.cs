using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ChiperPart : MonoBehaviour
{
    private TextMeshPro parttitle;              //파트 타이틀
    private TextMeshPro chipertitle;            //암호문 제목
    private TextMeshPro chipertext;             //암호문 내용
 
    
    private TextMeshPro inputfieldtext;         //암호문 검색창
    private string inputstring;                 //플레이어가 검색창에 입력한 내용
    private SpriteRenderer inputfieldcolor;     //검색창 배경 스프라이트
    private bool isreadyforinput;               //검색 준비가 되어 있는가?
    private bool iscursoroverinput;             //검색창에 커서가 올라갔는가?

    private bool isflash;                       //깜박임
 
    
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

    IEnumerator FlashInputField()//검색창을 깜박이게 만든다
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
            inputfieldtext.text = "클릭하여 입력...";
        else if (!isreadyforinput)
            inputfieldtext.text = inputstring;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField", 0.5f);
    }

    public void AddInputField(string value)//키워드에 한 단어 추가한다
    {
        if (!isreadyforinput)
            return;

        inputstring = inputstring + value;
        inputfieldtext.text = inputstring;
    }

    public void DeleteInputField()//키워드에서 한 단어 지운다
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

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ChipersTxt/Chiper_" + ChiperNum + ".txt";                //ArrayNum에 따라서 각기 다른 표의 FilePath가 저장
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)                                                                                     //FilePath가 유효하다면
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
