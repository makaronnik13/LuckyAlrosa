using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Meta.Tools.Editor
{
    public class ChaptersSubview : WorkViewBase
    {
        #region Private Variables
        private float _thinChaptersWidth = 3f;

        private float _doubleClickTime = 0.3f;
        private float _lastClickOnChapterTime;

        private Keynote _currentManipulatedKeynote;
        private Keynote _currentSelectedKeynote;
        
        private List<WorkView.Subview> _subviews = new List<WorkView.Subview>();
        
        private float initialX;
        private float pathMade;
        #endregion

        #region Constructor
        public ChaptersSubview()
        {

        }
        #endregion

        #region Main Methods
        public override void Draw(Rect localDrawingRect, Rect globalDrawingRect, Vector2 parentOffset, Rect field, Rect viewRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, WorkView workView, float timing0 = 0, float timing1 = 1, float chapterTiming0 = 0, float chapterTiming1 = 1)
        {
            base.Draw(localDrawingRect, globalDrawingRect, parentOffset, field, viewRect, timeline, serializedObject, timelineWindow, workView, timing0, timing1, chapterTiming0, chapterTiming1);

            _realRect.y = viewRect.y;
            _realRect.height = viewRect.height;
        }

        public void DrawThinChapter(Keynote keynote)
        {
            Rect lineRect = new Rect(_realRect.x, _realRect.y, _thinChaptersWidth, _realRect.height);

            WorkView.Subview newSubview = new WorkView.Subview();
            newSubview.RealRect = lineRect;
            newSubview.Timing0 = _timing0;
            newSubview.Timing1 = _timing1;
            newSubview.UpdateInstanceID(GUIUtility.GetControlID(FocusType.Passive) + 1);
            _subviews.Add(newSubview);

            float backgroundMultiplier = 1f;
            if (_currentManipulatedKeynote == keynote)
            {
                backgroundMultiplier = 1.18f;
            }
            else if (_currentSelectedKeynote == keynote)
            {
                backgroundMultiplier = 1.4f;
            }

            Handles.BeginGUI();

            Handles.color = keynote.BackgroundColor * backgroundMultiplier;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(lineRect.x, lineRect.y),
                new Vector3(lineRect.x + _thinChaptersWidth, lineRect.y),
                new Vector3(lineRect.x + _thinChaptersWidth, lineRect.y + lineRect.height),
                new Vector3(lineRect.x, lineRect.y + lineRect.height)});

            Handles.EndGUI();

            keynote.LeftBorderRect = new Rect();
            keynote.RightBorderRect = new Rect();
            keynote.BodyRect = lineRect;
        }

        #region Drawing Keynote
        public void DrawKeynote(Keynote keynote)
        {
            Rect lineRect = new Rect(_realRect.x, _realRect.y, keynote.Width, _realRect.height);
            
            WorkView.Subview newSubview = new WorkView.Subview();
            newSubview.RealRect = _realRect;
            newSubview.Timing0 = _timing0;
            newSubview.Timing1 = _timing1;
            newSubview.UpdateInstanceID(GUIUtility.GetControlID(FocusType.Passive) + 1);
            _subviews.Add(newSubview);

            float backgroundMultiplier = 1f;
            if (_currentManipulatedKeynote == keynote)
            {
                backgroundMultiplier = 1.18f;
            }
            else if (_currentSelectedKeynote == keynote)
            {
                backgroundMultiplier = 1.4f;
            }

            Handles.BeginGUI();
            
            Handles.color = keynote.BackgroundColor * backgroundMultiplier;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(lineRect.x, lineRect.y),
                new Vector3(lineRect.x + lineRect.width, lineRect.y),
                new Vector3(lineRect.x + lineRect.width, lineRect.y + lineRect.height),
                new Vector3(lineRect.x, lineRect.y + lineRect.height)});

            Handles.EndGUI();

            if (keynote.Opened)
            {
                Rect guiAreaRect = new Rect(lineRect.x + 2f, lineRect.y + 2f, lineRect.width - 4f, lineRect.height - 4f);
                float buttonsWidth = 40f;
                GUILayout.BeginArea(guiAreaRect);

				#if UNITY_EDITOR_WIN
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(guiAreaRect.width - 40f));
                Color newColor = EditorGUILayout.ColorField(keynote.BackgroundColor, GUILayout.Width(32f));
                GUILayout.EndHorizontal();

                if (newColor != keynote.BackgroundColor)
                {
                    Undo.RecordObject(_timeline, "Chapter's Background Color Changed");
                    keynote.BackgroundColor = newColor;
                }
				#endif

                GUILayout.BeginVertical();

                GUILayout.Space(16f);
                GUILayout.Label(keynote.Name, _viewSkin.GetStyle("KeynoteNameStyle"), GUILayout.Width(guiAreaRect.width));

                GUILayout.Label("(" + ((int)Mathf.Floor(keynote.Timing / 60f)).ToString() + ":" + ((int)Mathf.Floor(keynote.Timing % 60f)).ToString() + ":" + ((int)Mathf.Floor((keynote.Timing - Mathf.Floor(keynote.Timing)) * 100f)).ToString() + ")",
                    _viewSkin.GetStyle("KeynoteTimingStyle"));

                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(guiAreaRect.width - buttonsWidth - 10f));
                if (GUILayout.Button("Edit", _viewSkin.GetStyle("KeynoteBoxStyle"), new[] { GUILayout.Width(buttonsWidth), GUILayout.Height(14f) }))
                {
                    ChangeKeynotesNameWindow.Init(keynote, _timeline, _serializedObject);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(16f);
                GUILayout.Label("   " + keynote.Description, _viewSkin.GetStyle("KeynoteDescriptionStyle"), GUILayout.Width(guiAreaRect.width));
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(guiAreaRect.width - buttonsWidth - 10f));
                if (GUILayout.Button("Edit", _viewSkin.GetStyle("KeynoteBoxStyle"), new[] { GUILayout.Width(buttonsWidth), GUILayout.Height(14f) }))
                {
                    ChangeKeynotesDescriptionWindow.Init(keynote, _timeline, _serializedObject);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(16f);


                GUILayout.BeginHorizontal();
                GUILayout.Label("Action On Start: ", GUILayout.Width(100f));

                

                OnKeynoteStartAction newPauseCondition = (OnKeynoteStartAction)EditorGUILayout.EnumPopup(keynote.OnKeynoteStartAction, GUILayout.Width(guiAreaRect.width - 108f));
                if (newPauseCondition != keynote.OnKeynoteStartAction)
                {
                    Undo.RecordObject(_timeline, "Chapter's Start Action Changed");
                    keynote.OnKeynoteStartAction = newPauseCondition;
                }
                GUILayout.EndHorizontal();

                switch (newPauseCondition)
                {
                    case OnKeynoteStartAction.Delay:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Delay: ", GUILayout.Width(100f));
                        float newDelayOnStart = EditorGUILayout.FloatField(keynote.DelayOnStart, GUILayout.Width(guiAreaRect.width - 108f));
                        if (newDelayOnStart != keynote.DelayOnStart)
                        {
                            Undo.RecordObject(_timeline, "Chapter's Delay Setting Changed");
                            keynote.DelayOnStart = newDelayOnStart;
                        }
                        GUILayout.EndHorizontal();
                        break;
                    case OnKeynoteStartAction.Loop:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Loop: ", GUILayout.Width(100f));
                        bool newLoopedOnStart = EditorGUILayout.Toggle(keynote.LoopedOnStart, GUILayout.Width(guiAreaRect.width - 108f));
                        if (newLoopedOnStart != keynote.LoopedOnStart)
                        {
                            Undo.RecordObject(_timeline, "Chapter's Loop Setting Changed");
                            keynote.LoopedOnStart = newLoopedOnStart;
                        }
                        GUILayout.EndHorizontal();
                        break;
                }

                /*GUILayout.BeginHorizontal();
                GUILayout.Label("Launching Timer: ", GUILayout.Width(100f));
                float newDelay = EditorGUILayout.FloatField(keynote.LaunchingConditionInfo.Delay, GUILayout.Width(guiAreaRect.width - 108f));
                if (newDelay != keynote.LaunchingConditionInfo.Delay)
                {
                    Undo.RecordObject(_timeline, "Launching Timer's Delay Changed");
                    keynote.LaunchingConditionInfo.Delay = newDelay;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Looped: ", GUILayout.Width(100f));
                bool newLooping = EditorGUILayout.Toggle(keynote.Looped, GUILayout.Width(guiAreaRect.width - 108f));
                if (newLooping != keynote.Looped)
                {
                    Undo.RecordObject(_timeline, "Keynote's Loop Setting Changed");
                    keynote.Looped = newLooping;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Pause Action: ", GUILayout.Width(100f));
                ConditionType newPauseCondition = (ConditionType)EditorGUILayout.EnumPopup(keynote.PauseActionType, GUILayout.Width(guiAreaRect.width - 108f));
                if (newPauseCondition != keynote.PauseActionType)
                {
                    Undo.RecordObject(_timeline, "New Keynote's Pause Action Choosed");
                    keynote.PauseActionType = newPauseCondition;
                }
                GUILayout.EndHorizontal();

                switch (keynote.PauseActionType)
                {
                    case ConditionType.Timer:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Timer: ", GUILayout.Width(100f));
                        newDelay = EditorGUILayout.FloatField(keynote.PauseActionInfo.Delay, GUILayout.Width(guiAreaRect.width - 108f));
                        if (newDelay != keynote.PauseActionInfo.Delay)
                        {
                            Undo.RecordObject(_timeline, "Pause Action's Delay Changed");
                            keynote.PauseActionInfo.Delay = newDelay;
                        }
                        GUILayout.EndHorizontal();
                        break;
                    case ConditionType.KeyCode:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Key code: ", GUILayout.Width(100f));
                        AllowedKeyCodes newKeyCode = (AllowedKeyCodes)EditorGUILayout.EnumPopup(keynote.PauseActionInfo.KeyCode, GUILayout.Width(guiAreaRect.width - 108f));
                        if (newKeyCode != keynote.PauseActionInfo.KeyCode)
                        {
                            Undo.RecordObject(_timeline, "Pause Action's Key Code Changed");
                            keynote.PauseActionInfo.KeyCode = newKeyCode;
                        }
                        GUILayout.EndHorizontal();
                        break;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Next Action: ", GUILayout.Width(100f));
                AllowedKeyCodes newNextAction = (AllowedKeyCodes)EditorGUILayout.EnumPopup(keynote.NextActionInfo.KeyCode, GUILayout.Width(guiAreaRect.width - 108f));
                if (newNextAction != keynote.NextActionInfo.KeyCode)
                {
                    Undo.RecordObject(_timeline, "Next Action's Key Code Changed");
                    keynote.NextActionInfo.KeyCode = newNextAction;
                }
                GUILayout.EndHorizontal();*/
                
                /*GUILayout.Label("ConditionType:", _viewSkin.GetStyle("KeynoteDescriptionStyle"));
                ConditionType oldCondition = keynote.ConditionType;
                ConditionType newCondition = (ConditionType)EditorGUILayout.EnumPopup(keynote.ConditionType, GUILayout.Width(guiAreaRect.width - 6f));
                if (oldCondition != newCondition)
                {
                    Undo.RecordObject(_timeline, "New Keynote's Condition Choosed");
                    keynote.ConditionType = newCondition;
                }*/

                /*switch (keynote.LaunchingConditionType)
                {
                    case ConditionType.Timer:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Timer: ");
                        float newDelay = EditorGUILayout.FloatField(keynote.LaunchingConditionInfo.Delay);
                        if (newDelay != keynote.LaunchingConditionInfo.Delay)
                        {
                            Undo.RecordObject(_timeline, "Condition's Delay Changed");
                            keynote.LaunchingConditionInfo.Delay = newDelay;
                        }
                        GUILayout.EndHorizontal();
                        break;
                    case ConditionType.KeyCode:
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Key code: ");
                        KeyCode newKeyCode = (KeyCode)EditorGUILayout.EnumPopup(keynote.LaunchingConditionInfo.KeyCode);
                        if (newKeyCode != keynote.LaunchingConditionInfo.KeyCode)
                        {
                            Undo.RecordObject(_timeline, "Condition's Key Code Changed");
                            keynote.LaunchingConditionInfo.KeyCode = newKeyCode;
                        }
                        GUILayout.EndHorizontal();
                        break;
                }*/

                GUILayout.EndVertical();
                GUILayout.EndArea();

                keynote.LeftBorderRect = new Rect(lineRect.x - 2f, lineRect.y, 4f, lineRect.height);
                keynote.RightBorderRect = new Rect(lineRect.x + lineRect.width - 2f, lineRect.y, 4f, lineRect.height);
                EditorGUIUtility.AddCursorRect(keynote.LeftBorderRect, MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(keynote.RightBorderRect, MouseCursor.ResizeHorizontal);

                keynote.BodyRect = new Rect(lineRect.x + 2f, lineRect.y, lineRect.width - 4f, lineRect.height);
            }
            else
            {
                keynote.LeftBorderRect = new Rect();
                keynote.RightBorderRect = new Rect();
                keynote.BodyRect = lineRect;
            }
        }
        #endregion

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
            
            manipulateKeynotes(e);
        }

        private void manipulateKeynotes(Event e)
        {
            for (int i = 0; i < _subviews.Count; i++)
            {
                switch (e.GetTypeForControl(_subviews[i].InstanceID))
                {
                    case EventType.MouseDown:
                        _currentManipulatedKeynote = null;
                        checkKeynotesBodiesCycle(mousePosition, ref _currentManipulatedKeynote);

                        if (_currentManipulatedKeynote != null)
                        {
                            initialX = mousePosition.x;
                            pathMade = 0f;
                            e.Use();

                            _timeline.SavePreviousTimingsStartingFrom(_currentManipulatedKeynote.Timing);
                        }
                        //e.Use();
                        break;
                    case EventType.MouseUp:
                        if (e.button == 0)
                        {
                            if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickOnChapterTime && _currentManipulatedKeynote != null)
                            {
                                DoubleClickOnChapter(e);

                                /*if (e.type != EventType.Used)
                                {
                                    Debug.Log("e.type = " + e.type);
                                }*/
                            }
                            else
                            {
                                float temp;
                                if (_currentManipulatedKeynote != null)
                                {
                                    switch (_currentManipulatedKeynote.KeynoteDragingSide)
                                    {
                                        case KeynoteDragingSide.Body:
                                            temp = _currentManipulatedKeynote.Timing;
                                            _currentManipulatedKeynote.Timing = _currentManipulatedKeynote.PreviousTiming;
                                            Undo.RecordObject(_timeline, "Keynote's Position Changed");
                                            _currentManipulatedKeynote.Timing = temp;
                                            _timeline.SortKeynotes();
                                            break;
                                        case KeynoteDragingSide.LeftBorder:
                                            float tempTiming = _currentManipulatedKeynote.Timing;
                                            float tempWidth = _currentManipulatedKeynote.Width;
                                            _currentManipulatedKeynote.Timing = _currentManipulatedKeynote.PreviousTiming;
                                            _currentManipulatedKeynote.Width = _currentManipulatedKeynote.PreviousWidth;
                                            Undo.RecordObject(_timeline, "Keynote's Position and Width Changed");
                                            _currentManipulatedKeynote.Timing = tempTiming;
                                            _currentManipulatedKeynote.Width = tempWidth;
                                            _timeline.SortKeynotes();
                                            break;
                                        case KeynoteDragingSide.RightBorder:
                                            temp = _currentManipulatedKeynote.Width;
                                            _currentManipulatedKeynote.Width = _currentManipulatedKeynote.PreviousWidth;
                                            Undo.RecordObject(_timeline, "Keynote's Width Changed");
                                            _currentManipulatedKeynote.Width = temp;
                                            break;
                                    }

                                    e.Use();

                                    if (pathMade < 4f)
                                    {
                                        _currentSelectedKeynote = _currentManipulatedKeynote;
                                    }
                                    else
                                    {
                                        _currentSelectedKeynote = null;
                                    }
                                    _currentManipulatedKeynote = null;
                                }
                            }
                            
                            _lastClickOnChapterTime = (float)EditorApplication.timeSinceStartup;
                        }

                        _currentManipulatedKeynote = null;
                        break;
                    case EventType.MouseDrag:
                        if (_pressedLMB && _currentManipulatedKeynote != null)
                        {
                            pathMade += Mathf.Abs(initialX - mousePosition.x);
                            switch (_currentManipulatedKeynote.KeynoteDragingSide)
                            {
                                case KeynoteDragingSide.Body:
                                    float deltaTiming = (mousePosition.x - _currentManipulatedKeynote.StartDragingPosition.x) / _timeline.PixelPerSecond;
                                    if (e.shift)
                                    {
                                        _timeline.SetTimingsFrom(_currentManipulatedKeynote.PreviousTiming, deltaTiming);
                                    }
                                    _currentManipulatedKeynote.Timing = _currentManipulatedKeynote.PreviousTiming + deltaTiming;
                                    if (_currentManipulatedKeynote.Timing < 0f)
                                    {
                                        _currentManipulatedKeynote.Timing = 0f;
                                    }
                                    else if (_currentManipulatedKeynote.Timing > _timeline.Duration)
                                    {
                                        _currentManipulatedKeynote.Timing = _timeline.Duration;
                                    }
                                    break;
                                case KeynoteDragingSide.LeftBorder:
                                    _currentManipulatedKeynote.Timing = _currentManipulatedKeynote.PreviousTiming + (mousePosition.x - _currentManipulatedKeynote.StartDragingPosition.x) / _timeline.PixelPerSecond;
                                    if (_currentManipulatedKeynote.Timing < 0f)
                                    {
                                        _currentManipulatedKeynote.Timing = 0f;
                                    }
                                    else if (_currentManipulatedKeynote.Timing > _timeline.Duration)
                                    {
                                        _currentManipulatedKeynote.Timing = _timeline.Duration;
                                    }
                                    _currentManipulatedKeynote.Width = _currentManipulatedKeynote.PreviousWidth + (_currentManipulatedKeynote.PreviousTiming - _currentManipulatedKeynote.Timing) * _timeline.PixelPerSecond;
                                    break;
                                case KeynoteDragingSide.RightBorder:
                                    _currentManipulatedKeynote.Width = _currentManipulatedKeynote.PreviousWidth + (mousePosition.x - _currentManipulatedKeynote.StartDragingPosition.x);
                                    break;
                            }

                            e.Use();
                        }
                        break;
                }
            }

            switch (e.type)
            {
                case EventType.MouseUp:
                    WorkView.Subview activeSubview = null;
                    for (int i = 0; i < _subviews.Count; i++)
                    {
                        if (_subviews[i].RealRect.Contains(mousePosition))
                        {
                            activeSubview = _subviews[i];
                            break;
                        }
                    }

                    if (activeSubview == null)
                    {
                        _currentSelectedKeynote = null;
                    }
                    break;
                case EventType.KeyUp:
                    switch (e.keyCode)
                    {
                        case KeyCode.Delete:
                            DeleteActionDone();
                            break;
                        case KeyCode.Backspace:
                            if (e.command || e.control || e.keyCode == KeyCode.LeftCommand || e.keyCode == KeyCode.LeftApple ||
                                     e.keyCode == KeyCode.RightCommand || e.keyCode == KeyCode.RightApple)
                            {
                                DeleteActionDone();
                            }
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region Utility Methods
        private void DoubleClickOnChapter(Event e)
        {
            Keynote temp = null;
            checkKeynotesBodiesCycle(mousePosition, ref temp);
            if (temp != null)
            {
                temp.Opened = !temp.Opened;

                e.Use();
            }
        }

        private void DeleteActionDone()
        {
            if (_currentSelectedKeynote != null)
            {
                _timeline.RemoveKeynote(_currentSelectedKeynote);
                _currentSelectedKeynote = null;
            }
        }
        public void ClearSubviews()
        {
            _subviews.Clear();
        }

        private float getTimesPosition(float time)
        {
            float position = _realRect.width * ((time - _timing0) / (_timing1 - _timing0));

            return position;
        }

        private float getPositionsTime(float position)
        {
            float time = _timing0 + ((position - _realRect.x) / _realRect.width) * (_timing1 - _timing0);

            return time;
        }

        private void checkKeynotesBodiesCycle(Vector2 mousePosition, ref Keynote target)
        {
            if (_timeline != null)
            {
                List<Keynote> keynotes = _timeline.Keynotes;
                for (int j = 0; j < keynotes.Count; j++)
                {
                    if (keynotes[j].BodyRect.Contains(mousePosition))
                    {
                        target = keynotes[j];
                        target.StartDragingPosition = mousePosition;
                        target.PreviousTiming = target.Timing;
                        target.PreviousWidth = target.Width;
                        target.KeynoteDragingSide = KeynoteDragingSide.Body;
                        return;
                    }
                    if (keynotes[j].LeftBorderRect.Contains(mousePosition))
                    {
                        target = keynotes[j];
                        target.StartDragingPosition = mousePosition;
                        target.PreviousTiming = target.Timing;
                        target.PreviousWidth = target.Width;
                        target.KeynoteDragingSide = KeynoteDragingSide.LeftBorder;
                        return;
                    }
                    if (keynotes[j].RightBorderRect.Contains(mousePosition))
                    {
                        target = keynotes[j];
                        target.StartDragingPosition = mousePosition;
                        target.PreviousTiming = target.Timing;
                        target.PreviousWidth = target.Width;
                        target.KeynoteDragingSide = KeynoteDragingSide.RightBorder;
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
