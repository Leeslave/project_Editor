using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameService;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>1일차 복도 대화 → 정보 1반 소개 → 사무실 담화 → 자리 이동 → 단말기 진입.</summary>
public sealed class FirstDayWorkstationTutorial : MonoBehaviour
{
    public FirstDayCommuteTutorial commute;
    public bool HasStarted { get; private set; }

    private bool IsMorning => GameSystem.SaveService != null &&
        GameSystem.SaveService.CurrentSaveId >= 0 && GameSystem.SaveService.DayIndex == 1 &&
        GameSystem.GetService<IDayService>()?.Time == 1;
    private bool Talking => Chat.Instance != null && Chat.Instance.IsOpen;

    private IEnumerator Start()
    {
        while (!FirstDayOnboarding.WorkstationComplete)
        {
            var location = GameSystem.GetService<ILocationService>()?.CurrentPosition;
            if (IsMorning && location?.location == World.Hallway && location.position == 0 &&
                !FirstDayOnboarding.OfficeEntranceComplete && !Talking &&
                DialogueFinished("day 1/[1-1] 오전 복도.json") &&
                !commute.spotlight.gameObject.activeSelf &&
                !((GameSystem.GetService<ILoadService>() as MainWorldLoader)?.IsLoading ?? false))
            {
                float hallwayX = (int)World.Hallway * 1000;
                var entrance = FindObjectsOfType<Door>(true).FirstOrDefault(d =>
                    d.destination == World.Office && d.position == 0 &&
                    Mathf.Abs(d.transform.position.x - hallwayX) < 50);
                if (entrance == null)
                {
                    Debug.LogError("1일차 정보 1반 안내: 복도의 사무실 입구를 찾지 못했습니다.");
                    yield break;
                }
                var entranceSpotlight = commute.spotlight;
                entranceSpotlight.Steps = new List<TutorialController.TutorialStep>();
                if (!entrance.gameObject.activeInHierarchy)
                    entranceSpotlight.Steps.Add(FirstDayOnboarding.Click(commute.movementToggle.gameObject,
                        "정보 1반으로\n클로버와 함께 사무실로 들어갈 차례입니다.\n이동 버튼을 눌러 출입 화살표를 표시하세요.",
                        _ => commute.movementToggle.onClick.Invoke()));
                entranceSpotlight.Steps.Add(FirstDayOnboarding.Click(entrance.gameObject,
                    "클레이튼의 일터, 정보 1반\n이 문 안쪽은 클레이튼이 반장으로 근무하는 정보 1반입니다.\n클레이튼의 책상과 업무용 스크린도 이 사무실에 있습니다.\n화면을 눌러 입장 안내를 확인하세요.", anywhere: true));
                entranceSpotlight.Steps.Add(FirstDayOnboarding.Click(entrance.gameObject,
                    "정보 1반에 들어가기\n강조된 출입 화살표를 눌러 클레이튼의 일터로 들어가세요.", data =>
                    {
                        entrance.OnPointerClick((PointerEventData)data);
                        var current = GameSystem.GetService<ILocationService>()?.CurrentPosition;
                        FirstDayOnboarding.OfficeEntranceComplete =
                            current?.location == World.Office && current.position == 0;
                    }));
                entranceSpotlight.StartTutorial();
                while (entranceSpotlight.gameObject.activeSelf) yield return null;
                // 입장 시 자동으로 시작되는 사무실 대화는 다음 프레임부터 관찰한다.
                yield return null;
                continue;
            }
            if (IsMorning && location?.location == World.Office && location.position == 0)
            {
                // 복도 대화나 다른 NPC 대화의 열림/닫힘을 담화 종료로 오인하지 않는다.
                bool officeDialogueFinished = DialogueFinished("day 1/[1-1] 오전 사무실.json");
                if (officeDialogueFinished && !Talking && !commute.spotlight.gameObject.activeSelf &&
                    !((GameSystem.GetService<ILoadService>() as MainWorldLoader)?.IsLoading ?? false))
                    break;
            }
            if (GameSystem.SaveService != null && GameSystem.SaveService.CurrentSaveId >= 0 &&
                (GameSystem.SaveService.DayIndex > 1 ||
                 (GameSystem.SaveService.DayIndex == 1 && GameSystem.GetService<IDayService>()?.Time > 1)))
                yield break;
            yield return null;
        }
        if (FirstDayOnboarding.WorkstationComplete) yield break;

        // 입구(0)에서 자리(3)로 직행하는 화살표는 없다. 안쪽(1)을 거쳐야 한다.
        var toInterior = FindOfficeDoor(0, 1);
        var seat = FindOfficeDoor(1, 3);
        var terminal = FindObjectOfType<ScreenButton>(true);
        if (toInterior == null || seat == null || terminal == null)
        {
            Debug.LogError("1일차 자리 안내: 이동 화살표 또는 단말기를 찾지 못했습니다.");
            yield break;
        }
        HasStarted = true;
        var spotlight = commute.spotlight;
        spotlight.Steps = new List<TutorialController.TutorialStep>();
        if (!toInterior.gameObject.activeInHierarchy)
            spotlight.Steps.Add(FirstDayOnboarding.Click(commute.movementToggle.gameObject,
                "내 자리로 이동\n담화가 끝났습니다. 이제 업무를 시작할 차례입니다.\n이동 버튼을 눌러 이동할 수 있는 방향을 확인하세요.",
                _ => commute.movementToggle.onClick.Invoke()));
        spotlight.Steps.Add(FirstDayOnboarding.Click(toInterior.gameObject,
            "사무실 안쪽으로 이동\n강조된 오른쪽 화살표를 눌러 책상이 있는 사무실 안쪽으로 이동하세요.",
            data => toInterior.OnPointerClick((PointerEventData)data)));
        spotlight.StartTutorial();
        while (spotlight.gameObject.activeSelf) yield return null;
        yield return null;
        if (GameSystem.GetService<ILocationService>()?.CurrentPosition?.position != 1) yield break;

        spotlight.Steps = new List<TutorialController.TutorialStep>();
        spotlight.Steps.Add(FirstDayOnboarding.Click(seat.gameObject,
            "내 자리로 이동\n강조된 화살표를 눌러 클레이튼의 자리로 이동하세요.",
            data => seat.OnPointerClick((PointerEventData)data)));
        spotlight.StartTutorial();
        while (spotlight.gameObject.activeSelf) yield return null;
        // 카메라 이동과 자리의 상호작용 오브젝트 활성화를 기다린다.
        yield return null;

        var card = FindObjectsOfType<ChatObject>(true).FirstOrDefault(obj =>
            obj.chatAssets != null && obj.chatAssets.Any(asset => asset.chat.Contains("[1-2]")));
        if (card != null)
        {
            spotlight.Steps = new List<TutorialController.TutorialStep>
            {
                FirstDayOnboarding.Click(card.gameObject,
                    "새 사원증 확인\n책상 위 사원증을 눌러 클로버의 업무 안내를 확인하세요.", _ => card.StartChat())
            };
            spotlight.StartTutorial();
            while (spotlight.gameObject.activeSelf || Talking) yield return null;
        }
        while (Talking) yield return null;
        if (!IsMorning) yield break;
        spotlight.Steps = new List<TutorialController.TutorialStep>
        {
            FirstDayOnboarding.Click(terminal.gameObject,
                "스크린 가동\n자리의 스크린을 누르면 업무 단말기로 접속합니다.\n접속한 화면에서 전원을 켜고 사용법을 확인하세요.", data =>
                {
                    FirstDayOnboarding.WorkstationComplete = true;
                    terminal.OnPointerClick((PointerEventData)data);
                })
        };
        spotlight.StartTutorial();
    }

    private static Door FindOfficeDoor(int source, int destination)
    {
        float sourceX = (int)World.Office * 1000 + source * 100;
        return FindObjectsOfType<Door>(true)
            .Where(d => d.destination == World.Office && d.position == destination &&
                Mathf.Abs(d.transform.position.x - sourceX) < 50)
            .OrderBy(d => Mathf.Abs(d.transform.position.x - sourceX)).FirstOrDefault();
    }

    private static bool DialogueFinished(string path) => Chat.Instance?.LastCompletedDialogueSource?
        .Replace('\\', '/').EndsWith(path, System.StringComparison.Ordinal) == true;
}
