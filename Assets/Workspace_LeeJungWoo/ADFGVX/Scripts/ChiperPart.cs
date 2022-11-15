using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class ChiperPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro parttitle;              //파트 타이틀
    private TextMeshPro chiperui;
    private TextMeshPro chipertitle;            //암호문 제목
    private TextMeshPro chiper;                 //암호문 내용
    private TextMeshPro inputfield;             //암호문 검색창
    private TextMeshPro dateui;                 //날짜 제목
    private TextMeshPro date;                   //날짜
    private TextMeshPro senderui;               //작성자 제목
    private TextMeshPro sender;                 //작성자

    private SpriteRenderer inputfieldcolor;     //검색창 배경 스프라이트

    private string inputstring;                 //플레이어가 검색창에 입력한 내용
    public bool isreadyforinput;                //검색 준비가 되어 있는가?
    private bool iscursoroverinputfield;        //검색창에 커서가 올라갔는가?
    private bool isonprintflow;

    private bool isflash;                       //깜박임
    private bool skiponeflash;                  //true면 깜박임 한번 건너뛴다

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        parttitle = GetComponentsInChildren<TextMeshPro>()[0];
        chiperui = GetComponentsInChildren<TextMeshPro>()[1];
        chipertitle = GetComponentsInChildren<TextMeshPro>()[2];
        chiper = GetComponentsInChildren<TextMeshPro>()[3];
        inputfield = GetComponentsInChildren<TextMeshPro>()[4];
        dateui = GetComponentsInChildren<TextMeshPro>()[5];
        date = GetComponentsInChildren<TextMeshPro>()[6];
        senderui = GetComponentsInChildren<TextMeshPro>()[7];
        sender = GetComponentsInChildren<TextMeshPro>()[8];

        inputfieldcolor = GetComponentsInChildren<SpriteRenderer>()[0];
        inputfieldcolor.color = new Color(0, 1, 0, 0);
        inputstring = "";
        isflash = false;

        ClearChiperAll();
        InitializeChiperAll();
        StartCoroutine("FlashInputField");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                inputfieldcolor.color = new Color(0, 1, 0, 0);
                inputfield.text = inputstring + "…";
                isreadyforinput = true;
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (inputstring == "")
                    inputfield.text = "클릭하여 입력…";
                isreadyforinput = false;
                isflash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if(!isreadyforinput)
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
                inputfield.text = inputstring;
                isflash = false;
            }
            else
            {
                inputfield.text = inputstring + "…";
                isflash = true;
            }
        }
        else if (!isreadyforinput && inputstring != "")                             //입력 중이 아니나 빈칸이 아니라면, 입력 상태를 유지한다
            inputfield.text = inputstring;

        skiponeflash = !skiponeflash ? false : false;                               //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashInputField");
    }

    private void DelayFlashInputField()//깜박임을 0.5초 막는다
    {
        skiponeflash = true;
    }

    public void AddInputField(string value)//입력창에 한 단어 추가한다
    {
        if (!isreadyforinput)
            return;
        else if (inputstring.Length > 16)
        {
            adfgvx.UpdateInfoBox("파일 경로 최대 입력 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }
        DelayFlashInputField();
        inputstring = inputstring + value;
        inputfield.text = inputstring;
    }

    public void DeleteInputField()//입력창에서 한 단어 지운다
    {
        if (!isreadyforinput)
            return;
        else if(inputstring.Length < 1)
        {
            adfgvx.UpdateInfoBox("파일 경로 삭제 불가 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }
        DelayFlashInputField();
        inputstring = inputstring.Substring(0, inputstring.Length - 1);
        inputfield.text = inputstring;
    }

    public void UpdateChiperTitleAndText()
    {
        if (isonprintflow)          //아직 전에 명령받은 파일 불러오기 작업이 끝나지 않음
        {
            adfgvx.UpdateInfoBox("파일 불러오기 불가: 작업 진행 중");
            return;
        }
        else if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            adfgvx.UpdateInfoBox("파일 불러오기 불가: 현재 모드 암호화");
            return;
        }

        //return 키를 눌렀으니 검색창 선택과 깜박임을 비활성화한다
        isreadyforinput = false;
        isflash = false;

        string FilePath = "";
        FileInfo TxtFile = null;
        string Txtchiperui = "";
        string Txtchipertitle = "";
        string Txtchiper = "";
        string Txtdateui = "";
        string Txtdate = "";
        string Txtsenderui = "";
        string Txtsender = "";

        //ArrayNum에 따라서 각기 다른 표의 FilePath가 저장된다
        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ChipersTxt/" + inputstring + ".txt";                  
        TxtFile = new FileInfo(FilePath);
        
        if (TxtFile.Exists)//Filepath가 유효하다면
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            Txtchiperui = Reader.ReadLine();
            Txtchipertitle = Reader.ReadLine();
            Txtchiper = Reader.ReadLine();
            Txtdateui = Reader.ReadLine();
            Txtdate = Reader.ReadLine();
            Txtsenderui = Reader.ReadLine();
            Txtsender = Reader.ReadLine();
            Reader.Close();
        }
        else//Filepath가 유효하지 않다면
        {
            adfgvx.UpdateInfoBox("파일 불러오기 불가: 유효하지 않은 경로");
            DisplayError("파일 접근 불가!");
            return;
        }

        adfgvx.UpdateInfoBox("파일 불러오기 성공");

        //새로운 암호문을 불러오기에 앞서 이미 불러와져 있던 것을 비운다
        ClearChiperAll();

        //1차원 흐름 출력 시작
        StartCoroutine(printFlow(chiperui, Txtchiperui, 0, 2.0f));
        StartCoroutine(printFlow(chipertitle, Txtchipertitle, 0, 2.0f));
        StartCoroutine(printFlow(chiper, Txtchiper, 0, 2.0f));
        StartCoroutine(printFlow(dateui, Txtdateui, 0, 2.0f));
        StartCoroutine(printFlow(date, Txtdate, 0, 2.0f));
        StartCoroutine(printFlow(senderui, Txtsenderui, 0, 2.0f));
        StartCoroutine(printFlow(sender, Txtsender, 0, 2.0f));

        //작업 종료 시까지 깜박임, 새로운 작업 차단
        isonprintflow = true;
        Invoke("SetisonprintflowFalse", 2.0f);

        adfgvx.intermediatepart.ClearIntermediateChiperAll();

        //intermediatechiper에 연결
        StartCoroutine(printFlow(adfgvx.intermediatepart.chiperui, Txtchiperui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.chipertitle, Txtchipertitle, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.dateui, Txtdateui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.date, Txtdate, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.senderui, Txtsenderui, 0, 2.0f));
        StartCoroutine(printFlow(adfgvx.intermediatepart.sender, Txtsender, 0, 2.0f));
    }

    private void DisplayError(string value)//검색창에 에러 메세지를 띄운다
    {
        inputfield.text = value;
        inputstring = "";
        isreadyforinput = false;
        isflash = false;
    }

    public void ClearChiperAll()//불러와져 있던 암호문을 비운다
    {
        chiperui.text = "";
        chipertitle.text = "";
        chiper.text = "";
        dateui.text = "";
        date.text = "";
        senderui.text = "";
        sender.text = "";
    }

    public string GetChiperText()//불라와져 있던 암호문을 반환한다
    {
        return chiper.text;
    }

    public void InitializeChiperAll()
    {
        if (adfgvx.currentmode == ADFGVX.mode.Encoding)
        {
            parttitle.text = "암호화 데이터 저장";
            if (chiperui.text == "")
                chiperui.text = "[보안 등급]";
            if (chipertitle.text == "")
                chipertitle.text = "[파일의 제목]";
            if (chiper.text == "")
                chiper.text = "[파일의 내용]";
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
            parttitle.text = "암호화 데이터 로드";
            if (chiperui.text == "")
                chiperui.text = "[보안 등급]";
            if (chipertitle.text == "")
                chipertitle.text = "[파일의 제목]";
            if (chiper.text == "")
                chiper.text = "[파일의 내용]";
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

    private IEnumerator printFlow(TextMeshPro target, string value, int idx, float endtime)//tartget에 value를 endtime안에 순차적으로 채워 넣는다
    {
        if (idx >= value.Length)
            yield break;
        target.text += value.Substring(idx,1);
        yield return new WaitForSeconds(endtime/value.Length);
        StartCoroutine(printFlow(target,value,idx+1,endtime));
    }

    private void SetisonprintflowFalse()//isonprintflow변수를 거짓으로 한다_Invoke용
    {
        isonprintflow = false;
    }

    public void ReturnEncodingResult()//이번 암호화 결과물을 제출합니다
    {
        if(adfgvx.currentmode == ADFGVX.mode.Decoding)
        {
            adfgvx.UpdateInfoBox("파일 저장하기 불가: 현재 모드 복호화");
            return;
        }
        Debug.Log("암호화 결과물을 제출합시다!");
    }
}
