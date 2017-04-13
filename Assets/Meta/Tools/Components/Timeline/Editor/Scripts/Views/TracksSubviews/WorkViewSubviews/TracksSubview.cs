using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;

namespace Meta.Tools.Editor
{
    [Serializable]
    public class TracksSubview : WorkViewBase
    {
        #region Public Variables
        public static MetaKeyframeBase CurrentSelectedKeyframe;
        public static MetaKeyframeBase CurrentManipulatedKeyframe;
        public static MetaInterpolationBase CurrentManipulatedInterpolation;
        public static MetaInterpolationBase CurrentSelectedInterpolation;
        #endregion

        #region Private Variables
        private float _doubleClickTime = 0.3f;
        private float _lastClickOnKeyframeTime;
        private float _lastClickOnInterpolationTime;

        private Color _trackBackgroundColor = new Color(194f / 255f, 194f / 255f, 194f / 255f);

        private Texture2D _grayLine;
        private Texture2D _transparent;
        
        private bool _dragingFromKeyframe = false;

        private MetaKeyframeBase[] keyframeBuffer = null;
        private MetaInterpolationBase[] interpolationBuffer = null;

        private List<WorkView.Subview> _subviews = new List<WorkView.Subview>();

        private static Texture2D _trackBackgroundActive;
        private static Texture2D _trackBackgroundPassive;
        private static Texture2D _trackSelectedActive;
        private static Texture2D _trackSelectedPassive;
        private static Texture2D _trackUnselectedActive;
        private static Texture2D _trackUnselectedPassive;

        private Vector2 _offset;
        #endregion

        #region Constructor
        public TracksSubview()
        {
            _trackBackgroundActive = Resources.Load("Textures/NewDesign/track_active_background") as Texture2D;
            _trackBackgroundPassive = Resources.Load("Textures/NewDesign/track_passive_background") as Texture2D;
            _trackSelectedActive = Resources.Load("Textures/NewDesign/track_active_background_selected") as Texture2D;
            _trackSelectedPassive = Resources.Load("Textures/NewDesign/track_passive_background_selected") as Texture2D;
            _trackUnselectedActive = Resources.Load("Textures/NewDesign/track_active_background_unselected") as Texture2D;
            _trackUnselectedPassive = Resources.Load("Textures/NewDesign/track_passive_background_unselected") as Texture2D;
        }
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);

            //Debug.Log("tracks. _field.y = " + _field.y);

            WorkView.Subview newSubview = new WorkView.Subview();
            newSubview.RealRect = _realRect;
            newSubview.RealRect.y = globalDrawingRect.y;
            newSubview.Timing0 = _timing0;
            newSubview.Timing1 = _timing1;
            newSubview.ChapterTiming0 = _chapterTiming0;
            newSubview.ChapterTiming1 = _chapterTiming1;
            newSubview.UpdateInstanceID();
            _subviews.Add(newSubview);

            _offset = new Vector2(_viewRect.x + parentOffset.x, _viewRect.y + parentOffset.y);
            GUILayout.BeginArea(_viewRect);
            drawTracks(newSubview);
            GUILayout.EndArea();
        }

        protected Vector2 lastMousePosition;
        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            mousePosition = e.mousePosition;
            mousePosition.x -= _offset.x;
            mousePosition.y -= _offset.y;

            if (e.type == EventType.ValidateCommand)
            { // Validate the command
                if (e.commandName == "Copy" || e.commandName == "Paste")
                {
                    e.Use(); // without this line we won't get ExecuteCommand
                }

            }
            else if (e.type == EventType.ExecuteCommand)
            { // Execute the command

                if (e.commandName == "Copy")
                {
                    performCopying();
                }
                else if (e.commandName == "Paste")
                {
                    WorkView.Subview activeSubview = null;
                    Vector2 correctedMousePosition = lastMousePosition;
                    correctedMousePosition.y += _realRect.y - _field.y;
                    for (int i = 0; i < _subviews.Count; i++)
                    {
                        if (_subviews[i].RealRect.Contains(correctedMousePosition))
                        {
                            activeSubview = _subviews[i];
                            break;
                        }
                    }

                    int row = (int)Mathf.Floor((lastMousePosition.y - _field.y) / _timeline.PixelPerTrack);
                    float second = getPositionsTime(lastMousePosition.x, activeSubview);

                    paste(activeSubview, row, second);
                }
            }

            if (mousePosition.y >= 0 && mousePosition.x >= 0)
            {
                lastMousePosition = mousePosition;
            }
            /*if (e.type == EventType.Layout)
            {
                lastMousePosition = mousePosition;
                Debug.Log("lastMousePosition = " + lastMousePosition);
            }*/

            //Debug.Log("_viewRect = " + _viewRect);
            //Debug.Log("mousePosition = " + mousePosition);
            processInteraction(e);
            Rect rect = _viewRect;
            rect.y = 0;
            if (rect.Contains(mousePosition))
            {
                processDrop(e);

                if (_pressedLMB)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown:
                            _workView.NavigationStart(mousePosition);
                            break;
                        case EventType.MouseDrag:
                            if (CurrentManipulatedKeyframe == null && CurrentManipulatedInterpolation == null)
                            {
                                _workView.Navigate(mousePosition);
                            }
                            break;
                    }
                }
                else if (_pressedRMB)
                {

                }

                if (e.type == EventType.ScrollWheel)
                {
                    float horizontalDelta = e.delta.x;
                    float verticalDelta = e.delta.y * -1f;

                    _workView.NavigationStart(new Vector2(0f, 0f));
                    _workView.Navigate((new Vector2(horizontalDelta, verticalDelta)) * 15f);
                    /*if (horizontalDelta != 0f || verticalDelta != 0f)
                    {
                        if (!scrolled)
                        {
                            _workView.NavigationStart(new Vector2(0f, 0f));
                        }
                        else
                        {
                            _workView.Navigate(new Vector2(horizontalDelta, verticalDelta));
                        }
                        scrolled = true;
                    }
                    else
                    {
                        scrolled = false;
                    }*/
                }
                /*else
                {
                    scrolled = false;
                }*/

            }
        }

        //private bool scrolled = false;

        private void processDrop(Event e)
        {
            /*WorkView.Subview activeSubview = null;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(mousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }*/

            WorkView.Subview activeSubview = null;
            Vector2 correctedMousePosition = mousePosition;
            correctedMousePosition.y += _realRect.y - _field.y; ;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(correctedMousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }

            if (e.type == EventType.DragPerform)
            {
                //Debug.Log("drag");
            }

            if (activeSubview != null)
            {
                switch (e.type)
                {
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();

                        //Vector2 mousePosition = Event.current.mousePosition;
                        int row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
                        float second = getPositionsTime(mousePosition.x, activeSubview);

                        processDroping(DragAndDrop.objectReferences, row, second);
                        break;
                }
            }

            switch (e.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;
            }
        }

        private void performCopying()
        {
            if (CurrentSelectedKeyframe != null)
            {
                interpolationBuffer = null;
                keyframeBuffer = new MetaKeyframeBase[1];
                keyframeBuffer[0] = CurrentSelectedKeyframe;
            }
            else if (CurrentSelectedInterpolation != null)
            {
                keyframeBuffer = null;
                interpolationBuffer = new MetaInterpolationBase[1];

                interpolationBuffer[0] = CurrentSelectedInterpolation;
            }
        }

        private float RequiredTiming(int row)
        {
            float requiredTiming = _timeline.CurrentTime;
            TimelineTrack track = _timeline.Tracks.GetTrackOnTracksViewPosition(row);
            if (track != null)
            {
                for (int i = 0; i < track.Keyframes.Count; i++)
                {
                    if (track.Keyframes[i].Timing == requiredTiming)
                    {
                        requiredTiming += 1f;
                        if (requiredTiming > _timeline.Duration)
                        {
                            requiredTiming = _timeline.Duration;
                        }
                        break;
                    }
                }
            }
            return requiredTiming;
        }

        private MetaKeyframeBase InterpolationFrom(int row, float requiredTiming)
        {
            MetaKeyframeBase interpolateFrom = null;
            TimelineTrack track = _timeline.Tracks.GetTrackOnTracksViewPosition(row);
            if (track != null && track.InterpolationModeKeyframeAdding)
            {
                //if that's the selected mode - we make interpolation to previous keyframe automatically
                for (int i = 0; i < track.Keyframes.Count; i++)
                {
                    if ((interpolateFrom == null && track.Keyframes[i].Timing < requiredTiming) || (track.Keyframes[i].Timing < requiredTiming && track.Keyframes[i].Timing > interpolateFrom.Timing))
                    {
                        interpolateFrom = track.Keyframes[i];
                    }
                }
            }
            return interpolateFrom;
        }


        private void paste(WorkView.Subview activeSubview, int row = 0, float timing = 0f)
        {
            if (keyframeBuffer != null && keyframeBuffer.Length > 0)
            {
                Undo.RecordObject(_timeline, "Paste Keyframe");
                try
                {
                    //float requiredTiming = RequiredTiming(row);
                    MetaKeyframeBase interpolateFrom = InterpolationFrom(row, timing);
                    MetaKeyframeBase newKeyframe = _timeline.AddKeyframeOnTrack((MetaKeyframeBase)keyframeBuffer[0].Clone(), row, timing);
                    
                    if (interpolateFrom != null && newKeyframe != null)
                    {
                        _timeline.AddInterpolation(interpolateFrom, newKeyframe, "");
                    }
                }
                catch (Exception e)
                {
                    TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                }
            }
            else if (interpolationBuffer != null && interpolationBuffer.Length > 0)
            {
                Undo.RecordObject(_timeline, "Paste Interpolation");
                MetaInterpolationBase interpolation = (MetaInterpolationBase)interpolationBuffer[0].Clone();
                float timing0 = timing - (interpolationBuffer[0].Keyframes[1].Timing - interpolationBuffer[0].Keyframes[0].Timing) / 2f;
                float timing1 = timing + (interpolationBuffer[0].Keyframes[1].Timing - interpolationBuffer[0].Keyframes[0].Timing) / 2f;
                if (timing0 < 0f)
                {
                    timing0 = 0f;
                }
                else if (timing0 > _timeline.Duration)
                {
                    timing0 = _timeline.Duration;
                }
                if (timing1 < 0f)
                {
                    timing1 = 0f;
                }
                else if (timing1 > _timeline.Duration)
                {
                    timing1 = _timeline.Duration;
                }
                try
                {
                    _timeline.AddKeyframeOnTrack(interpolation.Keyframes[0], row, timing0);
                    _timeline.AddKeyframeOnTrack(interpolation.Keyframes[1], row, timing1);
                    _timeline.AddInterpolation(interpolation);
                }
                catch (Exception e)
                {
                    TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                }
            }
        }

        private void processInteraction(Event e)
        {
            /*WorkView.Subview activeSubview = null;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(mousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }*/

            WorkView.Subview activeSubview = null;
            Vector2 correctedMousePosition = mousePosition;
            correctedMousePosition.y += _realRect.y - _field.y;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(correctedMousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }

            if (activeSubview != null)
            {
                /*switch (e.type)
                {
                    case EventType.KeyUp:
                        //case when we pasting something on the mouseposition
                        switch (e.keyCode)
                        {
                            case KeyCode.V:
                                if (control || e.command || e.control || e.keyCode == KeyCode.LeftCommand || e.keyCode == KeyCode.LeftApple ||
                                        e.keyCode == KeyCode.RightCommand || e.keyCode == KeyCode.RightApple)
                                {
                                    int row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
                                    float second = getPositionsTime(mousePosition.x, activeSubview);

                                    paste(activeSubview, row, second);
                                }
                                break;
                        }
                        break;
                }*/

                if (_pressedLMB && !_pressedRMB)
                {
                    if (CurrentManipulatedKeyframe != null)
                    {
                        switch (/*e.GetTypeForControl(CurrentManipulatedKeyframe.ControlID)*/e.type)
                        {
                            case EventType.MouseDrag:
                                //draging keyframe
                                //float timeDelta = getPositionsTime(mousePosition.x, activeSubview) - getPositionsTime(CurrentManipulatedKeyframe.StartDragingPosition.x, activeSubview);
                                float newTime = getPositionsTime(mousePosition.x, activeSubview);

                                if (TimelineWindow.SnappingOn)
                                {
                                    float secondsToSnap = TimelineWindow.PixelsToSnap / _timeline.PixelPerSecond;

                                    List<float> allTimings = _timeline.GetAllTimings(CurrentManipulatedKeyframe);

                                    float minimumDelta = secondsToSnap + 1;
                                    int minimumIndex = -1;

                                    for (int i = 0; i < allTimings.Count; i++)
                                    {
                                        float delta = Mathf.Abs(allTimings[i] - newTime);
                                        if (delta < minimumDelta)
                                        {
                                            minimumDelta = delta;
                                            minimumIndex = i;
                                        }
                                    }

                                    if (minimumDelta <= secondsToSnap)
                                    {
                                        newTime = allTimings[minimumIndex];
                                    }
                                }

                                //float timeDelta = newTime - getPositionsTime(CurrentManipulatedKeyframe.StartDragingPosition.x, activeSubview);

                                CurrentManipulatedKeyframe.Timing = newTime;
                                /*float timeDelta = newTime - getPositionsTime(CurrentManipulatedKeyframe.StartDragingPosition.x, activeSubview);

                                CurrentManipulatedKeyframe.Timing = CurrentManipulatedKeyframe.PreviousTiming + timeDelta;*/
                                if (CurrentManipulatedKeyframe.Timing < 0f)
                                {
                                    CurrentManipulatedKeyframe.Timing = 0f;
                                }
                                else if (CurrentManipulatedKeyframe.Timing > _timeline.Duration)
                                {
                                    CurrentManipulatedKeyframe.Timing = _timeline.Duration;
                                }

                                e.Use();
                                break;
                        }
                    }
                    else if (CurrentManipulatedInterpolation != null)
                    {
                        switch (e.GetTypeForControl(CurrentManipulatedInterpolation.ControlID))
                        {
                            case EventType.MouseDrag:
                                //draging interpolation
                                float newTime = getPositionsTime(mousePosition.x, activeSubview);
                                float timeDelta = newTime - getPositionsTime(CurrentManipulatedInterpolation.StartDragingPosition.x, activeSubview);
                                float newKey0Timing = CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming + timeDelta;
                                float newKey1Timing = CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming + timeDelta;


                                if (TimelineWindow.SnappingOn)
                                {
                                    float secondsToSnap = TimelineWindow.PixelsToSnap / _timeline.PixelPerSecond;

                                    List<float> allTimings = _timeline.GetAllTimings(CurrentManipulatedInterpolation.Keyframes[0], CurrentManipulatedInterpolation.Keyframes[1]);

                                    float minimumDelta = secondsToSnap + 1;
                                    int minimumIndex = -1;

                                    for (int i = 0; i < allTimings.Count; i++)
                                    {
                                        float delta = Mathf.Abs(allTimings[i] - newKey0Timing);
                                        if (delta < minimumDelta)
                                        {
                                            minimumDelta = delta;
                                            minimumIndex = i;
                                        }
                                    }

                                    if (minimumDelta <= secondsToSnap)
                                    {
                                        float delta = allTimings[minimumIndex] - newKey0Timing;
                                        newKey0Timing = allTimings[minimumIndex];
                                        newKey1Timing += delta;
                                    }
                                    else
                                    {
                                        minimumDelta = secondsToSnap + 1;
                                        minimumIndex = -1;

                                        for (int i = 0; i < allTimings.Count; i++)
                                        {
                                            float delta = Mathf.Abs(allTimings[i] - newKey1Timing);
                                            if (delta < minimumDelta)
                                            {
                                                minimumDelta = delta;
                                                minimumIndex = i;
                                            }
                                        }

                                        if (minimumDelta <= secondsToSnap)
                                        {
                                            float delta = allTimings[minimumIndex] - newKey1Timing;
                                            newKey1Timing = allTimings[minimumIndex];
                                            newKey0Timing += delta;
                                        }
                                    }
                                }
                                CurrentManipulatedInterpolation.Keyframes[0].Timing = newKey0Timing;
                                CurrentManipulatedInterpolation.Keyframes[1].Timing = newKey1Timing;
                                //CurrentManipulatedInterpolation.Keyframes[0].Timing = CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming + timeDelta;
                                //CurrentManipulatedInterpolation.Keyframes[1].Timing = CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming + timeDelta;
                                if (CurrentManipulatedInterpolation.Keyframes[0].Timing < 0f)
                                {
                                    CurrentManipulatedInterpolation.Keyframes[0].Timing = 0f;
                                    CurrentManipulatedInterpolation.Keyframes[1].Timing = 0f + (CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming - CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming);
                                }
                                else if (CurrentManipulatedInterpolation.Keyframes[1].Timing < 0f)
                                {
                                    CurrentManipulatedInterpolation.Keyframes[1].Timing = 0f;
                                    CurrentManipulatedInterpolation.Keyframes[0].Timing = 0f + (CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming - CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming);
                                }
                                else if (CurrentManipulatedInterpolation.Keyframes[0].Timing > _timeline.Duration)
                                {
                                    CurrentManipulatedInterpolation.Keyframes[0].Timing = _timeline.Duration;
                                    CurrentManipulatedInterpolation.Keyframes[1].Timing = _timeline.Duration - (CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming - CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming);
                                }
                                else if (CurrentManipulatedInterpolation.Keyframes[1].Timing > _timeline.Duration)
                                {
                                    CurrentManipulatedInterpolation.Keyframes[1].Timing = _timeline.Duration;
                                    CurrentManipulatedInterpolation.Keyframes[0].Timing = _timeline.Duration - (CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming - CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming);
                                }

                                e.Use();
                                break;
                        }
                    }
                }
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_pressedLMB || _pressedRMB)
                    {
                        //on mouse down we just remembering currently manipulated keyframe or interpolation
                        if (CurrentSelectedKeyframe != null)
                        {
                            CurrentSelectedKeyframe.IsSelected = false;
                        }
                        CurrentSelectedKeyframe = null;
                        if (CurrentSelectedInterpolation != null)
                        {
                            CurrentSelectedInterpolation.IsSelected = false;
                        }
                        CurrentSelectedInterpolation = null;

                        if (CurrentManipulatedKeyframe != null)
                        {
                            CurrentManipulatedKeyframe.IsManipulated = false;
                        }
                        CurrentManipulatedKeyframe = null;
                        if (CurrentManipulatedInterpolation != null)
                        {
                            CurrentManipulatedInterpolation.IsManipulated = false;
                        }
                        CurrentManipulatedInterpolation = null;

                        checkKeyframeBodiesCycle(mousePosition, ref CurrentManipulatedKeyframe);
                        if (CurrentManipulatedKeyframe == null)
                        {
                            checkInterpolationsBodiesCycle(mousePosition, ref CurrentManipulatedInterpolation);

                            if (CurrentManipulatedInterpolation != null)
                            {
                                CurrentManipulatedInterpolation.IsManipulated = true;
                                //Debug.Log("IsManipulated");
                            }
                        }
                        else
                        {
                            CurrentManipulatedKeyframe.IsManipulated = true;
                        }
                    }

                    if (_pressedRMB && !_pressedLMB && CurrentManipulatedKeyframe != null)
                    {
                        _dragingFromKeyframe = true;
                    }
                    else
                    {
                        _dragingFromKeyframe = false;
                    }
                    break;
                case EventType.MouseUp:
                    _dragingFromKeyframe = false;

                    if (e.button == 0)
                    {
                        if (CurrentManipulatedKeyframe != null && Mathf.Abs(mousePosition.x - CurrentManipulatedKeyframe.StartDragingPosition.x) > 2f)
                        {
                            //After draging keyframe with LMB - move to new location
                            performKeyframeMoving();
                        }
                        else if (CurrentManipulatedInterpolation != null && Mathf.Abs(mousePosition.x - CurrentManipulatedInterpolation.StartDragingPosition.x) > 2f)
                        {
                            //After draging interpolation with LMB - move to new location
                            performInterpolationMoving();
                        }
                        else
                        {
                            //After clicking with LMB - selecting
                            performSelecting();
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (CurrentManipulatedKeyframe != null && Mathf.Abs(mousePosition.x - CurrentManipulatedKeyframe.StartDragingPosition.x) > 2f)
                        {
                            //After draging keyframe with RMB - creating interpolation
                            createInterpolation();
                        }
                        else if (CurrentManipulatedInterpolation != null && Mathf.Abs(mousePosition.x - CurrentManipulatedInterpolation.StartDragingPosition.x) > 2f)
                        {
                            //After draging interpolation with RMB
                        }
                        else
                        {
                            //After clicking with RMB - open keyframe or interpolation settings
                            openWindow();
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    break;
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.Backspace:
                            if (e.command || e.control || e.keyCode == KeyCode.LeftCommand || e.keyCode == KeyCode.LeftApple ||
                                     e.keyCode == KeyCode.RightCommand || e.keyCode == KeyCode.RightApple)
                            {
                                performDelete();
                            }
                            break;
                        case KeyCode.Delete:
                            performDelete();
                            break;
                        /*case KeyCode.C:
                            if (control || e.command || e.control || e.keyCode == KeyCode.LeftCommand || e.keyCode == KeyCode.LeftApple ||
                                     e.keyCode == KeyCode.RightCommand || e.keyCode == KeyCode.RightApple)
                            {
                                performCopying();
                            }
                            break;*/
                        case KeyCode.S:
                            if (CurrentSelectedKeyframe != null)
                            {
                                CurrentSelectedKeyframe.Fetch();
                            }
                            break;
                        case KeyCode.N:
                            int row = -1;
                            float second = -1f;
                            row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
                            if (Tracks.SelectedTrack >= 0)
                            {
                                if (row != Tracks.SelectedTrack)
                                {
                                    row = Tracks.SelectedTrack;
                                }
                                else if (activeSubview != null)
                                {
                                    second = getPositionsTime(lastMousePosition.x, activeSubview);
                                }
                            }
                            else if (activeSubview != null)
                            {
                                second = getPositionsTime(lastMousePosition.x, activeSubview);
                            }
                            _timeline.CreateNewKeyframeButtonPressed(_timeline.Tracks.GetTrackOnTracksViewPosition(row), second);
                            break;
                    }
                    break;
            }

            if (activeSubview != null)
            {
                if (e.button == 1)
                {
                    switch (e.type)
                    {
                        case EventType.MouseUp:
                            if (CurrentManipulatedKeyframe == null && CurrentManipulatedInterpolation == null)
                            {
                                ProcessContextMenu(mousePosition, activeSubview);
                            }
                            break;
                    }
                }
            }

            switch (e.type)
            {
                case EventType.MouseUp:
                    if (CurrentManipulatedKeyframe != null)
                    {
                        CurrentManipulatedKeyframe.IsManipulated = false;
                    }
                    CurrentManipulatedKeyframe = null;
                    if (CurrentManipulatedInterpolation != null)
                    {
                        CurrentManipulatedInterpolation.IsManipulated = false;
                    }
                    CurrentManipulatedInterpolation = null;
                    break;
            }

            if (CurrentManipulatedKeyframe != null && _dragingFromKeyframe)
            {
                _workView.DrawLineBetween(new Vector2(CurrentManipulatedKeyframe.BodyRect.center.x, CurrentManipulatedKeyframe.BodyRect.center.y),
                    mousePosition,
                    Color.white);
            }
        }

        private void openWindow()
        {
            Vector2 absolutePosition = mousePosition + _offset;
            if (CurrentManipulatedKeyframe != null)
            {
                TimelineWindow.Instance.ShowWindow(absolutePosition, CurrentManipulatedKeyframe);
                //_workView.ShowWindow(CurrentManipulatedKeyframe);
            }
            else if (CurrentManipulatedInterpolation != null)
            {
                TimelineWindow.Instance.ShowWindow(absolutePosition, CurrentManipulatedInterpolation);
                //_workView.ShowWindow(CurrentManipulatedInterpolation);
            }
        }

        private void createInterpolation()
        {
            MetaKeyframeBase targetKeyframe = null;
            checkKeyframeBodiesCycle(mousePosition, ref targetKeyframe);
            if (targetKeyframe != null && !targetKeyframe.Equals(CurrentManipulatedKeyframe) && CurrentManipulatedKeyframe.Target.Equals(targetKeyframe.Target))
            {
                Undo.RecordObject(_timeline, "Animation created");
                MetaInterpolationBase resultInterpolation = _timeline.AddInterpolation(CurrentManipulatedKeyframe, targetKeyframe, "");
                if (resultInterpolation != null)
                {
                    _serializedObject.Update();
                }
            }

            if (CurrentSelectedKeyframe != null)
            {
                CurrentSelectedKeyframe.IsSelected = false;
            }
            CurrentSelectedKeyframe = null;
        }

        private void performInterpolationMoving()
        {
            float resultTiming0 = CurrentManipulatedInterpolation.Keyframes[0].Timing;
            float resultTiming1 = CurrentManipulatedInterpolation.Keyframes[1].Timing;
            CurrentManipulatedInterpolation.Keyframes[0].Timing = CurrentManipulatedInterpolation.Keyframes[0].PreviousTiming;
            CurrentManipulatedInterpolation.Keyframes[1].Timing = CurrentManipulatedInterpolation.Keyframes[1].PreviousTiming;
            Undo.RecordObject(_timeline, "Animation timings changed");
            CurrentManipulatedInterpolation.Keyframes[0].Timing = resultTiming0;
            CurrentManipulatedInterpolation.Keyframes[1].Timing = resultTiming1;
            _serializedObject.Update();
            if (CurrentManipulatedInterpolation != null)
            {
                CurrentManipulatedInterpolation.IsManipulated = false;
            }
            CurrentManipulatedInterpolation = null;
            if (CurrentSelectedInterpolation != null)
            {
                CurrentSelectedInterpolation.IsSelected = false;
            }
            CurrentSelectedInterpolation = null;
        }

        private void performSelecting()
        {
            if (CurrentSelectedKeyframe != null)
            {
                CurrentSelectedKeyframe.IsSelected = false;
            }
            CurrentSelectedKeyframe = null;
            if (CurrentSelectedInterpolation != null)
            {
                CurrentSelectedInterpolation.IsSelected = false;
            }
            CurrentSelectedInterpolation = null;

            if (CurrentManipulatedKeyframe != null)
            {
                CurrentManipulatedKeyframe.IsManipulated = false;
            }
            CurrentManipulatedKeyframe = null;
            if (CurrentManipulatedInterpolation != null)
            {
                CurrentManipulatedInterpolation.IsManipulated = false;
            }
            CurrentManipulatedInterpolation = null;

            checkKeyframeBodiesCycle(mousePosition, ref CurrentSelectedKeyframe);
            if (CurrentSelectedKeyframe == null)
            {
                checkInterpolationsBodiesCycle(mousePosition, ref CurrentSelectedInterpolation);
                if (CurrentSelectedInterpolation != null)
                {
                    CurrentSelectedInterpolation.IsSelected = true;

                    if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickOnInterpolationTime)
                    {
                        doubleClickOnInterpolation();
                    }
                    _lastClickOnInterpolationTime = (float)EditorApplication.timeSinceStartup;

                    if (CurrentSelectedInterpolation.Keyframes[0].Target != null && CurrentSelectedInterpolation.Keyframes[1].Target != null)
                    {
                        //Selection.objects = new UnityEngine.Object[] { (CurrentSelectedInterpolation.Keyframes[0].Target as Component).gameObject, (CurrentSelectedInterpolation.Keyframes[1].Target as Component).gameObject };

                        if (CurrentSelectedInterpolation.Keyframes[0].Target is GameObject)
                        {
                            Selection.objects = new UnityEngine.Object[] { CurrentSelectedInterpolation.Keyframes[0].Target, CurrentSelectedInterpolation.Keyframes[1].Target };
                        }
                        else
                        {
                            Selection.objects = new UnityEngine.Object[] { (CurrentSelectedInterpolation.Keyframes[0].Target as Component).gameObject, (CurrentSelectedInterpolation.Keyframes[1].Target as Component).gameObject };
                        }
                    }
                }
                else
                {
                    if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickOnInterpolationTime)
                    {
                        doubleClickedOnEmptySpace();
                    }
                    _lastClickOnInterpolationTime = (float)EditorApplication.timeSinceStartup;
                }
            }
            else
            {
                CurrentSelectedKeyframe.IsSelected = true;
                CurrentSelectedKeyframe.OnSelected();

                if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickOnKeyframeTime)
                {
                    doubleClickOnKeyframe();
                }
                _lastClickOnKeyframeTime = (float)EditorApplication.timeSinceStartup;
                if (CurrentSelectedKeyframe.Target != null)
                {
                    if (CurrentSelectedKeyframe.Target is GameObject)
                    {
                        Selection.objects = new UnityEngine.Object[] { CurrentSelectedKeyframe.Target };
                    }
                    else
                    {
                        Selection.objects = new UnityEngine.Object[] { (CurrentSelectedKeyframe.Target as Component).gameObject };
                    }
                }
            }
        }

        private void performKeyframeMoving()
        {
            float resultTiming = CurrentManipulatedKeyframe.Timing;
            CurrentManipulatedKeyframe.Timing = CurrentManipulatedKeyframe.PreviousTiming;
            Undo.RecordObject(_timeline, "Keyframe timing changed");
            CurrentManipulatedKeyframe.Timing = resultTiming;
            _serializedObject.Update();
            if (CurrentManipulatedKeyframe != null)
            {
                CurrentManipulatedKeyframe.IsManipulated = false;
            }
            CurrentManipulatedKeyframe = null;
            if (CurrentSelectedKeyframe != null)
            {
                CurrentSelectedKeyframe.IsSelected = false;
            }
            CurrentSelectedKeyframe = null;
        }

        private void doubleClickedOnEmptySpace()
        {
            WorkView.Subview activeSubview = null;
            Vector2 correctedMousePosition = mousePosition;
            correctedMousePosition.y += _realRect.y - _field.y; ;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(correctedMousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }
            if (activeSubview != null)
            {
                float second = getPositionsTime(mousePosition.x, activeSubview);
                _timeline.GoToTiming(second, Timeline.AnimationMode.IgnoreKeynotes);
            }
        }

        private void doubleClickOnKeyframe()
        {
            if (CurrentSelectedKeyframe != null)
            {
                _timeline.GoToTiming(CurrentSelectedKeyframe.Timing, Timeline.AnimationMode.IgnoreKeynotes, true);
                CurrentSelectedKeyframe.Set();
            }
        }

        private void performDelete()
        {
            if (CurrentSelectedKeyframe != null)
            {
                Undo.RecordObject(_timeline, "Keyframe Deleted");
                _timeline.RemoveKeyframe(CurrentSelectedKeyframe);
                if (CurrentSelectedKeyframe != null)
                {
                    CurrentSelectedKeyframe.IsSelected = false;
                }
                CurrentSelectedKeyframe = null;
            }
            else if (CurrentSelectedInterpolation != null)
            {
                Undo.RecordObject(_timeline, "Animation Deleted");
                _timeline.RemoveInterpolation(CurrentSelectedInterpolation);
                if (CurrentSelectedInterpolation != null)
                {
                    CurrentSelectedInterpolation.IsSelected = false;
                }
                CurrentSelectedInterpolation = null;
            }
        }

        private void doubleClickOnInterpolation()
        {

        }

        private void processDroping(UnityEngine.Object[] objectReferences, int row, float second)
        {
            if (row < 0f || second < 0f || second > _timeline.Duration)
            {
                return;
            }
            else
            {
                if (!Tracks.ProprietaryMode)
                {
                    MetaKeyframeBase dropingOnKeyframe = null;

                    checkKeyframeBodiesCycle(mousePosition, ref dropingOnKeyframe);

                    if (dropingOnKeyframe != null)
                    {
                        _timeline.ChangeKeyframesTarget(dropingOnKeyframe, objectReferences[0]);

                        return;
                    }
                }

                if (objectReferences[0] is Component)
                {
                    Undo.RecordObject(_timeline, "Keyframe Added");
                    try
                    {
                        //float requiredTiming = RequiredTiming(row);
                        MetaKeyframeBase interpolateFrom = InterpolationFrom(row, second);
                        MetaKeyframeBase newKeyframe = _timeline.AddKeyframeOnTrack(objectReferences[0] as Component, row, second, "");

                        if (interpolateFrom != null && newKeyframe != null)
                        {
                            _timeline.AddInterpolation(interpolateFrom, newKeyframe, "");
                        }
                        
                        _serializedObject.Update();
                    }
                    catch (Exception e)
                    {
                        TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                    }
                }
                else if (objectReferences[0] is GameObject)
                {
                    Undo.RecordObject(_timeline, "Keyframe Added");
                    try
                    {
                        //float requiredTiming = RequiredTiming(row);
                        MetaKeyframeBase interpolateFrom = InterpolationFrom(row, second);
                        MetaKeyframeBase newKeyframe = _timeline.AddKeyframeOnTrack(objectReferences[0] as GameObject, row, second, "");

                        if (interpolateFrom != null && newKeyframe != null)
                        {
                            _timeline.AddInterpolation(interpolateFrom, newKeyframe, "");
                        }
                        
                        _serializedObject.Update();
                    }
                    catch (Exception e)
                    {
                        TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                    }
                }
                else if (objectReferences[0] is AudioClip)
                {
                    Type keyframeType = _timeline.GetKeyframeTypeForComponent(typeof(AudioSource));
                    if (keyframeType != null)
                    {
                        Undo.RecordObject(_timeline, "Keyframe Added");
                        try
                        {
                            if (Tracks.ProprietaryMode)
                            {
                                TimelineTrack track = Timeline.Instance.Tracks.GetTrackOnTracksViewPosition(row);
                                UnityEngine.Object target = null;
                                if (track == null)
                                {
                                    track = Timeline.Instance.Tracks.CreateTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                                    target = GetTarget(typeof(AudioSource));
                                }
                                else
                                {
                                    target = track.Target;
                                }
                                //float requiredTiming = RequiredTiming(row);
                                MetaKeyframeBase interpolateFrom = InterpolationFrom(row, second);
                                MetaKeyframeBase newKeyframe = _timeline.AddKeyframeOnTrack((target as Component).gameObject, objectReferences[0], keyframeType, typeof(AudioSource), row, second, "");

                                if (interpolateFrom != null && newKeyframe != null)
                                {
                                    _timeline.AddInterpolation(interpolateFrom, newKeyframe, "");
                                }
                            }
                            else
                            {
                                _timeline.AddKeyframeOnTrack(Selection.activeGameObject, objectReferences[0], keyframeType, typeof(AudioSource), row, second, "");
                            }
                            
                            _serializedObject.Update();
                        }
                        catch (Exception e)
                        {
                            TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                        }
                    }
                }
            }
        }

        private class TempMeuItemStruct
        {
            public float Priority;
            public string Path;
            public GenericMenu.MenuFunction2 Action;
            public int Index;
        }

        private void ProcessContextMenu(Vector2 mousePos, WorkView.Subview subview)
        {
            int row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
            Tracks.RowMouseAbove = row;

            Type[] supportedKeyframeTypes;
            Type[] supportedInterpolationTypes;
            Type[] keyframeTypes;
            Type[] interpolationTypes;
            Type[] interpolationKeyframeTypes;
            string[] keyframeMenuPaths;
            string[] interpolationMenuPaths;
            _timeline.GetSupportedTypes(out keyframeTypes, out interpolationTypes, out supportedKeyframeTypes, out supportedInterpolationTypes, out interpolationKeyframeTypes, out keyframeMenuPaths, out interpolationMenuPaths);

            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Chapter"), false, addKeynote, new object[] { mousePos, subview });
            menu.AddSeparator("");

            List<TempMeuItemStruct> menuItems = new List<TempMeuItemStruct>();

            if (/*Selection.activeGameObject != null*/true)
            {
                //int row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
                //Tracks.RowMouseAbove = row;
                float second = getPositionsTime(mousePosition.x, subview);

                float priorityCounter = 0f;
                float smallNumber = 0.00001f;

                for (int i = 0; i < supportedInterpolationTypes.Length; i++)
                {
                    if (interpolationMenuPaths[i] == "")
                    {
                        continue;
                    }

                    if (Tracks.ProprietaryMode)
                    {
                        TimelineTrack track = _timeline.Tracks.GetTrackOnTracksViewPosition(row);

                        if (track != null)
                        {
                            if (!track.Target.GetType().Equals(supportedInterpolationTypes[i]))
                            {
                                continue;
                            }
                        }
                    }

                    priorityCounter += smallNumber;


                    FieldInfo contextMenuPriorityFieldInfo = interpolationKeyframeTypes[i].GetField("ContextMenuPriority");
                    if (contextMenuPriorityFieldInfo != null)
                    {
                        menuItems.Add(new TempMeuItemStruct()
                        {
                            Path = interpolationMenuPaths[i],
                            Priority = (float)contextMenuPriorityFieldInfo.GetValue(null) + priorityCounter,
                            Action = (object index) =>
                            {
                                #region Seconds defining
                                float second1 = second - 0f;
                                if (second1 < 0f)
                                {
                                    second1 = 0f;
                                }
                                else if (second1 > _timeline.Duration)
                                {
                                    second1 = _timeline.Duration;
                                }

                                float second2 = second + 1f;
                                if (second2 < 0f)
                                {
                                    second2 = 0f;
                                }
                                else if (second2 > _timeline.Duration)
                                {
                                    second2 = _timeline.Duration;
                                }
                                #endregion

                                try
                                {
                                    MetaKeyframeBase key1 = null;
                                    if (Tracks.ProprietaryMode)
                                    {
                                        TimelineTrack track = Timeline.Instance.Tracks.GetTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                                        UnityEngine.Object target = null;
                                        if (track == null)
                                        {
                                            track = Timeline.Instance.Tracks.CreateTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                                            target = GetTarget(supportedInterpolationTypes[(int)index]);
                                        }
                                        else
                                        {
                                            target = track.Target;
                                        }
                                        if (target is GameObject)
                                        {
                                            key1 = _timeline.AddKeyframeOnTrack(target as GameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second1, "");
                                        }
                                        else
                                        {
                                            key1 = _timeline.AddKeyframeOnTrack((target as Component).gameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second1, "");
                                        }
                                    }
                                    else
                                    {
                                        key1 = _timeline.AddKeyframeOnTrack(Selection.activeGameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second1, "");
                                    }
                                    MetaKeyframeBase key2 = key1.Clone() as MetaKeyframeBase;
                                    key2.Timing = second2;
                                    _timeline.AddKeyframeOnTrack(key2, row);
                                    //MetaKeyframeBase key2 = _timeline.AddKeyframeOnTrack(Selection.activeGameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second2, "");

                                    _timeline.AddInterpolation(key1, key2, interpolationMenuPaths[(int)index]);
                                }
                                catch (Exception e)
                                {
                                    TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                                }
                            },
                            Index = i
                        });
                    }


                    /*menu.AddItem(new GUIContent(interpolationMenuPaths[i]), false, (object index) =>
                    {
                        #region Seconds defining
                        float second1 = second - 0.5f;
                        if (second1 < 0f)
                        {
                            second1 = 0f;
                        }
                        else if (second1 > _timeline.Duration)
                        {
                            second1 = _timeline.Duration;
                        }

                        float second2 = second + 0.5f;
                        if (second2 < 0f)
                        {
                            second2 = 0f;
                        }
                        else if (second2 > _timeline.Duration)
                        {
                            second2 = _timeline.Duration;
                        }
                        #endregion

                        MetaKeyframeBase key1 = _timeline.AddKeyframeOnTrack(Selection.activeGameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second1, "");
                        MetaKeyframeBase key2 = _timeline.AddKeyframeOnTrack(Selection.activeGameObject, interpolationKeyframeTypes[(int)index], supportedInterpolationTypes[(int)index], row, second2, "");
                        
                        _timeline.AddInterpolation(key1, key2, interpolationMenuPaths[(int)index]);
                    }, i);*/
                }

                for (int i = 0; i < supportedKeyframeTypes.Length; i++)
                {
                    if (keyframeMenuPaths[i] == "")
                    {
                        continue;
                    }

                    if (Tracks.ProprietaryMode)
                    {
                        TimelineTrack track = _timeline.Tracks.GetTrackOnTracksViewPosition(row);

                        if (track != null)
                        {
                            if (!track.Target.GetType().Equals(supportedKeyframeTypes[i]))
                            {
                                continue;
                            }
                        }
                    }

                    priorityCounter += smallNumber;


                    FieldInfo contextMenuPriorityFieldInfo = keyframeTypes[i].GetField("ContextMenuPriority");
                    if (contextMenuPriorityFieldInfo != null)
                    {
                        menuItems.Add(new TempMeuItemStruct()
                        {
                            Path = keyframeMenuPaths[i],
                            Priority = (float)contextMenuPriorityFieldInfo.GetValue(null) + priorityCounter,
                            Action = (object index) =>
                            {
                                MetaKeyframeBase resultKeyframe = null;
                                try
                                {
                                    if (Tracks.ProprietaryMode)
                                    {
                                        if (Tracks.RowMouseAbove >= 0)
                                        {
                                            TimelineTrack track = Timeline.Instance.Tracks.GetTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                                            UnityEngine.Object target = null;
                                            if (track == null)
                                            {
                                                track = Timeline.Instance.Tracks.CreateTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                                                target = GetTarget(supportedKeyframeTypes[(int)index]);
                                            }
                                            else
                                            {
                                                target = track.Target;
                                            }
                                            if (track != null)
                                            {
                                                //float requiredTiming = RequiredTiming(row);
                                                MetaKeyframeBase interpolateFrom = InterpolationFrom(row, second);
                                                if (target is GameObject)
                                                {
                                                    resultKeyframe = _timeline.AddKeyframeOnTrack(target as GameObject, keyframeTypes[(int)index], supportedKeyframeTypes[(int)index], row, second, keyframeMenuPaths[(int)index]);
                                                }
                                                else
                                                {
                                                    resultKeyframe = _timeline.AddKeyframeOnTrack((target as Component).gameObject, keyframeTypes[(int)index], supportedKeyframeTypes[(int)index], row, second, keyframeMenuPaths[(int)index]);
                                                }

                                                if (interpolateFrom != null && resultKeyframe != null)
                                                {
                                                    _timeline.AddInterpolation(interpolateFrom, resultKeyframe, "");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        resultKeyframe = _timeline.AddKeyframeOnTrack(Selection.activeGameObject, keyframeTypes[(int)index], supportedKeyframeTypes[(int)index], row, second, keyframeMenuPaths[(int)index]);
                                    }
                                }
                                catch (Exception e)
                                {
                                    TimelineExceptionWindow.Init("Track Error:", "Error:", e.Message);
                                }
                            },
                            Index = i
                        });
                    }


                    /*menu.AddItem(new GUIContent(keyframeMenuPaths[i]), false, (object index) =>
                    {
                        _timeline.AddKeyframeOnTrack(Selection.activeGameObject, keyframeTypes[(int)index], supportedKeyframeTypes[(int)index], row, second, keyframeMenuPaths[(int)index]);
                    }, i);*/
                }

            }

            menuItems.Sort(delegate (TempMeuItemStruct x, TempMeuItemStruct y)
            {
                if (x.Priority > y.Priority)
                {
                    return 1;
                }
                else if (x.Priority == y.Priority)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });

            for (int i = 0; i < menuItems.Count; i++)
            {
                menu.AddItem(new GUIContent(menuItems[i].Path), false, menuItems[i].Action, menuItems[i].Index);
            }

            menu.ShowAsContext();
        }

        private UnityEngine.Object GetTarget(Type type)
        {

            GameObject root = null;
            if (Selection.activeGameObject == null)
            {
                root = new GameObject();
                if (type != typeof(GameObject))
                {
                    root.name = "New " + type.Name + " Object";
                }
            }
            else
            {
                root = Selection.activeGameObject;
            }

            if (type == typeof(GameObject))
            {
                return root;
            }

            UnityEngine.Object target = null;
            target = root.GetComponent(type);
            if (target == null)
            {
                target = root.AddComponent(type);
            }

            return target;
        }
        #endregion

        #region Drawing Tracks
        private void drawTracks(WorkView.Subview subview)
        {
            int startTrack;
            int endTrack;

            findTracksRange(_field.y, _viewRect.height, _timeline.PixelPerTrack, out startTrack, out endTrack);
            endTrack++;

            //Debug.Log("tracksBeginningY = " + tracksBeginningY);

            for (int i = startTrack; i < endTrack; i++)
            {
                float tracksBeginningY = _field.y + i * _timeline.PixelPerTrack;
                float tracksEndingY = _field.y + (i + 1) * _timeline.PixelPerTrack;

                /*Handles.BeginGUI();

                Handles.color = Color.gray * 1.2f;
                Handles.DrawLine(new Vector3(trackRect.x, trackRect.y), new Vector3(trackRect.x + trackRect.width, trackRect.y));
                Handles.DrawLine(new Vector3(trackRect.x, trackRect.y + trackRect.height), new Vector3(trackRect.x + trackRect.width, trackRect.y + trackRect.height));

                Handles.EndGUI();*/


                bool active = false;
                if (_timeline.CurrentTime >= subview.ChapterTiming0 && _timeline.CurrentTime < subview.ChapterTiming1)
                {
                    active = true;
                }

                Rect trackRect = new Rect(_realRect.x, tracksBeginningY, _realRect.width, tracksEndingY - tracksBeginningY);
                if (active)
                {
                    GUI.DrawTexture(trackRect, _trackBackgroundActive, ScaleMode.StretchToFill, true);
                }
                else
                {
                    GUI.DrawTexture(trackRect, _trackBackgroundPassive, ScaleMode.StretchToFill, true);
                }

                Handles.BeginGUI();

                float multiplier = 1f;
                if (active)
                {
                    multiplier = 0.9f;
                }
                else
                {
                    multiplier = 0.8f;
                }

                Handles.color = Color.gray * multiplier;
                Handles.DrawLine(new Vector3(_realRect.x, tracksBeginningY), new Vector3(_realRect.x + _realRect.width, tracksBeginningY));
                Handles.DrawLine(new Vector3(_realRect.x + _realRect.width, tracksEndingY), new Vector3(_realRect.x, tracksEndingY));

                Handles.EndGUI();
            }

            for (int i = 0; i < _timeline.Tracks.Count; i++)
            {
                if (_timeline.Tracks.TrackOnIndex(i).TracksViewPosition >= startTrack && _timeline.Tracks.TrackOnIndex(i).TracksViewPosition <= endTrack)
                {
                    drawTracksBackground(i, subview);
                }
            }

            drawConflictsBackgrounds();

            for (int i = 0; i < _timeline.Tracks.Count; i++)
            {
                if (_timeline.Tracks.TrackOnIndex(i).TracksViewPosition >= startTrack && _timeline.Tracks.TrackOnIndex(i).TracksViewPosition <= endTrack)
                {
                    drawTrack(i, subview);
                }
            }
            processConflicts();
            _threadsInfos.Clear();

            drawConflictsLabels();
        }

        private void drawTracksBackground(int index, WorkView.Subview subview)
        {
            float tracksBeginningY = _field.y + _timeline.Tracks.TrackOnIndex(index).TracksViewPosition * _timeline.PixelPerTrack;
            float tracksEndingY = _field.y + (_timeline.Tracks.TrackOnIndex(index).TracksViewPosition + 1) * _timeline.PixelPerTrack;

            bool selected = false;
            float multiplier = 1f;
            if (TracksView.SelectedTrack == _timeline.Tracks.TrackOnIndex(index).TracksViewPosition)
            {
                multiplier = 1.07f;
                selected = true;
            }

            bool active = false;
            if (_timeline.CurrentTime >= subview.ChapterTiming0 && _timeline.CurrentTime < subview.ChapterTiming1)
            {
                active = true;
            }

            Rect trackRect = new Rect(_realRect.x, tracksBeginningY, _realRect.width, tracksEndingY - tracksBeginningY);
            if (selected)
            {
                if (active)
                {
                    GUI.DrawTexture(trackRect, _trackSelectedActive, ScaleMode.StretchToFill, true);
                }
                else
                {
                    GUI.DrawTexture(trackRect, _trackSelectedPassive, ScaleMode.StretchToFill, true);
                }
            }
            else
            {
                if (active)
                {
                    GUI.DrawTexture(trackRect, _trackUnselectedActive, ScaleMode.StretchToFill, true);
                }
                else
                {
                    GUI.DrawTexture(trackRect, _trackUnselectedPassive, ScaleMode.StretchToFill, true);
                }
            }

            /*Handles.BeginGUI();

            Handles.color = _trackBackgroundColor * multiplier;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(_realRect.x, tracksBeginningY + 1),
                new Vector3(_realRect.x + _realRect.width, tracksBeginningY + 1),
                new Vector3(_realRect.x + _realRect.width, tracksEndingY - 1),
                new Vector3(_realRect.x, tracksEndingY - 1),});

            Handles.EndGUI();*/
        }

        private void drawTrack(int index, WorkView.Subview subview)
        {
            float tracksBeginningY = _field.y + _timeline.Tracks.TrackOnIndex(index).TracksViewPosition * _timeline.PixelPerTrack;
            float tracksEndingY = _field.y + (_timeline.Tracks.TrackOnIndex(index).TracksViewPosition + 1) * _timeline.PixelPerTrack;
            
            bool active = false;
            if (_timeline.CurrentTime >= subview.ChapterTiming0 && _timeline.CurrentTime < subview.ChapterTiming1)
            {
                active = true;
            }

            if (_timeline.Tracks.TrackOnIndex(index).Interpolations != null && _timeline.Tracks.TrackOnIndex(index).Interpolations.Count > 0)
            {
                for (int i = 0; i < _timeline.Tracks.TrackOnIndex(index).Interpolations.Count; i++)
                {
                    processInterpolation(_timeline.Tracks.TrackOnIndex(index).Interpolations[i], tracksBeginningY, tracksEndingY, subview, active);
                }
            }

            if (_timeline.Tracks.TrackOnIndex(index).Keyframes != null && _timeline.Tracks.TrackOnIndex(index).Keyframes.Count > 0)
            {
                for (int i = 0; i < _timeline.Tracks.TrackOnIndex(index).Keyframes.Count; i++)
                {
                    processKeyframe(_timeline.Tracks.TrackOnIndex(index).Keyframes[i], tracksBeginningY, tracksEndingY, subview, active);
                }
            }
        }

        private class InterpolationInfo
        {
            public UnityEngine.Object Target;
            public MetaInterpolationBase.InterpolationThread[] ThreadsInfo;
            public MetaInterpolationBase Interpolation;
            public Rect Rect;

        }
        private List<InterpolationInfo> _threadsInfos = new List<InterpolationInfo>();
        private Dictionary<UnityEngine.Object, ConflictInterpolationInfo> conflicts = new Dictionary<UnityEngine.Object, ConflictInterpolationInfo>();

        private class ConflictInterpolationInfo
        {
            //public UnityEngine.Object Target;
            public List<string> Names = new List<string>();
            public Rect Rect;
        }

        private void processConflicts()
        {
            /*
             * Conflicting interpolations are those who trying to control the same parameter of same gameObject at the same time
             */

            List<InterpolationInfo[]> potentiallyConflictedPairs = new List<InterpolationInfo[]>();

            for (int i = 0; i < _threadsInfos.Count; i++)
            {
                for (int j = i + 1; j < _threadsInfos.Count; j++)
                {
                    if (_threadsInfos[i].Target == _threadsInfos[j].Target && InterpolationsIntersects(_threadsInfos[i].Rect, _threadsInfos[j].Rect))
                    {
                        InterpolationInfo[] newPair = new InterpolationInfo[2] { _threadsInfos[i], _threadsInfos[j] };
                        potentiallyConflictedPairs.Add(newPair);
                    }
                }
            }

            //List<ConflictInterpolationInfo> conflictInfos = new List<ConflictInterpolationInfo>();
            for (int n = 0; n < potentiallyConflictedPairs.Count; n++)
            {
                InterpolationInfo[] pair = potentiallyConflictedPairs[n];
                for (int i = 0; i < pair[0].ThreadsInfo.Length; i++)
                {
                    for (int j = 0; j < pair[1].ThreadsInfo.Length; j++)
                    {
                        if (pair[0].ThreadsInfo[i].Name == pair[1].ThreadsInfo[j].Name)
                        {
                            if (!conflicts.ContainsKey(pair[0].Target))
                            {
                                conflicts.Add(pair[0].Target, new ConflictInterpolationInfo() { Rect = InterpolationIntersectionRect(pair[0].Rect, pair[1].Rect) });
                            }
                            Rect rect = conflicts[pair[0].Target].Rect;

                            pair[0].Interpolation.ConflictStart = rect.x;
                            pair[0].Interpolation.ConflictEnd = rect.x + rect.width;
                            pair[0].Interpolation.IsConflicting = true;
                            pair[1].Interpolation.ConflictStart = rect.x;
                            pair[1].Interpolation.ConflictEnd = rect.x + rect.width;
                            pair[1].Interpolation.IsConflicting = true;

                            conflicts[pair[0].Target].Names.Add(pair[0].ThreadsInfo[i].Name);
                        }
                    }
                }
            }
        }

        private void drawConflictsBackgrounds()
        {
            foreach (KeyValuePair<UnityEngine.Object, ConflictInterpolationInfo> kvp in conflicts)
            {
                ConflictInterpolationInfo currentConflict = kvp.Value;
                Handles.BeginGUI();

                Handles.color = new Color(1f, 0f, 0f, 0.3f);
                Vector3[] vectors = new Vector3[4] { new Vector3(currentConflict.Rect.x, currentConflict.Rect.y),
                new Vector3(currentConflict.Rect.x + currentConflict.Rect.width, currentConflict.Rect.y),
                new Vector3(currentConflict.Rect.x + currentConflict.Rect.width, currentConflict.Rect.y + currentConflict.Rect.height),
                new Vector3(currentConflict.Rect.x, currentConflict.Rect.y + currentConflict.Rect.height)};
                Handles.DrawAAConvexPolygon(vectors);

                Handles.EndGUI();
            }
            if (conflicts.Count > 0)
            {
                conflicts.Clear();
            }
        }

        private void drawConflictsLabels()
        {
            foreach (KeyValuePair<UnityEngine.Object, ConflictInterpolationInfo> kvp in conflicts)
            {
                ConflictInterpolationInfo currentConflict = kvp.Value;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Conflict of ");
                for (int i = 0; i < currentConflict.Names.Count; i++)
                {
                    stringBuilder.Append(currentConflict.Names[i]);
                    if (i == currentConflict.Names.Count - 2)
                    {
                        stringBuilder.Append(" and ");
                    }
                    else if (i < currentConflict.Names.Count - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }
                stringBuilder.Append(" parameter(s)");
                GUIContent conflictContent = new GUIContent();
                conflictContent.text = stringBuilder.ToString();
                //conflictContent.tooltip = "You're trying to animate parameter of one object two times per frame. So one of them won't make change. Just disable animation of listed parameters in one of two conflicting animations";
                conflictContent.tooltip = "You're making two changes to the same parameter at one time. So one of them won't make change. Just disable animation of listed parameters in one of two conflicting animations (Right click on keyframe or animation)";

                GUILayout.BeginArea(currentConflict.Rect);
                GUILayout.FlexibleSpace();
                //GUILayout.BeginHorizontal();
                GUILayout.Label(conflictContent, _viewSkin.GetStyle("ConflictStyle"));
                //GUILayout.Label(conflictContent, _viewSkin.GetStyle("ConflictStyle"), new[] { GUILayout.Width(currentConflict.Rect.width), GUILayout.ExpandWidth(true) });
                //GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();

                Handles.EndGUI();
            }
        }

        private Rect InterpolationIntersectionRect(Rect rect1, Rect rect2)
        {
            float x = 0f;
            float y = 0f;
            float width = 0f;
            float height = 0f;
            //first define y
            if (rect1.y > rect2.y)
            {
                y = rect2.y;
                height = rect1.y + rect1.height - y;
            }
            else
            {
                y = rect1.y;
                height = rect2.y + rect2.height - y;
            }

            y += _timeline.PixelPerTrack / 2f;
            height -= _timeline.PixelPerTrack;

            //second define x
            Rect first = rect1;
            Rect second = rect2;
            if (rect1.x > rect2.x)
            {
                first = rect2;
                second = rect1;
            }

            x = second.x;
            if (first.x + first.width > second.x + second.width)
            {
                width = second.x + second.width - x;
            }
            else
            {
                width = first.x + first.width - x;
            }

            return new Rect(x, y, width, height);
        }

        private bool InterpolationsIntersects(Rect rect1, Rect rect2)
        {
            Rect first = rect1;
            Rect second = rect2;
            if (rect1.x > rect2.x)
            {
                first = rect2;
                second = rect1;
            }

            //return (second.x > first.x && second.x <= first.x + first.width);
            return second.x < first.x + first.width;
        }

        private void processInterpolation(MetaInterpolationBase interpolation, float tracksBeginningY, float tracksEndingY, WorkView.Subview subview, bool active)
        {
            if ((interpolation.Keyframes[0].Timing >= _timing0 && interpolation.Keyframes[0].Timing < _timing1) ||
                (interpolation.Keyframes[1].Timing >= _timing0 && interpolation.Keyframes[1].Timing < _timing1) ||
                (_timing0 >= interpolation.Keyframes[0].Timing && _timing0 < interpolation.Keyframes[1].Timing) ||
                (_timing1 >= interpolation.Keyframes[0].Timing && _timing1 < interpolation.Keyframes[1].Timing))
            {
                float beginningPos = getTimesPosition(interpolation.Keyframes[0].Timing, subview);
                float endingPos = getTimesPosition(interpolation.Keyframes[1].Timing, subview);
                if (beginningPos < 0f)
                {
                    beginningPos = 0f;
                }
                if (endingPos < 0f)
                {
                    endingPos = 0f;
                }
                float beginningX = _realRect.x + beginningPos;
                float endingX = _realRect.x + endingPos;

                if (beginningX > endingX)
                {
                    float temp = beginningX;
                    beginningX = endingX;
                    endingX = temp;
                }

                Rect interpolationRect = new Rect(beginningX, tracksBeginningY, endingX - beginningX, tracksEndingY - tracksBeginningY);
                interpolation.DrawInterpolation(interpolationRect, active);

                InterpolationInfo newInfo = new InterpolationInfo();
                newInfo.Target = interpolation.Target;
                newInfo.ThreadsInfo = interpolation.ActiveTreads();
                newInfo.Rect = interpolationRect;
                newInfo.Interpolation = interpolation;
                _threadsInfos.Add(newInfo);

                /*float interpolationAreaBeginningY = (tracksEndingY - tracksBeginningY - _interpolationWidth) / 2f;

                float interpolationMultiplier = 1f;
                if (CurrentManipulatedInterpolation == interpolation)
                {
                    interpolationMultiplier = 1.05f;
                }
                else if (CurrentSelectedInterpolation == interpolation)
                {
                    interpolationMultiplier = 1.12f;
                }


                if (TracksView.SelectedTrack == interpolation.TrackIndex)
                {
                    interpolationMultiplier *= 1.07f;
                }

                Handles.BeginGUI();

                Vector3[] vectors = new Vector3[4] { new Vector3(beginningX, tracksBeginningY + interpolationAreaBeginningY),
                new Vector3(endingX, tracksBeginningY + interpolationAreaBeginningY),
                new Vector3(endingX, tracksBeginningY + interpolationAreaBeginningY + _interpolationWidth),
                new Vector3(beginningX, tracksBeginningY + interpolationAreaBeginningY + _interpolationWidth)};

                Handles.color = _interpolationBackgroundColor * interpolationMultiplier;

                Handles.DrawAAConvexPolygon(vectors);

                Handles.color = Color.white;

                int numberOfThreads = interpolation.ActiveThreadsNumber;
                for (int j = 0; j < numberOfThreads; j++)
                {
                    float lineHeight = 0f;
                    lineHeight = tracksBeginningY + interpolationAreaBeginningY + (_interpolationWidth / (numberOfThreads + 1)) * (j + 1);

                    Handles.DrawLine(new Vector2(beginningX, lineHeight), new Vector2(endingX, lineHeight));
                }

                Handles.EndGUI();
                interpolation.BodyRect = Utilities.Encapsulate(new Rect(beginningX, tracksBeginningY + interpolationAreaBeginningY, 0f, _interpolationWidth), new Rect(endingX, tracksBeginningY + interpolationAreaBeginningY, 0f, _interpolationWidth));

                interpolation.ControlID = GUIUtility.GetControlID(FocusType.Passive) + 1;

                EditorGUIUtility.AddCursorRect(interpolation.BodyRect, MouseCursor.MoveArrow);*/
            }
        }

        private void processKeyframe(MetaKeyframeBase keyframe, float tracksBeginningY, float tracksEndingY, WorkView.Subview subview, bool active)
        {
            if (keyframe != null && keyframe.Timing >= _timing0 && keyframe.Timing < _timing1)
            {
                float height = tracksEndingY - tracksBeginningY;
                //Rect keyframeRect = new Rect(_realRect.x + getTimesPosition(keyframe.Timing, subview) - _keyframeRadius, tracksBeginningY + (_timeline.PixelPerTrack - _keyframeRadius * 2f) / 2, _keyframeRadius * 2f, _keyframeRadius * 2f);
                Rect keyframeRect = new Rect(_realRect.x + getTimesPosition(keyframe.Timing, subview) - height/2, tracksBeginningY, height, height);
                //keyframe.BodyRect = keyframeRect;
                //drawKeyframe(keyframe, keyframeRect);

                keyframe.DrawKeyframe(keyframeRect, active);

                //EditorGUIUtility.AddCursorRect(keyframeRect, MouseCursor.MoveArrow);
            }
        }

        /*private void drawKeyframe(MetaKeyframeBase keyframe, Rect keyframesRect)
        {
            Handles.BeginGUI();

            float rimMultipier = 1f;
            float centerMultipier = 1f;
            if (CurrentManipulatedKeyframe == keyframe)
            {
                rimMultipier = 1.2f;
            }
            else if (CurrentSelectedKeyframe == keyframe)
            {
                centerMultipier = 1.4f;
                rimMultipier = 1.2f;
            }

            Vector2 center = keyframesRect.center;
            Handles.color = _keyframeRimColor * rimMultipier;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(center.x - _keyframeRadius, center.y),
                new Vector3(center.x, center.y - _keyframeRadius),
                new Vector3(center.x + _keyframeRadius, center.y),
                new Vector3(center.x, center.y + _keyframeRadius)});

            Handles.color = keyframe.KeyframeColor * centerMultipier;

            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(center.x - (_keyframeRadius - _keframeRimThickness), center.y),
            new Vector3(center.x, center.y - (_keyframeRadius - _keframeRimThickness)),
            new Vector3(center.x + (_keyframeRadius - _keframeRimThickness), center.y),
            new Vector3(center.x, center.y + (_keyframeRadius - _keframeRimThickness))});

            Handles.EndGUI();
        }*/
        #endregion

        #region Utility Methods
        public void ClearSubviews()
        {
            _subviews.Clear();
        }

        private void addKeynote(object obj)
        {
            _timeline.AddKeynote(getPositionsTime(((Vector2)(obj as object[])[0]).x, (WorkView.Subview)(obj as object[])[1]), "Chapter", "Description...");
        }

        private float getTimesPosition(float time, WorkView.Subview subview)
        {
            return subview.RealRect.width * ((time - subview.Timing0) / (subview.Timing1 - subview.Timing0));
        }

        private float getPositionsTime(float position, WorkView.Subview subview)
        {
            return subview.Timing0 + ((position - subview.RealRect.x) / subview.RealRect.width) * (subview.Timing1 - subview.Timing0);
        }

        private void findTracksRange(float fieldY, float rectHeight, float trackHeight, out int startTrack, out int endTrack)
        {
            startTrack = (int)Mathf.Floor((fieldY * -1f) / trackHeight);
            endTrack = startTrack + (int)Mathf.Ceil(rectHeight / trackHeight);

            if (startTrack < 0)
            {
                startTrack = 0;
            }
            /*if (endTrack > _timeline.Tracks.Length)
            {
                endTrack = _timeline.Tracks.Length;
            }*/
        }

        private void checkKeyframeBodiesCycle(Vector2 mousePosition, ref MetaKeyframeBase target)
        {
            for (int i = 0; i < _timeline.Tracks.Count; i++)
            {
                if (_timeline.Tracks.TrackOnIndex(i).Keyframes.Count > 0)
                {
                    List<MetaKeyframeBase> keyframes = _timeline.Tracks.TrackOnIndex(i).Keyframes;
                    for (int j = 0; j < keyframes.Count; j++)
                    {
                        if (keyframes[j].BodyRect.Contains(mousePosition))
                        {
                            target = keyframes[j];
                            target.StartDragingPosition = mousePosition;
                            target.PreviousTiming = target.Timing;
                            return;
                        }
                    }
                }
            }
        }

        private void checkInterpolationsBodiesCycle(Vector2 mousePosition, ref MetaInterpolationBase target)
        {
            for (int i = 0; i < _timeline.Tracks.Count; i++)
            {
                if (_timeline.Tracks.TrackOnIndex(i).Interpolations.Count > 0)
                {
                    List<MetaInterpolationBase> interpolations = _timeline.Tracks.TrackOnIndex(i).Interpolations;
                    for (int j = 0; j < interpolations.Count; j++)
                    {
                        for (int n = 0; n < interpolations[j].BodyRects.Count; n++)
                        {
                            if (interpolations[j].BodyRects[n].Contains(mousePosition))
                            {
                                target = interpolations[j];
                                target.StartDragingPosition = mousePosition;
                                target.Keyframes[0].PreviousTiming = target.Keyframes[0].Timing;
                                target.Keyframes[1].PreviousTiming = target.Keyframes[1].Timing;
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
