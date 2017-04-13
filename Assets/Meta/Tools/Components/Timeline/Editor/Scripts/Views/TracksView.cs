using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    /// <summary>
    /// Tracks View consist of other Views, all of them share ability to scroll and scale tracks 
    /// representation. Currently it's CornerRectView, TracksHeadersView and WorkView, that consists 
    /// of number of subviews.
    /// </summary>
    public class TracksView : TimelineViewBase
    {
        #region Public Variables
        private static int _selectedTrack = -1;
        public static int SelectedTrack
        {
            get
            {
                return _selectedTrack;
            }
            set
            {
                Tracks.SelectedTrack = value;
                _selectedTrack = value;
            }
        }

        //Layout
        public float TracksHeadersFixedSize = 400f;
        public float TracksHeadersPercentage = 0.25f;
        public float TimelineFixedSize = 36f;
        public float TimelinePercentage = 0.12f;
        public float TimelineBarFixedSize = 36f;
        public float TimelineBarPercentage = 0.12f;

        //Scrolling Related Variables
        public bool FieldManuallyControlled = false;
        #endregion

        #region Private Variables
        private Vector2Interpolator _vector2Interpolator = new Vector2Interpolator();

        private Vector2 _tracksField;
        private Rect _tracksViewRect;
        private bool _fieldPositionIsManuallyControlled = false;
        
        private float _lastTime = 0f;
        private bool _updatesSubscribed = false;
        private bool _newVector2Subscribed = false;

        //Views
        private WorkView _workView;
        private TracksHeadersView _tracksHeadersView;
        private TopPanelView _topPanelView;
        private SecondPanelView _secondPanelView;
        #endregion

        #region  Public Properties
        public Vector2 TracksField
        {
            get
            {
                return _tracksField;
            }
            set
            {
                _tracksField = value;
            }
        }
        #endregion

        #region Constructor
        public TracksView()
        {
            createViews();
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            if (_workView == null || _tracksHeadersView == null || _topPanelView == null || _secondPanelView == null)
            {
                createViews();
                return;
            }

            TracksHeadersPercentage = TracksHeadersFixedSize / _rect.width;
            TimelinePercentage = TimelineFixedSize / _rect.height;
            TimelineBarPercentage = TimelineBarFixedSize / _rect.height;

            _topPanelView.UpdateView(_rect, new Rect(0, 0, 0, 1f - TimelinePercentage), _offset, _timeline, _serializedObject, _timelineWindow, this);
            _workView.UpdateView(_rect, new Rect(TracksHeadersPercentage, 0, 0, 0), _offset, _timeline, _serializedObject, _timelineWindow, this);
            _tracksHeadersView.UpdateView(_rect, new Rect(0, TimelinePercentage + TimelineBarPercentage, 1f - TracksHeadersPercentage, 0), _offset, _timeline, _serializedObject, _timelineWindow, this);
            _secondPanelView.UpdateView(_rect, new Rect(0, TimelinePercentage, 0, 1f - TimelinePercentage - TimelineBarPercentage), _offset, _timeline, _serializedObject, _timelineWindow, this);

            _tracksViewRect = GetRelativeRect(_rect, new Rect(TracksHeadersPercentage, TimelinePercentage + TimelineBarPercentage, 0, 0));
            //_tracksViewRect = new Rect(_rect.width * TracksHeadersPercentage, _rect.height * (TimelinePercentage + TimelineBarPercentage), _rect.width - _rect.width * TracksHeadersPercentage, _rect.height - _rect.height * (TimelinePercentage + TimelineBarPercentage));
        }

        protected Rect GetRelativeRect(Rect parentRect, Rect percentageRect)
        {
            return new Rect(parentRect.width * percentageRect.x, parentRect.height * percentageRect.y, parentRect.width - parentRect.width * percentageRect.x - parentRect.width * percentageRect.width, parentRect.height - parentRect.height * percentageRect.y - parentRect.height * percentageRect.height);
        }


        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.MouseUp:
                    FieldManuallyControlled = false;
                    break;
            }

            _workView.ProcessEvents(e);
            _tracksHeadersView.ProcessEvents(e);
            _topPanelView.ProcessEvents(e);
            _secondPanelView.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.MouseUp:
                    GUIUtility.keyboardControl = 0;
                    break;
            }

            OnEditorPlay();
        }
        #endregion

        #region Utility Methods
        private void createViews()
        {
            _workView = new WorkView();
            _tracksHeadersView = new TracksHeadersView();
            _topPanelView = new TopPanelView();
            _secondPanelView = new SecondPanelView();
        }

        #region Interpolation

        #region Navigation
        private Vector2 _initialMousePos;
        private Vector2 _initialFieldPos;
        public void NavigationStart(Vector2 mousePosition)
        {
            _initialMousePos = mousePosition;
            _initialFieldPos.x = TracksField.x;
            _initialFieldPos.y = TracksField.y;

            FieldManuallyControlled = true;
        }

        public void Navigate(Vector2 mousePosition)
        {
            SetTargetFieldPosition(new Vector2(_initialFieldPos.x + mousePosition.x - _initialMousePos.x, _initialFieldPos.y + mousePosition.y - _initialMousePos.y));
        }
        #endregion

        /*private void OnEnable()
        {
            //check for null views
            if (_workView == null || _cornerRectView == null || _tracksHeadersView == null)
            {
                createViews();
                return;
            }
        }

        private void OnFocus()
        {
            //check for null views
            if (_workView == null || _cornerRectView == null || _tracksHeadersView == null)
            {
                createViews();
                return;
            }
        }*/

        private void OnLostFocus()
        {
            //check for null views
            /*if (_workView == null || _cornerRectView == null || _tracksHeadersView == null)
            {
                createViews();
                return;
            }*/

            if (_updatesSubscribed)
            {
                EditorApplication.update -= editorUpdate;
                _updatesSubscribed = false;
            }

            if (_newVector2Subscribed)
            {
                unsubscribeOnNewVector2();
            }
        }

        public void SetCurrentFieldOfInterpolator()
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator.CurrentVector2 = new Vector2(TracksField.x, TracksField.y);
            }
        }

        public void SetCurrentVector2OfInterpolator(Vector2 vector2)
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator.CurrentVector2 = vector2;
            }
        }

        public void SetCurrentXOfInterpolator(float x)
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator.CurrentVector2 = new Vector2(x, TracksField.y);
            }
        }

        public void SetCurrentYOfInterpolator(float y)
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator.CurrentVector2 = new Vector2(TracksField.x, y);
            }
        }

        public void NavigateToKeynote(Keynote keynote)
        {
            SetCurrentFieldOfInterpolator();
            SetTargetFieldX(TimelineWindow.GetTimingsPosition(keynote.Timing) * -1f + keynote.Width);
        }

        public void NavigateToTiming(float timing)
        {
            if (timing < 0f)
            {
                timing = 0f;
            }
            else if (timing > _timeline.Duration)
            {
                timing = _timeline.Duration;
            }

            SetCurrentFieldOfInterpolator();
            SetTargetFieldX(TimelineWindow.GetTimingsPosition(timing) * -1f);
        }

        private void subscribeOnNewVector2()
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator._newVector2 += onNewVector2;
                _newVector2Subscribed = true;
            }
        }

        private void unsubscribeOnNewVector2()
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator._newVector2 -= onNewVector2;
                _newVector2Subscribed = false;
            }
        }

        private void editorUpdate()
        {
            if (_vector2Interpolator != null)
            {
                _vector2Interpolator.Update((float)EditorApplication.timeSinceStartup - _lastTime);
                _lastTime = (float)EditorApplication.timeSinceStartup;
            }
        }

        public void SetTargetFieldPosition(Vector2 target)
        {
            if (_vector2Interpolator != null)
            {
                if (!_newVector2Subscribed)
                {
                    subscribeOnNewVector2();
                }
                if (!_updatesSubscribed)
                {
                    _lastTime = (float)EditorApplication.timeSinceStartup;
                    EditorApplication.update += editorUpdate;
                    _updatesSubscribed = true;
                }

                _vector2Interpolator.SetTargetVector2(target);
                _fieldPositionIsManuallyControlled = true;
            }
        }

        public void SetTargetFieldY(float target)
        {
            if (_vector2Interpolator != null)
            {
                if (!_newVector2Subscribed)
                {
                    subscribeOnNewVector2();
                }
                if (!_updatesSubscribed)
                {
                    _lastTime = (float)EditorApplication.timeSinceStartup;
                    EditorApplication.update += editorUpdate;
                    _updatesSubscribed = true;
                }
                
                _vector2Interpolator.SetTargetVector2(new Vector2(TracksField.x, target));
                _fieldPositionIsManuallyControlled = true;
            }
        }

        public void OnEditorPlay()
        {
            /*
             * When slider moves, we must follow it, if it's not in the beginning or end and if user 
             * not navigate manually.
             * So the logic is when slider isn't in the center we try to return it in the center
             */

            if (_timeline != null && _timeline.Playing && !FieldManuallyControlled)
            {
                float center = /*_tracksViewRect.x + */_tracksViewRect.width / 2f;
                if (_timeline.TimeVector >= 0f)
                {
                    float centerEnding = center + 10f;
                    if (_workView.LocalSliderPosition + TracksField.x > centerEnding && _workView.EndGlobalPositionTiming < _timeline.Duration)
                    {
                        SetTargetFieldX(centerEnding - _workView.LocalSliderPosition);
                    }
                    else if (_workView.LocalSliderPosition + TracksField.x < 0f)
                    {
                        SetTargetFieldX(0f);
                    }
                }
                else
                {
                    float centerBeginning = center - 10f;
                    if (_workView.LocalSliderPosition + TracksField.x < centerBeginning && _workView.StartGlobalPositionTiming > 0f)
                    {
                        SetTargetFieldX(centerBeginning - _workView.LocalSliderPosition);
                    }
                    else if (_workView.LocalSliderPosition + TracksField.x > _tracksViewRect.width)
                    {
                        SetTargetFieldX((_workView.LocalSliderPosition + TracksField.x) * -1f);
                    }
                }
            }
        }

        public void SetTargetFieldX(float target)
        {
            if (_vector2Interpolator != null)
            {
                if (!_newVector2Subscribed)
                {
                    subscribeOnNewVector2();
                }
                if (!_updatesSubscribed)
                {
                    _lastTime = (float)EditorApplication.timeSinceStartup;
                    EditorApplication.update += editorUpdate;
                    _updatesSubscribed = true;
                }

                _vector2Interpolator.SetTargetVector2(new Vector2(target, TracksField.y));
                _fieldPositionIsManuallyControlled = true;
            }
        }

        private void onNewVector2(Vector2 newVector2)
        {
            TracksField = newVector2;
            _workView.SetNewFieldPosition(newVector2);
            _tracksHeadersView.SetNewFieldPosition(newVector2);

            //when we not draging - we must let tracks headers to return to it's recommended bounds
            if (_timeline != null)
            {
                if (!_fieldPositionIsManuallyControlled)
                {
                    float maxFieldYOffset = (_timeline.PixelPerTrack * (_timeline.Tracks.MaxTracksIndex + 32) - _tracksViewRect.height) * -1f;
                    float maxFieldXOffset = (_timeline.PixelPerSecond * (_timeline.Duration + 10f) - _tracksViewRect.width) * -1f;

                    if (TracksField.y > 0f && TracksField.x > 0f)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(0f, 0f));
                    }
                    else if (TracksField.y < maxFieldYOffset && TracksField.x < maxFieldXOffset)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(maxFieldXOffset, maxFieldYOffset));
                    }
                    else if (TracksField.x > 0f)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(0f, TracksField.y));
                    }
                    else if (TracksField.y > 0f)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(TracksField.x, 0f));
                    }
                    else if (TracksField.x < maxFieldXOffset)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(maxFieldXOffset, TracksField.y));
                    }
                    else if (TracksField.y < maxFieldYOffset)
                    {
                        _vector2Interpolator.SetTargetVector2(new Vector2(TracksField.x, maxFieldYOffset));
                    }
                }
            }

            _fieldPositionIsManuallyControlled = false;
        }
        #endregion Interpolation
        #endregion
    }
}
