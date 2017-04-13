using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [KeyframeFor(typeof(Transform)/*, new[] { "Transform/Keyframe" }*/)]
    public class TransformKeyframe : MetaKeyframeBase
    {
        public static float ContextMenuPriority = 1f;

        #region Public Fields
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public Vector3 LocalScale;
        public Vector3 GlobalPosition;
        public Vector3 GlobalRotation;

        public bool EnableLocalPosition = true;
        public bool EnableLocalRotation = true;
        public bool EnableLocalScale = true;
        public bool EnableGlobalPosition = false;
        public bool EnableGlobalRotation = false;
        #endregion

        #region Private Fields
        /*private Axis _currentAxisOfRotation = Axis.Y;
        private float _targetAngle;
        private float _currentAngle;
        private float _rotationSpeed = 150f;*/
        #endregion

        #region Public Properties
        public override UnityEngine.Object Target
        {
            get
            {
                return base.Target;
            }

            set
            {
                if (value is Transform)
                {
                    base.Target = value;
                }
                else if (value is GameObject)
                {
                    base.Target = (value as GameObject).transform;
                }
            }
        }

#if UNITY_EDITOR
        public override string TargetsName
        {
            get
            {
                return "Transform";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "Transform State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return base.ParametersWindowHeight + 100f;
            }
        }

        public override float ParametersWindowWidth
        {
            get
            {
                return 340f;
            }
        }
#endif

        public override int AnimationQueuePriority
        {
            get
            {
                return 100;
            }
        }
        #endregion

        #region Main Methods
        public override void Init(UnityEngine.Object target, string path)
        {
            base.Init(target, path);

#if UNITY_EDITOR
            _keyframeColor = new Color(125f / 255f, 255f / 255f, 125f / 255f);
#endif
        }

        public override void Apply(bool forward = true)
        {
            if (_target == null)
            {
                return;
            }

            Transform transform = _target as Transform;

            if (EnableGlobalPosition)
            {
                transform.position = GlobalPosition;
            }
            if (EnableGlobalRotation)
            {
                transform.rotation = Quaternion.Euler(GlobalRotation);
            }
            if (EnableLocalPosition)
            {
                transform.localPosition = LocalPosition;
            }
            if (EnableLocalRotation)
            {
                transform.localRotation = Quaternion.Euler(LocalRotation);
            }
            if (EnableLocalScale)
            {
                transform.localScale = LocalScale;
            }

            base.Apply();
        }

        public override void Fetch()
        {
            base.Fetch();

            Transform transform = _target as Transform;

            LocalPosition = transform.localPosition;
            if (Quaternion.Euler(LocalRotation).eulerAngles != transform.localRotation.eulerAngles)
            {
                LocalRotation = transform.localRotation.eulerAngles;
            }
            LocalScale = transform.localScale;
            GlobalPosition = transform.position;
            if (Quaternion.Euler(GlobalRotation).eulerAngles != transform.rotation.eulerAngles)
            {
                GlobalRotation = transform.rotation.eulerAngles;
            }
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            bool newEnableLocalPosition = EditorGUILayout.Toggle(EnableLocalPosition, GUILayout.Width(16));
            if (newEnableLocalPosition != EnableLocalPosition)
            {
                EnableLocalPosition = newEnableLocalPosition;
                if (EnableLocalPosition)
                {
                    EnableGlobalPosition = false;
                }
            }
            EditorGUILayout.LabelField("<color=#eeeeee>Local Position: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(90));
            LocalPosition = EditorGUILayout.Vector3Field(GUIContent.none, LocalPosition, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            bool newEnableLocalRotation = EditorGUILayout.Toggle(EnableLocalRotation, GUILayout.Width(16));
            if (newEnableLocalRotation != EnableLocalRotation)
            {
                EnableLocalRotation = newEnableLocalRotation;
                if (EnableLocalRotation)
                {
                    EnableGlobalRotation = false;
                }
            }
            EditorGUILayout.LabelField("<color=#eeeeee>Local Rotation: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(90));
            LocalRotation = EditorGUILayout.Vector3Field(GUIContent.none, LocalRotation, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            EnableLocalScale = EditorGUILayout.Toggle(EnableLocalScale, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Local Scale: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(90));
            LocalScale = EditorGUILayout.Vector3Field(GUIContent.none, LocalScale, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            bool newEnableGlobalPosition = EditorGUILayout.Toggle(EnableGlobalPosition, GUILayout.Width(16));
            if (newEnableGlobalPosition != EnableGlobalPosition)
            {
                EnableGlobalPosition = newEnableGlobalPosition;
                if (EnableGlobalPosition)
                {
                    EnableLocalPosition = false;
                }
            }
            EditorGUILayout.LabelField("<color=#eeeeee>Global Position: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(90));
            GlobalPosition = EditorGUILayout.Vector3Field(GUIContent.none, GlobalPosition, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            bool newEnableGlobalRotation = EditorGUILayout.Toggle(EnableGlobalRotation, GUILayout.Width(16));
            if (newEnableGlobalRotation != EnableGlobalRotation)
            {
                EnableGlobalRotation = newEnableGlobalRotation;
                if (EnableGlobalRotation)
                {
                    EnableLocalRotation = false;
                }
            }
            EditorGUILayout.LabelField("<color=#eeeeee>Global Rotation: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(90));
            GlobalRotation = EditorGUILayout.Vector3Field(GUIContent.none, GlobalRotation, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();

            base.DrawParametersWindow();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            TransformKeyframe clone = gameObject.AddComponent<TransformKeyframe>();

            clone.Init(_target, "");

            clone.LocalPosition = LocalPosition;
            clone.LocalRotation = LocalRotation;
            clone.LocalScale = LocalScale;
            clone.GlobalPosition = GlobalPosition;
            clone.GlobalRotation = GlobalRotation;

            clone.EnableGlobalPosition = EnableGlobalPosition;
            clone.EnableGlobalRotation = EnableGlobalRotation;
            clone.EnableLocalPosition = EnableLocalPosition;
            clone.EnableLocalRotation = EnableLocalRotation;
            clone.EnableLocalScale = EnableLocalScale;

#if UNITY_EDITOR
            clone._keyframeColor = _keyframeColor;
            clone._caption = _caption;
#endif

            clone.Timing = Timing;
            clone.hideFlags = hideFlags;

            return clone;
        }
        #endregion
    }
}