using System;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class TopPanelView : TracksViewBase
    {
        #region Private Variables
        private Texture2D _backgroundTexture;
        #endregion

        #region Constructor
        public TopPanelView()
        {
            _backgroundTexture = (Texture2D)Resources.Load("Textures/NewDesign/TopPanelBackground");
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            GUI.DrawTexture(_rect, _backgroundTexture);

            GUILayout.BeginArea(new Rect(10f, 0, 300, _rect.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            GUIContent durationContent = new GUIContent();
            durationContent.text = "<color=#cccccc><size=14>Duration of timeline:</size></color>";
            GUILayout.Label(durationContent, Utilities.RichTextStyle, GUILayout.Width(142));
            _serializedObject.FindProperty("_duration").floatValue = (float)Convert.ToDouble(GUILayout.TextField(_serializedObject.FindProperty("_duration").floatValue.ToString(), GUILayout.Width(50)));
            GUILayout.Label("<color=#cccccc><size=14>(secs)</size></color>", Utilities.RichTextStyle, GUILayout.Width(40));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        #endregion
    }
}
