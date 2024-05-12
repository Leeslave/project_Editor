using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource Audio = new();  // 오디오소스
    public List<AudioClip> clips = new();   // 사용할 오디오 클립들
    [SerializeField]
    private float overlapDelay = 0.5f;  // 오버랩 딜레이
    public bool isPlaying { get { return Audio.isPlaying; } }   // 현재 플레이 상태

    // 일반 재생
    public void Play()
    {
        Audio.Play();
    }

    // 일반 정지
    public void Stop()
    {
        Audio.Stop();
    }

    // 일시 정지
    public void Pause()
    {
        Audio.Pause();
    }

    // 재개
    public void Resume()
    {
        Audio.UnPause();
    }

    // 반복 설정
    public void loop(bool isLoop)
    {
        Audio.loop = isLoop;
    }

    

    public void SetClip(int _idx)
    {
        if (_idx < 0 || _idx >= clips.Count)
        {
            return;
        }
        Audio.clip = clips[_idx];
    }

    // 오버랩 재생
    public void OverlapPlay(int idx)
    {
        if (Audio.clip == clips[idx])
            return;
        StartCoroutine(Overlap(idx));
    }

    // 오버랩 코루틴
    private IEnumerator Overlap(int newClip)
    {   
        // 페이드 아웃
        if (isPlaying)
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
        float startVolume = Audio.volume;

        while (timer < overlapDelay)
        {
            Audio.volume = Mathf.Lerp(startVolume, 0f, timer / overlapDelay);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    // 페이드 인 코루틴
    private IEnumerator FadeIn()
    {
        float timer = 0;
        float startVolume = 0;

        Audio.volume = startVolume;

        while (timer < overlapDelay)
        {
            Audio.volume = Mathf.Lerp(startVolume, 1f, timer / overlapDelay);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
