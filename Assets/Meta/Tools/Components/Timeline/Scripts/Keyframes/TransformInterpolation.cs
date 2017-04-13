using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [InterpolationFor(typeof(TransformKeyframe), new[] { "Add Transform/Keyframe", "Add Transform/90\u00B0/x", "Add Transform/90\u00B0/y", "Add Transform/90\u00B0/z", "Add Transform/180\u00B0/x", "Add Transform/180\u00B0/y", "Add Transform/180\u00B0/z", "Add Transform/360\u00B0/x", "Add Transform/360\u00B0/y", "Add Transform/360\u00B0/z" })]
    public class TransformInterpolation : MetaInterpolationBase
    {
        public static float ContextMenuPriority = 1f;

        #region Public Fields
        public bool EulerInterpolation = false;

        public bool EnableLocalPosition = true;
        public bool EnableLocalRotation = true;
        public bool EnableLocalScale = true;
        public bool EnableGlobalPosition = true;
        public bool EnableGlobalRotation = true;
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

                result.Add("Add Transform/90\u00B0/x", () =>
                {
                    InitializeRotationBy(90f, Axis.X);
                });
                result.Add("Add Transform/90\u00B0/y", () =>
                {
                    InitializeRotationBy(90f, Axis.Y);
                });
                result.Add("Add Transform/90\u00B0/z", () =>
                {
                    InitializeRotationBy(90f, Axis.Z);
                });

                result.Add("Add Transform/180\u00B0/x", () =>
                {
                    InitializeRotationBy(180f, Axis.X);
                });
                result.Add("Add Transform/180\u00B0/y", () =>
                {
                    InitializeRotationBy(180f, Axis.Y);
                });
                result.Add("Add Transform/180\u00B0/z", () =>
                {
                    InitializeRotationBy(180f, Axis.Z);
                });

                result.Add("Add Transform/360\u00B0/x", () =>
                {
                    InitializeRotationBy(360f, Axis.X);
                });
                result.Add("Add Transform/360\u00B0/y", () =>
                {
                    InitializeRotationBy(360f, Axis.Y);
                });
                result.Add("Add Transform/360\u00B0/z", () =>
                {
                    InitializeRotationBy(360f, Axis.Z);
                });

                return result;
            }
        }

#if UNITY_EDITOR

        public override string ParametersWindowTitle
        {
            get
            {
                return "Transform Animation";
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
                TransformKeyframe state0 = _keyframes[0] as TransformKeyframe;
                TransformKeyframe state1 = _keyframes[1] as TransformKeyframe;
                int counter = 0;

                if (state0 != null && state1 != null)
                {
                    if (state0.EnableGlobalPosition && state1.EnableGlobalPosition && EnableGlobalPosition)
                    {
                        counter++;
                    }
                    if (state0.EnableGlobalRotation && state1.EnableGlobalRotation && EnableGlobalRotation)
                    {
                        counter++;
                    }
                    if (state0.EnableLocalPosition && state1.EnableLocalPosition && EnableLocalPosition)
                    {
                        counter++;
                    }
                    if (state0.EnableLocalRotation && state1.EnableLocalRotation && EnableLocalRotation)
                    {
                        counter++;
                    }
                    if (state0.EnableLocalScale && state1.EnableLocalScale && EnableLocalScale)
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
        private void InitializeRotationBy(float angle, Axis axis)
        {
            (Keyframes[0] as TransformKeyframe).EnableLocalRotation = true;
            (Keyframes[1] as TransformKeyframe).EnableLocalRotation = true;
            (Keyframes[0] as TransformKeyframe).EnableGlobalRotation = false;
            (Keyframes[1] as TransformKeyframe).EnableGlobalRotation = false;

            (Keyframes[0] as TransformKeyframe).EnableGlobalPosition = false;
            (Keyframes[1] as TransformKeyframe).EnableGlobalPosition = false;
            (Keyframes[0] as TransformKeyframe).EnableLocalPosition = true;
            (Keyframes[1] as TransformKeyframe).EnableLocalPosition = true;
            (Keyframes[0] as TransformKeyframe).EnableLocalScale = true;
            (Keyframes[1] as TransformKeyframe).EnableLocalScale = true;

            switch (axis)
            {
                case Axis.X:
                    (Keyframes[1] as TransformKeyframe).LocalRotation.x = (Keyframes[0] as TransformKeyframe).LocalRotation.x + angle;
                    break;
                case Axis.Y:
                    (Keyframes[1] as TransformKeyframe).LocalRotation.y = (Keyframes[0] as TransformKeyframe).LocalRotation.y + angle;
                    break;
                case Axis.Z:
                    (Keyframes[1] as TransformKeyframe).LocalRotation.z = (Keyframes[0] as TransformKeyframe).LocalRotation.z + angle;
                    break;
            }

            if (angle >= 180f)
            {
                EulerInterpolation = true;
            }
            else
            {
                EulerInterpolation = false;
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

            TransformKeyframe state0 = _keyframes[0] as TransformKeyframe;
            TransformKeyframe state1 = _keyframes[1] as TransformKeyframe;

            if (state0 != null && state1 != null)
            {
                if (state0.EnableGlobalPosition && state1.EnableGlobalPosition && EnableGlobalPosition)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Position";
                    thread.Color = Color.blue * 0.5f;
                    threads.Add(thread);
                }
                if (state0.EnableGlobalRotation && state1.EnableGlobalRotation && EnableGlobalRotation)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Rotation";
                    thread.Color = Color.red * 0.5f;
                    threads.Add(thread);
                }
                if (state0.EnableLocalPosition && state1.EnableLocalPosition && EnableLocalPosition)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Position";
                    thread.Color = Color.blue * 1f;
                    threads.Add(thread);
                }
                if (state0.EnableLocalRotation && state1.EnableLocalRotation && EnableLocalRotation)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Rotation";
                    thread.Color = Color.red * 1f;
                    threads.Add(thread);
                }
                if (state0.EnableLocalScale && state1.EnableLocalScale && EnableLocalScale)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Scale";
                    thread.Color = Color.green * 1f;
                    threads.Add(thread);
                }
            }

            return threads.ToArray();
        }

        public override void DrawParametersWindow()
        {
            TransformKeyframe state0 = _keyframes[0] as TransformKeyframe;
            TransformKeyframe state1 = _keyframes[1] as TransformKeyframe;

            _height = 20f;
            if (state0.EnableLocalPosition && state1.EnableLocalPosition)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Local Position Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(204));
                EnableLocalPosition = EditorGUILayout.Toggle(EnableLocalPosition, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
            if (state0.EnableLocalRotation && state1.EnableLocalRotation)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Local Rotation Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(204));
                EnableLocalRotation = EditorGUILayout.Toggle(EnableLocalRotation, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
            if (state0.EnableLocalScale && state1.EnableLocalScale)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Local Scale Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(204));
                EnableLocalScale = EditorGUILayout.Toggle(EnableLocalScale, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
            if (state0.EnableGlobalPosition && state1.EnableGlobalPosition)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Global Position Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(204));
                EnableGlobalPosition = EditorGUILayout.Toggle(EnableGlobalPosition, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
            if (state0.EnableGlobalRotation && state1.EnableGlobalRotation)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Global Rotation Enabled: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(204));
                EnableGlobalRotation = EditorGUILayout.Toggle(EnableGlobalRotation, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
            if ((state0.EnableGlobalRotation && state1.EnableGlobalRotation) ||
                (state0.EnableLocalRotation && state1.EnableLocalRotation))
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("<color=#eeeeee>Euler Angles Interpolation: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(164));
                EulerInterpolation = EditorGUILayout.Toggle(EulerInterpolation, GUILayout.Width(16));

                EditorGUILayout.EndHorizontal();
                _height += 20f;
            }
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);

            TransformKeyframe newMetaKeyframe = gameObject.AddComponent<TransformKeyframe>();

            newMetaKeyframe.Target = Target;

            TransformKeyframe state0 = _keyframes[_startIndex] as TransformKeyframe;
            TransformKeyframe state1 = _keyframes[_endIndex] as TransformKeyframe;

            newMetaKeyframe.Timing = Mathf.Lerp(state0.Timing, state1.Timing, ratio);

            if (state0.EnableGlobalPosition && state1.EnableGlobalPosition && EnableGlobalPosition/* && state0.GlobalPosition != state1.GlobalPosition*/)
            {
                newMetaKeyframe.EnableGlobalPosition = true;
                newMetaKeyframe.GlobalPosition = Vector3.Lerp(state0.GlobalPosition, state1.GlobalPosition, ratio);
            }
            else
            {
                newMetaKeyframe.EnableGlobalPosition = false;
            }

            if (state0.EnableGlobalRotation && state1.EnableGlobalRotation && EnableGlobalRotation/* && state0.GlobalRotation != state1.GlobalRotation*/)
            {
                newMetaKeyframe.EnableGlobalRotation = true;
                if (EulerInterpolation)
                {
                    newMetaKeyframe.GlobalRotation = Vector3.Lerp(state0.GlobalRotation, state1.GlobalRotation, ratio);
                }
                else
                {
                    newMetaKeyframe.GlobalRotation = Quaternion.Lerp(Quaternion.Euler(state0.GlobalRotation), Quaternion.Euler(state1.GlobalRotation), ratio).eulerAngles;
                }
            }
            else
            {
                newMetaKeyframe.EnableGlobalRotation = false;
            }

            if (state0.EnableLocalPosition && state1.EnableLocalPosition && EnableLocalPosition/* && state0.LocalPosition != state1.LocalPosition*/)
            {
                newMetaKeyframe.EnableLocalPosition = true;
                newMetaKeyframe.LocalPosition = Vector3.Lerp(state0.LocalPosition, state1.LocalPosition, ratio);
            }
            else
            {
                newMetaKeyframe.EnableLocalPosition = false;
            }

            if (state0.EnableLocalRotation && state1.EnableLocalRotation && EnableLocalRotation/* && state0.LocalRotation != state1.LocalRotation*/)
            {
                newMetaKeyframe.EnableLocalRotation = true;
                if (EulerInterpolation)
                {
                    newMetaKeyframe.LocalRotation = Vector3.Lerp(state0.LocalRotation, state1.LocalRotation, ratio);
                }
                else
                {
                    newMetaKeyframe.LocalRotation = Quaternion.Lerp(Quaternion.Euler(state0.LocalRotation), Quaternion.Euler(state1.LocalRotation), ratio).eulerAngles;
                }
            }
            else
            {
                newMetaKeyframe.EnableLocalRotation = false;
            }

            if (state0.EnableLocalScale && state1.EnableLocalScale && EnableLocalScale/* && state0.LocalScale != state1.LocalScale*/)
            {
                newMetaKeyframe.EnableLocalScale = true;
                newMetaKeyframe.LocalScale = Vector3.Lerp(state0.LocalScale, state1.LocalScale, ratio);
            }
            else
            {
                newMetaKeyframe.EnableLocalScale = false;
            }

            return newMetaKeyframe;
        }
        #endregion

        #region Utility Methods
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            TransformInterpolation clone = gameObject.AddComponent<TransformInterpolation>();

            clone.EnableGlobalPosition = EnableGlobalPosition;
            clone.EnableGlobalRotation = EnableGlobalRotation;
            clone.EnableLocalPosition = EnableLocalPosition;
            clone.EnableLocalRotation = EnableLocalRotation;
            clone.EnableLocalScale = EnableLocalScale;

            clone.EulerInterpolation = EulerInterpolation;

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
