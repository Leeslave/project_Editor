using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Win98 panels made from editable uGUI objects, without generated textures.</summary>
public static class RetroSaveUi
{
    public static readonly Color Face = new Color32(192, 192, 192, 255);
    public static readonly Color Blue = new Color32(0, 0, 128, 255);
    public static readonly Color Gray = new Color32(128, 128, 128, 255);

    public static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.layer = 5;
        var rect = (RectTransform)obj.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
        return rect;
    }

    public static Image Fill(string name, Transform parent, float x, float y, float w, float h, Color color)
    {
        Image image = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    public static void Stretch(RectTransform rect, float inset = 0)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }

    public static Text Label(string name, Transform parent, Font font, string value,
        float x, float y, float w, float h, int size = 20, Color? color = null,
        TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        Text text = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color ?? Color.black;
        text.alignment = alignment;
        text.supportRichText = false;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    public static Image Picture(string name, Transform parent, Sprite sprite,
        float x, float y, float w, float h, bool preserveAspect = true)
    {
        if (!preserveAspect)
        {
            RectTransform crop = Rect(name + "Clip", parent, x, y, w, h);
            crop.gameObject.AddComponent<RectMask2D>();
            Image cover = Fill(name, crop, 0, 0, w, h, Color.white);
            ((RectTransform)cover.transform).pivot = new Vector2(.5f, .5f);
            AspectRatioFitter fitter = cover.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            SetPicture(cover, sprite);
            return cover;
        }
        Image image = Fill(name, parent, x, y, w, h, Color.white);
        image.sprite = sprite;
        image.preserveAspect = preserveAspect;
        image.enabled = sprite != null;
        return image;
    }

    public static void SetPicture(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = sprite != null;
        AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
        if (fitter != null && sprite != null) fitter.aspectRatio = sprite.rect.width / sprite.rect.height;
    }

    public static RectTransform Frame(string name, Transform parent, float x, float y, float w, float h,
        bool inset = false, Color? fill = null)
    {
        Image background = Fill(name, parent, x, y, w, h, fill ?? Face);
        background.raycastTarget = true;
        Edge(background.transform, "LightOuter", 0, 0, w, h, inset ? Gray : Color.white, true);
        Edge(background.transform, "DarkOuter", 0, 0, w, h, inset ? Color.white : Color.black, false);
        Edge(background.transform, "LightInner", 1, 1, w - 2, h - 2, inset ? Color.black : new Color32(223,223,223,255), true);
        Edge(background.transform, "DarkInner", 1, 1, w - 2, h - 2, inset ? new Color32(223,223,223,255) : Gray, false);
        return (RectTransform)background.transform;
    }

    private static void Edge(Transform parent, string name, float x, float y, float w, float h, Color color, bool topLeft)
    {
        Fill(name + "Horizontal", parent, x, topLeft ? y : y + h - 1, w, 1, color);
        Fill(name + "Vertical", parent, topLeft ? x : x + w - 1, y, 1, h, color);
    }

    public static Button Button(string name, Transform parent, Font font, string value,
        float x, float y, float w, float h, UnityAction action = null)
    {
        RectTransform rect = Frame(name, parent, x, y, w, h);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(.96f, .96f, .96f);
        colors.pressedColor = new Color(.75f, .75f, .75f);
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0;
        button.colors = colors;
        Label("Label", rect, font, value, 4, 2, w - 8, h - 4, 20, null, TextAnchor.MiddleCenter);
        if (action != null) button.onClick.AddListener(action);
        return button;
    }

    public static void Line(Transform parent, Vector2 from, Vector2 to, Color color, bool dashed = false)
    {
        float distance = Vector2.Distance(from, to);
        if (distance < .01f) return;
        Vector2 direction = (to - from) / distance;
        float step = dashed ? 14 : distance;
        for (float start = 0; start < distance; start += step)
        {
            float length = Mathf.Min(dashed ? 7 : distance, distance - start);
            Vector2 point = from + direction * start;
            RectTransform rect = (RectTransform)Fill("RouteConnection", parent,
                point.x, point.y, length, 2, color).transform;
            rect.pivot = new Vector2(0, .5f);
            rect.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
}
