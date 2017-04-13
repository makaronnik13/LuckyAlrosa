using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class TimeSliderView : WorkViewBase
    {
        #region Private Variables
        private Color _sliderColor = new Color(115f / 255f, 194f / 255f, 242f / 255);

        private Texture2D _blackLine;
        
        private WorkView.Subview subview = new WorkView.Subview();

        private bool _draging = false;

        private Rect _dragingArea;
        #endregion

        #region Constructor
        public TimeSliderView()
        {
            _blackLine = new Texture2D(1, 1);
            _blackLine.SetPixel(0, 0, Color.black);
            _blackLine.Apply();
        }
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);
            
            subview.RealRect = _realRect;
            subview.Timing0 = _timing0;
            subview.Timing1 = _timing1;
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_dragingArea.Contains(mousePosition))
                    {
                        _draging = true;
                    }
                    break;
                case EventType.MouseUp:
                    _draging = false;
                    break;
                case EventType.MouseDrag:
                    if (_draging)
                    {
                        float newTiming = getPositionsTime(mousePosition.x, subview);
                        _timeline.GoToTiming(newTiming, Timeline.AnimationMode.IgnoreKeynotes);

                        e.Use();
                    }
                    break;
            }
        }
        #endregion

        #region Draw TimeSlider
        public void DrawTimeSlider()
        {
            //Debug.Log("DrawTimeSlider");
            float sliderGlobalPos = getTimesPosition(_timeline.CurrentTime, subview);

            Handles.BeginGUI();
            Handles.color = _sliderColor;
            Handles.DrawLine(new Vector3(_realRect.x + sliderGlobalPos, _viewRect.y), new Vector3(_realRect.x + sliderGlobalPos, _viewRect.y + _viewRect.height));
            Handles.EndGUI();
            /*if (_blackLine != null)
            {
                GUI.DrawTexture(new Rect(_realRect.x + sliderGlobalPos, _viewRect.y, 1f, _viewRect.height), _blackLine);
            }*/

            _dragingArea = new Rect(_realRect.x + sliderGlobalPos - 2f, _viewRect.y, 4f, 32f);
            EditorGUIUtility.AddCursorRect(_dragingArea, MouseCursor.SplitResizeLeftRight);
        }
        #endregion

        #region Utility Methods
        private float getTimesPosition(float time, WorkView.Subview subview)
        {
            float position = subview.RealRect.width * ((time - subview.Timing0) / (subview.Timing1 - subview.Timing0));

            return position;
        }

        private float getPositionsTime(float position, WorkView.Subview subview)
        {

            if (position > subview.RealRect.x + subview.RealRect.width)
            {
                List<Keynote> keynotes = _timeline.Keynotes;
                Keynote nextKeynote = null;
                for (int i = 0; i < keynotes.Count; i++)
                {
                    if (keynotes[i].Timing >= subview.Timing1)
                    {
                        nextKeynote = keynotes[i];
                        break;
                    }
                }
                
                if (nextKeynote != null && position > subview.RealRect.x + subview.RealRect.width + nextKeynote.Width)
                {
                    position = position - nextKeynote.Width;
                }
                else
                {
                    position = subview.RealRect.x + subview.RealRect.width;
                }
            }
            else if (position < subview.RealRect.x)
            {
                List<Keynote> keynotes = _timeline.Keynotes;
                Keynote nextKeynote = null;
                for (int i = 0; i < keynotes.Count; i++)
                {
                    if (keynotes[i].Timing >= subview.Timing0)
                    {
                        nextKeynote = keynotes[i];
                        break;
                    }
                }

                if (nextKeynote != null && position < subview.RealRect.x - nextKeynote.Width)
                {
                    position = position + nextKeynote.Width;
                }
                else
                {
                    position = subview.RealRect.x;
                }
            }
            /*float adding = ((position - subview.RealRect.x) / subview.RealRect.width) * (subview.Timing1 - subview.Timing0);
            if (adding > subview.Timing1 - subview.Timing0)
            {
                adding = subview.Timing1 - subview.Timing0;
            }*/

            float time = subview.Timing0 + ((position - subview.RealRect.x) / subview.RealRect.width) * (subview.Timing1 - subview.Timing0);

            if (time < 0f)
            {
                time = 0f;
            }
            else if (time > _timeline.Duration)
            {
                time = _timeline.Duration;
            }

            return time;
        }
        #endregion
    }
}
