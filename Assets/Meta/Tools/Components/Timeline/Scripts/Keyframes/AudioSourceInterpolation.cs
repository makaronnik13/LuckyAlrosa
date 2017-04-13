using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [InterpolationFor(typeof(AudioSourceKeyframe), new[] { "Add AudioSource/Keyframe" })]
    public class AudioSourceInterpolation : MetaInterpolationBase
    {
        public static float ContextMenuPriority = 7f;

        #region Public Fields
        public bool EnableVolume = true;
        public bool EnablePitch = true;

        public override Dictionary<string, Action> SubMenues
        {
            get
            {
                Dictionary<string, Action> result = new Dictionary<string, Action>();

                result.Add("Add AudioSource/Keyframe", () =>
                {
                    InitializeInterpolation();
                });

                return result;
            }
        }
        #endregion

        #region Public Properties
#if UNITY_EDITOR
        public override string ParametersWindowTitle
        {
            get
            {
                return "AudioSource Animation";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return 60f;
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
                AudioSourceKeyframe state0 = _keyframes[0] as AudioSourceKeyframe;
                AudioSourceKeyframe state1 = _keyframes[1] as AudioSourceKeyframe;
                int counter = 0;

                if (state0 != null && state1 != null)
                {
                    if (state0.EnableVolume && state1.EnableVolume && EnableVolume)
                    {
                        counter++;
                    }
                    if (state0.EnablePitch && state1.EnablePitch && EnablePitch)
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
        private void InitializeInterpolation()
        {
            var keyframe0 = (Keyframes[0] as AudioSourceKeyframe);
            var keyframe1 = (Keyframes[1] as AudioSourceKeyframe);

            keyframe0.Volume = keyframe1.Volume = 1;
            keyframe0.Pitch = keyframe1.Pitch = 1;
            keyframe0.ActionsIndexes[0] = 0;
            keyframe1.ActionsIndexes[0] = 1;
        }

        public override void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {
            base.Init(keyframe1, keyframe2, path);
            InitializeInterpolation();
        }

#if UNITY_EDITOR
        public override InterpolationThread[] ActiveTreads()
        {
            List<InterpolationThread> threads = new List<InterpolationThread>();

            AudioSourceKeyframe state0 = _keyframes[0] as AudioSourceKeyframe;
            AudioSourceKeyframe state1 = _keyframes[1] as AudioSourceKeyframe;

            if (state0 != null && state1 != null)
            {
                if (state0.EnableVolume && state1.EnableVolume && EnableVolume)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Volume";
                    thread.Color = Color.black * 0.5f;
                    threads.Add(thread);
                }

                if (state0.EnablePitch && state1.EnablePitch && EnablePitch)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Pitch";
                    thread.Color = Color.black * 0.5f;
                    threads.Add(thread);
                }
            }

            return threads.ToArray();
        }

        public override void DrawParametersWindow()
        {
            AudioSourceKeyframe state0 = _keyframes[0] as AudioSourceKeyframe;
            AudioSourceKeyframe state1 = _keyframes[1] as AudioSourceKeyframe;

            // Volume
            if (state0.EnableVolume && state1.EnableVolume)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<color=#eeeeee>Volume Enabled: </color>", Utilities.RichTextStyle, GUILayout.Width(164));
                EnableVolume = EditorGUILayout.Toggle(EnableVolume, GUILayout.Width(16));
                EditorGUILayout.EndHorizontal();
            }

            // Pitch
            if (state0.EnablePitch && state1.EnablePitch)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<color=#eeeeee>Pitch Enabled: </color>", Utilities.RichTextStyle, GUILayout.Width(164));
                EnablePitch = EditorGUILayout.Toggle(EnablePitch, GUILayout.Width(16));
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);

            AudioSourceKeyframe newMetaKeyframe = gameObject.AddComponent<AudioSourceKeyframe>();

            newMetaKeyframe.Target = Target;

            AudioSourceKeyframe state0 = _keyframes[_startIndex] as AudioSourceKeyframe;
            AudioSourceKeyframe state1 = _keyframes[_endIndex] as AudioSourceKeyframe;

            newMetaKeyframe.Timing = Mathf.Lerp(state0.Timing, state1.Timing, ratio);

            // AudioClip
            newMetaKeyframe.audioClip = state0.audioClip;

            // Volume
            if (state0.EnableVolume && state1.EnableVolume && EnableVolume)
            {
                newMetaKeyframe.EnableVolume = true;
                newMetaKeyframe.Volume = Mathf.Lerp(state0.Volume, state1.Volume, ratio);
            }
            else
            {
                newMetaKeyframe.EnableVolume = false;
            }

            // Mute
            newMetaKeyframe.Mute = state0.Mute;
            // Loop
            newMetaKeyframe.Loop = state0.Loop;

            // Pitch
            if (state0.EnablePitch && state1.EnablePitch && EnablePitch)
            {
                newMetaKeyframe.EnablePitch = true;
                newMetaKeyframe.Pitch = Mathf.Lerp(state0.Pitch, state1.Pitch, ratio);
            }
            else
            {
                newMetaKeyframe.EnablePitch = false;
            }

            return newMetaKeyframe;
        }
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            AudioSourceInterpolation clone = gameObject.AddComponent<AudioSourceInterpolation>();

            clone.EnableVolume = EnableVolume;
            clone.EnablePitch = EnablePitch;

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