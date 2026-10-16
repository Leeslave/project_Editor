using System;
using System.Collections.Generic;
using EditorGame.Documents;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DocumentStageData))]
public sealed class DocumentStageDataEditor : Editor
{
    private static readonly Dictionary<string, string> _labels = new Dictionary<string, string>
    {
        { "WorkCode", "업무 코드" }, { "Stage", "스테이지 번호" },
        { "Assignment", "문서 과제" }, { "Settlement", "명성치 정산" },
        { "UnlockCombination", "선행 조건 조합" }, { "Prerequisites", "선행 업무 목록" },
        { "Completion", "완료 명성치" }, { "IncorrectPerCount", "오답 1개당 명성치" },
        { "ConfidentialMission", "극비 임무 추가 명성치" }, { "CaughtPerCount", "발각 1회당 명성치" },
        { "Mode", "문서 업무 모드" }, { "TargetDateId", "조사 날짜" },
        { "Instructions", "플레이어 안내문" }, { "TargetPersonIds", "조사 대상 인물" },
        { "People", "인물 목록" }, { "Documents", "문서 목록" },
        { "Answers", "진술 대조 정답" }, { "Edits", "프로필 수정 지시" },
        { "PortraitKey", "초상화 키" }, { "Profile", "기본 프로필" },
        { "Name", "이름" }, { "Age", "나이" }, { "Sex", "성별" },
        { "Country", "국적" }, { "Job", "직업" }, { "PersonId", "인물" },
        { "DateId", "문서 날짜" }, { "Kind", "문서 종류" },
        { "Lines", "문서 내용 줄" }, { "Time", "시간" }, { "Text", "내용" },
        { "StatementLineId", "진술 줄" }, { "ActionLineId", "행동 줄" },
        { "Field", "수정할 항목" }, { "TargetValue", "목표 값" },
        { "DisplayText", "표시할 지시문" }
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EnsureIds();
        foreach (string field in new[] { "WorkCode", "Stage", "Assignment", "Settlement",
                     "UnlockCombination", "Prerequisites" })
        {
            SerializedProperty property = serializedObject.FindProperty(field);
            if (property != null) DrawProperty(property, Label(property));
        }
        serializedObject.ApplyModifiedProperties();
    }

    // 내부 ID를 제외한 속성을 종류에 맞는 입력으로 표시한다.
    private static void DrawProperty(SerializedProperty property, string label)
    {
        if (property.name == "Id") return;
        if (IsReference(property))
        {
            DrawReference(property, label);
            return;
        }
        var content = new GUIContent(label, property.tooltip);
        if (property.isArray && property.propertyType != SerializedPropertyType.String)
        {
            DrawList(property, content);
            return;
        }
        if (property.propertyType == SerializedPropertyType.Generic && property.hasVisibleChildren)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, content, true);
            if (!property.isExpanded) return;
            EditorGUI.indentLevel++;
            try
            {
                SerializedProperty child = property.Copy();
                SerializedProperty end = property.GetEndProperty();
                if (child.NextVisible(true))
                {
                    while (!SerializedProperty.EqualContents(child, end))
                    {
                        if (child.depth == property.depth + 1)
                            DrawProperty(child.Copy(), Label(child));
                        if (!child.NextVisible(false)) break;
                    }
                }
            }
            finally { EditorGUI.indentLevel--; }
            return;
        }
        if (TryDrawKoreanEnum(property, content)) return;
        EditorGUILayout.PropertyField(property, content, false);
    }

    // 인물 또는 문서 줄을 선택하는 참조 속성인지 확인한다.
    private static bool IsReference(SerializedProperty property)
    {
        return property.name == "PersonId" || property.name == "StatementLineId" ||
               property.name == "ActionLineId" ||
               (property.propertyType == SerializedPropertyType.String &&
                property.propertyPath.StartsWith("Assignment.TargetPersonIds.Array.data[", StringComparison.Ordinal));
    }

    // 이름과 내용으로 참조를 선택하고 끊어진 연결을 안내한다.
    private static void DrawReference(SerializedProperty property, string label)
    {
        var ids = new List<string> { "" };
        var names = new List<string> { "선택" };
        SerializedProperty assignment = property.serializedObject.FindProperty("Assignment");
        bool targetOnly = property.propertyPath.StartsWith("Assignment.Answers.", StringComparison.Ordinal) ||
                          property.propertyPath.StartsWith("Assignment.Edits.", StringComparison.Ordinal);
        if (property.name == "StatementLineId" || property.name == "ActionLineId")
            CollectLines(assignment, property,
                property.name == "StatementLineId" ? DocumentKind.Statement : DocumentKind.Action, ids, names);
        else
            CollectPeople(assignment, targetOnly, ids, names);

        int selected = ids.IndexOf(property.stringValue);
        bool invalid = selected < 0 && !string.IsNullOrEmpty(property.stringValue);
        if (invalid)
        {
            ids.Add(property.stringValue);
            names.Add("연결 오류 (다시 선택 필요)");
            selected = ids.Count - 1;
        }
        int next = EditorGUILayout.Popup(new GUIContent(label), Mathf.Max(0, selected), names.ToArray());
        if (next != selected) property.stringValue = ids[next];
        if (invalid)
            EditorGUILayout.HelpBox("참조 항목을 찾을 수 없습니다. 다시 선택하세요.", MessageType.Warning);
        else if (ids.Count == 1)
            EditorGUILayout.HelpBox("선택 가능한 항목을 먼저 작성하세요.", MessageType.Info);
    }

    // 전체 인물 또는 조사 대상 인물의 선택지를 수집한다.
    private static void CollectPeople(SerializedProperty assignment, bool targetOnly,
        List<string> ids, List<string> names)
    {
        if (assignment == null) return;
        SerializedProperty people = assignment.FindPropertyRelative("People");
        SerializedProperty targets = assignment.FindPropertyRelative("TargetPersonIds");
        for (int i = 0; i < people.arraySize; i++)
        {
            SerializedProperty person = people.GetArrayElementAtIndex(i);
            string id = person.FindPropertyRelative("Id").stringValue;
            if (string.IsNullOrEmpty(id) || (targetOnly && !Contains(targets, id))) continue;
            string name = person.FindPropertyRelative("Profile").FindPropertyRelative("Name").stringValue;
            ids.Add(id);
            names.Add($"{i + 1}. {(string.IsNullOrWhiteSpace(name) ? "이름 없음" : name)}");
        }
    }

    // 선택한 인물과 조사 날짜에 맞는 문서 줄을 수집한다.
    private static void CollectLines(SerializedProperty assignment, SerializedProperty reference,
        DocumentKind kind, List<string> ids, List<string> names)
    {
        if (assignment == null) return;
        string parentPath = reference.propertyPath.Substring(0, reference.propertyPath.LastIndexOf('.'));
        SerializedProperty person = reference.serializedObject.FindProperty(parentPath + ".PersonId");
        string personId = person == null ? "" : person.stringValue;
        string date = assignment.FindPropertyRelative("TargetDateId").stringValue;
        if (string.IsNullOrEmpty(personId) || string.IsNullOrEmpty(date)) return;
        SerializedProperty documents = assignment.FindPropertyRelative("Documents");
        for (int i = 0; i < documents.arraySize; i++)
        {
            SerializedProperty document = documents.GetArrayElementAtIndex(i);
            if (document.FindPropertyRelative("PersonId").stringValue != personId ||
                document.FindPropertyRelative("DateId").stringValue != date ||
                document.FindPropertyRelative("Kind").enumValueIndex != (int)kind) continue;
            SerializedProperty lines = document.FindPropertyRelative("Lines");
            for (int j = 0; j < lines.arraySize; j++)
            {
                SerializedProperty line = lines.GetArrayElementAtIndex(j);
                string id = line.FindPropertyRelative("Id").stringValue;
                if (string.IsNullOrEmpty(id)) continue;
                string time = line.FindPropertyRelative("Time").stringValue;
                string content = line.FindPropertyRelative("Text").stringValue;
                if (content.Length > 60) content = content.Substring(0, 60) + "…";
                ids.Add(id);
                names.Add($"{i + 1}-{j + 1}. {time} {content}");
            }
        }
    }

    // 직렬화된 문자열 목록에 지정한 값이 있는지 확인한다.
    private static bool Contains(SerializedProperty array, string value)
    {
        for (int i = 0; i < array.arraySize; i++)
            if (array.GetArrayElementAtIndex(i).stringValue == value) return true;
        return false;
    }

    // 목록 항목과 추가 및 삭제 버튼을 표시한다.
    private static void DrawList(SerializedProperty property, GUIContent content)
    {
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded,
            new GUIContent($"{content.text} ({property.arraySize})", content.tooltip), true);
        if (!property.isExpanded) return;
        EditorGUI.indentLevel++;
        try
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawProperty(property.GetArrayElementAtIndex(i), $"{ItemName(property.name)} {i + 1}");
                    if (GUILayout.Button("삭제"))
                    {
                        property.DeleteArrayElementAtIndex(i);
                        property.serializedObject.ApplyModifiedProperties();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            if (GUILayout.Button("추가"))
            {
                int index = property.arraySize;
                property.InsertArrayElementAtIndex(index);
                InitializeNewItem(property, property.GetArrayElementAtIndex(index));
                property.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
        }
        finally { EditorGUI.indentLevel--; }
    }

    // 새 항목의 ID를 발급하고 복제된 참조와 내용을 비운다.
    private static void InitializeNewItem(SerializedProperty list, SerializedProperty item)
    {
        if (list.name == "TargetPersonIds") { item.stringValue = ""; return; }
        if (list.name == "Prerequisites")
        {
            item.FindPropertyRelative("WorkCode").stringValue = "";
            item.FindPropertyRelative("Stage").intValue = -1;
            return;
        }
        SerializedProperty id = item.FindPropertyRelative("Id");
        if (id != null) id.stringValue = NewId();
        switch (list.name)
        {
            case "People":
                item.FindPropertyRelative("PortraitKey").stringValue = "";
                SerializedProperty profile = item.FindPropertyRelative("Profile");
                foreach (string field in new[] { "Name", "Age", "Sex", "Country", "Job" })
                    profile.FindPropertyRelative(field).stringValue = "";
                break;
            case "Documents":
                item.FindPropertyRelative("PersonId").stringValue = "";
                item.FindPropertyRelative("DateId").stringValue = "";
                item.FindPropertyRelative("Lines").ClearArray();
                break;
            case "Lines":
                item.FindPropertyRelative("Time").stringValue = "";
                item.FindPropertyRelative("Text").stringValue = "";
                break;
            case "Answers":
                item.FindPropertyRelative("PersonId").stringValue = "";
                item.FindPropertyRelative("StatementLineId").stringValue = "";
                item.FindPropertyRelative("ActionLineId").stringValue = "";
                break;
            case "Edits":
                item.FindPropertyRelative("PersonId").stringValue = "";
                item.FindPropertyRelative("TargetValue").stringValue = "";
                item.FindPropertyRelative("DisplayText").stringValue = "";
                break;
        }
    }

    // 기존 ID를 유지하면서 비어 있는 내부 ID를 채운다.
    private void EnsureIds()
    {
        SerializedProperty assignment = serializedObject.FindProperty("Assignment");
        if (assignment == null) return;
        EnsureId(assignment.FindPropertyRelative("Id"));
        foreach (string listName in new[] { "People", "Documents", "Answers", "Edits" })
        {
            SerializedProperty list = assignment.FindPropertyRelative(listName);
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);
                EnsureId(item.FindPropertyRelative("Id"));
                if (listName != "Documents") continue;
                SerializedProperty lines = item.FindPropertyRelative("Lines");
                for (int j = 0; j < lines.arraySize; j++)
                    EnsureId(lines.GetArrayElementAtIndex(j).FindPropertyRelative("Id"));
            }
        }
    }

    // 비어 있는 ID에 새 고유값을 지정한다.
    private static void EnsureId(SerializedProperty property)
    {
        if (property != null && string.IsNullOrWhiteSpace(property.stringValue))
            property.stringValue = NewId();
    }

    // 목록 순서나 표시 이름에 영향받지 않는 고유값을 생성한다.
    private static string NewId() => Guid.NewGuid().ToString("N");

    // 지원하는 열거형을 한국어 선택지로 표시한다.
    private static bool TryDrawKoreanEnum(SerializedProperty property, GUIContent content)
    {
        if (property.propertyType != SerializedPropertyType.Enum) return false;
        string[] options;
        switch (property.name)
        {
            case "Mode": options = new[] { "진술 대조", "극비 문서 조작" }; break;
            case "Kind": options = new[] { "진술 기록", "실제 행동 기록", "인물 프로필" }; break;
            case "Field": options = new[] { "이름", "나이", "성별", "국적", "직업" }; break;
            case "UnlockCombination": options = new[] { "미설정", "모두 완료", "하나 이상 완료" }; break;
            default: return false;
        }
        property.enumValueIndex = EditorGUILayout.Popup(content,
            Mathf.Clamp(property.enumValueIndex, 0, options.Length - 1), options);
        return true;
    }

    // 속성 위치에 맞는 한국어 표시 이름을 반환한다.
    private static string Label(SerializedProperty property)
    {
        if (property.name == "Stage" && property.propertyPath.StartsWith("Prerequisites.", StringComparison.Ordinal))
            return "선행 스테이지";
        if (property.name == "WorkCode" && property.propertyPath.StartsWith("Prerequisites.", StringComparison.Ordinal))
            return "선행 업무 코드";
        return _labels.TryGetValue(property.name, out string label) ? label : property.displayName;
    }

    // 목록 종류에 맞는 짧은 항목 이름을 반환한다.
    private static string ItemName(string listName)
    {
        switch (listName)
        {
            case "Prerequisites": return "선행 업무";
            case "TargetPersonIds": return "대상 인물";
            case "People": return "인물";
            case "Documents": return "문서";
            case "Lines": return "문서 줄";
            case "Answers": return "정답";
            case "Edits": return "수정 지시";
            default: return "항목";
        }
    }
}
