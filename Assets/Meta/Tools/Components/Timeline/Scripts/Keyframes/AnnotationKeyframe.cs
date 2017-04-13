using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [KeyframeFor(typeof(AnnotationComponent)/*, new[] { "Add Annotation/Show", "Add Annotation/Hide" }*/)]
    public class AnnotationKeyframe : MetaKeyframeBase
    {
        public static float ContextMenuPriority = 3f;

        #region Public Fields
        public KeyframeActions Actions
        {
            get
            {
                KeyframeActions actions = new KeyframeActions();
                actions.Add("Show", (object obj) =>
                {
                    (obj as AnnotationComponent).Show();
                }, (object obj) =>
                {
                    (obj as AnnotationComponent).Hide();
                });
                actions.Add("Hide", (object obj) =>
                {
                    (obj as AnnotationComponent).Hide();
                }, (object obj) =>
                {
                    (obj as AnnotationComponent).Show();
                });

                return actions;
            }
        }
        #endregion

        public int ActionIndex = 0;
        public bool EnableAction = true;

        public string Content = "Example Content";
        public bool EnableContent = true;

        #region Public Properties
        public override Object Target
        {
            get
            {
                return base.Target;
            }

            set
            {
                if (value is AnnotationComponent)
                {
                    base.Target = value;
                }
            }
        }

#if UNITY_EDITOR
        public override string TargetsName
        {
            get
            {
                return "Annotation";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "Annotation State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return base.ParametersWindowHeight + 120f;
            }
        }

        public override float ParametersWindowWidth
        {
            get
            {
                return 300f;
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

        #region MonoBehaviour Lifecycle
        protected override void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Main Methods
        public override void Init(UnityEngine.Object target, string path)
        {
            base.Init(target, path);

            if (path.Contains("Hide"))
            {
                ActionIndex = 1;
            }
            else
            {
                ActionIndex = 0;
            }

            (target as AnnotationComponent).Initialize();

#if UNITY_EDITOR
            _keyframeColor = new Color(17f / 255f, 189f / 255f, 207f / 255f);
#endif
        }

        public override void Apply(bool forward = true)
        {
            if (_target == null)
            {
                return;
            }

            AnnotationComponent annotationComponent = _target as AnnotationComponent;

            if (EnableAction)
            {
                if (forward)
                {
                    Actions.PerformForward(ActionIndex, Target);
                }
                else
                {
                    Actions.PerformBackward(ActionIndex, Target);
                }
            }

            /*if (EnableContent)
            {
                annotationComponent.SetContent(Content);
            }*/

            base.Apply();
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableAction = EditorGUILayout.Toggle(EnableAction, GUILayout.Width(16));
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Action: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            int newActionIndex = EditorGUILayout.Popup(ActionIndex, Actions.GetNames(), GUILayout.Width(150));

            if (ActionIndex != newActionIndex)
            {
                ActionIndex = newActionIndex;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Add text/web URL here: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EditorGUILayout.BeginHorizontal();

            //EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            //EnableContent = EditorGUILayout.Toggle(EnableContent, GUILayout.Width(16));
            //Content = EditorGUILayout.TextArea(Content, new[] { GUILayout.Width(266f), GUILayout.Height(80f) });

            SerializedObject serializedAnnotation = new SerializedObject((_target as AnnotationComponent).GetAnnotation());
            string newContent = EditorGUILayout.TextArea((serializedAnnotation.FindProperty("_text").objectReferenceValue as InputField).text, new[] { GUILayout.Width(266f), GUILayout.Height(80f) });
            if (newContent != (serializedAnnotation.FindProperty("_text").objectReferenceValue as InputField).text)
            {
                Undo.RecordObject(_target, "Annotation Text Changed");
                (serializedAnnotation.FindProperty("_text").objectReferenceValue as InputField).text = newContent;
                serializedAnnotation.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            AnnotationKeyframe clone = gameObject.AddComponent<AnnotationKeyframe>();

            clone.Target = Target;
            clone.EnableAction = EnableAction;
            clone.ActionIndex = ActionIndex;
            clone.EnableContent = EnableContent;
            clone.Content = Content;

#if UNITY_EDITOR
            clone._keyframeColor = _keyframeColor;
            clone._caption = _caption;
#endif

            clone.Timing = Timing;
            clone.hideFlags = hideFlags;
            clone.Init(_target, "");

            return clone;
        }
        #endregion
    }
}
