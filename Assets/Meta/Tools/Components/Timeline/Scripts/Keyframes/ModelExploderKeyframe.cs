using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [KeyframeFor(typeof(ModelExploder)/*, new[] { "Add ModelExploder/Keyframe" }*/)]
    public class ModelExploderKeyframe : MetaKeyframeBase
    {
        public static float ContextMenuPriority = 2f;

#if UNITY_EDITOR
        public static string[] AdditionalMenuItems()
        {
            List<string> avaliableStates = new List<string>();

            if (Tracks.ProprietaryMode)
            {
                if (Tracks.RowMouseAbove >= 0)
                {
                    TimelineTrack track = Timeline.Instance.Tracks.GetTrackOnTracksViewPosition(Tracks.RowMouseAbove);
                    if (track != null && track.Target is ModelExploder)
                    {
                        ModelExploder modelExploder = track.Target as ModelExploder;
                        for (int i = 0; i < modelExploder.ExplosionSettingsBundles.Count; i++)
                        {
                            if (i == 1)
                            {
                                continue;
                            }
                            avaliableStates.Add("Add ModelExploder/Availible States/" + modelExploder.ExplosionSettingsBundles[i].Name);
                        }
                    }
                }
            }
            else
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<ModelExploder>())
                {
                    for (int i = 0; i < Selection.activeGameObject.GetComponent<ModelExploder>().ExplosionSettingsBundles.Count; i++)
                    {
                        if (i == 1)
                        {
                            continue;
                        }
                        avaliableStates.Add("Add ModelExploder/Availible States/" + Selection.activeGameObject.GetComponent<ModelExploder>().ExplosionSettingsBundles[i].Name);
                    }
                }
            }

            return avaliableStates.ToArray();
        }
#endif

        #region Public Fields
        public bool Interpolated = false;
        public int State0;
        public int State1;
        public float Ratio;

        public int ExplosionSettingsBundleIndex = 0;
        
        public bool EnableExplosionSettingsBundleIndex = true;
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
                if (value is ModelExploder)
                {
                    base.Target = value;
                }
            }
        }

        public override Dictionary<string, Action> SubMenues
        {
            get
            {
                Dictionary<string, Action> result = new Dictionary<string, Action>();

                result.Add("ModelExploder/Radial Explosion Keyframe", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.Radial);
                });
                result.Add("ModelExploder/Sphere Explosion Keyframe", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.Sphere);
                });
                result.Add("ModelExploder/Axis-Wise Explosion Keyframe", () =>
                {
                    InitializeWithExplosionType(ExplosionRule.AxisWise);
                });

                return result;
            }
        }

#if UNITY_EDITOR
        public override string TargetsName
        {
            get
            {
                return "Model Exploder";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "ModelExploder State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return base.ParametersWindowHeight + 20f;
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
                return 50;
            }
        }
        #endregion

        #region Main Methods
        private void InitializeWithExplosionType(ExplosionRule rule)
        {
            (Target as ModelExploder).Initialize();
            (Target as ModelExploder).CurrentFields().ExplosionRule = rule;
            //(Target as ModelExploder).RecalculateAllExplosionTargets();
        }

        public override void Init(UnityEngine.Object target, string path)
        {
            base.Init(target, path);

            if (path != "")
            {
                ModelExploder modelExploder = _target as ModelExploder;

                for (int i = 0; i < modelExploder.ExplosionSettingsBundles.Count; i++)
                {
                    if (path.Contains(modelExploder.ExplosionSettingsBundles[i].Name))
                    {
                        ExplosionSettingsBundleIndex = i;

                        break;
                    }
                }
            }

#if UNITY_EDITOR
            _keyframeColor = new Color(125f / 255f, 125f / 255f, 255f / 255f);
#endif
        }

        public override void Apply(bool forward = true)
        {
            if (_target == null)
            {
                return;
            }

            ModelExploder modelExploder = _target as ModelExploder;

            /*if (EnableExplosionSettingsBundleIndex)
            {
                modelExploder.CurrentExplosionSettingsBundle = ExplosionSettingsBundleIndex;
            }*/

            if (Interpolated)
            {
                modelExploder.Explode(State0, State1, Ratio);
            }
            else
            {
                modelExploder.Explode(ExplosionSettingsBundleIndex, 0, 0f);
            }

            base.Apply();
        }

        public override void Fetch()
        {
            base.Fetch();

            ModelExploder modelExploder = _target as ModelExploder;
            
            ExplosionSettingsBundleIndex = modelExploder.FinalStateIndex;
        }

#if UNITY_EDITOR
        public override void OnSelected()
        {
            base.OnSelected();

            ModelExploder modelExploder = _target as ModelExploder;
            if (ExplosionSettingsBundleIndex < modelExploder.ExplosionSettingsBundles.Count)
            {
                modelExploder.SelectedStateIndex = ExplosionSettingsBundleIndex;

                int initial = 0;

                for (int i = 0; i < modelExploder.ExplosionSettingsBundles.Count; i++)
                {
                    if (modelExploder.ExplosionSettingsBundles[i].ID == modelExploder.ExplosionSettingsBundles[ExplosionSettingsBundleIndex].sourceID)
                    {
                        initial = i;
                        break;
                    }
                }

                modelExploder.InitialStateIndex = initial;
            }
        }

        public override void DrawParametersWindow()
        {
            /*EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            EnableInputValue = EditorGUILayout.Toggle(EnableInputValue, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Explosion point: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(150));
            InputValue = EditorGUILayout.FloatField(GUIContent.none, InputValue, GUILayout.Width(54));

            EditorGUILayout.EndHorizontal();*/
            EditorGUILayout.BeginHorizontal();

            List<string> availableStatesNames = (Target as ModelExploder).ExplosionSettingsBundlesNames;
            List<int> availableStatesIndexes = new List<int>();
            availableStatesIndexes.Add(0);
            int j = 2;
            for (int i = 0; i < availableStatesNames.Count; i++)
            {
                availableStatesIndexes.Add(j);
                j++;
            }
            availableStatesNames.Insert(0, "Default");

            EditorGUILayout.LabelField("<size=10>Enabled: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            EnableExplosionSettingsBundleIndex = EditorGUILayout.Toggle(EnableExplosionSettingsBundleIndex, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>State: </color>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            ExplosionSettingsBundleIndex = EditorGUILayout.IntPopup(ExplosionSettingsBundleIndex, availableStatesNames.ToArray(), availableStatesIndexes.ToArray(), GUILayout.Width(154));

            EditorGUILayout.EndHorizontal();

            //base.DrawParametersWindow();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            ModelExploderKeyframe clone = gameObject.AddComponent<ModelExploderKeyframe>();
            clone.Init(_target, "");

            //clone.EnableInputValue = EnableInputValue;
            clone.EnableExplosionSettingsBundleIndex = EnableExplosionSettingsBundleIndex;
            //clone.InputValue = InputValue;
            clone.ExplosionSettingsBundleIndex = ExplosionSettingsBundleIndex;

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
