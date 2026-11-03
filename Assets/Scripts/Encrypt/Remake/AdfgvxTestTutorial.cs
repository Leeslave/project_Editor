using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.AdfgvxRemake
{
    public sealed partial class AdfgvxTestController
    {
        [SerializeField] private bool autoStartTutorial = true;
        private readonly Dictionary<string, AdfgvxLesson> lessons = new Dictionary<string, AdfgvxLesson>();
        private readonly Dictionary<string, RectTransform> focusTargets = new Dictionary<string, RectTransform>();
        private AdfgvxLesson lesson;
        private LessonStep lessonStep;
        private bool LessonActive => lesson != null && lesson.Active;

        private void SelectLesson()
        {
            if (!lessons.TryGetValue(session.Task.Id, out lesson))
            {
                lesson = new AdfgvxLesson { Active = autoStartTutorial };
                lessons.Add(session.Task.Id, lesson);
            }
        }

        private T Focus<T>(string id, T target) where T : Component
        {
            focusTargets[id] = target.GetComponent<RectTransform>();
            return target;
        }

        private void PrepareLesson()
        {
            focusTargets.Clear();
            lessonStep = LessonActive ? lesson.Current(session) : null;
            if (lessonStep != null && lessonStep.Action == LessonAction.RecordCell)
                session.SelectedUnit = AdfgvxLesson.FindPendingUnit(session);
        }

        private void ToggleLesson()
        {
            SaveDraft();
            if (LessonActive) lesson.Active = false;
            else
            {
                lesson = new AdfgvxLesson();
                lessons[session.Task.Id] = lesson;
            }
            statusError = false;
            Refresh();
        }

        // Same spotlight interaction as the shared tutorial: darken the surroundings and
        // keep the real target clickable. Using four UI masks avoids the shared prefab's
        // world-camera/EventTrigger assumptions, and reacquires targets after each rebuild.
        private void BuildTutorialOverlay(Transform root)
        {
            if (!LessonActive || lessonStep == null) return;
            var overlay = Rect(root, "Focused tutorial", 0, 0, 1440, 1080);
            RectTransform target;
            bool found = focusTargets.TryGetValue(lessonStep.Target, out target) && target != null;
            Canvas.ForceUpdateCanvases();
            Rect hole = found ? FocusBounds(target, page.GetComponent<RectTransform>()) : new Rect(700, 530, 40, 40);
            Color shade = new Color(0, 0, 0, .84f);
            Shade(overlay, "Above target", 0, 0, 1440, hole.yMin, shade);
            Shade(overlay, "Below target", 0, hole.yMax, 1440, 1080 - hole.yMax, shade);
            Shade(overlay, "Left of target", 0, hole.yMin, hole.xMin, hole.height, shade);
            Shade(overlay, "Right of target", hole.xMax, hole.yMin, 1440 - hole.xMax, hole.height, shade);
            // Explanations require an explicit continue; gameplay steps pass pointer input
            // through the cutout directly to the actual Button or TMP_InputField.
            if (lessonStep.ReadThenContinue || !found)
                Shade(overlay, "Read before acting", hole.x, hole.y, hole.width, hole.height, Color.clear);
            const float edge = 3;
            Edge(overlay, "Focus top", hole.x, hole.y, hole.width, edge, cyan);
            Edge(overlay, "Focus bottom", hole.x, hole.yMax - edge, hole.width, edge, cyan);
            Edge(overlay, "Focus left", hole.x, hole.y, edge, hole.height, cyan);
            Edge(overlay, "Focus right", hole.xMax - edge, hole.y, edge, hole.height, cyan);

            const float width = 560, height = 360;
            Vector2 position = LessonCardPosition(hole, width, height);
            var card = Frame(overlay, "One action at a time", position.x, position.y, width, height, cyan);
            Label(card, "함께 풀기  /  " + lessonStep.Progress, 22, 15, 516, 30, 19, cyan);
            Label(card, lessonStep.Title, 22, 53, 516, 43, 27, Color.white);
            string text = found ? lessonStep.Text : "화면의 대상을 찾지 못했어요.\n[혼자 해보기]로 나간 뒤 위쪽에서 튜토리얼을 다시 켜 주세요.";
            Label(card, text, 22, 106, 516, 156, 23, Color.white);
            if (statusError)
                Label(card, "괜찮아요! 입력한 글자와 밝은 곳을 다시 확인해 주세요.", 22, 270, 516, 28, 18, amber);
            else
                Label(card, lessonStep.ReadThenContinue ? "설명을 읽으셨다면 아래 버튼을 눌러 주세요." : "밝은 곳에서 직접 해 보시면 다음으로 넘어가요!",
                    22, 270, 516, 28, 18, muted);
            Button(card, "혼자 해보기", 22, 310, 170, 36, () =>
            {
                lesson.Active = false;
                statusError = false;
                Refresh();
            }, true, muted, 19);
            if (lessonStep.ReadThenContinue && found)
            {
                LessonAction action = lessonStep.Action;
                Button(card, action == LessonAction.Finish ? "완료!" : "알겠어요, 해 볼게요!", 214, 310, 324, 36, () =>
                {
                    lesson.Continue(action);
                    statusError = false;
                    Refresh();
                }, true, cyan, 20);
            }
            else Label(card, "답은 직접 눌러서 완성해요", 214, 310, 324, 36, 19, cyan, true);
        }

        private Rect FocusBounds(RectTransform target, RectTransform localRoot)
        {
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 min = localRoot.InverseTransformPoint(corners[0]);
            Vector3 max = localRoot.InverseTransformPoint(corners[2]);
            float left = Mathf.Clamp(min.x - 8, 4, 1436);
            float right = Mathf.Clamp(max.x + 8, left + 1, 1437);
            float top = Mathf.Clamp(-max.y - 8, 4, 1076);
            float bottom = Mathf.Clamp(-min.y + 8, top + 1, 1077);
            return new Rect(left, top, right - left, bottom - top);
        }

        internal static Vector2 LessonCardPosition(Rect hole, float width, float height)
        {
            const float gap = 22, inset = 18;
            float y = Mathf.Clamp(hole.center.y - height / 2, inset, 1080 - height - inset);
            float x = Mathf.Clamp(hole.center.x - width / 2, inset, 1440 - width - inset);
            if (hole.xMax + gap + width <= 1440 - inset) return new Vector2(hole.xMax + gap, y);
            if (hole.xMin - gap - width >= inset) return new Vector2(hole.xMin - gap - width, y);
            if (hole.yMax + gap + height <= 1080 - inset) return new Vector2(x, hole.yMax + gap);
            if (hole.yMin - gap - height >= inset) return new Vector2(x, hole.yMin - gap - height);
            // All registered targets are narrower than 600px, leaving room on one side.
            return new Vector2(inset, inset);
        }

        private void Shade(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            if (width <= 0 || height <= 0) return;
            var image = Rect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
        }
    }
}
