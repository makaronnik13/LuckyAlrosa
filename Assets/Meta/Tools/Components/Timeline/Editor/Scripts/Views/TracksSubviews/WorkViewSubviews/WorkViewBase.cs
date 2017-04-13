using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    /// <summary>
    /// WorkViewBase class is dedicated for drawing in WorkView. It recieves the rect of where to draw and optionally 
    /// timings of animation and draws it's part of picture.
    /// </summary>
    [Serializable]
    public class WorkViewBase
    {
        #region Protected Variables
        protected GUISkin _viewSkin;
        protected Timeline _timeline;
        protected TimelineWindow _timelineWindow;
        protected SerializedObject _serializedObject;
        protected WorkView _workView;

        protected Rect _localDrawingRect;
        protected Rect _globalDrawingRect;
        protected float _timing0;
        protected float _timing1;
        protected float _chapterTiming0;
        protected float _chapterTiming1;

        protected Rect _field;
        protected Rect _viewRect;
        protected Rect _realRect;

        protected bool _pressedLMB;
        protected bool _pressedRMB;

        protected Vector2 mousePosition = Vector2.zero;

        protected Vector2 _parentOffset;
        //protected Rect _realRect;
        #endregion

        #region Constructor
        public WorkViewBase()
        {
            GetEditorSkin();
        }
        #endregion

        #region Main Methods
        public virtual void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0f, float timing1 = 1f, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            if (_viewSkin == null)
            {
                GetEditorSkin();
                return;
            }

            _parentOffset = parentOffset;

            _timeline = timeline;
            _serializedObject = serializedObject;
            _timelineWindow = timelineWindow;
            _workView = workView;

            _localDrawingRect = localDrawingRect;
            _globalDrawingRect = globalDrawingRect;
            _timing0 = timing0;
            _timing1 = timing1;
            _chapterTiming0 = chapterTiming0;
            _chapterTiming1 = chapterTiming1;

            _field = field;
            _viewRect = viewRect;
            //_realRect = new Rect(viewRect.x + localDrawingRect.x - field.x, viewRect.y + localDrawingRect.y - field.y, localDrawingRect.width, localDrawingRect.height);
            _realRect = new Rect(viewRect.x + localDrawingRect.x + field.x, viewRect.y + localDrawingRect.y + field.y, localDrawingRect.width, localDrawingRect.height);
        }

        public virtual void ProcessEvents(Event e)
        {
            mousePosition = e.mousePosition;
            mousePosition.x -= _parentOffset.x;
            mousePosition.y -= _parentOffset.y;

            if (_viewRect.Contains(mousePosition))
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (e.button == 0)
                        {
                            _pressedLMB = true;
                        }
                        else if (e.button == 1)
                        {
                            _pressedRMB = true;
                        }
                        break;
                    case EventType.MouseUp:
                        if (e.button == 0)
                        {
                            _pressedLMB = false;
                        }
                        else if (e.button == 1)
                        {
                            _pressedRMB = false;
                        }
                        break;
                }
            }
            else
            {
                _pressedLMB = false;
                _pressedRMB = false;
            }
        }
        #endregion

        #region Utility Methods
        protected void GetEditorSkin()
        {
            _viewSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/TimelineEditorSkin");
        }
        #endregion
    }
}
