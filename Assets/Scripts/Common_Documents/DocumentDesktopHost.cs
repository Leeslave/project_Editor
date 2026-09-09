using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorGame.Documents
{
    /// <summary>Scene entry point. Runtime callers supply an explicit assignment and restored/new work state.</summary>
    public sealed class DocumentDesktopHost : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset font;
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
        /// Called by the mode controller after selecting content and resolving its work-instance state.
        /// The host never changes daily work completion, reputation or save data.
        /// </summary>
        public void Show(DocumentAssignment content, DocumentPlayState state, IDictionary<string, Sprite> portraits = null)
        {
            if (font == null) throw new InvalidOperationException("A desktop TMP font must be assigned.");
            if (Desktop != null) throw new InvalidOperationException("Close the current desktop and retain its snapshot first.");
            var view = new GameObject("Document Desktop", typeof(RectTransform));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(view, gameObject.scene);
            var desktop = view.AddComponent<DocumentDesktop>();
            try { desktop.Initialize(content, state, font, portraits); }
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

        // Scene teardown must not reactivate legacy scripts or serialize a state that will be discarded.
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
            // Preview does not attach scoring/submission listeners or write to gameplay services.
        }

        private void OnDestroy() { ReleaseView(false); }
    }
}
