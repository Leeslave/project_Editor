using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValidTask_D : MonoBehaviour
{
    [SerializeField] GameObject Check;

    [SerializeField] Image ProgBar;
    [SerializeField] TMP_Text InfText;

    [SerializeField] GameObject OffBT;

    [SerializeField] GameObject ValdBT;

    string[] Test =
    {
        "Gathering Information...\n",
        "Checking Person Task...",
        "Checking News Task...",
        "Checking Documents Task...",
    };
    private void Start()
    {
        OffBT.SetActive(false);
        gameObject.SetActive(false);
    }

    bool IsInit = true;
    private void OnEnable()
    {
        // 최초 생성시 OnEnable 활성화 방지
        if (IsInit) { IsInit = false; return; }

        // GameClear
        if (IsEnd)
        {
            GameSystem.Instance.ClearTask("Document");
            GameSystem.LoadScene("Screen");
            gameObject.SetActive(false);
            return;
        }
        InfText.text = "";
        ProgBar.fillAmount = 0;
        Check.gameObject.SetActive(true);
        Fill = true;
        StartCoroutine(ValidStart());
    }

    bool Fill = true;       // true인 경우에만 ProgressBar를 채움
    bool IsEnd = false;     // Object가 활성화 됬을 때 false면 업무 평가를, true인 경우 업무를 종료한다.

    private void FixedUpdate()
    {
        if(Fill && ProgBar.fillAmount<1)ProgBar.fillAmount += Time.deltaTime * 0.25f;
    }

    IEnumerator ValidStart()
    {
        WaitForSeconds WFS = new WaitForSeconds(1f);
        WaitForSeconds WFS2 = new WaitForSeconds(3f);
        int[] Scores = new int[3];
        DB_M.DB_Docs.EvaluateWork(ref Scores);

        InfText.text = Test[0];
        yield return WFS;
        InfText.text += Test[1];
        yield return WFS;
        // Manipulation 평가
        if (Scores[0] != 0)
        {
            InfText.text += "\n<color=red>Unperformed tasks Detected! :(</color>\nAutomatically shut down after a while...";
            Fill = false;
            yield return WFS2;
            gameObject.SetActive(false);
        }
        InfText.text += "           Complete!\n";
        InfText.text += Test[2];
        yield return WFS;
        // News 평가
        if (Scores[1] != 0)
        {
            InfText.text += "\n<color=red>Unperformed tasks Detected! :(</color>\nAutomatically shut down after a while...";
            Fill = false;
            yield return WFS2;
            gameObject.SetActive(false);
        }

        InfText.text += "           Complete!\n";
        yield return WFS;
        InfText.text += Test[3];
        // Docs 평가
        if (Scores[2] != 0)
        {
            InfText.text += "\n<color=red>Unperformed tasks Detected! :(</color>\nAutomatically shut down after a while...";
            Fill = false;
            yield return WFS2;
            gameObject.SetActive(false);
        }
        InfText.text += "           Complete!\nAll tasks performed :D\nAutomatically shut down after a while...";
        Fill = false;
        yield return WFS2;
        // 모든 업무가 수행되었으면 ValidWalk Icon을 비활성화 하고 EndTask Icon을 활성화함.
        ValdBT.SetActive(false); OffBT.SetActive(true);
        IsEnd = true;
        gameObject.SetActive(false);
    }
}
