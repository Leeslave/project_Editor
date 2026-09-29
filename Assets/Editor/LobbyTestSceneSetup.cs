using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 로비 테스트에 필요한 MainController와 GameStart 씬 구성을 준비합니다.
/// </summary>
public static class LobbyTestSceneSetup
{
    private const string MainControllerPath = "Assets/Scenes/MainController.unity";
    private const string LobbyPath = "Assets/Scenes/Lobby/GameStart.unity";

    [MenuItem("Tools/Test Setup/Open Lobby with MainController", false, 10)]
    public static void OpenLobbyWithMainController()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene mainController = EditorSceneManager.OpenScene(MainControllerPath, OpenSceneMode.Single);
        Scene lobby = EditorSceneManager.OpenScene(LobbyPath, OpenSceneMode.Additive);

        if (!mainController.IsValid() || !lobby.IsValid() || !EditorSceneManager.SetActiveScene(lobby))
        {
            Debug.LogError("로비 테스트 씬 구성을 준비하지 못했습니다.");
            return;
        }

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(LobbyPath);
        SceneView.RepaintAll();
        Debug.Log("로비 테스트 준비 완료: MainController + GameStart (Active)");
    }

    [MenuItem("Tools/Test Setup/Open Lobby with MainController", true)]
    private static bool CanOpenLobbyWithMainController()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling;
    }
}
