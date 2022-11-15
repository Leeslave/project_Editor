using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntermediatePart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro parttitle;
    private TextMeshPro fileroute;
    public TextMeshPro chiperui;
    public TextMeshPro chipertitle;
    private TextMeshPro chiper;
    public TextMeshPro dateui;
    public TextMeshPro date;
    public TextMeshPro senderui;
    public TextMeshPro sender;

    private SpriteRenderer inputfieldcolor;
    private string inputstring;
    private bool iscursoroverinputfield;
    public bool isreadyforinput;
    private bool isflash;
    private bool skiponeflash;

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        parttitle = GetComponentsInChildren<TextMeshPro>()[0];
        fileroute = GetComponentsInChildren<TextMeshPro>()[1];
        chiperui = GetComponentsInChildren<TextMeshPro>()[2];
        chipertitle = GetComponentsInChildren<TextMeshPro>()[3];
        chiper = GetComponentsInChildren<TextMeshPro>()[4];
        dateui = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderui = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputfieldcolor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        inputstring = "";
        isflash = false;

        ClearIntermediateChiperAll();
        InitializeIntermediateChiperAll();
        StartCoroutine("FlashInputField");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                inputfieldcolor.color = new Color(0, 1, 0, 0);
                chiper.text = inputstring + "…";
                isreadyforinput = true;
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (inputstring == "")
                    chiper.text = "클릭하여 입력…";
                isreadyforinput = false;
                isflash = false;
            }
        }

        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
            fileroute.text = "경로: 암호화 데이터";
        else
            fileroute.text = "경로: 복호화 데이터";
    }

    private void OnMouseEnter()
    {
        if (!isreadyforinput)
            inputfieldcolor.color = new Color(0, 1, 0, 0.15f);
        iscursoroverinputfield = true;
    }

    private void OnMouseExit()
    {
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        iscursoroverinputfield = false;
    }

    IEnumerator FlashInputField()//검색창을 깜박이게 만든다
    {
        if (inputstring.Length <= 16 && isreadyforinput && !skiponeflash)           //입력창 길이를 넘기거나, 입력 중이 아니거나, 스킵 명령이 있다면 건너뛴다
        {
            if (isflash)
            {
                chiper.text = inputstring;
                isflash = false;
            }
            else
            {
                chiper.text = inputstring + "…";
                isflash = true;
            }
        }
        else if (!isreadyforinput && inputstring != "")                             //입력 중이 아니나 빈칸이 아니라면, 입력 상태를 유지한다
            chiper.text = inputstring;

        skiponeflash = !skiponeflash ? false : false;                               //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField");
    }

    private void DelayFlashInputField()//깜박임을 0.5초 막는다
    {
        skiponeflash = true;
    }

    public void AddIntermediateChiper(string value)
    {
        inputstring += value;
        chiper.text = inputstring;
        skiponeflash = true;
        return;
    }

    public void ClearIntermediateChiper()
    {
        chiper.text = "";
        return;
    }

    public void DeleteText()
    {
        if (!isreadyforinput)
            return;

        string text = inputstring;
        int DeleteLength = adfgvx.currentmode == ADFGVX.mode.Decoding ? 2 : 3;
        if (text.Length >= DeleteLength)
        {
            inputstring = text.Substring(0, text.Length - DeleteLength);
            chiper.text = inputstring;
            skiponeflash = true;
        }
        else
        {
            adfgvx.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            adfgvx.InformCurrentMode();
        }
    }

    public string GetIntermediateChiper()
    {
        return inputstring;
    }

    public void ClearIntermediateChiperAll()
    {
        inputstring = "";
        chiperui.text = "";
        chipertitle.text = "";
        chiper.text = "";
        dateui.text = "";
        date.text = "";
        senderui.text = "";
        sender.text = "";
    }

    public void InitializeIntermediateChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            parttitle.text = "암호화 디스플레이";
            if (chiperui.text == "")
                chiperui.text = "[보안 등급]";
            if (chipertitle.text == "")
                chipertitle.text = "[파일의 제목]";
            if (chiper.text == "")
                chiper.text = "[암호화 내용]";
            if (dateui.text == "")
                dateui.text = "[작성일]";
            if (date.text == "")
                date.text = "…";
            if (senderui.text == "")
                senderui.text = "[작성자]";
            if (sender.text == "")
                sender.text = "…";
        }
        else
        {
            parttitle.text = "복호화 디스플레이";
            if (chiperui.text == "")
                chiperui.text = "[보안 등급]";
            if (chipertitle.text == "")
                chipertitle.text = "[파일의 제목]";
            if (chiper.text == "")
                chiper.text = "클릭하여 입력…";
            if (dateui.text == "")
                dateui.text = "[작성일]";
            if (date.text == "")
                date.text = "…";
            if (senderui.text == "")
                senderui.text = "[작성자]";
            if (sender.text == "")
                sender.text = "…";
        }
    }
}
