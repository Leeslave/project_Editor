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
        if (GUILayout.Button("Start Turn"))
        {
            myScript.StartTurn(1.5f, 15f, 8);
        }
    }
}
