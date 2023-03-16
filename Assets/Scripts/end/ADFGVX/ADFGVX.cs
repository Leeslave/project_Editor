using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX���
    { Encoding, Decoding };

    [Header("���� ADFGVX ���")]
    public mode CurrentMode;

    [Header("Ʃ�丮��� ����")]
    public bool PlayAsTutorial;
    private int[] DecodeTutorialPhaseArray;
    private int[] EncodeTutorialPhaseArray;
    private int currentTutorialPhase;

    [Header("�ڷΰ��� ��ư")]
    public Button_ADFGVX_Quit quitButton;
    [Header("���� �α� ��Ʈ")]
    public DebugLog debugLog;
    [Header("ȭ��ǥ ��Ʈ")]
    public GuideArrowPart guidearrowpart;
    [Header("���� ���� ġȯ ��Ʈ")]
    public BiliteralSubstitutionPart biliteralsubstitutionpart;
    [Header("Ű ���� ��ġ ��Ʈ")]
    public TranspositionPart transpositionpart;
    [Header("��ȣȭ �� ��Ʈ")]
    public AfterDecodingPart afterDecodingPart;
    [Header("��ȣȭ �� ��Ʈ")]
    public BeforeEncodingPart beforeEncodingPart;
    [Header("��ȣȭ ������ �ε� ��Ʈ")]
    public EncodeDataLoadPart encodeDataLoadPart;
    [Header("��ȣȭ ������ ���� ��Ʈ")]
    public EncodeDataSavePart encodeDataSavePart;
    [Header("Ȯ�� â ��Ʈ")]
    public VerificationPanelPart verificationpart;
    [Header("Chat_ADFGVX")]
    public Chat_ADFGVX chat_ADFGVX;

    public enum Audio
    {
        TextFlow, AddChar, DeleteChar, MouseClick, DataProcessing
    }
    private AudioSource audioSource;
    public AudioClip[] audioClips;

    private float totalElapsedTime;

    private void Start()
    {
        //���� �˸�
        InformUpdate("��������ī ǥ�� ��ȣ �ý��� V7 ���� ����");

        //���� ��忡 ����
        if (CurrentMode == mode.Decoding)
        {
            afterDecodingPart.VisiblePart();
            encodeDataLoadPart.VisiblePart();
        }
        else if (CurrentMode == mode.Encoding)
        {
            guidearrowpart.Rotate180();
            beforeEncodingPart.VisiblePart();
            encodeDataSavePart.VisiblePart();
        }

        Invoke("SetTutorial", 1f);

        //����� �ҽ� ������Ʈ Ȯ��
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        StopWatch();
    }

    private void SetTutorial()
    {
        //Ʃ�丮�� ������ ���
        if (PlayAsTutorial)
        {
            if(CurrentMode == mode.Decoding)
                DecodeTutorialPhaseArray = chat_ADFGVX.GetListOfTutorialPhase().ToArray();
            else if(CurrentMode == mode.Encoding)
                EncodeTutorialPhaseArray = chat_ADFGVX.GetListOfTutorialPhase().ToArray();
            currentTutorialPhase = -1;
            MoveToNextTutorialPhase(0f);
        }
    }

    public void SetPartLayerWaitForSec(float endTime, int layer_quit, int layer_BiliteralSubstitution, int layer_Transposition, int layer_AfterDecode, 
        int layer_BeforeEncode, int layer_EncodeDataSave, int layer_EncodeDataLoad, int layer_Verification, int layer_DebugLog)//��� ��Ʈ ���̾� ����
    {
        StartCoroutine(SetPartLayerWaitForSec_IE(endTime, 0, layer_quit, layer_BiliteralSubstitution, layer_Transposition, layer_AfterDecode, layer_BeforeEncode, layer_EncodeDataSave, layer_EncodeDataLoad, layer_Verification, layer_DebugLog));
    }

    private IEnumerator<WaitForSeconds> SetPartLayerWaitForSec_IE(float endTime, float currentTime, int layer_quit, int layer_BiliteralSubstitution, int layer_Transposition, 
        int layer_AfterDecode, int layer_BeforeEncode, int layer_EncodeDataSave, int layer_EncodeDataLoad, int layer_Verification, int layer_DebugLog)//��� ��Ʈ ���̾� ����
    {
        if(endTime == 0f)
        {
            quitButton.gameObject.layer = layer_quit;
            biliteralsubstitutionpart.SetLayer(layer_BiliteralSubstitution);
            transpositionpart.SetLayer(layer_Transposition);
            afterDecodingPart.SetLayer(layer_AfterDecode);
            beforeEncodingPart.SetLayer(layer_BeforeEncode);
            encodeDataSavePart.SetLayer(layer_EncodeDataSave);
            encodeDataLoadPart.SetLayer(layer_EncodeDataLoad);
            verificationpart.SetLayer(layer_Verification);
            debugLog.SetLayer(layer_DebugLog); 
            yield break;
        }

        currentTime += endTime / 100;
        if (currentTime > endTime)
        {
            quitButton.gameObject.layer = layer_quit;
            biliteralsubstitutionpart.SetLayer(layer_BiliteralSubstitution);
            transpositionpart.SetLayer(layer_Transposition);
            afterDecodingPart.SetLayer(layer_AfterDecode);
            beforeEncodingPart.SetLayer(layer_BeforeEncode);
            encodeDataSavePart.SetLayer(layer_EncodeDataSave);
            encodeDataLoadPart.SetLayer(layer_EncodeDataLoad);
            verificationpart.SetLayer(layer_Verification);
            debugLog.SetLayer(layer_DebugLog); 
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(SetPartLayerWaitForSec_IE(endTime, currentTime, layer_quit, layer_BiliteralSubstitution, layer_Transposition, layer_AfterDecode, layer_BeforeEncode, layer_EncodeDataSave, layer_EncodeDataLoad, layer_Verification, layer_DebugLog));
    }

    public void StartStopWatch()//��ž��ġ �����
    {
        totalElapsedTime = 0;
    }

    private void StopWatch()//��ž��ġ
    {
        totalElapsedTime += Time.deltaTime;
    }

    public float GetTotalElapsedTime()//totalElapsedTime�� ��ȯ�մϴ�
    {
        return totalElapsedTime;
    }

    public bool ReturnDecodeScore()//��ȣȭ ����� ��ȯ�մϴ�
    {

        return EditStirng.CollectEnglishUpperAlphabet(encodeDataLoadPart.GetDecodedChiper()) == EditStirng.CollectEnglishUpperAlphabet(afterDecodingPart.GetInputField_Data().GetInputString());
    }

    public bool ReturnEncodeScore()//��ȣȭ ����� ��ȯ�մϴ�
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

    public void InformUpdate(string value)//InfoBox�� ������Ʈ�� ������ ����
    {
        debugLog.DebugInfo(value);
    }

    public void InformError(string value)//InfoBox�� ���� �߻� ������ ����
    {
        debugLog.DebugError(value);
    }

    public void PlayAudioSource(Audio value)//������ ���� ���
    {
        audioSource.clip = audioClips[((int)value)];
        audioSource.Play();
    }

    public void soundFlow(int length, float endTime)//���� �帧 ���
    {
        StartCoroutine(soundFlowIEnumerator(length, 0, endTime));
    }

    private IEnumerator<WaitForSeconds> soundFlowIEnumerator(int length, int idx, float endTime)//���� �帧 ��� ���
    {
        if (idx >= length)
            yield break;
        PlayAudioSource(Audio.TextFlow);
        yield return new WaitForSeconds(endTime / length);
        StartCoroutine(soundFlowIEnumerator(length, idx + 1, endTime));
    }










    //Ʃ�丮�� ����

    public int GetCurrentTutorialPhase()//���� Ʃ�丮�� �ܰ踦 ��ȯ�Ѵ�
    {
        return currentTutorialPhase;
    }

    public void MoveToNextTutorialPhase(float endTime)//endTime ���Ŀ� ���� Ʃ�丮�� �ܰ�� �Ѿ��
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

    public void DisplayTutorialDialog(int line, float endTime)//Ʃ�丮�� �� Ư�� �̺�Ʈ���� �ߵ�, ������ ���� ��ȭ�� ����
    {
        if (!PlayAsTutorial)
            return;
        SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2);
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
