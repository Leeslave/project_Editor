using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorGame.WorkIntegration
{
    /// <summary>접근 토큰 하나와 출구로 구성한 짧은 미로 인증. 업무 완료는 호스트만 처리한다.</summary>
    public sealed class MazeAuthenticationView : MonoBehaviour
    {
        private ShortMaze maze;
        private WorkTestUI ui;
        private Action<bool> finished;
        private Action cancel;
        private RectTransform canvas, screen, cursor;
        private GameObject token;
        private TMP_Text timer, progress;
        private float nextMove;
        private bool reported;
        private bool tutorialActive, approvalRead;
        private MazeWorkLesson lesson;
        private WorkTutorialBanner banner;
        private RectTransform exit;
        private TMP_FontAsset tutorialFont;
        private TMP_Text heading;
        private const float CellSize = 80;

        public void Configure(TMP_FontAsset font, int layout, Action<bool> completed, Action cancelled, bool showTutorial = false)
        {
            maze = new ShortMaze(layout);
            ui = new WorkTestUI(font);
            finished = completed;
            cancel = cancelled;
            tutorialFont = font;
            tutorialActive = showTutorial;
            lesson = new MazeWorkLesson(maze);
        }

        private void Start()
        {
            if (maze == null) { enabled = false; return; }
            canvas = ui.Canvas(transform, "ASTRA route verification", 100);
            screen = ui.Rect(canvas, "4 by 3 route", 0, 0, 1440, 1080);
            screen.anchorMin = screen.anchorMax = screen.pivot = new Vector2(.5f, .5f);
            screen.anchoredPosition = Vector2.zero;
            ui.Panel(screen, "Background", 0, 0, 1440, 1080, WorkTestUI.Background);
            heading = ui.Text(screen, "ASTRA SECURITY SYSTEM", 50, 30, 1300, 60, 36);
            ui.Text(screen, "접근 경로 검증 / MAZE", 50, 120, 1250, 55, 30);
            for (int y = 0; y < maze.Height; y++)
                for (int x = 0; x < maze.Width; x++)
                {
                    char cell = maze.Cell(x, y);
                    var rect = ui.Panel(screen, "Cell " + x + "," + y, 50 + x * CellSize,
                        200 + y * CellSize, CellSize - 3, CellSize - 3,
                        cell == '#' ? new Color32(49, 69, 56, 255) : new Color32(17, 30, 23, 255));
                    if (cell == 'K') token = ui.Text(rect, "KEY", 5, 22, 70, 35, 25, new Color32(239, 197, 93, 255)).gameObject;
                    if (cell == 'E') { exit = rect; ui.Text(rect, "EXIT", 5, 22, 70, 35, 23); }
                }
            cursor = ui.Panel(screen, "Access cursor", 0, 0, 28, 28, WorkTestUI.Green);
            ui.Text(screen, "인증 절차", 990, 215, 400, 55, 30);
            ui.Text(screen, "01 접근 토큰 확보\n\n02 승인 지점 이동", 990, 300, 400, 150, 25);
            progress = ui.Text(screen, "", 990, 485, 400, 110, 27);
            timer = ui.Text(screen, "", 990, 625, 400, 90, 30);
            ui.Text(screen, "방향키 또는 WASD로 이동\n토큰 확보 후 EXIT로 이동하세요.", 50, 950, 880, 80, 24);
            ui.Button(screen, "위", 1090, 770, 90, 60, () => Move(0, -1));
            ui.Button(screen, "왼쪽", 990, 840, 90, 60, () => Move(-1, 0));
            ui.Button(screen, "아래", 1090, 840, 90, 60, () => Move(0, 1));
            ui.Button(screen, "오른쪽", 1190, 840, 100, 60, () => Move(1, 0));
            ui.Button(screen, "인증 중단", 990, 945, 300, 65, () => cancel?.Invoke());
            banner = gameObject.AddComponent<WorkTutorialBanner>();
            banner.Initialize(tutorialFont, ContinueTutorial, ToggleTutorial);
            Refresh();
        }

        private void LateUpdate()
        {
            if (canvas == null) return;
            float scale = Mathf.Min(canvas.rect.width / 1440f, canvas.rect.height / 1080f);
            screen.localScale = new Vector3(scale, scale, 1);
        }

        private void Update()
        {
            if (maze == null || reported) return;
            RefreshTutorial();
            if (tutorialActive && !lesson.IntroRead) return;
            if (!tutorialActive) maze.Tick(Time.unscaledDeltaTime);
            if (maze.Failed) { reported = true; finished?.Invoke(false); return; }
            if (Time.unscaledTime >= nextMove)
            {
                int dx = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? -1 :
                    Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1 : 0;
                int dy = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? -1 :
                    Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? 1 : 0;
                if (dx != 0 || dy != 0)
                {
                    Move(dx, dx != 0 ? 0 : dy);
                    nextMove = Time.unscaledTime + .22f;
                }
            }
            Refresh();
        }

        private void Move(int dx, int dy)
        {
            if (reported || (tutorialActive && !lesson.IntroRead)) return;
            maze.Move(dx, dy);
            Refresh();
            if (!maze.Succeeded) return;
            reported = true;
            StartCoroutine(Approve());
        }

        private IEnumerator Approve()
        {
            progress.text = "ACCESS GRANTED\n접근이 승인되었습니다.";
            RefreshTutorial();
            while (tutorialActive && !approvalRead) yield return null;
            yield return new WaitForSecondsRealtime(.8f);
            finished?.Invoke(true);
        }

        private void Refresh()
        {
            if (cursor == null) return;
            cursor.anchoredPosition = new Vector2(50 + maze.X * CellSize + 24, -(200 + maze.Y * CellSize + 24));
            token.SetActive(!maze.HasToken);
            progress.text = maze.Succeeded ? "ACCESS GRANTED\n접근이 승인되었습니다." :
                maze.HasToken ? "접근 토큰 1 / 1\n승인 지점으로 이동하십시오." : "접근 토큰 0 / 1";
            timer.text = tutorialActive ? "튜토리얼\n시간 제한 없음" : "남은 시간\n" + Mathf.CeilToInt(maze.Remaining) + "초";
            RefreshTutorial();
        }

        private void ContinueTutorial()
        {
            if (lesson.Current(maze) == MazeLessonStage.Introduction) lesson.IntroRead = true;
            if (lesson.Current(maze) == MazeLessonStage.Approved) approvalRead = true;
            Refresh();
        }

        private void ToggleTutorial()
        {
            tutorialActive = !tutorialActive;
            if (tutorialActive) lesson.IntroRead = true;
            Refresh();
        }

        private void RefreshTutorial()
        {
            if (banner == null) return;
            heading.gameObject.SetActive(!tutorialActive);
            var stage = lesson.Current(maze);
            string title, text;
            RectTransform target = null;
            switch (stage)
            {
                case MazeLessonStage.Introduction:
                    title = "ASTRA 01 / 기밀 접근 인증";
                    text = "미로에서 토큰을 확보하고 EXIT로 이동합니다. 안내 중에는 시간이 흐르지 않습니다. '계속'을 누르세요.";
                    break;
                case MazeLessonStage.Move:
                    title = "ASTRA 02 / 접속 커서 이동";
                    text = "방향키 또는 WASD로 밝은 커서를 이동하세요. 벽은 통과할 수 없습니다. 화면의 방향 버튼도 사용할 수 있습니다.";
                    target = cursor;
                    break;
                case MazeLessonStage.Token:
                    title = "ASTRA 03 / 접근 토큰 확보";
                    text = "길을 따라 KEY 표시까지 이동하세요. 토큰을 얻기 전에는 EXIT에 도착해도 인증되지 않습니다.";
                    target = token.GetComponent<RectTransform>();
                    break;
                case MazeLessonStage.Exit:
                    title = "ASTRA 04 / 승인 지점 이동";
                    text = "토큰을 확보했습니다. EXIT까지 직접 이동해 경로 검증을 마치세요.";
                    target = exit;
                    break;
                default:
                    title = "ASTRA 05 / 현재 업무 접근 승인";
                    text = "인증은 업무 완료가 아닙니다. '업무 열기'를 누르면 선택한 업무로 연결되며, 다음 기밀 업무는 다시 인증해야 합니다.";
                    break;
            }
            banner.Show(tutorialActive, title, text, stage == MazeLessonStage.Introduction || stage == MazeLessonStage.Approved,
                target, stage == MazeLessonStage.Approved ? "업무 열기" : "계속");
        }
    }
}
