using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    public class KeynotesView : TimelineViewBase
    {
        #region Private Variables
        private Keynote _currentSelectedKeynote = null;
        private float _keynoteWidth = 90f;
        #endregion

        #region Constructor
        public KeynotesView()
        {

        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            Handles.BeginGUI();
            Handles.color = Color.black;
            Handles.DrawAAConvexPolygon(new[] {new Vector3(0, 0),
                    new Vector3(_rect.width, 0),
                    new Vector3(_rect.width, _rect.height),
                    new Vector3(0, _rect.height)});
            Handles.EndGUI();

            if (_timeline.Keynotes.Count > 0)
            {
                drawKeynotes();
                drawPlusButton();
            }
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            processInteractions(e);

            switch (e.type)
            {
                case EventType.MouseUp:
                    GUIUtility.keyboardControl = 0;
                    break;
            }
        }

        private void processInteractions(Event e)
		{
            switch (e.type)
            {
			    case EventType.MouseUp:
                    _currentSelectedKeynote = null;
                    checkKeynoteBookmarksBodiesCycle(e.mousePosition, ref _currentSelectedKeynote);
                    if (_currentSelectedKeynote != null)
                    {
                        _timelineWindow.NavigateToKeynote(_currentSelectedKeynote);
                        _timeline.GoToTiming(_currentSelectedKeynote.Timing, Timeline.AnimationMode.IgnoreKeynotes);
                    }
                    break;
            }
        }
        #endregion

        #region Utility Methods
        private void drawKeynotes()
        {
            Keynote currentKeynote = _timeline.GetCurrentKeynote();
            for (int i = 0; i < _timeline.Keynotes.Count; i++)
            {
                float xCoord = i * _keynoteWidth;
                Handles.BeginGUI();
                Handles.color = Color.black;
                if (currentKeynote != null && currentKeynote == _timeline.Keynotes[i])
                {
                    Handles.color = new Color(21f / 255f, 22f / 255f, 23f / 255f);
                }
                Handles.DrawAAConvexPolygon(new[] {new Vector3(xCoord, 0),
                    new Vector3(xCoord + _keynoteWidth, 0),
                    new Vector3(xCoord + _keynoteWidth, _height),
                    new Vector3(xCoord, _height)});
                Handles.EndGUI();

                GUIContent content = new GUIContent();
                content.text = _timeline.Keynotes[i].Name;
                _timeline.Keynotes[i].BookmarkRect = new Rect(xCoord, 0, _keynoteWidth, _height);
                if (currentKeynote != null && currentKeynote == _timeline.Keynotes[i])
                {
                    GUI.Box(_timeline.Keynotes[i].BookmarkRect, content, _viewSkin.GetStyle("KeynoteBookmarkStyleActive"));
                }
                else
                {
                    GUI.Box(_timeline.Keynotes[i].BookmarkRect, content, _viewSkin.GetStyle("KeynoteBookmarkStylePassive"));
                }
            }
        }

        private void drawPlusButton()
        {
            Rect plusButtonRect = new Rect(_timeline.Keynotes.Count * _keynoteWidth, 0f, 24f, _height);

            if (GUI.Button(plusButtonRect, "+", _viewSkin.GetStyle("PlusChapterButtonStyle")))
            {
                float lastChapterTiming = _timeline.Keynotes[_timeline.Keynotes.Count - 1].Timing;
                float targetTiming = lastChapterTiming + (_timeline.Duration - lastChapterTiming) * 0.25f;
                _timeline.AddKeynote(targetTiming, "Chapter", "Description...");
            }
        }


        private void checkKeynoteBookmarksBodiesCycle(Vector2 mousePosition, ref Keynote target)
        {
			List<Keynote> keynotes = _timeline.Keynotes;
			for (int j = 0; j < keynotes.Count; j++)
			{
				if (keynotes[j].BookmarkRect.Contains(mousePosition))
				{
					target = keynotes[j];
					return;
				}
			}
        }
        #endregion
    }
}
