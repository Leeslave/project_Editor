using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VK_ManagerScript))]
public class VK_ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VK_ManagerScript myScript = (VK_ManagerScript)target;
        if (GUILayout.Button("Start ComplexTurn"))
            myScript.StartComplexTurn(3f, 15f, 8, 7);
        if(GUILayout.Button("Start EyeTurn"))
            myScript.StartEyeTurn(3f, 15f, 8);
        if(GUILayout.Button("Start ArrowTurn"))
            myScript.StartArrowTurn(3f, 15f, 7);
        if (GUILayout.Button("Start Tutorial"))
            myScript.TutorialManager.StartVoightTutorial();
    }
}
