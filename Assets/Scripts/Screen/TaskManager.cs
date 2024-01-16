using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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

    /// 업무 완료 확인
    void OnEnable()
    {
        if (GameSystem.Instance.isTaskClear == true)
        {
            GameSystem.Instance.SetTime(2);
        }
    }

    /// 업무창 활성화/비활성화
    public void ActiveTaskWindow()
    {
        // 업무창 활성화
        if (!taskWindow.activeSelf)
        {
            taskWindow.SetActive(true);     // 오브젝트 활성화
            consoleInput.gameObject.SetActive(false);   //입력창 비활성화
            // 콘솔 대사 출력
            if (GameSystem.Instance.isTaskClear)
            {
                StartCoroutine(TaskConsoleAnimation(1));
            }
            else
            {
                StartCoroutine(TaskConsoleAnimation(0));
            }
        }
        // 업무창 비활성화
        else
        {
            taskConsoleAnimation.Pause();   // 업무 대사 중지
            StopAllCoroutines();            // 모든 코루틴 중지
            taskWindow.SetActive(false);    // 오브젝트 비활성화
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
        if (idx == 0)
        {
            consoleInput.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
        }
    }

    /// 업무 실행 이벤트 함수
    public void OnWorkEnter()
    {
        foreach(var work in GameSystem.Instance.today.workList.Keys)
        {
            if(work.code == consoleInput.text)
            {
                Debug.Log($"Work Entered! : {consoleInput.text}");
                consoleInput.text = "업무 로딩중...\n";
                GameSystem.LoadScene(work.code);
                return;
            }
        }
    }
}
