using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorGame.Documents
{
    /// <summary>과제와 신규 또는 복원 상태를 받아 문서 화면을 생성하는 씬 진입점이다.</summary>
    public sealed class DocumentDesktopHost : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private DocumentDesktopSkin skin = new DocumentDesktopSkin();
        [SerializeField] private GameObject[] legacyRoots = new GameObject[0];
        [SerializeField] private bool preview;
        [SerializeField] private TextAsset previewContent;
        [SerializeField] private DocumentMode previewMode;
        public DocumentDesktop Desktop { get; private set; }
        private readonly Dictionary<GameObject, bool> previousActive = new Dictionary<GameObject, bool>();
        private readonly Dictionary<DocumentMode, DocumentPlayState> previewStates = new Dictionary<DocumentMode, DocumentPlayState>();
        private GameObject ownedEventSystem;
        private DocumentMode? displayedPreview;

        private void Start()
        {
            if (preview) ShowPreview(previewMode);
        }

        /// <summary>
        /// 모드 컨트롤러가 콘텐츠와 업무 상태를 결정한 뒤 호출한다.
        /// 호스트는 업무 완료, 평판 및 저장 데이터를 변경하지 않는다.
        /// </summary>
        public void Show(DocumentAssignment content, DocumentPlayState state, IDictionary<string, Sprite> portraits = null)
        {
            if (font == null) throw new InvalidOperationException("A desktop TMP font must be assigned.");
            if (Desktop != null) throw new InvalidOperationException("Close the current desktop and retain its snapshot first.");
            var view = new GameObject("Document Desktop", typeof(RectTransform));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(view, gameObject.scene);
            var desktop = view.AddComponent<DocumentDesktop>();
            try { desktop.Initialize(content, state, font, portraits, skin); }
            catch { view.SetActive(false); Destroy(view); throw; }
            Desktop = desktop;
            foreach (var legacy in legacyRoots)
            {
                if (legacy == null) continue;
                previousActive[legacy] = legacy.activeSelf;
                legacy.SetActive(false);
            }
            if (EventSystem.current == null)
            {
                ownedEventSystem = new GameObject("Desktop EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(ownedEventSystem, gameObject.scene);
            }
        }

        public DocumentPlayState Close()
        {
            DocumentPlayState snapshot = Desktop == null ? null : Desktop.GetStateSnapshot();
            ReleaseView(true);
            return snapshot;
        }

        // 씬 종료 중에는 레거시 화면을 다시 켜거나 폐기할 상태를 만들지 않는다.
        private void ReleaseView(bool restoreLegacy)
        {
            if (Desktop != null)
            {
                Desktop.gameObject.SetActive(false);
                Destroy(Desktop.gameObject);
                Desktop = null;
            }
            if (restoreLegacy)
                foreach (var entry in previousActive) if (entry.Key != null) entry.Key.SetActive(entry.Value);
            previousActive.Clear();
            if (ownedEventSystem != null)
            {
                ownedEventSystem.SetActive(false);
                Destroy(ownedEventSystem);
                ownedEventSystem = null;
            }
        }

        [ContextMenu("Preview/Statement Check")]
        private void PreviewStatements() { ShowPreview(DocumentMode.StatementCheck); }

        [ContextMenu("Preview/Forgery")]
        private void PreviewForgery() { ShowPreview(DocumentMode.Forgery); }

        private void ShowPreview(DocumentMode mode)
        {
            if (!Application.isPlaying || !preview || previewContent == null) return;
            var assignments = JsonConvert.DeserializeObject<List<DocumentAssignment>>(previewContent.text,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var content = assignments?.SingleOrDefault(a => a.Mode == mode);
            if (content == null) throw new InvalidOperationException("Missing preview assignment.");
            if (Desktop != null && displayedPreview.HasValue)
                previewStates[displayedPreview.Value] = Close();
            if (!previewStates.TryGetValue(mode, out var state))
                state = DocumentContract.Begin(content, DocumentContract.Key("desktop-preview", content.Id));
            Show(content, state);
            displayedPreview = mode;
            // 미리보기는 판정 이벤트를 연결하거나 게임 서비스에 값을 쓰지 않는다.
        }

        private void OnDestroy() { ReleaseView(false); }
    }
}
