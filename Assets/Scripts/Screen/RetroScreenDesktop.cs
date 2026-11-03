using System;
using System.Collections.Generic;
using System.Linq;
using GameService;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Screen 씬의 표시와 창 포커스만 담당한다. 업무·메일·저장 처리는 기존 매니저에 위임한다.</summary>
[ExecuteAlways]
public sealed class RetroScreenDesktop : MonoBehaviour
{
    public ScreenManager screenManager;
    public WorkManager workManager;
    public MailManager mailManager;
    public MsgManager messageManager;
    public NewspaperManager newsManager;
    public MenuManager menuManager;
    public RectTransform fittedDesktop, taskRows;
    public GameObject homeWindow, mailWindow, messageWindow, newsWindow, menuWindow;
    public GameObject mailEmpty, inboxEmpty, messageEmpty;
    public Text dateText, connectionText, workSummary, taskEmpty, footerText;
    public Font font;
    public Sprite inset;
    public Button powerButton, resetButton;
    public GameObject bootPrompt;
    private string lastSignature;
    private float nextRefresh;

    private void Start()
    {
        if (Application.isPlaying)
        {
            RefreshPresentation();
            gameObject.AddComponent<FirstDayScreenTutorial>().desktop = this;
        }
    }

    private void LateUpdate()
    {
        if (fittedDesktop != null && fittedDesktop.parent is RectTransform parent)
        {
            float scale = Mathf.Min(parent.rect.width / 1600f, parent.rect.height / 1200f);
            fittedDesktop.localScale = Vector3.one * scale;
        }
        if (!Application.isPlaying || Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + .25f;
        RefreshPresentation();
    }

    public void RefreshPresentation()
    {
        IWorkService service = Application.isPlaying ? GameSystem.GetService<IWorkService>() : null;
        IDayService day = Application.isPlaying ? GameSystem.GetService<IDayService>() : null;
        var works = service?.GetList() ?? new List<(string code, string name)>();
        int done = works.Count(w => service != null && service.IsWorkClear(w.code));
        workSummary.text = service == null ? "업무 정보 대기" : $"배정 {works.Count:00}   /   완료 {done:00}";
        dateText.text = day == null ? "업무용 내부 단말" : day.GetDateInfo() + "   " + day.TimeData.daytime;
        bool on = screenManager != null && screenManager.desktop.activeSelf;
        connectionText.text = on ? "업무 화면" : screenManager.CurrentMode == ScreenManager.ScreenMode.OnBoot ? "부팅 중" :
            screenManager.CurrentMode == ScreenManager.ScreenMode.TryOff ? "종료 확인" : "전원 꺼짐";
        if (powerButton != null) powerButton.interactable = screenManager.CurrentMode != ScreenManager.ScreenMode.OnBoot;
        if (resetButton != null) resetButton.interactable = screenManager.CurrentMode == ScreenManager.ScreenMode.On;
        if (bootPrompt != null) bootPrompt.SetActive(screenManager.CurrentMode == ScreenManager.ScreenMode.Off);
        footerText.text = on ? "접근한 자료와 업무 내용은 외부로 반출할 수 없습니다." :
            screenManager.CurrentMode == ScreenManager.ScreenMode.TryOff ? "전원 버튼을 다시 누르면 단말기를 종료합니다." :
            screenManager.CurrentMode == ScreenManager.ScreenMode.OnBoot ? "업무 환경을 준비하고 있습니다." :
            "전원 버튼을 눌러 업무 단말기를 시작하십시오.";
        string signature = service == null ? "unavailable" : string.Join("|", works.Select(w => w.code + ":" + w.name + ":" + service.IsWorkClear(w.code)));
        if (signature != lastSignature)
        {
            lastSignature = signature;
            RenderTasks(works.Select(w => (w.code, w.name, service.IsWorkClear(w.code))).ToList());
        }
        if (mailEmpty != null) mailEmpty.SetActive(mailManager != null && !mailManager.mailPanel.gameObject.activeSelf);
        if (inboxEmpty != null) inboxEmpty.SetActive(mailManager != null && mailManager.mailList.childCount == 0);
        if (messageEmpty != null) messageEmpty.SetActive(messageManager != null && messageManager.MsgListPanel.childCount == 0);
    }

    public void PreviewAssignments(DailyData data)
    {
        if (Application.isPlaying || data == null) return;
        dateText.text = data.date + "   " + data.dayTimes[0].daytime;
        RenderTasks(data.workList.Where(w => w != null).Select(w => (w.code,
            string.IsNullOrWhiteSpace(w.name) ? Friendly(w.code) : w.name, false)).ToList());
        workSummary.text = $"배정 {data.workList.Count:00}   /   완료 00";
    }

    private static string Friendly(string code)
    {
        switch (code)
        {
            case "Document": return "진술 대조 업무";
            case "SecureDocument": return "기밀 문서 수정";
            case "ADFGVX": return "암호 통신 처리";
            case "ADFGVX_DT": return "암호해독 교육";
            case "ADFGVX_ET": return "암호작성 교육";
            case "Maze": return "접근 경로 검증";
            case "Dodge": return "보안 검증";
            default: return code;
        }
    }

    private void RenderTasks(List<(string code, string name, bool done)> works)
    {
        foreach (Transform child in taskRows)
        {
            child.gameObject.SetActive(false);
            if (Application.isPlaying) Destroy(child.gameObject); else DestroyImmediate(child.gameObject);
        }
        taskEmpty.gameObject.SetActive(works.Count == 0);
        taskEmpty.text = Application.isPlaying ? "현재 배정된 업무가 없습니다." : "배정된 업무는 접속 후 표시됩니다.";
        for (int i = 0; i < works.Count; i++)
        {
            var work = works[i];
            var row = RetroScreenUi.Panel("Assignment " + work.code, taskRows, 0, i * 82, 810, 74, RetroScreenUi.Paper, inset);
            RetroScreenUi.Label("Number", row.transform, font, (i + 1).ToString("00"), 16, 10, 48, 50, 22, new Color(.35f, .4f, .34f));
            string name = string.IsNullOrWhiteSpace(work.name) || work.name == work.code ? Friendly(work.code) : work.name;
            RetroScreenUi.Label("Work", row.transform, font, name, 82, 7, 550, 34, 23);
            RetroScreenUi.Label("Code", row.transform, font, work.code, 82, 39, 550, 25, 17, new Color(.35f, .4f, .34f));
            RetroScreenUi.Label("State", row.transform, font, work.done ? "완료" : "미완료", 650, 12, 140, 48, 22,
                work.done ? new Color(.2f, .4f, .25f) : new Color(.4f, .25f, .2f), TextAnchor.MiddleRight);
        }
        taskRows.sizeDelta = new Vector2(810, Mathf.Max(1, works.Count * 82));
    }

    private static void Focus(GameObject window) { window.SetActive(true); window.transform.SetAsLastSibling(); }
    public void OpenHome() { Focus(homeWindow); }
    public void OpenWork() { workManager.ActiveWorkWindow(); workManager.taskWindow.transform.SetAsLastSibling(); }
    public void OpenMail() { Focus(mailWindow); }
    public void OpenMessages() { Focus(messageWindow); }
    public void OpenNews() { newsManager.ActivateWindow(); newsWindow.transform.SetAsLastSibling(); }
    public void OpenMenu() { menuManager.ActivateWindow(); if (menuWindow.activeSelf) menuWindow.transform.SetAsLastSibling(); }
}
