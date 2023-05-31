using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranspositionPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private InputField_ADFGVX keyword;
    private TextField priority;

    //키 순위 전치 관련 변수
    private int[] place;                                //키 순위 저장 배열
    int rowLength;                                      //전치된 행렬의 열 길이
    int lineLength;                                     //전치된 행렬의 열 길이
    private string[] tempLine;                          //일시적으로 행렬 저장
    private int flowLine;                               //2차원 출력 인덱스
    private TextMeshPro[] lines;                        //전치된 행렬이 출력되는 텍스트
    private SpriteRenderer transposedMatrixGuide;       //전치된 행렬를 둘러싸는 스프라이트

    private void Awake()
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
        
        transposedMatrixGuide = GetComponentsInChildren<SpriteRenderer>()[0];
        transposedMatrixGuide.size = new Vector2(0, 0);
        tempLine = new string[9];
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        keyword.gameObject.layer = layer;
        if(layer == 2)
        {
            keyword.SetIsReadyForInput(false);
            keyword.SetIsFlash(false);
        }
    }

    public InputField_ADFGVX GetInputField_keyword()
    {
        return keyword;
    }

    public int[] GetPriority()
    {
        return place;
    }

    public void AddKeyword(string value)//전치 키 입력창에 추가
    {
        if (value.ToCharArray()[0] < 'A' || value.ToCharArray()[0] > 'Z')
        {
            adfgvx.InformError("유효하지 않은 전치 키 입력 시도 : 재확인 요망");
            return;
        }

        keyword.AddInputField(value + " ");
        UpdatePriority();
    }

    public void DeleteKeyword()//전치 키 입력창에 삭제
    {
        keyword.DeleteInputField(2);
        UpdatePriority();
    }
  
    private void UpdatePriority()//키 순위 업데이트
    {
        string result = "";
        string value = EditStirng.CollectEnglishUpperAlphabet(keyword.GetInputString());
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

        //전치 비움
        ClearTransposition();

        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (place.Length - 1));
        float size_x = 2.5f * place.Length;
        float size_y = 2.5f;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);

        priority.SetText(result);
    }

    public void OnTransposeDown()//키 순위에 따른 행렬 전치
    {
        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //튜토리얼 관련 코드
        if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 3 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (EditStirng.CollectEnglishUpperAlphabet(keyword.GetInputString()) != "SUKHOI")
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(85, 0f);
                return;
            }
        }
        if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (EditStirng.CollectEnglishUpperAlphabet(keyword.GetInputString()).Length != 7)
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(73, 0f);
                return;
            }
            else
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(2.0f);
        }
        if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 1 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (EditStirng.CollectEnglishUpperAlphabet(keyword.GetInputString()) != "HELLO")
            {
                adfgvx.chat_ADFGVX.DisplayTutorialDialog(41, 0f);
                return;
            }
            else
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(2.0f);
        }
    
        //에러 발생
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("전치 불가 : 전치 키 공백");
            return;
        }
        else if (adfgvx.encodeDataLoadPart.GetTextField_Data() == "암호화 데이터를 로드하여 시작…")
        {
            adfgvx.InformError("전치 불가 : 암호화 데이터 공백");
            return;
        }
        else if (EditStirng.CollectEnglishUpperAlphabet(adfgvx.encodeDataLoadPart.GetTextField_Data()).Length / place.Length > 12)
        {            
            adfgvx.InformError("전치 불가 : 메모리 용량 초과");
            return;
        }
        else if (EditStirng.CollectEnglishUpperAlphabet(adfgvx.encodeDataLoadPart.GetTextField_Data()).Length % place.Length != 0)
        {
            adfgvx.InformError("전치 불가 : 메모리 누수 발생");
            return;
        }

        //입력 차단
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //전치 비움
        ClearTransposition();

        string Chiper = EditStirng.CollectEnglishUpperAlphabet(adfgvx.encodeDataLoadPart.GetTextField_Data());
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

        adfgvx.InformUpdate("키 순위 전치 프로토콜 진행 중 : 강제 종료하지 마십시오");
        ResizeAndRePositionMatrixGuide();
        printFlow();

        //오디오 재생
        adfgvx.SoundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //튜토리얼 관련 코드
        if (adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 3 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            if (EditStirng.CollectEnglishUpperAlphabet(keyword.GetInputString()) == "SUKHOI")
                adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
    }

    public void OnTransposeReverseDown()//키 순위에 따른 행렬 역전치
    {
        keyword.SetIsReadyForInput(false);
        keyword.SetIsFlash(false);

        //에러 발생
        if (keyword.GetInputString().Length == 0)
        {
            adfgvx.InformError("역전치 불가 : 전치 키 공백");
            return;
        }
        else if (adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("역전치 불가 : 오리지널 데이터 공백");
            return;
        }
        else if (EditStirng.CollectEnglishUpperAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length / place.Length > 12)
        {
            adfgvx.InformError("역전치 불가 : 메모리 용량 초과");
            return;
        }
        else if (EditStirng.CollectEnglishUpperAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString()).Length % place.Length != 0)
        {
            adfgvx.InformError("역전치 불가 : 메모리 누수 발생");
            return;
        }

        //입력 차단
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //전치 비움
        ClearTransposition();

        string Chiper = EditStirng.CollectEnglishUpperAlphabet(adfgvx.beforeEncodingPart.GetInputField_Data().GetInputString());
        lineLength = place.Length;
        rowLength = Chiper.Length / place.Length;

        int length = Chiper.Length;
        for (int i = 0; i < length; i++)
        {
            tempLine[i % place.Length] += Chiper.Substring(0, 1);
            Chiper = Chiper.Substring(1);
        }

        adfgvx.InformUpdate("키 순위 전치 프로토콜 진행 중 : 강제 종료하지 마십시오");
        ResizeAndRePositionMatrixGuide();
        printFlow();

        //오디오 재생
        adfgvx.SoundFlow(rowLength + lineLength, 0.1f * (rowLength + lineLength));

        //튜토리얼 관련 코드
        if(adfgvx.chat_ADFGVX.GetCurrentTutorialPhase() == 2 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            adfgvx.chat_ADFGVX.MoveToNextTutorialPhase(0.1f * (rowLength + lineLength));
        }
    }

    public void ClearTransposition()//전치 행렬 비움
    {
        for (int i = 0; i < 9; i++)
        {
            tempLine[i] = "";
            lines[i].text = "";
        }
    }

    private void ResizeAndRePositionMatrixGuide()//전치 행렬 크기에 다른 크기 조정
    {
        float pos_x = transform.GetChild(0).localPosition.x + (2.5f * (lineLength - 1));
        float size_x = 2.5f * lineLength;
        float size_y = 2.5f * rowLength;
        GetComponentsInChildren<SpriteRenderer>()[0].transform.localPosition = new Vector3(pos_x, -3.5f, 0);
        GetComponentsInChildren<SpriteRenderer>()[0].size = new Vector2(size_x, size_y);
    }

    private void printFlow()//2차원 출력 개시
    {
        flowLine = 0;
        InvokeRepeating("printFlowLine", 0.0f, 0.1f);
        keyword.GetTMP().text = keyword.GetInputString();
    }

    private void printFlowLine()//2차원 열 출력
    {
        if (flowLine == lineLength)
        {
            CancelInvoke("printFlowLine");
            return;
        }
        StartCoroutine(printFlowRow(flowLine++, 0));
    }

    private IEnumerator printFlowRow(int flowLine, int FlowRow)//2차원 행 출력
    {
        if(flowLine == lineLength - 1 && FlowRow == rowLength)//마지막 출력 종료
        {
            //입력 회복
            adfgvx.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            adfgvx.InformUpdate("키 순위 전치 프로토콜 종료 : 총 작업 시간 " + (0.1f * (rowLength + lineLength)).ToString() + "s");
            yield break;
        }
        else if (FlowRow == rowLength)//행의 끝 도달
            yield break;
        lines[flowLine].text += tempLine[flowLine][FlowRow].ToString();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(printFlowRow(flowLine, FlowRow + 1));
    }
}