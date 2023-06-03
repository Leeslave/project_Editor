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
    List<Tuple<string, string>> workCodes = new List<Tuple<string, string>>();  // 업무 코드명
    public GameObject taskWindow;
    public AnimationController taskConsoleAnimation;    //업무 대화 콘솔 애니메이션
    public bool taskClear   // 모든 업무 완료 플래그
    {
        get { 
            bool taskResult = true;
            foreach(var work in GameSystem.Instance.todayData.workData.Keys)
            {
                taskResult = taskResult & GameSystem.Instance.todayData.workData[work];
            }
            return taskResult;
        }
    }
    public TMP_InputField consoleInput;

    ///////// 업무 코드명 /////////
    void Awake()
    {
        
    }

    /// 창 열고 닫기
    public void ActiveTaskWindow()
    {
        if (!taskWindow.activeSelf)
        {
            Debug.Log(GameSystem.Instance.todayData.workData);
            foreach(var work in GameSystem.Instance.todayData.workData.Keys)
            {
                Debug.Log($"Work-{work.Item1} Clear: {GameSystem.Instance.todayData.workData[work]}");
            }
            Debug.Log($"All Work: {taskClear}");
            taskWindow.SetActive(true);
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
        else
        {
            StopAllCoroutines();
            taskWindow.SetActive(false);
        }
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
                return;
            }
        }
    }
}
