using System.Collections;
using System.Linq;
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
    private void Start()
    {
        //초기화
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;

        //비활성 버튼까지 포함하여 오브젝트 검색
        Button[] buttons = FindObjectsOfType<Button>(true);
        moveLocationActiveButton = buttons.First(x => x.name == "MoveLocationActive");
        toOfficeStreetArrow = buttons.First(x => x.name == "ToOfficeStreet" && x.transform.parent.name == "BarStreet");
        openOfficeDoorArrow = buttons.First(x => x.name == "OfficeDoor" && x.transform.parent.name == "OfficeStreet");
        if(moveLocationActiveButton == null || toOfficeStreetArrow == null || openOfficeDoorArrow == null)
        {
            Debug.LogError("맵 이동 튜토리얼 시작 실패! 필요한 버튼을 찾지 못했습니다!");
            return;
        }
        Show();
    }

    public void Show() => StartCoroutine(nameof(ShowTutorial));
    private IEnumerator ShowTutorial()
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
