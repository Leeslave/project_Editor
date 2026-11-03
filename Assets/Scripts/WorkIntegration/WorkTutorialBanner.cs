using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.WorkIntegration
{
    /// <summary>입력을 가리지 않는 조작 안내. 강조는 실제 UI 좌표를 따라가며 클릭을 대신하지 않는다.</summary>
    public sealed class WorkTutorialBanner : MonoBehaviour
    {
        private WorkTestUI ui;
        private RectTransform canvas, root, panel;
        private TMP_Text title, body;
        private Button next, toggle;
        private Action continueAction, toggleAction;
        private readonly RectTransform[] edges = new RectTransform[4];
        private RectTransform target;

        public void Initialize(TMP_FontAsset font, Action continued, Action toggled)
        {
            ui = new WorkTestUI(font);
            continueAction = continued;
            toggleAction = toggled;
            canvas = ui.Canvas(transform, "Work tutorial", 1800);
            root = ui.Rect(canvas, "Top guide", 0, 0, 1440, 1080);
            root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, 1);
            root.anchoredPosition = Vector2.zero;
            panel = ui.Panel(root, "Guide background", 0, 0, 1440, 112, WorkTestUI.Background);
            title = ui.Text(panel, "", 24, 10, 1100, 35, 24);
            body = ui.Text(panel, "", 24, 48, 1090, 60, 20);
            next = ui.Button(panel, "계속", 1130, 15, 125, 75, () => continueAction?.Invoke());
            toggle = ui.Button(root, "안내 끄기", 1265, 15, 155, 75, () => toggleAction?.Invoke());
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i] = ui.Panel(root, "Target border " + i, 0, 0, 0, 0, new Color32(255, 208, 90, 255));
                edges[i].GetComponent<Image>().raycastTarget = false;
            }
        }

        public void Show(bool active, string heading, string text, bool canContinue, RectTransform focus = null,
            string continueLabel = "계속")
        {
            panel.gameObject.SetActive(active);
            title.text = heading;
            body.text = text;
            next.gameObject.SetActive(active && canContinue);
            next.GetComponentInChildren<TMP_Text>().text = continueLabel;
            toggle.GetComponentInChildren<TMP_Text>().text = active ? "안내 끄기" : "안내 켜기";
            target = active ? focus : null;
        }

        private void LateUpdate()
        {
            if (root == null) return;
            float scale = Mathf.Min(canvas.rect.width / 1440f, canvas.rect.height / 1080f);
            root.localScale = new Vector3(scale, scale, 1);
            foreach (var edge in edges) edge.gameObject.SetActive(false);
            if (target == null || !target.gameObject.activeInHierarchy) return;
            Rect bounds = Bounds(target);
            // 스크롤 영역 밖에 있는 행까지 강조 테두리가 튀어나오지 않도록 자른다.
            foreach (var mask in target.GetComponentsInParent<RectMask2D>())
            {
                Rect clip = Bounds(mask.GetComponent<RectTransform>());
                bounds = Rect.MinMaxRect(Mathf.Max(bounds.xMin, clip.xMin), Mathf.Max(bounds.yMin, clip.yMin),
                    Mathf.Min(bounds.xMax, clip.xMax), Mathf.Min(bounds.yMax, clip.yMax));
            }
            if (bounds.width < 2 || bounds.height < 2) return;
            float x = bounds.xMin + 720, y = -bounds.yMax;
            Place(edges[0], x, y, bounds.width, 3);
            Place(edges[1], x, y + bounds.height - 3, bounds.width, 3);
            Place(edges[2], x, y, 3, bounds.height);
            Place(edges[3], x + bounds.width - 3, y, 3, bounds.height);
        }

        private Rect Bounds(RectTransform rect)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector2 min, max;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root,
                RectTransformUtility.WorldToScreenPoint(null, corners[0]), null, out min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root,
                RectTransformUtility.WorldToScreenPoint(null, corners[2]), null, out max);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static void Place(RectTransform rect, float x, float y, float width, float height)
        {
            rect.gameObject.SetActive(true);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}
