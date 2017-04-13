using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class TimelineViewBase
    {
        #region Protected Variables
        protected GUISkin _viewSkin;
        protected Timeline _timeline;
        protected TimelineWindow _timelineWindow;
        protected SerializedObject _serializedObject;

        protected Vector2 _offset;
        protected float _width;
        protected float _height;
        protected Rect _rect;

        protected Vector2 _mousePosition;
        #endregion

        #region Private Variables
        private Rect _viewRect;
        #endregion Public Variables

        #region Constructor
        public TimelineViewBase()
        {
            GetEditorSkin();
        }
        #endregion

        #region Main Methods
        public virtual void UpdateView(Rect parentRect, Rect percentageRect, Vector2 parentOffset, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow)
        {
            if (_viewSkin == null)
            {
                GetEditorSkin();
                return;
            }
            
            _timeline = timeline;
            _serializedObject = serializedObject;
            _timelineWindow = timelineWindow;

            _viewRect = new Rect(parentRect.width * percentageRect.x, parentRect.height * percentageRect.y, parentRect.width - parentRect.width * percentageRect.x - parentRect.width * percentageRect.width, parentRect.height - parentRect.height * percentageRect.y - parentRect.height * percentageRect.height);
            _offset = new Vector2(parentOffset.x + _viewRect.x, parentOffset.y + _viewRect.y);
            _width = _viewRect.width;
            _height = _viewRect.height;
            _rect = new Rect(0, 0, _width, _height);

            GUILayout.BeginArea(_viewRect);
            Draw();
            GUILayout.EndArea();
        }
        
        protected virtual void Draw()
        {

        }

        public virtual void ProcessEvents(Event e)
        {
            _mousePosition = e.mousePosition;
            _mousePosition.x -= _offset.x;
            _mousePosition.y -= _offset.y;
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
