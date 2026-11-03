using GameService;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_ScreenTutorial : TutorialManager
{
    [Header("Managers")]
    public WorkManager workManager;
    public MailManager mailManager;

    [Header("GameObjects")]
    public GameObject[] tutorials;
    public GameObject[] panels;

    public int day;
    public int time;
    public float duration;
    [SerializeField] private int index;
    
    private SaveService SaveService => GameSystem.SaveService;
    private IDayService DayService => GameSystem.GetService<IDayService>();

    private void OnEnable()
    {
        // 리메이크 단말기는 FirstDayScreenTutorial이 부팅부터 안내한다.
        // 기존 팝업을 동시에 띄우거나 재부팅 때 처음부터 반복하지 않는다.
        if (GetComponentInParent<RetroScreenDesktop>() != null)
        {
            if (blocker != null) blocker.SetActive(false);
            foreach (var popup in tutorials) if (popup != null) popup.SetActive(false);
            return;
        }
        if (SaveService == null || SaveService.CurrentSaveId < 0 || DayService == null ||
            SaveService.DayIndex != day || DayService.Time != time)
        {
            return;
        }
        //StartCoroutine(ShowTutorial(duration));
        StartTutorial();
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
        workManager.ActiveWorkWindow();
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
