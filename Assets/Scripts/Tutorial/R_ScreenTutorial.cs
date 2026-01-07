using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_ScreenTutorial : TutorialManager
{
    [Header("Managers")]
    public TaskManager taskManager;
    public MailManager mailManager;

    [Header("GameObjects")]
    public GameObject[] tutorials;
    public GameObject[] panels;

    public int day;
    public int time;
    public float duration;
    [SerializeField] private int index;


    private void OnEnable()
    {
        if (GameSystem.Instance.gameData.date == day)
        {
            if (GameSystem.Instance.gameData.time == time)
            {
                //StartCoroutine(ShowTutorial(duration));
                StartTutorial();
            }
        }
    }

    public void StartTutorial()
    {
        index = 0;
        StartCoroutine(StartTutorial_IE());
    }

    private IEnumerator StartTutorial_IE()
    {
        yield return new WaitForSeconds(duration);

        yield return ShowPopUp(tutorials[index]);
        index++;
        panels[0].SetActive(true);
        yield return ShowPopUp(tutorials[index]);
        index++;
        panels[1].SetActive(true);
        yield return ShowPopUp(tutorials[index]);
        index++;
        panels[1].SetActive(false);
        panels[0].SetActive(false);
        yield return ShowPopUp(tutorials[index]);
        index++;
        panels[2].SetActive(true);
        yield return ShowPopUp(tutorials[index]);
        index++;
        panels[2].SetActive(false);
        yield return ShowPopUp(tutorials[index]);
        index++;
        taskManager.ActiveTaskWindow();
        yield return ShowPopUp(tutorials[index]);
        
        for (int i = 0; i < tutorials.Length; i++)
        {
            tutorials[i].SetActive(false);
        }

    }

    public IEnumerator ShowTutorial(float duration)
    {
        //StartCoroutine(ShowPopUp(UI));
        yield return new WaitForSeconds(duration);
        blocker.SetActive(false);
        // Destroy(UI);
        // UI = null;
    }

}
