using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX모드
    { Encoding,Decoding};
    public mode currentmode;//현재 ADFGVX모드

    [Header("모드 버튼")]
    public Button_ShiftMode shiftmode;

    [Header("현재 상태 표시")]
    public Info InfoBox;

    [Header("이중 문자 치환 파트")]
    public BiliteralSubstitutionPart biliteralsubstitutionpart;
    [Header("전치 파트")]
    public TranspositionPart transpositionpart;
    [Header("중간 파트")]
    public IntermediatePart intermediatepart;
    [Header("암호 파트")]
    public ChiperPart chiperpart;
   

    private void Start()
    {
        UpdateInfoBox("ADFGVX 테이블을 선택하십시오.");
    }


    public void OnClearDown()//Clear 버튼이 눌렸을 때
    {
        UpdateInfoBox("중간 암호 용지 비움");
        InformCurrentMode();
    }

    public void OnDeleteDown()//Delete 버튼이 눌렸을 때
    {
    }

    public void OnModeDown()//모드 전환 버튼이 눌렸을 때
    {
        chiperpart.ClearChiperAll();
        chiperpart.InitializeChiperAll();

        transpositionpart.ClearKeyWord();
        transpositionpart.ClearPriority();
        transpositionpart.ClearTransposition();

        intermediatepart.ClearIntermediateChiperAll();
        intermediatepart.InitializeIntermediateChiperAll();

        biliteralsubstitutionpart.InitializeText();
        
        if (currentmode == ADFGVX.mode.Encoding)
        {
            currentmode = ADFGVX.mode.Decoding;
            UpdateInfoBox("모드 전환 : 복호화");
        }
        else if (currentmode == ADFGVX.mode.Decoding)
        {
            currentmode = ADFGVX.mode.Encoding;
            UpdateInfoBox("모드 전환 : 암호화");
        }
        InformCurrentMode();
    }

    public void UpdateInfoBox(string Value)//InfoBox의 텍스트를 Value로 바꾼다
    {
        InfoBox.UpdateText(Value);
    }
    
    public void InformCurrentMode()//1초 후에 현재 모드 출력
    {
        if (currentmode == mode.Encoding)
            UpdateInfoBoxDelay(1, "암호화 과정 진행 중...");
        else if (currentmode == mode.Decoding)
            UpdateInfoBoxDelay(1, "복호화 과정 진행 중...");
    }
    
    private void UpdateInfoBoxDelay(float Time, string Value)//InfoBox의 텍스트를 Timer초 후에 Value로 바꾼다
    {
        StartCoroutine(UpdateInfoBoxTimer(Time, Value));        
    }
    
    private IEnumerator UpdateInfoBoxTimer(float Time, string Value)//UpdateInfoBoxDelay 코루틴
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
