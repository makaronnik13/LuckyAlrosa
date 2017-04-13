using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class WindowsView : TimelineViewBase
    {
        #region Public Variables
        public MetaKeyframeBase CurrentDrawedKeyframe;
        public MetaInterpolationBase CurrentDrawedInterpolation;

        public Vector2 ClickPosition;
        #endregion

        #region Private Variables
        private Rect _windowRect = new Rect(-1000, -1000, 0, 0);
        #endregion

        #region Constructor

        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            DrawWindows();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                    if (CurrentDrawedKeyframe != null || CurrentDrawedInterpolation != null)
                    {
                        if (!_windowRect.Contains(_mousePosition))
                        {
                            ResetWindow();
                            CurrentDrawedKeyframe = null;
                            CurrentDrawedInterpolation = null;
                        }
                        /*else
                        {
                            e.Use();
                        }*/
                    }
                    break; 
            }

            /*if (CurrentDrawedKeyframe != null)
            {
                switch (e.GetTypeForControl(CurrentDrawedKeyframe.GetHashCode()))
                {
                    case EventType.MouseDown:
                        if (!_windowRect.Contains(_mousePosition))
                        {
                            ResetWindow();
                            CurrentDrawedKeyframe = null;
                        }
                        //GUIUtility.hotControl = CurrentDrawedKeyframe.GetHashCode();
                        break;
                    case EventType.MouseUp:
                        if (!_windowRect.Contains(_mousePosition))
                        {
                            ResetWindow();
                            CurrentDrawedKeyframe = null;
                        }
                        break;
                }
            }
            else  if (CurrentDrawedInterpolation != null)
            {
                switch (e.GetTypeForControl(CurrentDrawedInterpolation.GetHashCode()))
                {
                    case EventType.MouseDown:
                        if (!_windowRect.Contains(_mousePosition))
                        {
                            CurrentDrawedInterpolation = null;
                            ResetWindow();
                        }
                        break;
                    case EventType.MouseUp:
                        if (!_windowRect.Contains(_mousePosition))
                        {
                            CurrentDrawedInterpolation = null;
                            ResetWindow();
                        }
                        break;
                }
            }*/
        }
        #endregion

        #region Draw Windows

        public void ResetWindow()
        {
            _windowRect = new Rect(-1000, -1000, 0, 0);
        }

        public void DrawWindows()
        {
            if (CurrentDrawedKeyframe != null)
            {
                if (_windowRect.x <= -1000)
                {
                    _windowRect.x = ClickPosition.x + CurrentDrawedKeyframe.BodyRect.width / 2;
                    _windowRect.y = ClickPosition.y + CurrentDrawedKeyframe.BodyRect.height + 10f;
                    _windowRect.width = CurrentDrawedKeyframe.ParametersWindowWidth;
                    _windowRect.height = CurrentDrawedKeyframe.ParametersWindowHeight;

                    if (_windowRect.x < 0f)
                    {
                        _windowRect.x = 0f;
                    }
                    else if (_windowRect.x > _rect.width - _windowRect.width)
                    {
                        _windowRect.x = _rect.width - _windowRect.width;
                    }
                    if (_windowRect.y < 0f)
                    {
                        _windowRect.y = 0f;
                    }
                    else if (_windowRect.y > _rect.height - _windowRect.height)
                    {
                        _windowRect.y = _rect.height - _windowRect.height;
                    }
                }

                _timelineWindow.BeginWindows();
                _windowRect = GUI.Window(CurrentDrawedKeyframe.GetHashCode(), _windowRect, DoMyWindow, CurrentDrawedKeyframe.ParametersWindowTitle, Utilities.TimelineSkin.window);
                _timelineWindow.EndWindows();
            }

            if (CurrentDrawedInterpolation != null)
            {
                if (_windowRect.x <= -1000)
                {
                    _windowRect.x = ClickPosition.x + CurrentDrawedInterpolation.BodyRects[0].width / 2;
                    _windowRect.y = ClickPosition.y + CurrentDrawedInterpolation.BodyRects[0].height + 10f;
                    _windowRect.width = CurrentDrawedInterpolation.ParametersWindowWidth;
                }

                _timelineWindow.BeginWindows();
                _windowRect = GUI.Window(CurrentDrawedInterpolation.GetHashCode(), _windowRect, DoMyWindow, CurrentDrawedInterpolation.ParametersWindowTitle, Utilities.TimelineSkin.window);
                _windowRect.height = CurrentDrawedInterpolation.ParametersWindowHeight;
                _timelineWindow.EndWindows();
            }
        }

        private void DoMyWindow(int windowID)
        {
            if (CurrentDrawedKeyframe != null)
            {
                EditorGUI.BeginChangeCheck();
                CurrentDrawedKeyframe.DrawParametersWindow();
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.Update();
                    _serializedObject.ApplyModifiedProperties();

                    _windowRect.height = CurrentDrawedKeyframe.ParametersWindowHeight;
                }
            }
            if (CurrentDrawedInterpolation != null)
            {
                EditorGUI.BeginChangeCheck();
                CurrentDrawedInterpolation.DrawParametersWindow();
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.Update();
                    _serializedObject.ApplyModifiedProperties();

                    _windowRect.height = CurrentDrawedInterpolation.ParametersWindowHeight;
                }
            }
            GUI.DragWindow();

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                    Event.current.Use();
                    /*if (CurrentDrawedKeyframe != null || CurrentDrawedInterpolation != null)
                    {
                        if (!_windowRect.Contains(Event.current.mousePosition))
                        {
                            ResetWindow();
                            CurrentDrawedKeyframe = null;
                            CurrentDrawedInterpolation = null;
                        }
                        else
                        {
                            Event.current.Use();
                        }
                    }*/
                    break;
            }

            //GUIUtility.hotControl = windowID;
            /*if (_windowRect.Contains(mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        Event.current.Use();
                        break;
                }
            }*/
            /*switch (Event.current.GetTypeForControl(windowID))
            {
                case EventType.MouseUp:
                    Event.current.Use();
                    break;
            }*/
        }
        #endregion
    }
}
