using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : SpineAnimation
{
    private bool flip = false;
    private float time;

    private int currentTimeScale;
    private Coroutine ConvertTimeScaleCoroutine;

    public void StartMetronome()
    {
        time = 0;
        currentTimeScale = 0;
        SetCurrentAnimation(AnimState.Idle, true, 1f);

    }

    private void Update()
    {
        time += Time.deltaTime;

        if (skeletonAnimation.Skeleton.FindBone("bone3").Rotation >= 90 && flip)
        {
            flip = false;
            GetComponent<AudioSource>().Play();
            //Debug.Log(time);
            time = 0;
        }
        else if(skeletonAnimation.Skeleton.FindBone("bone3").Rotation <= 90 && !flip)
        {
            flip = true;
            GetComponent<AudioSource>().Play();
            //Debug.Log(time);
            time = 0;
        }

        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (currentTimeScale >= 1 && ConvertTimeScaleCoroutine == null)
            {
                currentTimeScale--;
                SetTimeScale(3);
            }
        }
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (currentTimeScale <= 1 && ConvertTimeScaleCoroutine == null)
            {
                currentTimeScale++;
                SetTimeScale(3);
            }
        }
    }

    private void SetTimeScale(float convertTime)
    {
        float[] value = { 1.0f, 1.5f, 2f };
        ConvertTimeScale(value[currentTimeScale], convertTime);
    }

    private void ConvertTimeScale(float target, float time)
    {
        if (ConvertTimeScaleCoroutine != null)
            return;
        ConvertTimeScaleCoroutine = StartCoroutine(ConvertTimeScaleIEnumerator(skeletonAnimation.timeScale, target, time, 0));
    }

    private IEnumerator ConvertTimeScaleIEnumerator(float current, float target, float time, float currentTime)
    {
        currentTime += time / 100;
        if(currentTime >= time)
            yield break;

        skeletonAnimation.timeScale = current + ((target - current) * (currentTime / time));
        Debug.Log("현재 timeSclae : " + skeletonAnimation.timeScale + ", 현재 가속 진행율 : " + (currentTime / time).ToString("F2"));
        
        yield return new WaitForSeconds(time / 100);
        ConvertTimeScaleCoroutine = StartCoroutine(ConvertTimeScaleIEnumerator(current, target, time, currentTime));
    }
}
