using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorGame.Documents
{
    /// <summary>과제와 신규 또는 복원 상태를 받아 문서 화면을 생성하는 씬 진입점이다.</summary>
    public sealed class DocumentDesktopHost : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private DocumentDesktopSkin _skin = new DocumentDesktopSkin();
        [SerializeField] private bool _preview;
        [SerializeField, Min(0)] private int _previewStage;
        [SerializeField] private DocumentMode _previewMode;
        public DocumentDesktop Desktop { get; private set; }
        private readonly Dictionary<DocumentMode, DocumentPlayState> _previewStates = new Dictionary<DocumentMode, DocumentPlayState>();
        private GameObject _ownedEventSystem;
        private DocumentMode? _displayedPreview;

        // 설정된 모드와 스테이지의 미리보기를 시작한다.
        private void Start()
        {
            if (_preview) ShowPreview(_previewMode);
        }

        /// <summary>
        /// 모드 컨트롤러가 콘텐츠와 업무 상태를 결정한 뒤 호출한다.
        /// 호스트는 업무 완료, 평판 및 저장 데이터를 변경하지 않는다.
        /// </summary>
        public void Show(DocumentAssignment content, DocumentPlayState state, IDictionary<string, Sprite> portraits = null)
        {
            if (_font == null) throw new InvalidOperationException("A desktop TMP font must be assigned.");
            if (Desktop != null) throw new InvalidOperationException("Close the current desktop and retain its snapshot first.");
            Desktop = CreateDesktop(content, state, portraits);
            EnsureEventSystem();
        }

        // 문서 화면을 생성하고 초기화 실패 시 생성한 오브젝트를 해제한다.
        private DocumentDesktop CreateDesktop(DocumentAssignment content, DocumentPlayState state,
            IDictionary<string, Sprite> portraits)
        {
            var view = new GameObject("Document Desktop", typeof(RectTransform));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(view, gameObject.scene);
            var desktop = view.AddComponent<DocumentDesktop>();
            try
            {
                desktop.Initialize(content, state, _font, portraits, _skin);
                return desktop;
            }
            catch
            {
                view.SetActive(false);
                Destroy(view);
                throw;
            }
        }

        // 씬에 입력 이벤트 시스템이 없으면 생성한다.
        private void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                _ownedEventSystem = new GameObject("Desktop EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_ownedEventSystem, gameObject.scene);
            }
        }

        public DocumentPlayState Close()
        {
            DocumentPlayState snapshot = Desktop == null ? null : Desktop.GetStateSnapshot();
            ReleaseView();
            return snapshot;
        }

        // 호스트가 소유한 문서 화면과 입력 시스템을 해제한다.
        private void ReleaseView()
        {
            if (Desktop != null)
            {
                Desktop.gameObject.SetActive(false);
                Destroy(Desktop.gameObject);
                Desktop = null;
            }
            if (_ownedEventSystem != null)
            {
                _ownedEventSystem.SetActive(false);
                Destroy(_ownedEventSystem);
                _ownedEventSystem = null;
            }
        }

        // 진술 대조 모드의 미리보기를 연다.
        [ContextMenu("Preview/Statement Check")]
        private void PreviewStatements() { ShowPreview(DocumentMode.StatementCheck); }

        // 프로필 조작 모드의 미리보기를 연다.
        [ContextMenu("Preview/Forgery")]
        private void PreviewForgery() { ShowPreview(DocumentMode.Forgery); }

        // 서비스를 변경하지 않고 미리보기 과제와 상태를 표시한다.
        private void ShowPreview(DocumentMode mode)
        {
            if (!Application.isPlaying || !_preview) return;
            string workCode = DocumentContract.GetWorkCode(mode);
            DocumentStageData stageData = DocumentStageRepository.Load(workCode, _previewStage);
            if (stageData == null) throw new InvalidOperationException("Missing preview stage data.");
            stageData.ValidateForExecution();
            DocumentAssignment content = stageData.Assignment;
            if (Desktop != null && _displayedPreview.HasValue)
                _previewStates[_displayedPreview.Value] = Close();
            if (!_previewStates.TryGetValue(mode, out var state))
                state = DocumentContract.Begin(content, DocumentContract.Key("desktop-preview", content.Id));
            Show(content, state, DocumentPortraitLoader.Load(content));
            _displayedPreview = mode;
            // 미리보기는 판정 이벤트를 연결하거나 게임 서비스에 값을 쓰지 않는다.
        }

        // 호스트가 소유한 화면과 입력 시스템을 해제한다.
        private void OnDestroy() { ReleaseView(); }
    }
}
