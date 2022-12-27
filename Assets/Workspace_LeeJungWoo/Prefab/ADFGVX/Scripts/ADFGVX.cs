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

    private string decodedChiper;
    private string encodedChiper;

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
    [Header("Ȯ�� â")]
    public VerificationPanelPart verificationpart;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InformUpdate("��������ī ǥ�� ��ȣ �ý��� V7 ���� ����");
    }

    public void SetPartLayer(int layer_biliteralsubstitution, int layer_intermediate, int layer_transposition, int layer_chiper)//��� ��Ʈ ���̾� ����
    {
        //Debug.Log(layer_biliteralsubstitution.ToString() + ',' + layer_intermediate.ToString() + ',' + layer_transposition.ToString() + ',' + layer_chiper.ToString());
        biliteralsubstitutionpart.SetLayer(layer_biliteralsubstitution);
        intermediatepart.SetLayer(layer_intermediate);
        transpositionpart.SetLayer(layer_transposition);
        chiperpart.SetLayer(layer_chiper);
    }

    public void SetDecodedChiper(string value)
    {
        decodedChiper = value;
    }

    public string GetDecodeChiper()
    {
        return decodedChiper;
    }

    public void SetEncodedChiper(string value)
    {
        encodedChiper = value;
    }

    public string GetEncodedChiper()
    {
        return encodedChiper;
    }

    public bool ReturnDecodeScore()//��ȣȭ ����� ��ȯ�մϴ�
    {
        return CollectEnglishAlphabet(decodedChiper) == CollectEnglishAlphabet(intermediatepart.GetIntermediateChiper());
    }

    private string CollectEnglishAlphabet(string value)//��ĭ, ���� ���� �����ϰ� ���� ���ĺ��� ��Ƽ� ��ȯ�Ѵ�
    {
        //array0�� ����ִ� ���ĺ� ���� Ȯ��, ���Ӱ� ������� array1�� ���� Ȯ��
        int newarraylenght = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] >= 'A' && value[i] <= 'Z' || value[i] == '_')
                newarraylenght++;
        }

        //array0�� ���ĺ� ���� ��ŭ array �Ҵ�
        char[] array = new char[newarraylenght];
        int idx = 0;

        //array01�� idx�� �÷����鼭 ���ĺ� ���� ����
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

    public void InformUpdate(string Value)//InfoBox�� ������Ʈ�� ������ ����
    {
        InfoBox.UpdateText(Value);
    }

    public void InformError(string Value)//InfoBox�� ���� �߻� ������ ����
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
