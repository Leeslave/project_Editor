using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVXscript ADFGVX;

    //키워드 입력 창 선택
    private bool selected;
    private bool isover;

    //전치 암호 키워드와 그 문자 순위
    private string keyword;
    private int[] place;

    //입력창 깜박임
    private bool isflash;
    private bool onWork;

    //전치 작업
    int rowlength;
    int linelength;
    private string[] interline;
    private int FlowLine;

    private TextMeshPro[] lines;
    private TextMeshPro keywords;
    private TextMeshPro prioritys;

    private void Awake()
    {
        ADFGVX = GameObject.Find("ADFGVX_Script").GetComponent<ADFGVXscript>();

        lines = new TextMeshPro[9];
        interline = new string[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keywords = GetComponentsInChildren<TextMeshPro>()[9];
        prioritys = GetComponentsInChildren<TextMeshPro>()[10];

        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(0, 0);

        ClearKeyWord();
        ClearPriority();

        selected = false;
        isflash = false;
        StartCoroutine("FlashText", 0.5f);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isover && !selected)
                selected = true;
            else if (isover && selected)
                selected = true;
            else
                selected = false;
        }
    }

    private void OnMouseEnter()
    {
        isover = true;
    }

    private void OnMouseExit()
    {
        isover = false;
    }

    IEnumerator FlashText()//키워드 창을 깜박이게 만든다
    {
        if (keyword.Length <= 16 && selected && !onWork)
        {
            if (isflash)
            {
                keywords.text = keyword;
                isflash = false;
            }
            else
            {
                keywords.text = keyword + " _";
                isflash = true;
            }
        }
        else if (keyword == "")
            keywords.text = " 클릭하여 입력...";
        else if(!selected)
            keywords.text = keyword;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    public void AddKeyWord(string value)//키워드에 한 단어 추가한다
    {
        if(!selected)
            return;
        else if(keyword.Length>16)
        {
            ADFGVX.UpdateInfoBox("암호 키 최대 입력 재확인 요망");
            ADFGVX.InformCurrentMode();
            return;
        }

        keyword = keyword + " " + value;
        keywords.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//키워드에서 한 단어 지운다
    {
        if (!selected)
            return;
        else if(keyword.Length<2)
        {
            ADFGVX.UpdateInfoBox("암호문 삭제 불가 재확인 요망");
            ADFGVX.InformCurrentMode();
            return;
        }

        keyword = keyword.Substring(0, keyword.Length - 2);
        keywords.text = keyword;
        UpdatePriority();
    }

    private void ClearKeyWord()//키워드를 비운다
    {
        keyword = "";
        keywords.text = keyword;
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
        
        prioritys.text = Priority;
    }

    private void ClearPriority()//Priority를 비운다
    {
        prioritys.text = "";
    }

    private string CollectEnglishAlphabet(string value)//빈칸, 숫자 등을 제외하고 영어 알파벳만 모아서 반환한다
    {
        //array0에 들어있는 알파벳 개수 확인, 새롭게 만들어질 array1의 길이 확인
        int newarraylenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z')
                newarraylenght++;
        }

        //array0의 알파벳 개수 만큼 array 할당
        char[] array = new char[newarraylenght];
        int idx = 0;

        //array01에 idx를 늘려가면서 알파벳 전부 저장
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z')
            {
                array[idx] = value[i];
                idx++;
            }
        }
        return array.ArrayToString();
    }

    public void OnTransposeDown()//EncodedChiper를 들고와서 Transposition에 채워 넣는다
    {
        //에러 발생
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 작업 진행 중");
            return;
        }
        else if (keyword.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 암호 키 공백");
            return;
        }
        else if(GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text == "")
        {
            ADFGVX.InfoBox.UpdateText("전치 불가: 전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(GameObject.Find("EncodedChipertext").GetComponentsInChildren<TextMeshPro>()[1].text);
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
                    interline[j] = Chiper.Substring(0, rowlength);
                    Chiper = Chiper.Substring(rowlength);
                }
            }
        }

        ResizeAndRePositionEdge();
        printFlow();
    }

    public void OnTransposeReverseDown()//intermediateChiper를 들고와서 Transposition에 채워 넣는다
    {
        //에러 발생
        if (onWork == true)
        {
            ADFGVX.InfoBox.UpdateText("역전치 불가: 작업 진행 중");
            return;
        }
        else if (keyword.Length == 0)
        {
            ADFGVX.InfoBox.UpdateText("역전치 불가: 암호 키 공백");
            return;
        }
        else if(ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text == "")
        {
            ADFGVX.InfoBox.UpdateText("역전치 불가: 역전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(ADFGVX.InterChiperBox.GetComponentInChildren<TextMeshPro>().text);
        linelength = place.Length;
        rowlength = Chiper.Length / place.Length;

        //한 글자 씩 돌아가면서 채운다
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            interline[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();
    }

    private void ClearTransposition()//Transposition을 비운다
    {
        for (int i = 0; i < 9; i++)
        {
            interline[i] = "";
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

    private void printFlow()//흐름 출력
    {
        //흐름 출력 개시
        FlowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.2f);
        //깜박임 차단, 적절한 시간 후에 깜박임 회복
        keywords.text = keyword;
        onWork = true;
        Invoke("SetonWorkFalse", 0.2f * (rowlength + linelength));
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
        lines[FlowLine].text += interline[FlowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(printFlowRow(FlowLine, FlowRow + 1));
    }

    private void SetonWorkFalse()//onWork변수를 거짓으로 한다_Invoke용
    {
        onWork = false;
    }
}