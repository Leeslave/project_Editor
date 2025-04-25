using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
[CustomEditor(typeof(GameSystem))]
public class GameSystemEditor : Editor
{
    private GameSystem gameSystem;
    string param = "";

    public void OnEnable()
    {
        gameSystem = (GameSystem)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        // Debug Console func
        GUILayout.Space(10);
        
        // 날짜 변경
        GUILayout.BeginHorizontal();
        param = EditorGUILayout.TextField("입력값:", param);
        if (GUILayout.Button("함수 실행"))
        {
            if (gameSystem is not null)
            {
                Debug.Log($"Execute Pressed with {param}");
            }
        }
        GUILayout.EndHorizontal();
        // 시간대 변경
        
        // 데이터 확인하기
        
        // 업무 완료 전환
    }
}
#endif
