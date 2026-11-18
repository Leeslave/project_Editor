using System.Collections;
using System.Collections.Generic;
using GameService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Day 1 아침 대화가 끝난 뒤 실제 이동 조작으로 출근 동선을 안내한다.</summary>
public sealed class FirstDayCommuteTutorial : MonoBehaviour
{
    public TutorialController spotlight;
    public Button movementToggle;
    public Door toCompanyStreet;
    public Door companyEntrance;
    public GameObject companyBuilding;
    public bool HasStarted { get; private set; }
    public bool IsComplete { get; private set; }

    private IEnumerator Start()
    {
        FirstDayScreenTutorial.PrepareOverlay(spotlight.gameObject);
        spotlight.gameObject.SetActive(false);
        foreach (var border in spotlight.GetComponentsInChildren<TutorialBorder>(true))
            border.SetDescriptionPosition(new Vector2(.32f, .22f));
        bool sawMorningDialogue = false;
        while (!HasStarted)
        {
            var day = GameSystem.GetService<IDayService>();
            var location = GameSystem.GetService<ILocationService>();
            if (GameSystem.SaveService != null && GameSystem.SaveService.CurrentSaveId >= 0 &&
                GameSystem.SaveService.DayIndex == 1 && day?.Time == 0)
            {
                bool talking = Chat.Instance != null && Chat.Instance.IsOpen;
                if (talking) sawMorningDialogue = true;
                if (sawMorningDialogue && !talking && location?.CurrentPosition?.location == World.Street &&
                    location.CurrentPosition.position == 0 &&
                    !((GameSystem.GetService<ILoadService>() as MainWorldLoader)?.IsLoading ?? false))
                    Begin();
            }
            else
            {
                // MainWorld는 날짜가 바뀌어도 유지된다. 0일차에서는 종료하지 않고
                // 1일차 아침을 기다리며, 다른 날짜의 대화를 아침 대화로 기억하지 않는다.
                sawMorningDialogue = false;
                if (GameSystem.SaveService != null && GameSystem.SaveService.CurrentSaveId >= 0 &&
                    (GameSystem.SaveService.DayIndex > 1 ||
                     (GameSystem.SaveService.DayIndex == 1 && day != null && day.Time > 0)))
                    yield break;
            }
            yield return null;
        }
    }

    private void Begin()
    {
        HasStarted = true;
        spotlight.Steps = new List<TutorialController.TutorialStep>();
        // 기존 씬은 이동 화살표가 켜져 있을 수 있다. 닫힌 상태에서 조작을 직접 익히게 한다.
        if (toCompanyStreet.gameObject.activeInHierarchy) movementToggle.onClick.Invoke();
        Add(movementToggle.gameObject,
                "이동하기\n오른쪽 아래의 이동 버튼을 누르면 갈 수 있는 방향이 표시됩니다.\n강조된 이동 버튼을 눌러 보세요.",
                _ => movementToggle.onClick.Invoke());
        Add(toCompanyStreet.gameObject,
            "회사로 가는 길\n이동 화살표를 누르면 해당 장소로 이동합니다.\n강조된 화살표를 눌러 회사 앞으로 이동하세요.",
            data => toCompanyStreet.OnPointerClick((PointerEventData)data));
        Add(companyBuilding,
            "아슬라니아 정보국\n클레이튼이 근무하는 회사입니다. 건물 안으로 들어가 복도를 지나면 정보 1반 사무실이 나옵니다.\n사무실의 업무 단말기에서 오늘의 업무를 시작할 수 있습니다.\n아무 곳이나 눌러 출입 안내를 확인하세요.",
            null, true);
        Add(companyEntrance.gameObject,
            "회사에 들어가기\n강조된 출입 화살표를 눌러 건물 안으로 들어가세요.\n다른 장소에서도 이동 버튼과 화살표를 같은 방식으로 사용합니다.",
            data => companyEntrance.OnPointerClick((PointerEventData)data));
        var finished = new TutorialController.TutorialStep.Event();
        finished.AddListener(_ => IsComplete = true);
        spotlight.Steps[spotlight.Steps.Count - 1].EndEvents =
            new List<TutorialController.TutorialStep.Event> { finished };
        spotlight.StartTutorial();
    }

    private void Add(GameObject target, string text, UnityEngine.Events.UnityAction<BaseEventData> action,
        bool anywhere = false)
    {
        var callback = new TutorialController.TutorialStep.Event();
        if (action != null) callback.AddListener(action);
        spotlight.Steps.Add(new TutorialController.TutorialStep
        {
            TargetObject = target, Text = text, IsRect = true, HighlightTarget = true,
            TimeScale = 1, StartDelay = .15f, ContinueAutomatically = true,
            Events = new List<TutorialController.TutorialStep.EventData>
            {
                new TutorialController.TutorialStep.EventData
                {
                    TriggerType = EventTriggerType.PointerClick, ReactAnywhere = anywhere,
                    ClickCount = 1, Callback = callback, CompletesStep = true
                }
            }
        });
    }
}
