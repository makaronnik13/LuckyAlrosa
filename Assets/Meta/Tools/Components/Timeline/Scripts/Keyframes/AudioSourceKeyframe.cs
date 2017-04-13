using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Meta.Tools
{
    [KeyframeFor(typeof(AudioSource))]
    public class AudioSourceKeyframe : MetaKeyframeBase
    {
        public static float ContextMenuPriority = 7f;

        #region Public Fields
        public KeyframeActions Actions
        {
            get
            {
                KeyframeActions actions = new KeyframeActions();
                actions.Add("Play", (object obj) =>
                {
                    (obj as AudioSource).Play();
                }, (object obj) =>
                {
                    (obj as AudioSource).Stop();
                });
                actions.Add("Stop", (object obj) =>
                {
                    (obj as AudioSource).Stop();
                }, (object obj) =>
                {
                    (obj as AudioSource).Play();
                });
                actions.Add("Pause", (object obj) =>
                {
                    (obj as AudioSource).Pause();
                }, (object obj) =>
                {
                    (obj as AudioSource).Pause();
                });

                return actions;
            }
        }
        public List<int> ActionsIndexes = new List<int>();

        public bool EnableAction = true;
        public bool EnableVolume = true;
        public bool EnableMute = true;
        public bool EnableLoop = true;
        public bool EnablePitch = true;

        public AudioClip audioClip;
        public float Volume = 1f;
        public bool Mute = false;
        public bool Loop = false;
        public float Pitch = 1f;
        #endregion

        #region Private Fields
        private static float _height = 75f;
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
                if (value is AudioSource)
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
                return "Audio";
            }
        }

        public override string ParametersWindowTitle
        {
            get
            {
                return "AudioSource State";
            }
        }

        public override float ParametersWindowHeight
        {
            get
            {
                return base.ParametersWindowHeight + _height;
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

            (target as AudioSource).playOnAwake = false;

            if (ActionsIndexes.Count == 0)
            {
                ActionsIndexes.Add(0);
            }

#if UNITY_EDITOR
            _keyframeColor = new Color(252f / 255f, 157f / 255f, 2f / 255f);
#endif
        }

        public override void Apply(bool forward = true)
        {
            if (Target == null)
            {
                return;
            }

            AudioSource audioSource = Target as AudioSource;

            //audioSource.clip = audioClip;

            audioSource.volume = EnableVolume ?
                Volume : audioSource.volume;

            audioSource.mute = EnableMute ?
                Mute : audioSource.mute;

            audioSource.loop = EnableLoop ?
                Loop : audioSource.loop;

            audioSource.pitch = EnablePitch ?
                Pitch : audioSource.pitch;

            if (EnableAction)
            {
                for (int i = 0; i < ActionsIndexes.Count; i++)
                {
                    if (forward)
                    {
                        Actions.PerformForward(ActionsIndexes[i], Target);
                    }
                    else
                    {
                        Actions.PerformBackward(ActionsIndexes[i], Target);
                    }
                }
            }

            base.Apply();
        }

        public override void Fetch()
        {
            base.Fetch();

            if (Target != null)
            {
                AudioSource audioSource = Target as AudioSource;

                audioClip = audioSource.clip;
                Volume = audioSource.volume;
                Mute = audioSource.mute;
                Loop = audioSource.loop;
                Pitch = audioSource.pitch;
            }
            else
            {
                Debug.Log("Target == null!");
            }
        }

#if UNITY_EDITOR
        public override void DrawParametersWindow()
        {
            // AudioClip
            /*EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<color=#eeeeee>AudioClip: </color>", Utilities.RichTextStyle, GUILayout.Width(70));
            audioClip = (AudioClip)EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true);
            string commandName = Event.current.commandName;

            if (commandName == "ObjectSelectorClosed")
            {
                audioClip = (AudioClip)(EditorGUIUtility.GetObjectPickerObject());
            }
            EditorGUILayout.EndHorizontal();*/

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableAction = EditorGUILayout.Toggle(EnableAction, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Action: </color>", Utilities.RichTextStyle, GUILayout.Width(50));
            int newActionIndex = EditorGUILayout.Popup(ActionsIndexes[0], Actions.GetNames(), GUILayout.Width(150));

            if (ActionsIndexes[0] != newActionIndex)
            {
                ActionsIndexes[0] = newActionIndex;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            // Volume
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableVolume = EditorGUILayout.Toggle(EnableVolume, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Volume: </color>", Utilities.RichTextStyle, GUILayout.Width(50));
            Volume = EditorGUILayout.Slider(Volume, 0f, 1f);
            EditorGUILayout.EndHorizontal();

            // Mute
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableMute = EditorGUILayout.Toggle(EnableMute, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Mute: </color>", Utilities.RichTextStyle, GUILayout.Width(40));
            Mute = EditorGUILayout.Toggle(Mute, GUILayout.Width(16));
            EditorGUILayout.EndHorizontal();

            // Loop
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnableLoop = EditorGUILayout.Toggle(EnableLoop, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Loop: </color>", Utilities.RichTextStyle, GUILayout.Width(40));
            Loop = EditorGUILayout.Toggle(Loop, GUILayout.Width(16));
            EditorGUILayout.EndHorizontal();

            // Pitch
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=10><color=#eeeeee>Enabled: </color></size>", Utilities.RichTextStyle, GUILayout.Width(50));
            EnablePitch = EditorGUILayout.Toggle(EnablePitch, GUILayout.Width(16));
            EditorGUILayout.LabelField("<color=#eeeeee>Pitch: </color>", Utilities.RichTextStyle, GUILayout.Width(50));
            Pitch = EditorGUILayout.Slider(Pitch, -3f, 3f);
            EditorGUILayout.EndHorizontal();

            //base.DrawParametersWindow();
        }
#endif
        #endregion

        #region Interface Implementations

        public override object Clone()
        {
            AudioSourceKeyframe clone = gameObject.AddComponent<AudioSourceKeyframe>();

            clone.Target = Target;

            clone.EnableAction = EnableAction;
            clone.EnableVolume = EnableVolume;
            clone.EnableMute = EnableMute;
            clone.EnableLoop = EnableLoop;
            clone.EnablePitch = EnablePitch;

            clone.audioClip = audioClip;
            clone.Volume = Volume;
            clone.Mute = Mute;
            clone.Loop = Loop;
            clone.Pitch = Pitch;

            for (int i = 0; i < ActionsIndexes.Count; i++)
            {
                clone.ActionsIndexes.Add(ActionsIndexes[i]);
            }

#if UNITY_EDITOR
            clone._keyframeColor = _keyframeColor;
            clone._caption = _caption;
#endif

            clone.Timing = Timing;
            clone.hideFlags = hideFlags;
            clone.Init(Target, "");

            return clone;
        }
        #endregion
    }
}