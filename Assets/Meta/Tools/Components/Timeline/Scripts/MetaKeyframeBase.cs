using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Meta.Tools
{
    [Serializable]
    public class MetaKeyframeBase : MonoBehaviour, ICloneable, IComparable<MetaKeyframeBase>
    {
        //Default keyframes rendering parameters
        public static float KeyframeRadius = 8f;
        protected readonly float KeframeRimThickness = 1f;

        [NonSerialized]
        public bool IsSelected = false;
        [NonSerialized]
        public bool IsManipulated = false;
        [NonSerialized]
        public bool IsConflicting = false;

        public bool IsHided = false;

        #region Serializable Fields
#if UNITY_EDITOR
        [SerializeField]
        protected int _controlID;
        [SerializeField]
        protected Color _keyframeColor = Color.gray;
        [SerializeField]
        protected Color _keyframeRimColor = new Color(221f / 255f, 231f / 255f, 243f / 255f);
        [SerializeField]
        protected string _caption;
        [SerializeField]
        protected Vector2 _startDragingPosition;
        [SerializeField]
        protected Rect _bodyRect;
        [SerializeField]
        protected float _previousTiming;
#endif
        #endregion

        #region Private Fields
        [SerializeField]
        protected UnityEngine.Object _target;
        [SerializeField]
        protected int _trackIndex;
        [SerializeField]
        protected float _timing;
        [SerializeField]
        private bool _active = true;
        #endregion

        #region Public Properties
        virtual public UnityEngine.Object Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        virtual public Dictionary<string, Action> SubMenues
        {
            get
            {
                return null;
            }
        }


        virtual public string TargetsName
        {
            get
            {
                return "Void";
            }
        }

#if UNITY_EDITOR
        virtual public Color KeyframeColor
        {
            get
            {
                return _keyframeColor;
            }
        }

        virtual public Color KeyframeRimColor
        {
            get
            {
                return _keyframeRimColor;
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
                return 40f;
            }
        }

        virtual public float ParametersWindowWidth
        {
            get
            {
                return 0f;
            }
        }

        public Rect BodyRect
        {
            get
            {
                return _bodyRect;
            }
            set
            {
                _bodyRect = value;
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

        public float PreviousTiming
        {
            get
            {
                return _previousTiming;
            }
            set
            {
                _previousTiming = value;
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
#endif

        virtual public int AnimationQueuePriority
        {
            get
            {
                return 0;
            }
        }

        public float Timing
        {
            get
            {
                return _timing;
            }

            set
            {
                _timing = value;
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
        /// Initial function, wich is called on creating of keyframe right after it was created
        /// </summary>
        /// <param name="target">Target Object</param>
        /// <param name="path">MenuItem from wich it was Invoked</param>
        virtual public void Init(UnityEngine.Object target, string path)
        {
            _target = target;

            Fetch();

            Dictionary<string, Action> subMenues = SubMenues;
            if (subMenues != null && subMenues.ContainsKey(path))
            {
                if (subMenues[path] != null)
                {
                    subMenues[path].Invoke();
                }
            }
        }

        /// <summary>
        /// Applies state's parameters to target
        /// </summary>
        virtual public void Apply(bool forward = true)
        {
            GameObject gameObject = null;
            if (_target is GameObject)
            {
                gameObject = _target as GameObject;
            }
            else if (_target is Component)
            {
                gameObject = (_target as Component).gameObject;
            }

            if (gameObject != null && gameObject.activeSelf != _active)
            {
                gameObject.SetActive(_active);
            }
        }

        /// <summary>
        /// Records current target's state
        /// </summary>
        virtual public void Fetch()
        {

        }

        public void Set()
        {
            Apply();
        }

        public void Get()
        {
            Fetch();
        }

#if UNITY_EDITOR
        virtual public void OnSelected()
        {

        }

        /// <summary>
        /// Draws it's window with pararmeters in work view of Timline Tool
        /// </summary>
        virtual public void DrawParametersWindow()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("<size=10>Active: </size>", Utilities.TimelineSkin.GetStyle("TimelineRepresentativeStyle"), GUILayout.Width(50));
            bool newActive = EditorGUILayout.Toggle(_active, GUILayout.Width(16));
            if (newActive != _active)
            {
                _active = newActive;
            }

            if (GUILayout.Button("Set"))
            {
                Fetch();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws keyframe on track
        /// </summary>
        /// <param name="keyframeRect">Bounds of the keyframe. Center of bounds represent exact timing position position.
        /// Y and Height are equals to those of track. X is center.x minus Height/2 parameter and Width = Height</param>
        /// <param name="subviewActive">True if subview that this keyframe is located in is active right now</param>
        virtual public void DrawKeyframe(Rect keyframeRect, bool subviewActive)
        {
            //this is default keyframe renderer, that uses only color parameter

            Handles.BeginGUI();

            float rimMultipier = 1f;
            float centerMultipier = 1f;
            if (IsManipulated)
            {
                rimMultipier = 1.2f;
            }
            else if (IsSelected)
            {
                centerMultipier = 1.4f;
                rimMultipier = 1.2f;
            }

            if (!subviewActive)
            {
                centerMultipier *= 0.8f;
                rimMultipier *= 0.8f;
            }

            Rect rect = new Rect(keyframeRect.center.x - KeyframeRadius, keyframeRect.y + (keyframeRect.height - KeyframeRadius * 2f) / 2, KeyframeRadius * 2f, KeyframeRadius * 2f);

            Vector2 center = rect.center;
            Handles.color = _keyframeRimColor * rimMultipier;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(center.x - KeyframeRadius, center.y),
                new Vector3(center.x, center.y - KeyframeRadius),
                new Vector3(center.x + KeyframeRadius, center.y),
                new Vector3(center.x, center.y + KeyframeRadius)});

            Color requiredColor = KeyframeColor;
            if (IsHided || _target == null)
            {
                /*Debug.Log("IsHided");
                Debug.Log("After requiredColor = " + requiredColor);*/
                requiredColor = GetGrayscaleColor(requiredColor);
                /*Debug.Log("After requiredColor = " + requiredColor);*/
            }
            Handles.color = requiredColor * centerMultipier;

            float offset = KeyframeRadius - KeframeRimThickness;
            Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(center.x - offset, center.y),
            new Vector3(center.x, center.y - offset),
            new Vector3(center.x + offset, center.y),
            new Vector3(center.x, center.y + offset)});

            if (IsConflicting)
            {
                float stripeWidth = 0.4f;
                Vector3[] stripe = new Vector3[4];
                stripe[0] = new Vector3(center.x - offset * (0.5f + stripeWidth * 0.5f), center.y + offset * (0.5f - stripeWidth * 0.5f));
                stripe[1] = new Vector3(center.x - offset * (0.5f - stripeWidth * 0.5f), center.y + offset * (0.5f + stripeWidth * 0.5f));
                stripe[2] = new Vector3(center.x + offset * (0.5f + stripeWidth * 0.5f), center.y - offset * (0.5f - stripeWidth * 0.5f));
                stripe[3] = new Vector3(center.x + offset * (0.5f - stripeWidth * 0.5f), center.y - offset * (0.5f + stripeWidth * 0.5f));
                Handles.color = Color.red;
                Handles.DrawAAConvexPolygon(stripe);
            }

            Handles.EndGUI();

            BodyRect = rect;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.MoveArrow);

            //conflicting states assigning every frame, so it needs autoreturn for false state
            IsConflicting = false;
        }

        virtual protected void CreateCaption()
        {
            _caption = _target.GetType().ToString() + " Keyframe";
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
        public int CompareTo(MetaKeyframeBase other)
        {
            if (other == null) return 1;

            int animationQueuePriorityResult = AnimationQueuePriority.CompareTo(other.AnimationQueuePriority);
            if (animationQueuePriorityResult == 0)
            {
                int timingResult = Timing.CompareTo(other.Timing);
                if (timingResult == 0)
                {
                    return TrackIndex.CompareTo(other.TrackIndex);
                }
                else
                {
                    return timingResult;
                }
            }
            else
            {
                return animationQueuePriorityResult;
            }
        }

        virtual public object Clone()
        {
            return null;
        }
        #endregion
    }
}
