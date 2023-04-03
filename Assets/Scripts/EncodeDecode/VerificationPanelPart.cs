using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class VerificationPanelPart : MonoBehaviour
{
    private ADFGVX adfgvx;

    private SpriteRenderer panelBackgroundSprite_D;
    private SpriteRenderer panelGuideSprite;
    private TextField title;
    private TextField result;
    private TextField percentageInfo;
    private TextField percentage;
    private Gauge gauge;
    private Log log;
    private Button_ADFGVX_QuitVerificationPanel quit;

    private void Start()
    {
        adfgvx = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        panelBackgroundSprite_D = transform.GetChild(2).GetComponent<SpriteRenderer>();
        panelGuideSprite = transform.GetChild(3).GetComponent<SpriteRenderer>();
        title = transform.GetChild(4).GetComponent<TextField>();
        result = transform.GetChild(5).GetComponent<TextField>();
        percentageInfo = transform.GetChild(6).GetComponent<TextField>();
        percentage = transform.GetChild(7).GetComponent<TextField>();
        gauge = transform.GetChild(8).GetComponent<Gauge>();
        log = transform.GetChild(9).GetComponent<Log>();
        quit = transform.GetChild(10).GetComponent<Button_ADFGVX_QuitVerificationPanel>();

        UnvisiblePart();
    }

    public void SetLayer(int layer)//하위 요소의 입력 제어
    {
        this.gameObject.layer = layer;
        quit.gameObject.layer = layer;
    }

    public void VisiblePart()//파트 가시
    {
        this.transform.localPosition = new Vector3(-63.6f, -45.7f, 0);
    }

    public void UnvisiblePart()//파트 비가시
    {
        this.transform.localPosition = new Vector3(330, -45.7f, 0);
    }

    public void StartDecodeVerification()//복호화 검증 개시
    {
        StartCoroutine(StartDecodeVerification_IE());
    }

    private IEnumerator StartDecodeVerification_IE()//StartDecodeVerification_IEnumerator
    {
        //에러 발생
        if (adfgvx.encodeDataLoadPart.GetTextField_Data() == "암호화 데이터를 로드하여 시작…")
        {
            adfgvx.InformError("복호화 데이터 무결성 검증 불가 : 암호화 데이터 공백");
            yield break;
        }
        else if(adfgvx.afterDecodingPart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("복호화 데이터 무결성 검증 불가 : 복호화 데이터 공백");
            yield break;
        }
        else if(adfgvx.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            adfgvx.InformError("복호화 데이터 무결성 검증 불가 : 복호화 전치 키 공백");
            yield break;
        }

        adfgvx.InformUpdate("복호화 무결성 검증 작업 개시…");

        //연출
        title.SetSizeTextOnly(new Vector2(1,1));
        title.SetTextColor(Color.white);
        title.SetText("복호화 무결성 검증 진행 중…");
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);
        gauge.VisibleGaugeImediately();
        percentage.SetTextColor(Color.white);
        percentageInfo.SetTextColor(Color.white);
        result.SetTextColor(Color.clear);
        log.SetColorText(Color.white);
        gauge.FillGaugeBar(3.0f, new Color(0.13f, 0.67f, 0.28f, 1));
        percentage.FillPercentage(3.0f);

        //걸린 시간
        float totalElaspedTime = adfgvx.GetTotalElapsedTime();

        //입력 차단
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //파트 가시
        VisiblePart();

        //로그 파일 경로
        string filePath = adfgvx.ReturnDecodeScore() ? "DecodeSuccess" : "DecodeFail";
        log.FillLoadingLog(3.0f, filePath);

        //오디오 재생
        adfgvx.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //연출
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1f);
        percentage.ConvertColorTextOnly(1f, Color.clear);
        percentageInfo.ConvertColorTextOnly(1f, Color.clear);
        gauge.UnvisibleGauge(1f);
        log.HideTextOnly(1f);
        
        //결과 정보
        string info;
        string keword;
        string time;
        if (adfgvx.ReturnDecodeScore())
        {
            title.SetText("데이터 복호화 작업 성공");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "보안 등급 : " + adfgvx.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + adfgvx.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'을\n";
            keword = "전치 키 : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "로 복호화에 성공했습니다\n";
        }
        else
        {
            title.SetText("데이터 복호화 작업 실패");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "보안 등급 : " + adfgvx.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + adfgvx.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'을\n";
            keword = "전치 키 : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "로 복호화에 실패했습니다\n";
        }
        time = "총 작업 시간 : " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");

        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //결과 안내
        yield return new WaitForSeconds(1.5f);
        result.SetTextColor(Color.white);
        result.SetText("");
        result.FlowText(info + keword + time, 3f);

        //오디오 재생
        adfgvx.SoundFlow(30, 3f);

        //종료
        yield return new WaitForSeconds(4f);
        adfgvx.InformUpdate(adfgvx.ReturnDecodeScore() ? "복호화 데이터 무결성 검증에 성공했습니다" : "복호화 데이터 무결성 검증에 실패했습니다");

        //튜토리얼 관련 코드
        if (adfgvx.GetCurrentTutorialPhase() == 9 && adfgvx.CurrentMode == ADFGVX.mode.Decoding)
        {
            Debug.Log(adfgvx.ReturnDecodeScore());
            if (adfgvx.ReturnDecodeScore())
                adfgvx.MoveToNextTutorialPhase(0f);
            else
                adfgvx.DisplayTutorialDialog(166, 0f);
        }
        else        
        {
           adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 0 ,2);
        }
    }

    public void StartEncodeVerifiaction()//암호화 검증 개시
    {
        StartCoroutine(StartEncodeVerification_IE());
    }

    private IEnumerator StartEncodeVerification_IE()//StartEncodeVerifiaction_IEnumerator
    {
        if(adfgvx.encodeDataSavePart.GetInputField_Data().GetInputString() == "")
        {
            adfgvx.InformError("암호화 데이터 무결성 검증 불가 : 저장 파일 내용 공백");
            yield break;
        }
        if(adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() == "")
        {
            adfgvx.InformError("암호화 데이터 무결성 검증 불가 : 저장 파일 제목 공백");
            yield break;
        }
        if(adfgvx.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            adfgvx.InformError("암호화 데이터 무결성 검증 불가 : 암호화 전치 키 공백");
            yield break;
        }

        adfgvx.InformUpdate("암호화 무결성 검증 작업 개시…");

        //연출
        title.SetSizeTextOnly(new Vector2(1, 1));
        title.SetTextColor(Color.white);
        title.SetText("암호화 무결성 검증 진행 중…");
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);
        gauge.VisibleGaugeImediately();
        percentage.SetTextColor(Color.white);
        percentageInfo.SetTextColor(Color.white);
        result.SetTextColor(Color.clear);
        log.SetColorText(Color.white);
        gauge.FillGaugeBar(3f, new Color(0.13f, 0.67f, 0.28f, 1f));
        percentage.FillPercentage(3f);

        //걸린 시간
        float totalElaspedTime = adfgvx.GetTotalElapsedTime();

        //입력 차단
        adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);

        //파트 가시
        VisiblePart();

        //로그 파일 경로
        string filePath = adfgvx.ReturnEncodeScore() ? "EncodeSuccess" : "EncodeFail";
        log.FillLoadingLog(3f, filePath);

        //오디오 재생
        adfgvx.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //연출
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1f);
        percentage.ConvertColorTextOnly(1f, Color.clear);
        percentageInfo.ConvertColorTextOnly(1f, Color.clear);
        gauge.UnvisibleGauge(1f);
        log.HideTextOnly(1f);

        //결과 정보
        string info;
        string keyword;
        string time;
        if (adfgvx.ReturnEncodeScore())
        {
            title.SetText("데이터 암호화 작업 성공");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "보안 등급 : " + adfgvx.encodeDataSavePart.GetSecurityLevel() + " '" + adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() + "'을\n";
            keyword = "전치 키 : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "로 암호화 하는데 성공했습니다\n";
        }
        else
        {
            title.SetText("데이터 암호화 작업 성공");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "보안 등급 : " + adfgvx.encodeDataSavePart.GetSecurityLevel() + " '" + adfgvx.encodeDataSavePart.GetInputField_Title().GetInputString() + "'을\n";
            keyword = "전치 키 : " + EditStirng.CollectEnglishUpperAlphabet(adfgvx.transpositionpart.GetInputField_keyword().GetInputString()) + "로 암호화 하는데 실패했습니다\n";
        }

        time = "총 작업 시간 : " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");
        
        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //결과 안내
        yield return new WaitForSeconds(1.5f);
        result.SetText("");
        result.SetTextColor(Color.white);
        result.FlowText(info + keyword + time, 3f);

        //오디오 재생
        adfgvx.SoundFlow(30, 3f);

        //종료
        yield return new WaitForSeconds(4f);
        adfgvx.InformUpdate(adfgvx.ReturnDecodeScore() ? "암호화 데이터 무결성 검증에 성공했습니다" : "암호화 데이터 무결성 검증에 실패했습니다");

        //튜토리얼 관련 코드
        if(adfgvx.GetCurrentTutorialPhase() == 3 && adfgvx.CurrentMode == ADFGVX.mode.Encoding)
        {
            if (adfgvx.ReturnEncodeScore())
                adfgvx.MoveToNextTutorialPhase(0f);
            else
                adfgvx.DisplayTutorialDialog(67, 0f);
        }
        else
        {
            adfgvx.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 0, 2);
        }
    }

    private void ConvertSpriteSize(Vector2 targetSize, SpriteRenderer target, float endTime)
    {
        StartCoroutine(ConvertSpriteSize_IE(target.size, target, targetSize, endTime, 0));
    }

    private IEnumerator ConvertSpriteSize_IE(Vector2 currentSize, SpriteRenderer target, Vector2 targetSize, float endTime, float currentTime)
    {
        currentTime += endTime / 100;
        if (currentTime > endTime)
            yield break;

        float targetSizeX = currentSize.x + (targetSize.x - currentSize.x) * (currentTime / endTime);
        float targetSizeY = currentSize.y + (targetSize.y - currentSize.y) * (currentTime / endTime);
        target.size = new Vector2(targetSizeX, targetSizeY);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSpriteSize_IE(target.size, target, targetSize, endTime, currentTime));
    }
}
