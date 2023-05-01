using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MazeTimer : MonoBehaviour
{
    public GameObject Warning;

    TMP_Text Timer;
    RectTransform Rect;
    double NowTime = 20;
    bool warning = false;

    private void Awake()
    {
        Timer = GetComponent<TMP_Text>();
        Rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (NowTime > 0)
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

    IEnumerator EAE()
    {
        WaitForSeconds WF = new WaitForSeconds(1);
        Image s = Warning.GetComponent<Image>();

        while (NowTime > 0)
        {
            Rect.localScale = Rect.localScale * 1.1f;
            s.color += new Color(0,0,0,0.06f);
            yield return WF;
        }
        Timer.text = "Game\nOver";
    }

}
