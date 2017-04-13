using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Meta.Tools
{
    [InterpolationFor(typeof(TransitionKeyframe), new[] { "Add Transition/Keyframe", "Add Transition/Fade/In", "Add Transition/Fade/Out" })]
    public class TransitionInterpolation : MetaInterpolationBase
    {
        public static float ContextMenuPriority = 7f;

        #region Public Fields
        public bool EnableOpacity = true;
        #endregion

        #region Private Fields
        private static float _height = 45f;
        #endregion

        #region Public Properties

        public enum Fade
        {
            Out = 0,
            In
        }

        public override Dictionary<string, Action> SubMenues
        {
            get
            {
                Dictionary<string, Action> result = new Dictionary<string, Action>();

                result.Add("Add Transition/Fade/Out", () =>
                {
                    InitFade(Fade.Out);
                });
                result.Add("Add Transition/Fade/In", () =>
                {
                    InitFade(Fade.In);
                });

                return result;
            }
        }
#if UNITY_EDITOR
        public override string ParametersWindowTitle
        {
            get
            {
                return "Transition Animation";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return _height;
            }
        }

        public override float ParametersWindowWidth
        {
            get
            {
                return 200f;
            }
        }

        public override int ActiveThreadsNumber
        {
            get
            {
                TransitionKeyframe state0 = _keyframes[0] as TransitionKeyframe;
                TransitionKeyframe state1 = _keyframes[1] as TransitionKeyframe;
                int counter = 0;

                if (state0 != null && state1 != null)
                {
                    if (state0.EnableOpacity && state1.EnableOpacity && EnableOpacity)
                    {
                        counter++;
                    }
                }

                return counter;
            }
        }
#endif
        #endregion

        #region Main Methods

        private void InitFade(Fade fadeMode)
        {
            var keyframe0 = (Keyframes[0] as TransitionKeyframe);
            var keyframe1 = (Keyframes[1] as TransitionKeyframe);

            if (fadeMode == Fade.Out)
            {
                keyframe0.Opacity = 1;
                keyframe1.Opacity = 0;
            }
            else if (fadeMode == Fade.In)
            {
                keyframe0.Opacity = 0;
                keyframe1.Opacity = 1;
            }
        }

        public override void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {
            base.Init(keyframe1, keyframe2, path);
        }

#if UNITY_EDITOR
        public override InterpolationThread[] ActiveTreads()
        {
            List<InterpolationThread> threads = new List<InterpolationThread>();

            TransitionKeyframe state0 = _keyframes[0] as TransitionKeyframe;
            TransitionKeyframe state1 = _keyframes[1] as TransitionKeyframe;

            if (state0 != null && state1 != null)
            {
                if (state0.EnableOpacity && state1.EnableOpacity && EnableOpacity)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Enable Opacity";
                    thread.Color = Color.blue * 0.5f;
                    threads.Add(thread);
                }
            }

            return threads.ToArray();
        }

        public override void DrawParametersWindow()
        {
            TransitionKeyframe state0 = _keyframes[0] as TransitionKeyframe;
            TransitionKeyframe state1 = _keyframes[1] as TransitionKeyframe;

            // Opacity
            if (state0.EnableOpacity && state1.EnableOpacity)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<color=#eeeeee>Opacity Enabled: </color>", Utilities.RichTextStyle, GUILayout.Width(164));
                EnableOpacity = EditorGUILayout.Toggle(EnableOpacity, GUILayout.Width(16));
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);

            TransitionKeyframe newMetaKeyframe = gameObject.AddComponent<TransitionKeyframe>();

            newMetaKeyframe.Target = Target;

            TransitionKeyframe state0 = _keyframes[_startIndex] as TransitionKeyframe;
            TransitionKeyframe state1 = _keyframes[_endIndex] as TransitionKeyframe;

            newMetaKeyframe.Timing = Mathf.Lerp(state0.Timing, state1.Timing, ratio);

            // Opacity
            if (state0.EnableOpacity && state1.EnableOpacity)
            {
                newMetaKeyframe.EnableOpacity = true;
                newMetaKeyframe.renderersList = state0.renderersList;

                newMetaKeyframe.Opacity = Mathf.Lerp(
                    state0.Opacity,
                    state1.Opacity,
                    ratio);
            }
            else
            {
                newMetaKeyframe.EnableOpacity = false;
            }

            return newMetaKeyframe;
        }
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            TransitionInterpolation clone = gameObject.AddComponent<TransitionInterpolation>();

            clone.EnableOpacity = EnableOpacity;

#if UNITY_EDITOR
            clone._interpolationBackgroundColor = _interpolationBackgroundColor;
            clone._caption = _caption;
#endif

            clone.Init(_keyframes[0].Clone() as MetaKeyframeBase, _keyframes[1].Clone() as MetaKeyframeBase, "");

            return clone;
        }
        #endregion
    }
}
