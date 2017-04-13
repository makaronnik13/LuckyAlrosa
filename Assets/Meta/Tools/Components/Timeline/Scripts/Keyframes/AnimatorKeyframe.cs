using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    [KeyframeFor(typeof(Animator) /*, new[] { "Add Animation/AnimatorKeyframe" }*/)]
    public class AnimatorKeyframe : MetaKeyframeBase
    {
        #region Public Fields
        public KeyframeActions Actions
        {
            get
            {
                KeyframeActions actions = new KeyframeActions();
                actions.Add("Play", (object obj) =>
                {
                    if (EnableClip)
                    {
                        EditorAnimationPlayer.SampleClip(CurrentClip, Timing, Speed);
                    }
                }, (object obj) =>
                {
                    if (EnableClip)
                    {
                        EditorAnimationPlayer.SampleClip(CurrentClip, Timing, Speed);
                        EditorAnimationPlayer.RemoveClip(CurrentClip, true);
                    }
                });

                actions.Add("Stop", (object obj) =>
                {
                    CurrentClip.SampleClip(0, 0);
                });


                actions.Add("Pause", (object obj) =>
                {
                    EditorAnimationPlayer.PauseClip(CurrentClip, Timing, Speed);
                }, (object obj) =>
             {
                 EditorAnimationPlayer.RemoveClip(CurrentClip, false);
             });


                return actions;
            }
        }
        #endregion

        #region Public Properties

        public static float ContextMenuPriority = 3f;
        public List<int> ActionsIdexes = new List<int>();
        public TimeLineClip CurrentClip;
        public float Speed = 1;
        //enables
        public bool EnableAction = true;
        public bool EnableClip = true;
        public bool EnableSpeed = true;

#if UNITY_EDITOR
        public override string TargetsName
        {
            get
            {
                return "Animation";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "Animation State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return 95f;
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

        public int CurrentClipId
        {
            get
            {
                return _currentClipId;
            }

            set
            {
                _currentClipId = value;
                CurrentClip.Clip = Options[_currentClipId];
            }
        }

        public List<string> OptionsStrings
        {
            get
            {
                return _optionsStrings;
            }

            set
            {
                _optionsStrings = value;
            }
        }

        public List<AnimationClip> Options
        {
            get
            {
                return _options;
            }

            set
            {
                _options = value;
            }
        }
        #endregion

        #region Private Properties
        [SerializeField]
        private int _currentClipId = 0;

        [SerializeField]
        private List<AnimationClip> _options = new List<AnimationClip>();

        [SerializeField]
        private List<string> _optionsStrings = new List<string>();

        #endregion

        #region Main Methods

#if UNITY_EDITOR
        public override void Init(UnityEngine.Object target, string path)
        {

            _options.Clear();
            if ((target as Component).gameObject.GetComponent<Animation>())
            {
                (target as Component).gameObject.GetComponent<Animation>().playAutomatically = false;
            }

            if (!(target as Component).gameObject.GetComponent<Animator>())
            {
                target = (target as Component).gameObject.AddComponent<Animator>();
            }

            if (target.GetType() == typeof(Animation))
            {
                target = (target as Animation).GetComponent<Animator>();
            }

            UnityEditor.Animations.AnimatorController Controller = (target as Animator).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            //create RuntimeAnimatorController if it is not exists
            if (!Controller)
            {

                if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/AnimatorControllers/"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "AnimatorControllers");
                    }
                }

                string modelPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent((target as Animator).gameObject));
                var pos = modelPath.LastIndexOf('/');
                if (pos >= 0)
                {
                    modelPath = modelPath.Substring(0, pos);
                }

                AnimatorController animatorController;

                if (modelPath != "")
                {
                    animatorController = AnimatorController.CreateAnimatorControllerAtPath(modelPath + "/" + target.name + ".controller");
                }
                else
                {
                    animatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/AnimatorControllers/" + target.name + ".controller");
                }

                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent((target as Animator).gameObject)));
                UnityEditor.Animations.AnimatorStateMachine sm = animatorController.layers[0].stateMachine;

                foreach (UnityEngine.Object obj in objs)
                {
                    if (obj.GetType() == typeof(AnimationClip))
                    {
                        if (!obj.name.StartsWith("__preview__"))
                        {
                            UnityEditor.AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings((AnimationClip)obj);
                            settings.loopTime = true;
                            ((AnimationClip)obj).legacy = false;

                            AnimatorState newState = sm.AddState(obj.name);
                            newState.motion = (AnimationClip)obj;
                        }
                    }

                    if (obj.GetType() == typeof(Avatar))
                    {
                        (target as Animator).avatar = (Avatar)obj;
                    }
                }

                //set iddle state as default

                if (!objs.ToList().Find(ob => ob.name == "idle") || !objs.ToList().Find(ob => ob.name == "Idle"))
                {
                    sm.AddState("idle");
                }

                if (sm.states.ToList().Find(st => st.state.name == "Idle").state)
                {
                    sm.defaultState = sm.states.ToList().Find(st => st.state.name == "Idle").state;
                }
                else
                {
                    sm.defaultState = sm.states.ToList().Find(st => st.state.name == "idle").state;
                }
                (target as Animator).runtimeAnimatorController = animatorController;
            }


            //initiate keyframe
            Controller = (target as Animator).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            foreach (UnityEditor.Animations.ChildAnimatorState state in Controller.layers[0].stateMachine.states)
            {
                //add blend tree case
                bool isBlendTree = false;
                if (state.state.motion && state.state.motion.name == "Blend Tree")
                {
                    isBlendTree = true;
                }
                if (!isBlendTree)
                {
                    if ((AnimationClip)state.state.motion != null)
                    {
                        UnityEditor.AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings((AnimationClip)state.state.motion);
                        settings.loopTime = true;
                        ((AnimationClip)state.state.motion).legacy = false;
                        AnimationUtility.SetAnimationClipSettings((AnimationClip)state.state.motion, settings);
                    }
                    _options.Add((AnimationClip)state.state.motion);
                    _optionsStrings.Add(state.state.name);
                }
            }

            // CurrentState = Controller.layers[0].stateMachine.defaultState;


            // _currentStateName = CurrentState.name;

            if (ActionsIdexes.Count == 0)
            {
                ActionsIdexes.Add(0);
            }
            base.Init(target, path);

            CurrentClip = new TimeLineClip((Target as Animator).gameObject, _options[0], this);

            _keyframeColor = new Color(175f / 255f, 50f / 255f, 225f / 255f);

        }
#endif
        public override void Apply(bool forward = true)
        {
            if (_target == null)
            {
                return;
            }

            base.Apply();

            Animator Animator = _target as Animator;
            if (Animator.runtimeAnimatorController == null)
            {
                return;
            }

            Animator.enabled = false;

            //performing actions
            if (EnableAction)
            {
                //if (FindObjectOfType<Timeline>().Playing)
                //{
                for (int i = 0; i < ActionsIdexes.Count; i++)
                {
                    if (forward)
                    {
                        Actions.PerformForward(ActionsIdexes[i], Target);
                    }
                    else
                    {
                        Actions.PerformBackward(ActionsIdexes[i], Target);
                    }
                }
                //}
            }
        }

        public override void Fetch()
        {
            base.Fetch();
            Animator Animator = _target as Animator;
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            //actions
            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableAction = EditorGUILayout.Toggle(EnableAction, GUILayout.Width(16));
            EditorGUILayout.LabelField("<size=10>Action: </size>", Utilities.RichTextStyle, GUILayout.Width(50));
            int newActionIndex = EditorGUILayout.Popup(ActionsIdexes[0], Actions.GetNames(), GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
            if (ActionsIdexes[0] != newActionIndex)
            {
                ActionsIdexes[0] = newActionIndex;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableClip = EditorGUILayout.Toggle(EnableClip, GUILayout.Width(16));
            CurrentClipId = EditorGUILayout.Popup(CurrentClipId, OptionsStrings.ToArray(), GUILayout.ExpandWidth(true), GUILayout.Width(185));



            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableSpeed = EditorGUILayout.Toggle(EnableSpeed, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Speed: </color>", Utilities.RichTextStyle, GUILayout.Width(50));
            Speed = EditorGUILayout.FloatField(Speed, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10>Loop: </size>", Utilities.RichTextStyle, GUILayout.Width(50));
            CurrentClip.LoopClip = EditorGUILayout.Toggle(CurrentClip.LoopClip, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            AnimatorKeyframe clone = gameObject.AddComponent<AnimatorKeyframe>();
            clone.EnableAction = EnableAction;
            clone.EnableClip = EnableClip;
            clone.EnableSpeed = EnableSpeed;
            clone.CurrentClip = CurrentClip;
            clone.Options = Options;
            clone.CurrentClipId = CurrentClipId;
            clone.Speed = Speed;
            for (int i = 0; i < ActionsIdexes.Count; i++)
            {
                clone.ActionsIdexes.Add(ActionsIdexes[i]);
            }

#if UNITY_EDITOR
            clone._keyframeColor = _keyframeColor;
            clone._caption = _caption;
#endif
            clone.Init(_target, "");
            return clone;
        }
        #endregion
    }
}