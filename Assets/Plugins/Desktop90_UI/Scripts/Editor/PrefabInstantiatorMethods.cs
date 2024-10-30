using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace float_oat.Desktop90.EditorTools
{
    /// <summary>
    /// Contains methods for PrefabInstantiationMenu to call
    /// </summary>
    public sealed class PrefabInstantiatorMethods : Editor
    {
        /// <summary>
        /// Adds a UI element prefab into the scene. Adds a Canvas and EventSystem if needed
        /// </summary>
        /// <param name="prefabPath">Path to the prefab to create</param>
        public static void CreateUIElementPrefab(string prefabPath)
        {
            if (prefabPath == null)
            {
                throw new System.ArgumentNullException(nameof(prefabPath), "Prefab path cannot be null");
            }

            // create the new UI element
            Object UIElementPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            if (UIElementPrefab == null)
            {
                throw new System.InvalidOperationException("Missing prefab " + prefabPath + ". It may have been renamed, moved or deleted");
            }
            GameObject UIElementGO = (GameObject)PrefabUtility.InstantiatePrefab(UIElementPrefab);

            // add a Canvas and EventSystem if they are needed.
            // this code aims to mimick the defaut behavior of adding UI elements in the heirarchy
            Transform UIElementParent = FindOrCreateTransformToAddUIElementTo();
            CreateEventSystemIfDoesntExist();

            // parent the new UI element to the canvas and select it
            UIElementGO.transform.SetParent(UIElementParent, false);
            Selection.activeGameObject = UIElementGO;

            // register undo so undoing reverts this
            Undo.RegisterCreatedObjectUndo(UIElementGO, "Create " + UIElementPrefab.name);
        }

        /// <summary>
        /// Adds a UI container prefab into the scene. Adds the EventSystem if needed
        /// </summary>
        /// <param name="prefabPath"></param>
        public static void CreateUIContainerPrefab(string prefabPath)
        {
            if (prefabPath == null)
            {
                throw new System.ArgumentNullException(nameof(prefabPath), "Prefab path cannot be null");
            }

            // create the new UI container
            Object UIContainerPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            if (UIContainerPrefab == null)
            {
                throw new System.InvalidOperationException("Missing prefab " + prefabPath);
            }
            GameObject UIContainerGO = (GameObject)PrefabUtility.InstantiatePrefab(UIContainerPrefab);

            // add the EventSystem if needed
            CreateEventSystemIfDoesntExist();

            // parent new object to selected GameObject and select it
            if (Selection.activeTransform != null)
            {
                UIContainerGO.transform.SetParent(Selection.activeTransform, false);
            }
            Selection.activeGameObject = UIContainerGO;

            // register undo so undoing reverts this
            Undo.RegisterCreatedObjectUndo(UIContainerGO, "Create " + UIContainerPrefab.name);
        }

        public static void CreateCustomCursorEventSystem()
        {
            // check for existing EventSystem
            // if exists, replace StandaloneInputModule with the custom one, switching over all the settings
            EventSystem eventSystemInScene = FindObjectOfType<EventSystem>();
            if (eventSystemInScene == null)
            {
                // no event system in scene, so add it and give it the CustomCursor input module
                GameObject newEventSystem = new GameObject("CustomCursorEventSystem", typeof(EventSystem), typeof(CustomCursorStandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(newEventSystem, "Create CustomCursorEventSystem");
            }
            else
            {
                CustomCursorStandaloneInputModule existingCustomCusorInputModule = eventSystemInScene.GetComponent<CustomCursorStandaloneInputModule>();
                if (existingCustomCusorInputModule == null)
                {
                    StandaloneInputModule standaloneInputModule = eventSystemInScene.GetComponent<StandaloneInputModule>();
                    if (standaloneInputModule == null)
                    {
                        // no StandaloneInputModule on EventSystem, so ask to add the CustomCursor version to it
                        if (EditorUtility.DisplayDialog("Add CustomCursorStandaloneInputModule to EventSystem", "To use custom cursors, a CustomCursorStandaloneInputModule component will be added to the EventSystem in the scene. Proceed?", "Yes", "No"))
                        {
                            Undo.AddComponent<CustomCursorStandaloneInputModule>(eventSystemInScene.gameObject);
                        }
                    }
                    else
                    {
                        // ask to replace the existing StandaloneInputModule with the CustomCursor version of it
                        if (EditorUtility.DisplayDialog("Replace StandaloneInputModule with CustomCursorStandaloneInputModule", "To use custom cursors, the existing StandaloneInputModule in the scene will be replaced with a CustomCursorStandaloneInputModule component. Proceed?", "Yes", "No"))
                        {
                            var customCursorInputModule = Undo.AddComponent<CustomCursorStandaloneInputModule>(eventSystemInScene.gameObject);

                            // copy properties from old component to new one
                            customCursorInputModule.horizontalAxis = standaloneInputModule.horizontalAxis;
                            customCursorInputModule.verticalAxis = standaloneInputModule.verticalAxis;
                            customCursorInputModule.submitButton = standaloneInputModule.submitButton;
                            customCursorInputModule.cancelButton = standaloneInputModule.cancelButton;
                            customCursorInputModule.inputActionsPerSecond = standaloneInputModule.inputActionsPerSecond;
                            customCursorInputModule.repeatDelay = standaloneInputModule.repeatDelay;
                            customCursorInputModule.forceModuleActive = standaloneInputModule.forceModuleActive;

                            Undo.DestroyObjectImmediate(standaloneInputModule);
                        }
                    }
                }
                Selection.activeTransform = eventSystemInScene.transform;
            }
        }

        /// <summary>
        /// Creates an EventSystem if one doesn't exist in the scene
        /// </summary>
        private static void CreateEventSystemIfDoesntExist()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        /// <summary>
        /// Returns the Transform to parent new UI elements to
        /// <para>Finds an existing Canvas or creates a new one depending on the current selection and if a Canvas already exists</para>
        /// </summary>
        /// <returns>Transform component</returns>
        private static Transform FindOrCreateTransformToAddUIElementTo()
        {
            if (Selection.activeTransform == null)
            {
                // Add the UI element to a canvas in the scene if it exists, or create a new canvas
                Canvas canvasInScene = FindObjectOfType<Canvas>();
                if (canvasInScene != null)
                {
                    return canvasInScene.transform;
                }

                return CreateCanvas().transform;
            }
            else
            {
                // Add the UI element to the selected object if it has a Canvas higher up in the hierarchy, or create a canvas under it if it doesn't
                if (HasCanvasAncestor(Selection.activeTransform))
                {
                    return Selection.activeTransform;
                }

                Canvas newCanvas = CreateCanvas();
                newCanvas.transform.SetParent(Selection.activeTransform);
                return newCanvas.transform;
            }
        }

        private static bool HasCanvasAncestor(Transform transform)
        {
            Transform tempTransform = transform;
            while (tempTransform != null)
            {
                if (tempTransform.GetComponent<Canvas>() != null)
                {
                    return true;
                }
                tempTransform = tempTransform.parent;
            }
            return false;
        }

        /// <summary>
        /// Create a new Canvas object and configures it the same way as the default Canvas
        /// </summary>
        /// <returns>A new Canvas component that's attached to a new GameObject</returns>
        private static Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.layer = 5; // UI layer

            Canvas canvasComponent = canvasGO.GetComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;

            return canvasComponent;
        }
    }
}
