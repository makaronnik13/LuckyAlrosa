using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [InterpolationFor(typeof(AnimatorKeyframe))] //, new[] { "Animation/Interpolation"}
    public class AnimatorInterpolation : MetaInterpolationBase
    {
        #region Public Fields
        public bool EnableSpeed = true;
        public float Speed;
        #endregion

        public static float ContextMenuPriority = 4f;
        #region Public Properties
#if UNITY_EDITOR
        public static string[] AdditionalMenuItems()
        {
            List<string> avaliableAnimations = new List<string>();
            if (Selection.activeGameObject != null)
            {
                //get list of animationClips from controller
                if (Selection.activeGameObject.GetComponent<Animator>() && Selection.activeGameObject.GetComponent<Animator>().runtimeAnimatorController)
                {
                    foreach (UnityEditor.Animations.ChildAnimatorState state in (Selection.activeGameObject.GetComponent<Animator>().runtimeAnimatorController as UnityEditor.Animations.AnimatorController).layers[0].stateMachine.states)
                    {
                        //add blend tree case
                        bool isBlendTree = false;

                        if (state.state.motion && state.state.motion.name == "Blend Tree")
                        {
                            isBlendTree = true;
                        }
                        if (!isBlendTree)
                        {
                            avaliableAnimations.Add("Add Animation/" + state.state.name);
                        }
                    }
                }
                else
                {

                    if (PrefabUtility.GetPrefabParent(Selection.activeGameObject))
                    {
                        UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(Selection.activeGameObject)));
                        foreach (UnityEngine.Object obj in objs)
                        {
                            if (obj.GetType() == typeof(AnimationClip))
                            {
                                if (!obj.name.StartsWith("__preview__"))
                                {
                                    avaliableAnimations.Add("Add Animation/" + obj.name);
                                }
                            }
                        }
                    }
                    else
                    {
                        Animation anim = Selection.activeGameObject.GetComponent<Animation>();
                        //get list of animationClips from Animation component
                        if (anim)
                        {

                            //List<AnimationState> states = new List<AnimationState>(anim.Cast<AnimationState>());
                            /*for (int i = 0;i<anim.GetClipCount();i++)
                            {
                                Debug.Log(states[i]);
                                avaliableAnimations.Add("Add Animation/" + states[i].clip.name);
                            }*/
                            foreach (AnimationState state in anim)
                            {
                                avaliableAnimations.Add("Add Animation/" + state.clip.name);
                            }
                        }
                    }
                }
            }
            if (avaliableAnimations.Count() == 0)
            {
                avaliableAnimations.Add("Add Animation/Create animation");
            }
            return avaliableAnimations.ToArray();
        }

#endif


#if UNITY_EDITOR
        public override string ParametersWindowTitle
        {
            get
            {
                return "Animation";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return 55f;
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
                AnimatorKeyframe state0 = _keyframes[0] as AnimatorKeyframe;
                AnimatorKeyframe state1 = _keyframes[1] as AnimatorKeyframe;
                int counter = 0;

                if (state0.EnableSpeed && state1.EnableSpeed && EnableSpeed)
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
        public override void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {

            base.Init(keyframe1, keyframe2, path);
            //set animator state if it is in path
            string newPath = "";
            var p = path.LastIndexOf('/');
            if (p >= 0)
            {
                newPath = path.Substring(p + 1, path.Length - p - 1);
            }

            if (newPath != "Create animation" && path != "")
            {
                (keyframe2 as AnimatorKeyframe).CurrentClipId = (keyframe2 as AnimatorKeyframe).Options.IndexOf((keyframe2 as AnimatorKeyframe).Options.Find(o => o.name == newPath));
                (keyframe1 as AnimatorKeyframe).CurrentClipId = (keyframe1 as AnimatorKeyframe).Options.IndexOf((keyframe1 as AnimatorKeyframe).Options.Find(o => o.name == newPath));
            }
            (keyframe1 as AnimatorKeyframe).ActionsIdexes[0] = 0;
            (keyframe2 as AnimatorKeyframe).ActionsIdexes[0] = 1;
            SetClipSize();
        }
#endif
        /*
        private void CreateInterpolation()
        {
            Debug.Log("interpolation");
            Debug.Log((Keyframes[0] as AnimatorKeyframe).CurrentState);
            Debug.Log((Keyframes[1] as AnimatorKeyframe).CurrentState);
            (Keyframes[0] as AnimatorKeyframe).EnableAction = true;
            (Keyframes[1] as AnimatorKeyframe).EnableAction = true;
            (Keyframes[0] as AnimatorKeyframe).EnableClip = true;
            (Keyframes[1] as AnimatorKeyframe).EnableClip = true;
            (Keyframes[0] as AnimatorKeyframe).ActionsIdexes[0] = 0;
            (Keyframes[0] as AnimatorKeyframe).ActionsIdexes[0] = 1;  
        }
        */

#if UNITY_EDITOR
        public override InterpolationThread[] ActiveTreads()
        {
            List<InterpolationThread> threads = new List<InterpolationThread>();

            AnimatorKeyframe state0 = _keyframes[0] as AnimatorKeyframe;
            AnimatorKeyframe state1 = _keyframes[1] as AnimatorKeyframe;

            if (state0 != null && state1 != null)
            {
                if (state0.EnableSpeed && state1.EnableSpeed && EnableSpeed)
                {
                    InterpolationThread thread = new InterpolationThread();
                    thread.Name = "Animator speed";
                    thread.Color = new Color(175f / 255f, 50f / 255f, 225f / 255f);
                    threads.Add(thread);
                }
            }

            return threads.ToArray();
        }

        public void SetClipSize()
        {
            if ((Keyframes[0] as AnimatorKeyframe).CurrentClip.Clip != null)
            {
                Keyframes[1].Timing = Keyframes[0].Timing + (Keyframes[0] as AnimatorKeyframe).CurrentClip.Clip.length;
            }
        }

        public override void DrawParametersWindow()
        {
            AnimatorKeyframe animatorState0 = _keyframes[0] as AnimatorKeyframe;
            AnimatorKeyframe animatorState1 = _keyframes[1] as AnimatorKeyframe;

            if (animatorState0.EnableSpeed && animatorState1.EnableSpeed)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<color=#eeeeee>Speed Enabled: </color>", Utilities.RichTextStyle, GUILayout.Width(164));
                EnableSpeed = EditorGUILayout.Toggle(EnableSpeed, GUILayout.Width(16));
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
        public override MetaKeyframeBase Interpolate(float ratio)
        {
            base.Interpolate(ratio);
            AnimatorKeyframe newMetaKeyframe = gameObject.AddComponent<AnimatorKeyframe>();
            newMetaKeyframe.Target = Target;
            AnimatorKeyframe state0 = _keyframes[_startIndex] as AnimatorKeyframe;
            AnimatorKeyframe state1 = _keyframes[_endIndex] as AnimatorKeyframe;
            newMetaKeyframe.CurrentClip = state0.CurrentClip;
            newMetaKeyframe.Options = state0.Options;
            if (newMetaKeyframe.ActionsIdexes.Count == 0)
            {
                newMetaKeyframe.ActionsIdexes.Add(0);
            }

            if (state0.ActionsIdexes[0] == 2)
            {
                newMetaKeyframe.EnableAction = false;
            }

            newMetaKeyframe.ActionsIdexes[0] = state0.ActionsIdexes[0];

            newMetaKeyframe.CurrentClipId = state0.CurrentClipId;

            newMetaKeyframe.Timing = Mathf.Lerp(state0.Timing, state1.Timing, ratio);
            if (state0.EnableSpeed && state1.EnableSpeed && EnableSpeed)
            {
                newMetaKeyframe.EnableSpeed = true;
                newMetaKeyframe.Speed = Mathf.Lerp(state0.Speed, state1.Speed, ratio);
            }
            else
            {
                newMetaKeyframe.Speed = state0.Speed;
            }


            return newMetaKeyframe;
        }
        #endregion

        #region Interfaces Implementations
        public override object Clone()
        {
            AnimatorInterpolation clone = gameObject.AddComponent<AnimatorInterpolation>();
            clone.EnableSpeed = EnableSpeed;
            clone.Speed = Speed;

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
