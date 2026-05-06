using GameAction;
using GameService;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Utility;

public class TaskManager : MonoBehaviour
{
    /**
    * 업무 지원 프로그램 스크립트
    *   - 업무 안내 콘솔 기능
    *   - 해당 업무 실행
    *   - 업무 결과 돌려받기
    *   - 하루 업무 클리어
    */
    
    // Inject
    private IWorkService WorkService => GameSystem.GetService<IWorkService>();

    public GameObject taskWindow;       // 업무 프로그램 창
    public AnimationController taskConsoleAnimation;    //업무 대화 콘솔 애니메이션
    public TMP_InputField consoleInput;     // 업무 입력 창
    //public GameObject closeButton;      // 업무창 닫기 버튼

    private void Start()
    {
        WorkService.OnWorkClear += FinishWork;
    }

    private void OnDestroy()
    {
        WorkService.OnWorkClear -= FinishWork;
    }

    private static void FinishWork()
    {
        new NextTimeAction(2).Invoke();
    }

    /// 업무창 활성화/비활성화
    public void ActiveTaskWindow()
    {
        // 업무창 활성화
        if (!taskWindow.activeSelf)
        {
            taskWindow.SetActive(true);     // 오브젝트 활성화
            //closeButton.SetActive(false);
            consoleInput.gameObject.SetActive(false);   //입력창 비활성화
            // 콘솔 대사 출력
            StartCoroutine(WorkService.IsWorkClear() ? TaskConsoleAnimation(1) : TaskConsoleAnimation(0));
        }
    }

    /// 텍스트 출력 후 InputField 설정
    private IEnumerator TaskConsoleAnimation(int idx)
    {
        // 콘솔 텍스트 출력
        taskConsoleAnimation.anims[idx].Clear();
        taskConsoleAnimation.Play(idx);
        yield return new WaitUntil(() => taskConsoleAnimation.isFinished);
        //closeButton.SetActive(true);

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
        foreach(var work in WorkService.GetList())
        {
            if(work == consoleInput.text)
            {
                EditorLogger.Log($"Work Entered! : {consoleInput.text}");
                consoleInput.text = "업무 로딩중...\n";
                GameSystem.Instance.EnterScene(work);
                return;
            }
        }
    }
}
