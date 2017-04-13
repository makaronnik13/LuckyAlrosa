using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public static class ModelExploderEditorGUI
    {
        private static float _scrollPosition = 0f;

        private static float _sidePadding = 0f;
        public static float StateHeight = 40f;
        private static float _buttonsHeight = 16f;

        private static GUISkin _scrollBarSkin;

        private static void LoadScrollBarSkin()
        {
            _scrollBarSkin = (GUISkin)Resources.Load("GUISkins/ModelExploderScrollbarGUISkin");
        }

        public static int StatesList(Rect bounds, int selected, string[] names, int[] indexes, GUISkin skin, out string nameEdited, out int nameEditedIndex, out int saveButtonPressed)
        {
            Vector2 scrollVector2 = new Vector2(0f, _scrollPosition);

            if (_scrollBarSkin == null)
            {
                LoadScrollBarSkin();
            }
            GUISkin temp = GUI.skin;
            GUI.skin = _scrollBarSkin;
            _scrollPosition = GUILayout.BeginScrollView(scrollVector2, new GUILayoutOption[] { GUILayout.Width(bounds.width), GUILayout.Height(bounds.height) }).y;
            GUI.skin = temp;

            GUILayout.BeginHorizontal();
            GUILayout.Space(_sidePadding);

            GUILayout.Box(GUIContent.none, new GUIStyle(), new GUILayoutOption[] { GUILayout.Width(bounds.width - _sidePadding * 2f), GUILayout.Height(StateHeight * names.Length) });

            nameEdited = null;
            saveButtonPressed = -1;
            nameEditedIndex = -1;
            for (int i = 0; i < names.Length; i++)
            {
                GUIContent content = new GUIContent();

                GUIStyle style = null;
                if (selected == indexes[i])
                {
                    style = skin.GetStyle("StatesListSelected");
                }
                else
                {
                    style = skin.GetStyle("StatesListFon");
                }
                
                GUI.Box(new Rect(_sidePadding, i * StateHeight, bounds.width - _sidePadding * 2f, StateHeight), content, style);


                if (selected == indexes[i])
                {
                    style = skin.GetStyle("StatesTextFieldSelected");
                }
                else
                {
                    style = skin.GetStyle("StatesTextField");
                }

                GUI.SetNextControlName("StatesTextField_" + i.ToString());
                string newName = GUI.TextField(new Rect(_sidePadding, i * StateHeight, bounds.width - _sidePadding * 2f /*- _buttonsHeight * 3.3f - 8f*/, StateHeight), names[i], style);
                if (newName != names[i])
                {
                    nameEdited = newName;
                    nameEditedIndex = indexes[i];
                }

                /*if (GUI.Button(new Rect(_sidePadding, i * StateHeight, bounds.width - _sidePadding * 2f, StateHeight), GUIContent.none, new GUIStyle()))
                {
                    selected = indexes[i];
                }
                else*/
                if (GUI.GetNameOfFocusedControl() == "StatesTextField_" + i.ToString())
                {
                    selected = indexes[i];
                }
            }

            GUILayout.Space(_sidePadding);
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            return selected;
        }
    }
}
