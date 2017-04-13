using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class ChangeKeynotesNameWindow : EditorWindow
    {
        #region Variables
        public Keynote Keynote;
        public SerializedObject SerializedObject;
        public Timeline Timeline;

        private static ChangeKeynotesNameWindow Instance;
        private static string _wantedName = "";
        #endregion

        #region Main Methods
        public static void Init(Keynote keynote, Timeline timeline, SerializedObject serializedObject)
        {
            Instance = (ChangeKeynotesNameWindow)EditorWindow.GetWindow<ChangeKeynotesNameWindow>();
            Instance.minSize = new Vector2(360f, 80f);
            Instance.maxSize = new Vector2(360f, 81f);
            GUIContent titleContent = new GUIContent();
            titleContent.text = "Enter name";
            Instance.titleContent = titleContent;
            Instance.Timeline = timeline;
            Instance.SerializedObject = serializedObject;
            Instance.Keynote = keynote;
            _wantedName = Instance.Keynote.Name;
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
                    Undo.RecordObject(Instance.Timeline, "Keynote's Name Changed");
                    Keynote.Name = _wantedName;
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
