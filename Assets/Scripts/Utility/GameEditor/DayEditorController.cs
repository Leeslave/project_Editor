using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameEditor
{
    public sealed class DayEditorController : MonoBehaviour
    {
        private const float MainScrollSensitivity = 72f;
        private const float DropdownScrollSensitivity = 54f;

        [Serializable] public struct WorldBackground
        {
            public World world;
            public int position;
            public Sprite sprite;
        }

        [Serializable] public struct NpcVisual
        {
            public string type;
            public Sprite sprite;
        }

        [Header("프로젝트 에셋")]
        [SerializeField] private TMP_FontAsset koreanFont;
        [SerializeField] private List<WorldBackground> backgrounds = new();
        [SerializeField] private List<NpcVisual> npcVisuals = new();
        [SerializeField] private List<AudioClip> bgmClips = new();
        [SerializeField] private List<string> miniGameScenes = new();

        private readonly Color panelColor = new(0.10f, 0.12f, 0.16f, 0.97f);
        private readonly Color fieldColor = new(0.18f, 0.21f, 0.27f, 1);
        private readonly Color accentColor = new(0.20f, 0.55f, 0.85f, 1);
        private RectTransform fileContent;
        private RectTransform editorContent;
        private TMP_InputField newNameField;
        private TMP_Text statusText;
        private TMP_Text titleText;
        private DailyData currentData;
        private string currentFile;
        private int selectedTime;
        private AudioSource previewAudio;
        private GameObject visualModal;
        private Image modalBackground;
        private Image modalNpc;
        private TMP_Text modalMessage;
        private Slider modalScale;
        private Anchor modalAnchor;
        private Action modalChanged;

        private void Awake()
        {
            if (koreanFont == null)
            {
                Debug.LogError("DayEditor: 경기천년 TMP 폰트가 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }
            previewAudio = gameObject.AddComponent<AudioSource>();
            previewAudio.loop = true;
            BuildShell();
            RefreshFiles();
        }

        private void BuildShell()
        {
            var canvas = NewObject<Canvas>("DayEditorCanvas", transform);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            scaler.referencePixelsPerUnit = 100;
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            RectTransform root = Rect(canvas.gameObject);
            Stretch(root);
            root.pivot = new Vector2(.5f, .5f);
            Image bg = canvas.gameObject.AddComponent<Image>();
            bg.color = new Color(.045f, .055f, .075f, 1);

            RectTransform top = Panel("상단", root, panelColor);
            SetOffsets(top, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -78), Vector2.zero, new Vector2(.5f, 1));
            titleText = Text("DAY DATA EDITOR", top, 31, TextAlignmentOptions.MidlineLeft);
            Stretch(titleText.rectTransform, 24, 680, 8, 8);

            RectTransform toolbar = Horizontal(top, 8);
            SetOffsets(toolbar, new Vector2(1, .5f), new Vector2(1, .5f),
                new Vector2(-650, -18), new Vector2(-332, 18), new Vector2(1, .5f));
            LayoutElement toolbarLayout = toolbar.gameObject.AddComponent<LayoutElement>();
            toolbarLayout.minHeight = toolbarLayout.preferredHeight = 36;
            toolbarLayout.flexibleHeight = 0;
            LayoutElement validateLayout = Button("검증", toolbar, ValidateCurrent).gameObject.AddComponent<LayoutElement>();
            validateLayout.minWidth = validateLayout.preferredWidth = 82;
            validateLayout.minHeight = validateLayout.preferredHeight = 36;
            LayoutElement saveLayout = Button("저장", toolbar, SaveCurrent, accentColor).gameObject.AddComponent<LayoutElement>();
            saveLayout.minWidth = saveLayout.preferredWidth = 82;
            saveLayout.minHeight = saveLayout.preferredHeight = 36;
            LayoutElement bgmLayout = Button("BGM 정지", toolbar, () => previewAudio.Stop()).gameObject.AddComponent<LayoutElement>();
            bgmLayout.minWidth = bgmLayout.preferredWidth = 138;
            bgmLayout.minHeight = bgmLayout.preferredHeight = 36;

            statusText = Text("파일을 선택하세요.", top, 20, TextAlignmentOptions.MidlineRight);
            SetOffsets(statusText.rectTransform, new Vector2(1, 0), Vector2.one,
                new Vector2(-320, 8), new Vector2(-24, -8), new Vector2(1, .5f));

            RectTransform left = Panel("파일", root, panelColor);
            SetOffsets(left, new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(12, 12), new Vector2(370, -88), new Vector2(0, .5f));
            VerticalLayoutGroup lv = left.gameObject.AddComponent<VerticalLayoutGroup>();
            lv.padding = new RectOffset(14, 14, 14, 14); lv.spacing = 10;
            lv.childControlHeight = true; lv.childForceExpandHeight = false;
            Text("DAY FILES", left, 25, TextAlignmentOptions.Center).gameObject.AddComponent<LayoutElement>().preferredHeight = 42;
            Button("새로고침", left, RefreshFiles).gameObject.AddComponent<LayoutElement>().preferredHeight = 46;
            fileContent = Scroll("파일 목록", left, out _);
            fileContent.parent.parent.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;
            newNameField = Input("새 파일명", left, false);
            newNameField.gameObject.AddComponent<LayoutElement>().preferredHeight = 48;
            Button("기본값으로 새 파일", left, CreateNew).gameObject.AddComponent<LayoutElement>().preferredHeight = 50;

            RectTransform main = Panel("편집", root, panelColor);
            SetOffsets(main, Vector2.zero, Vector2.one,
                new Vector2(378, 12), new Vector2(-12, -88), new Vector2(.5f, .5f));
            VerticalLayoutGroup mv = main.gameObject.AddComponent<VerticalLayoutGroup>();
            mv.padding = new RectOffset(16, 16, 14, 14); mv.spacing = 10;
            mv.childControlHeight = true; mv.childForceExpandHeight = false;
            editorContent = Scroll("데이터 편집", main, out _);
            editorContent.parent.parent.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

            BuildVisualModal(root);
            ShowEmptyEditor("왼쪽에서 DailyData 파일을 선택하거나 새 파일을 만드세요.");
        }

        private void RefreshFiles()
        {
            Clear(fileContent);
            foreach (string file in DayEditorDataService.ListFiles())
            {
                string captured = file;
                Button(file, fileContent, () => Open(captured)).gameObject.AddComponent<LayoutElement>().preferredHeight = 42;
            }
            SetStatus($"{DayEditorDataService.ListFiles().Count}개 파일");
        }

        private void CreateNew()
        {
            if (!DayEditorDataService.TryNormalizeNewFileName(newNameField.text, out string name, out string error))
            {
                SetStatus(error, true); return;
            }
            currentFile = name;
            currentData = DayEditorDataService.CreateDefault();
            selectedTime = 0;
            RebuildEditor();
            SetStatus($"{name} 신규 데이터 — 저장 전");
        }

        private void Open(string file)
        {
            DayEditorLoadResult result = DayEditorDataService.Load(file);
            if (!result.CanEdit)
            {
                currentData = null; currentFile = null;
                ShowIssues(result.issues);
                titleText.text = $"열 수 없음 · {file}";
                return;
            }
            currentFile = file;
            currentData = DayEditorDataService.CloneAndNormalize(result.data);
            selectedTime = 0;
            if (result.issues.Count > 0)
            {
                ShowIssues(result.issues);
                SetStatus($"{file} 원본 검증 결과를 확인하세요.",
                    result.issues.Any(x => x.severity == DayEditorIssueSeverity.Error));
            }
            else
            {
                RebuildEditor();
                SetStatus($"{file} 열림");
            }
        }

        private void ValidateCurrent()
        {
            if (currentData == null) { SetStatus("편집 중인 파일이 없습니다.", true); return; }
            ShowIssues(CurrentIssues());
        }

        private void SaveCurrent()
        {
            if (currentData == null) { SetStatus("편집 중인 파일이 없습니다.", true); return; }
            List<DayEditorIssue> issues = CurrentIssues();
            if (issues.Any(x => x.severity == DayEditorIssueSeverity.Error))
            {
                ShowIssues(issues); SetStatus("구조 오류를 수정해야 저장할 수 있습니다.", true); return;
            }
            if (!DayEditorDataService.Save(currentFile, currentData, out string error))
            {
                SetStatus($"저장 실패: {error}", true); return;
            }
            DayEditorLoadResult check = DayEditorDataService.Load(currentFile);
            if (!check.CanEdit) { ShowIssues(check.issues); SetStatus("저장 후 재검증에 실패했습니다.", true); return; }
            RefreshFiles();
            SetStatus($"{currentFile} 저장 완료" + (issues.Count > 0 ? $" · 경고 {issues.Count}개" : ""));
        }

        private List<DayEditorIssue> CurrentIssues() =>
            DayEditorValidator.Validate(currentData, miniGameScenes, bgmClips);

        private void ShowIssues(IEnumerable<DayEditorIssue> issues)
        {
            List<DayEditorIssue> list = issues.ToList();
            Clear(editorContent);
            titleText.text = currentFile == null ? "검증 결과" : $"검증 결과 · {currentFile}";
            if (list.Count == 0) Text("문제가 없습니다.", editorContent, 24, TextAlignmentOptions.Center);
            foreach (DayEditorIssue issue in list)
            {
                TMP_Text line = Text(issue.ToString(), editorContent, 20, TextAlignmentOptions.TopLeft);
                line.color = issue.severity == DayEditorIssueSeverity.Error
                    ? new Color(1, .38f, .38f) : new Color(1, .76f, .25f);
                line.enableWordWrapping = true;
                line.gameObject.AddComponent<LayoutElement>().preferredHeight = 58;
            }
            if (currentData != null)
                Button("편집 화면으로 돌아가기", editorContent, RebuildEditor).gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
            int errors = list.Count(x => x.severity == DayEditorIssueSeverity.Error);
            SetStatus($"오류 {errors} · 경고 {list.Count - errors}", errors > 0);
        }

        private void RebuildEditor()
        {
            Clear(editorContent);
            if (currentData == null) return;
            titleText.text = $"편집 · {currentFile}";

            Section("기본 정보");
            RectTransform date = Horizontal(editorContent, 8);
            date.gameObject.AddComponent<LayoutElement>().preferredHeight = 54;
            LabeledInt("연", date, currentData.date.year, v => currentData.date.year = v);
            LabeledInt("월", date, currentData.date.month, v => currentData.date.month = v);
            LabeledInt("일", date, currentData.date.day, v => currentData.date.day = v);

            Section("시작 위치");
            WorldVectorRow(editorContent, currentData.startLocation, null);

            Section("업무");
            for (int i = 0; i < currentData.workList.Count; i++) BuildWork(i);
            Button("+ 업무 추가", editorContent, () => { currentData.workList.Add(new Work()); RebuildEditor(); })
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 44;

            Section("시간대");
            RectTransform tabs = Horizontal(editorContent, 7);
            tabs.gameObject.AddComponent<LayoutElement>().preferredHeight = 48;
            string[] labels = { "출근 전", "출근 후", "근무 후", "퇴근 후" };
            for (int i = 0; i < 4; i++)
            {
                int captured = i;
                Button(labels[i], tabs, () => { selectedTime = captured; RebuildEditor(); },
                    i == selectedTime ? accentColor : fieldColor).gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            }
            BuildTime(currentData.dayTimes[selectedTime]);
        }

        private void BuildWork(int index)
        {
            Work work = currentData.workList[index] ??= new Work();
            RectTransform row = Horizontal(editorContent, 7);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 55;
            LabeledString("이름", row, work.name, v => work.name = v);
            TMP_Dropdown code = Dropdown(row, miniGameScenes.Count > 0 ? miniGameScenes : new List<string> { "TestGame" },
                Mathf.Max(0, miniGameScenes.IndexOf(work.code)), v => work.code = v);
            code.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            LabeledInt("스테이지", row, work.stage, v => work.stage = v);
            Button("삭제", row, () => { currentData.workList.RemoveAt(index); RebuildEditor(); }, new Color(.65f, .2f, .2f))
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 90;
        }

        private void BuildTime(TimeData time)
        {
            Section($"시간대 {selectedTime + 1} 상세");
            RectTransform clock = Horizontal(editorContent, 8);
            clock.gameObject.AddComponent<LayoutElement>().preferredHeight = 54;
            LabeledDigits("시", clock, time.daytime.hour, v => time.daytime.hour = v);
            LabeledDigits("분", clock, time.daytime.minute, v => time.daytime.minute = v);

            Section($"NPC · {time.npc.Count}");
            for (int i = 0; i < time.npc.Count; i++) BuildNpc(time, i);
            Button("+ NPC 추가", editorContent, () => {
                time.npc.Add(new ChatObjectData {
                    name = "", objectType = "none", positions = new List<WorldVector>(),
                    anchor = new List<Anchor>(), chat = new List<string>(), onAwake = new List<bool>()
                }); RebuildEditor();
            }).gameObject.AddComponent<LayoutElement>().preferredHeight = 44;

            Section($"Action · {time.action.Count}");
            for (int i = 0; i < time.action.Count; i++) BuildAction(time, i);
            Button("+ Action 추가", editorContent, () => {
                time.action.Add(new ActionObjectData {
                    positions = new List<WorldVector>(), anchor = new List<Anchor>(), actionName = "", actionParam = ""
                }); RebuildEditor();
            }).gameObject.AddComponent<LayoutElement>().preferredHeight = 44;

            Section($"이동 차단 · {time.block.Count}");
            for (int i = 0; i < time.block.Count; i++)
            {
                int captured = i;
                WorldVectorRow(editorContent, time.block[i], () => { time.block.RemoveAt(captured); RebuildEditor(); });
            }
            Button("+ 차단 위치", editorContent, () => { time.block.Add(new WorldVector(World.Street, 0)); RebuildEditor(); })
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 44;

            Section($"BGM 재정의 · {time.bgm.Count}");
            for (int i = 0; i < time.bgm.Count; i++) BuildBgm(time, i);
            Button("+ BGM 재정의", editorContent, () => {
                time.bgm.Add(new BGMData { location = World.Street, code = 0 }); RebuildEditor();
            }).gameObject.AddComponent<LayoutElement>().preferredHeight = 44;
        }

        private void BuildNpc(TimeData time, int index)
        {
            ChatObjectData npc = time.npc[index];
            RectTransform box = Box(editorContent);
            VerticalLayoutGroup outerLayout = box.gameObject.AddComponent<VerticalLayoutGroup>();
            outerLayout.padding = new RectOffset(10, 10, 10, 10);
            outerLayout.spacing = 8;
            outerLayout.childControlHeight = true;
            outerLayout.childForceExpandHeight = false;

            RectTransform header = Horizontal(box, 8);
            header.gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
            TMP_Text headerTitle = Text($"NPC {index + 1}", header, 21, TextAlignmentOptions.MidlineLeft);
            headerTitle.color = new Color(.55f, .82f, 1);
            headerTitle.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Button("NPC 삭제", header, () => { time.npc.RemoveAt(index); RebuildEditor(); },
                    new Color(.65f, .2f, .2f))
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 100;

            RectTransform body = Horizontal(box, 10);
            HorizontalLayoutGroup bodyLayout = body.GetComponent<HorizontalLayoutGroup>();
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandHeight = true;

            RectTransform identityColumn = Panel("NPC 및 ChatData", body, new Color(.105f, .12f, .16f, 1));
            LayoutElement identityColumnLayout = identityColumn.gameObject.AddComponent<LayoutElement>();
            identityColumnLayout.flexibleWidth = .44f;
            VerticalLayoutGroup identityLayout = identityColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            identityLayout.padding = new RectOffset(10, 10, 9, 9);
            identityLayout.spacing = 7;
            identityLayout.childControlHeight = true;
            identityLayout.childForceExpandHeight = false;

            TMP_Text identityTitle = Text("NPC 정보 / ChatData", identityColumn, 19, TextAlignmentOptions.Left);
            identityTitle.color = new Color(.62f, .84f, 1);
            identityTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 28;
            RectTransform nameRow = Horizontal(identityColumn, 7);
            nameRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 46;
            LabeledString("NPC 이름", nameRow, npc.name, v => npc.name = v);
            string[] types = { "none", "Rex", "Clover", "Henderson", "Kennedy", "King", "Klayton",
                "Price", "Walter", "Mechanic", "Monk", "Reporter", "Nametag" };
            RectTransform typeRow = Horizontal(identityColumn, 7);
            typeRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 46;
            TMP_Text typeLabel = Text("NPC 타입", typeRow, 18, TextAlignmentOptions.MidlineRight);
            typeLabel.gameObject.AddComponent<LayoutElement>().preferredWidth = 92;
            Dropdown(typeRow, types, Mathf.Max(0, Array.IndexOf(types, npc.objectType)), v => npc.objectType = v)
                .gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            if (npc.chat.Count != npc.onAwake.Count)
                Button("chat/onAwake 대응 개수 맞춤", identityColumn, () => {
                    NormalizePairs(npc.chat, npc.onAwake, () => "", () => false); RebuildEditor();
                }, new Color(.65f, .42f, .12f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            for (int i = 0; i < Math.Min(npc.chat.Count, npc.onAwake.Count); i++)
            {
                int captured = i;
                RectTransform row = Horizontal(identityColumn, 6);
                row.gameObject.AddComponent<LayoutElement>().preferredHeight = 48;
                TMP_InputField chat = Input("ChatData 상대 경로", row, false);
                chat.text = npc.chat[i] ?? "";
                chat.onValueChanged.AddListener(v => npc.chat[captured] = v);
                chat.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                Toggle toggle = ToggleUI("자동", row, npc.onAwake[i], v => npc.onAwake[captured] = v);
                toggle.gameObject.AddComponent<LayoutElement>().preferredWidth = 92;
                Button("삭제", row, () => {
                    npc.chat.RemoveAt(captured); npc.onAwake.RemoveAt(captured); RebuildEditor();
                }).gameObject.AddComponent<LayoutElement>().preferredWidth = 66;
            }
            Button("+ ChatData", identityColumn, () => { npc.chat.Add(""); npc.onAwake.Add(false); RebuildEditor(); })
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 40;

            RectTransform anchorColumn = Panel("위치 및 Anchor", body, new Color(.105f, .12f, .16f, 1));
            LayoutElement anchorColumnLayout = anchorColumn.gameObject.AddComponent<LayoutElement>();
            anchorColumnLayout.flexibleWidth = .56f;
            VerticalLayoutGroup anchorLayout = anchorColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            anchorLayout.padding = new RectOffset(10, 10, 9, 9);
            anchorLayout.spacing = 7;
            anchorLayout.childControlHeight = true;
            anchorLayout.childForceExpandHeight = false;
            TMP_Text anchorTitle = Text("위치 / Anchor (X · Y · Size)", anchorColumn, 19, TextAlignmentOptions.Left);
            anchorTitle.color = new Color(.62f, .84f, 1);
            anchorTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 28;
            if (npc.positions.Count != npc.anchor.Count)
                Button("positions/anchor 대응 개수 맞춤", anchorColumn, () => {
                    NormalizePairs(npc.positions, npc.anchor, () => new WorldVector(World.Street, 0), () => new Anchor());
                    RebuildEditor();
                }, new Color(.65f, .42f, .12f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            for (int i = 0; i < Math.Min(npc.positions.Count, npc.anchor.Count); i++)
            {
                int captured = i;
                BuildAnchorRow(anchorColumn, npc.positions[i], npc.anchor[i], npc.objectType, () => {
                    npc.positions.RemoveAt(captured); npc.anchor.RemoveAt(captured); RebuildEditor();
                });
            }
            Button("+ 위치/Anchor", anchorColumn, () => {
                npc.positions.Add(new WorldVector(World.Street, 0)); npc.anchor.Add(new Anchor()); RebuildEditor();
            }).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;

            int chatRows = Math.Min(npc.chat.Count, npc.onAwake.Count);
            int anchorRows = Math.Min(npc.positions.Count, npc.anchor.Count);
            float leftHeight = 190 + chatRows * 55 + (npc.chat.Count != npc.onAwake.Count ? 47 : 0);
            float rightHeight = 92 + anchorRows * 57 + (npc.positions.Count != npc.anchor.Count ? 47 : 0);
            float bodyHeight = Mathf.Max(220, leftHeight, rightHeight);
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = bodyHeight;
            box.gameObject.AddComponent<LayoutElement>().preferredHeight = bodyHeight + 66;
        }

        private void BuildAction(TimeData time, int index)
        {
            ActionObjectData action = time.action[index];
            RectTransform box = Box(editorContent);
            VerticalLayoutGroup layout = box.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10); layout.spacing = 7;
            layout.childControlHeight = true; layout.childForceExpandHeight = false;
            RectTransform head = Horizontal(box, 7); head.gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
            LabeledString("Action", head, action.actionName, v => action.actionName = v);
            LabeledString("Param", head, action.actionParam, v => action.actionParam = v);
            Button("삭제", head, () => { time.action.RemoveAt(index); RebuildEditor(); }, new Color(.65f, .2f, .2f))
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 80;
            for (int i = 0; i < Math.Min(action.positions.Count, action.anchor.Count); i++)
            {
                int captured = i;
                BuildAnchorRow(box, action.positions[i], action.anchor[i], null, () => {
                    action.positions.RemoveAt(captured); action.anchor.RemoveAt(captured); RebuildEditor();
                });
            }
            if (action.positions.Count != action.anchor.Count)
                Button("positions/anchor 대응 개수 맞춤", box, () => {
                    NormalizePairs(action.positions, action.anchor, () => new WorldVector(World.Street, 0), () => new Anchor());
                    RebuildEditor();
                }, new Color(.65f, .42f, .12f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            Button("+ 위치/Anchor 쌍", box, () => {
                action.positions.Add(new WorldVector(World.Street, 0)); action.anchor.Add(new Anchor()); RebuildEditor();
            }).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
            box.gameObject.AddComponent<LayoutElement>().preferredHeight =
                125 + Math.Min(action.positions.Count, action.anchor.Count) * 55;
        }

        private void BuildAnchorRow(Transform parent, WorldVector vector, Anchor anchor, string npcType, Action remove)
        {
            RectTransform row = Horizontal(parent, 5); row.gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
            Dropdown(row, WorldNames(), (int)vector.location, v => vector.location = (World)Array.IndexOf(WorldNames(), v))
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 150;
            IntField(row, vector.position, v => vector.position = v).gameObject.AddComponent<LayoutElement>().preferredWidth = 80;
            FloatField(row, anchor.x, v => anchor.x = v).gameObject.AddComponent<LayoutElement>().preferredWidth = 95;
            FloatField(row, anchor.y, v => anchor.y = v).gameObject.AddComponent<LayoutElement>().preferredWidth = 95;
            FloatField(row, anchor.size, v => anchor.size = v).gameObject.AddComponent<LayoutElement>().preferredWidth = 95;
            Button("시각 편집", row, () => OpenVisual(vector, anchor, npcType, RebuildEditor))
                .gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Button("삭제", row, remove).gameObject.AddComponent<LayoutElement>().preferredWidth = 75;
        }

        private void BuildBgm(TimeData time, int index)
        {
            BGMData bgm = time.bgm[index];
            RectTransform row = Horizontal(editorContent, 7); row.gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
            Dropdown(row, WorldNames(), (int)bgm.location, v => bgm.location = (World)Array.IndexOf(WorldNames(), v))
                .gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            List<string> clips = bgmClips.Select((x, i) => $"{i}: {(x == null ? "(없음)" : x.name)}").ToList();
            if (clips.Count == 0) clips.Add("0: (클립 없음)");
            Dropdown(row, clips, Mathf.Clamp(bgm.code, 0, clips.Count - 1), v => bgm.code = clips.IndexOf(v))
                .gameObject.AddComponent<LayoutElement>().flexibleWidth = 2;
            Button("재생", row, () => PlayBgm(bgm.code)).gameObject.AddComponent<LayoutElement>().preferredWidth = 80;
            Button("삭제", row, () => { time.bgm.RemoveAt(index); RebuildEditor(); })
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 80;
        }

        private void WorldVectorRow(Transform parent, WorldVector vector, Action remove)
        {
            RectTransform row = Horizontal(parent, 7); row.gameObject.AddComponent<LayoutElement>().preferredHeight = 52;
            Dropdown(row, WorldNames(), (int)vector.location, v => vector.location = (World)Array.IndexOf(WorldNames(), v))
                .gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            LabeledInt("위치", row, vector.position, v => vector.position = v);
            LabeledString("표시명", row, vector.name, v => vector.name = v);
            Button("배경 보기", row, () => OpenVisual(vector, null, null, null))
                .gameObject.AddComponent<LayoutElement>().preferredWidth = 115;
            if (remove != null) Button("삭제", row, remove).gameObject.AddComponent<LayoutElement>().preferredWidth = 75;
        }

        private void PlayBgm(int code)
        {
            if (code < 0 || code >= bgmClips.Count || bgmClips[code] == null)
            { SetStatus("재생할 BGM 클립이 없습니다.", true); return; }
            previewAudio.clip = bgmClips[code]; previewAudio.Play();
            SetStatus($"BGM 재생: {bgmClips[code].name}");
        }

        private string[] WorldNames() => Enumerable.Range(0, (int)World.Max).Select(i => ((World)i).ToString()).ToArray();

        private static void NormalizePairs<TA, TB>(IList<TA> a, IList<TB> b, Func<TA> makeA, Func<TB> makeB)
        {
            int count = Math.Max(a.Count, b.Count);
            while (a.Count < count) a.Add(makeA());
            while (b.Count < count) b.Add(makeB());
        }

        private void BuildVisualModal(RectTransform root)
        {
            visualModal = Panel("시각 편집", root, new Color(0, 0, 0, .9f)).gameObject;
            Stretch(Rect(visualModal));
            visualModal.transform.SetAsLastSibling();
            RectTransform card = Panel("프리뷰", visualModal.transform, panelColor);
            SetOffsets(card, new Vector2(.18f, .10f), new Vector2(.82f, .90f),
                Vector2.zero, Vector2.zero, new Vector2(.5f, .5f));
            modalBackground = NewObject<Image>("배경", card);
            Stretch(modalBackground.rectTransform, 20, 20, 85, 20);
            modalBackground.preserveAspect = true;
            modalNpc = NewObject<Image>("NPC", modalBackground.transform);
            modalNpc.rectTransform.anchorMin = modalNpc.rectTransform.anchorMax = new Vector2(.5f, .5f);
            modalNpc.rectTransform.sizeDelta = new Vector2(300, 500);
            modalNpc.preserveAspect = true;
            var drag = modalNpc.gameObject.AddComponent<DayEditorPreviewDrag>();
            drag.onDragged = delta => {
                if (modalAnchor == null) return;
                RectTransform bgRect = modalBackground.rectTransform;
                modalAnchor.x += delta.x / Mathf.Max(1, bgRect.rect.width) * 2f;
                modalAnchor.y += delta.y / Mathf.Max(1, bgRect.rect.height) * 2f;
                RefreshModalNpc();
            };
            modalMessage = Text("", card, 20, TextAlignmentOptions.Center);
            SetOffsets(modalMessage.rectTransform, new Vector2(0, 1), Vector2.one,
                new Vector2(20, -72), new Vector2(-20, -20), new Vector2(.5f, 1));
            modalScale = NewObject<Slider>("크기", card);
            SetOffsets(modalScale.GetComponent<RectTransform>(), new Vector2(.25f, 0), new Vector2(.65f, 0),
                new Vector2(20, 20), new Vector2(-20, 62), new Vector2(.5f, 0));
            modalScale.minValue = 0f; modalScale.maxValue = 10f;
            modalScale.onValueChanged.AddListener(v => { if (modalAnchor != null) { modalAnchor.size = v; RefreshModalNpc(); } });
            Button done = Button("완료", card, CloseVisual, accentColor);
            RectTransform doneRect = done.GetComponent<RectTransform>();
            doneRect.anchorMin = new Vector2(1, 0); doneRect.anchorMax = new Vector2(1, 0);
            doneRect.pivot = new Vector2(1, 0); doneRect.anchoredPosition = new Vector2(-20, 18);
            doneRect.sizeDelta = new Vector2(150, 48);
            visualModal.SetActive(false);
        }

        private void OpenVisual(WorldVector vector, Anchor anchor, string npcType, Action changed)
        {
            modalAnchor = anchor; modalChanged = changed;
            WorldBackground found = backgrounds.FirstOrDefault(x => x.world == vector.location && x.position == vector.position);
            modalBackground.sprite = found.sprite;
            modalBackground.color = found.sprite == null ? new Color(.22f, .22f, .25f) : Color.white;
            NpcVisual npc = npcVisuals.FirstOrDefault(x => x.type == npcType);
            modalNpc.sprite = npc.sprite;
            modalNpc.enabled = anchor != null && npc.sprite != null;
            modalScale.gameObject.SetActive(anchor != null);
            if (anchor != null) modalScale.SetValueWithoutNotify(Mathf.Clamp(anchor.size, modalScale.minValue, modalScale.maxValue));
            modalMessage.text = found.sprite == null ? $"{vector}: 대응 배경 이미지 없음" :
                anchor != null && npc.sprite == null ? $"{vector}: NPC 표시 이미지 없음 — 수치 편집 가능" : vector.ToString();
            Canvas.ForceUpdateCanvases();
            RefreshModalNpc();
            visualModal.SetActive(true);
        }

        private void RefreshModalNpc()
        {
            if (modalAnchor == null) return;
            UnityEngine.Rect previewRect = modalBackground.rectTransform.rect;
            modalNpc.rectTransform.anchoredPosition = new Vector2(
                modalAnchor.x * previewRect.width * .5f,
                modalAnchor.y * previewRect.height * .5f);
            modalNpc.rectTransform.localScale = Vector3.one * modalAnchor.size;
        }

        private void CloseVisual()
        {
            visualModal.SetActive(false); modalChanged?.Invoke();
            modalAnchor = null; modalChanged = null;
        }

        private void ShowEmptyEditor(string message)
        {
            Clear(editorContent);
            Text(message, editorContent, 24, TextAlignmentOptions.Center);
        }

        private void Section(string label)
        {
            TMP_Text text = Text(label, editorContent, 25, TextAlignmentOptions.Left);
            text.color = new Color(.55f, .82f, 1);
            text.gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
        }

        private void SetStatus(string message, bool error = false)
        {
            statusText.text = message;
            statusText.color = error ? new Color(1, .4f, .4f) : new Color(.8f, .9f, 1);
        }

        // UI helpers
        private T NewObject<T>(string name, Transform parent) where T : Component
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            rect.localScale = Vector3.one;
            if (typeof(T) == typeof(RectTransform)) return go.GetComponent<T>();
            return go.AddComponent<T>();
        }

        private RectTransform Panel(string name, Transform parent, Color color)
        {
            Image image = NewObject<Image>(name, parent); image.color = color; return image.rectTransform;
        }

        private RectTransform Box(Transform parent) => Panel("항목", parent, new Color(.13f, .15f, .20f, 1));

        private TMP_Text Text(string value, Transform parent, float size, TextAlignmentOptions alignment)
        {
            TextMeshProUGUI text = NewObject<TextMeshProUGUI>("Text", parent);
            text.font = koreanFont; text.fontSize = size; text.text = value; text.alignment = alignment;
            text.color = Color.white; text.raycastTarget = false; return text;
        }

        private Button Button(string label, Transform parent, Action click, Color? color = null)
        {
            Image image = NewObject<Image>(label, parent); image.color = color ?? fieldColor;
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image; button.onClick.AddListener(() => click?.Invoke());
            TMP_Text text = Text(label, button.transform, 20, TextAlignmentOptions.Center); Stretch(text.rectTransform, 6, 6, 4, 4);
            return button;
        }

        private TMP_InputField Input(string placeholder, Transform parent, bool integer)
        {
            Image image = NewObject<Image>("Input", parent); image.color = fieldColor;
            TMP_InputField input = image.gameObject.AddComponent<TMP_InputField>();
            input.contentType = integer ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.Standard;
            input.targetGraphic = image;
            input.interactable = true;
            input.customCaretColor = true;
            input.caretColor = new Color(.45f, .85f, 1f, 1f);
            input.caretWidth = 3;
            input.caretBlinkRate = .75f;
            input.selectionColor = new Color(.20f, .55f, .85f, .55f);
            input.shouldHideMobileInput = false;
            TMP_Text value = Text("", input.transform, 20, TextAlignmentOptions.MidlineLeft);
            Stretch(value.rectTransform, 10, 10, 4, 4);
            TMP_Text hint = Text(placeholder, input.transform, 18, TextAlignmentOptions.MidlineLeft);
            hint.color = new Color(1, 1, 1, .35f); Stretch(hint.rectTransform, 10, 10, 4, 4);
            input.textComponent = value; input.placeholder = hint;
            return input;
        }

        private TMP_InputField IntField(Transform parent, int value, Action<int> changed)
        {
            TMP_InputField input = Input("0", parent, true); input.text = value.ToString();
            input.onEndEdit.AddListener(v => { if (int.TryParse(v, out int parsed)) changed(parsed); else input.text = value.ToString(); });
            return input;
        }

        private TMP_InputField FloatField(Transform parent, float value, Action<float> changed)
        {
            TMP_InputField input = Input("0", parent, false);
            input.contentType = TMP_InputField.ContentType.DecimalNumber; input.text = value.ToString("0.###");
            input.onEndEdit.AddListener(v => { if (float.TryParse(v, out float parsed)) changed(parsed); else input.text = value.ToString("0.###"); });
            return input;
        }

        private void LabeledInt(string label, Transform parent, int value, Action<int> changed)
        {
            RectTransform box = Horizontal(parent, 5); box.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text(label, box, 18, TextAlignmentOptions.MidlineRight).gameObject.AddComponent<LayoutElement>().preferredWidth = 65;
            IntField(box, value, changed).gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        }

        private void LabeledString(string label, Transform parent, string value, Action<string> changed)
        {
            RectTransform box = Horizontal(parent, 5); box.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text(label, box, 18, TextAlignmentOptions.MidlineRight).gameObject.AddComponent<LayoutElement>().preferredWidth = 65;
            TMP_InputField input = Input(label, box, false); input.text = value ?? "";
            input.onValueChanged.AddListener(v => changed(v)); input.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        }

        private void LabeledDigits(string label, Transform parent, string value, Action<string> changed)
        {
            RectTransform box = Horizontal(parent, 5); box.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text(label, box, 18, TextAlignmentOptions.MidlineRight).gameObject.AddComponent<LayoutElement>().preferredWidth = 65;
            TMP_InputField input = Input(label, box, true); input.text = value ?? "";
            input.onValueChanged.AddListener(v => changed(v)); input.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        }

        private TMP_Dropdown Dropdown(Transform parent, IEnumerable<string> values, int selected, Action<string> changed)
        {
            Image image = NewObject<Image>("Dropdown", parent); image.color = fieldColor;
            TMP_Dropdown dropdown = image.gameObject.AddComponent<TMP_Dropdown>();
            dropdown.targetGraphic = image;
            dropdown.interactable = true;
            TMP_Text caption = Text("", dropdown.transform, 18, TextAlignmentOptions.MidlineLeft);
            Stretch(caption.rectTransform, 10, 32, 3, 3); dropdown.captionText = caption;
            TMP_Text arrow = Text("▼", dropdown.transform, 15, TextAlignmentOptions.Center);
            arrow.rectTransform.anchorMin = new Vector2(1, 0);
            arrow.rectTransform.anchorMax = Vector2.one;
            arrow.rectTransform.pivot = new Vector2(1, .5f);
            arrow.rectTransform.sizeDelta = new Vector2(30, 0);
            arrow.rectTransform.anchoredPosition = Vector2.zero;
            BuildDropdownTemplate(dropdown);
            dropdown.options = values.Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            dropdown.value = Mathf.Clamp(selected, 0, Math.Max(0, dropdown.options.Count - 1));
            dropdown.RefreshShownValue();
            dropdown.onValueChanged.AddListener(i => { if (i >= 0 && i < dropdown.options.Count) changed(dropdown.options[i].text); });
            return dropdown;
        }

        private void BuildDropdownTemplate(TMP_Dropdown dropdown)
        {
            RectTransform template = Panel("Template", dropdown.transform, new Color(.11f, .13f, .17f, 1));
            template.anchorMin = new Vector2(0, 0);
            template.anchorMax = new Vector2(1, 0);
            template.pivot = new Vector2(.5f, 1);
            template.anchoredPosition = Vector2.zero;
            template.sizeDelta = new Vector2(0, 260);
            template.gameObject.AddComponent<Canvas>().overrideSorting = true;
            template.GetComponent<Canvas>().sortingOrder = 100;
            CanvasGroup templateGroup = template.gameObject.AddComponent<CanvasGroup>();
            templateGroup.alpha = 1;
            templateGroup.interactable = true;
            templateGroup.blocksRaycasts = true;
            template.gameObject.AddComponent<GraphicRaycaster>();
            ScrollRect scroll = template.gameObject.AddComponent<ScrollRect>();

            RectTransform viewport = Panel("Viewport", template, Color.clear);
            Stretch(viewport, 3, 3, 3, 3);
            viewport.gameObject.AddComponent<RectMask2D>();

            RectTransform content = NewObject<RectTransform>("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(.5f, 1);
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup vertical = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vertical.childControlHeight = true;
            vertical.childControlWidth = true;
            vertical.childForceExpandHeight = false;
            vertical.spacing = 1;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform item = Panel("Item", content, fieldColor);
            item.anchorMin = new Vector2(0, 1);
            item.anchorMax = Vector2.one;
            item.pivot = new Vector2(.5f, 1);
            item.anchoredPosition = Vector2.zero;
            item.sizeDelta = new Vector2(0, 38);
            item.gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
            Toggle toggle = item.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = item.GetComponent<Image>();
            toggle.isOn = false;
            toggle.toggleTransition = Toggle.ToggleTransition.None;
            Image check = NewObject<Image>("Item Checkmark", item);
            check.color = accentColor;
            check.rectTransform.anchorMin = new Vector2(0, .5f);
            check.rectTransform.anchorMax = new Vector2(0, .5f);
            check.rectTransform.pivot = new Vector2(0, .5f);
            check.rectTransform.sizeDelta = new Vector2(12, 24);
            check.rectTransform.anchoredPosition = new Vector2(3, 0);
            toggle.graphic = check;
            TMP_Text itemLabel = Text("Option", item, 18, TextAlignmentOptions.MidlineLeft);
            Stretch(itemLabel.rectTransform, 24, 6, 2, 2);

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.scrollSensitivity = DropdownScrollSensitivity;
            scroll.inertia = true;
            scroll.decelerationRate = .16f;
            dropdown.template = template;
            dropdown.itemText = itemLabel;
            template.gameObject.SetActive(false);
        }

        private Toggle ToggleUI(string label, Transform parent, bool value, Action<bool> changed)
        {
            RectTransform box = Horizontal(parent, 6);
            Image bg = NewObject<Image>("Check", box); bg.color = fieldColor;
            bg.gameObject.AddComponent<LayoutElement>().preferredWidth = 34;
            Image mark = NewObject<Image>("Mark", bg.transform); mark.color = accentColor; Stretch(mark.rectTransform, 6, 6, 6, 6);
            Toggle toggle = box.gameObject.AddComponent<Toggle>(); toggle.targetGraphic = bg; toggle.graphic = mark;
            toggle.isOn = value; toggle.onValueChanged.AddListener(v => changed(v));
            Text(label, box, 18, TextAlignmentOptions.MidlineLeft).gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            return toggle;
        }

        private RectTransform Horizontal(Transform parent, float spacing)
        {
            RectTransform row = NewObject<RectTransform>("Row", parent);
            HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing; layout.childControlWidth = true; layout.childControlHeight = true;
            layout.childForceExpandWidth = false; layout.childForceExpandHeight = true;
            return row;
        }

        private RectTransform Scroll(string name, Transform parent, out ScrollRect scroll)
        {
            Image root = NewObject<Image>(name, parent); root.color = new Color(.07f, .08f, .11f, 1);
            scroll = root.gameObject.AddComponent<ScrollRect>();
            RectTransform viewport = Panel("Viewport", root.transform, Color.clear);
            Stretch(viewport); viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform content = NewObject<RectTransform>("Content", viewport);
            content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(.5f, 1); content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 8, 8); layout.spacing = 7;
            layout.childControlHeight = true; layout.childForceExpandHeight = false;
            ContentSizeFitter fit = content.gameObject.AddComponent<ContentSizeFitter>();
            fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport; scroll.content = content; scroll.horizontal = false;
            scroll.scrollSensitivity = MainScrollSensitivity;
            scroll.inertia = true;
            scroll.decelerationRate = .16f;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.elasticity = .08f;
            return content;
        }

        private static RectTransform Rect(GameObject go) => go.GetComponent<RectTransform>();
        private static void Clear(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--) Destroy(parent.GetChild(i).gameObject);
        }
        private static void Stretch(RectTransform r, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.pivot = new Vector2(.5f, .5f);
            r.offsetMin = new Vector2(left, bottom); r.offsetMax = new Vector2(-right, -top);
            r.localScale = Vector3.one;
        }

        private static void SetOffsets(RectTransform r, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Vector2 pivot)
        {
            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;
            r.pivot = pivot;
            r.offsetMin = offsetMin;
            r.offsetMax = offsetMax;
            r.localScale = Vector3.one;
        }
    }

    public sealed class DayEditorPreviewDrag : MonoBehaviour, IDragHandler
    {
        public Action<Vector2> onDragged;
        public void OnDrag(PointerEventData eventData) => onDragged?.Invoke(eventData.delta);
    }
}
