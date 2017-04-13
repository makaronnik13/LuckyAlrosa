using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    /// <summary>
    /// This is View Base class for views that rendering tracks. They have common features of scrolling, scaling etc.
    /// </summary>
    [Serializable]
    public class TracksViewBase
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

        protected TracksView _tracksView;

        protected Vector2 _mousePosition;
        #endregion

        #region Private Variables
        private Rect _viewRect;
        #endregion Public Variables

        #region Constructor
        public TracksViewBase()
        {
            GetEditorSkin();
        }
        #endregion

        #region Main Methods
        public virtual void UpdateView(Rect parentRect, Rect percentageRect, Vector2 parentOffset, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, TracksView tracksView)
        {
            if (_viewSkin == null)
            {
                GetEditorSkin();
                return;
            }
            
            _timeline = timeline;
            _serializedObject = serializedObject;
            _timelineWindow = timelineWindow;

            _tracksView = tracksView;

            _viewRect = GetRelativeRect(parentRect, percentageRect);
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

        protected Rect GetRelativeRect(Rect editorRect, Rect percentageRect)
        {
            return new Rect(editorRect.width * percentageRect.x, editorRect.height * percentageRect.y, editorRect.width - editorRect.width * percentageRect.x - editorRect.width * percentageRect.width, editorRect.height - editorRect.height * percentageRect.y - editorRect.height * percentageRect.height);
        }

        public virtual void ProcessEvents(Event e)
        {
            _mousePosition = e.mousePosition;
            _mousePosition.x -= _offset.x;
            _mousePosition.y -= _offset.y;
        }

        public virtual void SetNewFieldPosition(Vector2 fieldPosition)
        {

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
