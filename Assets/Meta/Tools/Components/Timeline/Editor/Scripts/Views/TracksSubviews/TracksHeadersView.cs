using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class TracksHeadersView : TracksViewBase
    {
        #region Private Variables
        private TimelineTrack _currentlyManipulatedTrack = null;
        private int _highlightedVisiblePosition = -1;

        private static Texture2D _backgoundGradient;
        private static Texture2D _backgoundTrackSelectedGradient;
        private static Texture2D _backgoundTrackUnselectedGradient;

        /*private static Texture2D _eyeSelectedNormal;
        private static Texture2D _eyeSelectedActive;
        private static Texture2D _eyeUnselectedNormal;
        private static Texture2D _eyeUnselectedActive;
        private static Texture2D _soloSelectedNormal;
        private static Texture2D _soloSelectedActive;
        private static Texture2D _soloUnselectedNormal;
        private static Texture2D _soloUnselectedActive;
        private static Texture2D _newKeyframeSelectedNormal;
        private static Texture2D _newKeyframeSelectedActive;
        private static Texture2D _newKeyframeUnselectedNormal;
        private static Texture2D _newKeyframeUnselectedActive;*/

        private GUIContent _eyeGUIContent;
        private GUIContent _dotGUIContent;
        private GUIContent _newGUIContent;

        private Rect _field = new Rect();
        
        private bool _pressedLMB;
        //private bool _pressedRMB;
        
        private Vector2 _initialMousePos;
        private Vector2 _initialFieldPos;
        
        private float _pathMade;

        private float _doubleClickTime = 0.3f;
        private float _lastClickOnChapterTime;

        private float _buttonsHeight = 16f;
        private float _interpolationButtonHeight = 28f;

        private Color _trackBackgroundColor = new Color(194f / 255f, 194f / 255f, 194f / 255f);
        #endregion

        #region Constructor
        public TracksHeadersView()
        {
            _backgoundGradient = Resources.Load("Textures/NewDesign/track_background") as Texture2D;
            _backgoundTrackSelectedGradient = Resources.Load("Textures/NewDesign/track_background_selected") as Texture2D;
            _backgoundTrackUnselectedGradient = Resources.Load("Textures/NewDesign/track_background_unselected") as Texture2D;

            _eyeGUIContent = new GUIContent();
            _eyeGUIContent.tooltip = "Show Track";

            _dotGUIContent = new GUIContent();
            _dotGUIContent.tooltip = "Solo Track";

            _newGUIContent = new GUIContent();
            _newGUIContent.tooltip = "New Keyframe";

            /*_eyeSelectedNormal = Resources.Load("Textures/NewDesign/Icon_Eye_Static") as Texture2D;
            _eyeSelectedActive = Resources.Load("Textures/NewDesign/Icon_Eye_Over") as Texture2D;
            _eyeUnselectedNormal = Resources.Load("Textures/NewDesign/Eye_Static") as Texture2D;
            _eyeUnselectedActive = Resources.Load("Textures/NewDesign/Eye_Over") as Texture2D;

            _soloSelectedNormal = Resources.Load("Textures/NewDesign/Icon_Dot_Static") as Texture2D;
            _soloSelectedActive = Resources.Load("Textures/NewDesign/Icon_Dot_Over") as Texture2D;
            _soloUnselectedNormal = Resources.Load("Textures/NewDesign/Dot_Static") as Texture2D;
            _soloUnselectedActive = Resources.Load("Textures/NewDesign/Dot_Over") as Texture2D;

            _newKeyframeSelectedNormal = Resources.Load("Textures/NewDesign/Icon_Add_KeyFrame_Static") as Texture2D;
            _newKeyframeSelectedActive = Resources.Load("Textures/NewDesign/Icon_Add_KeyFrame_Over") as Texture2D;
            _newKeyframeUnselectedNormal = Resources.Load("Textures/NewDesign/Add_KeyFrame_Static") as Texture2D;
            _newKeyframeUnselectedActive = Resources.Load("Textures/NewDesign/Add_KeyFrame_Over") as Texture2D;*/

            _field.y = 0f;
        }
        #endregion

        #region Main Methods
        public static void DeselectAllTracks()
        {
            TracksView.SelectedTrack = -1;
        }

        /*public override void UpdateView(Rect editorRect, Rect percentageRect, Timeline timeline, SerializedObject serializedObject, TimelineWindow timelineWindow, TracksView tracksView)
        {
            base.UpdateView(editorRect, percentageRect, timeline, serializedObject, timelineWindow, tracksView);

            initializeField();

            Rect bottomRect = ViewRect;
            bottomRect.height = editorRect.height - editorRect.height * percentageRect.height - editorRect.height * percentageRect.height;
            bottomRect.y = 0f;
            //bottomRect.y = editorRect.height * percentageRect.height;
            bottomRect.x = 0f;
            bottomRect.width = editorRect.width;
            GUILayout.BeginArea(ViewRect);
            drawTracksHeaders();
            GUILayout.EndArea();

            //ProcessEvents(e);
        }*/

        protected override void Draw()
        {
            base.Draw();
            
            initializeField();

            drawTracksHeaders();

            //Debug.Log("headers: _field.y = " + _field.y);
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            Vector2 mousePosition = e.mousePosition;
            mousePosition.x -= _offset.x;
            mousePosition.y -= _offset.y;

            _highlightedVisiblePosition = -1;
            if (_currentlyManipulatedTrack != null)
            {
                _highlightedVisiblePosition = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
            }
            
            if (_rect.Contains(mousePosition))
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (e.button == 0)
                        {
                            _pressedLMB = true;
                            _initialMousePos = mousePosition;
                            _initialFieldPos = new Vector2(0f, _field.y);

                            _pathMade = 0f;
                        }
                        else if (e.button == 1)
                        {
                            //_pressedRMB = true;
                            _currentlyManipulatedTrack = null;
                            checkTracksBodiesCycle(mousePosition, ref _currentlyManipulatedTrack);
                            if (_currentlyManipulatedTrack != null)
                            {
                                _currentlyManipulatedTrack.CurrentMousePosition = mousePosition;
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        int row = (int)Mathf.Floor((mousePosition.y - _field.y) / _timeline.PixelPerTrack);
                        if (e.button == 0)
                        {
                            _pressedLMB = false;
                            
                            if (_pathMade < 7f)
                            {
                                if (row >= 0)
                                {
                                    if (TracksView.SelectedTrack == row)
                                    {
                                        TracksView.SelectedTrack = -1;
                                    }
                                    else
                                    {
                                        TracksView.SelectedTrack = row;
                                        UnityEngine.Object target = _timeline.Tracks.GetTrackOnTracksViewPosition(row).Target;
                                        if (target is Component)
                                        {
                                            target = (target as Component).gameObject;
                                        }
                                        Selection.activeGameObject = target as GameObject;
                                    }
                                }

                                if (_doubleClickTime >= (float)EditorApplication.timeSinceStartup - _lastClickOnChapterTime)
                                {
                                    TimelineTrack track = null;
                                    checkTracksBodiesCycle(mousePosition, ref track);
                                    if (track != null)
                                    {
                                        ChangeTracksNameWindow.Init(track, _timeline, _serializedObject);
                                    }
                                }

                                _lastClickOnChapterTime = (float)EditorApplication.timeSinceStartup;
                            }
                        }
                        else if (e.button == 1)
                        {
                            //_pressedRMB = false;

                            //ProcessContextMenu(row);
                            if (_currentlyManipulatedTrack != null && Vector2.Distance(_currentlyManipulatedTrack.CurrentMousePosition, _currentlyManipulatedTrack.StartingManipulationMousePosition) > 0f)
                            {
                                _timeline.Tracks.Swap(_currentlyManipulatedTrack, _highlightedVisiblePosition);
                            }
                            else
                            {
                                ProcessContextMenu(row);
                            }
                        }

                        _currentlyManipulatedTrack = null;
                        break;
                    case EventType.MouseDrag:
                        if (_pressedLMB)
                        {
                            _pathMade += Vector2.Distance(_initialMousePos, mousePosition);
                            _tracksView.SetTargetFieldY(_initialFieldPos.y + (mousePosition.y - _initialMousePos.y));
                        }
                        if (_currentlyManipulatedTrack != null)
                        {
                            _currentlyManipulatedTrack.CurrentMousePosition = mousePosition;
                        }
                        break;
                    case EventType.ScrollWheel:
                        /*Undo.RecordObject(_timeline, "Track Resolution Changed");
                        _timeline.ChangeTrackResolution(e.delta.y);*/

                        float horizontalDelta = e.delta.x;
                        float verticalDelta = e.delta.y * -1f;

                        _tracksView.NavigationStart(new Vector2(0f, 0f));
                        _tracksView.Navigate((new Vector2(horizontalDelta, verticalDelta)) * 15f);
                        break;
                    case EventType.KeyUp:
                        switch (e.keyCode)
                        {
                            case KeyCode.Delete:
                                if (TracksView.SelectedTrack >= 0)
                                {
                                    //_timeline.DeleteTrackOnTracksViewPosition(TracksView.SelectedTrack);
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                _pressedLMB = false;
                //_pressedRMB = false;
                _currentlyManipulatedTrack = null;
            }
        }

        private void ProcessContextMenu(int row)
        {
            GenericMenu menu = new GenericMenu();

            TimelineTrack track = _timeline.Tracks.TrackOnTracksViewPosition(row);

            if (track == null)
            {
                //menu.AddItem(new GUIContent("Create New Track"), false, CreateNewTrack, row);
            }
            else
            {
                menu.AddItem(new GUIContent("Remove Track"), false, RemoveTrack, row);
            }

            menu.ShowAsContext();
        }

        private void CreateNewTrack(object obj)
        {
            _timeline.Tracks.GetTrackOnTracksViewPosition((int)obj);
        }

        private void RemoveTrack(object obj)
        {
            _timeline.Tracks.DeleteTrackOnTracksViewPosition((int)obj);
        }

        public override void SetNewFieldPosition(Vector2 fieldPosition)
        {
            _field.y = fieldPosition.y;
        }
        #endregion

        #region Drawing Methods
        private void drawTracksHeaders()
        {
            int startTrack;
            int endTrack;

            findTracksRange(_field.y, _height, _timeline.PixelPerTrack, out startTrack, out endTrack);
            endTrack++;

            for (int i = startTrack; i < endTrack; i++)
            {
                Rect headerRect = new Rect(1, i * _timeline.PixelPerTrack + _field.y, _width - 0, _timeline.PixelPerTrack);
                /*if (headerRect.y + headerRect.height > ViewRect.y + ViewRect.height)
                {
                    headerRect.height = ViewRect.y + ViewRect.height - headerRect.y;
                }*/

                GUI.DrawTexture(headerRect, _backgoundGradient, ScaleMode.StretchToFill);

                Handles.BeginGUI();

                Handles.color = Color.gray * 1.2f;
                Handles.DrawLine(new Vector3(headerRect.x, headerRect.y), new Vector3(headerRect.x + headerRect.width, headerRect.y));
                Handles.DrawLine(new Vector3(headerRect.x, headerRect.y + headerRect.height), new Vector3(headerRect.x + headerRect.width, headerRect.y + headerRect.height));

                Handles.EndGUI();
            }

            int currentlySelectedIndex = 0;
            for (int i = 0; i < _timeline.Tracks.Count; i++)
            {
                if (_timeline.Tracks.TrackOnIndex(i).TracksViewPosition >= startTrack && _timeline.Tracks.TrackOnIndex(i).TracksViewPosition <= endTrack)
                {
                    if (_timeline.Tracks.TrackOnIndex(i) != _currentlyManipulatedTrack)
                    {
                        drawTracksHeader(i);
                    }
                    else
                    {
                        currentlySelectedIndex = i;
                    }
                }
            }

            if (_currentlyManipulatedTrack != null)
            {
                drawTracksHeader(currentlySelectedIndex);
            }

            Handles.BeginGUI();

            Handles.color = Color.gray * 1.2f;
            Vector3 deviderLineBeginning = new Vector3(_width - 0.01f, 0);
            Vector3 deviderLineEnding = new Vector3(_width - 0.01f, _height);
            if (_field.y >= 0)
            {
                deviderLineBeginning = new Vector3(_width - 0.01f,  _field.y);
                deviderLineEnding = new Vector3(_width - 0.01f, _height + _field.y);
            }
            Handles.DrawLine(deviderLineBeginning, deviderLineEnding);

            Handles.EndGUI();
        }

        private void drawTracksHeader(int index)
        {
            Rect headerRect = new Rect(0, _timeline.Tracks.TrackOnIndex(index).TracksViewPosition * _timeline.PixelPerTrack + _field.y, _width, _timeline.PixelPerTrack);
            if (_currentlyManipulatedTrack == _timeline.Tracks.TrackOnIndex(index))
            {
                headerRect.x += _currentlyManipulatedTrack.CurrentMousePosition.x - _currentlyManipulatedTrack.StartingManipulationMousePosition.x;
                headerRect.y += _currentlyManipulatedTrack.CurrentMousePosition.y - _currentlyManipulatedTrack.StartingManipulationMousePosition.y;
            }
            _timeline.Tracks.TrackOnIndex(index).BodyRect = headerRect;

            Handles.BeginGUI();

            float padding = 1f;
            Handles.color = Color.gray;
            /*Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(headerRect.x, headerRect.y),
                new Vector3(headerRect.x + headerRect.width, headerRect.y),
                new Vector3(headerRect.x + headerRect.width, headerRect.y + headerRect.height),
                new Vector3(headerRect.x, headerRect.y + headerRect.height)});*/

            float multiplier = 1f;
            bool selected = false;
            if (TracksView.SelectedTrack == _timeline.Tracks.TrackOnIndex(index).TracksViewPosition)
            {
                multiplier = 1.1f;
                selected = true;
            }
            /*if (_highlightedVisiblePosition == _timeline.Tracks.TrackOnIndex(index).TracksViewPosition && _currentlyManipulatedTrack != _timeline.Tracks.TrackOnIndex(index))
            {
                multiplier *= 1.12f;
            }*/

            Handles.color = new Color(222f / 255f, 222f / 255f, 222f / 255f) * multiplier;
            /*Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(headerRect.x + padding, headerRect.y + padding * 0.5f),
                new Vector3(headerRect.x + headerRect.width - padding * 1f, headerRect.y + padding * 0.5f),
                new Vector3(headerRect.x + headerRect.width - padding * 1f, headerRect.y + headerRect.height - padding * 1f),
                new Vector3(headerRect.x + padding, headerRect.y + headerRect.height - padding * 1f)});*/

            Handles.EndGUI();

            if (selected)
            {
                GUI.DrawTexture(headerRect, _backgoundTrackSelectedGradient, ScaleMode.StretchToFill, true);
            }
            else
            {
                GUI.DrawTexture(headerRect, _backgoundTrackUnselectedGradient, ScaleMode.StretchToFill, true);
            }

            GUI.Label(new Rect(headerRect.x + 10, headerRect.y, headerRect.width - 20 - (_buttonsHeight + 4f) * 3f, _timeline.PixelPerTrack),
                "<size=16><color=#222244>" + _timeline.Tracks.TrackOnIndex(index).Name + "</color></size>",
                Utilities.RichClippedLeftCenterTextStyle
                );

            /*GUILayout.BeginArea(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 3f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight));
            if (GUILayout.Button(GUIContent.none, _timeline.Tracks.TrackOnIndex(index).Block ? _viewSkin.GetStyle("EyeButtonDisabledStyle") : _viewSkin.GetStyle("EyeButtonEnabledStyle"), new[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                Undo.RecordObject(_timeline, "Track's Block Setting Changed");
                _timeline.Tracks.TrackOnIndex(index).Block = !_timeline.Tracks.TrackOnIndex(index).Block;
            }
            GUILayout.EndArea();*/

            GUILayout.BeginArea(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 2f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight));

            GUIStyle eyeStyle = null;
            if (selected)
            {
                if (_timeline.Tracks.TrackOnIndex(index).Hided)
                {
                    eyeStyle = _viewSkin.GetStyle("EyeButtonDisabledSelectedStyle");
                }
                else
                {
                    eyeStyle = _viewSkin.GetStyle("EyeButtonEnabledSelectedStyle");
                }
            }
            else
            {
                if (_timeline.Tracks.TrackOnIndex(index).Hided)
                {
                    eyeStyle = _viewSkin.GetStyle("EyeButtonDisabledUnselectedStyle");
                }
                else
                {
                    eyeStyle = _viewSkin.GetStyle("EyeButtonEnabledUnselectedStyle");
                }
            }

            if (GUILayout.Button(_eyeGUIContent, eyeStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                Undo.RecordObject(_timeline, "Track's Hide Setting Changed");
                _timeline.Tracks.TrackOnIndex(index).Soloed = false;
                _timeline.Tracks.TrackOnIndex(index).Hided = !_timeline.Tracks.TrackOnIndex(index).Hided;
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 1f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight));

            GUIStyle soloStyle = null;
            if (selected)
            {
                if (_timeline.Tracks.TrackOnIndex(index).Soloed)
                {
                    soloStyle = _viewSkin.GetStyle("SoloButtonEnabledSelectedStyle");
                }
                else
                {
                    soloStyle = _viewSkin.GetStyle("SoloButtonDisabledSelectedStyle");
                }
            }
            else
            {
                if (_timeline.Tracks.TrackOnIndex(index).Soloed)
                {
                    soloStyle = _viewSkin.GetStyle("SoloButtonEnabledUnselectedStyle");
                }
                else
                {
                    soloStyle = _viewSkin.GetStyle("SoloButtonDisabledUnselectedStyle");
                }
            }

            if (GUILayout.Button(_dotGUIContent, soloStyle, new[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                Undo.RecordObject(_timeline, "Track's Solo Setting Changed");
                if (_timeline.Tracks.TrackOnIndex(index).Soloed == false)
                {
                    _timeline.Tracks.DeSoloAllTracks();
                }
                _timeline.Tracks.TrackOnIndex(index).Soloed = !_timeline.Tracks.TrackOnIndex(index).Soloed;

                if (_timeline.Tracks.TrackOnIndex(index).Soloed)
                {
                    _timeline.Tracks.TrackOnIndex(index).Hided = false;
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 3f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight));

            GUIStyle addKeyframeStyle = null;
            if (selected)
            {
                addKeyframeStyle = _viewSkin.GetStyle("NewKeyframeButtonSelectedStyle");
            }
            else
            {
                addKeyframeStyle = _viewSkin.GetStyle("NewKeyframeButtonUnselectedStyle");
            }

            if (GUILayout.Button(_newGUIContent, addKeyframeStyle, new[] { GUILayout.Width(_buttonsHeight), GUILayout.Height(_buttonsHeight) }))
            {
                _timeline.CreateNewKeyframeButtonPressed(_timeline.Tracks.TrackOnIndex(index));
            }

            GUILayout.EndArea();

           /* GUILayout.BeginArea(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 2f - (_keyframButtonHeight + 4f) - (_interpolationButtonHeight + 4f),
                headerRect.y + (headerRect.height - _interpolationButtonHeight) / 2f,
                _interpolationButtonHeight, _interpolationButtonHeight));

            if (GUILayout.Button(GUIContent.none, _timeline.Tracks.TrackOnIndex(index).InterpolationModeKeyframeAdding ? _viewSkin.GetStyle("InterpolationButtonActive") : _viewSkin.GetStyle("InterpolationButtonInactive"), new[] { GUILayout.Width(_interpolationButtonHeight), GUILayout.Height(_interpolationButtonHeight) }))
            {
                _timeline.Tracks.TrackOnIndex(index).InterpolationModeKeyframeAdding = !_timeline.Tracks.TrackOnIndex(index).InterpolationModeKeyframeAdding;
            }
            

            GUILayout.EndArea();*/

            /*GUI.DrawTexture(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 3f,
                headerRect.y + (headerRect.height - _buttonsHeight)/2f,
                _buttonsHeight, _buttonsHeight), _eyeEnabledTexture);

            GUI.DrawTexture(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 2f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight), _blockDisabledTexture);

            GUI.DrawTexture(new Rect(headerRect.x + headerRect.width - (_buttonsHeight + 4f) * 1f,
                headerRect.y + (headerRect.height - _buttonsHeight) / 2f,
                _buttonsHeight, _buttonsHeight), _soloDisabledTexture);*/
        }
        #endregion

        #region Utility Methods

        private void initializeField()
        {
            _field.x = 0;

            _tracksView.SetCurrentYOfInterpolator(_field.y);
        }

        private static void findTracksRange(float fieldY, float rectHeight, float trackHeight, out int startTrack, out int endTrack)
        {
            if (fieldY < 0f)
            {
                startTrack = (int)Mathf.Floor(Mathf.Abs(fieldY) / trackHeight);
                endTrack = startTrack + (int)Mathf.Ceil(rectHeight / trackHeight);
            }
            else
            {
                startTrack = 0;
                endTrack = (int)Mathf.Ceil(rectHeight / trackHeight);
            }
        }

        private void checkTracksBodiesCycle(Vector2 mousePosition, ref TimelineTrack target)
        {
            if (_timeline != null)
            {
                for (int i = 0; i < _timeline.Tracks.Count; i++)
                {
                    if (_timeline.Tracks.TrackOnIndex(i).BodyRect.Contains(mousePosition))
                    {
                        target = _timeline.Tracks.TrackOnIndex(i);
                        target.StartingManipulationMousePosition = mousePosition;
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
