using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class ChangeStatesNameWindow : EditorWindow
    {
        #region Variables
        public ExplosionSettingsBundle Bundle;
        public SerializedObject SerializedObject;
        public ModelExploder ModelExploder;

        private static ChangeStatesNameWindow Instance;
        private static string _wantedName = "";
        #endregion

        #region Main Methods
        public static void Init(ExplosionSettingsBundle bundle, ModelExploder modelExploder, SerializedObject serializedObject)
        {
            Instance = (ChangeStatesNameWindow)EditorWindow.GetWindow<ChangeStatesNameWindow>();
            Instance.minSize = new Vector2(360f, 80f);
            Instance.maxSize = new Vector2(360f, 81f);
            GUIContent titleContent = new GUIContent();
            titleContent.text = "Enter name";
            Instance.titleContent = titleContent;
            Instance.SerializedObject = serializedObject;
            Instance.Bundle = bundle;
            Instance.ModelExploder = modelExploder;
            _wantedName = Instance.Bundle.Name;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();

            _wantedName = EditorGUILayout.TextField("Enter Name: ", _wantedName);

            GUILayout.Space(10f);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(_wantedName))
                {
                    Undo.RecordObject(Instance.ModelExploder, "States's Name Changed");
                    Bundle.Name = _wantedName;
                    SerializedObject.Update();
                    SerializedObject.ApplyModifiedProperties();
                    Instance.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Node Message:", "Please enter a valid name!", "OK");
                }
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(40)))
            {
                Instance.Close();
            }
            GUILayout.EndHorizontal();


            GUILayout.Space(20f);
            GUILayout.EndVertical();
            GUILayout.Space(20f);
        }
        #endregion
    }

}
