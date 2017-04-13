using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Meta.Tools.Editor
{
    public class ChaptersHeadsView : TimelineViewBase
    {
        #region Private Variables
        private Color _backgroundColor = new Color(23f / 255f, 26f / 255f, 29f / 255f);
        private Color _dividerLineColor = new Color(39f / 255f, 42f / 255f, 46f / 255f);

        private Texture2D _openArrowTexture;
        private Texture2D _closeArrowTexture;

        private TracksView _tracksView;

        private float _defaultHeight = 32f;
        private float _defaultWidth = 180f;
        private float _widthForList = 40f;

        private float _birdyWidth = 14f;
        private float _padding = 8f;
        private float _dividerLinePadding = 14f;
        private float _blackBoxPadding = 12f;
        private float _blackBoxHeight = 36f;

        private float _doubleClickTime = 0.3f;
        private float _lastClickTime;

        private List<ChapterHeader> _chapterHeaders = new List<ChapterHeader>();
        private Keynote _currentManipulatedChapter = null;
        private Keynote _currentSelectedChapter = null;
        #endregion

        #region Private Classes
        private class ChapterHeader
        {
            public Rect BodyRect;
            public Rect NameRect;
            public Keynote Chapter;
        }
        #endregion

        #region Constructor
        public ChaptersHeadsView(TracksView tracksView)
        {
            SetTracksView(tracksView);

            _openArrowTexture = (Texture2D)Resources.Load("Textures/NewDesign/Icon_Arrow_Open");
            _closeArrowTexture = (Texture2D)Resources.Load("Textures/NewDesign/Icon_Arrow_Close");
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            DrawChapters();
        }

        private void DrawChapters()
        {
            _chapterHeaders.Clear();
            for (int i = 0; i < _timeline.Keynotes.Count; i++)
            {
                DrawChapter(_timeline.Keynotes[i]);
            }
        }

        private void DrawChapter(Keynote chapter)
        {
            if (chapter.Opened)
            {
                DrawOpened(chapter);
            }
            else
            {
                DrawClosed(chapter);
            }
        }

        private void DrawOpened(Keynote chapter)
        {
            float xCoordinate = _tracksView.TracksField.x + chapter.Timing * _timeline.PixelPerSecond;

            Rect commonRect = new Rect(xCoordinate, 0f, _defaultWidth + _widthForList, _rect.height);
            GUILayout.BeginArea(commonRect);

            Rect areaRect = new Rect(0f, 0f, _defaultWidth, commonRect.height);
            GUILayout.BeginArea(areaRect);

            Handles.BeginGUI();
            Handles.color = _backgroundColor;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(0, 0),
                new Vector3(0, areaRect.height),
                new Vector3(areaRect.width, areaRect.height),
                new Vector3(areaRect.width, 0)});
            Handles.EndGUI();
            
            GUIContent chapterHeaderText = new GUIContent();
            chapterHeaderText.text = chapter.Name;
            GUIStyle style = _viewSkin.GetStyle("ChapterHeadTextStyle");
            Vector2 headerTextSize = style.CalcSize(chapterHeaderText);
            Rect nameRect = new Rect(_blackBoxPadding, (_defaultHeight - headerTextSize.y) / 2f, areaRect.width - _padding * 2f - _birdyWidth - _blackBoxPadding, headerTextSize.y);
            GUI.Label(nameRect, chapterHeaderText, style);
            if (GUI.Button(new Rect(areaRect.width - _padding - _birdyWidth, _defaultHeight * 0.4f, _birdyWidth, _defaultHeight * 0.2f), GUIContent.none, chapter.Opened ? _viewSkin.GetStyle("ChapterOpenerCloseStyle") : _viewSkin.GetStyle("ChapterOpenerOpenStyle")))
            {
                chapter.Opened = !chapter.Opened;
            }

            //----

            Handles.BeginGUI();
            Handles.color = _dividerLineColor;
            float dividerLineHeight = (_defaultHeight - headerTextSize.y) / 2f + headerTextSize.y + _dividerLinePadding;
            Handles.DrawLine(new Vector3(_dividerLinePadding, dividerLineHeight), new Vector3(areaRect.width - _dividerLinePadding, dividerLineHeight));
            Handles.EndGUI();

            //----

            Handles.BeginGUI();
            Handles.color = Color.black;
            float blackBoxHeight = dividerLineHeight + _dividerLinePadding;
            Rect blackBoxRect = new Rect(_blackBoxPadding, blackBoxHeight, areaRect.width - _blackBoxPadding * 2f, _blackBoxHeight);
            Handles.DrawAAConvexPolygon(new[] { new Vector3(blackBoxRect.x, blackBoxRect.y),
                new Vector3(blackBoxRect.x + blackBoxRect.width, blackBoxRect.y),
                new Vector3(blackBoxRect.x + blackBoxRect.width, blackBoxRect.y + blackBoxRect.height),
                new Vector3(blackBoxRect.x, blackBoxRect.y + blackBoxRect.height)});
            Handles.EndGUI();

            GUILayout.BeginArea(blackBoxRect);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Action on Start", _viewSkin.GetStyle("ChapterOptionsTextStyle"));
            Rect actionOnStartRect = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            //----

            float optionHeight = blackBoxRect.y + blackBoxRect.height/* + _dividerLinePadding*/;
            Rect optionRect = new Rect(_blackBoxPadding, optionHeight, areaRect.width - _blackBoxPadding * 2f, _blackBoxHeight);

            GUILayout.BeginArea(optionRect);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            switch (chapter.OnKeynoteStartAction)
            {
                case OnKeynoteStartAction.Loop:
                    GUILayout.Label("Loop", _viewSkin.GetStyle("ChapterOptionsTextStyle"));
                    Rect lastRect = GUILayoutUtility.GetLastRect();

                    bool newLoopedOnStart = CustomGUI.DrawToggle(new Rect(areaRect.width - 50f, lastRect.y + 10f, 20f, 20f), chapter.LoopedOnStart);
                    if (newLoopedOnStart != chapter.LoopedOnStart)
                    {
                        Undo.RecordObject(_timeline, "Chapter's Loop Setting Changed");
                        chapter.LoopedOnStart = newLoopedOnStart;
                    }
                    break;
                case OnKeynoteStartAction.Delay:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Delay", _viewSkin.GetStyle("ChapterOptionsTextStyle"));

                    GUILayout.FlexibleSpace();

                    float newDelayOnStart = EditorGUILayout.FloatField(chapter.DelayOnStart, GUILayout.Width(40f));
                    if (newDelayOnStart != chapter.DelayOnStart)
                    {
                        Undo.RecordObject(_timeline, "Chapter's Delay Setting Changed");
                        chapter.DelayOnStart = newDelayOnStart;
                    }
                    GUILayout.EndHorizontal();
                    break;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
            
            GUILayout.EndArea();


            OnKeynoteStartAction newActionOnStart = (OnKeynoteStartAction)CustomGUI.EnumPopup(new Rect(blackBoxRect.x + blackBoxRect.width - 50, blackBoxRect.y, 46f, blackBoxRect.height), commonRect ,chapter.OnKeynoteStartAction);
            if (newActionOnStart != chapter.OnKeynoteStartAction)
            {
                Undo.RecordObject(_timeline, "Chapter's Start Action Changed");
                chapter.OnKeynoteStartAction = newActionOnStart;
            }

            GUILayout.EndArea();

            _chapterHeaders.Add(new ChapterHeader() { Chapter = chapter, BodyRect = new Rect(commonRect.x, commonRect.y, areaRect.width, areaRect.height), NameRect = nameRect });
        }

        private void DrawClosed(Keynote chapter)
        {
            float xCoordinate = _tracksView.TracksField.x + chapter.Timing * _timeline.PixelPerSecond;

            GUIContent chapterHeaderText = new GUIContent();
            chapterHeaderText.text = chapter.Name;
            GUIStyle style = _viewSkin.GetStyle("ChapterHeadTextStyle");
            Vector2 headerTextSize = style.CalcSize(chapterHeaderText);

            float headerWidth = _padding * 3f + _birdyWidth + headerTextSize.x;

            Rect areaRect = new Rect(xCoordinate, _rect.height - _defaultHeight, headerWidth, _defaultHeight);
            GUILayout.BeginArea(areaRect);

            Handles.BeginGUI();
            Handles.color = _backgroundColor;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(0, 0),
                new Vector3(0, areaRect.height),
                new Vector3(areaRect.width, areaRect.height),
                new Vector3(areaRect.width, 0)});
            Handles.EndGUI();

            Rect nameRect = new Rect(_padding, (_defaultHeight - headerTextSize.y) / 2f, headerTextSize.x, headerTextSize.y);
            GUI.Label(nameRect, chapterHeaderText, style);
            if (GUI.Button(new Rect(_padding * 2f + headerTextSize.x, _defaultHeight * 0.4f, _birdyWidth, _defaultHeight * 0.2f), GUIContent.none, chapter.Opened ? _viewSkin.GetStyle("ChapterOpenerCloseStyle") : _viewSkin.GetStyle("ChapterOpenerOpenStyle")))
            {
                chapter.Opened = !chapter.Opened;
            }
            GUILayout.EndArea();

            _chapterHeaders.Add(new ChapterHeader() { Chapter = chapter, BodyRect = areaRect, NameRect = new Rect(areaRect.x + nameRect.x, areaRect.y + nameRect.y, nameRect.width, nameRect.height) });
        }

        private void DrawArrow(Rect rect, bool opened)
        {
            if (opened)
            {
                GUI.DrawTexture(rect, _closeArrowTexture);
            }
            else
            {
                GUI.DrawTexture(rect, _openArrowTexture);
            }
        }

        private void DrawBirdy(Rect rect)
        {
            float birdyFatness = 1f;

            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawAAConvexPolygon(new[] { new Vector3(rect.x, rect.y + rect.height - birdyFatness),
                new Vector3(rect.x + rect.width / 2f - birdyFatness / 2f, rect.y),
                new Vector3(rect.x + rect.width / 2f + birdyFatness / 2f, rect.y),
                new Vector3(rect.x + rect.width / 2f + birdyFatness / 2f, rect.y + birdyFatness),
                new Vector3(rect.x + birdyFatness, rect.y + rect.height)
            });
            Handles.DrawAAConvexPolygon(new[] { new Vector3(rect.x + rect.width / 2f - birdyFatness / 2f, rect.y + birdyFatness),
                new Vector3(rect.x + rect.width / 2f - birdyFatness / 2f, rect.y),
                new Vector3(rect.x + rect.width / 2f + birdyFatness / 2f, rect.y),
                new Vector3(rect.x + rect.width, rect.y + rect.height - birdyFatness),
                new Vector3(rect.x + rect.width - birdyFatness, rect.y + rect.height)
            });
            Handles.EndGUI();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (_currentManipulatedChapter != null)
                    {
                        float deltaTiming = (_mousePosition.x - _currentManipulatedChapter.StartDragingPosition.x) / _timeline.PixelPerSecond;
                        if (e.shift)
                        {
                            _timeline.SetTimingsFrom(_currentManipulatedChapter.PreviousTiming, deltaTiming);
                        }
                        float newTiming = _currentManipulatedChapter.PreviousTiming + deltaTiming;
                        if (newTiming < 0f)
                        {
                            newTiming = 0f;
                        }
                        else if (newTiming > _timeline.Duration)
                        {
                            newTiming = _timeline.Duration;
                        }
                        _currentManipulatedChapter.Timing = newTiming;
                    }
                    break;
                case EventType.MouseUp:
                    if (_currentManipulatedChapter != null)
                    {
                        float temp = _currentManipulatedChapter.Timing;
                        _currentManipulatedChapter.Timing = _currentManipulatedChapter.PreviousTiming;
                        Undo.RecordObject(_timeline, "Chapters's Position Changed");
                        _currentManipulatedChapter.Timing = temp;
                        _timeline.SortKeynotes();
                        _currentManipulatedChapter = null;
                    }
                    _currentSelectedChapter = null;
                    break;
                case EventType.KeyUp:
                    switch(e.keyCode)
                    {
                        case KeyCode.Delete:
                            if (_currentSelectedChapter != null)
                            {
                                _timeline.RemoveKeynote(_currentSelectedChapter);
                                _currentSelectedChapter = null;
                            }
                            break;
                    }
                    break;
            }

            for (int i = _chapterHeaders.Count - 1; i >= 0; i--)
            {
                if (_chapterHeaders[i].BodyRect.Contains(_mousePosition))
                {
                    switch(e.type)
                    {
                        case EventType.MouseDown:
                            if (e.button == 0)
                            {
                                _chapterHeaders[i].Chapter.PreviousTiming = _chapterHeaders[i].Chapter.Timing;
                                _chapterHeaders[i].Chapter.StartDragingPosition = _mousePosition;
                                _currentManipulatedChapter = _chapterHeaders[i].Chapter;

                                if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickTime)
                                {
                                    processDoubleClick(_chapterHeaders[i].Chapter);
                                }
                                _lastClickTime = (float)EditorApplication.timeSinceStartup;

                                _timeline.SavePreviousTimingsStartingFrom(_currentManipulatedChapter.Timing);
                            }
                            else if (e.button == 1)
                            {
                                if (_chapterHeaders[i].NameRect.Contains(_mousePosition))
                                {
                                    ChangeKeynotesNameWindow.Init(_chapterHeaders[i].Chapter, _timeline, _serializedObject);
                                }
                            }
                            break;
                        case EventType.MouseUp:
                            _currentSelectedChapter = _chapterHeaders[i].Chapter;
                            break;
                    }
                    break;
                }
            }
        }

        private void processDoubleClick(Keynote chapter)
        {
            chapter.Opened = !chapter.Opened;
        }

        public void SetTracksView(TracksView tracksView)
        {
            _tracksView = tracksView;
        }
        #endregion

        #region Utility Methods
        #endregion
    }
}
