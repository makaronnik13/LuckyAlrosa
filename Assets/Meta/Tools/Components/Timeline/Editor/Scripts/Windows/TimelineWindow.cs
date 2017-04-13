using System;
using UnityEngine;
using UnityEditor;

namespace Meta.Tools.Editor
{
    public class TimelineWindow : EditorWindow
    {
        #region Public fields
        public static TimelineWindow Instance;

        public static bool SnappingOn = true;
        public static float PixelsToSnap = 10f;

        private const float _fixedMargin = 10f;
        #endregion

        #region Private fields
        //editor update variables
        private static float _lastTime;
        private static bool _subscribedToUpdates = false;
        private static bool _subscribedToPlaymodeChanged = false;

        //timeline reference
        [SerializeField]
        private Timeline _timeline;
        private SerializedObject _currentTimelineSerialized;
        private static KeyCode _keyCode = KeyCode.None;

        //Layout percentages
        private float _keynotesPercentage = 0.08f;
        private float _keynotesFixedHeight = 22f;
        private float _headerFixedHeight = 116f;//94f;
        private float _headerPercentage = 0.25f;
        private float _headerMinHeight = 48f;

        //Views
        private KeynotesView _keynotesView;
        private HeaderView _headerView;
        private TracksView _tracksView;
        private ChaptersHeadsView _chaptersHeadsView;
        private WindowsView _windowsView;
        #endregion

        #region Public Properties
        public Timeline Timeline
        {
            get
            {
                return _timeline;
            }
        }
        #endregion

        [MenuItem("Meta/Presenter Tool %m")]
        public static void Init()
        {
            if (Instance == null)
            {
                FindWindow();

                if (Instance != null)
                {
                    Instance.FindTimeline();

                    Instance.CreateViews();
                }
            }
        }

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            if (_subscribedToPlaymodeChanged)
            {
                _subscribedToPlaymodeChanged = false;
                EditorApplication.playmodeStateChanged -= StateChange;
            }
            if (_subscribedToUpdates)
            {
                EditorApplication.update -= EditorUpdate;

                _subscribedToUpdates = false;
            }

            _headerView = null;
            _keynotesView = null;
            _tracksView = null;
            _chaptersHeadsView = null;

            Instance = null;
        }

        private void OnEnable()
        {
            if (!_subscribedToPlaymodeChanged)
            {
                _subscribedToPlaymodeChanged = true;
                EditorApplication.playmodeStateChanged += StateChange;
            }
        }

        private void StateChange()
        {
            _timeline = null;
            _keynotesView = null;
            _headerView = null;
            _tracksView = null;
            _chaptersHeadsView = null;

            Repaint();
        }

        private static void FindWindow()
        {
            if (Instance == null)
            {
                Instance = (TimelineWindow)GetWindow(typeof(TimelineWindow));

                Instance.minSize = new Vector2(640f, 240f);

                GUIContent windowTitle = new GUIContent();
                windowTitle.text = "Timeline Tool";
                windowTitle.image = (Texture2D)Resources.Load("Textures/timeline-list-grid-timeline-icon");

                Instance.titleContent = windowTitle;

                Instance.Repaint();
                return;
            }
        }

        private void FindTimeline()
        {
            if (_timeline == null)
            {
                _timeline = Timeline.Instance;

                Repaint();
                return;
            }
        }

        #region Monobehaviour LifeCycle 

        private void OnGUI()
        {
            //Debug.Log("OnGUI");
            FindWindow();
            //setting timeline
            FindTimeline();

            //check for null views
            if (_tracksView == null || _headerView == null || _keynotesView == null || _windowsView == null || _chaptersHeadsView == null)
            {
                CreateViews();
            }
            
            if (_timeline != null)
            {
                //serializing timeline
                _currentTimelineSerialized = new SerializedObject(_timeline);
                Event e = Event.current;

                _headerPercentage = _headerFixedHeight / position.height;
                _keynotesPercentage = _keynotesFixedHeight / position.height;
                //Layout ratios calculated
                float headerPerc = _headerPercentage;
                float keynotesPerc = _keynotesPercentage;
                
                /*if (headerPerc * position.height < _headerMinHeight)
                {
                    headerPerc = _headerMinHeight / position.height;
                }*/

                Handles.BeginGUI();
                Handles.color = new Color(20f/255f, 31f/255f, 45f/255f);
                Handles.DrawAAConvexPolygon(new[] {new Vector3(0, 0),
                    new Vector3(0 + position.width, 0),
                    new Vector3(0 + position.width, 0 + position.height),
                    new Vector3(0, 0 + position.height)});
                Handles.EndGUI();

                float horisontalMargin = _fixedMargin / position.width;
                float verticalMargin = _fixedMargin / position.height;

                //Update views
                if (_timeline.Keynotes.Count > 0)
                {
                    _tracksView.UpdateView(position, new Rect(horisontalMargin, headerPerc + keynotesPerc + verticalMargin, horisontalMargin, verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);
                    _headerView.UpdateView(position, new Rect(horisontalMargin, keynotesPerc + verticalMargin, horisontalMargin, 1f - headerPerc - keynotesPerc - verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);
                    _keynotesView.UpdateView(position, new Rect(0f, 0f, 0f, 1f - keynotesPerc), Vector2.zero, _timeline, _currentTimelineSerialized, this);

                    _chaptersHeadsView.UpdateView(position, new Rect(horisontalMargin + _tracksView.TracksHeadersPercentage * (1f - horisontalMargin * 2f), keynotesPerc, horisontalMargin, 1f - headerPerc - keynotesPerc - verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);

                    /*_tracksView.UpdateView(position, new Rect(0f, headerPerc + keynotesPerc, 0f, 0f), _timeline, _currentTimelineSerialized, this);
                    _headerView.UpdateView(position, new Rect(0f, keynotesPerc, 0f, 1f - headerPerc - keynotesPerc), _timeline, _currentTimelineSerialized, this);
                    _keynotesView.UpdateView(position, new Rect(0f, 0f, 0f, 1f - keynotesPerc), _timeline, _currentTimelineSerialized, this);*/
                }
                else
                {
                    _tracksView.UpdateView(position, new Rect(horisontalMargin, headerPerc + keynotesPerc + verticalMargin, horisontalMargin, verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);
                    _headerView.UpdateView(position, new Rect(horisontalMargin, verticalMargin, horisontalMargin, 1f - headerPerc - keynotesPerc - verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);

                    _chaptersHeadsView.UpdateView(position, new Rect(horisontalMargin + _tracksView.TracksHeadersPercentage * (1f - horisontalMargin * 2f), 0, horisontalMargin, 1f - headerPerc - keynotesPerc - verticalMargin), Vector2.zero, _timeline, _currentTimelineSerialized, this);

                    /*_tracksView.UpdateView(position, new Rect(0f, headerPerc + keynotesPerc, 0f, 0f), _timeline, _currentTimelineSerialized, this);
                    _headerView.UpdateView(position, new Rect(0f, 0f, 0f, 1f - headerPerc - keynotesPerc), _timeline, _currentTimelineSerialized, this);*/
                }
                _windowsView.UpdateView(position, new Rect(0, 0, 0, 0), Vector2.zero, _timeline, _currentTimelineSerialized, this);

                //process events
                _windowsView.ProcessEvents(e);

                _tracksView.ProcessEvents(e);
                if (_timeline.Keynotes.Count > 0)
                {
                    _keynotesView.ProcessEvents(e);
                }
                _headerView.ProcessEvents(e);
                _chaptersHeadsView.ProcessEvents(e);

                _keyCode = KeyCode.None;
                switch (e.type)
                {
                    case EventType.KeyUp:
                        _keyCode = e.keyCode;
                        break;
                }

                WindowShowed = _windowsView.CurrentDrawedKeyframe != null || _windowsView.CurrentDrawedInterpolation != null;

                //serializing
                _currentTimelineSerialized.ApplyModifiedProperties();

                Repaint();
            }
        }
        #endregion

        public void CreateViews()
        {
            _tracksView = new TracksView();
            _headerView = new HeaderView();
            _keynotesView = new KeynotesView();
            _windowsView = new WindowsView();
            _chaptersHeadsView = new ChaptersHeadsView(_tracksView);

            SubscribeToEditorUpdates();

            Repaint();
            return;
        }

        #region Playback Manipulations
        //subscribing for editor updates to make timeline animate in editor
        public void SubscribeToEditorUpdates()
        {
            if (!_subscribedToUpdates)
            {
                EditorApplication.update += EditorUpdate;
                _lastTime = (float)EditorApplication.timeSinceStartup;

                _subscribedToUpdates = true;
            }
        }

        //maiking timeline updatable in editor mode
        public static void OnEditorUpdate(float deltaTime)
        {
            if (Instance != null)
            {
                if (Instance.Timeline != null)
                {
                    if (Instance.WindowShowed)
                    {
                        _keyCode = KeyCode.None;
                    }
                    Instance.Timeline.EditorUpdate(deltaTime, _keyCode);
                }
            }
            else
            {
                FindWindow();
                return;
            }
        }

        public void Stop()
        {
            _timeline.Stop();
        }

        public void Play()
        {
            _timeline.Play();
        }

        public void Pause()
        {
            _timeline.Pause();
        }

        private void EditorUpdate()
        {
            if (Instance != null)
            {
                float deltaTime = (float)EditorApplication.timeSinceStartup - _lastTime;
                _lastTime = (float)EditorApplication.timeSinceStartup;
                OnEditorUpdate(deltaTime);
            }
        }
        #endregion

        #region Cross-views Methods

        #region Windows
        public bool WindowShowed = false;

        public void ShowWindow(Vector2 clickPosition, MetaKeyframeBase keyframe)
        {
            _windowsView.CurrentDrawedKeyframe = keyframe;
            _windowsView.ClickPosition = clickPosition;
        }

        public void ShowWindow(Vector2 clickPosition, MetaInterpolationBase interpolation)
        {
            _windowsView.CurrentDrawedInterpolation = interpolation;
            _windowsView.ClickPosition = clickPosition;
        }
        #endregion

        public void OnSaveExplosionStateButtonClicked(ModelExploder related)
        {
            MetaKeyframeBase interpolateFrom = null;
            if (TracksSubview.CurrentSelectedKeyframe != null && TracksSubview.CurrentSelectedKeyframe is ModelExploderKeyframe)
            {
                //if we have modelexploder keyframe selected - we make interpolation to it automatically
                interpolateFrom = TracksSubview.CurrentSelectedKeyframe;
            }

            int row = TracksView.SelectedTrack;
            if (row < 0)
            {
                row = 0;
            }

            MetaKeyframeBase newKeyframe = null;
            if (related != null)
            {
                //if we have some gameObject selected - we make keyframe with it as target
                
                newKeyframe = _timeline.AddKeyframeOnTrack(related, row, _timeline.CurrentTime, "");

                related.UpdateInitialTransforms(1);
                //modelExploder.RecalculateAllExplosionTargets(0, 1);
                related.Save(1);

                (newKeyframe as ModelExploderKeyframe).ExplosionSettingsBundleIndex = related.ExplosionSettingsBundles.Count - 1;
            }

            if (interpolateFrom != null && newKeyframe != null)
            {
                _timeline.AddInterpolation(interpolateFrom, newKeyframe, "");
            }
        }

        public void OnSetKeyframeButtonClicked()
        {
            if (TracksSubview.CurrentSelectedKeyframe != null)
            {
                Undo.RecordObject(_timeline, "Keyframe's Data Changed");
                TracksSubview.CurrentSelectedKeyframe.Get();
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    int row = TracksView.SelectedTrack;
                    if (row < 0)
                    {
                        row = 0;
                    }

                    Undo.RecordObject(_timeline, "New Keyframe Added");
                    _timeline.AddKeyframeOnTrack(Selection.activeGameObject.transform, row, _timeline.CurrentTime, "");
                }
            }
        }

        public void NavigateToKeynote(Keynote keynote)
        {
            _tracksView.NavigateToKeynote(keynote);
        }

        public void NavigateToTiming(float timing)
        {
            _tracksView.NavigateToTiming(timing);
        }
        #endregion

        #region Utility Methods
        public static float GetTimingsPosition(float timing)
        {
            float position = 0f;

            if (Instance._timeline.Keynotes != null && Instance._timeline.Keynotes.Count > 0)
            {
                float lastKeynoteTiming = 0f;
                bool defined = false;
                for (int i = 0; i < Instance._timeline.Keynotes.Count; i++)
                {
                    position += (Instance._timeline.Keynotes[i].Timing - lastKeynoteTiming) * Instance._timeline.PixelPerSecond;

                    if (timing < Instance._timeline.Keynotes[i].Timing)
                    {
                        position = position - (Instance._timeline.Keynotes[i].Timing - timing) * Instance._timeline.PixelPerSecond;
                        defined = true;
                        break;
                    }
                    else
                    {
                        position += Instance._timeline.Keynotes[i].Width;
                        lastKeynoteTiming = Instance._timeline.Keynotes[i].Timing;
                    }
                }

                if (!defined)
                {
                    position += (timing - Instance._timeline.Keynotes[Instance._timeline.Keynotes.Count - 1].Timing) * Instance._timeline.PixelPerSecond;
                }
            }
            else
            {
                position = timing * Instance._timeline.PixelPerSecond;
            }

            return position;
        }

        public static float GetPositionsTiming(float position)
        {
            float timing = 0f;
            float pos = 0f;

            if (Instance._timeline.Keynotes != null && Instance._timeline.Keynotes.Count > 0)
            {
                float lastKeynoteTiming = 0f;
                bool defined = false;
                for (int i = 0; i < Instance._timeline.Keynotes.Count; i++)
                {
                    pos += (Instance._timeline.Keynotes[i].Timing - lastKeynoteTiming) * Instance._timeline.PixelPerSecond;

                    if (position <= pos)
                    {
                        timing = Instance._timeline.Keynotes[i].Timing - (pos - position) / Instance._timeline.PixelPerSecond;
                        defined = true;
                        break;
                    }
                    else
                    {
                        pos += Instance._timeline.Keynotes[i].Width;
                        lastKeynoteTiming = Instance._timeline.Keynotes[i].Timing;
                    }
                }

                if (!defined)
                {
                    timing = Instance._timeline.Keynotes[Instance._timeline.Keynotes.Count - 1].Timing + (position - pos) / Instance._timeline.PixelPerSecond;
                }
            }
            else
            {
                timing = position / Instance._timeline.PixelPerSecond;
            }

            return timing;
        }
        #endregion
    }
}
