using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>하루를 다시 선택하면 초기화하고, 씬 이동 중에는 유지하는 안내 진행 상태.</summary>
public static class FirstDayOnboarding
{
    public static bool OfficeEntranceComplete { get; set; }
    public static bool WorkstationComplete { get; set; }
    public static bool ScreenComplete { get; set; }

    public static void Reset()
    {
        OfficeEntranceComplete = false;
        WorkstationComplete = false;
        ScreenComplete = false;
    }

    public static TutorialController.TutorialStep Click(GameObject target, string text,
        UnityAction<BaseEventData> action = null, bool anywhere = false)
    {
        var callback = new TutorialController.TutorialStep.Event();
        if (action != null) callback.AddListener(action);
        return new TutorialController.TutorialStep
        {
            TargetObject = target, Text = text, IsRect = true, HighlightTarget = true,
            TimeScale = 1, StartDelay = .2f, ContinueAutomatically = true,
            Events = new List<TutorialController.TutorialStep.EventData>
            {
                new TutorialController.TutorialStep.EventData
                {
                    TriggerType = EventTriggerType.PointerClick, ReactAnywhere = anywhere,
                    ClickCount = 1, Callback = callback, CompletesStep = true
                }
            }
        };
    }
}
