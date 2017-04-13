using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class TimelineExceptionWindow : EditorWindow
    {
        #region Variables
        private static TimelineExceptionWindow Instance;
        private static string _excepstionMessage = "";
        private static string _caption = "";

        private static float _width = 360f;
        private static float _height = 120f;
        #endregion

        #region Main Methods
        public static void Init(string Title, string Caption, string ExcepstionMessage)
        {
            Instance = (TimelineExceptionWindow)EditorWindow.GetWindow<TimelineExceptionWindow>();
            Instance.minSize = new Vector2(_width, _height);
            Instance.maxSize = new Vector2(_width, _height);
            GUIContent titleContent = new GUIContent(); 
            titleContent.text = Title;
            Instance.titleContent = titleContent;
            Instance.position = new Rect((Screen.currentResolution.width - _width)/2f, (Screen.currentResolution.height - _height)/2f, _width, _height);

            _excepstionMessage = ExcepstionMessage;
            _caption = Caption;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            
            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<b>" + _caption + "</b>", Utilities.RichTextStyle);

            GUILayout.Space(10f);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(_excepstionMessage, Utilities.RichWordWrapCenterTextStyle, GUILayout.Width(_width - 20f));

            GUILayout.Space(10f);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(40)))
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
