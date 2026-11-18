using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Comic dialogue presentation. Story actions remain owned by Chat.</summary>
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class ComicDialogueSkin : MonoBehaviour
{
    [SerializeField] private Sprite dialogueSprite;
    [SerializeField] private Sprite rightDialogueSprite;
    [SerializeField] private Sprite choiceSprite;
    [SerializeField] private Sprite speakerSprite;
    [SerializeField] private Sprite[] controlSprites;
    [SerializeField] private TMP_FontAsset font;

    private Chat chat;
    [SerializeField, HideInInspector] private Transform ui;
    [SerializeField, HideInInspector] private GameObject speech;
    [SerializeField, HideInInspector] private GameObject choices;
    [SerializeField, HideInInspector] private GameObject options;
    [SerializeField, HideInInspector] private GameObject history;
    [SerializeField, HideInInspector] private GameObject skipModal;
    [SerializeField, HideInInspector] private Button nextButton;
    [SerializeField, HideInInspector] private Button skipButton;
    [SerializeField, HideInInspector] private Button branchButton;
    [SerializeField, HideInInspector] private Button speechButton;
    [SerializeField, HideInInspector] private RectTransform layout;
    private Vector2 lastCanvasSize;
    [SerializeField, HideInInspector] private GameObject rolePlate;
    [SerializeField, HideInInspector] private TMP_Text roleInfo;
    [SerializeField, HideInInspector] private RectTransform speakerPlate;
    [SerializeField, HideInInspector] private RectTransform continueMarker;
    public bool SpeakerOnRight { get; private set; }
    private readonly Dictionary<string, Sprite[]> standingPortraits = new Dictionary<string, Sprite[]>();
    [SerializeField, HideInInspector] private Quaternion[] portraitFacings = new Quaternion[4];

    public float DialogueFontScale => 38f / TalkParagraph.NORMALFONTSIZE;

    private void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        chat = GetComponent<Chat>();
        if (ui != null)
        {
            WireGeneratedControls();
            RefreshState();
            return;
        }
        ui = transform.Find("ChatUI");
        speech = ui.Find("Speech").gameObject;
        choices = ui.Find("ChoicePanel").gameObject;
        options = ui.Find("OptionPanel").gameObject;
        history = ui.Find("RemindPanel").gameObject;
        skipModal = ui.Find("SkipPanel").gameObject;

        var scaler = GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = .5f;
        transform.localScale = Vector3.one;
        Stretch((RectTransform)ui);
        ui.GetComponent<Image>().color = Color.clear;

        // A centered reference frame keeps the approved proportions at any Game view size.
        var frame = Node("Comic Layout", ui);
        layout = frame;
        frame.anchorMin = frame.anchorMax = new Vector2(.5f, .5f);
        frame.pivot = new Vector2(.5f, .5f);
        frame.sizeDelta = new Vector2(1920, 1080);
        frame.anchoredPosition = Vector2.zero;
        speech.transform.SetParent(frame, false);
        choices.transform.SetParent(frame, false);
        options.transform.SetParent(frame, false);
        Place((RectTransform)speech.transform, 35, 720, 1550, 325);
        Style(speech.GetComponent<Image>(), dialogueSprite);
        Colors(speech.GetComponent<Button>());
        Place(chat.text.rectTransform, 105, 88, 1320, 205);
        TextStyle(chat.text, 38, Color.black);
        chat.text.alignment = TextAlignmentOptions.TopLeft;
        chat.text.overflowMode = TextOverflowModes.Ellipsis;
        chat.text.lineSpacing = 5;
        chat.text.enableAutoSizing = true;
        chat.text.fontSizeMin = 24;
        chat.text.fontSizeMax = 38;

        var plate = speech.transform.Find("Talker");
        speakerPlate = (RectTransform)plate;
        Place((RectTransform)plate, 65, 2, 260, 79);
        Style(plate.GetComponent<Image>(), speakerSprite);
        Place(chat.talkerName.rectTransform, 15, 0, 230, 69);
        TextStyle(chat.talkerName, 32, Color.white);
        chat.talkerName.alignment = TextAlignmentOptions.Center;
        chat.talkerName.enableAutoSizing = true;
        chat.talkerName.fontSizeMin = 20;
        chat.talkerName.fontSizeMax = 32;
        var info = plate.Find("Info").GetComponent<TMP_Text>();
        var role = Node("Speaker Info Plate", speech.transform).gameObject.AddComponent<Image>();
        Place((RectTransform)role.transform, 105, 86, 1320, 42);
        role.color = Color.clear;
        role.raycastTarget = false;
        rolePlate = role.gameObject;
        roleInfo = info;
        info.transform.SetParent(role.transform, false);
        Place(info.rectTransform, 0, 0, 1320, 42);
        TextStyle(info, 27, new Color(.25f, .25f, .25f));
        info.alignment = TextAlignmentOptions.MidlineLeft;
        info.enableAutoSizing = true;
        info.fontSizeMin = 22;
        info.fontSizeMax = 27;
        info.overflowMode = TextOverflowModes.Ellipsis;
        continueMarker = AddText("Continue Marker", speech.transform, "▼", 1430, 248, 45, 45, 30, Color.black).rectTransform;

        Place((RectTransform)choices.transform, 1200, 245, 660, 390);
        choices.GetComponent<Image>().color = Color.clear;
        choices.GetComponent<Image>().raycastTarget = false;
        for (int i = 0; i < choices.transform.childCount; i++)
        {
            var choice = choices.transform.GetChild(i);
            Place((RectTransform)choice, 0, i * 130, 660, 110);
            Style(choice.GetComponent<Image>(), choiceSprite);
            Colors(choice.GetComponent<Button>());
            // Chat.SetChoice addresses child zero; retain the existing text object and listener.
            var label = choice.GetChild(0).GetComponent<TMP_Text>();
            Place(label.rectTransform, 132, 10, 460, 82);
            TextStyle(label, 32, Color.black);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.enableAutoSizing = true;
            label.fontSizeMin = 22;
            label.fontSizeMax = 32;
            var badge = Node("Number Badge", choice).gameObject.AddComponent<Image>();
            Place((RectTransform)badge.transform, 8, 8, 100, 90);
            Style(badge, speakerSprite);
            badge.raycastTarget = false;
            var number = AddText("Number", badge.transform, (i + 1).ToString("00"), 0, 0, 100, 90, 30, Color.white);
            number.alignment = TextAlignmentOptions.Center;
        }

        Place((RectTransform)options.transform, 1620, 720, 260, 330);
        // A horizontal speaker plate cannot frame this tall control grid without distortion.
        var optionImage = options.GetComponent<Image>();
        optionImage.sprite = null;
        optionImage.color = Color.clear;
        var optionFrame = Node("Control Frame", options.transform);
        Stretch(optionFrame);
        optionFrame.offsetMin = new Vector2(-12, -12);
        optionFrame.offsetMax = new Vector2(12, 12);
        optionFrame.gameObject.AddComponent<Image>().raycastTarget = false;
        optionFrame.SetAsFirstSibling();
        var optionFill = Node("Fill", optionFrame);
        Stretch(optionFill);
        optionFill.offsetMin = new Vector2(3, 3);
        optionFill.offsetMax = new Vector2(-3, -3);
        var fillImage = optionFill.gameObject.AddComponent<Image>();
        fillImage.color = new Color(.025f, .025f, .025f, .95f);
        fillImage.raycastTarget = false;
        options.GetComponent<Image>().raycastTarget = false;
        var remind = options.transform.Find("Remind").GetComponent<Button>();
        skipButton = options.transform.Find("Skip").GetComponent<Button>();
        var buttons = new Button[6];
        buttons[0] = NewButton("Dialogue", options.transform);
        buttons[1] = NewButton("Branches", options.transform);
        buttons[2] = remind;
        buttons[3] = skipButton;
        buttons[4] = NewButton("Earlier History", options.transform);
        buttons[5] = NewButton("Continue", options.transform);
        string[] captions = { "대화", "선택지", "기록", "넘기기", "이전 기록", "계속" };
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            foreach (Transform child in button.transform) child.gameObject.SetActive(false);
            Place((RectTransform)button.transform, (i % 2) * 135, (i / 2) * 110, 125, 82);
            Style(button.GetComponent<Image>(), controlSprites[i]);
            Colors(button);
            var caption = AddText("Comic Caption", button.transform, captions[i], 0, 80, 125, 28, 19, Color.white);
            caption.alignment = TextAlignmentOptions.Center;
        }
        speechButton = buttons[0];
        branchButton = buttons[1];
        nextButton = buttons[5];
        speechButton.onClick.AddListener(() => speech.SetActive(!speech.activeSelf));
        branchButton.onClick.AddListener(FocusChoice);
        buttons[4].onClick.AddListener(ShowEarlierHistory);
        nextButton.onClick.AddListener(chat.NextChat);

        // Replace the old 0.25-scaled full-body portraits with a fixed close-up layout.
        float[] portraitX = { 30, 340, 980, 1290 };
        // Keep the close-up size, with lanyard ID cards level with the speaker name tab.
        float[] portraitY = { 335, 335, 340, 340 };
        for (int i = 0; i < 4; i++)
        {
            var portrait = ui.Find("Character_" + (i + 1)).GetComponent<Image>();
            var rect = portrait.rectTransform;
            var facing = rect.localRotation;
            portraitFacings[i] = facing;
            rect.SetParent(frame, false);
            Place(rect, portraitX[i], portraitY[i], 600, 1120);
            rect.pivot = new Vector2(.5f, 1);
            rect.anchoredPosition += new Vector2(300, 0);
            rect.localRotation = facing;
            rect.SetSiblingIndex(i);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
        }

        // Preserve the existing modal events while keeping their text readable at 1080p.
        var confirmPanel = skipModal.transform.Find("InfoPanel");
        Style(confirmPanel.GetComponent<Image>(), choiceSprite);
        confirmPanel.GetComponent<Image>().preserveAspect = false;
        var modalRect = (RectTransform)confirmPanel;
        modalRect.anchorMin = modalRect.anchorMax = modalRect.pivot = new Vector2(.5f, .5f);
        modalRect.anchoredPosition = Vector2.zero;
        modalRect.sizeDelta = new Vector2(1120, 480);
        modalRect.localScale = Vector3.one;
        Place((RectTransform)confirmPanel.Find("SkipInfo"), 100, 65, 920, 110);
        Place((RectTransform)confirmPanel.Find("SkipAddInfo"), 100, 190, 920, 60);
        Place((RectTransform)confirmPanel.Find("BackButton"), 160, 300, 340, 90);
        Place((RectTransform)confirmPanel.Find("SkipButton"), 620, 300, 340, 90);
        foreach (var label in confirmPanel.GetComponentsInChildren<TMP_Text>(true))
        {
            TextStyle(label, label.name == "SkipInfo" ? 40 : label.name == "SkipAddInfo" ? 26 : 30, Color.black);
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 22;
            label.fontSizeMax = label.name == "SkipInfo" ? 40 : label.name == "SkipAddInfo" ? 26 : 30;
            if (label.name == "Label")
            {
                Stretch(label.rectTransform);
                label.rectTransform.offsetMin = new Vector2(24, 10);
                label.rectTransform.offsetMax = new Vector2(-24, -10);
            }
        }
        foreach (var button in confirmPanel.GetComponentsInChildren<Button>(true))
        {
            Style(button.GetComponent<Image>(), choiceSprite);
            Colors(button);
            button.transform.localScale = Vector3.one;
        }
        var close = history.transform.Find("Return");
        var closeRect = (RectTransform)close;
        closeRect.anchorMin = closeRect.anchorMax = Vector2.one;
        closeRect.pivot = Vector2.one;
        closeRect.anchoredPosition = new Vector2(-45, -40);
        closeRect.sizeDelta = new Vector2(125, 82);
        Style(close.GetComponent<Image>(), controlSprites[4]);
        Colors(close.GetComponent<Button>());

        // Keep the original modal objects and their persistent UnityEvent bindings on top.
        history.transform.SetAsLastSibling();
        skipModal.transform.SetAsLastSibling();
        RefreshState();
    }

    private void WireGeneratedControls()
    {
        speechButton.onClick.RemoveListener(ToggleSpeech);
        speechButton.onClick.AddListener(ToggleSpeech);
        branchButton.onClick.RemoveListener(FocusChoice);
        branchButton.onClick.AddListener(FocusChoice);
        var earlier = options.transform.Find("Earlier History").GetComponent<Button>();
        earlier.onClick.RemoveListener(ShowEarlierHistory);
        earlier.onClick.AddListener(ShowEarlierHistory);
        nextButton.onClick.RemoveListener(chat.NextChat);
        nextButton.onClick.AddListener(chat.NextChat);
    }

    private void ToggleSpeech() => speech.SetActive(!speech.activeSelf);

    private void LateUpdate()
    {
        FitLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        FitLayout();
    }

    private void FitLayout()
    {
        if (layout == null) return;
        var size = ((RectTransform)transform).rect.size;
        if (size == lastCanvasSize || size.x <= 0 || size.y <= 0) return;
        lastCanvasSize = size;
        layout.localScale = Vector3.one * Mathf.Min(size.x / 1920f, size.y / 1080f);
    }

    public void RefreshState()
    {
        if (ui == null) return;
        bool choosing = chat.IsChoosing;
        nextButton.interactable = !choosing;
        skipButton.interactable = !choosing;
        speechButton.interactable = !choosing;
        branchButton.interactable = choosing;
        rolePlate.SetActive(!string.IsNullOrWhiteSpace(roleInfo.text));
        bool hasRole = rolePlate.activeSelf;
        Place(chat.text.rectTransform, 105, (hasRole ? 140 : 88) + (SpeakerOnRight ? 30 : 0), 1320, hasRole ? 155 : 205);
    }

    public void SetSpeaker(TalkParagraph paragraph)
    {
        if (ui == null) return;
        int speaker = FindSpeaker(paragraph);
        SpeakerOnRight = speaker >= 2;
        speech.GetComponent<Image>().sprite = SpeakerOnRight && rightDialogueSprite != null
            ? rightDialogueSprite : dialogueSprite;
        // The generated right sprite has more alpha padding; normalize its visible body
        // to the left sprite while keeping all text at the same screen coordinates.
        float offset = SpeakerOnRight ? 30 : 0;
        Place((RectTransform)speech.transform, 35, 720 - offset, 1550, SpeakerOnRight ? 395 : 325);
        Place(speakerPlate, SpeakerOnRight ? 980 : 65, 2 + offset, 260, 79);
        Place((RectTransform)rolePlate.transform, 105, 86 + offset, 1320, 42);
        Place(continueMarker, 1430, 248 + offset, 45, 45);
        roleInfo.alignment = SpeakerOnRight ? TextAlignmentOptions.MidlineRight : TextAlignmentOptions.MidlineLeft;
    }

    private static int FindSpeaker(TalkParagraph paragraph)
    {
        if (paragraph.characters == null || string.IsNullOrWhiteSpace(paragraph.talker)) return -1;
        string name = paragraph.talker.Trim();
        string alias = name;
        switch (name)
        {
            case "클레이튼": alias = "Klayton"; break;
            case "클로버": alias = "Clover"; break;
            case "렉스": alias = "Rex"; break;
            case "킹": alias = "King"; break;
            case "헨더슨": alias = "Henderson"; break;
            case "케네디": alias = "Kennedy"; break;
            case "월터": alias = "Walter"; break;
            case "프라이스": alias = "Price"; break;
            case "기계공": alias = "Mechanic"; break;
            case "수도사": alias = "Monk"; break;
            case "기자": alias = "Reporter"; break;
        }
        int highlighted = -1, highlightedCount = 0, visible = -1, visibleCount = 0;
        for (int i = 0; i < paragraph.characters.Length; i++)
        {
            var character = paragraph.characters[i];
            if (string.IsNullOrEmpty(character.fileName)) continue;
            visible = i; visibleCount++;
            string file = System.IO.Path.GetFileNameWithoutExtension(character.fileName);
            if (file.Equals(alias, System.StringComparison.OrdinalIgnoreCase)
                || file.StartsWith(alias + "_", System.StringComparison.OrdinalIgnoreCase)
                || file.Equals(name, System.StringComparison.OrdinalIgnoreCase)) return i;
            if (character.isHighlight) { highlighted = i; highlightedCount++; }
        }
        if (highlightedCount == 1) return highlighted;
        return visibleCount == 1 ? visible : -1;
    }

    public void ResetPanels()
    {
        if (ui == null) return;
        history.SetActive(false);
        skipModal.SetActive(false);
        speech.SetActive(true);
    }

    public void StyleHistoryNode(GameObject node)
    {
        foreach (var label in node.GetComponentsInChildren<TMP_Text>(true))
            TextStyle(label, label.name == "Talker" ? 32 : 28, Color.black);
    }

    public Sprite GetStandingPortrait(CharacterCG character)
    {
        string key = character.fileName;
        if (key == "Klayton_stand") key = "Klayton";
        if (key == "Clover_stand") key = "Clover";
        if (key != "Klayton" && key != "Clover") return null;
        if (!standingPortraits.TryGetValue(key, out var sprites))
        {
            sprites = Resources.LoadAll<Sprite>("Chat/Standing/" + key);
            System.Array.Sort(sprites, (a, b) => string.CompareOrdinal(a.name, b.name));
            standingPortraits.Add(key, sprites);
        }
        return character.index >= 0 && character.index < sprites.Length ? sprites[character.index] : null;
    }

    public void PositionPortrait(Image portrait, int slot, bool standing)
    {
        var rect = portrait.rectTransform;
        // These sheets contain square bust crops. Match the existing face size and
        // lanyard height rather than centering a square in the old full-body rectangle.
        bool klayton = portrait.sprite != null && portrait.sprite.name.StartsWith("Klayton_");
        rect.sizeDelta = standing ? new Vector2(430, 430) : new Vector2(600, 1120);
        bool flip = klayton ? slot >= 2 : slot < 2;
        rect.localRotation = standing ? Quaternion.Euler(0, flip ? 180 : 0, 0) : portraitFacings[slot];
        var position = rect.anchoredPosition;
        position.y = -(standing ? 410 : 335);
        position.x = new float[] { 330, 640, 1100, standing ? 1370 : 1280 }[slot];
        if (standing && portrait.sprite != null)
        {
            var offset = PortraitOffset(portrait.sprite.name);
            position += new Vector2(flip ? -offset.x : offset.x, offset.y);
        }
        rect.anchoredPosition = position;
    }

    // Align visible silhouette center and baseline to expression 00; do not accumulate offsets.
    private static Vector2 PortraitOffset(string name) => name switch
    {
            "Klayton_00" => new Vector2(0.0000f, 0.0000f),
            "Klayton_01" => new Vector2(24.6890f, 0.0000f),
            "Klayton_02" => new Vector2(42.6914f, 0.0000f),
            "Klayton_03" => new Vector2(0.0000f, 0.0000f),
            "Klayton_04" => new Vector2(25.2033f, 0.0000f),
            "Klayton_05" => new Vector2(42.6914f, 0.0000f),
            "Klayton_06" => new Vector2(0.0000f, -3.0861f),
            "Klayton_07" => new Vector2(25.2033f, -4.1148f),
            "Clover_00" => new Vector2(0.0000f, 0.0000f),
            "Clover_01" => new Vector2(26.7464f, 0.0000f),
            "Clover_02" => new Vector2(54.0072f, 0.0000f),
            "Clover_03" => new Vector2(0.0000f, 0.0000f),
            "Clover_04" => new Vector2(26.7464f, 0.0000f),
            "Clover_05" => new Vector2(53.4928f, 0.0000f),
            "Clover_06" => new Vector2(0.5144f, 0.0000f),
            "Clover_07" => new Vector2(27.2608f, 0.0000f),
            "Clover_08" => new Vector2(54.0072f, 0.0000f),
        _ => Vector2.zero
    };

    private void FocusChoice()
    {
        if (!chat.IsChoosing || EventSystem.current == null) return;
        foreach (Transform choice in choices.transform)
            if (choice.gameObject.activeSelf && choice.GetComponent<Button>().interactable)
            {
                EventSystem.current.SetSelectedGameObject(choice.gameObject);
                return;
            }
    }

    private void ShowEarlierHistory()
    {
        history.SetActive(true);
        options.SetActive(false);
        Canvas.ForceUpdateCanvases();
        history.GetComponentInChildren<ScrollRect>(true).verticalNormalizedPosition = 1;
    }

    private Button NewButton(string name, Transform parent)
    {
        var image = Node(name, parent).gameObject.AddComponent<Image>();
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        return button;
    }

    private TMP_Text AddText(string name, Transform parent, string value, float x, float y, float w, float h, int size, Color color)
    {
        var label = Node(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
        label.text = value;
        Place(label.rectTransform, x, y, w, h);
        TextStyle(label, size, color);
        return label;
    }

    private void TextStyle(TMP_Text text, int size, Color color)
    {
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = FontStyles.Normal;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        text.margin = Vector4.zero;
    }

    private static void Style(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.color = Color.white;
        image.preserveAspect = false;
    }

    private static void Colors(Button button)
    {
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(.82f, .82f, .82f);
        colors.pressedColor = new Color(.55f, .55f, .55f);
        colors.selectedColor = new Color(.9f, .9f, .9f);
        colors.disabledColor = new Color(.46f, .46f, .46f, .7f);
        button.colors = colors;
    }

    private static RectTransform Node(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.gameObject.layer = parent.gameObject.layer;
        rect.SetParent(parent, false);
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void Place(RectTransform rect, float x, float y, float w, float h)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }
}
