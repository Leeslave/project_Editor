using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    /**
    * 업무 지원 프로그램 스크립트
    *   - 업무 안내 콘솔 기능
    *   - 해당 업무 실행
    *   - 업무 결과 돌려받기
    *   - 하루 업무 클리어
    */

    [SerializeField]
    List<Tuple<string, string>> workCodes = new List<Tuple<string, string>>();
    public AnimationController taskConsoleAnimation;
    public bool taskClear = false;
    public TMP_InputField consoleInput;

    ///////// 업무 코드명 /////////
    void Awake()
    {
        
    }

    // 창이 새로 열릴때마다 초기화 및 실행
    private void OnEnable()
    {
        // InputField 비활성화
        consoleInput.gameObject.SetActive(false);
        // 콘솔 초기화
        if (taskClear)
        {
            StartCoroutine(TaskConsoleAnimation(1));
        }
        else
        {
            StartCoroutine(TaskConsoleAnimation(0));
        }
    }

    // 창 종료시 모든 코루틴 종료
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// 텍스트 출력 후 InputField 설정
    private IEnumerator TaskConsoleAnimation(int idx)
    {
        // 콘솔 텍스트 출력
        taskConsoleAnimation.anims[idx].Clear();
        taskConsoleAnimation.Play(idx);
        yield return new WaitUntil(() => taskConsoleAnimation.isFinished);

        consoleInput.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
    }

    /// 업무 실행 이벤트 함수
    public void OnWorkEnter()
    {
        Debug.Log($"Work Entered! : {consoleInput.text}");
        foreach(var work in GameSystem.Instance.todayData.workData.Keys)
        {
            if(work.Item1 == consoleInput.text)
            {
                SceneManager.LoadScene(consoleInput.text);
            }
        }
    }
}
