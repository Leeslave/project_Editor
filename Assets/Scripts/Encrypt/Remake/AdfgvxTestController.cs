using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorGame.AdfgvxRemake
{
    /// <summary>Standalone, non-destructive ADFGVX remake. Reads the existing Resources data.</summary>
    public sealed partial class AdfgvxTestController : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private int initialStage = 6;
        [SerializeField] private CipherDirection initialDirection = CipherDirection.Decrypt;
        private readonly List<CipherTask> tasks = new List<CipherTask>();
        private readonly Dictionary<string, CipherSession> sessions = new Dictionary<string, CipherSession>();
        private readonly Dictionary<CipherDirection, int> selectedStages = new Dictionary<CipherDirection, int>();
        private CipherSession session;
        private RectTransform screen, canvasRect;
        private GameObject page;
        private TMP_InputField keyInput;
        private bool guidance = true;
        private int hoveredCell = -1;
        private string status = "업무 문서를 불러오면 시작합니다. 과제와 모드를 바꿔도 이 실행 중의 작업은 유지됩니다.";
        private bool statusError;
        private bool showReset;
        private bool hosted;
        private string hostedTitle;
        private Action<CipherTask> completedCallback;
        private Action returnCallback;
        private TMP_FontAsset runtimeFont;
        private readonly Color green = new Color32(45, 255, 75, 255);
        private readonly Color cyan = new Color32(51, 255, 255, 255);
        private readonly Color amber = new Color32(255, 199, 66, 255);
        private readonly Color red = new Color32(255, 103, 103, 255);
        private readonly Color ink = new Color32(13, 17, 17, 255);
        private readonly Color muted = new Color32(139, 158, 151, 255);

        /// <summary>sceneLoaded에서 Start 이전에 지정한다. 단독 TEST 실행은 기존 동작을 유지한다.</summary>
        public void ConfigureWork(int stage, string title, Action<CipherTask> completed, Action returnToList, bool startTutorial = false)
        {
            if (screen != null) throw new InvalidOperationException("ConfigureWork must run before Start.");
            hosted = true;
            initialStage = stage;
            initialDirection = CipherDirection.Decrypt;
            hostedTitle = title;
            completedCallback = completed;
            returnCallback = returnToList;
            autoStartTutorial = startTutorial;
        }

        private void Start()
        {
            EnsureCanvas();
            try
            {
                LoadTasks();
                CipherTask first = tasks.FirstOrDefault(t => t.Direction == initialDirection && t.Stage == initialStage)
                    ?? (hosted ? throw new InvalidOperationException("지정한 해독 과제가 없습니다: " + initialStage) : tasks.First());
                Select(first);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Label(screen, "ADFGVX TEST / 데이터 오류", 30, 40, 1380, 60, 28, red);
                Label(screen, exception.Message, 30, 115, 1380, 600, 22, Color.white);
            }
        }

        private void LoadTasks()
        {
            TextAsset json = Resources.Load<TextAsset>("GameData/Encrypt/ADFGVXStageData");
            if (json == null) throw new InvalidOperationException("기존 ADFGVXStageData.json을 찾을 수 없습니다.");
            var data = JsonConvert.DeserializeObject<ADFGVXStageData>(json.text);
            if (data == null) throw new InvalidOperationException("암호 과제 데이터가 비어 있습니다.");
            AddTasks(data.Decrypt, CipherDirection.Decrypt);
            AddTasks(data.Encrypt, CipherDirection.Encrypt);
            if (tasks.Count == 0) throw new InvalidOperationException("실행할 암호 과제가 없습니다.");
        }

        private void AddTasks(Dictionary<string, Dictionary<string, string>> records, CipherDirection direction)
        {
            if (records == null) return;
            string prefix = direction == CipherDirection.Encrypt ? "encrypt" : "decrypt";
            foreach (var record in records.OrderBy(pair => int.Parse(pair.Key)))
            {
                var values = record.Value;
                int tableNumber = int.Parse(values[prefix + "Table"]);
                TextAsset table = Resources.Load<TextAsset>("GameData/Encrypt/Tables/Table_" + tableNumber);
                if (table == null) throw new InvalidOperationException("치환표가 없습니다: " + tableNumber);
                var task = new CipherTask
                {
                    Stage = int.Parse(record.Key), Direction = direction,
                    Title = values.ContainsKey("title") ? values["title"] : "ENCRYPT-STAGE-" + record.Key,
                    Input = values["targetText"], Key = values[prefix + "Key"],
                    TableNumber = tableNumber, Table = Compact(table.text),
                    Intermediate = values[prefix + "TransposeText"], Result = values["resultText"]
                };
                task.Validate();
                tasks.Add(task);
            }
        }

        private static string Compact(string value) => new string(value.Where(c => !char.IsWhiteSpace(c) && c != '\uFEFF').ToArray());

        private void EnsureCanvas()
        {
            if (font == null) font = TMP_Settings.defaultFontAsset;
            if (font == null) throw new InvalidOperationException("ADFGVX_TEST의 Font 참조를 지정하세요.");
            // The shared atlas may already be full. Generate a runtime atlas without changing that asset.
            if (font.sourceFontFile != null)
            {
                runtimeFont = TMP_FontAsset.CreateFontAsset(font.sourceFontFile);
                runtimeFont.isMultiAtlasTexturesEnabled = true;
                font = runtimeFont;
            }
            var canvas = new GameObject("ADFGVX Test Canvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.transform.SetParent(transform, false);
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.GetComponent<Canvas>().sortingOrder = 100;
            canvas.GetComponent<Canvas>().pixelPerfect = true;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = .5f;
            canvasRect = canvas.GetComponent<RectTransform>();
            var background = new GameObject("Letterbox", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(canvas.transform, false);
            var back = background.GetComponent<RectTransform>();
            back.anchorMin = Vector2.zero; back.anchorMax = Vector2.one;
            back.offsetMin = back.offsetMax = Vector2.zero;
            background.GetComponent<Image>().color = new Color32(6, 9, 8, 255);
            background.GetComponent<Image>().raycastTarget = false;
            screen = new GameObject("4 by 3 terminal", typeof(RectTransform)).GetComponent<RectTransform>();
            screen.SetParent(canvas.transform, false);
            screen.anchorMin = screen.anchorMax = screen.pivot = new Vector2(.5f, .5f);
            screen.sizeDelta = new Vector2(1440, 1080);
            if (EventSystem.current == null)
            {
                var events = new GameObject("ADFGVX Test EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                events.transform.SetParent(transform, false);
            }
        }

        private void LateUpdate()
        {
            if (canvasRect == null || screen == null) return;
            float scale = Mathf.Min(canvasRect.rect.width / 1440f, canvasRect.rect.height / 1080f);
            screen.localScale = new Vector3(scale, scale, 1);
        }

        private void Update()
        {
            if (session == null || (keyInput != null && keyInput.isFocused) || showReset || LessonActive) return;
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (control && Input.GetKeyDown(KeyCode.Z)) { Undo(); return; }
            if (control && Input.GetKeyDown(KeyCode.Y)) { Redo(); return; }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(1);
        }

        private void OnDestroy()
        {
            if (runtimeFont == null) return;
            foreach (Texture2D texture in runtimeFont.atlasTextures)
                if (texture != null) Destroy(texture);
            if (runtimeFont.material != null) Destroy(runtimeFont.material);
            Destroy(runtimeFont);
        }

        private void SaveDraft()
        {
            if (session != null && keyInput != null) session.KeyDraft = keyInput.text;
        }

        private void Select(CipherTask task)
        {
            SaveDraft();
            keyInput = null;
            if (!sessions.TryGetValue(task.Id, out session))
            { session = new CipherSession(task); sessions.Add(task.Id, session); }
            selectedStages[task.Direction] = task.Stage;
            SelectLesson();
            hoveredCell = -1;
            showReset = false;
            statusError = false;
            status = session.Submitted ? "제출 완료된 테스트 과제입니다. 초기화하면 다시 플레이할 수 있습니다."
                : "과제별 작업을 이 실행 중 보관합니다. 업무 문서의 키와 치환표를 확인하세요.";
            Refresh();
        }

        private void SwitchDirection(CipherDirection direction)
        {
            if (hosted) return;
            int stage;
            CipherTask target = selectedStages.TryGetValue(direction, out stage)
                ? tasks.First(t => t.Direction == direction && t.Stage == stage)
                : tasks.First(t => t.Direction == direction);
            Select(target);
        }

        private void ChangeStage(int delta)
        {
            if (hosted) return;
            var available = tasks.Where(t => t.Direction == session.Task.Direction).ToList();
            int index = available.IndexOf(session.Task);
            Select(available[(index + delta + available.Count) % available.Count]);
        }

        private void Refresh()
        {
            SaveDraft();
            keyInput = null;
            PrepareLesson();
            if (page != null) { page.SetActive(false); Destroy(page); }
            page = Rect(screen, "Current task", 0, 0, 1440, 1080).gameObject;
            var root = page.transform;
            BuildHeader(root);
            BuildTranspose(root);
            BuildTable(root);
            BuildResult(root);
            BuildSource(root);
            BuildFooter(root);
            BuildTutorialOverlay(root);
            if (showReset) BuildResetDialog(root);
        }

        private void BuildHeader(Transform root)
        {
            Frame(root, "Header", 16, 12, 1408, 58, green);
            Label(root, "ADFGVX  프로토콜 매니저", 30, 22, 380, 36, 26, green);
            Button(root, "복호화", 440, 22, 115, 36, () => SwitchDirection(CipherDirection.Decrypt),
                !hosted, session.Task.Direction == CipherDirection.Decrypt ? cyan : muted);
            Button(root, "암호화", 565, 22, 115, 36, () => SwitchDirection(CipherDirection.Encrypt),
                !hosted, session.Task.Direction == CipherDirection.Encrypt ? cyan : muted);
            Button(root, "◀", 710, 22, 42, 36, () => ChangeStage(-1), !hosted);
            Label(root, hosted ? hostedTitle : "기존 과제  /  STAGE " + session.Task.Stage, 764, 22, 275, 36, 19, Color.white);
            Button(root, "▶", 1040, 22, 42, 36, () => ChangeStage(1), !hosted);
            Button(root, LessonActive ? "튜토리얼 켜짐" : "튜토리얼 켜기", 1100, 22, 180, 36,
                ToggleLesson, true, LessonActive ? cyan : green, 19);
            Label(root, "TEST", 1300, 22, 110, 36, 19, muted);
            Frame(root, "Steps", 16, 84, 1408, 45, green);
            bool encrypt = session.Task.Direction == CipherDirection.Encrypt;
            string[] steps = encrypt
                ? new[] { "01 문서 / 키", "02 문자 치환", "03 키 순위 전치", "04 결과 / 제출" }
                : new[] { "01 문서 / 키", "02 키 순위 전치", "03 문자 치환", "04 결과 / 제출" };
            int current = CurrentStep - 1;
            for (int i = 0; i < 4; i++)
            {
                float x = 50 + i * 350;
                if (i == current && !session.Submitted)
                    Frame(root, "Current step", x - 12, 90, 282, 33, cyan);
                Label(root, steps[i] + (i < current || session.Submitted ? "  OK" : ""), x, 89, 290, 34,
                    21, i == current ? cyan : i < current ? green : muted);
                if (i < 3) Label(root, "▶", x + 290, 89, 45, 34, 22, muted);
            }
        }

        private void BuildTranspose(Transform root)
        {
            int step = session.Task.Direction == CipherDirection.Encrypt ? 3 : 2;
            var panel = Section(root, "Transposition", "키 순위 전치", step, 16, 145, 322, 821);
            Label(panel, session.TranspositionComplete ? "전치 완료 OK" : "키를 적용하고 열을 선택하세요", 18, 55, 286, 30, 18,
                session.TranspositionComplete ? cyan : muted);
            Frame(panel, "Applied key", 18, 97, 286, 43, session.KeyApplied ? cyan : muted);
            Label(panel, session.KeyApplied ? "적용된 키  " + session.Task.Key : "01 섹션에서 키를 적용하세요",
                28, 100, 266, 37, 20, session.KeyApplied ? cyan : muted);
            int[] order = session.Order;
            int[] priorities = new int[order.Length];
            for (int i = 0; i < order.Length; i++) priorities[order[i]] = i + 1;
            int width = session.Task.Key.Length;
            float cell = Mathf.Min(48, 284f / width);
            float left = (322 - cell * width) / 2;
            string matrix = session.Task.Intermediate;
            int rows = (matrix.Length + width - 1) / width;
            float rowHeight = Mathf.Min(34, 407f / Math.Max(1, rows));
            Label(panel, "알파벳 순위 / 같은 글자는 왼쪽부터", 18, 153, 286, 32, 16, muted);
            for (int c = 0; c < width; c++)
            {
                int column = c;
                bool placed = session.PlacedColumns.Contains(c);
                string title = session.KeyApplied ? session.Task.Key[c] + "\n" + priorities[c] : "?\n-";
                Focus("column:" + c, Button(panel, title, left + c * cell, 190, cell - 3, 64, () => Place(column),
                    session.CanTranspose && !placed, placed ? cyan : green, 20));
                for (int r = 0; r < rows; r++)
                {
                    int at = r * width + c;
                    bool visible = session.Loaded && (session.Task.Direction == CipherDirection.Encrypt
                        ? session.SubstitutionComplete : placed);
                    string value = at >= matrix.Length ? "" : visible ? matrix[at].ToString() : ".";
                    var box = Frame(panel, "Matrix " + r + " " + c, left + c * cell, 262 + r * rowHeight,
                        cell - 3, rowHeight - 2, placed ? cyan * .65f : new Color(.15f, .32f, .23f));
                    Label(box, value, 0, 0, cell - 3, rowHeight - 2, 19, placed ? cyan : Color.white, true);
                }
            }
            string instruction = session.Task.Direction == CipherDirection.Decrypt
                ? "오른쪽 암호 묶음 ▶ 해당 순위의 열\n열을 채운 뒤 가로로 읽습니다."
                : "치환을 마치면 1순위 열부터 클릭\n선택한 열이 결과에 자동 기록됩니다.";
            Label(panel, instruction, 18, 690, 286, 68, 18, muted);
            string progress = session.TranspositionComplete ? "모든 열 배치 완료"
                : "다음 배치: " + (session.PlacedColumns.Count + 1) + "순위";
            Label(panel, progress, 18, 768, 286, 35, 21, session.TranspositionComplete ? cyan : amber);
        }

        private void BuildTable(Transform root)
        {
            int step = session.Task.Direction == CipherDirection.Encrypt ? 2 : 3;
            var panel = Section(root, "Substitution", "이중 문자 치환", step, 354, 145, 460, 821);
            Label(panel, "치환 테이블 " + session.TableNumber + "번", 20, 62, 240, 34, 22, Color.white);
            Focus("tablePrevious", Button(panel, "이전", 274, 61, 76, 36, () => ChangeTable(-1), !session.Submitted));
            Focus("tableNext", Button(panel, "다음", 360, 61, 76, 36, () => ChangeTable(1), !session.Submitted));
            string table = DisplayedTable();
            int hint = HintCell(table);
            const float size = 52;
            for (int r = 0; r < 7; r++)
                for (int c = 0; c < 7; c++)
                {
                    float x = 46 + c * size, y = 130 + r * 60;
                    if (r == 0 || c == 0)
                    {
                        if (r == 0 && c == 0) continue;
                        string axis = AdfgvxCipher.Alphabet[(r == 0 ? c : r) - 1].ToString();
                        bool active = (guidance || LessonActive) && hint >= 0 && (r == 0 ? hint % 6 == c - 1 : hint / 6 == r - 1);
                        Label(panel, axis, x, y, size - 3, 56, 23, active ? amber : green, true);
                    }
                    else
                    {
                        int index = (r - 1) * 6 + c - 1;
                        int captured = index;
                        bool active = hoveredCell == index;
                        Focus("cell:" + index, Button(panel, table[index].ToString(), x, y, size - 3, 56, () => Record(captured),
                            session.CanSubstitute, active ? cyan : (guidance || LessonActive) && hint == index ? amber : muted, 23));
                    }
                }
            for (int i = 0; i < 6; i++)
            {
                focusTargets["row:" + i] = Rect(panel, "Lesson row " + i, 46, 190 + i * 60, 361, 56);
                focusTargets["axis:" + i] = Rect(panel, "Lesson column " + i, 98 + i * 52, 130, 49, 416);
            }
            string target = !session.CanSubstitute ? "앞 단계를 완료하면 치환을 시작합니다."
                : session.Task.Direction == CipherDirection.Encrypt
                    ? "현재 평문  " + session.Task.Input[session.SelectedUnit] + "  ▶  치환표에서 선택"
                    : "현재 암호쌍  " + session.Task.Intermediate.Substring(session.SelectedUnit * 2, 2) + "  ▶  문자 선택";
            Label(panel, target, 20, 574, 420, 54, 21, session.CanSubstitute ? amber : muted);
            Label(panel, "표를 클릭해 기록 / 04 결과 칸에서 수정", 20, 642, 420, 36, 18, Color.white);
            Button(panel, guidance ? "안내 강조 ON" : "안내 강조 OFF", 20, 751, 420, 46,
                () => { guidance = !guidance; Refresh(); }, true, guidance ? cyan : muted);
        }

        private void BuildResult(Transform root)
        {
            var panel = Section(root, "Result", "결과 확인 / 제출", 4, 830, 145, 594, 390);
            bool encrypt = session.Task.Direction == CipherDirection.Encrypt;
            int filled = session.Entries.Count(e => !string.IsNullOrEmpty(e));
            Label(panel, filled + " / " + session.Entries.Length, 480, 55, 96, 30, 19, cyan, true);
            Label(panel, session.Submitted ? "검증 완료 OK" : "수정할 칸 선택 ▶ 치환표에서 다시 기록", 20, 55, 554, 32, 18,
                session.Submitted ? cyan : muted);
            string[] expected = session.ExpectedEntries;
            const int columns = 12;
            for (int i = 0; i < session.Entries.Length; i++)
            {
                int unit = i;
                string value = session.Entries[i];
                Color color = string.IsNullOrEmpty(value) ? muted : value == expected[i] ? cyan : red;
                if (i == session.SelectedUnit && !session.Submitted) color = amber;
                Focus("result:" + i, Button(panel, string.IsNullOrEmpty(value) ? "_" : value, 20 + i % columns * 46,
                    100 + i / columns * 43, 42, 38, () => SelectUnit(unit), session.CanSubstitute,
                    color, encrypt ? 18 : 22));
            }
            int wrong = Enumerable.Range(0, expected.Length).Count(i => !string.IsNullOrEmpty(session.Entries[i]) && session.Entries[i] != expected[i]);
            Label(panel, wrong > 0 ? "불일치 " + wrong + "칸 / 해당 칸을 선택해 수정하세요."
                : session.SubstitutionComplete ? "문자 치환 완료 OK" : "중간부터 수정해도 나머지 기록은 유지됩니다.",
                20, 283, 554, 32, 18, wrong > 0 ? red : session.SubstitutionComplete ? cyan : muted);
            Focus("submit", Button(panel, session.Submitted ? "제출 완료 OK" : "검증 후 제출", 344, 334, 230, 40, Submit,
                session.CanSubmit, session.Submitted ? cyan : green));
            Label(panel, "◀ ▶ 칸 이동", 20, 338, 300, 32, 17, muted);
        }

        private void BuildSource(Transform root)
        {
            var panel = Section(root, "Source", "문서 / 키 입력", 1, 830, 551, 594, 415);
            bool encrypt = session.Task.Direction == CipherDirection.Encrypt;
            Label(panel, session.Task.Title, 20, 61, 385, 36, 22, Color.white);
            Focus("load", Button(panel, session.Loaded ? "불러옴 OK" : "문서 열기", 422, 59, 152, 40,
                () => { session.Load(); Feedback("01 섹션에서 지시된 키를 입력하고 적용하세요.", false); }, !session.Loaded));
            Label(panel, "업무 지시  /  키 " + session.Task.Key + "   /   치환표 " + session.Task.TableNumber + "번",
                20, 111, 554, 32, 21, cyan);
            keyInput = InputField(panel, session.KeyDraft, 20, 152, 260, 38);
            keyInput.interactable = session.Loaded && !session.Submitted && !session.KeyApplied;
            Button(panel, session.KeyApplied ? "키 적용됨 OK" : "키 적용", 294, 152, 160, 38, ApplyKey,
                session.Loaded && !session.Submitted && !session.KeyApplied);
            focusTargets["key"] = Rect(panel, "Key lesson focus", 20, 152, 434, 38);
            if (!session.Loaded)
            {
                Label(panel, "문서 열기를 눌러 원문을 불러오세요.\n파일명이나 평문을 다시 타이핑하지 않습니다.",
                    20, 230, 554, 120, 23, muted);
                return;
            }
            if (encrypt)
            {
                Label(panel, "암호화할 원문 / 클릭해 치환할 위치 선택", 20, 201, 554, 28, 18, muted);
                for (int i = 0; i < session.Task.Input.Length; i++)
                {
                    int unit = i;
                    Button(panel, session.Task.Input[i].ToString(), 20 + i % 16 * 34, 236 + i / 16 * 39,
                        31, 35, () => SelectUnit(unit), session.CanSubstitute,
                        i == session.SelectedUnit ? amber : Color.white, 20);
                }
                Label(panel, "03 열 선택으로 생성된 최종 암호문", 20, 321, 554, 28, 18, muted);
                Label(panel, string.IsNullOrEmpty(session.TranspositionOutput) ? "치환 완료 후 왼쪽 열을 순서대로 선택하세요."
                    : Group(session.TranspositionOutput, 2), 20, 356, 554, 45, 19, cyan);
            }
            else if (!session.TranspositionComplete)
            {
                Label(panel, "02 암호 묶음 선택 ▶ 02 섹션의 열에 배치", 20, 201, 554, 30, 19, amber);
                string[] chunks = AdfgvxCipher.SplitCiphertext(session.Task.Input, session.Task.Key);
                float rowHeight = Mathf.Min(32, 166f / chunks.Length);
                for (int i = 0; i < chunks.Length; i++)
                {
                    int rank = i;
                    bool done = i < session.PlacedColumns.Count;
                    Focus("chunk:" + i, Button(panel, (i + 1) + "   " + chunks[i] + (done ? "  OK" : ""), 20, 239 + i * rowHeight,
                        554, rowHeight - 3, () => { session.SelectedChunk = rank; Refresh(); },
                        session.KeyApplied && !done, done ? cyan : i == session.SelectedChunk ? amber : muted, 19));
                }
            }
            else
            {
                Label(panel, "03 전치 후 암호쌍 / 클릭해 치환할 위치 선택", 20, 201, 554, 30, 18, muted);
                for (int i = 0; i < session.Entries.Length; i++)
                {
                    int unit = i;
                    string pair = session.Task.Intermediate.Substring(i * 2, 2);
                    bool done = !string.IsNullOrEmpty(session.Entries[i]);
                    Button(panel, pair, 20 + i % 10 * 55, 239 + i / 10 * 46, 50, 40,
                        () => SelectUnit(unit), !session.Submitted,
                        i == session.SelectedUnit ? amber : done ? cyan : Color.white, 20);
                }
                Label(panel, "03 표에서 문자 선택 ▶ 04 결과에 기록", 20, 379, 554, 26, 18, muted);
            }
        }

        private void BuildFooter(Transform root)
        {
            var panel = Frame(root, "Status", 16, 982, 1408, 82, green);
            Label(panel, status, 18, 10, 890, 62, 20, statusError ? red : session.Submitted ? cyan : Color.white);
            Button(panel, "되돌리기", 936, 20, 132, 40, Undo, session.CanUndo);
            Button(panel, "다시 실행", 1078, 20, 132, 40, Redo, session.CanRedo);
            if (hosted)
                Button(panel, session.Submitted ? "업무 목록" : "업무 중단", 1220, 20, 168, 40, () => returnCallback?.Invoke());
            else
                Button(panel, "초기화", 1220, 20, 168, 40, () => { showReset = true; Refresh(); });
        }

        private void BuildResetDialog(Transform root)
        {
            var veil = Rect(root, "Reset confirmation", 0, 0, 1440, 1080);
            veil.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, .88f);
            var dialog = Frame(veil, "Reset", 430, 390, 580, 260, amber);
            Label(dialog, "현재 과제만 처음부터 다시 할까요?", 24, 24, 532, 50, 26, amber);
            Label(dialog, "다른 과제와 모드의 기록은 유지됩니다.", 24, 90, 532, 45, 21, Color.white);
            Button(dialog, "취소", 24, 175, 250, 50, () => { showReset = false; Refresh(); });
            Button(dialog, "현재 과제 초기화", 294, 175, 262, 50, () =>
            {
                CipherTask task = session.Task;
                sessions[task.Id] = new CipherSession(task);
                lessons.Remove(task.Id);
                Select(task);
            }, true, amber);
        }

        private void ApplyKey()
        {
            bool success = session.ApplyKey(keyInput.text, out string message);
            Feedback(message, !success);
        }
        private void Record(int cell)
        {
            hoveredCell = cell;
            int recordedUnit = session.SelectedUnit;
            bool success = session.RecordCell(cell, DisplayedTable(), out string message);
            if (success && LessonActive && !lesson.FirstCharacterRecorded)
            {
                lesson.FirstCharacterRecorded = true;
                lesson.FirstRecordedUnit = recordedUnit;
            }
            Feedback(message, !success);
        }
        private void Place(int column)
        {
            bool success = session.PlaceColumn(column, out string message);
            Feedback(message, !success);
        }
        private void Submit()
        {
            if (session.Submitted) return;
            bool success = session.Submit(out string message);
            if (success && hosted)
            {
                completedCallback?.Invoke(session.Task);
                message = "업무 제출 완료. 해독 결과를 확인한 뒤 업무 목록으로 돌아가세요.";
            }
            Feedback(message, !success);
        }
        private void Undo()
        {
            SaveDraft(); session.Undo(); keyInput = null; hoveredCell = -1;
            Feedback("한 단계 되돌렸습니다. Ctrl+Y로 다시 실행할 수 있습니다.", false);
        }
        private void Redo()
        {
            SaveDraft(); session.Redo(); keyInput = null; hoveredCell = -1;
            Feedback("다시 실행했습니다.", false);
        }
        private void Feedback(string message, bool error)
        {
            status = message; statusError = error; Refresh();
        }
        private void SelectUnit(int unit)
        {
            session.SelectedUnit = unit; hoveredCell = -1; Refresh();
        }
        private void MoveSelection(int delta)
        {
            if (!session.CanSubstitute) return;
            SelectUnit((session.SelectedUnit + delta + session.Entries.Length) % session.Entries.Length);
        }
        private void ChangeTable(int delta)
        {
            int next = (session.TableNumber + delta + 7) % 7;
            if (Resources.Load<TextAsset>("GameData/Encrypt/Tables/Table_" + next) == null)
            { Feedback("치환표 파일을 찾을 수 없습니다: " + next, true); return; }
            session.TableNumber = next; hoveredCell = -1; Refresh();
        }
        private string DisplayedTable()
        {
            var table = Resources.Load<TextAsset>("GameData/Encrypt/Tables/Table_" + session.TableNumber);
            string content = table == null ? "" : Compact(table.text);
            return content.Length == 36 ? content : session.Task.Table;
        }
        private int HintCell(string table)
        {
            if (!session.CanSubstitute) return -1;
            if (session.Task.Direction == CipherDirection.Encrypt)
                return table.IndexOf(session.Task.Input[session.SelectedUnit]);
            string pair = session.Task.Intermediate.Substring(session.SelectedUnit * 2, 2);
            return AdfgvxCipher.Alphabet.IndexOf(pair[0]) * 6 + AdfgvxCipher.Alphabet.IndexOf(pair[1]);
        }
        private static string Group(string value, int width) => string.Join(" ", Enumerable.Range(0, (value.Length + width - 1) / width)
            .Select(i => value.Substring(i * width, Math.Min(width, value.Length - i * width))));

        private int CurrentStep => !session.Loaded || !session.KeyApplied ? 1
            : session.Task.Direction == CipherDirection.Encrypt
                ? !session.SubstitutionComplete ? 2 : !session.TranspositionComplete ? 3 : 4
                : !session.TranspositionComplete ? 2 : !session.SubstitutionComplete ? 3 : 4;

        private RectTransform Section(Transform parent, string name, string title, int step,
            float x, float y, float width, float height)
        {
            bool active = CurrentStep == step && !session.Submitted;
            var panel = Frame(parent, name, x, y, width, height, active ? cyan : green);
            focusTargets[name == "Transposition" ? "transpose" : name == "Substitution" ? "table" : name.ToLowerInvariant()] = panel;
            if (active) panel.GetComponent<Image>().color = new Color32(13, 29, 27, 255);
            Label(panel, step.ToString("00") + "  " + title, 18, 14, width - 116, 36, 26, active ? cyan : green);
            if (active)
            {
                Frame(panel, "Current section", width - 88, 18, 68, 29, cyan);
                Label(panel, "현재", width - 85, 18, 62, 29, 18, cyan, true);
            }
            return panel;
        }

        private RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }
        private RectTransform Frame(Transform parent, string name, float x, float y, float width, float height, Color border)
        {
            var rect = Rect(parent, name, x, y, width, height);
            rect.gameObject.AddComponent<Image>().color = ink;
            // Draw complete edges inside the rect. The old 1px Outline sits outside the
            // geometry and loses edges at fractional canvas scales / neighboring graphics.
            const float thickness = 2;
            Edge(rect, "Top", 0, 0, width, thickness, border);
            Edge(rect, "Bottom", 0, height - thickness, width, thickness, border);
            Edge(rect, "Left", 0, 0, thickness, height, border);
            Edge(rect, "Right", width - thickness, 0, thickness, height, border);
            return rect;
        }

        private void Edge(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            var edge = Rect(parent, name + " border", x, y, width, height).gameObject.AddComponent<Image>();
            edge.color = color;
            edge.raycastTarget = false;
        }
        private TextMeshProUGUI Label(Transform parent, string text, float x, float y, float width, float height,
            float size, Color color, bool centered = false)
        {
            var rect = Rect(parent, "Text", x, y, width, height);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.alignment = centered ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.raycastTarget = false;
            return label;
        }
        private Button Button(Transform parent, string text, float x, float y, float width, float height,
            Action action, bool interactable = true, Color? tint = null, float size = 20)
        {
            Color color = tint ?? green;
            var rect = Frame(parent, text, x, y, width, height, interactable ? color : color * .45f);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(.22f, .35f, .31f);
            colors.pressedColor = new Color(.35f, .55f, .48f);
            colors.disabledColor = new Color(.65f, .65f, .65f, 1);
            button.colors = colors;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.interactable = interactable;
            Label(rect, text, 3, 0, width - 6, height, size, interactable ? color : color * .65f, true);
            button.onClick.AddListener(() => action());
            return button;
        }
        private TMP_InputField InputField(Transform parent, string text, float x, float y, float width, float height)
        {
            var rect = Frame(parent, "Key input", x, y, width, height, session.KeyApplied ? cyan : muted);
            var viewport = Rect(rect, "Viewport", 8, 4, width - 16, height - 8);
            viewport.gameObject.AddComponent<RectMask2D>();
            var label = Label(viewport, "", 0, 0, width - 16, height - 8, 23, Color.white);
            var placeholder = Label(viewport, "키 입력…", 0, 0, width - 16, height - 8, 21, muted);
            var input = rect.gameObject.AddComponent<TMP_InputField>();
            input.textViewport = viewport; input.textComponent = label; input.placeholder = placeholder;
            input.targetGraphic = rect.GetComponent<Image>();
            input.characterLimit = 9;
            input.contentType = TMP_InputField.ContentType.Alphanumeric;
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.onValidateInput = (current, index, added) => char.IsLetterOrDigit(added) ? char.ToUpperInvariant(added) : '\0';
            input.SetTextWithoutNotify(text ?? "");
            input.onValueChanged.AddListener(value => session.KeyDraft = value);
            input.onSubmit.AddListener(value => ApplyKey());
            return input;
        }
    }
}
