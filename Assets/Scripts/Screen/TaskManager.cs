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

    public GameObject taskWindow;       // 업무 프로그램 창
    public AnimationController taskConsoleAnimation;    //업무 대화 콘솔 애니메이션
    public TMP_InputField consoleInput;     // 업무 입력 창
    public bool taskClear   // 모든 업무 완료 플래그
    {
        get { 
            bool taskResult = true;
            foreach(var work in GameSystem.Instance.today.workData)
            {
                taskResult = taskResult & work.isClear;
            }
            return taskResult;
        }
    }

    /// 업무 완료 확인 TODO: 업무 클리어 후 돌아올때 확인으로 수정 필요
    void OnEnable()
    {
        if (taskClear == true)
        {
            GameSystem.Instance.SetTime();
        }
    }

    /// 창 열고 닫기
    public void ActiveTaskWindow()
    {
        if (!taskWindow.activeSelf)
        {
            // 업무 창 활성화
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
            taskConsoleAnimation.Pause();
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

        // 텍스트 출력 후 입력창 활성화
        consoleInput.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
    }

    /// 업무 실행 이벤트 함수
    public void OnWorkEnter()
    {
        foreach(var work in GameSystem.Instance.today.workData)
        {
            if(work.code == consoleInput.text)
            {
                Debug.Log($"Work Entered! : {consoleInput.text}");
                consoleInput.text = "업무 로딩중...\n";
                StartCoroutine(LoadWorkAsync(work.code));
                return;
            }
        }
    }

    /// 씬 비동기 로딩
    IEnumerator LoadWorkAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
