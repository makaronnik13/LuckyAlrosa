using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class SecondPanelView : TracksViewBase
    {
        private const float _innerRectPadding = 0.33f;
        private const float _innerRectThickness = 0.33f;

        private Color _barOutlineColor = new Color(99f / 255f, 104f / 255f, 109f / 255f);
        private Color _barBackgoundColor = new Color(25f / 255f, 29f / 255f, 37f / 255f);
        private Color _barColor = new Color(115f / 255f, 194f / 255f, 242f / 255f);

        private Vector2 _initialPos;
        private float _pathMade = 999f;

        private bool _scrollButtonPressed = false;
        private float _xWhenScrollButonPressed;
        private float _pixelsPerSecondWhenScrollButonPressed;

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            if (_rect.Contains(_mousePosition))
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        _pathMade = 0f;
                        _initialPos = _mousePosition;
                        if (e.button == 2)
                        {
                            _scrollButtonPressed = true;
                            _xWhenScrollButonPressed = _mousePosition.x;
                            _pixelsPerSecondWhenScrollButonPressed = _timeline.PixelPerSecond;
                        }
                        break;
                    case EventType.MouseDrag:
                        _pathMade += Vector2.Distance(_initialPos, _mousePosition);
                        break;
                    case EventType.MouseUp:
                        if (_pathMade < 4f)
                        {
                            //_workView.ProcessClickOnTimeline(mousePosition.x);
                            _pathMade = 999f;
                        }
                        break;
                    case EventType.ScrollWheel:
                        Undo.RecordObject(_timeline, "Time Resolution Changed");
                        _timeline.ChangeTimeResolution(e.delta.y);
                        break;
                }
            }

            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (_scrollButtonPressed)
                    {
                        float delta = _xWhenScrollButonPressed - _mousePosition.x;
                        float newPixelsPerSecond = _pixelsPerSecondWhenScrollButonPressed + delta;
                        _timeline.PixelPerSecond = newPixelsPerSecond;
                    }
                    break;
                case EventType.MouseUp:
                    _scrollButtonPressed = false;
                    break;
            }
        }

        protected override void Draw()
        {
            base.Draw();

            DrawLeftPart();
            DrawRightPart();
        }

        private void DrawLeftPart()
        {
            Rect leftRect = new Rect(_rect.x, _rect.y, _tracksView.TracksHeadersPercentage * _rect.width, _rect.height);
            GUILayout.BeginArea(leftRect);
            DrawTime();
            DrawControls();
            DrawSlider();
            GUILayout.EndArea();
        }

        private void DrawTime()
        {
            Rect playbackTimeArea = new Rect(0, 0, 136f, _rect.height);
            //drawing time
            GUILayout.BeginArea(playbackTimeArea);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            GUILayout.Label("<color=#cccccc><size=14>Time:</size></color>", Utilities.RichTextStyle, GUILayout.Width(36));
            GUILayout.TextField(((int)Mathf.Floor(_timeline.CurrentTime / 60f)).ToString(), GUILayout.Width(24));
            GUILayout.Label("<color=#cccccc><size=14>:</size></color>", Utilities.RichTextStyle, GUILayout.Width(4));
            GUILayout.TextField(((int)Mathf.Floor(_timeline.CurrentTime % 60f)).ToString(), GUILayout.Width(24));
            GUILayout.Label("<color=#cccccc><size=14>:</size></color>", Utilities.RichTextStyle, GUILayout.Width(4));
            GUILayout.TextField(((int)Mathf.Floor((_timeline.CurrentTime - Mathf.Floor(_timeline.CurrentTime)) * 100f)).ToString(), GUILayout.Width(24));

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void DrawControls()
        {
            float littleButtonsHeight = 16f;
            float bigButtonsHeight = 18f;
            float biggestButtonsHeight = 24f;
            float longButtonsWidth = 26f;
            float averageButtonsWidth = 20f;
            float smallButtonsWidth = 18f;

            Rect buttonsArea = new Rect(136 + 12, 0, 140f, _rect.height);
            GUILayout.BeginArea(buttonsArea);
            /*GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();*/
            GUILayout.BeginHorizontal();

            //GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _viewSkin.GetStyle("ToStartButtonStyle"), new[] { GUILayout.Width(averageButtonsWidth), GUILayout.Height(littleButtonsHeight) }))
            {
                _timeline.GoToTiming(0f);
                _timelineWindow.NavigateToTiming(0f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _viewSkin.GetStyle("RewindButtonStyle"), new[] { GUILayout.Width(longButtonsWidth), GUILayout.Height(littleButtonsHeight) }))
            {
                _timeline.GoToTiming(_timeline.CurrentTime - 0.1f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _viewSkin.GetStyle("StopButtonStyle"), new[] { GUILayout.Width(smallButtonsWidth), GUILayout.Height(bigButtonsHeight) }))
            {
                _timelineWindow.Stop();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _timeline.Playing ? _viewSkin.GetStyle("PauseButtonStyle") : _viewSkin.GetStyle("PlayButtonStyle"), new[] { GUILayout.Width(biggestButtonsHeight), GUILayout.Height(biggestButtonsHeight) }))
            {
                if (_timeline.Playing)
                {
                    _timelineWindow.Pause();
                }
                else
                {
                    _timelineWindow.Play();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _viewSkin.GetStyle("FastForwardButtonStyle"), new[] { GUILayout.Width(longButtonsWidth), GUILayout.Height(littleButtonsHeight) }))
            {
                _timeline.GoToTiming(_timeline.CurrentTime + 0.1f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GUIContent.none, _viewSkin.GetStyle("ToEndButtonStyle"), new[] { GUILayout.Width(averageButtonsWidth), GUILayout.Height(littleButtonsHeight) }))
            {
                _timeline.GoToTiming(_timeline.Duration);
                _timelineWindow.NavigateToTiming(_timeline.Duration);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            /*GUILayout.FlexibleSpace();
            GUILayout.EndVertical();*/
            GUILayout.EndArea();
        }

        private void DrawSlider()
        {
            Rect sliderArea = new Rect(136f + 140f + 12f, 0f, 100f, _rect.height);
            GUILayout.BeginArea(sliderArea);
            float newPixelPerSecond = CustomGUI.DrawSlider(new Rect(0, sliderArea.height * 0.25f, 100f, sliderArea.height * 0.5f), _timeline.PixelPerSecond, 10f, 140f);
            if (_timeline.PixelPerSecond != newPixelPerSecond)
            {
                Undo.RecordObject(_timeline, "New Timeline Scale Setted");
                _timeline.PixelPerSecond = newPixelPerSecond;
            }
            GUILayout.EndArea();
        }

        private void DrawRightPart()
        {
            Rect rightRect = new Rect(_rect.x + _tracksView.TracksHeadersPercentage * _rect.width, _rect.y, _rect.width - _tracksView.TracksHeadersPercentage * _rect.width, _rect.height);
            GUILayout.BeginArea(rightRect);
            Rect localRect = rightRect;
            localRect.x = 0;
            localRect.y = 0;
            DrawBar(localRect);
            GUILayout.EndArea();
        }

        private void DrawBar(Rect localRect)
        {
            Rect innerRect = new Rect(localRect.x, localRect.y + localRect.height * _innerRectPadding, localRect.width, localRect.height * _innerRectThickness);
            Handles.BeginGUI();
            Handles.color = _barBackgoundColor;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(innerRect.x, innerRect.y),
                new Vector3(innerRect.x + innerRect.width, innerRect.y),
                new Vector3(innerRect.x + innerRect.width, innerRect.y + innerRect.height),
                new Vector3(innerRect.x, innerRect.y + innerRect.height) });

            float localSliderPosition = _timeline.CurrentTime * _timeline.PixelPerSecond + _tracksView.TracksField.x;
            Handles.color = _barColor;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(innerRect.x, innerRect.y),
                new Vector3(innerRect.x + localSliderPosition, innerRect.y),
                new Vector3(innerRect.x + localSliderPosition, innerRect.y + innerRect.height),
                new Vector3(innerRect.x, innerRect.y + innerRect.height) });

            Handles.color = _barOutlineColor;
            Handles.DrawAAPolyLine(new[] { new Vector3(innerRect.x, innerRect.y),
                new Vector3(innerRect.x + innerRect.width, innerRect.y),
                new Vector3(innerRect.x + innerRect.width, innerRect.y + innerRect.height),
                new Vector3(innerRect.x, innerRect.y + innerRect.height) });

            Handles.color = Color.white;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(localSliderPosition - 4f, innerRect.y - 6f),
                new Vector3(localSliderPosition + 4f, innerRect.y - 6f),
                new Vector3(localSliderPosition, innerRect.y) });

            Handles.EndGUI();
        }
    }
}
