using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorGame.AdfgvxRemake;
using EditorGame.Documents;
using GameService;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace EditorGame.WorkIntegration
{
    /// <summary>TEST 씬의 업무·인증 호스트. 실제 플레이어 저장 파일을 읽거나 쓰지 않는다.</summary>
    [DefaultExecutionOrder(-13000)]
    public sealed class WorkIntegrationTest : MonoBehaviour, IWorkExecutionService
    {
        public static WorkIntegrationTest Instance { get; private set; }
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private WorkAssignment[] assignments =
        {
            new WorkAssignment { Id = "normal-document", Title = "일반 문서 대조", Kind = WorkKind.Document },
            new WorkAssignment { Id = "secret-document", Title = "기밀 문서 수정", Kind = WorkKind.Document,
                Security = WorkSecurity.Confidential },
            new WorkAssignment { Id = "normal-cipher", Title = "일반 암호해독", Kind = WorkKind.ADFGVX, Stage = 6 },
            new WorkAssignment { Id = "secret-cipher", Title = "기밀 암호해독", Kind = WorkKind.ADFGVX,
                Security = WorkSecurity.Confidential, Stage = 12, MazeLayout = 1 }
        };
        private readonly WorkExecution execution = new WorkExecution();
        private DirectPlaySaveService save;
        private WorkTestUI ui;
        private RectTransform canvas, screen;
        private GameObject page;
        private Scene contentScene;
        private string expectedScene, loadError, notice = "업무를 선택하세요. 기밀 업무에는 미로 인증이 필요합니다.";
        private bool owner, busy;
        private bool tutorialEnabled = true, introductionDone;
        private int introductionPage;
        private readonly List<string> results = new List<string>();
        public TMP_FontAsset Font => font;
        public string CurrentExecutionId => execution.ExecutionId;
        public string CurrentWorkCode => execution.Current?.WorkCode;
        public bool isScreenOn { get; set; }
        public event Action OnWorkClear;

        private void Awake()
        {
            if (Instance != null) { enabled = false; return; }
            if (GameSystem.GetService<IWorkService>() != null || GameSystem.SaveService != null)
            {
                Debug.LogError("업무 TEST는 MainController 없이 단독으로 열어 주세요.", this);
                enabled = false;
                return;
            }
            owner = true;
            Instance = this;
            GameSystem.RegisterService<IWorkService>(this);
            save = gameObject.AddComponent<DirectPlaySaveService>();
            var data = new DailyData { date = new Date(2000, 1, 1) };
            foreach (var assignment in assignments) data.workList.Add(new Work(assignment.WorkCode, assignment.Stage));
            save.Configure(0, 0, data);
            save.Init();
            save.SelectDay(0, 0);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (!owner) return;
            if (font == null) font = TMP_Settings.defaultFontAsset;
            ui = new WorkTestUI(font);
            canvas = ui.Canvas(transform, "Work integration TEST", 2000);
            screen = ui.Rect(canvas, "4 by 3 work screen", 0, 0, 1440, 1080);
            screen.anchorMin = screen.anchorMax = screen.pivot = new Vector2(.5f, .5f);
            screen.anchoredPosition = Vector2.zero;
            if (EventSystem.current == null)
                new GameObject("Work TEST EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).transform.SetParent(transform);
            Render();
        }

        private void LateUpdate()
        {
            if (canvas == null) return;
            float scale = Mathf.Min(canvas.rect.width / 1440f, canvas.rect.height / 1080f);
            screen.localScale = new Vector3(scale, scale, 1);
        }

        private void Update()
        {
            if (owner && !busy && (execution.Phase == ExecutionPhase.Working ||
                execution.Phase == ExecutionPhase.Authenticating || execution.Phase == ExecutionPhase.Completed) &&
                Input.GetKeyDown(KeyCode.F10))
                ReturnToWorkList();
        }

        private void Render()
        {
            if (page != null) { page.SetActive(false); Destroy(page); }
            page = ui.Rect(screen, "Work page", 0, 0, 1440, 1080).gameObject;
            if (execution.Phase == ExecutionPhase.Working || execution.Phase == ExecutionPhase.Completed ||
                execution.Phase == ExecutionPhase.Authenticating)
            {
                // 각 미니게임의 종료 버튼을 사용한다. 호스트 UI가 게임 입력을 가리지 않는다.
                return;
            }
            ui.Panel(page.transform, "Background", 0, 0, 1440, 1080, WorkTestUI.Background);
            ui.Text(page.transform, "ASTRA / 업무 통합 TEST", 60, 42, 1300, 65, 36);
            ui.Text(page.transform, "NORMAL / CONFIDENTIAL     ·     MAZE ACCESS VERIFICATION", 60, 110, 1300, 45, 23);
            if (busy)
            {
                ui.Text(page.transform, "화면을 준비하고 있습니다...", 90, 420, 1200, 100, 32);
                return;
            }
            if (execution.Phase == ExecutionPhase.Idle)
            {
                if (!introductionDone) { RenderIntroduction(); return; }
                ui.Text(page.transform, notice, 60, 180, 970, 110, 25);
                ui.Button(page.transform, tutorialEnabled ? "튜토리얼 켜짐" : "튜토리얼 켜기", 1090, 180, 260, 55,
                    () => { tutorialEnabled = !tutorialEnabled; Render(); });
                for (int i = 0; i < assignments.Length; i++)
                {
                    var task = assignments[i];
                    bool done = execution.IsCompleted(task.Id);
                    string grade = task.Security == WorkSecurity.Normal ? "일반" : "기밀 / MAZE";
                    ui.Button(page.transform, $"[{grade}]  {task.Title}  {(done ? "· 완료" : "· 실행")}",
                        90, 320 + i * 105, 1260, 78, () => Begin(task), !done);
                }
                ui.Text(page.transform, "제출한 업무 " + assignments.Count(a => execution.IsCompleted(a.Id)) + " / " + assignments.Length +
                    "    ·    테스트 평판 " + save.Renown, 90, 760, 1240, 50, 25);
                ui.Text(page.transform, string.Join("\n", results.TakeLast(3)), 90, 830, 1250, 155, 22);
                ui.Text(page.transform, "테스트 진행은 메모리에서만 유지됩니다. Play를 종료하면 초기화됩니다.", 60, 1010, 1300, 45, 21);
                return;
            }
            bool failed = execution.Phase == ExecutionPhase.AuthenticationFailed;
            ui.Text(page.transform, "CONFIDENTIAL ACCESS REQUESTED", 90, 280, 1250, 65, 36);
            ui.Text(page.transform, execution.Current.Title, 90, 365, 1250, 60, 30);
            ui.Text(page.transform, failed ? "접근 경로 검증에 실패했습니다. 다시 시도할 수 있습니다." :
                "기밀 자료에 접근하려면 지정된 인증 경로를 통과하십시오.", 90, 460, 1250, 100, 27);
            ui.Text(page.transform, "미로에서 접근 토큰 1개를 확보한 뒤 승인 지점으로 이동합니다.\n인증 권한은 현재 선택한 업무에만 적용됩니다.",
                90, 580, 1250, 120, 25);
            ui.Button(page.transform, failed ? "인증 재시도" : "인증 시작", 90, 790, 610, 75, StartAuthentication);
            ui.Button(page.transform, "업무 목록으로", 740, 790, 610, 75, ReturnToWorkList);
        }

        private void RenderIntroduction()
        {
            string[] titles = { "01 / 정보국 업무", "02 / 기밀 접근 절차", "03 / 제출과 복귀" };
            string[] texts =
            {
                "일반·기밀 문서와 암호해독, 총 네 가지 업무를 수행합니다.\n일반 업무는 선택하면 바로 시작합니다.",
                "기밀 업무는 ASTRA의 미로 인증을 먼저 거칩니다.\n토큰 확보 → EXIT 이동 → 선택한 업무 실행 순서입니다.\n인증 성공만으로 업무가 완료되지는 않습니다.",
                "문서는 조사·수정을 마친 뒤 제출하고, 암호는 해독 결과를 제출합니다.\n완료 화면에서 업무 목록으로 돌아오세요.\n중단 버튼이나 F10으로 나가면 미제출 업무는 완료되지 않습니다."
            };
            ui.Text(page.transform, titles[introductionPage], 90, 300, 1250, 75, 36);
            ui.Text(page.transform, texts[introductionPage], 90, 420, 1250, 250, 29);
            ui.Text(page.transform, "각 미니게임에서는 실제 조작에 맞춰 안내합니다. 안내를 꺼도 작업 내용은 유지됩니다.",
                90, 700, 1250, 95, 24);
            ui.Button(page.transform, introductionPage == 2 ? "업무 시작" : "다음", 90, 850, 610, 75, () =>
            {
                if (introductionPage < 2) introductionPage++;
                else introductionDone = true;
                Render();
            });
            ui.Button(page.transform, "안내 없이 시작", 740, 850, 610, 75, () =>
            {
                introductionDone = true;
                tutorialEnabled = false;
                Render();
            });
        }

        private void Begin(WorkAssignment task)
        {
            if (busy || !execution.Begin(task)) return;
            if (execution.Phase == ExecutionPhase.Working) StartCoroutine(OpenContent(task.SceneName));
            else Render();
        }

        private void StartAuthentication()
        {
            if (busy || !execution.StartAuthentication()) return;
            StartCoroutine(OpenContent("Maze_TEST"));
        }

        private IEnumerator OpenContent(string sceneName)
        {
            busy = true;
            expectedScene = sceneName;
            loadError = null;
            Render();
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
                loadError = "빌드 씬 목록에 " + sceneName + " 씬이 없습니다.";
            else
            {
                var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                if (operation == null) loadError = "씬을 열 수 없습니다: " + sceneName;
                else yield return operation;
            }
            expectedScene = null;
            busy = false;
            if (loadError != null)
            {
                notice = loadError;
                yield return CloseContent();
                execution.Return();
            }
            Render();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != expectedScene) return;
            contentScene = scene;
            // 입력은 호스트의 EventSystem 하나만 사용한다.
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var events in root.GetComponentsInChildren<EventSystem>(true)) events.gameObject.SetActive(false);
                foreach (var camera in root.GetComponentsInChildren<Camera>(true)) camera.enabled = false;
                foreach (var listener in root.GetComponentsInChildren<AudioListener>(true)) listener.enabled = false;
            }
            try
            {
                if (scene.name == "Maze_TEST")
                {
                    var maze = FindInScene<MazeAuthenticationView>(scene);
                    string token = execution.ExecutionId;
                    maze.Configure(font, execution.Current.MazeLayout,
                        success => AuthenticationFinished(token, success), ReturnToWorkList, tutorialEnabled);
                    maze.gameObject.SetActive(true);
                }
                else if (scene.name == "ADFGVX_TEST")
                {
                    string token = execution.ExecutionId;
                    string code = CurrentWorkCode;
                    FindInScene<AdfgvxTestController>(scene).ConfigureWork(execution.Current.Stage,
                        execution.Current.Security == WorkSecurity.Confidential ? "기밀 암호해독" : "일반 암호해독",
                        task =>
                        {
                            if (execution.ExecutionId != token || !execution.HasAccess(code)) return;
                            ClearWork(code);
                            results.Add(execution.Current.Title + " : " + task.Result);
                        }, ReturnToWorkList, tutorialEnabled);
                }
                else if (scene.name == "Document_TEST")
                {
                    var host = FindInScene<DocumentDesktopHost>(scene);
                    foreach (var controller in host.GetComponents<DocumentWorkController>()) controller.ConfigureTutorial(font, tutorialEnabled);
                    host.gameObject.SetActive(true);
                }
            }
            catch (Exception exception)
            {
                loadError = "업무 씬 구성 오류: " + exception.Message;
                Debug.LogException(exception, this);
            }
        }

        private static T FindInScene<T>(Scene scene) where T : Component
        {
            return scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<T>(true)).First();
        }

        private void AuthenticationFinished(string token, bool success)
        {
            if (busy || !execution.FinishAuthentication(token, success)) return;
            StartCoroutine(AfterAuthentication(success));
        }

        private IEnumerator AfterAuthentication(bool success)
        {
            busy = true;
            yield return CloseContent();
            busy = false;
            if (success) yield return OpenContent(execution.Current.SceneName);
            else Render();
        }

        public void ReturnToWorkList()
        {
            if (!owner || busy) return;
            StartCoroutine(ReturnRoutine());
        }

        private IEnumerator ReturnRoutine()
        {
            busy = true;
            bool done = execution.Phase == ExecutionPhase.Completed;
            notice = done ? "업무 제출이 완료되었습니다." : "업무를 중단했습니다. 완료로 처리되지 않았습니다.";
            yield return CloseContent();
            execution.Return();
            busy = false;
            Render();
        }

        private IEnumerator CloseContent()
        {
            if (contentScene.IsValid() && contentScene.isLoaded) yield return SceneManager.UnloadSceneAsync(contentScene);
            contentScene = default;
        }

        public void Init(DailyData data) { }
        public List<(string code, string name)> GetList() => assignments.Select(a => (a.WorkCode, a.Title)).ToList();
        public bool TryStartWork(string workCode, out string sceneName)
        {
            sceneName = null;
            var task = assignments.FirstOrDefault(a => a.WorkCode == workCode);
            if (busy || !execution.Begin(task)) return false;
            sceneName = execution.Phase == ExecutionPhase.Working ? task.SceneName : "Maze_TEST";
            return true;
        }
        public bool IsWorkAuthorized(string code) => execution.HasAccess(code);
        public int GetStage(string code) => execution.Current?.WorkCode == code ? execution.Current.Stage : -1;
        public bool IsWorkClear(string code) => assignments.Any(a => a.WorkCode == code) &&
            assignments.Where(a => a.WorkCode == code).All(a => execution.IsCompleted(a.Id));
        public bool IsWorkClear() => assignments.All(a => execution.IsCompleted(a.Id));
        public bool ClearWork(string code)
        {
            if (!execution.Complete(execution.ExecutionId, code)) return IsWorkClear();
            save.RecordWorkCompletion(code, execution.Current.Stage);
            if (execution.Current.Kind == WorkKind.Document) results.Add(execution.Current.Title + " : 제출 완료");
            if (IsWorkClear()) OnWorkClear?.Invoke();
            Render();
            return IsWorkClear();
        }

        private void OnDestroy()
        {
            if (!owner) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (ReferenceEquals(GameSystem.GetService<IWorkService>(), this)) GameSystem.UnRegister<IWorkService>();
            Instance = null;
        }
    }
}
