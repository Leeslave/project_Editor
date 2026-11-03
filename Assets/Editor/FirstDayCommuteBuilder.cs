using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class FirstDayCommuteBuilder
{
    public const string ScenePath = "Assets/Scenes/MainWorld.unity";

    [MenuItem("Tools/Tutorial/Build First Day Commute %#F8")]
    public static void Build()
    {
        SessionState.SetString("DayEditorSceneBuilder.Version", "DayEditorScene.v2:" +
            AssetDatabase.AssetPathToGUID("Assets/Editor/DayEditor.unity"));
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
        var existing = all.Select(t => t.GetComponent<FirstDayCommuteTutorial>()).FirstOrDefault(t => t != null);
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
        all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
        var doors = all.Select(t => t.GetComponent<Door>()).Where(d => d != null).ToArray();
        foreach (var door in doors)
            Debug.Log($"COMMUTE_DOOR: {door.transform.parent.name}/{door.name} at {door.transform.position} -> {door.destination}/{door.position}");
        var host = new GameObject("First Day Commute Tutorial").AddComponent<FirstDayCommuteTutorial>();
        host.gameObject.AddComponent<FirstDayWorkstationTutorial>().commute = host;
        host.movementToggle = all.Select(t => t.GetComponent<Button>()).First(b => b != null && b.name == "MoveLocationActive");
        host.toCompanyStreet = doors.Where(d => d.destination == World.Street && d.position == 1)
            .OrderBy(d => Mathf.Abs(d.transform.position.x)).First();
        host.companyEntrance = doors.Where(d => d.destination == World.Hallway && d.position == 0)
            .OrderBy(d => Mathf.Abs(d.transform.position.x - 100)).First();
        // 배경 전체 대신 중앙 회사 건물의 전면을 조명한다. 렌더러가 없는 월드 사각형이다.
        var companyFocus = new GameObject("Company Building Focus", typeof(RectTransform));
        companyFocus.transform.SetParent(host.transform, false);
        companyFocus.transform.position = host.companyEntrance.transform.position + new Vector3(0, 18, 0);
        ((RectTransform)companyFocus.transform).sizeDelta = new Vector2(36, 26);
        host.companyBuilding = companyFocus;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Common_Documents/Tutorial_Shader/TutorialSet.prefab");
        var overlay = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        overlay.name = "Commute Spotlight";
        overlay.transform.SetParent(host.transform, false);
        overlay.transform.localScale = Vector3.one;
        host.spotlight = overlay.GetComponent<TutorialController>();
        host.spotlight.AutoStart = false;
        overlay.GetComponent<Canvas>().sortingOrder = 90;
        var scaler = overlay.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1200, 900);
        scaler.matchWidthOrHeight = .5f;
        var text = overlay.GetComponentInChildren<TMP_Text>(true);
        text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/neodgm SDF.asset");
        text.fontSize = 25;
        text.enableAutoSizing = false;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.raycastTarget = false;
        var card = (RectTransform)text.transform.parent;
        foreach (var fitter in card.GetComponentsInChildren<ContentSizeFitter>(true)) UnityEngine.Object.DestroyImmediate(fitter);
        foreach (var layout in card.GetComponentsInChildren<LayoutGroup>(true)) UnityEngine.Object.DestroyImmediate(layout);
        card.anchorMin = card.anchorMax = card.pivot = new Vector2(.5f, .5f);
        card.sizeDelta = new Vector2(640, 240);
        text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = new Vector2(22, 20); text.rectTransform.offsetMax = new Vector2(-22, -20);
        foreach (var image in card.GetComponentsInChildren<Image>(true)) image.raycastTarget = false;
        overlay.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Could not save commute tutorial");
        Debug.Log("COMMUTE_BUILD_OK");
    }
}
