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
        if (IsInit) { IsInit = false; return; }
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

    bool Fill = true;
    bool IsEnd = false;

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
        ValdBT.SetActive(false); OffBT.SetActive(true);
        IsEnd = true;
        gameObject.SetActive(false);
    }
}
