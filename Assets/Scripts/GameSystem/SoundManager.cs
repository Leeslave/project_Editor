using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;  // 오디오소스
    public List<AudioClip> clips;   // 사용할 오디오 클립들
    [SerializeField]
    private float overlapDelay = 0.5f;  // 오버랩 딜레이
    private bool onPlay => audioSource.isPlaying; // 현재 플레이 상태

    // 일반 재생
    public void Play()
    {
        audioSource.Play();
    }

    // 샷 재생
    public void PlayShot()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }

    // 일반 정지
    public void Stop()
    {
        audioSource.Stop();
    }

    // 일시 정지
    public void Pause()
    {
        audioSource.Pause();
    }

    // 재개
    public void Resume()
    {
        audioSource.UnPause();
    }

    // 반복 설정
    public void Loop(bool isLoop)
    {
        audioSource.loop = isLoop;
    }

    

    public void SetClip(int _idx)
    {
        if (_idx < 0 || _idx >= clips.Count)
        {
            return;
        }
        audioSource.clip = clips[_idx];
    }

    // 오버랩 재생
    public void OverlapPlay(int idx)
    {
        if (audioSource.clip == clips[idx])
            return;
        StartCoroutine(Overlap(idx));
    }

    // 오버랩 코루틴
    private IEnumerator Overlap(int newClip)
    {   
        // 페이드 아웃
        if (onPlay)
            yield return StartCoroutine(FadeOut());

        SetClip(newClip);
        Play();

        // 페이드 인
        yield return StartCoroutine(FadeIn());
    }

    // 페이드 아웃 코루틴
    private IEnumerator FadeOut()
    {
        float timer = 0;
        float startVolume = audioSource.volume;

        while (timer < overlapDelay)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / overlapDelay);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // 페이드 인 코루틴
    private IEnumerator FadeIn()
    {
        float timer = 0;
        float startVolume = 0;

        audioSource.volume = startVolume;

        while (timer < overlapDelay)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 1f, timer / overlapDelay);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
