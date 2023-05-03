using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    /**
    * 업무 지원 프로그램 스크립트
    *   - 업무 안내 콘솔 기능
    *   - 해당 업무 실행
    *   - 업무 결과 돌려받기
    *   - 하루 업무 클리어
    */

    public AnimationController taskConsoleAnimation;
    public bool taskClear = false;
    
    private void Awake()
    {
        taskConsoleAnimation.anims[0].Clear();
        taskConsoleAnimation.Play(0);
    }

    private void OnEnabled()
    {
        taskConsoleAnimation.anims[0].Clear();
        taskConsoleAnimation.Play(0);
    }
}
