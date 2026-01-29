using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExecuteTest : MonoBehaviour
{
    [SerializeField] private TutorialOverlayHighlighter highlighter;
    [SerializeField] private Transform target;

    void StartTutorial()
    {
        highlighter.Begin(target);
    }

    void EndTutorial()
    {
        highlighter.End();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { StartTutorial();}
        if (Input.GetKeyDown(KeyCode.K)) { EndTutorial();}
    }
}
