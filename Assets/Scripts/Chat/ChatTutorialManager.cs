
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ChatTutorialManager : TutorialManager
{
    private static ChatTutorialManager Instance;

    public static ChatTutorialManager Get()
    {
        if (Instance != null) 
            return Instance;
        return null;
    }
    
    public RectTransform TargetArea;
    public GameObject[] tutorials;
    public static int idx = 0;


    public void Show(float duration)
    {
        StartCoroutine(ShowTutorial(duration));
    }

    public IEnumerator ShowTutorial(float duration)
    {
        blocker = tutorials[idx];
        StartCoroutine(ShowPopUp(tutorials[idx]));
        yield return new WaitForSeconds(duration);
        blocker.SetActive(false);
        if (idx >= tutorials.Length - 1)
        {
            idx = 0;
        }
        else
        {
            idx++;
            StartCoroutine(ShowTutorial(duration));
        }    
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            blocker.SetActive(false);
        }
        else if (Instance != this)
            Destroy(gameObject);
    }
}
