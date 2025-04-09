using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovingMapTutorialManager : TutorialManager
{
    private static MovingMapTutorialManager Instance { get; set; }

    [SerializeField] private Button moveLocationActiveButton;
    [SerializeField] private Button toOfficeStreetArrow;
    [SerializeField] private Button openOfficeDoorArrow;
    [Space]
    [SerializeField] private GameObject movableButtonPanel;
    [SerializeField] private GameObject moveToOfficeStreetPanel;
    [SerializeField] private GameObject openOfficeDoorPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public static MovingMapTutorialManager Get() => Instance; 
    
    public void Show(float duration) => StartCoroutine(ShowTutorial(duration));
    private IEnumerator ShowTutorial(float duration)
    {
        //페이즈 종료 이벤트 추가
        moveLocationActiveButton.onClick.AddListener(MoveToNextMovingTutorialPhase);
        yield return ShowPopUp(movableButtonPanel);
        //페이즈 종료 이벤트 제거
        moveLocationActiveButton.onClick.RemoveListener(MoveToNextMovingTutorialPhase);
        
        //페이즈 종료 이벤트 추가
        toOfficeStreetArrow.onClick.AddListener(MoveToNextMovingTutorialPhase);
        yield return ShowPopUp(moveToOfficeStreetPanel);
        //페이즈 종료 이벤트 제거
        toOfficeStreetArrow.onClick.RemoveListener(MoveToNextMovingTutorialPhase);
        
        //페이즈 종료 이벤트 추가
        openOfficeDoorArrow.onClick.AddListener(MoveToNextMovingTutorialPhase);
        yield return ShowPopUp(openOfficeDoorPanel);
        //페이즈 종료 이벤트 제거
        openOfficeDoorArrow.onClick.RemoveListener(MoveToNextMovingTutorialPhase);
        
        //블로커 비활성화
        blocker.SetActive(false);
    }
    private void MoveToNextMovingTutorialPhase() => MoveToNextTutorialPhase(1f);

}
