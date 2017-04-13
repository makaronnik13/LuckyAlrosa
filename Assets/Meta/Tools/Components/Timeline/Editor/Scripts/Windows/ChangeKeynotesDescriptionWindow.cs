using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class ChangeKeynotesDescriptionWindow : EditorWindow
    {
        #region Variables
        public Keynote Keynote;
        public SerializedObject SerializedObject;
        public Timeline Timeline;

        private static ChangeKeynotesDescriptionWindow Instance;
        private static string _wantedDescription = "";
        #endregion

        #region Main Methods
        public static void Init(Keynote keynote, Timeline timeline, SerializedObject serializedObject)
        {
            Instance = (ChangeKeynotesDescriptionWindow)EditorWindow.GetWindow<ChangeKeynotesDescriptionWindow>();
            GUIContent titleContent = new GUIContent();
            titleContent.text = "Enter description";
            Instance.titleContent = titleContent;
            Instance.Timeline = timeline;
            Instance.SerializedObject = serializedObject;
            Instance.Keynote = keynote;
            _wantedDescription = Instance.Keynote.Description;
        }

        private void OnGUI()
        {
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(20f);

            EditorGUILayout.LabelField("Enter description", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            EditorStyles.textField.wordWrap = true;
            _wantedDescription = EditorGUILayout.TextField("Description: ", _wantedDescription, GUILayout.ExpandHeight(true));
            EditorStyles.textField.wordWrap = false;

            GUILayout.Space(10f);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(40)))
            {
                Undo.RecordObject(Instance.Timeline, "Keynote's Description Changed");
                Keynote.Description = _wantedDescription;
                SerializedObject.Update();
                SerializedObject.ApplyModifiedProperties();
                Instance.Close();
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
