using System.Collections;
using System.Collections.Generic;
using GameService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>단말기 전원 조작 후 실제 데스크톱 구성요소를 순서대로 안내한다.</summary>
public sealed class FirstDayScreenTutorial : MonoBehaviour
{
    public RetroScreenDesktop desktop;
    public TutorialController Spotlight { get; private set; }

    private IEnumerator Start()
    {
        while (GameSystem.SaveService == null || GameSystem.SaveService.CurrentSaveId < 0 ||
               GameSystem.GetService<IDayService>() == null) yield return null;
        if (GameSystem.SaveService.DayIndex != 1 || GameSystem.GetService<IDayService>().Time != 1 ||
            FirstDayOnboarding.ScreenComplete) yield break;

        var prefab = Resources.Load<GameObject>("Tutorial/FirstDaySpotlight");
        if (prefab == null)
        {
            Debug.LogError("스크린 안내용 조명 프리팹이 없습니다.");
            yield break;
        }
        var overlay = Instantiate(prefab, transform);
        // 별도 루트 Canvas로 두어 단말기 화면 배율을 이중으로 적용하지 않는다.
        overlay.transform.SetParent(null, false);
        overlay.name = "First Day Screen Spotlight";
        Spotlight = overlay.GetComponent<TutorialController>();
        Spotlight.AutoStart = false;
        PrepareOverlay(overlay);
        overlay.SetActive(false);
        Spotlight.Steps = new List<TutorialController.TutorialStep>();
        if (desktop.screenManager.CurrentMode == ScreenManager.ScreenMode.Off)
        {
            Spotlight.Steps.Add(FirstDayOnboarding.Click(desktop.powerButton.gameObject,
                "스크린 전원 켜기\n강조된 전원 버튼을 눌러 업무 단말기를 켜세요.\n부팅이 끝나면 화면의 구성요소를 안내합니다.",
                _ => desktop.powerButton.onClick.Invoke()));
            Spotlight.StartTutorial();
            while (Spotlight.gameObject.activeSelf) yield return null;
        }
        while (desktop.screenManager.CurrentMode != ScreenManager.ScreenMode.On ||
               GameSystem.Instance.IsLoading) yield return null;

        desktop.OpenHome();
        Spotlight.Steps = new List<TutorialController.TutorialStep>();
        Explain(desktop.homeWindow,
            "업무 단말\n중앙 화면에서 오늘 배정된 업무와 완료 상태를 확인할 수 있습니다.\n왼쪽의 버튼으로 필요한 프로그램을 엽니다.\n화면을 눌러 다음 안내로 진행하세요.");
        Open("Launcher 2", "수신함\n내부 메일과 업무 지시를 확인하는 곳입니다.\n강조된 수신함 버튼을 눌러 보세요.", desktop.OpenMail);
        Explain(desktop.mailWindow,
            "메일 읽기\n왼쪽 목록에서 제목을 누르면 오른쪽에 본문이 열립니다.\n지시사항과 첨부된 자료를 확인한 뒤 업무를 진행하세요.\n화면을 눌러 다음으로 진행하세요.", () => desktop.mailWindow.SetActive(false));
        Open("Launcher 3", "메신저\n동료와 주고받은 연락을 확인합니다.\n강조된 메신저 버튼을 눌러 보세요.", desktop.OpenMessages);
        Explain(desktop.messageWindow,
            "연락 확인\n대화 목록을 선택하면 동료의 메시지를 읽을 수 있습니다.\n새로운 연락과 업무에 필요한 정보를 확인하세요.\n화면을 눌러 다음으로 진행하세요.", () => desktop.messageWindow.SetActive(false));
        Open("Launcher 4", "신문 열람\n그날의 보도와 국가 소식을 확인할 수 있습니다.\n강조된 신문 열람 버튼을 눌러 보세요.", desktop.OpenNews);
        Explain(desktop.newsWindow,
            "신문과 보도\n신문에 실린 사건과 보도 내용을 살펴볼 수 있습니다.\n화면을 눌러 다음으로 진행하세요.", () => desktop.newsWindow.SetActive(false));
        Open("Launcher 5", "프로그램\n단말기에 설치된 프로그램을 확인하는 메뉴입니다.\n강조된 프로그램 버튼을 눌러 보세요.", desktop.OpenMenu);
        Explain(desktop.menuWindow,
            "프로그램 메뉴\n필요한 프로그램을 이곳에서도 실행할 수 있습니다.\n화면을 눌러 다음으로 진행하세요.", () => desktop.menuWindow.SetActive(false));
        Explain(desktop.powerButton.gameObject,
            "전원과 재부팅\n아래쪽 전원 버튼으로 단말기를 종료하고, 재부팅 버튼으로 다시 시작합니다.\n전원이 꺼지면 ‘자리로’ 버튼으로 사무실에 돌아갈 수 있습니다.\n화면을 눌러 업무 안내로 진행하세요.");
        Open("Launcher 1", "업무 지원\n배정된 업무를 실행하는 프로그램입니다.\n강조된 업무 지원 버튼을 눌러 보세요.", desktop.OpenWork);
        Explain(desktop.workManager.taskWindow,
            "업무 시작 준비 완료\n업무 목록에서 오늘 배정된 교육을 확인하세요.\n입력란에 업무 코드나 이름을 입력하고 Enter를 누르면 실행됩니다.\n오늘은 암호 해독 교육부터 시작합니다.\n화면을 누르면 안내를 마칩니다.", () => FirstDayOnboarding.ScreenComplete = true);
        Spotlight.StartTutorial();
    }

    private void Open(string name, string text, UnityEngine.Events.UnityAction action)
    {
        Spotlight.Steps.Add(FirstDayOnboarding.Click(desktop.screenManager.desktop.transform.Find(name).gameObject,
            text, _ => action()));
    }

    private void Explain(GameObject target, string text, UnityEngine.Events.UnityAction completed = null)
    {
        Spotlight.Steps.Add(FirstDayOnboarding.Click(target, text, _ => completed?.Invoke(), true));
    }

    private void OnDestroy()
    {
        if (Spotlight != null) Destroy(Spotlight.gameObject);
    }

    public static void PrepareOverlay(GameObject overlay)
    {
        var canvas = overlay.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var rootRect = (RectTransform)overlay.transform;
        rootRect.localScale = Vector3.one;
        foreach (var border in overlay.GetComponentsInChildren<TutorialBorder>(true))
        {
            var rect = (RectTransform)border.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }
        var scaler = overlay.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1200, 900);
        scaler.matchWidthOrHeight = .5f;
        var label = overlay.GetComponentInChildren<TMP_Text>(true);
        label.fontSize = 25;
        if (Chat.Instance != null) label.font = Chat.Instance.text.font;
        label.color = new Color(.08f, .08f, .08f);
        label.enableWordWrapping = true;
        label.lineSpacing = 8;
        label.enableAutoSizing = false;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.raycastTarget = false;
        var card = (RectTransform)label.transform.parent;
        foreach (var fitter in card.GetComponentsInChildren<ContentSizeFitter>(true)) Destroy(fitter);
        foreach (var layout in card.GetComponentsInChildren<LayoutGroup>(true)) Destroy(layout);
        card.anchorMin = card.anchorMax = card.pivot = new Vector2(.5f, .5f);
        card.sizeDelta = new Vector2(640, 240);
        card.localScale = Vector3.one;
        var cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.sprite = Resources.Load<Sprite>("Tutorial/GuideCard");
            cardImage.color = Color.white;
            cardImage.type = Image.Type.Simple;
        }
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = new Vector2(42, 24);
        label.rectTransform.offsetMax = new Vector2(-42, -24);
        foreach (var image in card.GetComponentsInChildren<Image>(true)) image.raycastTarget = false;
        foreach (var border in overlay.GetComponentsInChildren<TutorialBorder>(true))
            border.SetDescriptionPosition(new Vector2(.6f, .22f));
    }
}
