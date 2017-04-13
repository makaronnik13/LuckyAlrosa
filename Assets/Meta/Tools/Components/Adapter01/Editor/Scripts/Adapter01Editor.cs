using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    [CustomEditor(typeof(Adapter01))]
    public class Adapter01Editor : UnityEditor.Editor
    {
        private Adapter01 _adapter01;

        private void OnEnable()
        {
            _adapter01 = target as Adapter01;
        }

        public override void OnInspectorGUI()
        {
            GUIContent explosionLabelContent = new GUIContent();
            explosionLabelContent.text = "<b><i><size=18>  Adapter01 </size></i></b>";
            explosionLabelContent.tooltip = "Adapts float values to ranged between 0 and 1 float values.";
            EditorGUILayout.LabelField(explosionLabelContent, Utilities.RichTextStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tween Target:");
            serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = EditorGUILayout.ObjectField(serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue, typeof(MonoBehaviour), true) as MonoBehaviour;
            if (serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue != null)
            {
                MonoBehaviour[] scripts = (serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue as MonoBehaviour).GetComponents<MonoBehaviour>();
                bool founded = false;
                for (int i = 0; i < scripts.Length; i++)
                {
                    if (scripts[i] is I01InputFloatValue)
                    {
                        founded = true;
                        _adapter01.Target = scripts[i] as I01InputFloatValue;
                        serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = scripts[i];
                        break;
                    }
                }
                if (!founded)
                {
                    _adapter01.Target = null;
                    serializedObject.FindProperty("_containingMonoBehaviour").objectReferenceValue = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
