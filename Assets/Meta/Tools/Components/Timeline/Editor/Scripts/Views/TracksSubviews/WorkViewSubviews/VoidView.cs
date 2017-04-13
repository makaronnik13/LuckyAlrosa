using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class VoidView : WorkViewBase
    {
        #region Private Variables
        //private Texture2D _voidTexture;
        #endregion

        #region Constructor
        public VoidView()
        {
            //_voidTexture = (Texture2D)Resources.Load("Textures/BackgroundImages/28211d0561b8d4d6ceabf2bcf0b6ac8a");
        }
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);

            drawVoid();
        }
        #endregion

        #region Drawing Void
        private void drawVoid()
        {
            if (_field.x > 0f)
            {
                //Debug.Log("Left void drawing");
                Rect leftVoidRect = new Rect(_viewRect.x, _viewRect.y, _field.x, _viewRect.height);
                //GUI.DrawTextureWithTexCoords(leftVoidRect, _voidTexture, new Rect(0f, 0f, leftVoidRect.width / 128f, leftVoidRect.height / 128f));
                
                Handles.BeginGUI();
                
                Handles.color = new Color(64 / 255, 64 / 255, 64 / 255);
                Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(leftVoidRect.x, leftVoidRect.y),
                new Vector3(leftVoidRect.x + leftVoidRect.width, leftVoidRect.y),
                new Vector3(leftVoidRect.x + leftVoidRect.width, leftVoidRect.y + leftVoidRect.height),
                new Vector3(leftVoidRect.x, leftVoidRect.y + leftVoidRect.height)});

                Handles.EndGUI();
            }

            //Debug.Log("_workView.EndGlobalPosition = " + _workView.EndGlobalPosition);
            //Debug.Log("_viewRect.width = " + _viewRect.width);
            //Debug.Log("_field.x = " + _field.x);
            if (_workView.EndGlobalPosition < _viewRect.width)
            {
                //Debug.Log("Right void drawing");
                Rect rightVoidRect = new Rect(_viewRect.x + _workView.EndGlobalPosition, _viewRect.y, _viewRect.width - _workView.EndGlobalPosition, _viewRect.height);
                //GUI.DrawTextureWithTexCoords(rightVoidRect, _voidTexture, new Rect(0f, 0f, rightVoidRect.width / 128f, rightVoidRect.height / 128f));

                Handles.BeginGUI();
                
                Handles.color = new Color(64 / 255, 64 / 255, 64 / 255);
                Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(rightVoidRect.x, rightVoidRect.y),
                new Vector3(rightVoidRect.x + rightVoidRect.width, rightVoidRect.y),
                new Vector3(rightVoidRect.x + rightVoidRect.width, rightVoidRect.y + rightVoidRect.height),
                new Vector3(rightVoidRect.x, rightVoidRect.y + rightVoidRect.height)});

                Handles.EndGUI();
            }
        }
        #endregion

        #region Utility Methods
        #endregion
    }
}
