using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.Documents
{
    /// <summary>문서 창과 입력을 표시한다. 판정, 완료 및 저장은 외부에서 처리한다.</summary>
    public sealed class DocumentDesktop : MonoBehaviour
    {
        public event Action<ViewRecord> DocumentOpened;
        public event Action<string, string> PairSelectionChanged;
        public event Action<string, ProfileField, string> ProfileChanged;
        public event Action RecordRequested;
        public event Action<string> InvestigationCompletionRequested;
        public event Action SubmissionRequested;
        private DocumentAssignment _assignment;
        private DocumentPlayState _playState;
        private DocumentDesktopUI _desktopUi;
        private RectTransform _desktopRoot;
        private RectTransform _windowArea;
        private RectTransform _taskbar;
        private Vector2 _lastParentSize;
        private TMP_Text _progressText;
        private TMP_Text _recordHistoryText;
        private string _statusMessage;
        private Button _recordButton;
        private Button _cancelButton;
        private Button _submitButton;
        private string _selectedStatementLineId;
        private string _selectedActionLineId;
        private string _selectedPersonId;
        private string _selectedDate;
        private DocumentContentIndex _contentIndex;
        private bool _submissionAllowed = true;
        private readonly Dictionary<string, DocumentDesktopWindow> _windowsByKey = new Dictionary<string, DocumentDesktopWindow>();
        private readonly Dictionary<string, Button> _taskbarButtonsByKey = new Dictionary<string, Button>();
        private readonly Dictionary<string, Button> _lineButtonsById = new Dictionary<string, Button>();
        private readonly Dictionary<string, Button> _investigationButtonsByPersonId = new Dictionary<string, Button>();
        private readonly Dictionary<string, PersonEditState> _editsByPersonId = new Dictionary<string, PersonEditState>();
        private readonly Dictionary<string, Sprite> _portraitsByKey = new Dictionary<string, Sprite>();

        public DocumentPlayState GetStateSnapshot() => DocumentContract.CopyState(_playState);

        public void Initialize(DocumentAssignment content, DocumentPlayState playState, TMP_FontAsset font,
            IDictionary<string, Sprite> portraitAssets = null, DocumentDesktopSkin skin = null)
        {
            if (_desktopRoot != null) throw new InvalidOperationException("Desktop is already initialized.");
            if (font == null) throw new ArgumentNullException(nameof(font));
            DocumentContract.Validate(content);
            ValidateState(content, playState);
            _assignment = DocumentContract.Copy(content);
            _playState = DocumentContract.CopyState(playState);
            _contentIndex = new DocumentContentIndex(_assignment);
            foreach (var person in _playState.People) _editsByPersonId.Add(person.PersonId, person);
            if (portraitAssets != null)
                foreach (var pair in portraitAssets) _portraitsByKey.Add(pair.Key, pair.Value);
            _desktopUi = new DocumentDesktopUI(font, skin);
            Build();
        }

        // 화면이 사용하는 상태 구조만 검사하며 기록 판정과 저장 변환은 처리하지 않는다.
        private static void ValidateState(DocumentAssignment content, DocumentPlayState value)
        {
            if (value == null || value.AssignmentId != content.Id || string.IsNullOrWhiteSpace(value.WorkInstanceId) ||
                !Enum.IsDefined(typeof(SubmissionPhase), value.Phase) || value.People == null ||
                value.Investigations == null || value.Views == null)
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

        // 작업 영역과 안내 패널을 만들고 초기 상태를 표시한다.
        private void Build()
        {
            CreateDesktopRoot();
            CreateWorkspace();
            CreateInstructionPanel();
            RefreshStatus();
            RefreshRecordHistory();
            RefreshControls();
        }

        // 화면 비율을 유지하는 문서 바탕 화면을 생성한다.
        private void CreateDesktopRoot()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            gameObject.AddComponent<GraphicRaycaster>();
            var backdrop = _desktopUi.Panel("Letterbox", transform, Color.black);
            DocumentDesktopUI.Fill(backdrop);
            _desktopRoot = _desktopUi.Panel("Desktop 5 by 3", backdrop, DocumentDesktopUI.Surface);
            _desktopRoot.anchorMin = _desktopRoot.anchorMax = _desktopRoot.pivot = new Vector2(.5f, .5f);
            _desktopRoot.sizeDelta = new Vector2(1600, 960);
        }

        // 문서 아이콘과 창 영역, 작업표시줄을 배치한다.
        private void CreateWorkspace()
        {
            var workspace = _desktopUi.Panel("Workspace 4 by 3", _desktopRoot,
                _assignment.Mode == DocumentMode.Forgery ? Color.black : DocumentDesktopUI.Surface);
            DocumentDesktopUI.Place(workspace, 0, 0, 1280, 960);
            _windowArea = _desktopUi.Rect("Windows", workspace);
            DocumentDesktopUI.Place(_windowArea, 0, 0, 1280, 844);
            _windowArea.gameObject.AddComponent<RectMask2D>();
            _taskbar = _desktopUi.Taskbar(workspace);
            if (_assignment.Mode == DocumentMode.Forgery)
            {
                CreateFolderShortcut(workspace, DocumentKind.Profile, 20);
                var mark = _desktopUi.Text(workspace, "CONFIDENTIAL", 32);
                mark.color = Color.red;
                mark.alignment = TextAlignmentOptions.Right;
                DocumentDesktopUI.Place(mark.rectTransform, 800, 852, 460, 40);
            }
            else
            {
                CreateFolderShortcut(workspace, DocumentKind.Statement, 20);
                CreateFolderShortcut(workspace, DocumentKind.Action, 190);
            }
            // 창은 바탕 화면 아이콘보다 위에 두고 작업표시줄 영역은 침범하지 않는다.
            _windowArea.SetAsLastSibling();
        }

        // 업무 안내와 조사 및 제출 버튼을 생성한다.
        private void CreateInstructionPanel()
        {
            var panel = _desktopUi.Frame("Instructions", _desktopRoot);
            DocumentDesktopUI.Place(panel, 1280, 0, 320, 960);
            var heading = _desktopUi.Text(panel, _assignment.Mode == DocumentMode.Forgery ? "극비 업무" : "진술 대조", 30);
            DocumentDesktopUI.Place(heading.rectTransform, 18, 18, 284, 48);
            var content = _desktopUi.Scroll(panel, 12, 80, 296, _assignment.Mode == DocumentMode.StatementCheck ? 400 : 560);
            if (_assignment.Mode == DocumentMode.StatementCheck && _assignment.Stage == 0)
                _desktopUi.Text(content, "바탕화면의 '진술 파일'과 '실제 행동' 아이콘을 클릭해 문서를 여세요.");
            _desktopUi.Text(content, "날짜: " + _assignment.TargetDateId);
            _desktopUi.Text(content, "대상: " + string.Join(", ", _assignment.TargetPersonIds.Select(id => _contentIndex.PeopleById[id].Profile.Name)));
            _desktopUi.Text(content, _assignment.Instructions ?? "");
            foreach (var instruction in _assignment.Edits)
                _desktopUi.Text(content, _contentIndex.PeopleById[instruction.PersonId].Profile.Name + " — " + instruction.DisplayText);
            if (_assignment.Mode == DocumentMode.StatementCheck)
                foreach (var personId in _assignment.TargetPersonIds)
                {
                    string targetId = personId;
                    var complete = _desktopUi.Button(content, _contentIndex.PeopleById[targetId].Profile.Name + " 조사완료", () =>
                    {
                        if (CanCompleteInvestigation(targetId)) InvestigationCompletionRequested?.Invoke(targetId);
                    });
                    DocumentDesktopUI.Row(complete, 52);
                    _investigationButtonsByPersonId.Add(targetId, complete);
                }
            if (_assignment.Mode == DocumentMode.StatementCheck)
            {
                var history = _desktopUi.Scroll(panel, 12, 492, 296, 150);
                _recordHistoryText = _desktopUi.Text(history, "");
            }
            _progressText = _desktopUi.Text(panel, "");
            DocumentDesktopUI.Place(_progressText.rectTransform, 18, 660, 284, 128);
            _recordButton = _desktopUi.Button(panel, "기록하기", () => { if (CanRecord) RecordRequested?.Invoke(); });
            DocumentDesktopUI.Place((RectTransform)_recordButton.transform, 18, 800, 136, 48);
            _cancelButton = _desktopUi.Button(panel, "취소하기", ClearSelection);
            DocumentDesktopUI.Place((RectTransform)_cancelButton.transform, 166, 800, 136, 48);
            _recordButton.gameObject.SetActive(_assignment.Mode == DocumentMode.StatementCheck);
            _cancelButton.gameObject.SetActive(_assignment.Mode == DocumentMode.StatementCheck);
            _submitButton = _desktopUi.Button(panel, "ValidTask · 제출", () => { if (CanSubmit) SubmissionRequested?.Invoke(); });
            DocumentDesktopUI.Place((RectTransform)_submitButton.transform, 18, 878, 284, 60);
        }

        // 부모 영역 크기가 바뀌면 바탕 화면의 배율을 갱신한다.
        private void LateUpdate()
        {
            if (_desktopRoot == null) return;
            var parent = (RectTransform)_desktopRoot.parent;
            if (parent.rect.size == _lastParentSize) return;
            _lastParentSize = parent.rect.size;
            float scale = Mathf.Min(parent.rect.width / 1600f, parent.rect.height / 960f);
            _desktopRoot.localScale = Vector3.one * scale;
        }

        private bool CanRecord => CanSelect && _selectedStatementLineId != null && _selectedActionLineId != null && RecordRequested != null;
        private bool CanSubmit => CanSelect && _submissionAllowed && SubmissionRequested != null &&
            (_assignment.Mode == DocumentMode.Forgery || _playState.Investigations.All(i => i.Completed));

        public void RefreshControls()
        {
            _desktopUi.SetInteractable(_recordButton, CanRecord);
            _desktopUi.SetInteractable(_cancelButton, CanSelect && (_selectedStatementLineId != null || _selectedActionLineId != null));
            _desktopUi.SetInteractable(_submitButton, CanSubmit);
            foreach (var pair in _investigationButtonsByPersonId)
                _desktopUi.SetInteractable(pair.Value, CanCompleteInvestigation(pair.Key));
        }

        // 문서 종류에 대응하는 폴더 표시 이름을 반환한다.
        private static string GetKindName(DocumentKind kind)
        {
            return kind == DocumentKind.Statement ? "진술 파일" : kind == DocumentKind.Action ? "실제 행동" : "인물";
        }

        // 문서 종류별 폴더를 여는 바로가기를 만든다.
        private void CreateFolderShortcut(Transform parent, DocumentKind kind, float x)
        {
            var button = _desktopUi.Button(parent, GetKindName(kind), () => OpenFolder(kind));
            _desktopUi.Icon(button, _desktopUi.WindowIcon, false);
            DocumentDesktopUI.Place((RectTransform)button.transform, x, 22, 154, 74);
        }

        // 제목 표시줄과 작업표시줄 버튼을 포함한 창을 생성한다.
        private RectTransform CreateWindow(string key, string title, float width, float height, Action onClose = null)
        {
            var frame = _desktopUi.Frame(title, _windowArea);
            int slot = _windowsByKey.Count % 6;
            DocumentDesktopUI.Place(frame, 20 + slot * 90, 118 + slot * 35, width, height);
            var bar = _desktopUi.Panel("Title", frame, DocumentDesktopUI.Navy);
            DocumentDesktopUI.Place(bar, 4, 4, width - 8, 40);
            var label = _desktopUi.Text(bar, title, 21);
            label.color = Color.white;
            label.overflowMode = TextOverflowModes.Ellipsis;
            DocumentDesktopUI.Place(label.rectTransform, 8, 5, width - 112, 32);
            var window = frame.gameObject.AddComponent<DocumentDesktopWindow>();
            window.Initialize(_windowArea, bar.GetComponent<Image>(), focused =>
            {
                foreach (var item in _windowsByKey.Values) item.SetFocused(item == focused);
            });
            bar.gameObject.AddComponent<DocumentWindowDrag>().Initialize(window, _windowArea);
            var min = _desktopUi.Button(bar, "−", () => frame.gameObject.SetActive(false));
            _desktopUi.Icon(min, _desktopUi.MinimizeSymbol, true);
            DocumentDesktopUI.Place((RectTransform)min.transform, width - 96, 4, 38, 32);
            var close = _desktopUi.Button(bar, "×", onClose ?? (() => CloseWindow(key)));
            _desktopUi.Icon(close, _desktopUi.CloseSymbol, true);
            DocumentDesktopUI.Place((RectTransform)close.transform, width - 52, 4, 36, 32);
            _windowsByKey.Add(key, window);
            var task = _desktopUi.Button(_taskbar, title, window.Focus);
            DocumentDesktopUI.Row(task, 34);
            task.GetComponent<LayoutElement>().preferredWidth = 230;
            task.GetComponent<LayoutElement>().minWidth = 230;
            task.GetComponentInChildren<TMP_Text>().overflowMode = TextOverflowModes.Ellipsis;
            _taskbarButtonsByKey.Add(key, task);
            var body = _desktopUi.Scroll(frame, 8, 52, width - 16, height - 60);
            window.Move(Vector2.zero);
            window.Focus();
            return body;
        }

        public void OpenFolder(DocumentKind kind)
        {
            if ((_assignment.Mode == DocumentMode.Forgery) != (kind == DocumentKind.Profile)) return;
            string key = "folder:" + kind;
            if (_windowsByKey.TryGetValue(key, out var existing)) { existing.Focus(); return; }
            var body = CreateWindow(key, GetKindName(kind), 470, 570);
            foreach (var document in _assignment.Documents.Where(d => d.Kind == kind))
            {
                var button = _desktopUi.Button(body, _contentIndex.PeopleById[document.PersonId].Profile.Name + "  /  " + document.DateId,
                    () => OpenDocument(document.Id));
                DocumentDesktopUI.Row(button, 70);
            }
        }

        public void OpenDocument(string documentId)
        {
            if (documentId == null || !_contentIndex.DocumentsById.TryGetValue(documentId, out var document))
                throw new ArgumentException("Unknown document ID.");
            if ((_assignment.Mode == DocumentMode.Forgery) != (document.Kind == DocumentKind.Profile)) return;
            string key = "document:" + document.Id;
            if (_windowsByKey.TryGetValue(key, out var existing)) { existing.Focus(); return; }
            var body = CreateWindow(key, _contentIndex.PeopleById[document.PersonId].Profile.Name + " · " + GetKindName(document.Kind), 570, 620);
            RenderDocument(document, body);
            if (_playState.Phase != SubmissionPhase.Working) return;
            var view = new ViewRecord
            {
                Id = DocumentContract.Key(_playState.WorkInstanceId, "view", Guid.NewGuid().ToString("N")),
                DocumentId = document.Id,
                Authorized = _assignment.TargetPersonIds.Contains(document.PersonId) && document.DateId == _assignment.TargetDateId
            };
            _playState.Views.Add(view);
            DocumentOpened?.Invoke(DocumentContract.Copy(view));
        }

        // 문서 종류에 맞는 프로필 또는 기록 줄을 표시한다.
        private void RenderDocument(DocumentContent document, RectTransform body)
        {
            _desktopUi.Text(body, _contentIndex.PeopleById[document.PersonId].Profile.Name + "\n" + document.DateId, 24);
            if (document.Kind == DocumentKind.Profile)
            {
                var person = _contentIndex.PeopleById[document.PersonId];
                if (person.PortraitKey != null && _portraitsByKey.TryGetValue(person.PortraitKey, out var sprite) && sprite != null)
                {
                    var portrait = _desktopUi.Panel("Portrait", body, Color.white).GetComponent<Image>();
                    portrait.sprite = sprite;
                    portrait.preserveAspect = true;
                    DocumentDesktopUI.Row(portrait, 150);
                }
                else _desktopUi.Text(body, "[사진 없음]");
                foreach (ProfileField field in Enum.GetValues(typeof(ProfileField)))
                    CreateProfileInput(body, document, field);
                return;
            }
            foreach (var line in document.Lines)
            {
                var button = _desktopUi.Button(body, line.Time + "  " + line.Text, () =>
                {
                    _windowsByKey["document:" + document.Id].Focus();
                    SelectLine(document, line.Id);
                });
                _desktopUi.SetInteractable(button, CanInvestigate(document.PersonId));
                var label = button.GetComponentInChildren<TMP_Text>();
                label.alignment = TextAlignmentOptions.TopLeft;
                float preferred = label.GetPreferredValues(label.text, 500, 0).y + 20;
                DocumentDesktopUI.Row(button, Mathf.Max(66, preferred));
                _lineButtonsById[line.Id] = button;
            }
            RefreshSelection();
        }

        // 프로필 항목의 입력값과 변경 이벤트를 연결한다.
        private void CreateProfileInput(Transform parent, DocumentContent document, ProfileField field)
        {
            _desktopUi.Text(parent, field.ToString(), 18);
            var rect = _desktopUi.InputPanel(field.ToString(), parent);
            DocumentDesktopUI.Row(rect, 68);
            var input = rect.gameObject.AddComponent<TMP_InputField>();
            var viewport = _desktopUi.Rect("Viewport", rect);
            DocumentDesktopUI.Fill(viewport, 8);
            viewport.gameObject.AddComponent<RectMask2D>();
            var label = _desktopUi.Text(viewport, "");
            DocumentDesktopUI.Fill(label.rectTransform);
            input.textViewport = viewport;
            input.textComponent = label;
            input.targetGraphic = rect.GetComponent<Image>();
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.text = _editsByPersonId[document.PersonId].Values.Get(field);
            input.interactable = _playState.Phase == SubmissionPhase.Working && document.DateId == _assignment.TargetDateId;
            input.onSelect.AddListener(_ => _windowsByKey["document:" + document.Id].Focus());
            input.onValueChanged.AddListener(value =>
            {
                if (_playState.Phase != SubmissionPhase.Working) return;
                _editsByPersonId[document.PersonId].Values.Set(field, value);
                ProfileChanged?.Invoke(document.PersonId, field, value);
            });
            input.onEndEdit.AddListener(value =>
            {
                if (_playState.Phase != SubmissionPhase.Working) return;
                _editsByPersonId[document.PersonId].Values.Set(field, value);
            });
        }

        private bool CanSelect => _playState.Phase == SubmissionPhase.Working;
        // 해당 인물의 조사가 진행 중인지 확인한다.
        private bool CanInvestigate(string personId) => CanSelect &&
            _playState.Investigations.Any(i => i.PersonId == personId && !i.Completed);
        // 기록이 있는 미완료 조사인지 확인한다.
        private bool CanCompleteInvestigation(string personId)
        {
            var investigation = _playState.Investigations.FirstOrDefault(i => i.PersonId == personId);
            return CanSelect && investigation != null && !investigation.Completed &&
                investigation.Records.Count > 0 && InvestigationCompletionRequested != null;
        }

        // 같은 인물과 날짜의 진술 및 행동 선택을 갱신한다.
        private void SelectLine(DocumentContent document, string lineId)
        {
            if (!CanInvestigate(document.PersonId)) return;
            if (_selectedPersonId != document.PersonId || _selectedDate != document.DateId) ClearSelection();
            _selectedPersonId = document.PersonId;
            _selectedDate = document.DateId;
            if (document.Kind == DocumentKind.Statement) _selectedStatementLineId = lineId;
            else _selectedActionLineId = lineId;
            RefreshSelection();
            RefreshControls();
            PairSelectionChanged?.Invoke(_selectedStatementLineId, _selectedActionLineId);
        }

        public void ClearSelection()
        {
            _selectedStatementLineId = _selectedActionLineId = _selectedPersonId = _selectedDate = null;
            RefreshSelection();
            RefreshControls();
            PairSelectionChanged?.Invoke(null, null);
        }

        // 선택된 문서 줄의 강조 표시를 갱신한다.
        private void RefreshSelection()
        {
            foreach (var pair in _lineButtonsById)
                if (pair.Value != null)
                    _desktopUi.SetSelected(pair.Value, pair.Key == _selectedStatementLineId || pair.Key == _selectedActionLineId);
        }

        // 입력 조합을 확정한 뒤 창과 작업표시줄 버튼을 해제한다.
        private void CloseWindow(string key)
        {
            if (!_windowsByKey.TryGetValue(key, out var window)) return;
            // 입력창을 제거하기 전에 진행 중인 한글 조합을 확정한다.
            foreach (var input in window.GetComponentsInChildren<TMP_InputField>(true))
                if (input.isFocused) input.DeactivateInputField();
            // 창만 제거하고 수정값과 조사 상태는 유지한다.
            window.gameObject.SetActive(false);
            Destroy(window.gameObject);
            _taskbarButtonsByKey[key].gameObject.SetActive(false);
            Destroy(_taskbarButtonsByKey[key].gameObject);
            _windowsByKey.Remove(key);
            _taskbarButtonsByKey.Remove(key);
            if (key.StartsWith("document:", StringComparison.Ordinal) && _contentIndex.DocumentsById.TryGetValue(key.Substring(9), out var document))
                foreach (var line in document.Lines) _lineButtonsById.Remove(line.Id);
        }

        /// <summary>외부에서 판정한 조사 상태를 화면에 반영한다.</summary>
        public void UpdateInvestigation(InvestigationState investigation)
        {
            if (!CanSelect) throw new InvalidOperationException("Submission is locked.");
            if (investigation == null || investigation.Records == null || investigation.MissingAnswerIds == null)
                throw new ArgumentException("Investigation and record collections are required.");
            int index = _playState.Investigations.FindIndex(i => i.PersonId == investigation.PersonId);
            if (index < 0) throw new ArgumentException("Unknown investigation target.");
            if (_playState.Investigations[index].Completed) throw new InvalidOperationException("Investigation is locked.");
            _playState.Investigations[index] = DocumentContract.Copy(investigation);
            if (investigation.Completed && _selectedPersonId == investigation.PersonId) ClearSelection();
            foreach (var document in _assignment.Documents.Where(d => d.PersonId == investigation.PersonId))
                foreach (var line in document.Lines)
                    if (_lineButtonsById.TryGetValue(line.Id, out var button) && button != null)
                        _desktopUi.SetInteractable(button, CanInvestigate(document.PersonId));
            RefreshStatus();
            RefreshRecordHistory();
            RefreshControls();
        }

        /// <summary>판정 상태를 바꾸지 않고 안내 문구만 표시한다.</summary>
        public void ShowStatusMessage(string message)
        {
            _statusMessage = message;
            RefreshStatus();
        }

        /// <summary>제출 잠금 이후 읽기 전용 결과 창을 연다.</summary>
        public void ShowSubmissionResult(string title, IEnumerable<string> lines, Action onConfirm = null)
        {
            const string key = "submission:result";
            if (_windowsByKey.TryGetValue(key, out var existing))
            {
                existing.Focus();
                return;
            }
            bool confirmed = false;
            Action confirm = () =>
            {
                if (confirmed) return;
                confirmed = true;
                onConfirm?.Invoke();
            };
            var body = CreateWindow(key, title, 760, 650, onConfirm == null ? null : confirm);
            foreach (string line in lines) _desktopUi.Text(body, line, 21);
            if (onConfirm != null)
            {
                var confirmButton = _desktopUi.Button(body, "확인 · 업무 종료", confirm);
                DocumentDesktopUI.Row(confirmButton, 58);
            }
        }

        /// <summary>모드별 제출 조건을 유지하면서 외부 제출 허용 상태를 반영한다.</summary>
        public void SetSubmissionAllowed(bool allowed)
        {
            _submissionAllowed = allowed;
            RefreshControls();
        }

        public void LockSubmission()
        {
            if (!CanSelect) return;
            _playState.Phase = SubmissionPhase.Submitted;
            _statusMessage = null;
            ClearSelection();
            foreach (var input in GetComponentsInChildren<TMP_InputField>(true)) input.interactable = false;
            foreach (var button in _lineButtonsById.Values) if (button != null) _desktopUi.SetInteractable(button, false);
            foreach (var button in _investigationButtonsByPersonId.Values) if (button != null) _desktopUi.SetInteractable(button, false);
            RefreshStatus();
        }

        // 업무 진행 상황과 안내 메시지를 표시한다.
        private void RefreshStatus()
        {
            string summary = _playState.Phase != SubmissionPhase.Working ? "제출 완료 · 열람 전용" :
                _assignment.Mode == DocumentMode.StatementCheck
                    ? "조사 완료 " + _playState.Investigations.Count(i => i.Completed) + " / " + _playState.Investigations.Count +
                      "\n기록 " + _playState.Investigations.Sum(i => i.Records.Count) + "건"
                    : "문서를 닫아도 수정 내용은 유지됩니다.";
            _progressText.text = string.IsNullOrEmpty(_statusMessage) ? summary : summary + "\n" + _statusMessage;
        }

        // 저장된 진술 및 행동 조합을 기록 이력에 표시한다.
        private void RefreshRecordHistory()
        {
            if (_recordHistoryText == null) return;
            var lines = _contentIndex.LinesById;
            var entries = new List<string> { "기록 이력" };
            foreach (InvestigationState investigation in _playState.Investigations)
            foreach (PairRecord pair in investigation.Records)
                entries.Add(_contentIndex.PeopleById[investigation.PersonId].Profile.Name + "\n진술: " +
                    FormatLine(lines[pair.StatementLineId]) + "\n행동: " + FormatLine(lines[pair.ActionLineId]));
            if (entries.Count == 1) entries.Add("기록 없음");
            _recordHistoryText.text = string.Join("\n\n", entries);
        }

        // 기록 줄의 시간과 내용을 하나의 문장으로 묶는다.
        private static string FormatLine(DocumentLine line)
        {
            return line.Time + " " + line.Text;
        }
    }
}
