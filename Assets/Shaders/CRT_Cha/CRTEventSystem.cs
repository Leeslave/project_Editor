using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CRTEventSystem : StandaloneInputModule
{
    public static CRTEventSystem System;
    public static Material CRTMat;
    static ParticleSystem Test;

    [SerializeField] bool IsTest = true;

    protected override void Awake()
    {
        if (System == null)
        {
            System = this; Test = transform.GetChild(0).GetComponent<ParticleSystem>();
        }
        else Destroy(this);
#if UNITY_EDITOR
        if (!IsTest) Destroy(transform.GetChild(0).gameObject);
#else
        Destroy(transform.GetChild(0).gameObject);
        IsTest = false;
#endif

        base.Awake();
    }

    public static Vector2 DistortMousePosition(Vector2 CurrentPos)
    {
        Vector2 PosToUv = new Vector2(CurrentPos.x / Screen.width - 0.5f, CurrentPos.y / Screen.width - 0.5f);
        float Distort = CRTMat.GetFloat("_Distort");
        float factor = (1f + Distort * (PosToUv.x * PosToUv.x + PosToUv.y * PosToUv.y));
        Vector2 Result = CurrentPos * factor;
        return Result;
    }

    public override void Process()
    {
        if (!eventSystem.isFocused)
            return;

        bool usedEvent = SendUpdateEventToSelectedObject();

        if (!ProcessTouchEvents() && input.mousePresent)
        {
            var mouseData = GetMousePointerEventData(0);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            var rightButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData;
            var middleButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData;

            Vector2 originalMousePosition = Input.mousePosition;
            Vector2 distortedPosition = DistortMousePosition(originalMousePosition);

            RaycastResult raycastResult = RaycastWithDistortion(distortedPosition);
            if (raycastResult.gameObject != null) leftButtonData.buttonData.pointerCurrentRaycast = raycastResult;

            //m_CurrentFocusedGameObject = leftButtonData.buttonData.pointerCurrentRaycast.gameObject;
            // Left
            leftButtonData.buttonData.position = distortedPosition;
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Right
            ProcessMousePress(rightButtonData);
            ProcessDrag(rightButtonData.buttonData);

            // Middle
            ProcessMousePress(middleButtonData);
            ProcessDrag(middleButtonData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }

            // For Test
            if (leftButtonData.PressedThisFrame() && IsTest)
            {
                Vector3 TestPos = Camera.main.ScreenToWorldPoint(distortedPosition); TestPos.z = 0;
                Test.transform.position = TestPos;
                Test.Play();
            }
        }

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    public static RaycastResult RaycastWithDistortion(Vector2 distortedPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = distortedPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Raycast된 UI 요소 반환
        return results.Count > 0 ? results[0] : default(RaycastResult);
    }

    private bool ProcessTouchEvents()
    {
        for (int i = 0; i < input.touchCount; ++i)
        {
            Touch touch = input.GetTouch(i);

            if (touch.type == TouchType.Indirect)
                continue;

            bool released;
            bool pressed;
            var pointer = GetTouchPointerEventData(touch, out pressed, out released);

            ProcessTouchPress(pointer, pressed, released);

            if (!released)
            {
                ProcessMove(pointer);
                ProcessDrag(pointer);
            }
            else
                RemovePointerData(pointer);
        }
        return input.touchCount > 0;
    }

}
