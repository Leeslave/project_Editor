using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DayTutorialManager : TutorialManager
{
    public GameObject[] target;
    public int date = 0;
    public int time = 0;
    public float duration;
    private void OnEnable()
    {
        if (date == GameSystem.Instance.dateIndex)
        {
            if (time == GameSystem.Instance.dateIndex)
            {
                StartCoroutine(ShowTutorial(duration, 0));
            }
        }
    }
    
    public IEnumerator ShowTutorial(float delay, int i)
    {
        if (target.Length <= i)
        {
            blocker.SetActive(false);
            yield break;
        }
        StartCoroutine(ShowPopUp(target[i]));
        yield return new WaitForSeconds(delay);
        target[i].SetActive(false);
        StartCoroutine(ShowTutorial(delay, i + 1));
    }
}
