using EditorGame.WorkIntegration;
using TMPro;
using UnityEngine;

namespace EditorGame.Documents
{
    /// <summary>문서 UI의 실제 조작 상태에 맞춰 안내와 강조를 갱신한다.</summary>
    public sealed class DocumentWorkTutorial : MonoBehaviour
    {
        private DocumentDesktop desktop;
        private DocumentWorkLesson lesson;
        private WorkTutorialBanner banner;
        private DocumentLessonStep step;
        private bool active = true;
        private float nextObserve;

        public void Initialize(DocumentDesktop view, TMP_FontAsset font, bool activeAtStart = true)
        {
            desktop = view;
            active = activeAtStart;
            lesson = new DocumentWorkLesson();
            banner = gameObject.AddComponent<WorkTutorialBanner>();
            banner.Initialize(font, Continue, Toggle);
            desktop.ProfileEditCommitted += OnCommitted;
            // 안내를 꺼도 다시 켜기 버튼이 문서 헤더를 덮지 않도록 위쪽 공간을 유지한다.
            desktop.SetTutorialVisible(true);
            Refresh();
        }

        private void Update()
        {
            if (desktop == null || Time.unscaledTime < nextObserve) return;
            nextObserve = Time.unscaledTime + .1f;
            Refresh();
        }

        private void OnCommitted(string personId, ProfileField field, string value)
        {
            lesson.CommittedFields.Add(DocumentContract.Key(personId, field.ToString()));
        }

        private void Continue()
        {
            if (step == null || !step.ReadThenContinue) return;
            lesson.Continue(step.Id);
            if (lesson.Finished) active = false;
            desktop.SetTutorialVisible(true);
            Refresh();
        }

        private void Toggle()
        {
            active = !active;
            if (active && lesson.Finished) lesson = new DocumentWorkLesson { IntroRead = true };
            desktop.SetTutorialVisible(true);
            Refresh();
        }

        private void Refresh()
        {
            step = desktop.GetTutorialStep(lesson);
            banner.Show(active && step != null, step?.Title ?? "", step?.Text ?? "", step?.ReadThenContinue ?? false,
                desktop.GetTutorialTarget(step?.Target), step?.Id == "result" ? "안내 완료" : "계속");
        }

        private void OnDestroy()
        {
            if (desktop != null) desktop.ProfileEditCommitted -= OnCommitted;
        }
    }
}
