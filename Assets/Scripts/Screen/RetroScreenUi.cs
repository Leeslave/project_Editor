using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>프로젝트의 Desktop90 스프라이트와 글꼴을 사용하는 단말기 UI.</summary>
public static class RetroScreenUi
{
    public static readonly Color Desk = new Color32(24, 43, 40, 255);
    public static readonly Color Face = new Color32(199, 201, 187, 255);
    public static readonly Color Paper = new Color32(232, 231, 214, 255);
    public static readonly Color Ink = new Color32(28, 42, 35, 255);
    public static readonly Color Title = new Color32(42, 64, 53, 255);
    public static readonly Color Green = new Color32(170, 204, 160, 255);

    public static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.gameObject.layer = 5;
        rect.SetParent(parent, false);
        Place(rect, x, y, w, h);
        return rect;
    }

    public static void Place(RectTransform rect, float x, float y, float w, float h)
    {
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
    }

    public static Image Panel(string name, Transform parent, float x, float y, float w, float h,
        Color color, Sprite sprite = null, bool blocks = false)
    {
        var image = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Image>();
        image.color = color;
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.raycastTarget = blocks;
        return image;
    }

    public static Text Label(string name, Transform parent, Font font, string value,
        float x, float y, float w, float h, int size = 24, Color? color = null,
        TextAnchor align = TextAnchor.MiddleLeft)
    {
        var label = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Text>();
        label.font = font;
        label.text = value;
        label.fontSize = size;
        label.color = color ?? Ink;
        label.alignment = align;
        label.supportRichText = false;
        label.raycastTarget = false;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        return label;
    }

    public static Image Icon(string name, Transform parent, Sprite sprite, float x, float y, float w, float h)
    {
        var image = Panel(name, parent, x, y, w, h, Color.white);
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        image.enabled = sprite != null;
        return image;
    }

    public static Button Button(string name, Transform parent, Font font, string value,
        float x, float y, float w, float h, Sprite normal, Sprite selected, Sprite pressed, UnityAction action = null)
    {
        var image = Panel(name, parent, x, y, w, h, Face, normal, true);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState { highlightedSprite = selected, selectedSprite = selected, pressedSprite = pressed };
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        Label("Label", image.transform, font, value, 12, 4, w - 24, h - 8, 23, null, TextAnchor.MiddleCenter);
        if (action != null) button.onClick.AddListener(action);
        return button;
    }
}
