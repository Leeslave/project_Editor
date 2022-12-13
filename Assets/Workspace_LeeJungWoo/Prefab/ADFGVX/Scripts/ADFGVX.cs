using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX���
    { Encoding,Decoding};
    public mode currentmode;//���� ADFGVX���

    private AudioSource audioSource;
    [Header("����� Ŭ��")]
    public AudioClip[] electronicAudioClips;

    [Header("���� ���� ǥ��")]
    public Info InfoBox;
    [Header("���� ���� ġȯ ��Ʈ")]
    public BiliteralSubstitutionPart biliteralsubstitutionpart;
    [Header("��ġ ��Ʈ")]
    public TranspositionPart transpositionpart;
    [Header("�߰� ��Ʈ")]
    public IntermediatePart intermediatepart;
    [Header("��ȣ ��Ʈ")]
    public ChiperPart chiperpart;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateInfoBox("��������ī ǥ�� ��ȣ �ý��� V7 ���� ����");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlayAudioSource(3);
    }

    public void OnModeDown()//��� ��ȯ ��ư�� ������ ��
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
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
        }
        else if (currentmode == ADFGVX.mode.Decoding)
        {
            currentmode = ADFGVX.mode.Encoding;
            UpdateInfoBox("��� ��ȯ : ��ȣȭ");
        }
    }

    public void UpdateInfoBox(string Value)//InfoBox�� �ؽ�Ʈ�� Value�� �ٲ۴�
    {
        InfoBox.UpdateText(Value);
    }

    public void InformError(string Value)
    {
        InfoBox.UpdateText(Value);
        PlayAudioSource(4);
    }

    public void PlayAudioSource(int idx)//������ ���� ���
    {
        audioSource.clip = electronicAudioClips[idx];
        audioSource.Play();
    }

    public void soundFlow(int length, int idx, float endTime)//���� �帧 ���
    {
        StartCoroutine(soundFlowIEnumerator(length, idx, endTime));
    }

    private IEnumerator soundFlowIEnumerator(int length, int idx, float endTime)//���� �帧 ��� ���
    {
        if (idx >= length)
            yield break;
        PlayAudioSource(0);
        yield return new WaitForSeconds(endTime / length);
        StartCoroutine(soundFlowIEnumerator(length, idx + 1, endTime));
    }
}
