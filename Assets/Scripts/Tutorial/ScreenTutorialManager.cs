using System.Collections;
using UnityEngine;

public class ScreenTutorialManager : TutorialManager
{
    public GameObject UI;
    public int day;
    public int time;
    public float duration;
    private void OnEnable()
    {
        if (GameSystem.Instance.gameData.date == day)
        {
            if (GameSystem.Instance.gameData.time == time)
            {
                StartCoroutine(ShowTutorial(duration));
            }
        }
    }
    
    public IEnumerator ShowTutorial(float duration)
    {
        StartCoroutine(ShowPopUp(UI));
        yield return new WaitForSeconds(duration);
        blocker.SetActive(false);
        // Destroy(UI);
        // UI = null;
    }
}
