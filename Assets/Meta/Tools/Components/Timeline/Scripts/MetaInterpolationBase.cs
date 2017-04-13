using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class MetaInterpolationBase : MonoBehaviour, ICloneable
    {
#if UNITY_EDITOR
        public class InterpolationThread
        {
            public string Name;
            public Color Color;
        }
#endif

        //Default interpolation rendering parameters
        protected readonly float InterpolationWidth = 8f;

        [NonSerialized]
        public bool IsSelected = false;
        [NonSerialized]
        public bool IsManipulated = false;

        [NonSerialized]
        protected bool _isConflicting = false;
        [NonSerialized]
        public float ConflictStart = 0f;
        [NonSerialized]
        public float ConflictEnd = 0f;



        public bool IsHided = false;

        #region Serializable Fields
#if UNITY_EDITOR
        [SerializeField]
        protected int _controlID;
        [SerializeField]
        protected Color _interpolationBackgroundColor = new Color(209f / 255f, 209f / 255f, 209f / 255f);
        [SerializeField]
        protected string _caption;
        [SerializeField]
        protected Vector2 _startDragingPosition;
        [SerializeField]
        protected List<Rect> _bodyRects = new List<Rect>();
#endif
        #endregion

        #region Protected Fields
        [SerializeField]
        protected MetaKeyframeBase[] _keyframes;

        [SerializeField]
        protected int _trackIndex;
        [SerializeField]
        protected float _timing;

        [SerializeField]
        protected int _startIndex = 0;
        [SerializeField]
        protected int _endIndex = 1;
        #endregion

        #region Public Properties

#if UNITY_EDITOR
        public bool IsConflicting
        {
            get
            {
                return _isConflicting;
            }
            set
            {
                _isConflicting = value;
                if (Keyframes[0].BodyRect.center.x >= ConflictStart && Keyframes[0].BodyRect.center.x <= ConflictEnd)
                {
                    Keyframes[0].IsConflicting = _isConflicting;
                }
                if (Keyframes[1].BodyRect.center.x >= ConflictStart && Keyframes[1].BodyRect.center.x <= ConflictEnd)
                {
                    Keyframes[1].IsConflicting = _isConflicting;
                }
            }
        }
#endif

        public MetaKeyframeBase[] Keyframes
        {
            get
            {
                return _keyframes;
            }
        }

        public UnityEngine.Object Target
        {
            get
            {
                if (Keyframes != null && Keyframes[0] != null)
                {
                    return Keyframes[0].Target;
                }
                return null;
            }
        }

        virtual public Dictionary<string, Action> SubMenues
        {
            get
            {
                return null;
            }
        }

#if UNITY_EDITOR
        virtual public Color InterpolationBackgroundColor
        {
            get
            {
                return _interpolationBackgroundColor;
            }
        }

        public int ControlID
        {
            get
            {
                return _controlID;
            }
            set
            {
                _controlID = value;
            }
        }

        virtual public string ParametersWindowTitle
        {
            get
            {
                return "";
            }
        }

        virtual public float ParametersWindowHeight
        {
            get
            {
                return 0f;
            }
        }

        virtual public float ParametersWindowWidth
        {
            get
            {
                return 0f;
            }
        }

        public List<Rect> BodyRects
        {
            get
            {
                return _bodyRects;
            }
            set
            {
                _bodyRects = value;
            }
        }

        public Vector2 StartDragingPosition
        {
            get
            {
                return _startDragingPosition;
            }
            set
            {
                _startDragingPosition = value;
            }
        }
#endif

        virtual public int ActiveThreadsNumber
        {
            get
            {
                return 0;
            }
        }

        public int TrackIndex
        {
            get
            {
                return _trackIndex;
            }
            set
            {
                _trackIndex = value;
            }
        }
        #endregion

        #region MonoBehaviour Lifecylce
        virtual protected void Awake()
        {

        }
        virtual protected void OnEnable()
        {

        }
        virtual protected void Start()
        {

        }

        virtual protected void FixedUpdate()
        {

        }
        virtual protected void Update()
        {

        }
        virtual protected void LateUpdate()
        {

        }

        virtual protected void OnGUI()
        {

        }

        virtual protected void OnDisable()
        {

        }
        virtual protected void OnDestroy()
        {

        }
        #endregion

        #region Main Methods
        /// <summary>
        /// Interpolation function makes new keyframe wich is interpolated state between current keyframe's state
        /// and nextKeyframe's state at ratio point.
        /// </summary>
        /// <param name="nextKeyframe">Next keyframe</param>
        /// <param name="ratio">Point between 0 and 1</param>
        /// <returns>Interpolated keyframe</returns>
        virtual public MetaKeyframeBase Interpolate(float ratio)
        {
            //Debug.Log("_keyframes[0].Timing = " + _keyframes[0].Timing);
            //Debug.Log("_keyframes[1].Timing = " + _keyframes[1].Timing);
            if (_keyframes[0].Timing > _keyframes[1].Timing)
            {
                _startIndex = 1;
                _endIndex = 0;
            }
            else
            {
                _startIndex = 0;
                _endIndex = 1;
            }

            return null;
        }

        /// <summary>
        /// Initial function, wich is called on creating of keyframe right after it was created
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path">MenuItem from wich it was Invoked</param>
        virtual public void Init(MetaKeyframeBase keyframe1, MetaKeyframeBase keyframe2, string path)
        {
            _keyframes = new MetaKeyframeBase[] { keyframe1, keyframe2 };

#if UNITY_EDITOR
            CreateCaption();
#endif

            Dictionary<string, Action> subMenues = SubMenues;
            if (subMenues != null && subMenues.ContainsKey(path))
            {
                if (subMenues[path] != null)
                {
                    subMenues[path].Invoke();
                }
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Returns Info about currently active interpolation threads
        /// </summary>
        /// <returns></returns>
        virtual public InterpolationThread[] ActiveTreads()
        {
            return null;
        }

        /// <summary>
        /// Draws it's window with pararmeters in work view of Timline Tool
        /// </summary>
        virtual public void DrawParametersWindow()
        {

        }

        /// <summary>
        /// Draws Interpolation on track
        /// </summary>
        /// <param name="interpolationRect">Bounds of the interpolation. X is first keyframe's position, X + Widht - second.
        /// Y and Height are equals to those of track.</param>
        /// <param name="subviewActive">True if subview that this interpolation is located in is active right now</param>
        virtual public void DrawInterpolation(Rect interpolationRect, bool subviewActive)
        {
            float interpolationAreaBeginningY = interpolationRect.y + (interpolationRect.height - InterpolationWidth) / 2f;

            float interpolationColorMultiplier = 1f;
            if (IsManipulated)
            {
                interpolationColorMultiplier = 1.05f;
            }
            else if (IsSelected)
            {
                interpolationColorMultiplier = 1.12f;
            }


            if (Tracks.SelectedTrack == TrackIndex)
            {
                interpolationColorMultiplier *= 1.07f;
            }

            if (!subviewActive)
            {
                interpolationColorMultiplier *= 0.8f;
            }

            Handles.BeginGUI();

            Vector3[] vectors = new Vector3[4] { new Vector3(interpolationRect.x, interpolationAreaBeginningY),
                new Vector3(interpolationRect.x + interpolationRect.width, interpolationAreaBeginningY),
                new Vector3(interpolationRect.x + interpolationRect.width, interpolationAreaBeginningY + InterpolationWidth),
                new Vector3(interpolationRect.x, interpolationAreaBeginningY + InterpolationWidth)};

            Color requiredColor = _interpolationBackgroundColor;
            if (IsHided)
            {
                requiredColor = GetGrayscaleColor(requiredColor);
            }
            Handles.color = requiredColor * interpolationColorMultiplier;

            Handles.DrawAAConvexPolygon(vectors);

            Handles.color = Color.white;

            InterpolationThread[] activeTreads = ActiveTreads();
            
            if (activeTreads != null)
            {
                for (int j = 0; j < activeTreads.Length; j++)
                {
                    float lineHeight = 0f;
                    lineHeight = interpolationAreaBeginningY + (InterpolationWidth / (activeTreads.Length + 1)) * (j + 1);

                    Handles.color = activeTreads[j].Color;
                    Handles.DrawLine(new Vector2(interpolationRect.x, lineHeight), new Vector2(interpolationRect.x + interpolationRect.width, lineHeight));
                }
            }

            Handles.EndGUI();
            
            if (_bodyRects.Count > 2)
            {
                _bodyRects.Clear();
            }

            _bodyRects.Add(Utilities.Encapsulate(new Rect(interpolationRect.x, interpolationAreaBeginningY, 0f, InterpolationWidth), new Rect(interpolationRect.x + interpolationRect.width, interpolationAreaBeginningY, 0f, InterpolationWidth)));

            EditorGUIUtility.AddCursorRect(_bodyRects[_bodyRects.Count - 1], MouseCursor.MoveArrow);
            
            //conflicting states assigning every frame, so it needs autoreturn for false state
            _isConflicting = false;
        }

        virtual protected void CreateCaption()
        {
            _caption = _keyframes[0].GetType().ToString() + " Interpolation";
        }
#endif
        #endregion

        #region Utility Methods
        protected Color GetGrayscaleColor(Color color)
        {
            float average = (color.r + color.g + color.b) / 3f;

            return new Color(average, average, average);
        }
        #endregion

        #region Interfaces Implementations
        virtual public object Clone()
        {
            return null;
        }
        #endregion
    }
}
