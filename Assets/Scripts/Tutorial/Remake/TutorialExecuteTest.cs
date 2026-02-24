using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExecuteTest : MonoBehaviour
{
    [SerializeField] private TutorialOverlayHighlighter highlighter;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject dim;

    void StartTutorial()
    {
        dim.SetActive(true);
        highlighter.Begin();
    }

    void EndTutorial()
    {
        dim.SetActive(false);
        highlighter.End();
    }

    void Start()
    {
        dim.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { StartTutorial();}
        if (Input.GetKeyDown(KeyCode.K)) { EndTutorial();}
    }
}
