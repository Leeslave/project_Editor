using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// <summary>
/// Maze의 Timer를 담당하는 Script
/// </summary>
public class MazeTimer : MonoBehaviour
{
    public GameObject Warning;
    public GameObject Player;

    TMP_Text Timer;
    RectTransform Rect;
    public double NowTime = 20;
    bool warning = false;

    private void Awake()
    {
        Timer = GetComponent<TMP_Text>();
        Rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Timer.text = string.Format("{0:0.00}", NowTime);
    }


    /// <summary>
    /// 현재 남은 시간이 10초 미만일 경우, 화면이 점점 어두워지는 연출을 통해 게임 오버를 연출
    /// </summary>
    private void Update()
    {
        if (NowTime > 0 && NowTime <= 900)
        {
            NowTime -= Time.deltaTime;
            Timer.text = string.Format("{0:0.00}", NowTime);
            if (NowTime <= 10 && !warning)
            {
                Warning.SetActive(true);
                StartCoroutine(EAE());
                Rect.anchoredPosition = new Vector3(0, 0, 0);
                warning = true;
            }
        }
    }

    /// <summary>
    /// 실제 화면이 어두워지는 코드를 담은 함수. 코루틴으로 실행 됨.
    /// </summary>
    /// <returns>None</returns>
    IEnumerator EAE()
    {
        WaitForSeconds WF = new WaitForSeconds(1);
        Image s = Warning.GetComponent<Image>();
        Color ss = new Color(0, 0, 0, 0.1f);

        while (NowTime > 0)
        {
            yield return WF;
            Rect.localScale *= 1.1f;
            s.color += ss;
        }
        Timer.text = "Game\nOver";
        Destroy(Player);
    }

    private void OnDisable ()
    {
        Warning.SetActive(false);
    }
}
