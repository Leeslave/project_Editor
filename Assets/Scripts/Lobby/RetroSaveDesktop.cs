using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using UnityEngine;
using UnityEngine.UI;
using Utility;

/// <summary>Scene-bound save explorer. All selectable records come from PlayerData.</summary>
public sealed class RetroSaveDesktop : MonoBehaviour
{
    [Header("Existing project resources")]
    public Font font;
    public Sprite folderIcon;
    public Sprite defaultThumbnail;
    [Header("Explorer objects")]
    public RectTransform dayContent;
    public RectTransform slotContent;
    public RetroSaveSlot slotPrefab;
    public Text address;
    public Text itemCount;
    public Text recordTitle;
    public Text recordDetails;
    public Text status;
    public Image recordPreview;
    public Button loadButton;
    [Header("Branch map objects")]
    public GameObject mapWindow;
    public RectTransform routeContent;
    public RectTransform graphContent;
    public Text mapTitle;
    public Text mapDetails;
    public Image mapPreview;
    public Button selectRecordButton;
    public GameObject mapTask;

    private readonly List<DaySave> records = new();
    private DaySave selected;
    private DaySave mapSelected;
    private int? routeFilter;
    private bool navigating;
    public IReadOnlyList<DaySave> Records => records;
    public int SelectedSaveId => selected?.dayID ?? -1;
    public int MapSelectedSaveId => mapSelected?.dayID ?? -1;

    private void Start()
    {
        try
        {
            SaveService service = GameSystem.SaveService;
            PlayerData data;
            if (service != null)
            {
                service.Init();
                data = service.GetPlayerData();
            }
            else data = DataLoader.GetPlayerData(); // Standalone scene preview remains read-only.
            if (data == null) { data = new PlayerData(); data.Init(); }
            Bind(data);
            if (service == null) status.text = "게임 시작 화면에서 기록을 불러와 주세요.";
        }
        catch (Exception exception)
        {
            status.text = "저장 기록을 읽지 못했습니다. 파일을 확인해 주세요.";
            loadButton.interactable = false;
            Debug.LogException(exception, this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && mapWindow.activeSelf) CloseMap();
    }

    public static string BranchName(int branch) => branch == 0 ? "기본 분기" : $"분기 {branch:00}";
    public static string FileName(DaySave save) => $"day{save.Day:00}_{save.Branch:00}.sav";

    public void Bind(PlayerData data)
    {
        records.Clear();
        if (data != null) records.AddRange(data.Where(item => item != null).OrderBy(item => item.dayID));
        selected = records.LastOrDefault();
        mapSelected = selected;
        routeFilter = null;
        RenderExplorer();
        RenderMap();
        CloseMap();
        status.text = records.Count == 0 ? "저장된 기록이 없습니다." : "날짜와 분기를 선택한 뒤 불러오기를 누르세요.";
    }

    public void SelectRecord(int id)
    {
        DaySave record = records.FirstOrDefault(item => item.dayID == id);
        if (record == null) return;
        selected = record;
        RenderExplorer();
        status.text = $"{record.Day}일차 / {BranchName(record.Branch)} 선택됨";
    }

    private void RenderExplorer()
    {
        Clear(dayContent);
        int index = 0;
        foreach (int day in records.Select(item => item.Day).Distinct().OrderByDescending(day => day))
        {
            int capturedDay = day;
            Button button = RetroSaveUi.Button($"Day_{day:00}", dayContent, font, $"    {day}일차",
                8, 8 + index++ * 52, 213, 46, () => SelectRecord(records.First(item => item.Day == capturedDay).dayID));
            bool active = selected != null && day == selected.Day;
            Image icon = RetroSaveUi.Picture("FolderIcon", button.transform, folderIcon, 12, 12, 26, 22);
            icon.color = active ? Color.white : new Color32(166, 139, 47, 255);
            SetSelected(button, active);
        }
        dayContent.sizeDelta = new Vector2(dayContent.sizeDelta.x, Mathf.Max(410, index * 52 + 16));
        Clear(slotContent);
        List<DaySave> slots = selected == null ? new List<DaySave>() : records.Where(item => item.Day == selected.Day).ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            DaySave record = slots[i];
            RetroSaveSlot slot = Instantiate(slotPrefab, slotContent);
            slot.name = $"Save_{record.dayID:0000}";
            slot.gameObject.SetActive(true);
            RectTransform rect = (RectTransform)slot.transform;
            rect.anchoredPosition = new Vector2(16 + i % 2 * 478,  -12 - i / 2 * 234);
            slot.Bind(record, Thumbnail(record), record == selected, () => SelectRecord(record.dayID));
        }
        slotContent.sizeDelta = new Vector2(976, Mathf.Max(250, ((slots.Count + 1) / 2) * 234 + 20));
        address.text = selected == null ? "내 기록" : $"내 기록 \\ {selected.Day}일차";
        itemCount.text = $"{slots.Count}개 항목";
        recordTitle.text = selected == null ? "저장된 기록 없음" : $"{selected.Day}일차 · {BranchName(selected.Branch)}";
        recordDetails.text = selected == null ? "이야기를 시작하면 기록이 생성됩니다." :
            $"파일 이름   {FileName(selected)}\n명성        {selected.renown}\n저장 방식   자동 저장";
        RetroSaveUi.SetPicture(recordPreview, selected == null ? defaultThumbnail : Thumbnail(selected));
        loadButton.interactable = selected != null && GameSystem.SaveService != null && GameSystem.Instance != null && !navigating;
        Reveal(dayContent, Mathf.Max(0, records.Select(item => item.Day).Distinct().OrderByDescending(day => day).ToList()
            .IndexOf(selected?.Day ?? -1) * 52 - 30));
        Reveal(slotContent, Mathf.Max(0, slots.IndexOf(selected) / 2 * 234 - 10));
    }

    public void OpenMap()
    {
        mapSelected = selected;
        routeFilter = null;
        RenderMap();
        mapWindow.SetActive(true);
        mapWindow.transform.SetAsLastSibling();
        mapTask.SetActive(true);
    }

    public void CloseMap()
    {
        mapWindow.SetActive(false);
        mapTask.SetActive(false);
    }

    public void FilterRoute(int branch)
    {
        routeFilter = branch < 0 ? (int?)null : branch;
        RenderMap();
    }

    public void SelectMapRecord(int id)
    {
        mapSelected = records.FirstOrDefault(item => item.dayID == id);
        RenderMap();
    }

    public void UseMapRecord()
    {
        if (mapSelected == null) return;
        SelectRecord(mapSelected.dayID);
        CloseMap();
    }

    private void RenderMap()
    {
        Clear(routeContent);
        Button all = RetroSaveUi.Button("AllRoutes", routeContent, font, "전체 보기", 6, 6, 186, 44, () => FilterRoute(-1));
        SetSelected(all, routeFilter == null);
        int[] branches = records.Select(item => item.Branch).Distinct().OrderBy(branch => branch).ToArray();
        for (int i = 0; i < branches.Length; i++)
        {
            int branch = branches[i];
            Button button = RetroSaveUi.Button($"Route_{branch:00}", routeContent, font, BranchName(branch),
                6, 59 + i * 49, 186, 43, () => FilterRoute(branch));
            SetSelected(button, routeFilter == branch);
        }
        routeContent.sizeDelta = new Vector2(198, Mathf.Max(470, 70 + branches.Length * 49));
        Clear(graphContent);
        int[] days = records.Select(item => item.Day).Distinct().OrderBy(day => day).ToArray();
        int[] shownBranches = branches.Where(branch => !routeFilter.HasValue || branch == routeFilter.Value).ToArray();
        graphContent.sizeDelta = new Vector2(Mathf.Max(910, 180 + days.Length * 140), Mathf.Max(240, 40 + shownBranches.Length * 105));
        for (int row = 0; row < shownBranches.Length; row++)
        {
            int branch = shownBranches[row];
            float y = 53 + row * 105;
            RetroSaveUi.Label("RouteLabel", graphContent, font, BranchName(branch), 12, y - 10, 125, 40, 18,
                mapSelected?.Branch == branch ? RetroSaveUi.Blue : new Color(.3f, .3f, .3f));
            List<DaySave> route = records.Where(item => item.Branch == branch).OrderBy(item => item.Day).ToList();
            Color color = mapSelected?.Branch == branch ? RetroSaveUi.Blue : new Color(.35f, .35f, .35f);
            for (int i = 1; i < route.Count; i++)
            {
                float fromX = 175 + Array.IndexOf(days, route[i - 1].Day) * 140;
                float toX = 175 + Array.IndexOf(days, route[i].Day) * 140;
                // Connect only known saves within the same branch; parentage is not stored.
                RetroSaveUi.Line(graphContent, new Vector2(fromX, y), new Vector2(toX, y), color,
                    route[i].Day - route[i - 1].Day > 1);
            }
            foreach (DaySave record in route)
            {
                float x = 175 + Array.IndexOf(days, record.Day) * 140;
                Button node = RetroSaveUi.Button($"Node_{record.dayID:0000}", graphContent, font, "",
                    x - 15, y - 15, 30, 30, () => SelectMapRecord(record.dayID));
                node.targetGraphic.color = mapSelected == record ? RetroSaveUi.Blue : Color.white;
                RetroSaveUi.Label("DayLabel", graphContent, font, $"{record.Day}일차", x - 48, y + 20, 96, 30,
                    18, color, TextAnchor.MiddleCenter);
            }
        }
        if (records.Count == 0)
            RetroSaveUi.Label("EmptyMap", graphContent, font, "저장된 기록이 생기면 지도가 표시됩니다.", 30, 65, 820, 60, 20);
        mapTitle.text = mapSelected == null ? "선택한 기록 없음" : $"{mapSelected.Day}일차 · {BranchName(mapSelected.Branch)}";
        mapDetails.text = mapSelected == null ? "지도에서 저장된 날짜를 선택하세요." :
            $"명성 {mapSelected.renown} · {FileName(mapSelected)}\n이 기록을 선택하면 불러오기 화면으로 돌아갑니다.";
        RetroSaveUi.SetPicture(mapPreview, mapSelected == null ? defaultThumbnail : Thumbnail(mapSelected));
        selectRecordButton.interactable = mapSelected != null;
        Reveal(routeContent, 0);
        Reveal(graphContent, Mathf.Max(0, Array.IndexOf(shownBranches, mapSelected?.Branch ?? -1) * 105 - 60));
    }

    public void LoadSelected()
    {
        GameSystem system = GameSystem.Instance;
        SaveService service = GameSystem.SaveService;
        if (selected == null || navigating || system == null || service == null || system.IsLoading) return;
        int day = selected.Day;
        int branch = selected.Branch;
        navigating = true;
        loadButton.interactable = false;
        status.text = "기록을 불러오는 중입니다...";
        system.EnterScene("MainWorld", () => service.SelectDay(day, branch));
    }

    public void ReturnToTitle()
    {
        GameSystem system = GameSystem.Instance;
        if (system == null || system.IsLoading || navigating) return;
        navigating = true;
        system.EnterScene("GameStart");
    }

    private Sprite Thumbnail(DaySave save)
    {
        if (save != null && !string.IsNullOrEmpty(save.thumbnail))
        {
            Sprite sprite = Resources.Load<Sprite>($"thumbnail/{save.thumbnail}");
            if (sprite != null) return sprite;
        }
        return defaultThumbnail;
    }

    private static void SetSelected(Button button, bool active)
    {
        button.targetGraphic.color = active ? RetroSaveUi.Blue : Color.white;
        Text label = button.transform.Find("Label").GetComponent<Text>();
        label.color = active ? Color.white : Color.black;
    }

    private static void Reveal(RectTransform content, float top)
    {
        ScrollRect scroll = content.parent.parent.GetComponent<ScrollRect>();
        if (scroll == null) return;
        Canvas.ForceUpdateCanvases();
        scroll.StopMovement();
        float overflow = content.rect.height - scroll.viewport.rect.height;
        scroll.verticalNormalizedPosition = overflow > 0 ? 1 - Mathf.Clamp01(top / overflow) : 1;
        scroll.horizontalNormalizedPosition = 0;
    }

    private static void Clear(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(false);
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }
    }
}
