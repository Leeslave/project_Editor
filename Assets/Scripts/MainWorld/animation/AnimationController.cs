using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour
{
    /**
    *   애니메이션 실행
    *   - 각각의 애니메이션들을 리스트 순서대로 실행
    *   - 리스트 내 특정 애니메이션들만 실행
    */
    public AnimBase[] anims;    // 애니메이션 리스트
    public bool isFinished;     // 애니메이션 

    /// <summary>모든 애니메이션 순차 실행</summary>
    public void Play()
    {
        isFinished = false;
        StartCoroutine("PlayAnimInOrder");
    }

    /// <summary>특정 애니메이션 실행</summary>
    /// <remarks>리스트내의 특정 애니메이션만 실행 with index</remarks>
    /// <param name=index>실행시킬 애니메이션의 index</param>
    public void Play(int index)
    {
        isFinished = false;
        StartCoroutine(PlayAnimEach(index));
    }

    /// <summary>애니메이션 중지</summary>
    public void Pause()
    {
        StopAllCoroutines();
        isFinished = true;
    }

    /// 순서대로 모든 애니메이션 출력
    IEnumerator PlayAnimInOrder()
    {
        foreach (AnimBase iter in anims)
        {
            yield return StartCoroutine(iter.Play());
        }
        isFinished = true;
    }

    /// 리스트 내 애니메이션만 순서대로 출력
    IEnumerator PlayAnimEach(int index)
    {
        yield return StartCoroutine(anims[index].Play());
        isFinished = true;
    }
}
