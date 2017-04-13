using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class ChangeTracksNameWindow : EditorWindow
    {
        #region Variables
        public TimelineTrack Track;
        public SerializedObject SerializedObject;
        public Timeline Timeline;

        private static ChangeTracksNameWindow Instance;
        private static string _wantedName = "";
        #endregion

        #region Main Methods
        public static void Init(TimelineTrack track, Timeline timeline, SerializedObject serializedObject)
        {
            Instance = (ChangeTracksNameWindow)EditorWindow.GetWindow<ChangeTracksNameWindow>();
            Instance.minSize = new Vector2(360f, 80f);
            Instance.maxSize = new Vector2(360f, 81f);
            GUIContent titleContent = new GUIContent();
            titleContent.text = "Enter name";
            Instance.titleContent = titleContent;
            Instance.Timeline = timeline;
            Instance.SerializedObject = serializedObject;
            Instance.Track = track;
            _wantedName = Instance.Track.Name;
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
                    Undo.RecordObject(Instance.Timeline, "Track's Name Changed");
                    Track.Name = _wantedName;
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
