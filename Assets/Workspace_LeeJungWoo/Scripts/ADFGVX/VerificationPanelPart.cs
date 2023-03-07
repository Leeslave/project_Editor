using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class VerificationPanelPart : MonoBehaviour
{
    private ADFGVX GameManager;

    private SpriteRenderer screenBlurSprite;
    private SpriteRenderer panelBackgroundSprite_U;
    private SpriteRenderer panelBackgroundSprite_D;
    private SpriteRenderer panelGuideSprite;

    private TextField title;
    private TextField result;
    private TextField percentageInfo;
    private TextField percentage;
    private Gauge loadingGauge;
    private Log consoleLog;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<ADFGVX>();

        title = transform.Find("Title").GetComponent<TextField>();
        result = transform.Find("Result").GetComponent<TextField>();
        percentageInfo = transform.Find("PercentageInfo").GetComponent<TextField>();
        percentage = transform.Find("Percentage").GetComponent<TextField>();
        loadingGauge = transform.Find("LoadingGauge").GetComponent<Gauge>();
        consoleLog = transform.Find("ConsoleLog").GetComponent<Log>();

        screenBlurSprite = GetComponentsInChildren<SpriteRenderer>()[0];
        panelBackgroundSprite_U = GetComponentsInChildren<SpriteRenderer>()[1];
        panelBackgroundSprite_D = GetComponentsInChildren<SpriteRenderer>()[2];
        panelGuideSprite = GetComponentsInChildren<SpriteRenderer>()[3];

        UnvisiblePart();
    }

    public void SetLayer(int layer)//모든 입력 제어
    {
        this.gameObject.layer = layer;
        GameObject.Find("XButton").layer = layer;
    }

    public void VisiblePart()//확인 창 가시
    {
        this.transform.localPosition = new Vector3(-63.6f, -45.7f, 0);
    }

    public void UnvisiblePart()//확인 창 비가시
    {
        this.transform.localPosition = new Vector3(330, -45.7f, 0);
    }

    public void StartDecodeVerification()//복호화 확인 시작하기
    {
        StartCoroutine(StartDecodeVerificationIEnumerator());
    }

    private IEnumerator StartDecodeVerificationIEnumerator()//복호화 확인 시작하기
    {
        if (GameManager.encodeDataLoadPart.GetData() == "암호화 데이터를 로드하여 시작…")
        {
            GameManager.InformError("암호화 데이터 빈 칸 : 복호화 시퀀스 진행 불가");
            yield break;
        }
        else if(GameManager.afterDecodingPart.GetInputField_Data().GetInputString() == "")
        {
            GameManager.InformError("복호화 데이터 빈 칸 : 복호화 시퀀스 진행 불가");
            yield break;
        }
        else if(GameManager.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            GameManager.InformError("복호화 키 빈 칸 : 복호화 시퀀스 진행 불가");
            yield break;
        }

        GameManager.InformUpdate("복호화 데이터 저장 시퀀스 개시");

        //결과 창 제목 초기화
        title.SetSizeTextOnly(new Vector2(1,1));
        title.SetTextColor(Color.white);
        title.SetText("최종 복호화 시퀀스 진행 중");

        //결과 창 크기 조정
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);

        loadingGauge.VisibleGaugeImediately();
        
        percentage.SetTextColor(Color.white);
        percentageInfo.SetTextColor(Color.white);
        result.SetTextColor(Color.clear);
        consoleLog.SetColorText(Color.white);

        //플레이 시간 저장
        float totalElaspedTime = GameManager.GetTotalElapsedTime();

        //모든 입력 차단
        GameManager.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2, 2);

        //확인 패널 가시 모드
        VisiblePart();

        //게이지 바
        loadingGauge.FillGaugeBar(3.0f, new Color(0.13f, 0.67f, 0.28f, 1));
        percentage.FillPercentage(3.0f);

        //로딩 로그
        string filePath = GameManager.ReturnDecodeScore() ? "DecodeSuccess" : "DecodeFail";
        consoleLog.FillLoadingLog(3.0f, filePath);

        //작업 중인 것 같은 사운드 재생
        GameManager.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //확인 창 작업 종료 연출
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1f);
        percentage.ConvertColorTextOnly(1f, Color.clear);
        percentageInfo.ConvertColorTextOnly(1f, Color.clear);
        loadingGauge.UnvisibleGauge(1f);
        consoleLog.HideTextOnly(1f);
        
        //결과 세부사항 흐름 출력 준비
        string info;
        string keword;
        string time;
        if (GameManager.ReturnDecodeScore())
        {
            title.SetText("최종 복호화 시퀀스 완료");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "알림 : " + GameManager.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + GameManager.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'를\n";
            keword = "암호 키 : " + EditStirng.CollectEnglishUpperAlphabet(GameManager.transpositionpart.GetInputField_keyword().GetInputString()) + "로 복호화 성공\n";
        }
        else
        {
            title.SetText("최종 복호화 시퀀스 실패");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "경고 : " + GameManager.encodeDataLoadPart.GetTextField_SecurityLevel().GetText() + " '" + GameManager.encodeDataLoadPart.GetInputField_filePath().GetInputString() + "'를\n";
            keword = "암호 키 : " + EditStirng.CollectEnglishUpperAlphabet(GameManager.transpositionpart.GetInputField_keyword().GetInputString()) + "로 복호화 실패\n";
        }
        time = "총 작업 시간: " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");

        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //결과 세부사항 출력
        yield return new WaitForSeconds(1.5f);
        result.SetTextColor(Color.white);
        result.SetText("");
        result.FlowText(info + keword + time, 3f);

        //소리 재생
        GameManager.soundFlow(30, 3f);

        //확인 창 입력 회복
        yield return new WaitForSeconds(4f);
        GameManager.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 0 ,2);
        GameManager.InformUpdate(GameManager.ReturnDecodeScore() ? "복호화 데이터 저장 시퀀스 성공" : "복호화 데이터 저장 시퀀스 실패");

        //튜토리얼 관련 코드
        if (GameManager.GetCurrentTutorialPhase() == 9)
        {
            Debug.Log(GameManager.ReturnDecodeScore());
            if (GameManager.ReturnDecodeScore())
                GameManager.MoveToNextTutorialPhase(0f);
            else
                GameManager.DisplayTutorialDialog(118, 0f);
        }
    }

    public void StartEncodeVerifiaction()//암호화 확인 시작하기
    {
        StartCoroutine(StartEncodeVerificationIEnumerator());
    }

    private IEnumerator StartEncodeVerificationIEnumerator()//암호화 확인 시작하기
    {
        if(GameManager.encodeDataSavePart.GetInputField_Data().GetInputString() == "")
        {
            GameManager.InformError("암호화 내용 빈 칸 : 암호화 시퀀스 진행 불가");
            yield break;
        }
        if(GameManager.encodeDataSavePart.GetInputField_Title().GetInputString() == "")
        {
            GameManager.InformError("암호화 제목 빈 칸 : 암호화 시퀀스 진행 불가");
            yield break;
        }
        if(GameManager.transpositionpart.GetInputField_keyword().GetInputString() == "")
        {
            GameManager.InformError("암호화 키 빈 칸 : 암호화 시퀀스 진행 불가");
            yield break;
        }

        GameManager.InformUpdate("암호화 데이터 저장 시퀀스 개시");

        //결과 창 제목 초기화
        title.SetSizeTextOnly(new Vector2(1, 1));
        title.SetTextColor(Color.white);
        title.SetText("최종 암호화 시퀀스 진행 중");

        //결과 창 크기 조정
        panelBackgroundSprite_D.size = new Vector2(231.8f, 46.3f);
        panelGuideSprite.size = new Vector2(57.9f, 11.7f);

        //결과 창 게이지 초기화
        loadingGauge.VisibleGaugeImediately();

        percentage.SetTextColor(Color.white);
        percentageInfo.SetTextColor(Color.white);
        result.SetTextColor(Color.clear);
        consoleLog.SetColorText(Color.white);

        //플레이 시간 저장
        float totalElaspedTime = GameManager.GetTotalElapsedTime();

        //모든 입력 차단
        GameManager.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 2, 2);

        //확인 패널 가시 모드
        VisiblePart();

        //게이지 바
        loadingGauge.FillGaugeBar(3f, new Color(0.13f, 0.67f, 0.28f, 1f));
        percentage.FillPercentage(3f);

        //로딩 로그
        string filePath = true ? "EncodeSuccess" : "EncodeFail";
        consoleLog.FillLoadingLog(3f, filePath);

        //작업 중인 것 같은 사운드 재생
        GameManager.PlayAudioSource(ADFGVX.Audio.DataProcessing);

        //확인 창 작업 종료 연출
        yield return new WaitForSeconds(4f);
        title.ConvertSizeTextOnly(new Vector2(1.66f,1.66f), 1f);
        percentage.ConvertColorTextOnly(1f, Color.clear);
        percentageInfo.ConvertColorTextOnly(1f, Color.clear);
        loadingGauge.UnvisibleGauge(1f);
        consoleLog.HideTextOnly(1f);

        //결과 안내 흐름 출력
        string info;
        string keyword;
        string time;
        if (GameManager.ReturnEncodeScore())
        {
            title.SetText("최종 암호화 시퀀스 성공");
            title.ConvertColorTextOnly(3f, new Color(0.1f, 0.35f, 0.85f, 1f));

            info = "알림 : " + GameManager.encodeDataSavePart.GetSecurityLevel() + " '" + GameManager.encodeDataSavePart.GetInputField_Title().GetInputString() + "'를\n";
            keyword = "암호 키 : " + EditStirng.CollectEnglishUpperAlphabet(GameManager.transpositionpart.GetInputField_keyword().GetInputString()) + "로 암호화 성공\n";
        }
        else
        {
            title.SetText("최종 암호화 시퀀스 실패");
            title.ConvertColorTextOnly(3f, new Color(0.76f, 0.28f, 0.28f, 1f));

            info = "경고 : " + GameManager.encodeDataSavePart.GetSecurityLevel() + " '" + GameManager.encodeDataSavePart.GetInputField_Title().GetInputString() + "'를\n";
            keyword = "암호 키 : " + EditStirng.CollectEnglishUpperAlphabet(GameManager.transpositionpart.GetInputField_keyword().GetInputString()) + "로 암호화 실패\n";
        }

        time = "총 작업 시간: " + Mathf.FloorToInt(totalElaspedTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(totalElaspedTime % 60).ToString("D2");
        
        ConvertSpriteSize(new Vector2(231.8f, 57.6f), panelBackgroundSprite_D, 1f);
        ConvertSpriteSize(new Vector2(57.9f, 14.6f), panelGuideSprite, 1f);

        //결과 세부사항 출력
        yield return new WaitForSeconds(1.5f);
        result.SetText("");
        result.SetTextColor(Color.white);
        result.FlowText(info + keyword + time, 3f);

        //소리 재생
        GameManager.soundFlow(30, 3f);

        //확인 창 입력 회복
        yield return new WaitForSeconds(4f);
        GameManager.SetPartLayer(2, 2, 2, 2, 2, 2, 2, 0, 2);
        GameManager.InformUpdate(GameManager.ReturnDecodeScore() ? "암호화 데이터 저장 시퀀스 성공" : "암호화 데이터 저장 시퀀스 실패");
    }

    private void ConvertSpriteSize(Vector2 targetSize, SpriteRenderer target, float endTime)
    {
        StartCoroutine(ConvertSpriteSizeIEnumerator(target.size, target, targetSize, endTime, 0));
    }

    private IEnumerator ConvertSpriteSizeIEnumerator(Vector2 currentSize, SpriteRenderer target, Vector2 targetSize, float endTime, float currentTime)
    {
        currentTime += endTime / 100;
        if (currentTime >= endTime)
            yield break;

        float targetSizeX = currentSize.x + (targetSize.x - currentSize.x) * (currentTime / endTime);
        float targetSizeY = currentSize.y + (targetSize.y - currentSize.y) * (currentTime / endTime);
        target.size = new Vector2(targetSizeX, targetSizeY);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(ConvertSpriteSizeIEnumerator(target.size, target, targetSize, endTime, currentTime));
    }
}
