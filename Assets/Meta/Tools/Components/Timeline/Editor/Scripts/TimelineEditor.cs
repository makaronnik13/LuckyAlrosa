using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    [CustomEditor(typeof(Timeline))]
    public class TimelineEditor : UnityEditor.Editor
    {
        private Timeline _timeline;
        private Texture2D _keysTexture;
        private GUISkin _guiSkin;

        private void OnEnable()
        {
            _timeline = target as Timeline;
            _keysTexture = (Texture2D)Resources.Load("Textures/keys");
            _guiSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/TimelineEditorSkin");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("<i><b>    Summary:</b></i>", Utilities.TitleStyle);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            GUIContent numberOfTracks = new GUIContent();
            numberOfTracks.text = "Number of tracks: <b>" + _timeline.Tracks.Count + "</b>";
            EditorGUILayout.LabelField(numberOfTracks, Utilities.RichTextStyle);

            GUIContent numberOfKeyframes = new GUIContent();
            numberOfKeyframes.text = "Number of keyframes: <b>" + _timeline.Tracks.NumberOfKeyframes + "</b>";
            EditorGUILayout.LabelField(numberOfKeyframes, Utilities.RichTextStyle);

            GUIContent numberOfAnimations = new GUIContent();
            numberOfAnimations.text = "Number of animations: <b>" + _timeline.Tracks.NumberOfInterpolations + "</b>";
            EditorGUILayout.LabelField(numberOfAnimations, Utilities.RichTextStyle);

            GUIContent numberOfChapters = new GUIContent();
            numberOfChapters.text = "Number of chapters: <b>" + _timeline.Keynotes.Count + "</b>";
            EditorGUILayout.LabelField(numberOfChapters, Utilities.RichTextStyle);

            GUIContent durationOfAnimation = new GUIContent();
            durationOfAnimation.text = "Duration of timeline: <b>" + _timeline.Duration + " sec.</b>";
            EditorGUILayout.LabelField(durationOfAnimation, Utilities.RichTextStyle);

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

            GUIContent instructions = new GUIContent();
            instructions.text = "<size=16><b> Key bindings: </b></size>";
            EditorGUILayout.LabelField(instructions, Utilities.RichTextStyle);
            
            GUILayout.Box(_keysTexture, _guiSkin.GetStyle("TimelineKeysStyle"));

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            GUIContent timelineLabelContent = new GUIContent();
            timelineLabelContent.text = "<b><i><size=18>  Timeline </size></i></b>";
            timelineLabelContent.tooltip = "Manages timeline.";

            Timeline.AnimationUpdateMode newUpdateMode = (Timeline.AnimationUpdateMode)EditorGUILayout.EnumPopup("Update event: ", _timeline.UpdateMode);
            if (newUpdateMode != _timeline.UpdateMode)
            {
                Undo.RecordObject(target, "Update Event Changed");
                _timeline.UpdateMode = newUpdateMode;
            }

            bool newPlayOnAwake = EditorGUILayout.Toggle("Play On Awake:", _timeline.PlayOnAwake);
            if (newPlayOnAwake != _timeline.PlayOnAwake)
            {
                Undo.RecordObject(target, "Play On Awake Setting Changed");
                _timeline.PlayOnAwake = newPlayOnAwake;
            }

            TimelineWindow.SnappingOn = EditorGUILayout.Toggle("Snapping:", TimelineWindow.SnappingOn);


            if (GUILayout.Button("Open Timeline Window"))
            {
                EditorWindow.GetWindow(typeof(TimelineWindow));
                TimelineWindow.Init();
            }
        }
    }
}