using UnityEditor;
using UnityEngine;

namespace float_oat.Desktop90.EditorTools
{
    /// <summary>
    /// Editor extention to show inspector properties for D90Button
    /// </summary>
    [CustomEditor(typeof(D90Button))]
    [CanEditMultipleObjects]
    public class D90ButtonEditor : UnityEditor.UI.ButtonEditor
    {
        SerializedProperty graphicProperty;
        SerializedProperty enabledColorProperty;
        SerializedProperty disabledColorProperty;

        SerializedProperty pressInContentProperty;
        SerializedProperty pressInDistanceProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            graphicProperty = serializedObject.FindProperty("GraphicToColor");
            enabledColorProperty = serializedObject.FindProperty("EnabledGraphicColor");
            disabledColorProperty = serializedObject.FindProperty("DisabledGraphicColor");

            pressInContentProperty = serializedObject.FindProperty("PressInContent");
            pressInDistanceProperty = serializedObject.FindProperty("DistanceToPressIn");
        }

        public override void OnInspectorGUI()
        {
            // show the base Button GUI, and the added properties
            base.OnInspectorGUI();

            ShowProperty(graphicProperty);
            ShowProperty(enabledColorProperty);
            ShowProperty(disabledColorProperty);

            ShowProperty(pressInContentProperty);
            ShowProperty(pressInDistanceProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowProperty(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));
        }
    }
}
