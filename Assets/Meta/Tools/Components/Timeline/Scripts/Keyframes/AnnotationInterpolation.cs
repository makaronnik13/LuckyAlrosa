using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [InterpolationFor(typeof(AnnotationKeyframe), new[] { "Add Annotation/Keyframe"/*, "AudioSource/SoundUp", "AudioSource/SoundDown", "AudioSource/PitchUp", "AudioSource/PitchDown"*/ })]
    public class AnnotationInterpolation : MetaInterpolationBase
    {
        public static float ContextMenuPriority = 3f;

        #region Public Fields
        #endregion

        #region Public Properties
#if UNITY_EDITOR
        public override string ParametersWindowTitle
        {
            get
            {
                return "Annotation Animation";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return 40f;
            }
        }

        public override float ParametersWindowWidth
        {
            get
            {
                return 240f;
            }
        }

        public override int ActiveThreadsNumber
        {
            get
            {
                return 0;
            }
        }
#endif
        #endregion

        #region Main Methods
        public override void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {
            base.Init(keyframe1, keyframe2, path);

            var key0 = (Keyframes[0] as AnnotationKeyframe);
            var key1 = (Keyframes[1] as AnnotationKeyframe);

            key0.ActionIndex = 0;
            key1.ActionIndex = 1;
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            EditorGUILayout.LabelField("<color=#eeeeee>No interpolated arguments availible</color>", Utilities.RichTextStyle);
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);

            return null;
        }
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            AnnotationInterpolation clone = gameObject.AddComponent<AnnotationInterpolation>();

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