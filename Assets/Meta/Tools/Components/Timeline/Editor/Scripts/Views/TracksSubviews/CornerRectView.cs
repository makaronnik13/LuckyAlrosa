using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class CornerRectView : TracksViewBase
    {
        #region Private Variables
        //private Texture2D _uselessRect;
        #endregion

        #region Constructor
        public CornerRectView()
        {
            //_uselessRect = (Texture2D)Resources.Load("Textures/BackgroundImages/Flag_of_Seychelles.svg");
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            DrawCornerRect();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
        }
        #endregion

        #region Drawing
        public void DrawCornerRect()
        {
            //GUI.DrawTexture(ViewRect, _uselessRect, ScaleMode.StretchToFill);


            /*Handles.BeginGUI();

            float padding = 0f;
            Handles.color = Color.gray;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(ViewRect.x + padding, ViewRect.y + padding),
                new Vector3(ViewRect.x + ViewRect.width - padding * 2f, ViewRect.y + ViewRect.height - padding * 2f),
                new Vector3(ViewRect.x + padding, ViewRect.y + ViewRect.height - padding * 2f)});
            Handles.color = new Color(222f / 255f, 222f / 255f, 222f / 255f);
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(ViewRect.x + padding, ViewRect.y + padding),
                new Vector3(ViewRect.x + ViewRect.width - padding * 2f, ViewRect.y + padding),
                new Vector3(ViewRect.x + ViewRect.width - padding * 2f, ViewRect.y + ViewRect.height - padding * 2f)});

            Handles.EndGUI();*/
        }
        #endregion
    }
}
