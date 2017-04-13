using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class NodePopupWindow : EditorWindow
    {
        #region Variables
        private static NodePopupWindow _currentPopupWindow;
        private static string _wantedName = "Enter a name...";
        #endregion

        #region Main Methods
        public static void InitNodePopup()
        {
            _currentPopupWindow = (NodePopupWindow)EditorWindow.GetWindow<NodePopupWindow>();
            GUIContent popupTitleContent = new GUIContent();
            popupTitleContent.text = "Node Popup";
            _currentPopupWindow.titleContent = popupTitleContent;
        }

        private void OnGUI()
        {
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            GUILayout.Space(20f);

            EditorGUILayout.LabelField("Create New Graph", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            _wantedName = EditorGUILayout.TextField("Enter Name: ", _wantedName);

            GUILayout.Space(10f);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(_wantedName) && _wantedName != "Enter a name...")
                {
                    //NodeUtils.CreateNewGraph(_wantedName);
                    _currentPopupWindow.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Node MEssage:", "Please enter a valid graph name!", "OK");
                }
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(40)))
            {
                _currentPopupWindow.Close();
            }
            GUILayout.EndHorizontal();


            GUILayout.Space(20f);
            GUILayout.EndVertical();
            GUILayout.Space(20f);
        }
        #endregion

        #region Utility Methods

        #endregion
    }
}
