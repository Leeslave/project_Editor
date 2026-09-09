using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.Documents
{
    // Shared uGUI construction keeps the runtime hierarchy and authored content independent.
    internal sealed class DocumentDesktopUI
    {
        internal static readonly Color Surface = new Color32(192, 192, 192, 255);
        internal static readonly Color Ink = new Color32(20, 20, 20, 255);
        internal static readonly Color Navy = new Color32(0, 0, 128, 255);
        private readonly TMP_FontAsset font;

        internal DocumentDesktopUI(TMP_FontAsset font) { this.font = font; }

        internal RectTransform Rect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        internal static void Place(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        internal static void Fill(RectTransform rect, float inset = 0)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        internal RectTransform Panel(string name, Transform parent, Color color)
        {
            var rect = Rect(name, parent);
            rect.gameObject.AddComponent<Image>().color = color;
            return rect;
        }

        internal TMP_Text Text(Transform parent, string value, float size = 22)
        {
            var text = Rect("Text", parent).gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = Ink;
            text.richText = false;
            text.raycastTarget = false;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            return text;
        }

        internal Button Button(Transform parent, string title, Action click)
        {
            var rect = Panel(title, parent, Surface);
            var border = rect.gameObject.AddComponent<Outline>();
            border.effectColor = Color.gray;
            border.effectDistance = new Vector2(1, -1);
            var button = rect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(.85f, .88f, 1);
            colors.selectedColor = new Color(.7f, .78f, 1);
            colors.pressedColor = new Color(.6f, .65f, .8f);
            colors.disabledColor = new Color(.6f, .6f, .6f);
            button.colors = colors;
            button.targetGraphic = rect.GetComponent<Image>();
            if (click != null) button.onClick.AddListener(() => click());
            var label = Text(rect, title);
            label.alignment = TextAlignmentOptions.Midline;
            Fill(label.rectTransform, 5);
            return button;
        }

        internal RectTransform Scroll(Transform parent, float x, float y, float width, float height)
        {
            var outer = Panel("Scroll", parent, new Color32(244, 244, 235, 255));
            Place(outer, x, y, width, height);
            var scroll = outer.gameObject.AddComponent<ScrollRect>();
            var viewport = Panel("Viewport", outer, Color.white);
            Fill(viewport);
            viewport.offsetMax = new Vector2(-20, 0);
            viewport.gameObject.AddComponent<RectMask2D>();
            var content = Rect("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(0, 1);
            content.sizeDelta = Vector2.zero;
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 8;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 35;
            var track = Panel("Scrollbar", outer, Surface);
            Place(track, width - 18, 0, 18, height);
            var handle = Panel("Handle", track, Color.gray);
            Fill(handle);
            var bar = track.gameObject.AddComponent<Scrollbar>();
            bar.handleRect = handle;
            bar.targetGraphic = handle.GetComponent<Image>();
            bar.direction = Scrollbar.Direction.BottomToTop;
            scroll.verticalScrollbar = bar;
            return content;
        }

        internal static void Row(Component component, float height)
        {
            var layout = component.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = layout.preferredHeight = height;
        }

        internal RectTransform Taskbar(Transform parent)
        {
            var outer = Panel("Taskbar", parent, Surface);
            Place(outer, 0, 906, 1280, 54);
            var viewport = Panel("Viewport", outer, Surface);
            Fill(viewport, 4);
            viewport.gameObject.AddComponent<RectMask2D>();
            var content = Rect("Tasks", viewport);
            content.anchorMin = Vector2.zero;
            content.anchorMax = new Vector2(0, 1);
            content.pivot = new Vector2(0, 1);
            content.sizeDelta = Vector2.zero;
            var layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            content.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scroll = outer.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = true;
            scroll.vertical = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 60;
            return content;
        }
    }
}
