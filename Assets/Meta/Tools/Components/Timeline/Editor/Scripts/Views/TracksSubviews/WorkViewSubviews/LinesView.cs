using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class LinesView : WorkViewBase
    {
        #region Public Variables
        public Vector2 Point0;
        public Vector2 Point1;
        public Color Color;
        #endregion

        #region Private Variables
        private bool _needToDrawLine = false;
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);

            drawLines();
        }
        #endregion

        #region Drawing Lines
        private void drawLines()
        {
            if (_needToDrawLine)
            {
                Handles.BeginGUI();

                Handles.color = Color;
                Handles.DrawLine(Point0, Point1);

                Handles.EndGUI();

                _needToDrawLine = false;
            }
        }

        internal void DrawLine(Color color, Vector2 point0, Vector2 point1, Vector2 offset)
        {
            Color = color;
            Point0 = new Vector2(point0.x + offset.x, point0.y + offset.y);
            Point1 = new Vector2(point1.x + offset.x, point1.y + offset.y);

            _needToDrawLine = true;
        }
        #endregion

        #region Utility Methods

        #endregion
    }
}
