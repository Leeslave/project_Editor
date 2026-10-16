using System.Collections;
using GameService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditingTaskEvaluator : MonoBehaviour
{
    [SerializeField] private GameObject _checkPanel;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private GameObject _finishButton;
    [SerializeField] private GameObject _evaluateButton;
    private bool _initializing = true;
    private bool _evaluating;
    private bool _completed;
    private IWorkService WorkService => GameSystem.GetService<IWorkService>();

    // 평가 화면과 업무 종료 버튼을 처음에는 숨긴다.
    private void Start()
    {
        _finishButton.SetActive(false);
        gameObject.SetActive(false);
    }

    // 첫 활성화를 제외하고 평가 또는 업무 종료를 실행한다.
    private void OnEnable()
    {
        if (_initializing) { _initializing = false; return; }
        if (_completed)
        {
            WorkService.ClearWork(EditingDesktopController.Instance.WorkCode);
            GameSystem.Instance.EnterScene("Screen");
            gameObject.SetActive(false);
            return;
        }
        _statusText.text = "";
        _progressBar.fillAmount = 0;
        _checkPanel.SetActive(true);
        _evaluating = true;
        StartCoroutine(EvaluateTasks());
    }

    // 평가 연출 중에만 진행 막대를 채운다.
    private void FixedUpdate()
    {
        if (_evaluating) _progressBar.fillAmount = Mathf.Min(1, _progressBar.fillAmount + Time.deltaTime * 0.25f);
    }

    // 인물과 뉴스 평가를 차례로 표시하고 완료 버튼을 전환한다.
    private IEnumerator EvaluateTasks()
    {
        var stepDelay = new WaitForSeconds(1);
        var finishDelay = new WaitForSeconds(3);
        int[] scores = EditingDesktopController.Instance.EvaluateWork();
        string[] labels = { "Checking Person Task...", "Checking News Task..." };
        _statusText.text = "Gathering Information...\n";
        yield return stepDelay;
        for (int i = 0; i < scores.Length; i++)
        {
            _statusText.text += labels[i];
            yield return stepDelay;
            if (scores[i] != 0)
            {
                _statusText.text += "\n<color=red>Unperformed tasks Detected! :(</color>\nAutomatically shut down after a while...";
                _evaluating = false;
                yield return finishDelay;
                gameObject.SetActive(false);
                yield break;
            }
            _statusText.text += "           Complete!\n";
        }
        _statusText.text += "All tasks performed :D\nAutomatically shut down after a while...";
        _evaluating = false;
        yield return finishDelay;
        _evaluateButton.SetActive(false);
        _finishButton.SetActive(true);
        _completed = true;
        gameObject.SetActive(false);
    }
}
