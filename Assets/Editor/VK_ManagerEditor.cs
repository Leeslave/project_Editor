using log4net.Config;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VK_ManagerScript))]
public class VK_ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VK_ManagerScript myScript = (VK_ManagerScript)target;
        if(GUILayout.Button("Start RandomTurn"))
            myScript.StartRandomTurn();
        if (GUILayout.Button("Start ComplexTurn"))
            myScript.StartComplexTurn(2f, 10f, 8, 7);
        if(GUILayout.Button("Start EyeTurn"))
            myScript.StartEyeTurn(2f, 10f, 8);
        if(GUILayout.Button("Start ArrowTurn"))
            myScript.StartArrowTurn(2f, 10f, 12);
        if (GUILayout.Button("Start Tutorial"))
            myScript.TutorialManager.StartVoightTutorial();
    }
}
