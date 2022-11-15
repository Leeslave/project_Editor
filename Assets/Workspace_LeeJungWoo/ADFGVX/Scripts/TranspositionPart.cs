using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro[] lines;
    private TextMeshPro keywordfieldtext;
    private TextMeshPro priority;
    private SpriteRenderer transposedlineguidebox;
    private SpriteRenderer keywordfieldcolor;

    private bool isflash;                               //깜박임
    private bool skiponeflash;                          //true면 깜박임 한번 건너뛴다

    private bool isonprintflow;                         //흐름 출력 작업 중인가? 작업 중이면 전치 버튼과 역전치 버튼을 누를 수 없다
    public bool isreadyforinput;                        //키워드 입력 가능한 상태인가?
    private bool iscursoroverinputfield;                //입력 필드에 커서가 올라가 있는가?

    //전치, 역전치 때 사용하는 변수들
    private string keyword;                             //입력 받은 키워드
    private int[] place;                                //키워드 문자의 순위
    int rowlength;                                      //전치 행렬의 행 길이
    int linelength;                                     //전치 행렬의 열 길이
    private string[] templine;                          //흐름 출력 전, 잠시 저장해놓는다
    private int FlowLine;                               //흐름 출력 인덱스

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keywordfieldtext = GetComponentsInChildren<TextMeshPro>()[9];
        priority = GetComponentsInChildren<TextMeshPro>()[10];
        transposedlineguidebox = GetComponentsInChildren<SpriteRenderer>()[0];
        keywordfieldcolor = GetComponentsInChildren<SpriteRenderer>()[4];

        transposedlineguidebox.size = new Vector2(0, 0);
        keywordfieldcolor.color = new Color(0, 1, 0, 0);
        templine = new string[9];
        isreadyforinput = false;
        isflash = false;

        ClearKeyWord();
        ClearPriority();
        StartCoroutine("FlashText");
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (iscursoroverinputfield && !isreadyforinput)
            {
                isreadyforinput = true;
                keywordfieldcolor.color = new Color(0, 1, 0, 0);
                keywordfieldtext.text = keyword + " …";
                isflash = true;
            }
            else if (iscursoroverinputfield && isreadyforinput)
                isreadyforinput = true;
            else
            {
                if (keyword == "")
                    keywordfieldtext.text = " 클릭하여 입력…";
                isreadyforinput = false;
                isflash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!isreadyforinput)
            keywordfieldcolor.color = new Color(0, 1, 0, 0.15f);
        iscursoroverinputfield = true;
    }

    private void OnMouseExit()
    {
        keywordfieldcolor.color = new Color(0, 1, 0, 0);
        iscursoroverinputfield = false;
    }

    IEnumerator FlashText()//키워드 창을 깜박이게 만든다
    {
        if (keyword.Length <= 16 && isreadyforinput && !isonprintflow && !skiponeflash)
        {
            if (isflash)
            {
                keywordfieldtext.text = keyword;
                isflash = false;
            }
            else
            {
                keywordfieldtext.text = keyword + " …";
                isflash = true;
            }
        }
        else if (!isreadyforinput && keyword == "")
            keywordfieldtext.text = " 클릭하여 입력…";
        else if(!isreadyforinput && keyword != "")
            keywordfieldtext.text = keyword;

        skiponeflash = !skiponeflash ? false : false;                               //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void DelayFlashInputField()//깜박임을 0.5초 막는다
    {
        skiponeflash = true;
    }

    public void AddKeyWord(string value)//키워드에 한 단어 추가한다
    {
        if(!isreadyforinput)
            return;
        else if(keyword.Length>16)
        {
            adfgvx.UpdateInfoBox("암호 키 최대 입력 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }

        DelayFlashInputField();
        keyword = keyword + " " + value;
        keywordfieldtext.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//키워드에서 한 단어 지운다
    {
        if (!isreadyforinput)
            return;
        else if(keyword.Length<2)
        {
            adfgvx.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            adfgvx.InformCurrentMode();
            return;
        }

        DelayFlashInputField();
        keyword = keyword.Substring(0, keyword.Length - 2);
        keywordfieldtext.text = keyword;
        UpdatePriority();
    }

    public void ClearKeyWord()//키워드를 비운다
    {
        keyword = "";
        keywordfieldtext.text = keyword;
    }

    private void UpdatePriority()//Priority를 새로 구한다
    {
        string Priority = "";
        string value = CollectEnglishAlphabet(keyword);
        place = new int[value.Length];

        for(int i = 0; i < value.Length; i++)
        {
            place[i] = 1;
        }

        for (int i = 0; i < value.Length; i++)
        {
            for (int j = 0; j < value.Length; j++)
            {
                if(i!=j)
                {
                    if (value[i] > value[j])
                        place[i]++;
                    if (value[i] == value[j] && i < j)
                        place[j]++;
                }
            }
        }

        for (int i = 0; i < keyword.Length / 2; i++)
        {
            Priority += " ";
            Priority += place[i].ToString();
        }

        ClearTransposition();

        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (place.Length - 1));
        float size_x = 2.5f * place.Length;
        float size_y = 2.5f;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -4, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
        
        priority.text = Priority;
    }

    public void ClearPriority()//Priority를 비운다
    {
        priority.text = "";
    }

    private string CollectEnglishAlphabet(string value)//빈칸, 숫자 등을 제외하고 영어 알파벳만 모아서 반환한다
    {
        //array0에 들어있는 알파벳 개수 확인, 새롭게 만들어질 array1의 길이 확인
        int newarraylenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newarraylenght++;
        }

        //array0의 알파벳 개수 만큼 array 할당
        char[] array = new char[newarraylenght];
        int idx = 0;

        //array01에 idx를 늘려가면서 알파벳 전부 저장
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
            {
                array[idx] = value[i];
                idx++;
            }
        }
        return array.ArrayToString();
    }

    public void OnTransposeDown()//EncodedChiper를 들고와서 Transposition에 채워 넣는다
    {
        if (!isreadyforinput)
            return;
        //에러 발생
        if (isonprintflow == true)
        {
            adfgvx.InfoBox.UpdateText("전치 불가: 작업 진행 중");
            return;
        }
        else if (keyword.Length == 0)
        {
            adfgvx.InfoBox.UpdateText("전치 불가: 암호 키 공백");
            return;
        }
        else if(adfgvx.chiperpart.GetChiperText()=="")
        {
            adfgvx.InfoBox.UpdateText("전치 불가: 전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.chiperpart.GetChiperText());
        int InputPriority = 1;
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;
        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    templine[j] = Chiper.Substring(0, rowlength);
                    Chiper = Chiper.Substring(rowlength);
                }
            }
        }

        ResizeAndRePositionEdge();
        printFlow();

        isreadyforinput = false;
        isflash = false;
    }

    public void OnTransposeReverseDown()//IntermediateChiper를 들고와서 Transposition에 채워 넣는다
    {
        if (!isreadyforinput)
            return;

        //에러 발생
        if (isonprintflow == true)
        {
            adfgvx.InfoBox.UpdateText("역전치 불가: 작업 진행 중");
            return;
        }
        else if (keyword.Length == 0)
        {
            adfgvx.InfoBox.UpdateText("역전치 불가: 암호 키 공백");
            return;
        }
        else if(adfgvx.intermediatepart.GetIntermediateChiper() == "")
        {
            adfgvx.InfoBox.UpdateText("역전치 불가: 역전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.intermediatepart.GetIntermediateChiper());
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;

        //한 글자 씩 돌아가면서 채운다
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            templine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();

        isreadyforinput = false;
        isflash = false;
    }

    public void ClearTransposition()//Transposition을 비운다
    {
        for (int i = 0; i < 9; i++)
        {
            templine[i] = "";
            lines[i].text = "";
        }
    }

    private void ResizeAndRePositionEdge()//Transposition의 가이드 라인 사이즈를 행과 열 크기에 맞춰서 사이즈를 조정하고 재배치한다
    {
        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (linelength - 1));
        float size_x = 2.5f * linelength;
        float size_y = 2.5f * rowlength;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -4, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
    }

    private void printFlow()//2차원 흐름 출력
    {
        //흐름 출력 개시
        FlowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.1f);
        //깜박임 차단, 적절한 시간 후에 깜박임 회복
        keywordfieldtext.text = keyword;
        isonprintflow = true;
        Invoke("SetisonprintflowFalse", 0.1f * (rowlength + linelength));
    }

    private void printFlowLine()//흐름 출력 오른쪽
    {
        if (FlowLine >= linelength)
        {
            CancelInvoke("printFlowLine");
            return;
        }
        StartCoroutine(printFlowRow(FlowLine++, 0));
    }

    private IEnumerator printFlowRow(int FlowLine, int FlowRow)//흐름 출력 아래쪽
    {
        if (FlowRow >= rowlength)
            yield break;
        lines[FlowLine].text += templine[FlowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(FlowLine, FlowRow + 1));
    }

    private void SetisonprintflowFalse()//isonprintflow변수를 거짓으로 한다_Invoke용
    {
        isonprintflow = false;
    }
}