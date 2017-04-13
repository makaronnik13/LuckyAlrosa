using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class Timeline : MonoBehaviour
    {
        public static bool HideComponentsMode = true;

        #region Singleton fields
        public static Timeline Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (Timeline)FindObjectOfType(typeof(Timeline));

                        if (FindObjectsOfType(typeof(Timeline)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<Timeline>();
                            singleton.name = "Presenter";
                        }
                    }

                    return _instance;
                }
            }
        }

        private static Timeline _instance;

        private static object _lock = new object();
        #endregion

        #region Public Fields
        /// <summary>
        /// Invoked on animation reached the end
        /// </summary>
        public Action _animationEnded;

        /// <summary>
        /// Invoked on animation reached keynote
        /// </summary>
        public Action<Keynote> _keynoteReached;

        /// <summary>
        /// Tracks with all the keyframes and interpolations on it
        /// </summary>
        public Tracks Tracks = new Tracks();
        #endregion

        #region Public Enumerations
        public enum AnimationUpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        public enum AnimationMode
        {
            //Default mode means we must consider all keynotes setiings and timevector. Used in default playback
            Default = 0,
            //In that mode we conside all keynotes conditions except loops. Used in case of premature jump to next chapter
            InterruptLoops,
            //In that mode we ignoring all keynotes. Used in case of explicit setting of timing by user for example
            IgnoreKeynotes
        }
        #endregion

        #region Serialized Fields
        [SerializeField]
        private AnimationUpdateMode _updateMode = AnimationUpdateMode.FixedUpdate;
        [SerializeField]
        private float _timeVector = 1f;
        [SerializeField]
        private bool _playOnAwake = true;
        [SerializeField]
        private float _currentTime = 0f;
        [SerializeField]
        private float _duration = 30f;
        [SerializeField]
        private List<Keynote> _keynotes = new List<Keynote>();
        /*[SerializeField]
        private TimelineTrack[] _tracks = new TimelineTrack[32];*/

        [SerializeField]
        private KeyCode _nextActionKey = KeyCode.RightArrow;
        [SerializeField]
        private KeyCode _previousActionKey = KeyCode.LeftArrow;
        [SerializeField]
        private KeyCode _freezeActionKey = KeyCode.Space;
        [SerializeField]
        private KeyCode _returnToBeginningActionKey = KeyCode.Return;

#if UNITY_EDITOR
        [SerializeField]
        private string _name = "New Story";
        [SerializeField]
        private float _pixelPerSecond = 100f;
        [SerializeField]
        private float _pixelPerTrack = 40f;
#endif
        #endregion
        
        #region Private Fields
        private float _playTimer = 0f;
        private float _playTimerDelay = -1f;

        private List<MetaKeyframeBase> _commands = new List<MetaKeyframeBase>();
        private List<MetaKeyframeBase> _keyframes = new List<MetaKeyframeBase>();
        private List<MetaKeyframeBase> _interpolationKeyframes = new List<MetaKeyframeBase>();

        private bool _animated;

        //A keynote that we are currently at
        private Keynote _currentKeynote;
        #endregion

        #region Public Properties
        /// <summary>
        /// Controls in wich callback updates will be performed
        /// </summary>
        public AnimationUpdateMode UpdateMode
        {
            get
            {
                return _updateMode;
            }
            set
            {
                _updateMode = value;
            }
        }

        /// <summary>
        /// Vector of time. It can be used to adjust speed and direction of animation.
        /// </summary>
        public float TimeVector
        {
            get
            {
                return _timeVector;
            }
            set
            {
                _timeVector = value;
            }
        }

        public bool Paused
        {
            get
            {
                return !_animated;
            }
        }

        public bool PlayOnAwake
        {
            get
            {
                return _playOnAwake;
            }
            set
            {
                _playOnAwake = value;
            }
        }

        /// <summary>
        /// This is a handler of local time for this timeline. As Timeline started it goes all the 
        /// way from 0 to duration, and _currrentTime is a current position on this way.
        /// </summary>
        public float CurrentTime
        {
            get
            {
                return _currentTime;
            }
            set
            {
                _currentTime = value;
                if (_currentTime > Duration)
                {
                    _currentTime = Duration;
                }
                else if (_currentTime < 0f)
                {
                    _currentTime = 0f;
                }
            }
        }

        public float Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }

        public List<Keynote> Keynotes
        {
            get
            {
                return _keynotes;
            }
        }

        /*public TimelineTrack[] Tracks
        {
            get
            {
                return _tracks;
            }
        }*/

#if UNITY_EDITOR
        /// <summary>
        /// This is the scale variable, describing how much pixels will be allocated to represent one second on a timeline.
        /// </summary>
        public float PixelPerSecond
        {
            get
            {
                return _pixelPerSecond;
            }
            set
            {
                if (value < 3f)
                {
                    value = 3f;
                }
                _pixelPerSecond = value;
            }
        }

        public Keynote GetCurrentKeynote()
        {
            Keynote result = null;

            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (i == _keynotes.Count - 1 && _currentTime >= _keynotes[i].Timing)
                {
                    result = _keynotes[i];
                }
                else if (_currentTime >= _keynotes[i].Timing && _currentTime < _keynotes[i + 1].Timing)
                {
                    result = _keynotes[i];
                    break;
                }
            }

            return result;
        }

        public Keynote ToPreviousChapter()
        {
            if (_currentKeynote != null)
            {
                if (_currentKeynote.Timing < _currentTime)
                {
                    GoToTiming(_currentKeynote.Timing, AnimationMode.IgnoreKeynotes);
                }
                else
                {
                    int previousKeynote = _keynotes.IndexOf(_currentKeynote) - 1;
                    if (previousKeynote >= 0)
                    {
                        _currentKeynote = _keynotes[previousKeynote];
                        GoToTiming(_currentKeynote.Timing, AnimationMode.IgnoreKeynotes);
                    }
                }
            }
            else if (_keynotes.Count > 0)
            {
                for (int i = _keynotes.Count - 1; i >= 0; i--)
                {
                    if (_keynotes[i].Timing < _currentTime)
                    {
                        _currentKeynote = _keynotes[i];
                        GoToTiming(_currentKeynote.Timing, AnimationMode.IgnoreKeynotes);
                        break;
                    }
                }
            }

            return _currentKeynote;
        }

        public Keynote ToNextChapter()
        {
            if (_currentKeynote != null)
            {
                int nextKeynote = _keynotes.IndexOf(_currentKeynote) + 1;
                if (nextKeynote < _keynotes.Count)
                {
                    _currentKeynote = _keynotes[nextKeynote];
                    GoToTiming(_currentKeynote.Timing, AnimationMode.IgnoreKeynotes);
                }
            }
            else if (_keynotes.Count > 0)
            {
                for (int i = 0; i < _keynotes.Count; i++)
                {
                    if (_keynotes[i].Timing > _currentTime)
                    {
                        _currentKeynote = _keynotes[i];
                        GoToTiming(_currentKeynote.Timing, AnimationMode.IgnoreKeynotes);
                        break;
                    }
                }
            }

            return _currentKeynote;
        }

        /// <summary>
        /// This is the scale variable, describing how much pixels will be allocated to represent one track on a timeline.
        /// </summary>
        public float PixelPerTrack
        {
            get
            {
                return _pixelPerTrack;
            }
            set
            {
                _pixelPerTrack = value;
            }
        }

        /*public int NumberOfKeyframes
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < Tracks.Length; i++)
                {
                    if (Tracks[i] != null && Tracks[i].Keyframes != null)
                    {
                        counter += Tracks[i].Keyframes.Count;
                    }
                }
                return counter;
            }
        }

        public int NumberOfTracks
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < Tracks.Length; i++)
                {
                    if (Tracks[i] != null)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public int NumberOfInterpolations
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < Tracks.Length; i++)
                {
                    if (Tracks[i] != null && Tracks[i].Interpolations != null)
                    {
                        counter += Tracks[i].Interpolations.Count;
                    }
                }
                return counter;
            }
        }*/

        public bool Playing
        {
            get
            {
                return _animated;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
#endif
        #endregion

        #region Monobehaviour LifeCycle 

        private void Awake()
        {
            if (PlayOnAwake)
            {
                Play();
            }
        }

        private void Update()
        {
            if (Application.isPlaying && UpdateMode == AnimationUpdateMode.Update)
            {
                commonUpdate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (Application.isPlaying && UpdateMode == AnimationUpdateMode.FixedUpdate)
            {
                commonUpdate(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (Application.isPlaying && UpdateMode == AnimationUpdateMode.LateUpdate)
            {
                commonUpdate(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            Tracks.Clear();
            /*for (int i = 0; i < _tracks.Length; i++)
            {
                if (_tracks[i] != null && _tracks[i].Keyframes != null && _tracks[i].Keyframes.Count > 0)
                {
                    for (int j = 0; j < _tracks[i].Keyframes.Count; j++)
                    {
                        Destroy(_tracks[i].Keyframes[j]);
                    }
                }
            }*/
        }

        #endregion

        #region Component Interface Methods
        public void Play(bool fromBeginning = false)
        {
            if (fromBeginning)
            {
                Stop();
            }
            else
            {
                if ((TimeVector >= 0f && _currentTime == _duration) ||
                    (TimeVector < 0f && _currentTime == 0f))
                {
                    Stop();
                }
            }
            
            _animated = true;
        }

        public void Stop()
        {
            _animated = false;
            if (TimeVector >= 0f)
            {
                _currentTime = _duration;
                GoToTiming(0f, AnimationMode.IgnoreKeynotes);
            }
            else
            {
                _currentTime = 0f;
                GoToTiming(_duration, AnimationMode.IgnoreKeynotes);
            }
            //_currentKeynote = null;
        }

        public void Pause()
        {
            _animated = false;
        }

        public void DeleteTrackOnTracksViewPosition(int tracksViewPosition)
        {
            Tracks.DeleteTrackOnTracksViewPosition(tracksViewPosition);
        }

        public List<float> GetAllTimings(MetaKeyframeBase except0 = null, MetaKeyframeBase except1 = null)
        {
            List<float> timings = Tracks.GetAllTimings(except0, except1);

            float offset = 0.0001f;
            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (_keynotes[i].Timing == 0f)
                {
                    timings.Add(_keynotes[i].Timing + offset);
                }
                else if (_keynotes[i].Timing == _duration)
                {
                    timings.Add(_keynotes[i].Timing - offset);
                }
                else
                {
                    timings.Add(_keynotes[i].Timing + offset);
                    timings.Add(_keynotes[i].Timing - offset);
                }
            }

            /*if (!timings.Contains(0f))
            {
                timings.Add(0f);
            }
            if (!timings.Contains(_duration))
            {
                timings.Add(_duration);
            }*/

            return timings;
        }

        public Keynote[] GetKeynotes()
        {
            return _keynotes.ToArray();
        }

        public Keynote GetKeynoteByName(string name)
        {
            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (_keynotes[i].Name == name)
                {
                    return _keynotes[i];
                }
            }

            return null;
        }

        public Keynote[] GetKeynotesByName(string name)
        {
            List<Keynote> keynotes = new List<Keynote>();
            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (_keynotes[i].Name == name)
                {
                    keynotes.Add(_keynotes[i]);
                }
            }

            return keynotes.ToArray();
        }

        public Keynote AddKeynote(float timing, string name = "", string description = "")
        {
            Keynote newKeynote = new Keynote();
            newKeynote.Timing = timing;
            newKeynote.Name = name;
            newKeynote.Description = description;

            _keynotes.Add(newKeynote);
            _keynotes.Sort((x, y) => x.Timing.CompareTo(y.Timing));
            newKeynote.Name = newKeynote.Name + " #" + _keynotes.Count;

            return newKeynote;
        }

        public bool RemoveKeynote(Keynote keynote)
        {
            if (_keynotes.Contains(keynote))
            {
                _keynotes.Remove(keynote);
                _keynotes.Sort((x, y) => x.Timing.CompareTo(y.Timing));
                return true;
            }
            else
            {
                return false;
            }
        }

#if UNITY_EDITOR
        public void SavePreviousTimingsStartingFrom(float timing)
        {
            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (_keynotes[i].Timing > timing)
                {
                    _keynotes[i].PreviousTiming = _keynotes[i].Timing;
                }
            }

            Tracks.SavePreviousTimingsStartingFrom(timing);
        }

        public void SetTimingsFrom(float timing, float delta)
        {
            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (_keynotes[i].PreviousTiming > timing)
                {
                    _keynotes[i].Timing = _keynotes[i].PreviousTiming + delta;
                }
            }

            Tracks.SetTimingsFrom(timing, delta);
        }
#endif

        public void RemoveKeyframe(MetaKeyframeBase keyframe)
        {
            Tracks.DeleteKeyframe(keyframe);
            /*if (keyframe != null)
            {
                for (int i = 0; i < _tracks.Length; i++)
                {
                    if (_tracks[i] != null && _tracks[i].Keyframes != null)
                    {
                        if (_tracks[i].Remove(keyframe))
                        {
                            break;
                        }
                    }
                }

                DestroyImmediate(keyframe);
            }*/
        }

        public MetaInterpolationBase AddInterpolation(MetaInterpolationBase interpolation)
        {
            /*interpolation.hideFlags = HideFlags.HideInInspector;

            if (interpolation.Keyframes[0].TrackIndex >= 0 && interpolation.Keyframes[0].TrackIndex < _tracks.Length &&
                interpolation.Keyframes[1].TrackIndex >= 0 && interpolation.Keyframes[1].TrackIndex < _tracks.Length)
            {
                CheckTrackExistance(interpolation.Keyframes[0].TrackIndex);
                CheckTrackExistance(interpolation.Keyframes[1].TrackIndex);

                _tracks[interpolation.Keyframes[0].TrackIndex].Add(interpolation);
                interpolation.TrackIndex = interpolation.Keyframes[0].TrackIndex;
                //interpolation.Target = interpolation.Keyframes[0].Target;
            }*/
            Tracks.AddInterpolation(interpolation);

            return interpolation;
        }

        public float[] GetKeynoteRangeByTime(float timing)
        {
            float[] result = null;

            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (i < _keynotes.Count - 1)
                {
                    if (timing >= _keynotes[i].Timing)
                    {
                        result = new float[] { _keynotes[i].Timing, _keynotes[i + 1].Timing };
                    }
                }
                else
                {
                    if (timing >= _keynotes[i].Timing)
                    {
                        result = new float[] { _keynotes[i].Timing, _duration };
                    }
                }
            }

            if (result == null && _keynotes.Count > 0)
            {
                result = new float[] { 0f, _keynotes[0].Timing };
            }

            if (result == null)
            {
                result = new float[] { 0f, _duration };
            }

            return result;
        }

        public void RemoveInterpolation(MetaInterpolationBase interpolation)
        {
            Tracks.DeleteInterpolation(interpolation);
            /*if (interpolation != null)
            {
                for (int i = 0; i < _tracks.Length; i++)
                {
                    if (_tracks[i].Interpolations != null && _tracks[i].Interpolations.Contains(interpolation))
                    {
                        _tracks[i].Interpolations.Remove(interpolation);
                        DestroyImmediate(interpolation);
                        return true;
                    }
                }

                DestroyImmediate(interpolation);
            }

            return false;*/
        }

        /// <summary>
        /// Called only when we have no target. So it creates one.
        /// </summary>
        /// <param name="type">Target type needed</param>
        /// <param name="go">Optionally - gameObject to create target on</param>
        /// <returns>Target</returns>
        private UnityEngine.Object GetTarget(Type type, GameObject go = null)
        {
            GameObject root = go;
            if (root == null)
            {
#if UNITY_EDITOR
                if (Selection.activeGameObject == null)
                {
                    root = new GameObject("New " + type.Name + " Object");
                }
                else
                {
                    root = Selection.activeGameObject;
                }
#else
                root = new GameObject("New " + type.Name + " Object");
#endif
            }

            if (type.Equals(typeof(GameObject)))
            {
                return root;
            }

            UnityEngine.Object target = null;
            target = root.GetComponent(type);
            if (target == null)
            {
                target = root.AddComponent(type);
            }

            return target;
        }

        //---- THREE KINDS OF KEYFRAMES ADDER
        /// <summary>
        /// Most popular keyframe adder - we have defined target, but we have only number of row that we want to use.
        /// It's typical for droped, pasted keyframes or maybe used from script. Not guaranteed that track on that row will match.
        /// </summary>
        /// <param name="onRow"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public MetaKeyframeBase AddKeyframe(int onRow, UnityEngine.Object target, int row, float second, string path = "")
        {
            MetaKeyframeBase result = null;

            TimelineTrack requiredTrack = Tracks.GetTrackOnTracksViewPosition(onRow);
            /*if (requiredTrack != null)
            {
                if (requiredTrack.Target != target)
                {
                    if (requiredTrack.Target is GameObject)
                    {
                        throw new Exception("This track is only for " + (requiredTrack.Target as GameObject).name + " keyframes.");
                    }
                    else if (requiredTrack.Target is Component)
                    {
                        throw new Exception("This track is only for " + (requiredTrack.Target as Component).GetType().Name + " (" + (requiredTrack.Target as Component).gameObject.name + ") keyframes.");
                    }
                    else
                    {
                        throw new Exception("Track's target has an unknown type");
                    }
                }
            }*/

            return result;
        }

        /// <summary>
        /// Keyframe adder used in context menu case - we now row and type of keyframe, but we haven't each of them yet.
        /// So we first get them, and result isn't guaranteed.
        /// </summary>
        /// <param name="onRow"></param>
        /// <param name="ofType"></param>
        /// <returns></returns>
        public MetaKeyframeBase AddKeyframe(int onRow, Type ofType, int row, float second, string path = "")
        {
            MetaKeyframeBase result = null;

            return result;
        }

        /// <summary>
        /// Case used by track's header's button - when we know both exactly track and target that we want. No surprises.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public MetaKeyframeBase AddKeyframe(TimelineTrack track, UnityEngine.Object target, int row, float second, string path = "")
        {
            MetaKeyframeBase result = null;

            return result;
        }
        //----------------------------------
#if UNITY_EDITOR
        public MetaKeyframeBase AddKeyframeOnTrack(MetaKeyframeBase keyframe, int tracksViewPosition)
        {
            Tracks.AddKeyframeOnTrack(keyframe, tracksViewPosition);

            GoToTiming(keyframe.Timing, AnimationMode.IgnoreKeynotes, true);
            return keyframe;
        }

        public MetaKeyframeBase AddKeyframeOnTrack(MetaKeyframeBase keyframe, int tracksViewPosition, float timing)
        {
            keyframe.Timing = timing;
            Tracks.AddKeyframeOnTrack(keyframe, tracksViewPosition);
            
            GoToTiming(keyframe.Timing, AnimationMode.IgnoreKeynotes, true);
            return keyframe;
        }

        public MetaKeyframeBase AddKeyframeOnTrack(GameObject activeGameObject, UnityEngine.Object targetObject, Type keyframeType, Type targetType, int row, float second, string path)
        {
            MetaKeyframeBase result = null;

            if (targetType == typeof(AudioSource))
            {
                if (activeGameObject == null)
                {
                    activeGameObject = new GameObject("Audio");
                }

                Component component = activeGameObject.GetComponent<AudioSource>();
                if (activeGameObject.GetComponent(targetType) == null)
                {
                    component = activeGameObject.AddComponent<AudioSource>();
                }
                (component as AudioSource).clip = targetObject as AudioClip;

                result = AddKeyframeOnTrack(component, row, second, path);
            }

            if (result != null)
            {
                GoToTiming(result.Timing, AnimationMode.IgnoreKeynotes, true);
            }
            return result;
        }


        public MetaKeyframeBase AddKeyframeOnTrack(GameObject activeGameObject, Type keyframeType, Type targetType, int row, float second, string path)
        {
            MetaKeyframeBase result = null;

            if (targetType == typeof(GameObject))
            {
                if (activeGameObject == null)
                {
                    activeGameObject = new GameObject();
                }

                result = AddKeyframeOnTrack(activeGameObject, row, second, path);
            }
            else
            {
                if (activeGameObject == null)
                {
                    if (targetType != typeof(Transform))
                    {
                        activeGameObject = new GameObject(targetType.Name);
                    }
                    else
                    {
                        activeGameObject = gameObject;
                    }
                }

                Component component = activeGameObject.GetComponent(targetType);
                if (activeGameObject.GetComponent(targetType) == null)
                {
                    component = activeGameObject.AddComponent(targetType);
                }

                result = AddKeyframeOnTrack(component, row, second, path);
            }

            if (result != null)
            {
                GoToTiming(result.Timing, AnimationMode.IgnoreKeynotes, true);
            }
            return result;
        }

        public MetaKeyframeBase AddKeyframeOnTrack(GameObject gameObject, int row, float timing, string path)
        {
            Component newKeyframe = null;

            if (row >= 0)
            {
                Type keyframeType = GetKeyframeTypeForComponent(typeof(GameObject));

                if (keyframeType != null)
                {
                    newKeyframe = gameObject.AddComponent(keyframeType);
                    if (HideComponentsMode)
                    {
                        newKeyframe.hideFlags = HideFlags.HideInInspector;
                    }
                    (newKeyframe as MetaKeyframeBase).Timing = timing;
                    (newKeyframe as MetaKeyframeBase).Init(gameObject, path);
                    //(newKeyframe as MetaKeyframeBase).Fetch();

                    AddKeyframeOnTrack(newKeyframe as MetaKeyframeBase, row);
                }
            }

            if (newKeyframe != null)
            {
                GoToTiming((newKeyframe as MetaKeyframeBase).Timing, AnimationMode.IgnoreKeynotes, true);
            }
            return newKeyframe as MetaKeyframeBase;
        }

        public MetaKeyframeBase AddKeyframeOnTrack(Component component, int row, float timing, string path)
        {
            Component newKeyframe = null;

            if (row >= 0)
            {
                Type keyframeType = GetKeyframeTypeForComponent(component.GetType());

                if (keyframeType != null)
                {
                    newKeyframe = gameObject.AddComponent(keyframeType);
                    if (HideComponentsMode)
                    {
                        newKeyframe.hideFlags = HideFlags.HideInInspector;
                    }
                    (newKeyframe as MetaKeyframeBase).Timing = timing;
                    (newKeyframe as MetaKeyframeBase).Init(component, path);
                    //(newKeyframe as MetaKeyframeBase).Fetch();

                    AddKeyframeOnTrack(newKeyframe as MetaKeyframeBase, row);
                }
            }

            if (newKeyframe != null)
            {
                GoToTiming((newKeyframe as MetaKeyframeBase).Timing, AnimationMode.IgnoreKeynotes, true);
            }
            return newKeyframe as MetaKeyframeBase;
        }
        public MetaInterpolationBase AddInterpolation(MetaKeyframeBase key1, MetaKeyframeBase key2, string path)
        {
            MetaInterpolationBase newInterpolation = null;

            Type interpolationType = GetInterpolationTypeFor(key1.GetType());
            if (interpolationType != null)
            {
                newInterpolation = gameObject.AddComponent(interpolationType) as MetaInterpolationBase;
                if (HideComponentsMode)
                {
                    newInterpolation.hideFlags = HideFlags.HideInInspector;
                }
                newInterpolation.Init(key1, key2, path);
                //newInterpolation.TrackIndex = key1.TrackIndex;

                Tracks.AddInterpolation(newInterpolation);
            }

            return newInterpolation;
        }
#endif
#endregion

        #region Main Methods
#if UNITY_EDITOR
        public void CreateNewKeyframeButtonPressed(TimelineTrack track, float timing = -1f)
        {
            if (track == null)
            {
                return;
            }
            //timing will be by default equals current time, but if there is already a keyframe with that timing - we move 1 second forward
            float requiredTiming = timing;
            if (requiredTiming < 0)
            {
                requiredTiming = _currentTime;
            }
            for (int i = 0; i < track.Keyframes.Count; i++)
            {
                if (track.Keyframes[i].Timing == requiredTiming)
                {
                    requiredTiming += 1f;
                    if (requiredTiming > _duration)
                    {
                        requiredTiming = _duration;
                    }
                    break;
                }
            }

            MetaKeyframeBase interpolateFrom = null;
            if (track.InterpolationModeKeyframeAdding)
            {
                //if that's the selected mode - we make interpolation to previous keyframe automatically
                for (int i = 0; i < track.Keyframes.Count; i++)
                {
                    if ((interpolateFrom == null && track.Keyframes[i].Timing < requiredTiming) || (track.Keyframes[i].Timing < requiredTiming && track.Keyframes[i].Timing > interpolateFrom.Timing))
                    {
                        interpolateFrom = track.Keyframes[i];
                    }
                }
            }

            Undo.RecordObject(this, "New Keyframe Added");
            MetaKeyframeBase newKeyframe = null;
            if (track.Target is ModelExploder)
            {
                ModelExploder modelExploder = track.Target as ModelExploder;

                int oldSelectedIndex = modelExploder.SelectedStateIndex;
                newKeyframe = AddKeyframeOnTrack(modelExploder, track.TracksViewPosition, requiredTiming, "");

                modelExploder.UpdateInitialTransforms(1);
                //modelExploder.RecalculateAllExplosionTargets(0, 1);
                MetaKeyframeBase closestPrevious = Tracks.GetClosestPreviousKeyframe(newKeyframe, track.TracksViewPosition);
                int sourceIndex = -1;
                if (closestPrevious != null)
                {
                    sourceIndex = (closestPrevious as ModelExploderKeyframe).ExplosionSettingsBundleIndex;
                }
                
                modelExploder.Save(1, sourceIndex);


                (newKeyframe as ModelExploderKeyframe).ExplosionSettingsBundleIndex = modelExploder.ExplosionSettingsBundles.Count - 1;

                //modelExploder.ExplosionSettingsBundles[(newKeyframe as ModelExploderKeyframe).ExplosionSettingsBundleIndex].sourceID = oldSelectedIndex;

                modelExploder.SelectedStateIndex = (newKeyframe as ModelExploderKeyframe).ExplosionSettingsBundleIndex;
            }
            else
            {
                if (track.Target is GameObject)
                {
                    newKeyframe = AddKeyframeOnTrack(track.Target as GameObject, track.TracksViewPosition, requiredTiming, "");
                }
                else
                {
                    newKeyframe = AddKeyframeOnTrack(track.Target as Component, track.TracksViewPosition, requiredTiming, "");
                }
            }

            if (interpolateFrom != null && newKeyframe != null)
            {
                AddInterpolation(interpolateFrom, newKeyframe, "");
            }

            if (newKeyframe != null)
            {
                GoToTiming(newKeyframe.Timing, AnimationMode.IgnoreKeynotes);
            }
        }

        public void EditorUpdate(float deltaTime, KeyCode keyCode = KeyCode.None)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (Tracks.ProprietaryMode)
                {
                    Tracks.CheckInexistingObjects();
                }
                /*if (!EditorAnimationPlayer.Initialized)
                {
                    if (Application.isEditor && !Application.isPlaying)
                    {
                        EditorAnimationPlayer.DeInitiate();
                        EditorAnimationPlayer.Initiate();
                    }
                }*/

                commonUpdate(deltaTime, keyCode);
            }
        }
#endif

        private void ResetTimer()
        {
            _playTimerDelay = -1f;
            _playTimer = 0f;
        }

        private void ProcessTimer(float deltaTime)
        {
            if (_playTimerDelay >= 0f)
            {
                _playTimer += deltaTime;

                if (_playTimer >= _playTimerDelay)
                {
                    ResetTimer();

                    Play();
                }
            }
        }

        //This methos is a common callback for all type of updates
        private void commonUpdate(float deltaTime, KeyCode keyCode = KeyCode.None)
        {
            /*if (_currentKeynote != null)
            {
                _currentKeynote.Update(deltaTime, keyCode);
            }*/

            ProcessTimer(deltaTime);

            CheckActionKeys(keyCode);

            if (_animated)
            {
                Animate(deltaTime, _timeVector);
            }

            //_clipLibrary.Update(deltaTime);
        }

        private void CheckActionKeys(KeyCode keyCode = KeyCode.None)
        {
            KeyCode actualKeynote = keyCode;
            if (actualKeynote == KeyCode.None)
            {
                if (Input.GetKeyUp(_nextActionKey))
                {
                    OnNextActionDone();
                }
                else if (Input.GetKeyUp(_previousActionKey))
                {
                    OnPreviousActionDone();
                }
                else if (Input.GetKeyUp(_returnToBeginningActionKey))
                {
                    OnPlayFromBeginningActionDone();
                }
                else if (Input.GetKeyUp(_freezeActionKey))
                {
                    OnPauseActionDone();
                }
            }
            else
            {
                if (keyCode == _nextActionKey)
                {
                    OnNextActionDone();
                }
                else if (keyCode == _previousActionKey)
                {
                    OnPreviousActionDone();
                }
                else if (keyCode == _returnToBeginningActionKey)
                {
                    OnPlayFromBeginningActionDone();
                }
                else if (keyCode == _freezeActionKey)
                {
                    OnPauseActionDone();
                }
            }
        }

        /// <summary>
        /// Invoked when some keynote's duration ended
        /// </summary>
        /// <param name="keynote"></param>
        /// <param name="interruptLoop"></param>
        /// <returns>do keynote really ended and we need to go further</returns>
        private bool KeynoteEnd(Keynote keynote, AnimationMode animationMode = AnimationMode.Default)
        {
            keynote.ReachedEndOfKeynote();

            //if we looped and don't interrupting loops - go to beginning
            if (animationMode != AnimationMode.IgnoreKeynotes && keynote.OnKeynoteStartAction == OnKeynoteStartAction.Loop && keynote.LoopedOnStart && animationMode != AnimationMode.InterruptLoops)
            {
                //start from beginning
                GoToTiming(_currentKeynote.Timing);

                return false;
            }
            else
            {
                /*keynote._conditionComplied -= OnConditionComplied;
                keynote._pauseActionDone -= OnPauseActionDone;
                keynote._nextActionDone -= OnNextActionDone;*/

                return true;
            }
        }

        /// <summary>
        /// Invoked when we just started new keynote
        /// </summary>
        /// <param name="keynote"></param>
        /// <returns>returns timing that we need to go to</returns>
        private void KeynoteStart(Keynote keynote, AnimationMode animationMode = AnimationMode.Default)
        {
            /*keynote._conditionComplied += OnConditionComplied;
            keynote._pauseActionDone += OnPauseActionDone;
            keynote._nextActionDone += OnNextActionDone;*/

            if (animationMode != AnimationMode.IgnoreKeynotes)
            {
                if (keynote.OnKeynoteStartAction == OnKeynoteStartAction.Pause)
                {
                    Pause();
                }
                else if (keynote.OnKeynoteStartAction == OnKeynoteStartAction.Delay)
                {
                    Pause();

                    _playTimerDelay = keynote.DelayOnStart;
                }

                keynote.ReachedKeynoteMoment();

                if (_keynoteReached != null)
                {
                    _keynoteReached.Invoke(_currentKeynote);
                }
            }

            GoToTiming(keynote.Timing);
        }

        /// <summary>
        /// Perform all operations related to keynotes. It can or cannot interrupt usual playback, depending on keynotes that are active now.
        /// </summary>
        /// <param name="timing"></param>
        /// <param name="interruptLoop"></param>
        /// <returns>Is usual playback needs to be interrupted</returns>
        private bool CheckCurrentKeynote(float timing, AnimationMode animationMode = AnimationMode.Default)
        {
            //Usual playback needs to be interruped when some keynotes making actions to playback. That happens when we reached new 
            //keynote and it looped us back to it's beginning, when we reached new keynote and it pause playback so we listen to launch
            //action to continue.
            bool interruptUsualPlayback = false;

            Keynote newKeynote = GetCurrentKeynote(timing);

            if (newKeynote != null && newKeynote != _currentKeynote)
            {
                //case when we've reached new Keynote. When we reached keynote we must at first check if old keynote isn't looping.
                //And if it is, we must go to beginning. If it's not looped, then we stop and start to listen to it's launching, pause 
                //and next events.

                bool needToGoNext = true;
                if (_currentKeynote != null)
                {
                    needToGoNext = KeynoteEnd(_currentKeynote, animationMode);
                }

                if (needToGoNext)
                {
                    if (_currentKeynote != null)
                    {
                        _currentKeynote.Ended();
                    }

                    _currentKeynote = newKeynote;

                    KeynoteStart(_currentKeynote, animationMode);
                }

                //We interrupt always when reached new keynote, because if we do not need to go next, that means, we go back, and interrupt.
                //And if we doesn't - that means, we Pause at the beginning of every keynote and interrupt anyway
                interruptUsualPlayback = true;
            }

            return interruptUsualPlayback;
        }

        public void GoToTiming(float timing, AnimationMode animationMode = AnimationMode.Default, bool inclusive = false)
        {
            float lastFrameTiming = _currentTime;
            float currentFrameTiming = timing;

            bool performFinalTick = false;
            if (TimeVector >= 0f && currentFrameTiming >= _duration)
            {
                currentFrameTiming = _duration;
                performFinalTick = true;
            }
            else if (TimeVector < 0f && currentFrameTiming <= 0f)
            {
                currentFrameTiming = 0f;
                performFinalTick = true;
            }
            _currentTime = currentFrameTiming;

            performAnimationsForPeriod(lastFrameTiming, currentFrameTiming);
            if (animationMode == AnimationMode.IgnoreKeynotes)
            {
                _currentKeynote = GetCurrentKeynote(timing);
            }
            if (performFinalTick)
            {
                performAnimationsForPeriod(currentFrameTiming, currentFrameTiming);
                
                if (_currentKeynote != null)
                {
                    if (KeynoteEnd(_currentKeynote, animationMode))
                    {
                        _animated = false;

                        if (_animationEnded != null)
                        {
                            _animationEnded.Invoke();
                        }
                    }
                }
                else
                {
                    _animated = false;

                    if (_animationEnded != null)
                    {
                        _animationEnded.Invoke();
                    }
                }
            }
            else if (inclusive)
            {
                performAnimationsForPeriod(currentFrameTiming, currentFrameTiming);
            }
        }

        public void StartFromKeynote(Keynote keynote, AnimationMode animationMode = AnimationMode.InterruptLoops)
        {
            //Starting from keynote means firstly performing animation from current position to keynote position.
            //Second, we must apply new current keynote ignoring looping.
            //GoToTiming(keynote.Timing);
            CheckCurrentKeynote(keynote.Timing, animationMode);
        }

        /// <summary>
        /// Animate function is a controller for automatic playback. It controls how animation is performed by default.
        /// </summary>
        private void Animate(float deltaTime, float vector = 1f, bool ignoreKeynotes = false)
        {
            float lastFrameTiming = _currentTime;
            float currentFrameTiming = lastFrameTiming + deltaTime * vector;
            
            if (CheckCurrentKeynote(currentFrameTiming))
            {
                return;
            }

            bool performFinalTick = false;
            if (TimeVector >= 0f && currentFrameTiming >= _duration)
            {
                currentFrameTiming = _duration;
                performFinalTick = true;
            }
            else if (TimeVector < 0f && currentFrameTiming <= 0f)
            {
                currentFrameTiming = 0f;
                performFinalTick = true;
            }
            _currentTime = currentFrameTiming;

            //_clipLibrary.GoTo(currentFrameTiming);

            performAnimationsForPeriod(lastFrameTiming, currentFrameTiming);
            if (performFinalTick)
            {
                performAnimationsForPeriod(currentFrameTiming, currentFrameTiming);

                if (_currentKeynote != null)
                {
                    if (KeynoteEnd(_currentKeynote))
                    {
                        _animated = false;

                        if (_animationEnded != null)
                        {
                            _animationEnded.Invoke();
                        }
                    }
                }
            }
        }

        public void ChangeKeyframesTarget(MetaKeyframeBase dropingOnKeyframe, UnityEngine.Object newTarget)
        {
            dropingOnKeyframe.Target = newTarget;
        }

        private void performAnimationsForPeriod(float periodOrigin, float periodEnding)
        {
            _commands.Clear();
            _keyframes.Clear();
            _interpolationKeyframes.Clear();

            int multiplier = 1;

            int soloTrackIndex = Tracks.GetSoloTrack();

            if (periodOrigin <= periodEnding)
            {
                /*
                 * forward time case
                 */

                for (int i = 0; i < Tracks.Count; i++)
                {
                    if (soloTrackIndex >= 0 && i != soloTrackIndex)
                    {
                        continue;
                    }

                    if (Tracks.TrackOnIndex(i).Hided)
                    {
                        continue;
                    }

                    for (int j = 0; j < Tracks.TrackOnIndex(i).Keyframes.Count; j++)
                    {
                        if ((Tracks.TrackOnIndex(i).Keyframes[j].Timing >= periodOrigin && Tracks.TrackOnIndex(i).Keyframes[j].Timing < periodEnding) ||
                            (Tracks.TrackOnIndex(i).Keyframes[j].Timing == periodOrigin))
                        {
                            _keyframes.Add(Tracks.TrackOnIndex(i).Keyframes[j]);
                        }
                    }
                }
            }
            else
            {
                /*
                 * backward time case
                 */

                multiplier = -1;

                float temp = periodOrigin;
                periodOrigin = periodEnding;
                periodEnding = temp;

                //Collecting required keyframes
                for (int i = 0; i < Tracks.Count; i++)
                {
                    if (soloTrackIndex >= 0 && i != soloTrackIndex)
                    {
                        continue;
                    }

                    if (Tracks.TrackOnIndex(i).Hided)
                    {
                        continue;
                    }

                    for (int j = 0; j < Tracks.TrackOnIndex(i).Keyframes.Count; j++)
                    {
                        if ((Tracks.TrackOnIndex(i).Keyframes[j].Timing > periodOrigin && Tracks.TrackOnIndex(i).Keyframes[j].Timing <= periodEnding) ||
                            (Tracks.TrackOnIndex(i).Keyframes[j].Timing == periodEnding))
                        {
                            _keyframes.Add(Tracks.TrackOnIndex(i).Keyframes[j]);
                        }
                    }
                }
            }

            //Collecting required interpolation keyframes
            for (int i = 0; i < Tracks.Count; i++)
            {
                if (soloTrackIndex >= 0 && i != soloTrackIndex)
                {
                    continue;
                }

                if (Tracks.TrackOnIndex(i).Hided)
                {
                    continue;
                }

                for (int j = 0; j < Tracks.TrackOnIndex(i).Interpolations.Count; j++)
                {
                    int startIndex = 0;
                    int endIndex = 1;
                    if (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[0].Timing > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[1].Timing)
                    {
                        startIndex = 1;
                        endIndex = 0;
                    }
                    if ((periodOrigin > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing && periodOrigin < Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing) ||
                        (periodEnding > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing && periodEnding < Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing) ||
                        (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing > periodOrigin && Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing < periodEnding) ||
                        (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing > periodOrigin && Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing < periodEnding))
                    {
                        //We slice interpolation into sub-keyframes
                        for (int m = 0; m < _keyframes.Count; m++)
                        {
                            //Creating a sub-keyframe on every moment it interrupted by keyframes
                            if (_keyframes[m].Timing > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing && _keyframes[m].Timing < Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing)
                            {
                                float ratio = (_keyframes[m].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing) / (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing);

                                MetaKeyframeBase interpolatedKeyframe = Tracks.TrackOnIndex(i).Interpolations[j].Interpolate(ratio);
                                if (interpolatedKeyframe != null)
                                {
                                    _interpolationKeyframes.Add(interpolatedKeyframe);
                                }
                            }
                        }

                        //and also we must create one final subkeyframe if interpolation was interrupted by the end of period
                        if (multiplier >= 0)
                        {
                            if (periodEnding > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing && periodEnding < Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing)
                            {
                                //float ratio = (periodEnding - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing) / (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing);
                                float ratio = (periodEnding - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing) / (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing);

                                MetaKeyframeBase interpolatedKeyframe = Tracks.TrackOnIndex(i).Interpolations[j].Interpolate(ratio);
                                if (interpolatedKeyframe != null)
                                {
                                    _interpolationKeyframes.Add(interpolatedKeyframe);
                                }
                            }
                        }
                        else
                        {
                            if (periodOrigin > Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing && periodOrigin < Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing)
                            {
                                //float ratio = (periodOrigin - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing) / (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing);
                                float ratio = (periodOrigin - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing) / (Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[endIndex].Timing - Tracks.TrackOnIndex(i).Interpolations[j].Keyframes[startIndex].Timing);

                                MetaKeyframeBase interpolatedKeyframe = Tracks.TrackOnIndex(i).Interpolations[j].Interpolate(ratio);
                                if (interpolatedKeyframe != null)
                                {
                                    _interpolationKeyframes.Add(interpolatedKeyframe);
                                }
                            }
                        }
                    }
                }
            }

            _commands.AddRange(_keyframes);
            
            _commands.AddRange(_interpolationKeyframes);
            _commands.Sort((x, y) => x.CompareTo(y) * multiplier);

            bool forward = true;
            if (multiplier < 0)
            {
                forward = false;
            }

            for (int i = 0; i < _commands.Count; i++)
            {
                _commands[i].Apply(forward);
            }

            for (int i = 0; i < _interpolationKeyframes.Count; i++)
            {
                DestroyImmediate(_interpolationKeyframes[i]);
            }
        }
#endregion

#region Keynotes Callbacks
        /*private void OnConditionComplied(Keynote keynote)
        {
            //keynote._conditionComplied -= OnConditionComplied;

            Play();
        }*/

        private void OnPauseActionDone()
        {
            ResetTimer();
            if (Paused)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        private void OnPlayFromBeginningActionDone()
        {
            ResetTimer();
            Play(true);
        }

        private void OnPreviousActionDone()
        {
            ResetTimer();

            int indexOfKeynote = -1;

            if (TimeVector < 0)
            {
                indexOfKeynote = _keynotes.Count;
            }

            if (_currentKeynote != null)
            {
                indexOfKeynote = _keynotes.IndexOf(_currentKeynote);
            }
            /*float absTimeVector = Mathf.Abs(_timeVector);
            float floatIncrement = (float)Mathf.Round(_timeVector / absTimeVector);
            int intIncrement = (int)floatIncrement;
            int nextIndex = indexOfKeynote - intIncrement;*/
            int nextIndex = indexOfKeynote - 1;
            if (nextIndex < _keynotes.Count && nextIndex >= 0)
            {
                StartFromKeynote(_keynotes[nextIndex], AnimationMode.IgnoreKeynotes);
            }
            else
            {
                //if we hitting Next and there's no next keynote - we must go to the end of animation
                if (TimeVector >= 0f)
                {
                    GoToTiming(0f, AnimationMode.InterruptLoops);
                }
                else
                {
                    GoToTiming(_duration, AnimationMode.InterruptLoops);
                }

                Pause();
            }
        }

        private void OnNextActionDone()
        {
            ResetTimer();

            if (Paused)
            {
                Play();
            }
            else
            {
                //keynote._nextActionDone -= OnNextActionDone;

                int indexOfKeynote = -1;

                if (TimeVector < 0)
                {
                    indexOfKeynote = _keynotes.Count;
                }

                _currentKeynote = GetCurrentKeynote(_currentTime);

                if (_currentKeynote != null)
                {
                    indexOfKeynote = _keynotes.IndexOf(_currentKeynote);
                }
                /*float absTimeVector = Mathf.Sqrt(_timeVector * _timeVector);
                float floatIncrement = Mathf.Round(_timeVector / absTimeVector);
                int intIncrement = (int)floatIncrement;
                int nextIndex = indexOfKeynote + intIncrement;*/
                int nextIndex = indexOfKeynote + 1;
                if (nextIndex < _keynotes.Count && nextIndex >= 0)
                {
                    StartFromKeynote(_keynotes[nextIndex], AnimationMode.IgnoreKeynotes);
                }
                else
                {
                    //if we hitting Next and there's no next keynote - we must go to the end of animation
                    if (TimeVector >= 0f)
                    {
                        GoToTiming(_duration, AnimationMode.InterruptLoops);
                    }
                    else
                    {
                        GoToTiming(0f, AnimationMode.InterruptLoops);
                    }

                    Pause();
                }
            }
        }
#endregion

#region Utility Methods
        public void SortKeynotes()
        {
            _keynotes.Sort((x, y) => x.Timing.CompareTo(y.Timing));
        }

#if UNITY_EDITOR
        public void GetSupportedTypes(out Type[] keyframeTypes, out Type[] interpolationTypes, out Type[] supportedKeyframeTypes, out Type[] supportedInterpolationTypes, out Type[] interpolationKeyframeTypes, out string[] keyframePaths, out string[] interpolationPaths)
        {
            List<Type> kTypes = new List<Type>();
            List<Type> iTypes = new List<Type>();
            List<Type> iKTypes = new List<Type>();
            List<Type> sKTypes = new List<Type>();
            List<Type> sITypes = new List<Type>();
            List<string> keyframeMenuPaths = new List<string>();
            List<string> interpolationMenuPaths = new List<string>();
            Assembly assembly = Assembly.Load("Assembly-CSharp");

            //KeyframeFor keyframeForAttribute = null;

            foreach (Type type in assembly.GetTypes())
            {
                KeyframeFor keyframeForAttribute = Attribute.GetCustomAttribute(type, typeof(KeyframeFor)) as KeyframeFor;

                if (keyframeForAttribute != null)
                {
                    kTypes.Add(type);
                    sKTypes.Add(keyframeForAttribute.Type);
                    keyframeMenuPaths.Add("");

                    if (keyframeForAttribute.Paths != null)
                    {
                        for (int i = 0; i < keyframeForAttribute.Paths.Length; i++)
                        {
                            kTypes.Add(type);
                            sKTypes.Add(keyframeForAttribute.Type);
                            keyframeMenuPaths.Add(keyframeForAttribute.Paths[i]);
                        }
                    }

                    MethodInfo additionalMenuItemsMethodInfo = type.GetMethod("AdditionalMenuItems");
                    if (additionalMenuItemsMethodInfo != null)
                    {
                        string[] additionalItems = (string[])additionalMenuItemsMethodInfo.Invoke(null, null);

                        for (int i = 0; i < additionalItems.Length; i++)
                        {
                            kTypes.Add(type);
                            sKTypes.Add(keyframeForAttribute.Type);
                            keyframeMenuPaths.Add(additionalItems[i]);
                        }
                    }
                }
                else
                {
                    InterpolationFor interpolationForAttribute = Attribute.GetCustomAttribute(type, typeof(InterpolationFor)) as InterpolationFor;

                    if (interpolationForAttribute != null)
                    {
                        keyframeForAttribute = Attribute.GetCustomAttribute(interpolationForAttribute.Type, typeof(KeyframeFor)) as KeyframeFor;

                        if (keyframeForAttribute != null && interpolationForAttribute.Paths != null)
                        {
                            for (int i = 0; i < interpolationForAttribute.Paths.Length; i++)
                            {
                                iTypes.Add(type);
                                sITypes.Add(keyframeForAttribute.Type);
                                iKTypes.Add(interpolationForAttribute.Type);
                                interpolationMenuPaths.Add(interpolationForAttribute.Paths[i]);
                            }
                        }

                        MethodInfo additionalMenuItemsMethodInfo = type.GetMethod("AdditionalMenuItems");
                        if (additionalMenuItemsMethodInfo != null)
                        {
                            string[] additionalItems = (string[])additionalMenuItemsMethodInfo.Invoke(null, null);

                            for (int i = 0; i < additionalItems.Length; i++)
                            {
                                iTypes.Add(type);
                                sITypes.Add(keyframeForAttribute.Type);
                                iKTypes.Add(interpolationForAttribute.Type);
                                interpolationMenuPaths.Add(additionalItems[i]);
                            }
                        }
                    }
                }
            }

            keyframeTypes = kTypes.ToArray();
            interpolationTypes = iTypes.ToArray();
            supportedKeyframeTypes = sKTypes.ToArray();
            supportedInterpolationTypes = sITypes.ToArray();
            interpolationKeyframeTypes = iKTypes.ToArray();
            keyframePaths = keyframeMenuPaths.ToArray();
            interpolationPaths = interpolationMenuPaths.ToArray();
        }

        public Type GetKeyframeTypeForComponent(Type componentType)
        {
            if (componentType == typeof(Animation))
            {
                return typeof(AnimatorKeyframe);
            }

            Type result = null;

            Type[] supportedKeyframeTypes;
            Type[] supportedInterpolationTypes;
            Type[] keyframeTypes;
            Type[] interpolationTypes;
            Type[] interpolationKeyframeTypes;
            string[] keyframeMenuPaths;
            string[] interpolationMenuPaths;
            GetSupportedTypes(out keyframeTypes, out interpolationTypes, out supportedKeyframeTypes, out supportedInterpolationTypes, out interpolationKeyframeTypes, out keyframeMenuPaths, out interpolationMenuPaths);

            for (int i = 0; i < supportedKeyframeTypes.Length; i++)
            {
                if (supportedKeyframeTypes[i] == componentType)
                {
                    result = keyframeTypes[i];
                    break;
                }
            }

            return result;
        }

        public Type GetInterpolationTypeFor(Type metaKeyframeType)
        {
            Type result = null;
            Assembly assembly = Assembly.Load("Assembly-CSharp");

            foreach (Type type in assembly.GetTypes())
            {
                InterpolationFor attribute = Attribute.GetCustomAttribute(type, typeof(InterpolationFor)) as InterpolationFor;

                if (attribute != null && attribute.Type == metaKeyframeType)
                {
                    result = type;
                    break;
                }
            }

            return result;
        }
#endif

        private Keynote GetCurrentKeynote(float timing)
        {
            Keynote result = null;

            for (int i = 0; i < _keynotes.Count; i++)
            {
                if (timing < _keynotes[i].Timing)
                {
                    break;
                }
                else if (i == _keynotes.Count - 1)
                {
                    result = _keynotes[i];
                    break;
                }
                else if (timing < _keynotes[i + 1].Timing && timing >= _keynotes[i].Timing)
                {
                    result = _keynotes[i];
                    break;
                }
            }

            return result;
        }

        /*private void CheckTrackExistance(int trackIndex)
        {
            if (_tracks[trackIndex] == null)
            {
                _tracks[trackIndex] = new TimelineTrack();
            }
            if (Tracks[trackIndex].Keyframes.Count == 0)
            {
                Tracks[trackIndex].Name = "Track" + (trackIndex < 9 ? ("0" + (trackIndex + 1).ToString()) : (trackIndex + 1).ToString());
            }
        }*/

        private bool IsBetweenNegative(float a, float b, float value)
        {
            return (value >= a && value < b) || (value >= b && value < a);
        }

        private bool IsBetweenPositive(float a, float b, float value)
        {
            return (value > a && value <= b) || (value > b && value <= a);
        }

        private bool IsBetweenSaturated(float a, float b, float value)
        {
            return (value >= a && value <= b) || (value >= b && value <= a);
        }

#if UNITY_EDITOR
        public void ChangeTimeResolution(float y)
        {
            PixelPerSecond -= y;
        }

        public void ChangeTrackResolution(float y)
        {
            PixelPerTrack -= y;
            if (PixelPerTrack < MetaKeyframeBase.KeyframeRadius * 2f)
            {
                PixelPerTrack = MetaKeyframeBase.KeyframeRadius * 2f;
            }
        }
#endif
#endregion
    }
}
