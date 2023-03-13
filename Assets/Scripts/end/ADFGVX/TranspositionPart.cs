using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX keyword;
    private TextField priority;

    private TextMeshPro[] lines;

    private SpriteRenderer transposedLineGuideBox;

    //전치, 역전치 때 사용하는 변수들
    private int[] place;                                //키워드 문자의 순위
    int rowLength;                                      //전치 행렬의 행 길이
    int lineLength;                                     //전치 행렬의 열 길이
    private string[] tempLine;                          //흐름 출력 전, 잠시 저장해놓는다
    private int flowLine;                               //흐름 출력 인덱스

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        lines = new TextMeshPro[9];
        for (int i=0;i<9;i++)
        {
            lines[i] = GetComponentsInChildren<TextMeshPro>()[i];
            lines[i].text = "";
        }

        keyword = transform.Find("Keyword").GetComponent<InputField_ADFGVX>();
        priority = transform.Find("Priority").GetComponent<TextField>();
        priority.SetText("");
        
        transposedLineGuideBox = GetComponentsInChildren<SpriteRenderer>()[0];
        transposedLineGuideBox.size = new Vector2(0, 0);
        tempLine = new string[9];
    }

    public void SetLayer(int layer)//이 게임오브젝트 하위 요소의 레이어 제어
    {
        transform.Find("Keyword").gameObject.layer = layer;
    }

    public InputField_ADFGVX GetInputField_keyword()
    {
        return keyword;
    }

    public int[] GetPriority()
    {
        return place;
    }

    public void AddKeyword(string value)//Keyword에 한 글자 입력
    {
        if (value.ToCharArray()[0] < 'A' || value.ToCharArray()[0] > 'Z')
        {
            adfgvx.InformError("유효하지 않은 전치 키 입력 : 입력 불가");
            return;
        }

        keyword.AddInputField(value + " ");
        UpdatePriority();
    }

    public void DeleteKeyword()//Keyword에서 한 글자 삭제
    {
        keyword.DeleteInputField(2);
        UpdatePriority();
    }
  
    private void UpdatePriority()//Priority를 새로 구한다
    {
        string result = "";
        string value = CollectEnglishAlphabet(keyword.GetInputString());
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

        for (int i = 0; i < keyword.GetInputString().Length / 2; i++)
        {
            result += place[i].ToString();
            result += " ";
        }

        ClearTransposition();

        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (place.Length - 1));
        float size_x = 2.5f * place.Length;
        float size_y = 2.5f;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);

        priority.SetText(result);
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

    public void OnTransposeDown()//EncodeData를 키 순서에 따라서 Transposition에 전치
    {
        //에러 발생
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("전치 실패 : 전치 키 공백");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (adfgvx.encodeDataLoadPart.GetData() == "암호화 데이터를 로드하여 시작…")
        {
            adfgvx.InformError("전치 실패 : 전치 대상 공백");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData()).Length / place.Length > 12)
        {            
            //튜토리얼 관련 코드
            if (adfgvx.GetCurrentTutorialPhase() == 1 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (CollectEnglishAlphabet(keyword.GetInputString()) != "HELLO")
                    adfgvx.DisplayTutorialDialog(41, 0f);
                else
                    adfgvx.MoveToNextTutorialPhase(2.0f);
            }

            adfgvx.InformError("전치 실패 : 메모리 용량 초과");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData()).Length % place.Length != 0)
        {
            //튜토리얼 관련 코드
            if (adfgvx.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
            {
                if (CollectEnglishAlphabet(keyword.GetInputString()).Length != 7)
                    adfgvx.DisplayTutorialDialog(73, 0f);
                else
                    adfgvx.MoveToNextTutorialPhase(2.0f);
            }

            adfgvx.InformError("전치 실패 : 메모리 누수 발생");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }

        //키 순위 초기화
        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.encodeDataLoadPart.GetData());
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

        adfgvx.InformUpdate("전치 작업 중… 프로그램을 강제 종료하지 마십시오");
        ResizeAndRePositionEdge();
        printFlow();

        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //사운드 재생
        adfgvx.soundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //튜토리얼 관련 코드
        if (adfgvx.GetCurrentTutorialPhase() == 3 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (CollectEnglishAlphabet(keyword.GetInputString()) != "SUKHOI")
                adfgvx.DisplayTutorialDialog(85, 0f);
            else
                adfgvx.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
    }

    public void OnTransposeReverseDown()//OriginalData를 키 순서에 따라서 Transposition에 역전치
    {
        //에러 발생
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("역전치 실패 : 전치 키 공백");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("역전치 실패 : 역전치 대상 공백");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length / place.Length > 12)
        {
            adfgvx.InformError("역전치 실패 : 메모리 용량 초과");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }
        else if (CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length % place.Length != 0)
        {
            adfgvx.InformError("역전치 실패 : 메모리 누수 발생");
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
            return;
        }

        ClearTransposition();

        string Chiper = CollectEnglishAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString());
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;

        //한 글자 씩 돌아가면서 채운다
        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            tempLine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        adfgvx.InformUpdate("역전치 작업 중… 프로그램을 강제 종료하지 마십시오");
        ResizeAndRePositionEdge();
        printFlow();

        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //사운드 재생
        adfgvx.soundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //튜토리얼 관련 코드
        if(adfgvx.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            adfgvx.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
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
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //흐름 출력 개시
        flowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.1f);
        keyword.SetMarkText(keyword.GetInputString());
    }

    private void printFlowLine()//흐름 출력 오른쪽
    {
        if (flowLine == lineLength)
        {
            CancelInvoke("printFlowLine");
            return;
        }
        StartCoroutine(printFlowRow(flowLine++, 0));
    }

    private IEnumerator printFlowRow(int flowLine, int FlowRow)//흐름 출력 아래쪽
    {
        if(flowLine == lineLength - 1 && FlowRow == rowLength)//흐름 출력 최종 종료 시
        {
            adfgvx.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            adfgvx.InformUpdate("전치 작업 종료 : 총 작업 시간 " + (0.1f * (rowLength + lineLength)).ToString() + "s");
            yield break;
        }
        else if (FlowRow == rowLength)
            yield break;
        lines[flowLine].text += tempLine[flowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(flowLine, FlowRow + 1));
    }
}