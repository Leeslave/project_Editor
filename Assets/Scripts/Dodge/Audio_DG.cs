using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_DG : MonoBehaviour
{
    [SerializeField] AudioClip originClip;
    [SerializeField] AudioClip BossClip;
    AudioClip NoiszeClip;
    [SerializeField] AudioSource Audio;
    [SerializeField] float NoiszeAmount;

    List<AudioClip> Clips;

    private void Awake()
    {
        LastTime = InitTime;
        NoiszeClip = CreateNoisyClip(originClip, NoiszeAmount);
        Clips = new List<AudioClip> {originClip,BossClip};

    }

    private AudioClip CreateNoisyClip(AudioClip clip, float noiseAmount)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // 노이즈를 추가하여 새로운 샘플 배열 생성
        for (int i = 0; i < samples.Length / 50; i++)
        {
            samples[i] += Random.Range(-noiseAmount, noiseAmount);
        }



        // 새로운 AudioClip 생성
        AudioClip noisyClip = AudioClip.Create("NoisyClip", samples.Length / clip.channels, clip.channels, clip.frequency / 2, false);
        noisyClip.SetData(samples, 0);

        return noisyClip;
    }

    [SerializeField] float InitTime;
    float LastTime = 0;
    bool IsNoise = false;
    
    public void NoiszeOn()
    {
        IsNoise = true; LastTime = Audio.time < InitTime? InitTime : Audio.time;

        Audio.clip = NoiszeClip;  Audio.time = InitTime; Audio.Play();
    }

    int CurPlaying = 0;
    public void MusicOn(int ClipType = 0)
    {
        if(!IsNoise && Audio.isPlaying && ClipType == CurPlaying) return;
        if (CurPlaying != ClipType || IsNoise) Audio.clip = Clips[ClipType];
        else Audio.time = LastTime;
        CurPlaying = ClipType; IsNoise = false;
        Audio.Play();
    }

    public void MusicOff(bool Reset = false)
    {
        if (!Audio.isPlaying) return;
        LastTime = Reset ? InitTime : Audio.time;  Audio.Stop();
    }
}
