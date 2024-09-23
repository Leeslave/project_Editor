using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTutorialManager : TutorialManager
{
    public GameObject UI;
    public static int count = 0;
    public float duration;
    private void Awake()
    {
        if (count <= 0)
        {
            StartCoroutine(ShowTutorial(duration));
            count++;
        }
    }
    
    public IEnumerator ShowTutorial(float duration)
    {
        StartCoroutine(ShowPopUp(UI));
        yield return new WaitForSeconds(duration);
        blocker.SetActive(false);
    }
}
