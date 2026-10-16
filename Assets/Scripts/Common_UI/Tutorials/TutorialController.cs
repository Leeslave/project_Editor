using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance { get; private set; }
    public bool AutoStart;
    public GameObject CompletionSignal;
    public TutorialBorder CurrentBorder;
    public List<TutorialStep> Steps;
    [SerializeField] private GameObject _circleHighlight;
    [SerializeField] private GameObject _boxHighlight;
    [SerializeField] private RectTransform _textBox;
    private TutorialBorder _circleBorder;
    private TutorialBorder _boxBorder;
    private GameObject _currentTarget;
    private Vector2 _targetPosition;
    private float _targetRadius;

    [Serializable]
    public class TutorialStep
    {
        [Header("조명할 오브젝트")]
        public GameObject TargetObject;
        public List<int> TargetChildIndices = new List<int>();
        [Header("설명 문구"), Multiline(3)] public string Text;
        [Header("마우스 이벤트")] public List<EventData> Events;
        [Header("기타 이벤트")]
        public List<Event> StartEvents;
        public List<Event> EndEvents;
        [Header("오브젝트 설정")]
        public List<GameObject> DisabledObjects;
        public List<GameObject> EnabledObjects;
        public GameObject ActiveTarget;
        public List<int> ActivationChildIndices;
        public GameObject DeactivationTarget;
        public List<int> DeactivationChildIndices;
        [Header("기타 설정")]
        public float StartDelay;
        public bool IsRect;
        public float TimeScale = 1;
        public bool ContinueAutomatically;
        public bool HighlightTarget;
        public bool KeepEvents;
        public bool RemoveAfterCompletion = true;

        [Serializable]
        public class Event : UnityEvent<BaseEventData> { }

        [Serializable]
        public class EventData
        {
            public EventTriggerType TriggerType;
            public bool ReactAnywhere;
            public int ClickCount = 1;
            public Event Callback;
            public bool CompletesStep;
            public bool IncludeTargetActions;
        }
    }

    // 다른 컴포넌트가 튜토리얼에 접근하기 전에 인스턴스와 조명 영역을 준비한다.
    private void Awake()
    {
        Instance = this;
        _boxBorder = _boxHighlight.GetComponent<TutorialBorder>();
        _circleBorder = _circleHighlight.GetComponent<TutorialBorder>();
    }

    // 자동 시작 설정이 있을 때 첫 단계를 예약한다.
    private void Start()
    {
        if (AutoStart) Invoke(nameof(StartTutorial), 0.2f);
    }

    // 씬 종료 후 파괴된 튜토리얼 인스턴스가 남지 않게 한다.
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool AdvanceTutorial()
    {
        if (Steps == null || Steps.Count == 0) return false;
        TutorialStep step = Steps[0];
        if (step.RemoveAfterCompletion) Steps.RemoveAt(0);
        InvokeEvents(step.EndEvents);
        if ((step.ContinueAutomatically || !gameObject.activeSelf) && Steps.Count > 0) StartTutorial();
        else gameObject.SetActive(false);
        return true;
    }

    public void StartTutorial()
    {
        if (Steps == null || Steps.Count == 0) return;
        InvokeEvents(Steps[0].StartEvents);
        gameObject.SetActive(true);
        StartCoroutine(ShowCurrentStep(Steps[0]));
    }

    // 현재 단계의 대상, 활성화 조건과 조명 영역을 설정한다.
    private IEnumerator ShowCurrentStep(TutorialStep step)
    {
        _boxHighlight.SetActive(false);
        _circleHighlight.SetActive(false);
        _textBox.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(step.StartDelay);
        Time.timeScale = step.TimeScale;
        SetObjectsActive(step.DisabledObjects, false);
        SetObjectsActive(step.EnabledObjects, true);
        _currentTarget = ResolveTarget(step.TargetObject, step.TargetChildIndices);
        if (_currentTarget != null)
            _targetPosition = WorldToScreenInCameraRect(Camera.main, _currentTarget.transform.position);
        GameObject activationTarget = ResolveTarget(step.ActiveTarget, step.ActivationChildIndices);
        if (activationTarget != null) activationTarget.AddComponent<TutorialActivationTrigger>();
        GameObject deactivationTarget = ResolveTarget(step.DeactivationTarget, step.DeactivationChildIndices);
        if (deactivationTarget != null) deactivationTarget.AddComponent<TutorialDeactivationTrigger>();
        GameObject highlight = step.IsRect ? _boxHighlight : _circleHighlight;
        TutorialBorder border = step.IsRect ? _boxBorder : _circleBorder;
        highlight.SetActive(true);
        _targetRadius = border.Init(_currentTarget, step.Text, step.HighlightTarget, step.KeepEvents);
    }

    // 원본 단계의 오브젝트 참조를 바꾸지 않고 자식 경로를 따라간다.
    private static GameObject ResolveTarget(GameObject target, List<int> childIndices)
    {
        if (target == null || childIndices == null) return target;
        foreach (int index in childIndices) target = target.transform.GetChild(index).gameObject;
        return target;
    }

    // 단계가 지정한 오브젝트들을 같은 활성화 상태로 변경한다.
    private static void SetObjectsActive(List<GameObject> objects, bool active)
    {
        if (objects == null) return;
        foreach (GameObject target in objects) if (target != null) target.SetActive(active);
    }

    // 단계의 시작 또는 종료 이벤트를 순서대로 호출한다.
    private static void InvokeEvents(List<TutorialStep.Event> events)
    {
        if (events == null) return;
        foreach (TutorialStep.Event callback in events) callback?.Invoke(null);
    }

    public void ChangeTarget(GameObject target) => CurrentBorder.ChangeTarget(target);
    public void ResetTarget() => CurrentBorder.ChangeTarget(_currentTarget);
    public void OnPointer() => CurrentBorder.DisableRaycast();
    public void OutPointer() => CurrentBorder.EnableRaycast();

    public void UpdateTargetPosition(Transform target)
    {
        _targetPosition = WorldToScreenInCameraRect(Camera.main, target.position);
    }

    public static Vector2 WorldToScreenInCameraRect(Camera camera, Vector3 worldPosition)
    {
        if (camera == null) return Vector2.zero;
        Vector3 viewport = camera.WorldToViewportPoint(worldPosition);
        return new Vector2((camera.rect.x + viewport.x * camera.rect.width) * Screen.width,
            (camera.rect.y + viewport.y * camera.rect.height) * Screen.height);
    }

    public void SetEvent(EventTrigger trigger)
    {
        if (Steps == null || Steps.Count == 0 || _currentTarget == null) return;
        TutorialStep step = Steps[0];
        if (step.Events == null) return;
        EventTrigger targetTrigger = _currentTarget.GetComponent<EventTrigger>();
        foreach (TutorialStep.EventData eventData in step.Events)
            AddStepEvent(targetTrigger, trigger, eventData);
    }

    // 대상의 기존 동작과 단계 완료 조건을 하나의 포인터 이벤트에 연결한다.
    private void AddStepEvent(EventTrigger targetTrigger, EventTrigger trigger, TutorialStep.EventData settings)
    {
        var targetActions = new List<EventTrigger.TriggerEvent>();
        if (targetTrigger != null && settings.IncludeTargetActions)
            foreach (EventTrigger.Entry targetEntry in targetTrigger.triggers)
                if (targetEntry.eventID == settings.TriggerType) targetActions.Add(targetEntry.callback);
        var entry = new EventTrigger.Entry { eventID = settings.TriggerType };
        entry.callback.AddListener(eventData =>
        {
            if (!(eventData is PointerEventData pointer) || pointer.clickCount < settings.ClickCount) return;
            if (!settings.ReactAnywhere && Vector2.Distance(_targetPosition, pointer.position) >= _targetRadius) return;
            foreach (EventTrigger.TriggerEvent callback in targetActions) callback.Invoke(eventData);
            settings.Callback?.Invoke(eventData);
            if (settings.CompletesStep) AdvanceTutorial();
        });
        trigger.triggers.Add(entry);
    }

    public void ResetTime() => Time.timeScale = 1;
}
