using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Builds the Chat hierarchy from code; no legacy Chat prefab dependency.</summary>
public static class ComicChatBuilder
{
    public const string PrefabPath = "Assets/Prefabs/ChatSystem/ComicChatCanvas.prefab";
    private const string LegacyPath = "Assets/Prefabs/ChatSystem/ChatCanvas.prefab";
    private static readonly string[] Scenes = { "Assets/Scenes/MainWorld.unity", "Assets/Editor/ChatEditor.unity" };

    [MenuItem("Tools/Chat/Rebuild Comic Chat Prefab")]
    public static void Rebuild()
    {
        var root = Build();
        try { PrefabUtility.SaveAsPrefabAsset(root, PrefabPath); }
        finally { UnityEngine.Object.DestroyImmediate(root); }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Chat/Replace Legacy Chat In Scenes")]
    public static void Migrate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play Mode first.");
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save open scenes before Chat migration.");
        var setup = EditorSceneManager.GetSceneManagerSetup();
        Rebuild();
        try
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            foreach (string path in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                var oldChats = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Chat>(true)).ToArray();
                foreach (var old in oldChats)
                {
                    if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(old.gameObject) != LegacyPath) continue;
                    var replacement = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                    replacement.transform.SetParent(old.transform.parent, false);
                    replacement.transform.SetSiblingIndex(old.transform.GetSiblingIndex());
                    foreach (var added in PrefabUtility.GetAddedGameObjects(old.gameObject).ToArray())
                    {
                        var child = added.instanceGameObject;
                        string parentPath = AnimationUtility.CalculateTransformPath(child.transform.parent, old.transform);
                        var parent = string.IsNullOrEmpty(parentPath) ? replacement.transform : replacement.transform.Find(parentPath);
                        if (parent == null) throw new InvalidOperationException("Cannot preserve added Chat child: " + parentPath);
                        child.transform.SetParent(parent, false);
                    }
                    foreach (var added in PrefabUtility.GetAddedComponents(old.gameObject))
                    {
                        if (added.instanceComponent.gameObject != old.gameObject) continue;
                        UnityEditorInternal.ComponentUtility.CopyComponent(added.instanceComponent);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(replacement);
                    }
                    RemapReferences(scene, old.gameObject, replacement);
                    UnityEngine.Object.DestroyImmediate(old.gameObject);
                }
                if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Cannot save " + path);
            }
            // Delete only after every scene was saved and no serialized asset references remain.
            string guid = AssetDatabase.AssetPathToGUID(LegacyPath);
            if (!string.IsNullOrEmpty(guid))
            {
                var refs = AssetDatabase.GetAllAssetPaths().Where(p => p != LegacyPath &&
                    (p.EndsWith(".unity") || p.EndsWith(".prefab")) &&
                    System.IO.File.ReadAllText(p).Contains(guid)).ToArray();
                if (refs.Length != 0) throw new InvalidOperationException("Legacy references remain: " + string.Join(", ", refs));
                if (!AssetDatabase.DeleteAsset(LegacyPath)) throw new InvalidOperationException("Cannot remove legacy prefab.");
            }
            Debug.Log("COMIC_CHAT_MIGRATION_COMPLETE");
        }
        finally { if (setup.Length > 0 && setup.All(s => !string.IsNullOrEmpty(s.path))) EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }

    private static void RemapReferences(Scene scene, GameObject old, GameObject replacement)
    {
        foreach (var component in scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Component>(true)))
        {
            if (component == null || component.transform.IsChildOf(old.transform)) continue;
            var so = new SerializedObject(component);
            var it = so.GetIterator();
            while (it.Next(true))
            {
                if (it.propertyType != SerializedPropertyType.ObjectReference) continue;
                var value = it.objectReferenceValue;
                var tr = value is GameObject go ? go.transform : (value as Component)?.transform;
                if (tr == null || (tr != old.transform && !tr.IsChildOf(old.transform))) continue;
                string path = AnimationUtility.CalculateTransformPath(tr, old.transform);
                var target = string.IsNullOrEmpty(path) ? replacement.transform : replacement.transform.Find(path);
                if (target == null)
                {
                    var candidates = replacement.GetComponentsInChildren<Transform>(true).Where(t => t.name == tr.name).ToArray();
                    if (candidates.Length == 1) target = candidates[0];
                }
                if (target == null) throw new InvalidOperationException("Cannot remap Chat reference: " + path);
                it.objectReferenceValue = value is GameObject ? (UnityEngine.Object)target.gameObject : target.GetComponent(value.GetType());
                if (it.objectReferenceValue == null) throw new InvalidOperationException("Cannot remap " + value.GetType());
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static GameObject Build()
    {
        var root = new GameObject("ComicChatCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.layer = 5;
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        root.GetComponent<Canvas>().sortingOrder = 10;
        var chat = root.AddComponent<Chat>();
        var ui = Panel("ChatUI", root.transform); Stretch(ui);
        var backdrop = Panel("Background", ui); Stretch(backdrop);
        backdrop.GetComponent<Image>().color = Color.white;
        var backdropButton = backdrop.gameObject.AddComponent<Button>();
        UnityEventTools.AddPersistentListener(backdropButton.onClick, chat.NextChat);
        var portraits = new List<Image>();
        for (int i = 1; i <= 4; i++) portraits.Add(Panel("Character_" + i, ui).GetComponent<Image>());
        var speech = Panel("Speech", ui); var speechButton = speech.gameObject.AddComponent<Button>();
        UnityEventTools.AddPersistentListener(speechButton.onClick, chat.NextChat);
        var body = Label("Text", speech);
        var talker = Panel("Talker", speech); var name = Label("Name", talker); var info = Label("Info", talker);
        var choices = Panel("ChoicePanel", ui);
        for (int i = 0; i < 3; i++)
        {
            var button = Button("Choice_" + i, choices); Label("Text", button.transform);
            UnityEventTools.AddIntPersistentListener(button.onClick, chat.OnChoicePressed, i);
        }
        var options = Panel("OptionPanel", ui);
        var remind = Button("Remind", options); var skip = Button("Skip", options);
        var history = Panel("RemindPanel", ui); Stretch(history);
        history.GetComponent<Image>().color = new Color(0, 0, 0, .9f);
        var viewport = Panel("Viewport", history); Stretch(viewport); viewport.offsetMin = new Vector2(80, 70); viewport.offsetMax = new Vector2(-80, -140);
        viewport.gameObject.AddComponent<RectMask2D>();
        var content = Rect("Content", viewport); content.anchorMin = new Vector2(0, 1); content.anchorMax = Vector2.one; content.pivot = new Vector2(.5f, 1); content.sizeDelta = Vector2.zero;
        var group = content.gameObject.AddComponent<VerticalLayoutGroup>(); group.childControlHeight = false; group.childForceExpandHeight = false; group.spacing = 20;
        content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var scroll = history.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport; scroll.content = content; scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
        var close = Button("Return", history);
        UnityEventTools.AddBoolPersistentListener(close.onClick, history.gameObject.SetActive, false);
        UnityEventTools.AddBoolPersistentListener(close.onClick, options.gameObject.SetActive, true);
        UnityEventTools.AddBoolPersistentListener(remind.onClick, history.gameObject.SetActive, true);
        var modal = Panel("SkipPanel", ui); Stretch(modal); modal.GetComponent<Image>().color = new Color(0, 0, 0, .65f);
        var card = Panel("InfoPanel", modal); var prompt = Label("SkipInfo", card); prompt.text = "정말로 현재 대화를\n건너뛰시겠습니까?";
        Label("SkipAddInfo", card).text = "다시보기를 통해 대화 내용을 확인할 수 있습니다.";
        var back = Button("BackButton", card); Label("Label", back.transform).text = "뒤로가기";
        var confirm = Button("SkipButton", card); Label("Label", confirm.transform).text = "건너뛰기";
        UnityEventTools.AddBoolPersistentListener(skip.onClick, modal.gameObject.SetActive, true);
        UnityEventTools.AddBoolPersistentListener(back.onClick, modal.gameObject.SetActive, false);
        UnityEventTools.AddPersistentListener(confirm.onClick, chat.OnSkipPressed);
        UnityEventTools.AddBoolPersistentListener(confirm.onClick, modal.gameObject.SetActive, false);
        var curtain = (GameObject)PrefabUtility.InstantiatePrefab(Asset<GameObject>("fa31987a2e74fca48ab1b97c8582726b"), root.transform);
        Set(chat, "curtain", curtain.GetComponent<FadeCurtain>());
        Set(chat, "background", backdrop.GetComponent<Image>()); Set(chat, "talkPanel", speech.gameObject);
        Set(chat, "talkerName", name); Set(chat, "talkerInfo", info); Set(chat, "text", body);
        Set(chat, "choicePanel", choices.gameObject); Set(chat, "optionPanel", options.gameObject); Set(chat, "remindContent", content.gameObject);
        Set(chat, "remindTalkNode", Asset<GameObject>("73561ba2235f846d1a6c72aabed61c83"));
        Set(chat, "remindChoiceNode", Asset<GameObject>("9723efae8ecd641568f310a0f525658e"));
        Set(chat, "bgm", Sound(root, "BGM", true, "b5aedc6a5da6b244aa2eb0e29766dbd8", "6e28d088461bb4145a650ba2deccf3b0", "6babfcbb326eef04596c2d87cf3b7513"));
        Set(chat, "textSFX", Sound(root, "TextSFX", false, "9fc13e636899a7f418d0f93141fb6c1a", "54cba5d6d81f6fb499757894c5258a20"));
        var cp = new SerializedObject(chat); var list = cp.FindProperty("standImages"); list.arraySize = portraits.Count;
        for (int i = 0; i < portraits.Count; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = portraits[i]; cp.ApplyModifiedPropertiesWithoutUndo();
        var skin = root.AddComponent<ComicDialogueSkin>();
        Set(skin, "dialogueSprite", Art("DialoguePanel")); Set(skin, "rightDialogueSprite", Art("DialoguePanelRight")); Set(skin, "speakerSprite", Art("SpeakerPlate")); Set(skin, "choiceSprite", Art("ChoiceButton"));
        Set(skin, "font", Asset<TMP_FontAsset>("2c0a54ae745cfd143bfdfa3d019d9b29"));
        var sp = new SerializedObject(skin); var icons = sp.FindProperty("controlSprites"); icons.arraySize = 6;
        var iconNames = new[] { "Talk", "Branch", "History", "Skip", "Back", "Continue" };
        for (int i = 0; i < 6; i++) icons.GetArrayElementAtIndex(i).objectReferenceValue = Art(iconNames[i]); sp.ApplyModifiedPropertiesWithoutUndo();
        skin.Apply(); EditorUtility.SetDirty(skin);
        history.gameObject.SetActive(false); modal.gameObject.SetActive(false); choices.gameObject.SetActive(false); ui.gameObject.SetActive(false);
        return root;
    }

    private static SoundManager Sound(GameObject root, string name, bool bgm, params string[] clips)
    {
        var go = new GameObject(name); go.transform.SetParent(root.transform, false);
        var source = go.AddComponent<AudioSource>(); source.playOnAwake = false; source.loop = bgm;
        var manager = go.AddComponent<SoundManager>(); manager.clips = clips.Select(Asset<AudioClip>).ToList(); source.clip = manager.clips[0];
        Set(manager, "audioSource", source); var so = new SerializedObject(manager); so.FindProperty("onBGMChannel").boolValue = bgm; so.ApplyModifiedPropertiesWithoutUndo(); return manager;
    }
    private static T Asset<T>(string guid) where T : UnityEngine.Object
    {
        var value = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        if (value == null) throw new InvalidOperationException("Missing Chat asset: " + guid); return value;
    }
    private static Sprite Art(string name) => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/DialogueConceptTest/Art/" + name + ".png");
    private static void Set(UnityEngine.Object target, string field, UnityEngine.Object value)
    {
        var so = new SerializedObject(target); so.FindProperty(field).objectReferenceValue = value; so.ApplyModifiedPropertiesWithoutUndo();
    }
    private static RectTransform Rect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform)); go.layer = 5; go.transform.SetParent(parent, false); return (RectTransform)go.transform;
    }
    private static RectTransform Panel(string name, Transform parent) { var r = Rect(name, parent); r.gameObject.AddComponent<Image>(); return r; }
    private static Button Button(string name, Transform parent) { var r = Panel(name, parent); var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = r.GetComponent<Image>(); return b; }
    private static TMP_Text Label(string name, Transform parent)
    {
        var r = Rect(name, parent); Stretch(r); var label = r.gameObject.AddComponent<TextMeshProUGUI>(); label.font = Asset<TMP_FontAsset>("2c0a54ae745cfd143bfdfa3d019d9b29"); label.fontSize = 28; label.raycastTarget = false; return label;
    }
    private static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
}