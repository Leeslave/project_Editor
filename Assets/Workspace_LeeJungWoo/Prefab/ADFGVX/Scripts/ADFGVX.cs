using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ADFGVX : MonoBehaviour
{
    public enum mode//ADFGVX모드
    { Encoding,Decoding};
    public mode currentmode;//현재 ADFGVX모드

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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateInfoBox("아츠토츠카 표준 암호 시스템 V7 가동 정상");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlayAudioSource(3);
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
    }

    public void UpdateInfoBox(string Value)//InfoBox의 텍스트를 Value로 바꾼다
    {
        InfoBox.UpdateText(Value);
    }

    public void InformError(string Value)
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
