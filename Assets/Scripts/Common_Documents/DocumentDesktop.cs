using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.Documents
{
    /// <summary>Common desktop presentation. Scoring, completion and persistent storage belong to game services.</summary>
    public sealed class DocumentDesktop : MonoBehaviour
    {
        public event Action<ViewRecord> DocumentOpened;
        public event Action<string, string> PairSelectionChanged;
        public event Action<string, ProfileField, string> ProfileChanged;
        public event Action RecordRequested;
        public event Action SubmissionRequested;
        private DocumentAssignment assignment;
        private DocumentPlayState state;
        private DocumentDesktopUI ui;
        private RectTransform root, area, taskbar;
        private TMP_Text progress;
        private Button record, cancel, submit;
        private string statementId, actionId, selectedPerson, selectedDate;
        private bool submissionAllowed = true;
        private readonly Dictionary<string, DocumentDesktopWindow> windows = new Dictionary<string, DocumentDesktopWindow>();
        private readonly Dictionary<string, Button> tasks = new Dictionary<string, Button>();
        private readonly Dictionary<string, DocumentContent> documents = new Dictionary<string, DocumentContent>();
        private readonly Dictionary<string, Button> lineButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, PersonContent> people = new Dictionary<string, PersonContent>();
        private readonly Dictionary<string, PersonEditState> edits = new Dictionary<string, PersonEditState>();
        private readonly Dictionary<string, Sprite> portraits = new Dictionary<string, Sprite>();

        public DocumentPlayState GetStateSnapshot() => DocumentContract.Copy(state);

        public void Initialize(DocumentAssignment content, DocumentPlayState playState, TMP_FontAsset font,
            IDictionary<string, Sprite> portraitAssets = null, DocumentDesktopSkin skin = null)
        {
            if (root != null) throw new InvalidOperationException("Desktop is already initialized.");
            if (font == null) throw new ArgumentNullException(nameof(font));
            DocumentContract.Validate(content);
            ValidateState(content, playState);
            assignment = DocumentContract.Copy(content);
            state = DocumentContract.Copy(playState);
            foreach (var person in assignment.People) people.Add(person.Id, person);
            foreach (var document in assignment.Documents) documents.Add(document.Id, document);
            foreach (var person in state.People) edits.Add(person.PersonId, person);
            if (people.Keys.Any(id => !edits.ContainsKey(id) || edits[id].Values == null))
                throw new ArgumentException("Play state is missing a person profile.");
            if (portraitAssets != null)
                foreach (var pair in portraitAssets) portraits.Add(pair.Key, pair.Value);
            ui = new DocumentDesktopUI(font, skin);
            Build();
        }

        // Validate the state shape consumed by this view. Record scoring and save migration belong to controllers.
        private static void ValidateState(DocumentAssignment content, DocumentPlayState value)
        {
            if (value == null || value.AssignmentId != content.Id || string.IsNullOrWhiteSpace(value.WorkInstanceId) ||
                !Enum.IsDefined(typeof(SubmissionPhase), value.Phase) || value.People == null ||
                value.Investigations == null || value.Views == null || value.Events == null)
                throw new ArgumentException("A matching, structurally valid work state is required.");
            var peopleIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var person in value.People)
                if (person == null || person.Values == null || string.IsNullOrWhiteSpace(person.PersonId) || !peopleIds.Add(person.PersonId))
                    throw new ArgumentException("Invalid or duplicate profile state.");
            if (!peopleIds.SetEquals(content.People.Select(p => p.Id)))
                throw new ArgumentException("Profile state must match the assignment people.");
            var investigationIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var investigation in value.Investigations)
                if (investigation == null || investigation.Records == null || investigation.MissingAnswerIds == null ||
                    string.IsNullOrWhiteSpace(investigation.PersonId) || !investigationIds.Add(investigation.PersonId))
                    throw new ArgumentException("Invalid or duplicate investigation state.");
            if (content.Mode == DocumentMode.StatementCheck ? !investigationIds.SetEquals(content.TargetPersonIds) : investigationIds.Count != 0)
                throw new ArgumentException("Investigation state must match the assignment targets and mode.");
        }

        private void Build()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            gameObject.AddComponent<GraphicRaycaster>();
            var backdrop = ui.Panel("Letterbox", transform, Color.black);
            DocumentDesktopUI.Fill(backdrop);
            root = ui.Panel("Desktop 5 by 3", backdrop, DocumentDesktopUI.Surface);
            root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
            root.sizeDelta = new Vector2(1600, 960);
            var workspace = ui.Panel("Workspace 4 by 3", root,
                assignment.Mode == DocumentMode.Forgery ? Color.black : DocumentDesktopUI.Surface);
            DocumentDesktopUI.Place(workspace, 0, 0, 1280, 960);
            area = ui.Rect("Windows", workspace);
            DocumentDesktopUI.Place(area, 0, 0, 1280, 844);
            area.gameObject.AddComponent<RectMask2D>();
            taskbar = ui.Taskbar(workspace);
            if (assignment.Mode == DocumentMode.Forgery)
            {
                Folder(workspace, DocumentKind.Profile, 20);
                var mark = ui.Text(workspace, "CONFIDENTIAL", 32);
                mark.color = Color.red;
                mark.alignment = TextAlignmentOptions.Right;
                DocumentDesktopUI.Place(mark.rectTransform, 800, 852, 460, 40);
            }
            else
            {
                Folder(workspace, DocumentKind.Statement, 20);
                Folder(workspace, DocumentKind.Action, 190);
            }
            // Desktop icons remain below windows; the taskbar and watermark are outside their bounds.
            area.SetAsLastSibling();
            var panel = ui.Frame("Instructions", root);
            DocumentDesktopUI.Place(panel, 1280, 0, 320, 960);
            var heading = ui.Text(panel, assignment.Mode == DocumentMode.Forgery ? "극비 업무" : "진술 대조", 30);
            DocumentDesktopUI.Place(heading.rectTransform, 18, 18, 284, 48);
            var content = ui.Scroll(panel, 12, 80, 296, 560);
            ui.Text(content, "날짜: " + assignment.TargetDateId);
            ui.Text(content, "대상: " + string.Join(", ", assignment.TargetPersonIds.Select(id => people[id].Profile.Name)));
            ui.Text(content, assignment.Instructions ?? "");
            foreach (var instruction in assignment.Edits)
                ui.Text(content, people[instruction.PersonId].Profile.Name + " — " + instruction.DisplayText);
            progress = ui.Text(panel, "");
            DocumentDesktopUI.Place(progress.rectTransform, 18, 660, 284, 128);
            record = ui.Button(panel, "기록하기", () => { if (CanRecord) RecordRequested?.Invoke(); });
            DocumentDesktopUI.Place((RectTransform)record.transform, 18, 800, 136, 48);
            cancel = ui.Button(panel, "취소하기", ClearSelection);
            DocumentDesktopUI.Place((RectTransform)cancel.transform, 166, 800, 136, 48);
            record.gameObject.SetActive(assignment.Mode == DocumentMode.StatementCheck);
            cancel.gameObject.SetActive(assignment.Mode == DocumentMode.StatementCheck);
            submit = ui.Button(panel, "ValidTask · 제출", () => { if (CanSubmit) SubmissionRequested?.Invoke(); });
            DocumentDesktopUI.Place((RectTransform)submit.transform, 18, 878, 284, 60);
            RefreshStatus();
            RefreshControls();
        }

        private void LateUpdate()
        {
            if (root == null) return;
            var parent = (RectTransform)root.parent;
            float scale = Mathf.Min(parent.rect.width / 1600f, parent.rect.height / 960f);
            root.localScale = Vector3.one * scale;
            RefreshControls();
        }

        private bool CanRecord => CanSelect && statementId != null && actionId != null && RecordRequested != null;
        private bool CanSubmit => CanSelect && submissionAllowed && SubmissionRequested != null &&
            (assignment.Mode == DocumentMode.Forgery || state.Investigations.All(i => i.Completed));

        private void RefreshControls()
        {
            ui.SetInteractable(record, CanRecord);
            ui.SetInteractable(cancel, CanSelect && (statementId != null || actionId != null));
            ui.SetInteractable(submit, CanSubmit);
        }

        private static string KindName(DocumentKind kind)
        {
            return kind == DocumentKind.Statement ? "진술 파일" : kind == DocumentKind.Action ? "실제 행동" : "인물";
        }

        private void Folder(Transform parent, DocumentKind kind, float x)
        {
            var button = ui.Button(parent, KindName(kind), () => OpenFolder(kind));
            ui.Icon(button, ui.WindowIcon, false);
            DocumentDesktopUI.Place((RectTransform)button.transform, x, 22, 154, 74);
        }

        private RectTransform NewWindow(string key, string title, float width, float height)
        {
            var frame = ui.Frame(title, area);
            int slot = windows.Count % 6;
            DocumentDesktopUI.Place(frame, 20 + slot * 90, 118 + slot * 35, width, height);
            var bar = ui.Panel("Title", frame, DocumentDesktopUI.Navy);
            DocumentDesktopUI.Place(bar, 4, 4, width - 8, 40);
            var label = ui.Text(bar, title, 21);
            label.color = Color.white;
            label.overflowMode = TextOverflowModes.Ellipsis;
            DocumentDesktopUI.Place(label.rectTransform, 8, 5, width - 112, 32);
            var window = frame.gameObject.AddComponent<DocumentDesktopWindow>();
            window.Initialize(area, bar.GetComponent<Image>(), focused =>
            {
                foreach (var item in windows.Values) item.SetFocused(item == focused);
            });
            bar.gameObject.AddComponent<DocumentWindowDrag>().Initialize(window, area);
            var min = ui.Button(bar, "−", () => frame.gameObject.SetActive(false));
            ui.Icon(min, ui.MinimizeSymbol, true);
            DocumentDesktopUI.Place((RectTransform)min.transform, width - 96, 4, 38, 32);
            var close = ui.Button(bar, "×", () => CloseWindow(key));
            ui.Icon(close, ui.CloseSymbol, true);
            DocumentDesktopUI.Place((RectTransform)close.transform, width - 52, 4, 36, 32);
            windows.Add(key, window);
            var task = ui.Button(taskbar, title, window.Focus);
            DocumentDesktopUI.Row(task, 34);
            task.GetComponent<LayoutElement>().preferredWidth = 230;
            task.GetComponent<LayoutElement>().minWidth = 230;
            task.GetComponentInChildren<TMP_Text>().overflowMode = TextOverflowModes.Ellipsis;
            tasks.Add(key, task);
            var body = ui.Scroll(frame, 8, 52, width - 16, height - 60);
            window.Move(Vector2.zero);
            window.Focus();
            return body;
        }

        public void OpenFolder(DocumentKind kind)
        {
            if ((assignment.Mode == DocumentMode.Forgery) != (kind == DocumentKind.Profile)) return;
            string key = "folder:" + kind;
            if (windows.TryGetValue(key, out var existing)) { existing.Focus(); return; }
            var body = NewWindow(key, KindName(kind), 470, 570);
            foreach (var document in assignment.Documents.Where(d => d.Kind == kind))
            {
                var button = ui.Button(body, people[document.PersonId].Profile.Name + "  /  " + document.DateId,
                    () => OpenDocument(document.Id));
                DocumentDesktopUI.Row(button, 70);
            }
        }

        public void OpenDocument(string documentId)
        {
            if (documentId == null || !documents.TryGetValue(documentId, out var document))
                throw new ArgumentException("Unknown document ID.");
            if ((assignment.Mode == DocumentMode.Forgery) != (document.Kind == DocumentKind.Profile)) return;
            string key = "document:" + document.Id;
            if (windows.TryGetValue(key, out var existing)) { existing.Focus(); return; }
            var body = NewWindow(key, people[document.PersonId].Profile.Name + " · " + KindName(document.Kind), 570, 620);
            RenderDocument(document, body);
            if (state.Phase != SubmissionPhase.Working) return;
            var view = new ViewRecord
            {
                Id = DocumentContract.Key(state.WorkInstanceId, "view", Guid.NewGuid().ToString("N")),
                DocumentId = document.Id,
                Authorized = assignment.TargetPersonIds.Contains(document.PersonId) && document.DateId == assignment.TargetDateId
            };
            state.Views.Add(view);
            DocumentOpened?.Invoke(DocumentContract.Copy(view));
        }

        private void RenderDocument(DocumentContent document, RectTransform body)
        {
            ui.Text(body, people[document.PersonId].Profile.Name + "\n" + document.DateId, 24);
            if (document.Kind == DocumentKind.Profile)
            {
                var person = people[document.PersonId];
                if (person.PortraitKey != null && portraits.TryGetValue(person.PortraitKey, out var sprite) && sprite != null)
                {
                    var portrait = ui.Panel("Portrait", body, Color.white).GetComponent<Image>();
                    portrait.sprite = sprite;
                    portrait.preserveAspect = true;
                    DocumentDesktopUI.Row(portrait, 150);
                }
                else ui.Text(body, "[사진 없음]");
                foreach (ProfileField field in Enum.GetValues(typeof(ProfileField))) ProfileInput(body, document, field);
                return;
            }
            foreach (var line in document.Lines)
            {
                var button = ui.Button(body, line.Time + "  " + line.Text, () =>
                {
                    windows["document:" + document.Id].Focus();
                    SelectLine(document, line.Id);
                });
                ui.SetInteractable(button, CanInvestigate(document.PersonId));
                var label = button.GetComponentInChildren<TMP_Text>();
                label.alignment = TextAlignmentOptions.TopLeft;
                float preferred = label.GetPreferredValues(label.text, 500, 0).y + 20;
                DocumentDesktopUI.Row(button, Mathf.Max(66, preferred));
                lineButtons[line.Id] = button;
            }
            RefreshSelection();
        }

        private void ProfileInput(Transform parent, DocumentContent document, ProfileField field)
        {
            ui.Text(parent, field.ToString(), 18);
            var rect = ui.InputPanel(field.ToString(), parent);
            DocumentDesktopUI.Row(rect, 68);
            var input = rect.gameObject.AddComponent<TMP_InputField>();
            var viewport = ui.Rect("Viewport", rect);
            DocumentDesktopUI.Fill(viewport, 8);
            viewport.gameObject.AddComponent<RectMask2D>();
            var label = ui.Text(viewport, "");
            DocumentDesktopUI.Fill(label.rectTransform);
            input.textViewport = viewport;
            input.textComponent = label;
            input.targetGraphic = rect.GetComponent<Image>();
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.text = GetField(edits[document.PersonId].Values, field);
            input.interactable = state.Phase == SubmissionPhase.Working && document.DateId == assignment.TargetDateId;
            input.onSelect.AddListener(_ => windows["document:" + document.Id].Focus());
            input.onValueChanged.AddListener(value =>
            {
                if (state.Phase != SubmissionPhase.Working) return;
                SetField(edits[document.PersonId].Values, field, value);
                ProfileChanged?.Invoke(document.PersonId, field, value);
            });
        }

        private static string GetField(ProfileValues value, ProfileField field)
        {
            switch (field)
            {
                case ProfileField.Name: return value.Name;
                case ProfileField.Age: return value.Age;
                case ProfileField.Sex: return value.Sex;
                case ProfileField.Country: return value.Country;
                default: return value.Job;
            }
        }

        private static void SetField(ProfileValues value, ProfileField field, string text)
        {
            switch (field)
            {
                case ProfileField.Name: value.Name = text; break;
                case ProfileField.Age: value.Age = text; break;
                case ProfileField.Sex: value.Sex = text; break;
                case ProfileField.Country: value.Country = text; break;
                case ProfileField.Job: value.Job = text; break;
            }
        }

        private bool CanSelect => state.Phase == SubmissionPhase.Working;
        private bool CanInvestigate(string personId) => CanSelect &&
            state.Investigations.Any(i => i.PersonId == personId && !i.Completed);

        private void SelectLine(DocumentContent document, string lineId)
        {
            if (!CanInvestigate(document.PersonId)) return;
            if (selectedPerson != document.PersonId || selectedDate != document.DateId) ClearSelection();
            selectedPerson = document.PersonId;
            selectedDate = document.DateId;
            if (document.Kind == DocumentKind.Statement) statementId = lineId;
            else actionId = lineId;
            RefreshSelection();
            PairSelectionChanged?.Invoke(statementId, actionId);
        }

        public void ClearSelection()
        {
            statementId = actionId = selectedPerson = selectedDate = null;
            RefreshSelection();
            PairSelectionChanged?.Invoke(null, null);
        }

        private void RefreshSelection()
        {
            foreach (var pair in lineButtons)
                if (pair.Value != null)
                    ui.SetSelected(pair.Value, pair.Key == statementId || pair.Key == actionId);
        }

        private void CloseWindow(string key)
        {
            if (!windows.TryGetValue(key, out var window)) return;
            // Removing the view never removes profile edits, records or the investigation state.
            window.gameObject.SetActive(false);
            Destroy(window.gameObject);
            tasks[key].gameObject.SetActive(false);
            Destroy(tasks[key].gameObject);
            windows.Remove(key);
            tasks.Remove(key);
            if (key.StartsWith("document:", StringComparison.Ordinal) && documents.TryGetValue(key.Substring(9), out var document))
                foreach (var line in document.Lines) lineButtons.Remove(line.Id);
        }

        /// <summary>Controllers publish evaluated investigation state; the desktop does not judge selected pairs.</summary>
        public void UpdateInvestigation(InvestigationState investigation)
        {
            if (!CanSelect) throw new InvalidOperationException("Submission is locked.");
            if (investigation == null || investigation.Records == null || investigation.MissingAnswerIds == null)
                throw new ArgumentException("Investigation and record collections are required.");
            int index = state.Investigations.FindIndex(i => i.PersonId == investigation.PersonId);
            if (index < 0) throw new ArgumentException("Unknown investigation target.");
            if (state.Investigations[index].Completed) throw new InvalidOperationException("Investigation is locked.");
            state.Investigations[index] = DocumentContract.Copy(investigation);
            if (investigation.Completed && selectedPerson == investigation.PersonId) ClearSelection();
            foreach (var document in assignment.Documents.Where(d => d.PersonId == investigation.PersonId))
                foreach (var line in document.Lines)
                    if (lineButtons.TryGetValue(line.Id, out var button) && button != null)
                        ui.SetInteractable(button, CanInvestigate(document.PersonId));
            RefreshStatus();
        }

        /// <summary>Additional controller gate; true never bypasses the mode's investigation or submission locks.</summary>
        public void SetSubmissionAllowed(bool allowed) { submissionAllowed = allowed; }

        public void LockSubmission()
        {
            if (!CanSelect) return;
            state.Phase = SubmissionPhase.Submitted;
            ClearSelection();
            foreach (var input in GetComponentsInChildren<TMP_InputField>(true)) input.interactable = false;
            foreach (var button in lineButtons.Values) if (button != null) ui.SetInteractable(button, false);
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            progress.text = state.Phase != SubmissionPhase.Working ? "제출 완료 · 열람 전용" :
                assignment.Mode == DocumentMode.StatementCheck
                    ? "조사 완료 " + state.Investigations.Count(i => i.Completed) + " / " + state.Investigations.Count +
                      "\n기록 " + state.Investigations.Sum(i => i.Records.Count) + "건"
                    : "문서를 닫아도 수정 내용은 유지됩니다.";
        }
    }
}
