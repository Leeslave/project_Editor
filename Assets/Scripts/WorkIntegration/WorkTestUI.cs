using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.WorkIntegration
{
    internal sealed class WorkTestUI
    {
        private readonly TMP_FontAsset font;
        internal static readonly Color Green = new Color32(164, 207, 170, 255);
        internal static readonly Color Background = new Color32(12, 20, 17, 255);
        internal WorkTestUI(TMP_FontAsset font) { this.font = font; }

        internal RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
            return rect;
        }

        internal RectTransform Panel(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var rect = Rect(parent, name, x, y, w, h);
            rect.gameObject.AddComponent<Image>().color = color;
            return rect;
        }

        internal TMP_Text Text(Transform parent, string text, float x, float y, float w, float h,
            float size = 24, Color? color = null)
        {
            var rect = Rect(parent, text, x, y, w, h);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = font;
            label.fontSize = size;
            label.text = text;
            label.color = color ?? Green;
            label.raycastTarget = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }

        internal Button Button(Transform parent, string text, float x, float y, float w, float h,
            Action clicked, bool enabled = true)
        {
            var rect = Panel(parent, text, x, y, w, h, new Color32(36, 57, 45, 255));
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            button.interactable = enabled;
            button.onClick.AddListener(() => clicked());
            var label = Text(rect, text, 12, 6, w - 24, h - 12, 22);
            label.alignment = TextAlignmentOptions.Midline;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            return button;
        }

        internal RectTransform Canvas(Transform parent, string name, int order)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(parent, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = .5f;
            return go.GetComponent<RectTransform>();
        }
    }
}
