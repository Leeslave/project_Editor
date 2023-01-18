using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : SpineAnimation
{
    private bool flip = false;

    private int currentTimeScale;
    private Coroutine ConvertTimeScaleCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentTimeScale = 0;
        SetCurrentAnimation(AnimState.Idle, true, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (skeletonAnimation.Skeleton.FindBone("bone3").Rotation > 90 && flip)
        {
            flip = false;
            GetComponent<AudioSource>().Play();
        }
        else if(skeletonAnimation.Skeleton.FindBone("bone3").Rotation < 90 && !flip)
        {
            flip = true;
            GetComponent<AudioSource>().Play();
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
        float[] value = { 1.2f, 1.8f, 2.4f };
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
