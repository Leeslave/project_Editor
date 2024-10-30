using UnityEditor;

namespace float_oat.Desktop90.EditorTools
{
    /// <summary>
    /// Provides the menu items for the 'GameObject -> UI - 90's desktop theme' menu
    /// </summary>
    public class PrefabInstantiatorMenu : Editor
    {
        private static readonly string PREFABS_PATH = "Assets/Desktop90_UI/Prefabs/UIElements/";
        private static readonly string PREFABS_EXTENSION = ".prefab";

        [MenuItem("GameObject/UI - 90's desktop theme/Text", false, 10)]
        public static void CreateText() => CreateUIElementPrefab("D90 Text");

        [MenuItem("GameObject/UI - 90's desktop theme/Button", false, 10)]
        public static void CreateButton() => CreateUIElementPrefab("D90 Button");

        [MenuItem("GameObject/UI - 90's desktop theme/Toggle", false, 10)]
        public static void CreateToggle() => CreateUIElementPrefab("D90 Toggle");

        [MenuItem("GameObject/UI - 90's desktop theme/Slider", false, 10)]
        public static void CreateSlider() => CreateUIElementPrefab("D90 Slider");

        [MenuItem("GameObject/UI - 90's desktop theme/Dropdown", false, 10)]
        public static void CreateDropdown() => CreateUIElementPrefab("D90 Dropdown");

        [MenuItem("GameObject/UI - 90's desktop theme/Scrollbar", false, 10)]
        public static void CreateScrollbar() => CreateUIElementPrefab("D90 Scrollbar");

        [MenuItem("GameObject/UI - 90's desktop theme/Input Field", false, 10)]
        public static void CreateInputField() => CreateUIElementPrefab("D90 Input Field");

        [MenuItem("GameObject/UI - 90's desktop theme/Input Field With Label", false, 10)]
        public static void CreateInputFieldWithLabel() => CreateUIElementPrefab("D90 Input Field With Label");

        [MenuItem("GameObject/UI - 90's desktop theme/Scroll View", false, 10)]
        public static void CreateScrollView() => CreateUIElementPrefab("D90 Scroll View");

        [MenuItem("GameObject/UI - 90's desktop theme/Window/Window", false, 21)]
        public static void CreateWindow() => CreateUIElementPrefab("Window");

        [MenuItem("GameObject/UI - 90's desktop theme/Window/Alert Window", false, 21)]
        public static void CreateAlertWindow() => CreateUIElementPrefab("Alert Window");

        [MenuItem("GameObject/UI - 90's desktop theme/Window/Question Window", false, 21)]
        public static void CreateQuestionWindow() => CreateUIElementPrefab("Question Window");

        [MenuItem("GameObject/UI - 90's desktop theme/Window/Info Window", false, 21)]
        public static void CreateInfoWindow() => CreateUIElementPrefab("Info Window");

        [MenuItem("GameObject/UI - 90's desktop theme/Desktop Icon", false, 21)]
        public static void CreateDesktopIcon() => CreateUIElementPrefab("DesktopIcon");

        [MenuItem("GameObject/UI - 90's desktop theme/Desktop", false, 21)]
        public static void CreateDesktop() => CreateUIContainerPrefab("Desktop");

        [MenuItem("GameObject/UI - 90's desktop theme/Pause Menu", false, 21)]
        public static void CreatePauseScreen() => CreateUIContainerPrefab("Pause Menu");

        [MenuItem("GameObject/UI - 90's desktop theme/Custom Cursor Event System", false, 31)]
        public static void CreateCursorEventSystem() => PrefabInstantiatorMethods.CreateCustomCursorEventSystem();

        private static void CreateUIElementPrefab(string prefabName)
            => PrefabInstantiatorMethods.CreateUIElementPrefab(PREFABS_PATH + prefabName + PREFABS_EXTENSION);

        private static void CreateUIContainerPrefab(string prefabName)
            => PrefabInstantiatorMethods.CreateUIContainerPrefab(PREFABS_PATH + prefabName + PREFABS_EXTENSION);
    }
}
