using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 배경음악 스택
    private static readonly List<SoundManager> bgmChannel = new();
    [SerializeField] private bool onBGMChannel;
    
    [SerializeField] private AudioSource audioSource;  // 오디오소스
    public List<AudioClip> clips;   // 사용할 오디오 클립들
    [SerializeField] private float overlapDelay = 0.5f;  // 오버랩 딜레이
    
    // 플레이 상태
    public enum AudioState { Stopped, Playing, Paused }
    
    private AudioState OnPlay
    {
        get
        {
            if (audioSource.isPlaying) return AudioState.Playing;
            return audioSource.time == 0 ? AudioState.Stopped : AudioState.Paused;
        }
    }

    # region Control
    
    // 일반 재생
    public void Play()
    {
        if (onBGMChannel)
        {
            if (bgmChannel.Count > 0)
            {
                bgmChannel.Last().Pause();
            }
            bgmChannel.Add(this);
        }
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
        if (onBGMChannel)
        {
            bgmChannel.Remove(this);
            ResumeBGM();
        }
    }

    // 일시 정지
    public void Pause()
    {
        audioSource.Pause();
    }

    // 재개
    public void Resume()
    {
        if (onBGMChannel)
        {
            if (bgmChannel.Count > 0)
            {
                bgmChannel.Last().Pause();
            }
            bgmChannel.Remove(this);
            bgmChannel.Add(this);
        }
        audioSource.UnPause();
    }

    // 반복 설정
    public void Loop(bool isLoop)
    {
        audioSource.loop = isLoop;
    }
    
    # endregion
    
    #region Global BGM

    public static void ResumeBGM()
    {
        if (bgmChannel.Count > 0)
        {
            bgmChannel.Last().Resume();
        }
    }

    public static void PauseBGM()
    {
        if (bgmChannel.Count > 0)
        {
            bgmChannel.Last().Pause();
        }
    }

    #endregion


    public void  SetClip(int idx, bool swap = false)
    {
        bool playing = OnPlay == AudioState.Playing;
        if (idx < 0 || idx >= clips.Count)
        {
            return;
        }
        if (audioSource.clip == clips[idx])
        {
            return;
        }
         
        audioSource.clip = clips[idx];
        if (swap && playing)
        {
            Play();
        }
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
        if (OnPlay ==  AudioState.Playing)
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
