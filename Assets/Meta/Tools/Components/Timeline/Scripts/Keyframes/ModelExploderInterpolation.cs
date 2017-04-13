using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [InterpolationFor(typeof(ModelExploderKeyframe), new[] { "Add ModelExploder/Keyframe", "Add ModelExploder/Radial Explosion", "Add ModelExploder/Sphere Explosion", "Add ModelExploder/Axis-Wise Explosion" })]
    public class ModelExploderInterpolation : MetaInterpolationBase
    {
        public static float ContextMenuPriority = 2.1f;

        #region Public Fields
        public bool EnableInputValue = true;
        public bool EnableExplosionSettingsBundleIndex = true;
        #endregion

        #region Private Fields
        private float _height = 20f;
        #endregion

        #region Public Properties
        public override Dictionary<string, Action> SubMenues
        {
            get
            {
                Dictionary<string, Action> result = new Dictionary<string, Action>();

                result.Add("Add ModelExploder/Keyframe", () =>
                {
                    //MakeAnimation();
                });
                result.Add("Add ModelExploder/Radial Explosion", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.Radial);
                    //MakeAnimation();
                });
                result.Add("Add ModelExploder/Sphere Explosion", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.Sphere);
                    //MakeAnimation();
                });
                result.Add("Add ModelExploder/Axis-Wise Explosion", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.AxisWise);
                    //MakeAnimation();
                });

                return result;
            }
        }

#if UNITY_EDITOR

        public override string ParametersWindowTitle
        {
            get
            {
                return "ModelExploder Animation";
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
                return 240f;
            }
        }

        public override int ActiveThreadsNumber
        {
            get
            {
                ModelExploderKeyframe state0 = _keyframes[0] as ModelExploderKeyframe;
                ModelExploderKeyframe state1 = _keyframes[1] as ModelExploderKeyframe;
                int counter = 0;
                
                if (state0.EnableExplosionSettingsBundleIndex && state1.EnableExplosionSettingsBundleIndex && EnableExplosionSettingsBundleIndex)
                {
                    counter++;
                }

                return counter;
            }
        }
#endif
        #endregion

        #region Main Methods

#if UNITY_EDITOR
        public override InterpolationThread[] ActiveTreads()
        {
            List<InterpolationThread> threads = new List<InterpolationThread>();

            ModelExploderKeyframe state0 = _keyframes[0] as ModelExploderKeyframe;
            ModelExploderKeyframe state1 = _keyframes[1] as ModelExploderKeyframe;

            if (state0 != null && state1 != null)
            {
                if (state0.EnableExplosionSettingsBundleIndex && state1.EnableExplosionSettingsBundleIndex && EnableExplosionSettingsBundleIndex)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "State Index";
                    thread.Color = Color.black * 1.0f;
                    threads.Add(thread);
                }
            }

            return threads.ToArray();
        }
#endif

        public override void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {
            base.Init(keyframe1, keyframe2, path);
        }

        private void InitializeWithExplosionType(ExplosionRule rule)
        {
            ModelExploder modelExploder = Target as ModelExploder;
            modelExploder.Initialize();
            modelExploder.CurrentFields().ExplosionRule = rule;
            if (rule == ExplosionRule.Sphere)
            {
                modelExploder.ExplosionSettingsBundles[1].DefaultSettings.Distance *= 2f;
            }
            
            modelExploder.UpdateInitialTransforms(1);
            modelExploder.RecalculateAllExplosionTargets(0, 1);
            modelExploder.Save(1, 0);
            //(Target as ModelExploder).RecalculateAllExplosionTargets();

            (Keyframes[0] as ModelExploderKeyframe).ExplosionSettingsBundleIndex = 0;
            (Keyframes[1] as ModelExploderKeyframe).ExplosionSettingsBundleIndex = modelExploder.ExplosionSettingsBundles.Count - 1;
        }

        /*private void MakeAnimation()
        {
            (Keyframes[0] as ModelExploderKeyframe).InputValue = 0f;
            (Keyframes[1] as ModelExploderKeyframe).InputValue = 1f;
        }*/

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            ModelExploderKeyframe modelExploderState0 = _keyframes[0] as ModelExploderKeyframe;
            ModelExploderKeyframe modelExploderState1 = _keyframes[1] as ModelExploderKeyframe;

            _height = 20f;
            /*if (modelExploderState0.EnableInputValue && modelExploderState1.EnableInputValue)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Input Value Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(164));
                EnableInputValue = EditorGUILayout.Toggle(EnableInputValue, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }*/
            if (modelExploderState0.EnableExplosionSettingsBundleIndex && modelExploderState1.EnableExplosionSettingsBundleIndex)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>State: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(164));
                EnableExplosionSettingsBundleIndex = EditorGUILayout.Toggle(EnableExplosionSettingsBundleIndex, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);

            ModelExploderKeyframe newMetaKeyframe = gameObject.AddComponent<ModelExploderKeyframe>();

            newMetaKeyframe.Target = Target;

            ModelExploderKeyframe state0 = _keyframes[_startIndex] as ModelExploderKeyframe;
            ModelExploderKeyframe state1 = _keyframes[_endIndex] as ModelExploderKeyframe;

            newMetaKeyframe.Timing = Mathf.Lerp(state0.Timing, state1.Timing, ratio);

            /*if (state0.EnableInputValue && state1.EnableInputValue && EnableInputValue)
            {
                newMetaKeyframe.EnableInputValue = true;
                newMetaKeyframe.InputValue = Mathf.Lerp(state0.InputValue, state1.InputValue, ratio);
            }
            else
            {
                newMetaKeyframe.EnableInputValue = false;
            }*/

            if (state0.EnableExplosionSettingsBundleIndex && state1.EnableExplosionSettingsBundleIndex && EnableExplosionSettingsBundleIndex)
            {
                newMetaKeyframe.EnableExplosionSettingsBundleIndex = true;
                //newMetaKeyframe.ExplosionSettingsBundleIndex = (int)Mathf.Lerp(state0.ExplosionSettingsBundleIndex, state1.ExplosionSettingsBundleIndex, ratio);

                newMetaKeyframe.Interpolated = true;
                newMetaKeyframe.State0 = state0.ExplosionSettingsBundleIndex;
                newMetaKeyframe.State1 = state1.ExplosionSettingsBundleIndex;
                newMetaKeyframe.Ratio = ratio;
            }
            else
            {
                newMetaKeyframe.EnableExplosionSettingsBundleIndex = false;
            }

            return newMetaKeyframe;
        }
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            ModelExploderInterpolation clone = gameObject.AddComponent<ModelExploderInterpolation>();

            clone.EnableInputValue = EnableInputValue;
            clone.EnableExplosionSettingsBundleIndex = EnableExplosionSettingsBundleIndex;

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
