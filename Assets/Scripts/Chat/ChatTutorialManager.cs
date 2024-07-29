
using System;
using System.Collections;
using UnityEngine;

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
    public GameObject UI;

    private void SetArea(Vector2 position, Vector2 size)
    {
        TargetArea.localPosition = new Vector3(position.x, position.y, 0);
        TargetArea.sizeDelta = new Vector2(size.x, size.y);
    }

    public void Show(float duration)
    {
        StartCoroutine(ShowTutorial(duration));
    }

    public IEnumerator ShowTutorial(float duration)
    {
        StartCoroutine(ShowPopUp(UI));
        yield return new WaitForSeconds(duration);
        blocker.SetActive(false);
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
