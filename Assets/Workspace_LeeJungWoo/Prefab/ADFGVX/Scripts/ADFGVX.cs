using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX모드
    { Encoding,Decoding};
    public mode currentmode;//현재 ADFGVX모드

    private string decodedChiper;
    private string encodedChiper;

    private AudioSource audioSource;
    [Header("오디오 클립")]
    public AudioClip[] electronicAudioClips;

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
    [Header("확인 창")]
    public VerificationPanelPart verificationpart;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InformUpdate("아츠토츠카 표준 암호 시스템 V7 가동 정상");
    }

    public void SetPartLayer(int layer_biliteralsubstitution, int layer_intermediate, int layer_transposition, int layer_chiper)//모든 파트 레이어 설정
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

    public bool ReturnDecodeScore()//복호화 결과를 반환합니다
    {
        return CollectEnglishAlphabet(decodedChiper) == CollectEnglishAlphabet(intermediatepart.GetIntermediateChiper());
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

    public void InformUpdate(string Value)//InfoBox에 업데이트한 사항을 띄운다
    {
        InfoBox.UpdateText(Value);
    }

    public void InformError(string Value)//InfoBox에 에러 발생 사항을 띄운다
    {
        InfoBox.UpdateText(Value);
        PlayAudioSource(4);
    }

    public void PlayAudioSource(int idx)//선택한 사운드 재생
    {
        audioSource.clip = electronicAudioClips[idx];
        audioSource.Play();
    }

    public void soundFlow(int length, int idx, float endTime)//사운드 흐름 재생
    {
        StartCoroutine(soundFlowIEnumerator(length, idx, endTime));
    }

    private IEnumerator soundFlowIEnumerator(int length, int idx, float endTime)//사운드 흐름 재생 재귀
    {
        if (idx >= length)
            yield break;
        PlayAudioSource(0);
        yield return new WaitForSeconds(endTime / length);
        StartCoroutine(soundFlowIEnumerator(length, idx + 1, endTime));
    }
}
