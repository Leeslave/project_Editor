using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ADFGVX : MonoBehaviour
{
    public enum mode
    { Encoding, Decoding };

    [Header("현재 ADFGVX 모드")]
    public mode CurrentMode;

    [Header("튜토리얼 실행여부")]
    public bool PlayAsTutorial;
    private int[] DecodeTutorialPhaseArray;
    private int[] EncodeTutorialPhaseArray;
    private int currentTutorialPhase;

    [Header("메인메뉴로 나가는 버튼")]
    public Button_ADFGVX_Quit quitButton;
    [Header("디버그 로그 창")]
    public DebugLog debugLog;
    [Header("안내 화살표")]
    public GuideArrowPart guidearrowpart;
    [Header("이중 문자 치환 파트")]
    public BiliteralSubstitutionPart biliteralsubstitutionpart;
    [Header("키 순위 전치 파트")]
    public TranspositionPart transpositionpart;
    [Header("복호화 후 파트")]
    public AfterDecodingPart afterDecodingPart;
    [Header("암호화 전 파트")]
    public BeforeEncodingPart beforeEncodingPart;
    [Header("암호화 데이터 로드 파트")]
    public EncodeDataLoadPart encodeDataLoadPart;
    [Header("암호화 데이터 저장 파트")]
    public EncodeDataSavePart encodeDataSavePart;
    [Header("무결성 검증 파트")]
    public VerificationPanelPart verificationPart;
    [Header("채팅 프리펩")]
    public Chat_ADFGVX chat_ADFGVX;

    public enum Audio
    {
        TextFlow, AddChar, DeleteChar, DataProcessing
    }

    [Header("오디오 소스 컴포넌트")]
    public AudioSource audioSource;
    [Header("오디오 클립")]
    public AudioClip[] audioClips;

    private float totalElapsedTime;

    private void Start()
    {
        //현재 모드에 따라 창 배치
        if (CurrentMode == mode.Decoding)
        {
            afterDecodingPart.gameObject.transform.localPosition = new Vector3(66.9f, 14.6f, 0);
            encodeDataLoadPart.gameObject.transform.localPosition = new Vector3(102.3f, -68.2f, 0);
        }
        else if (CurrentMode == mode.Encoding)
        {
            guidearrowpart.Rotate180();
            beforeEncodingPart.gameObject.transform.localPosition = new Vector3(96.3f, -19f, 0);
            encodeDataSavePart.gameObject.transform.localPosition = new Vector3(70.7f, -67.9f, 0);
        }

        //튜토리얼 활성
        if (PlayAsTutorial)
        {
            if(CurrentMode == mode.Decoding)
                DecodeTutorialPhaseArray = chat_ADFGVX.GetArrayOfTutorialPhase();
            else if(CurrentMode == mode.Encoding)
                EncodeTutorialPhaseArray = chat_ADFGVX.GetArrayOfTutorialPhase();
            currentTutorialPhase = -1;
            MoveToNextTutorialPhase(0f);
        }

        InformUpdate("아츠토츠카 표준 암호 체계 V7 가동 상태 정상");
    }

    private void Update()
    {
        UpdateStopWatch();
    }

    public void SetPartLayerWaitForSec(float endTime, int quitGame, int BiliteralSubstitution, int ArrayPlusMinus, int Delete, int Clear, int Transposition, int AfterDecoding, 
        int BeforeEncoding, int EncodeDataSave, int EncodeDataLoad, int Verification, int DebugLog)//모든 파트의 입력 제어
    {
        StartCoroutine(SetPartLayerWaitForSec_IE(endTime, 0, quitGame, BiliteralSubstitution, ArrayPlusMinus, Delete, Clear, Transposition, AfterDecoding, BeforeEncoding, EncodeDataSave, EncodeDataLoad, Verification, DebugLog));
    }

    private IEnumerator<WaitForSeconds> SetPartLayerWaitForSec_IE(float endTime, float currentTime, int quitGame, int BiliteralSubstitution, int ArrayPlusMinus, int Delete, int Clear, int Transposition, 
        int AfterDecoding, int BeforeEncoding, int EncodeDataSave, int EncodeDataLoad, int Verification, int DebugLog)//SetPartLayerWaitForSec_IE
    {
        if(endTime == 0f)
        {
            quitButton.gameObject.layer = quitGame;
            biliteralsubstitutionpart.SetLayer(BiliteralSubstitution, ArrayPlusMinus, Delete, Clear);
            transpositionpart.SetLayer(Transposition);
            afterDecodingPart.SetLayer(AfterDecoding);
            beforeEncodingPart.SetLayer(BeforeEncoding);
            encodeDataSavePart.SetLayer(EncodeDataSave);
            encodeDataLoadPart.SetLayer(EncodeDataLoad);
            verificationPart.SetLayer(Verification);
            debugLog.SetLayer(DebugLog); 
            yield break;
        }

        currentTime += endTime / 100;
        if (currentTime > endTime)
        {
            quitButton.gameObject.layer = quitGame;
            biliteralsubstitutionpart.SetLayer(BiliteralSubstitution, ArrayPlusMinus, Delete, Clear);
            transpositionpart.SetLayer(Transposition);
            afterDecodingPart.SetLayer(AfterDecoding);
            beforeEncodingPart.SetLayer(BeforeEncoding);
            encodeDataSavePart.SetLayer(EncodeDataSave);
            encodeDataLoadPart.SetLayer(EncodeDataLoad);
            verificationPart.SetLayer(Verification);
            debugLog.SetLayer(DebugLog); 
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(SetPartLayerWaitForSec_IE(endTime, currentTime, quitGame, BiliteralSubstitution, ArrayPlusMinus, Delete, Clear, Transposition, AfterDecoding, BeforeEncoding, EncodeDataSave, EncodeDataLoad, Verification, DebugLog));
    }

    public void StartStopWatch()//시간 측정 시작
    {
        totalElapsedTime = 0;
    }

    private void UpdateStopWatch()//시간 측정 
    {
        totalElapsedTime += Time.deltaTime;
    }

    public float GetTotalElapsedTime()//시간 측정 값 반환
    {
        return totalElapsedTime;
    }

    public bool ReturnDecodeScore()//복호화 결과 반환
    {

        return EditStirng.CollectEnglishUpperAlphabet(encodeDataLoadPart.GetDecodedChiper()) == EditStirng.CollectEnglishUpperAlphabet(afterDecodingPart.GetInputField_Data().GetInputString());
    }

    public bool ReturnEncodeScore()//암호화 결과 반환
    {
        string original = EditStirng.CollectEnglishUpperAlphabet(beforeEncodingPart.GetInputField_Data().GetInputString());
        string keyword = EditStirng.CollectEnglishUpperAlphabet(transpositionpart.GetInputField_keyword().GetInputString());
        int[] place = new int[keyword.Length];
        place = transpositionpart.GetPriority();

        string encode = EditStirng.CollectEnglishUpperAlphabet(encodeDataSavePart.GetInputField_Data().GetInputString());

        char[] answer = new char[original.Length];

        for (int i = 0; i < place.Length; i++)
        {
            for (int j = 0; j < original.Length / place.Length; j++)
            {
                answer[(original.Length / place.Length) * (place[i] - 1) + j] += original[place.Length * j + i];
            }
        }

        return answer.ArrayToString() == encode;
    }

    public void InformUpdate(string value)//디버그 로그 창 업데이트
    {
        debugLog.DebugInfo(value);
    }

    public void InformError(string value)//디버그 로그 창 에러
    {
        debugLog.DebugError(value);
    }

    public void PlayAudioSource(Audio value)//오디오 재생
    {
        audioSource.clip = audioClips[((int)value)];
        audioSource.Play();
    }

    public void SoundFlow(int length, float endTime)//흐름 출력 재생
    {
        StartCoroutine(SoundFlowIEnumerator(length, 0, endTime));
    }

    private IEnumerator<WaitForSeconds> SoundFlowIEnumerator(int length, int idx, float endTime)//soundFlow_IE
    {
        if (idx >= length)
            yield break;
        PlayAudioSource(Audio.TextFlow);
        yield return new WaitForSeconds(endTime / length);
        StartCoroutine(SoundFlowIEnumerator(length, idx + 1, endTime));
    }










    //튜토리얼 관련 코드
    public int GetCurrentTutorialPhase()//현재 튜토리얼 페이즈 반환
    {
        return currentTutorialPhase;
    }

    public void MoveToNextTutorialPhase(float endTime)//endTime을 대기한 후 다음 튜토리얼 페이즈로 이동
    {
        if (!PlayAsTutorial)
            return;
        currentTutorialPhase++;
        Debug.Log("Start TutorialPhase : " + currentTutorialPhase);
        StartCoroutine(MoveToNextTutorialPhase_IE(0, endTime));
    }

    public IEnumerator<WaitForSeconds> MoveToNextTutorialPhase_IE(float currentTime, float endTime)//MoveToNextTutorialPhase_IEnumerator
    {
        if(endTime == 0f)
        {
            int phase = CurrentMode == ADFGVX.mode.Decoding ? DecodeTutorialPhaseArray[currentTutorialPhase] : EncodeTutorialPhaseArray[currentTutorialPhase];
            DisplayTutorialDialog(phase, 0f);
            yield break;
        }

        currentTime += endTime / 100;
        if (currentTime > endTime)
        {
            int phase = CurrentMode == ADFGVX.mode.Decoding ? DecodeTutorialPhaseArray[currentTutorialPhase] : EncodeTutorialPhaseArray[currentTutorialPhase];
            DisplayTutorialDialog(phase, 0f);
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(MoveToNextTutorialPhase_IE(currentTime, endTime));
    }

    public void DisplayTutorialDialog(int line, float endTime)//endTime을 대기한 후 다음 튜토리얼 대화를 표시
    {
        if (!PlayAsTutorial)
            return;
        SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);
        StartCoroutine(DisplayTutorialDialog_IE(line, 0, endTime));
    }

    public IEnumerator<WaitForSeconds> DisplayTutorialDialog_IE(int line, float currentTime, float endTime)//DisplayTutorialDialog_IEnumerator
    {
        if(endTime == 0f)
        {
            chat_ADFGVX.LoadLine(line);
            yield break;
        }

        currentTime += endTime / 100;
        if(currentTime>endTime)
        {
            chat_ADFGVX.LoadLine(line);
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(DisplayTutorialDialog_IE(line, currentTime, endTime));
    }
}
