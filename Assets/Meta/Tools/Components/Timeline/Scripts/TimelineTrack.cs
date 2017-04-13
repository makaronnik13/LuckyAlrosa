using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class TimelineTrack
    {
        #region Public Fields
        public UnityEngine.Object Target;

        public int TracksViewPosition = -1;
#if UNITY_EDITOR
        public Rect BodyRect;
        public Vector2 StartingManipulationMousePosition;
        public Vector2 CurrentMousePosition;

        public bool InterpolationModeKeyframeAdding = false;
#endif
        #endregion

        #region Serialized Fields
        [SerializeField]
        private string _name;
        [SerializeField]
        private List<MetaKeyframeBase> _keyframes = new List<MetaKeyframeBase>();
        [SerializeField]
        private List<MetaInterpolationBase> _interpolations = new List<MetaInterpolationBase>();

        [SerializeField]
        private bool _soloed = false;
        [SerializeField]
        private bool _hided = false;

        #endregion

        #region Private Fields
        #endregion

        #region Public Properties
        public bool Soloed
        {
            get
            {
                return _soloed;
            }
            set
            {
                _soloed = value;
            }
        }

        public bool Hided
        {
            get
            {
                return _hided;
            }
            set
            {
                _hided = value;

                for (int i = 0; i < _keyframes.Count; i++)
                {
                    _keyframes[i].IsHided = _hided;
                }
                for (int i = 0; i < _interpolations.Count; i++)
                {
                    _interpolations[i].IsHided = _hided;
                }
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

        public List<MetaKeyframeBase> Keyframes
        {
            get
            {
                return _keyframes;
            }
        }

        public List<MetaInterpolationBase> Interpolations
        {
            get
            {
                return _interpolations;
            }
        }
        #endregion

        #region Interface Methods

        public MetaInterpolationBase Add(MetaInterpolationBase interpolation)
        {
            _interpolations.Add(interpolation);

            return interpolation;
        }

        public MetaKeyframeBase Add(MetaKeyframeBase keyframe)
        {
            _keyframes.Add(keyframe);

            return keyframe;
        }

        public bool Remove(MetaKeyframeBase keyframe)
        {
            if (_keyframes.Contains(keyframe))
            {
                for (int i = 0; i < _interpolations.Count; i++)
                {
                    for (int j = 0; j < _interpolations[i].Keyframes.Length; j++)
                    {
                        if (_interpolations[i].Keyframes[j] == keyframe)
                        {
                            UnityEngine.Object.DestroyImmediate(_interpolations[i]);
                            _interpolations.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }

                _keyframes.Remove(keyframe);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(MetaInterpolationBase interpolation)
        {
            if (_interpolations.Contains(interpolation))
            {
                _interpolations.Remove(interpolation);

                return true;
            }
            else
            {
                return false;
            }
        }

        /*public TimelineKeyframe Add(Component target, float timing)
        {
            TimelineKeyframe newKeyframe = new TimelineKeyframe(target);
            newKeyframe.Timing = timing;
            _keyframes.Add(newKeyframe);

            return newKeyframe;
        }

        public TimelineInterpolation Add(TimelineKeyframe key1, TimelineKeyframe key2)
        {
            if (!key1.IsCompatible(key2))
            {
                return null;
            }

            TimelineInterpolation newInterpolation = new TimelineInterpolation(key1, key2);
            _interpolations.Add(newInterpolation);

            return newInterpolation;
        }

        public TimelineInterpolation Add(TimelineInterpolation interpolation)
        {
            _interpolations.Add(interpolation);

            return interpolation;
        }

        public bool Remove(TimelineKeyframe keyframe)
        {
            if (_keyframes.Contains(keyframe))
            {
                for (int i = 0; i < _interpolations.Count; i++)
                {
                    for (int j = 0; j < _interpolations[i].Keyframes.Length; j++)
                    {
                        if (_interpolations[i].Keyframes[j] == keyframe)
                        {
                            _interpolations.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }

                _keyframes.Remove(keyframe);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(TimelineInterpolation interpolation)
        {
            if (_interpolations.Contains(interpolation))
            {
                _interpolations.Remove(interpolation);

                return true;
            }
            else
            {
                return false;
            }
        }*/
        #endregion
    }
}