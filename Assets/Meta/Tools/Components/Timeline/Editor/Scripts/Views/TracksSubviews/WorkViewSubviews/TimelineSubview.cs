using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class TimelineSubview : WorkViewBase
    {
        #region Private Variables
        private float _timelineSecondHeight = 8f;
        private float _timelineSecondWidth = 2f;
        private float _timeline100MSecondHeight = 3f;
        private float _timeline100MSecondWidth = 1f;
        
        private Texture2D _grayLine;
        private Texture2D _blackLine;
        private Texture2D _background;
        
        private List<WorkView.Subview> _subviews = new List<WorkView.Subview>();
        
        private float _initialX;
        private float _pathMade = 999f;
        #endregion

        #region Constructor
        public TimelineSubview()
        {
            _grayLine = new Texture2D(1, 1);
            _grayLine.SetPixel(0, 0, Color.gray);
            _grayLine.Apply();

            _blackLine = new Texture2D(1, 1);
            _blackLine.SetPixel(0, 0, Color.black);
            _blackLine.Apply();

            _background = new Texture2D(1, 1);
            _background.SetPixel(0, 0, Color.clear);
            _background.Apply();
        }
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);

            _realRect.y = viewRect.y;
            _realRect.height = viewRect.height;

            WorkView.Subview newSubview = new WorkView.Subview();
            newSubview.RealRect = _realRect;
            newSubview.Timing0 = _timing0;
            newSubview.Timing1 = _timing1;
            newSubview.UpdateInstanceID();
            _subviews.Add(newSubview);

            //GUILayout.BeginArea(globalDrawingRect);
            drawTimeline(newSubview);
            //GUILayout.EndArea();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            processNavigation(e);
        }

        private void processNavigation(Event e)
        {
            if (_pressedLMB)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        _pathMade = 0f;
                        _initialX = mousePosition.x;
                        _workView.NavigationXStart(mousePosition.x);
                        break;
                    case EventType.MouseDrag:
                        _workView.NavigateX(mousePosition.x);
                        _pathMade += Mathf.Abs(_initialX - mousePosition.x);
                        break;
                }
            }
            if (_pressedRMB)
            {

            }

            WorkView.Subview activeSubview = null;
            for (int i = 0; i < _subviews.Count; i++)
            {
                if (_subviews[i].RealRect.Contains(mousePosition))
                {
                    activeSubview = _subviews[i];
                    break;
                }
            }

            if (activeSubview != null && e.button == 0)
            {
                switch (e.type)
                {
                    case EventType.MouseUp:
                        if (_pathMade < 4f)
                        {
                            _workView.ProcessClickOnTimeline(mousePosition.x);
                            _pathMade = 999f;
                        }
                        break;
                    case EventType.ScrollWheel:
                        Undo.RecordObject(_timeline, "Time Resolution Changed");
                        _timeline.ChangeTimeResolution(e.delta.y);
                        break;
                }
            }
        }

        #endregion

        #region Drawing
        private void drawTimeline(WorkView.Subview subview)
        {
            //GUI.Box(_realRect, _background);

            float startTiming = _timing0;
            float endTiming = _timing1;

            float timeAdding = 1f;
            float markedValue = 10f;
            bool secondsMode = true;
            if (_timeline.PixelPerSecond > 40f)
            {
                timeAdding = 0.1f;
                markedValue = 1f;
                secondsMode = false;
            }

            float startTime = Mathf.Floor(startTiming / timeAdding) * timeAdding;
            float startPadding = startTiming - startTime;
            float endTime = Mathf.Ceil(endTiming * 10f) / 10f;

            Handles.BeginGUI();
            Handles.color = Color.white;
            _realRect.height = _realRect.height - 1f;
            for (float time = startTime; time < endTime; time = Mathf.Floor((time + timeAdding)*10f)/10f)
            {
                float pos = getTimesPosition(time, subview);

                if (pos >= 0f && time >= startTime)
                {
                    float markedRest = time / markedValue - Mathf.Floor(time / markedValue);
                    float halfMarkedRest = time % (markedValue / 2f);

                    if (time == 10f)
                    {
                        time = 10f;
                    }

                    if (markedRest == 0f)
                    {
                        Rect secondsRect = new Rect(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height, 0, 0);
                        if (pos > 0f)
                        {
                            Handles.DrawLine(new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height * 0.33f), new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height - 1));
                        }

                        if (secondsMode)
                        {
                            float minutes = Mathf.Floor(time / 60f);
                            float seconds = time % 60f;
                            DateTime date = new DateTime(2007, 8, 1, 1, (int)minutes, (int)seconds);
                            GUI.Label(new Rect(secondsRect.x + 2f, subview.RealRect.y + Mathf.Round(subview.RealRect.height * 0.33f) - 4f, 8, 10), "<size=9><color=#ffffff>" + date.ToString("m:ss") + "</color></size>", Utilities.RichTextStyle);
                        }
                        else
                        {
                            GUI.Label(new Rect(secondsRect.x + 2f, subview.RealRect.y + Mathf.Round(subview.RealRect.height * 0.33f) - 4f, 8, 10), "<size=9><color=#ffffff>" + Mathf.Round(time).ToString() + "</color></size>", Utilities.RichTextStyle);
                        }
                    }
                    else if (halfMarkedRest == 0f)
                    {
                        Rect secondsRect = new Rect(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height, 0, 0);
                        Handles.DrawLine(new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height * 0.5f), new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height - 1));
                    }
                    else
                    {
                        Rect secondsRect = new Rect(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height, 0, 0);
                        Handles.DrawLine(new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height * 0.67f), new Vector3(subview.RealRect.x + pos, subview.RealRect.y + subview.RealRect.height - 1));
                    }
                }
            }
            Handles.EndGUI();
        }
        #endregion

        #region Utility Methods
        public void ClearSubviews()
        {
            _subviews.Clear();
        }

        private float getTimesPosition(float time, WorkView.Subview subview)
        {
            return subview.RealRect.width * ((time - subview.Timing0) / (subview.Timing1 - subview.Timing0));
        }
        #endregion
    }
}
