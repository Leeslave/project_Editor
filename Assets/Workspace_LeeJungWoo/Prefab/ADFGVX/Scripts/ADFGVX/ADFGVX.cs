using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX���
    { Encoding,Decoding};
    public mode currentmode;//���� ADFGVX���

    [Header("�ڷΰ��� ��ư")]
    public Button_QuitADFGVX quitButton;
    [Header("���� â ��Ʈ")]
    public InfoPart infoPart;
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

    public enum Audio
    {
        TextFlow, AddChar, DeleteChar, MouseClick, Error, DataProcessing,
    }
    private AudioSource audioSource;
    public AudioClip[] audioClips;

    private float totalElapsedTime;

    private void Start()
    {
        InformUpdate("��������ī ǥ�� ��ȣ �ý��� V7 ���� ����");

        if(currentmode == mode.Decoding)
        {
            afterDecodingPart.VisiblePart();
            encodeDataLoadPart.VisiblePart();
        }
        else if(currentmode == mode.Encoding)
        {
            guidearrowpart.Rotate180();

            beforeEncodingPart.VisiblePart();
            encodeDataSavePart.VisiblePart();
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        StopWatch();
    }

    public void SetPartLayer(int layer_ADFGVX, int layer_BiliteralSubstitution, int layer_Transposition, int layer_AfterDecode, int layer_BeforeEncode, int layer_EncodeDataSave, int layer_EncodeDataLoad, int layer_Verification)//��� ��Ʈ ���̾� ����
    {
        quitButton.gameObject.layer = layer_ADFGVX;
        biliteralsubstitutionpart.SetLayer(layer_BiliteralSubstitution);
        transpositionpart.SetLayer(layer_Transposition);
        afterDecodingPart.SetLayer(layer_AfterDecode);
        beforeEncodingPart.SetLayer(layer_BeforeEncode);
        encodeDataSavePart.SetLayer(layer_EncodeDataSave);
        encodeDataLoadPart.SetLayer(layer_EncodeDataLoad);
        verificationpart.SetLayer(layer_Verification);
    }

    public void SetPartLayerWaitForSec(float endTime, int layer_ADFGVX, int layer_BiliteralSubstitution, int layer_Transposition, int layer_AfterDecode, int layer_BeforeEncode, int layer_EncodeDataSave, int layer_EncodeDataLoad, int layer_Verification)//��� ��Ʈ ���̾� ����
    {
        StartCoroutine(SetPartLayerWaitForSecIEnumerator(endTime, 0, layer_ADFGVX, layer_BiliteralSubstitution, layer_Transposition, layer_AfterDecode, layer_BeforeEncode, layer_EncodeDataSave, layer_EncodeDataLoad, layer_Verification));
    }

    private IEnumerator SetPartLayerWaitForSecIEnumerator(float endTime, float currentTime, int layer_ADFGVX, int layer_BiliteralSubstitution, int layer_Transposition, int layer_AfterDecode, int layer_BeforeEncode, int layer_EncodeDataSave, int layer_EncodeDataLoad, int layer_Verification)//��� ��Ʈ ���̾� ����
    {
        currentTime += endTime / 100;
        if(currentTime>=endTime)
        {
            quitButton.gameObject.layer = layer_ADFGVX;
            biliteralsubstitutionpart.SetLayer(layer_BiliteralSubstitution);
            transpositionpart.SetLayer(layer_Transposition);
            afterDecodingPart.SetLayer(layer_AfterDecode);
            beforeEncodingPart.SetLayer(layer_BeforeEncode);
            encodeDataSavePart.SetLayer(layer_EncodeDataSave);
            encodeDataLoadPart.SetLayer(layer_EncodeDataLoad);
            verificationpart.SetLayer(layer_Verification);
            yield break;
        }

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(SetPartLayerWaitForSecIEnumerator(endTime, currentTime, layer_ADFGVX, layer_BiliteralSubstitution, layer_Transposition, layer_AfterDecode, layer_BeforeEncode, layer_EncodeDataSave, layer_EncodeDataLoad, layer_Verification));
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
        infoPart.UpdateText(value);
    }

    public void InformError(string value)//InfoBox�� ���� �߻� ������ ����
    {
        infoPart.UpdateText(value);
        PlayAudioSource(Audio.Error);
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

    private IEnumerator soundFlowIEnumerator(int length, int idx, float endTime)//���� �帧 ��� ���
    {
        if (idx >= length)
            yield break;
        PlayAudioSource(Audio.TextFlow);
        yield return new WaitForSeconds(endTime / length);
        StartCoroutine(soundFlowIEnumerator(length, idx + 1, endTime));
    }
}
