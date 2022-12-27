using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private TextMeshPro[] lines;
    private TextMeshPro keyWordFieldText;
    private TextMeshPro priority;
    private SpriteRenderer transposedLineGuideBox;
    private SpriteRenderer keyWordFieldColor;

    private bool isFlash;                               //깜박임
    private bool skipOneFlash;                          //true면 깜박임 한번 건너뛴다

    private bool isOnPrintFlow;                         //흐름 출력 작업 중인가? 작업 중이면 전치 버튼과 역전치 버튼을 누를 수 없다
    public bool isReadyForInput;                        //키워드 입력 가능한 상태인가?
    private bool isCursorOverInputField;                //입력 필드에 커서가 올라가 있는가?

    //전치, 역전치 때 사용하는 변수들
    private string keyword;                             //입력 받은 키워드
    private int[] place;                                //키워드 문자의 순위
    int rowLength;                                      //전치 행렬의 행 길이
    int lineLength;                                     //전치 행렬의 열 길이
    private string[] tempLine;                          //흐름 출력 전, 잠시 저장해놓는다
    private int flowLine;                               //흐름 출력 인덱스

    private void Awake()
    {
        adfgvx = GameObject.Find("ADFGVX").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }
        keyWordFieldText = GetComponentsInChildren<TextMeshPro>()[9];
        priority = GetComponentsInChildren<TextMeshPro>()[10];
        transposedLineGuideBox = GetComponentsInChildren<SpriteRenderer>()[0];
        keyWordFieldColor = GetComponentsInChildren<SpriteRenderer>()[4];

        transposedLineGuideBox.size = new Vector2(0, 0);
        keyWordFieldColor.color = new Color(0, 1, 0, 0);
        tempLine = new string[9];
        isReadyForInput = false;
        isFlash = false;

        ClearKeyWord();
        ClearPriority();
        FlashText();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isCursorOverInputField && !isReadyForInput)
            {
                isReadyForInput = true;
                keyWordFieldColor.color = new Color(0, 1, 0, 0);
                keyWordFieldText.text = keyword;
                isFlash = true;
            }
            else if (isCursorOverInputField && isReadyForInput)
            {

            }
            else
            {
                if (keyword == "")
                    keyWordFieldText.text = " 클릭하여 입력…";
                isReadyForInput = false;
                isFlash = false;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!isReadyForInput)
            keyWordFieldColor.color = new Color(0, 1, 0, 0.15f);
        isCursorOverInputField = true;
    }

    private void OnMouseExit()
    {
        keyWordFieldColor.color = new Color(0, 1, 0, 0);
        isCursorOverInputField = false;
    }

    public void SetLayer(int layer)//모든 입력 차단
    {
        this.gameObject.layer = layer;
    }

    private void FlashText()//키워드 창을 깜박이게 만든다
    {
        StartCoroutine(FlashTextIEnumerator());
    }

    private IEnumerator FlashTextIEnumerator()//FlashText 재귀
    {
        if (keyword.Length <= 16 && isReadyForInput && !isOnPrintFlow && !skipOneFlash)
        {
            if (isFlash)
            {
                keyWordFieldText.text = keyword;
                isFlash = false;
            }
            else
            {
                keyWordFieldText.text = keyword + " …";
                isFlash = true;
            }
        }
        else if (!isReadyForInput && keyword == "")
            keyWordFieldText.text = " 클릭하여 입력…";
        else if(!isReadyForInput && keyword != "")
            keyWordFieldText.text = keyword;

        //이번 턴에 스킵했으니 다음 번에는 깜박여야 한다
        skipOneFlash = !skipOneFlash ? false : false;
        
        //0.5초 대기
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FlashText", 0.5f);
    }

    private void DelayFlashInputField()//깜박임을 0.5초 막는다
    {
        skipOneFlash = true;
    }

    public void AddKeyWord(string value)//키워드에 한 단어 추가한다
    {
        if(!isReadyForInput)
            return;
        else if(keyword.Length>16)
        {
            adfgvx.InformError("암호 키 최대 입력 : 입력 불가");
            return;
        }
        DelayFlashInputField();
        keyword = keyword + " " + value;
        keyWordFieldText.text = keyword;
        UpdatePriority();
    }

    public void DeleteKeyWord()//키워드에서 한 단어 지운다
    {
        if (!isReadyForInput)
            return;
        else if(keyword.Length<2)
        {
            adfgvx.InformError("암호 키 최소 입력 : 삭제 불가");
            return;
        }

        DelayFlashInputField();
        keyword = keyword.Substring(0, keyword.Length - 2);
        keyWordFieldText.text = keyword;
        UpdatePriority();
    }

    public void ClearKeyWord()//키워드를 비운다
    {
        keyword = "";
        keyWordFieldText.text = keyword;
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
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
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
        if (!isReadyForInput)
            return;

        //에러 발생
        if (keyword.Length == 0)
        {
            adfgvx.InformError("전치 실패 : 암호 키 공백");
            return;
        }
        else if(adfgvx.chiperpart.GetChiperText()=="[파일의 내용]")
        {
            adfgvx.InformError("전치 실패 : 전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.chiperpart.GetChiperText());
        int InputPriority = 1;
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;
        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < place.Length; j++)
            {
                if (place[j] == InputPriority)
                {
                    InputPriority++;
                    tempLine[j] = Chiper.Substring(0, rowLength);
                    Chiper = Chiper.Substring(rowLength);
                }
            }
        }

        adfgvx.InformUpdate("작업 중 : 프로그램을 강제 종료하지 마십시오");
        ResizeAndRePositionEdge();
        printFlow();
        isFlash = false;
        isReadyForInput = false;

        adfgvx.soundFlow(rowLength + lineLength, 0, 0.1f * (rowLength + lineLength));
    }

    public void OnTransposeReverseDown()//IntermediateChiper를 들고와서 Transposition에 채워 넣는다
    {
        if (!isReadyForInput)
            return;

        //에러 발생
        if (keyword.Length == 0)
        {
            adfgvx.InformError("역전치 실패 : 암호 키 공백");
            return;
        }
        else if(adfgvx.intermediatepart.GetIntermediateChiper() == "")
        {
            adfgvx.InformError("역전치 실패 : 역전치 대상 공백");
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.intermediatepart.GetIntermediateChiper());
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;

        //한 글자 씩 돌아가면서 채운다
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            tempLine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        ResizeAndRePositionEdge();
        printFlow();
        isReadyForInput = false;
        isFlash = false;

        //사운드 재생
        adfgvx.soundFlow(rowLength + lineLength, 0, 0.1f * (rowLength + lineLength));
    }

    public void ClearTransposition()//Transposition을 비운다
    {
        for (int i = 0; i < 9; i++)
        {
            tempLine[i] = "";
            lines[i].text = "";
        }
    }

    private void ResizeAndRePositionEdge()//Transposition의 가이드 라인 사이즈를 행과 열 크기에 맞춰서 사이즈를 조정하고 재배치한다
    {
        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (lineLength - 1));
        float size_x = 2.5f * lineLength;
        float size_y = 2.5f * rowLength;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
    }

    private void printFlow()//2차원 평면 흐름 출력
    {
        adfgvx.SetPartLayer(2, 2, 2, 2);

        //흐름 출력 개시
        flowLine = 0;
        InvokeRepeating("printflowLine", 0.0f, 0.1f);

        //깜박임 차단, 적절한 시간 후에 깜박임 회복
        keyWordFieldText.text = keyword;
        isOnPrintFlow = true;
        Invoke("SetisOnPrintFlowFalse", 0.1f * (rowLength + lineLength));
    }

    private void printflowLine()//흐름 출력 오른쪽
    {
        if (flowLine == lineLength)
        {
            CancelInvoke("printflowLine");
            return;
        }
        StartCoroutine(printFlowRow(flowLine++, 0));
    }

    private IEnumerator printFlowRow(int flowLine, int FlowRow)//흐름 출력 아래쪽
    {
        if(flowLine == lineLength - 1 && FlowRow == rowLength)//흐름 출력 최종 종료 시
        {
            adfgvx.SetPartLayer(0, 0, 0, 0);
            adfgvx.InformUpdate("전치 작업 종료 : 총 작업 시간 " + (0.1f * (rowLength + lineLength)).ToString() + "s");
            yield break;
        }
        else if (FlowRow == rowLength)
            yield break;
        lines[flowLine].text += tempLine[flowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(flowLine, FlowRow + 1));
    }

    private void SetisOnPrintFlowFalse()//isOnPrintFlow변수를 거짓으로 한다_Invoke용
    {
        isOnPrintFlow = false;
    }
}