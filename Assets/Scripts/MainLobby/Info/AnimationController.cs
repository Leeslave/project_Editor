using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    /**
    *   애니메이션 실행
    *   - 각각의 애니메이션들을 리스트 순서대로 실행
    */
    public AnimBase[] anims;
    public bool isFinished;

    public void Play()
    {
        isFinished = false;
        StartCoroutine("PlayAnimInOrder");
    }
    
    public void Pause()
    {
        StopCoroutine("PlayAnimInOrder");
        isFinished = true;
    }

    IEnumerator PlayAnimInOrder()
    {
        foreach (AnimBase iter in anims)
        {
            yield return StartCoroutine(iter.Play());
        }
        isFinished = true;
    }
}
