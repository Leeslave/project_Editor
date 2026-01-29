using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialHighlightConfig))]
public class TutorialHighlightConfigEditor : Editor
{
    SerializedProperty isSquare;
    SerializedProperty isCircle;
    SerializedProperty sWidth;
    SerializedProperty sHeight;
    SerializedProperty cRadius;

    private void OnEnable()
    {
        isSquare = serializedObject.FindProperty("isSquare");
        isCircle = serializedObject.FindProperty("isCircle");
        sWidth = serializedObject.FindProperty("sWidth");
        sHeight = serializedObject.FindProperty("sHeight");
        cRadius = serializedObject.FindProperty("cRadius");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool sq = isSquare.boolValue;
        bool ci = isCircle.boolValue;

        using (new EditorGUI.DisabledScope(ci))
        {
            bool next = EditorGUILayout.Toggle("Is Square", sq);
            if (next != sq)
            {
                isSquare.boolValue = next;
                if (next) isCircle.boolValue = false;
            }
        }

        using (new EditorGUI.DisabledGroupScope(sq))
        {
            bool next = EditorGUILayout.Toggle("Is Circle",ci);
            if (next != ci)
            {
                isCircle.boolValue = next;
                if (next) isSquare.boolValue = false;
            }
        }

        EditorGUILayout.Space(10);

        using (new EditorGUI.DisabledScope(!sq))
        {
            EditorGUILayout.PropertyField(sWidth);
            EditorGUILayout.PropertyField(sHeight);
        }

        using (new EditorGUI.DisabledGroupScope(!ci))
        {
            EditorGUILayout.PropertyField(cRadius);
        }

        if (!sq && !ci)
        {
            EditorGUILayout.HelpBox("Select Square or Circle to edit size", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
    
}
