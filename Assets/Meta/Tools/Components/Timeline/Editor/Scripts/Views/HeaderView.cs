using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class HeaderView : TimelineViewBase
    {
        /*private GUIContent _playButtonContent = new GUIContent();
        private GUIContent _pauseButtonContent = new GUIContent();
        private GUIContent _stopButtonContent = new GUIContent();

        private GUIContent _nextChapterButtonContent = new GUIContent();
        private GUIContent _previousChapterButtonContent = new GUIContent();*/

        private GUIContent _setKeyframeContent = new GUIContent();

        private Texture _logoTexture;

        private bool initialized = false;

        #region Constructor
        public HeaderView()
        {
            _logoTexture = (Texture)Resources.Load("Textures/NewDesign/Meta_Presenter");
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            if (!initialized)
            {
                initialize();
            }

            drawLogo();
            drawProperties();
            drawControls();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
        }

        private void initialize()
        {
            /*_playButtonContent.image = (Texture2D)Resources.Load("Textures/meta-tools-play-button");
            _playButtonContent.tooltip = "Start playing";
            _pauseButtonContent.image = (Texture2D)Resources.Load("Textures/meta-tools-pause-button");
            _pauseButtonContent.tooltip = "Pause playing";
            _stopButtonContent.image = (Texture2D)Resources.Load("Textures/meta-tools-stop-button");
            _stopButtonContent.tooltip = "Stop playing";

            _nextChapterButtonContent.image = (Texture2D)Resources.Load("Textures/black-white-android-2");
            _nextChapterButtonContent.tooltip = "To the next chapter";
            _previousChapterButtonContent.image = (Texture2D)Resources.Load("Textures/black-white-android-1");
            _previousChapterButtonContent.tooltip = "To the previous chapter";*/

            /*_setKeyframeContent.image = (Texture2D)Resources.Load("Textures/arrowdot2");
            _setKeyframeContent.tooltip = "Take state snapshot/Add new keyframe";*/

            initialized = true;
        }
        #endregion

        #region Utility Methods
        private void drawProperties()
        {
            /*GUILayout.BeginArea(new Rect(0, 0, 300f, _height));

            GUILayout.BeginHorizontal();

            GUIContent durationContent = new GUIContent();
            durationContent.text = "<color=#cccccc><size=14>Duration of timeline:</size></color>";
            GUILayout.Label(durationContent, Utilities.RichTextStyle, GUILayout.Width(142));
            _serializedObject.FindProperty("_duration").floatValue = (float)Convert.ToDouble(GUILayout.TextField(_serializedObject.FindProperty("_duration").floatValue.ToString(), GUILayout.Width(50)));
            GUILayout.Label("<color=#cccccc><size=14>(secs)</size></color>", Utilities.RichTextStyle, GUILayout.Width(40));

            GUILayout.EndHorizontal();

            GUILayout.EndArea();*/
        }

        private void drawLogo()
        {
            //GUI.DrawTexture(new Rect(0, 0, 232f, 83f), _logoTexture);
            GUI.DrawTexture(new Rect(98 + 10, 38, 137f, 49f), _logoTexture);
            /*GUILayout.BeginArea(new Rect(ViewRect.x + ViewRect.width - 210f, ViewRect.y, 210f, 50f));

            GUILayout.Label("<i><b><size=24><color=#ffffff>Presenter Tool</color></size></b></i>", Utilities.RichTextStyle);

            GUILayout.EndArea();*/
        }

        private void drawControls()
        {
            /*Rect playbackTimeArea = new Rect(_width / 2f - 70f, _height - 48f, 140f, 24f);
            //drawing time
            GUILayout.BeginArea(playbackTimeArea);

            GUILayout.BeginHorizontal();

            GUILayout.Label("<color=#cccccc><size=14>Time</size></color>", Utilities.RichTextStyle, GUILayout.Width(40));
            GUILayout.TextField(((int)Mathf.Floor(_timeline.CurrentTime / 60f)).ToString(), GUILayout.Width(24));
            GUILayout.Label("<color=#cccccc><size=14>:</size></color>", Utilities.RichTextStyle, GUILayout.Width(4));
            GUILayout.TextField(((int)Mathf.Floor(_timeline.CurrentTime % 60f)).ToString(), GUILayout.Width(24));
            GUILayout.Label("<color=#cccccc><size=14>:</size></color>", Utilities.RichTextStyle, GUILayout.Width(4));
            GUILayout.TextField(((int)Mathf.Floor((_timeline.CurrentTime - Mathf.Floor(_timeline.CurrentTime)) * 100f)).ToString(), GUILayout.Width(24));

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            //drawing buttons
            Rect buttonsArea = new Rect(_width / 2f - 208f, _height - 32f, 416f, 32f);
            GUIStyle buttonsAreaStyle = new GUIStyle();
            buttonsAreaStyle.alignment = TextAnchor.LowerCenter;
            GUILayout.BeginArea(buttonsArea);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_stopButtonContent, _viewSkin.GetStyle("ToStartButtonStyle"), new[] { GUILayout.Width(28), GUILayout.Height(22) }))
            {
                _timeline.GoToTiming(0f);
                _timelineWindow.NavigateToTiming(0f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_stopButtonContent, _viewSkin.GetStyle("RewindButtonStyle"), new[] { GUILayout.Width(36), GUILayout.Height(22) }))
            {
                _timeline.GoToTiming(_timeline.CurrentTime - 0.1f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_stopButtonContent, _viewSkin.GetStyle("StopButtonStyle"), new[] { GUILayout.Width(24), GUILayout.Height(24) }))
            {
                _timelineWindow.Stop();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(GUIContent.none, _timeline.Playing ? _viewSkin.GetStyle("PauseButtonStyle") : _viewSkin.GetStyle("PlayButtonStyle"), new[] { GUILayout.Width(32), GUILayout.Height(32) }))
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

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_stopButtonContent, _viewSkin.GetStyle("FastForwardButtonStyle"), new[] { GUILayout.Width(36), GUILayout.Height(22) }))
            {
                _timeline.GoToTiming(_timeline.CurrentTime + 0.1f);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_stopButtonContent, _viewSkin.GetStyle("ToEndButtonStyle"), new[] { GUILayout.Width(28), GUILayout.Height(22) }))
            {
                _timeline.GoToTiming(_timeline.Duration);
                _timelineWindow.NavigateToTiming(_timeline.Duration);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (!Tracks.ProprietaryMode)
            {
                //other functions buttons right
                Rect otherFunctionsButtonsRightArea = new Rect(buttonsArea.x + buttonsArea.width + 24f, _height - 32f, 120f, 32f);
                GUILayout.BeginArea(otherFunctionsButtonsRightArea);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(_setKeyframeContent, new[] { GUILayout.Width(32f), GUILayout.Height(32f) }))
                {
                    _timelineWindow.OnSetKeyframeButtonClicked();
                }
                ModelExploder related = ModelExploder.ModelExploderRelated;
                if (related != null)
                {
                    if (GUILayout.Button("M", new[] { GUILayout.Width(32f), GUILayout.Height(32f) }))
                    {
                        _timelineWindow.OnSaveExplosionStateButtonClicked(related);
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }*/

            //secondary settings area
            /*Rect secondarySettingsArea = new Rect(_width - 128f, _height - 128f, 128f, 128f);
            GUILayout.BeginArea(secondarySettingsArea);

            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Snapping: ", _viewSkin.GetStyle("SecondarySettingsAreaStyle"), GUILayout.Width(70f));
            TimelineWindow.SnappingOn = EditorGUILayout.Toggle(TimelineWindow.SnappingOn);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndArea();*/
        }
        #endregion
    }
}
