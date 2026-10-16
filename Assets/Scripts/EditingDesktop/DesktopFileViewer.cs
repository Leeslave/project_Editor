using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DesktopFileViewer : DesktopDropTarget
{

    [SerializeField] private TMP_Text _statusText;

    [SerializeField] public DesktopTabButton[] Tabs;

    [SerializeField] private GameObject[] _processes;

    [SerializeField] private GameObject _placeholderImage;

    [NonSerialized] private int _currentTabIndex;
    [NonSerialized] public int CurrentFileType = 0;
    private const string IdleMessage = "\n\n\n\nDrag Option File Here!";
    private const string ErrorMessage = "<size=40><color=#FF0000>404 Not Found</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
    private readonly string[] _loadingSteps =
        {
            "Decoding File...",
            "Identifying File Type...",
            "Collecting Information...",
            "Configuring UI..."
        };

    [SerializeField] private GameObject _windowRoot;
    // 파일을 열기 전 편집 화면을 숨긴다.
    private void Start()
    {
        _windowRoot.SetActive(false);
    }

    private const float LoadingStepDelay = 0.01f;
    private const float LoadingFinishDelay = 0.1f;
    // Manipulation
    protected override IEnumerator LoadProfileFile()
    {
        bool isAllowed = false;
        foreach (var person in EditingDesktopController.Instance.PersonDataManager.PeopleCorrect)
        {
            if (person.Item1 == EditingDesktopController.Instance.DragState.IconName)
                isAllowed = true;
        }

        // 금일 ToDoList에 있는 인물에만 접근 가능
        if (isAllowed)
        {
            EditingDesktopController.Instance.PersonDataManager.PeopleName = EditingDesktopController.Instance.DragState.IconName;
            CurrentFileType = 1;
            CommonBatch();
            _placeholderImage.SetActive(false);
            yield return ShowLoading();

            // 인물 정보 수정용 Process 활성화
            ActivateProcess(0);
        }
        else
        {
            _statusText.text = "<size=40><color=#FF0000>401 Not Unauthorized</color></size>\n<color=#C8AF10>(x_x)</color>\n\nOops! Something's Wrong.\nDrag Correct Option File Here!";
            CanAttach = true;
        }
    }

    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private TMP_Text _reporter;

    // News
    protected override IEnumerator LoadNewsFile()
    {
        CommonBatch();
        CurrentFileType = 2;
        _placeholderImage.SetActive(false);
        News curNews = EditingDesktopController.Instance.FindNews(EditingDesktopController.Instance.DragState.IconName);
        EditingDesktopController.Instance.NewsManager.CurNews = curNews;
        yield return ShowLoading();

        // News에 Text 추가
        _title.text = curNews.Title;
        _date.text = curNews.Date;
        _reporter.text = curNews.Reporter;
        for (int i = 0; i < curNews.Main.Count; i++)
        {
            EditingDesktopController.Instance.NewsManager.ActiveText(curNews.Main[i]);
        }

        // News 수정용 Process 활성화
        ActivateProcess(1);
    }

    // 파일 읽기 진행률을 표시하고 드롭 입력을 다시 허용한다.
    private IEnumerator ShowLoading()
    {
        string completedSteps = "";
        WaitForSeconds stepDelay = new WaitForSeconds(LoadingStepDelay);
        foreach (string step in _loadingSteps)
        {
            completedSteps += step;
            for (int progress = 0; progress <= 10; progress++)
            {
                _statusText.text = completedSteps + $"<size=20>{progress * 10}% </size>";
                yield return stepDelay;
            }
            completedSteps += " <size=20>Complete!\n</size>";
        }
        _statusText.text = completedSteps + "\n\nEnd!\n\n Wait a little...";
        yield return new WaitForSeconds(LoadingFinishDelay);

        _statusText.text = IdleMessage;
        _placeholderImage.SetActive(true);
        CanAttach = true;
    }

    // 선택한 편집 화면을 표시하고 앞으로 가져온다.
    private void ActivateProcess(int index)
    {
        GameObject process = _processes[index];
        process.SetActive(true);
        process.transform.SetAsLastSibling();
        process.transform.position = Vector3.zero;
    }

    public void CommonBatch()
    {
        foreach (GameObject s in _processes) s.SetActive(false);
        _statusText.text = IdleMessage;
        _placeholderImage.SetActive(true);
    }

    protected override void ShowDropError()
    {
        _statusText.text = ErrorMessage;
    }

    public void ChangeTab(int index)
    {
        Tabs[_currentTabIndex].CloseTab();
        _currentTabIndex = index;
    }

    // 편집 화면이 닫히면 드롭 입력을 다시 허용한다.
    private void OnDisable()
    {
        CanAttach = true;
    }
}
