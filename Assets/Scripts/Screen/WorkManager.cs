using GameService;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Utility;

public class WorkManager : MonoBehaviour
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
    public AnimationController workConsoleAnimation;    //업무 대화 콘솔 애니메이션
    public TMP_InputField consoleInput;     // 업무 입력 창

    private bool _isEntering;

    private void Awake()
    {
        // 기존 입력창의 글꼴과 배치를 재사용하여 안내용 텍스트 생성
        if (consoleInput.placeholder == null)
        {
            TMP_Text placeholder = Instantiate(consoleInput.textComponent,
                consoleInput.textComponent.transform.parent);
            placeholder.name = "Placeholder";
            placeholder.text = "";
            placeholder.raycastTarget = false;
            consoleInput.placeholder = placeholder;
        }
    }

    /// 업무창 활성화/비활성화
    public void ActiveWorkWindow()
    {
        // 업무창 활성화
        if (!taskWindow.activeSelf)
        {
            taskWindow.SetActive(true);     // 오브젝트 활성화
            //closeButton.SetActive(false);
            consoleInput.gameObject.SetActive(false);   //입력창 비활성화
            // 콘솔 대사 출력
            StartCoroutine(WorkService.IsWorkClear() ? WorkConsoleAnimation(1) : WorkConsoleAnimation(0));
        }
    }

    /// 텍스트 출력 후 InputField 설정
    private IEnumerator WorkConsoleAnimation(int idx)
    {
        // 콘솔 텍스트 출력
        workConsoleAnimation.anims[idx].Clear();
        workConsoleAnimation.Play(idx);
        yield return new WaitUntil(() => workConsoleAnimation.isFinished);
        //closeButton.SetActive(true);

        // 텍스트 출력 후 입력창 활성화
        if (idx == 0)
        {
            consoleInput.text = "";
            consoleInput.interactable = true;
            consoleInput.gameObject.SetActive(true);
            ShowWorkMessage("업무 코드 또는 이름 입력: " + string.Join(", ",
                WorkService.GetList().Select(work => work.name == work.code
                    ? work.code : $"{work.name} ({work.code})")));
            EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
        }
    }

    /// 업무 실행 이벤트 함수
    public void OnWorkEnter()
    {
        if (_isEntering || !consoleInput.gameObject.activeInHierarchy ||
            !consoleInput.interactable || GameSystem.Instance.IsLoading) return;

        string input = consoleInput.text.Trim();
        if (string.IsNullOrEmpty(input))
        {
            ShowWorkMessage("업무 코드 또는 이름을 입력하세요.");
            return;
        }

        var works = WorkService.GetList();
        // 업무 코드를 먼저 비교하고, 코드가 없을 때만 표시 이름으로 검색
        var matches = works.Where(work => work.code == input).ToList();
        if (matches.Count == 0)
            matches = works.Where(work => work.name == input).ToList();

        if (matches.Count != 1)
        {
            ShowWorkMessage(matches.Count == 0
                ? "등록된 업무가 없습니다. 업무 코드를 확인하세요."
                : "같은 이름의 업무가 있습니다. 업무 코드로 입력하세요.");
            return;
        }

        string workCode = matches[0].code;
        if (WorkService.IsWorkClear(workCode))
        {
            ShowWorkMessage("이미 완료한 업무입니다. 다른 업무를 선택하세요.");
            return;
        }
        if (!WorkService.TryStartWork(workCode, out string sceneName))
        {
            ShowWorkMessage("업무를 불러올 수 없습니다. 다른 업무를 선택하세요.");
            return;
        }

        _isEntering = true;
        EditorLogger.Log($"Work Entered! : {workCode}");
        consoleInput.interactable = false;
        consoleInput.text = "업무 로딩중...\n";
        GameSystem.Instance.EnterScene(sceneName);
    }

    /// <summary>
    /// 입력창에 업무 안내를 표시하고 다시 입력할 수 있도록 초점 설정
    /// </summary>
    private void ShowWorkMessage(string message)
    {
        consoleInput.text = "";
        if (consoleInput.placeholder is TMP_Text placeholder)
            placeholder.text = message;
        consoleInput.ActivateInputField();
    }
}
