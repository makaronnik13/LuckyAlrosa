using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class Tracks
    {
        //Mode when each track can only contain keyframes from one particular UnityEndgine.Object
        public static bool ProprietaryMode = true;
        public static int SelectedTrack = -1;
        public static int RowMouseAbove = -1;

        [SerializeField]
        private List<TimelineTrack> _tracks = new List<TimelineTrack>();

        internal List<float> GetAllTimings(MetaKeyframeBase except0 = null, MetaKeyframeBase except1 = null)
        {
            List<float> result = new List<float>();

            for (int i = 0; i < _tracks.Count; i++)
            {
                for (int j = 0; j < _tracks[i].Keyframes.Count; j++)
                {
                    if (_tracks[i].Keyframes[j] != except0 && _tracks[i].Keyframes[j] != except1)
                    {
                        result.Add(_tracks[i].Keyframes[j].Timing);
                    }
                }
            }

            return result;
        }

        public int MaxTracksIndex
        {
            get
            {
                int index = 0;
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (_tracks[i].TracksViewPosition > index)
                    {
                        index = _tracks[i].TracksViewPosition;
                    }
                }
                return index;
            }
        }

        public int GetSoloTrack()
        {
            int index = -1;

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Soloed)
                {
                    index = i;
                }
            }

            return index;
        }

        public void DeSoloAllTracks()
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                _tracks[i].Soloed = false;
            }
        }

        public TimelineTrack TrackOnIndex(int index)
        {
            if (index >= 0 && index < _tracks.Count)
            {
                return _tracks[index];
            }
            else
            {
                return null;
            }
        }

        public int Count
        {
            get
            {
                return _tracks.Count;
            }
        }

        public int NumberOfKeyframes
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < _tracks.Count; i++)
                {
                    counter += _tracks[i].Keyframes.Count;
                }
                return counter;
            }
        }

        public int NumberOfInterpolations
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < _tracks.Count; i++)
                {
                    counter += _tracks[i].Interpolations.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Returns track by it's order in the tracks view
        /// </summary>
        /// <param name="viewTracksPosition"></param>
        /// <returns></returns>
        public TimelineTrack TrackOnTracksViewPosition(int tracksViewPosition)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].TracksViewPosition == tracksViewPosition)
                {
                    return _tracks[i];
                }
            }

            return null;
        }

        public TimelineTrack GetTrackOnTracksViewPosition(int tracksViewPosition)
        {
            if (tracksViewPosition < 0)
            {
                return null;
            }

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].TracksViewPosition == tracksViewPosition)
                {
                    return _tracks[i];
                }
            }

            return null;
        }

        public int GetTracksIndexOnTracksViewPosition(int tracksViewPosition)
        {
            if (tracksViewPosition < 0)
            {
                return -1;
            }

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].TracksViewPosition == tracksViewPosition)
                {
                    return i;
                }
            }

            return -1;
        }

        public TimelineTrack CreateTrackOnTracksViewPosition(int tracksViewPosition)
        {
            TimelineTrack _newTrack = new TimelineTrack();
            _newTrack.TracksViewPosition = tracksViewPosition;
            _newTrack.Name = "New Track";
            _tracks.Add(_newTrack);

            return _newTrack;
        }

        public void CheckInexistingObjects()
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Target == null)
                {
                    _tracks.RemoveAt(i);
                    i--;
                }
            }
        }

        private void CheckUnnessesaryComponents()
        {
            GameObject root = null;
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Keyframes != null && _tracks[i].Keyframes.Count > 0)
                {
                    root = _tracks[i].Keyframes[0].gameObject;
                    break;
                }
            }
            if (root != null)
            {
                MetaInterpolationBase[] interpolations = root.GetComponents<MetaInterpolationBase>();
                MetaKeyframeBase[] keyframes = root.GetComponents<MetaKeyframeBase>();

                for (int i = 0; i < interpolations.Length; i++)
                {
                    bool found = false;
                    for (int j = 0; j < _tracks.Count; j++)
                    {
                        if (_tracks[j].Interpolations.Contains(interpolations[i]))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        UnityEngine.Object.DestroyImmediate(interpolations[i]);
                    }
                }

                for (int i = 0; i < keyframes.Length; i++)
                {
                    bool found = false;
                    for (int j = 0; j < _tracks.Count; j++)
                    {
                        if (_tracks[j].Keyframes.Contains(keyframes[i]))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        UnityEngine.Object.DestroyImmediate(keyframes[i]);
                    }
                }
            }
        }

        public void Swap(TimelineTrack currentlyManipulatedTrack, int highlightedVisiblePosition)
        {
            int index = GetTracksIndexOnTracksViewPosition(highlightedVisiblePosition);
            if (index >= 0)
            {
                _tracks[index].TracksViewPosition = currentlyManipulatedTrack.TracksViewPosition;
            }
            currentlyManipulatedTrack.TracksViewPosition = highlightedVisiblePosition;
        }

        public void AddKeyframeOnTrack(MetaKeyframeBase keyframe, int tracksViewPosition)
        {
            TimelineTrack requiredTrack = GetTrackOnTracksViewPosition(tracksViewPosition);

            if (ProprietaryMode)
            {
                if (requiredTrack == null || requiredTrack.Target == null)
                {
                    for (int i = 0; i < _tracks.Count; i++)
                    {
                        if (_tracks[i].Target == keyframe.Target)
                        {
                            if (keyframe.Target is GameObject)
                            {
                                throw new Exception("Track for " + (keyframe.Target as GameObject).name + " keyframes already exists.");
                            }
                            else if (keyframe.Target is Component)
                            {
                                throw new Exception("Track for " + (keyframe.Target as Component).GetType().Name + " (" + (keyframe.Target as Component).gameObject.name + ") keyframes already exists.");
                            }
                            else
                            {
                                throw new Exception("Keyframes's target has an unknown type");
                            }
                        }
                    }
                    if (requiredTrack == null)
                    {
                        requiredTrack = CreateTrackOnTracksViewPosition(tracksViewPosition);
                    }
                    requiredTrack.Target = keyframe.Target;
                    GameObject targetGO = null;
                    if (requiredTrack.Target is GameObject)
                    {
                        targetGO = requiredTrack.Target as GameObject;
                    }
                    else if (requiredTrack.Target is Component)
                    {
                        targetGO = (requiredTrack.Target as Component).gameObject;
                    }
                    requiredTrack.Name = keyframe.TargetsName + " (" + targetGO.name + ") Track";
                }
                else if (requiredTrack.Target != keyframe.Target)
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
            }

            if (Timeline.HideComponentsMode)
            {
                keyframe.hideFlags = HideFlags.HideInInspector;
            }
            if (requiredTrack != null)
            {
                keyframe.TrackIndex = tracksViewPosition;
                requiredTrack.Add(keyframe);
            }
        }

        public void AddInterpolation(MetaInterpolationBase interpolation)
        {
            if (interpolation.Keyframes[0] != null)
            {
                TimelineTrack requiredTrack = GetTrackOnTracksViewPosition(interpolation.Keyframes[0].TrackIndex);
                if (requiredTrack == null)
                {
                    requiredTrack = CreateTrackOnTracksViewPosition(interpolation.Keyframes[0].TrackIndex);
                }
                if (Timeline.HideComponentsMode)
                {
                    interpolation.hideFlags = HideFlags.HideInInspector;
                }
                if (requiredTrack != null)
                {
                    interpolation.TrackIndex = interpolation.Keyframes[0].TrackIndex;
                    requiredTrack.Add(interpolation);
                }
            }

            CheckUnnessesaryComponents();
        }

        public void DeleteKeyframe(MetaKeyframeBase keyframe)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                for (int j = 0; j < _tracks[i].Interpolations.Count; j++)
                {
                    if (_tracks[i].Interpolations[j].Keyframes[0] == keyframe || _tracks[i].Interpolations[j].Keyframes[1] == keyframe)
                    {
                        //UnityEngine.Object.DestroyImmediate(_tracks[i].Interpolations[j]);
                        _tracks[i].Interpolations.RemoveAt(j);
                        j--;
                    }
                }
            }

            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Keyframes.Contains(keyframe))
                {
                    _tracks[i].Keyframes.Remove(keyframe);
                    //UnityEngine.Object.DestroyImmediate(keyframe);
                    break;
                }
            }
        }

        public void DeleteInterpolation(MetaInterpolationBase interpolation)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].Interpolations.Contains(interpolation))
                {
                    _tracks[i].Interpolations.Remove(interpolation);
                    //UnityEngine.Object.DestroyImmediate(interpolation);
                    break;
                }
            }
        }

        public void DeleteTrackOnTracksViewPosition(int tracksViewPosition)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].TracksViewPosition == tracksViewPosition)
                {
                    /*for (int j = 0; j < _tracks[i].Interpolations.Count; j++)
                    {
                        UnityEngine.Object.DestroyImmediate(_tracks[i].Interpolations[j]);
                    }
                    for (int j = 0; j < _tracks[i].Keyframes.Count; j++)
                    {
                        UnityEngine.Object.DestroyImmediate(_tracks[i].Keyframes[j]);
                    }*/
                    _tracks.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear()
        {
            /*while (_tracks.Count > 0)
            {
                DeleteTrackOnTracksViewPosition(0);
            }*/
        }

#if UNITY_EDITOR
        internal void SavePreviousTimingsStartingFrom(float timing)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                for (int j = 0; j < _tracks[i].Keyframes.Count; j++)
                {
                    if (_tracks[i].Keyframes[j].Timing > timing)
                    {
                        _tracks[i].Keyframes[j].PreviousTiming = _tracks[i].Keyframes[j].Timing;
                    }
                }
            }
        }

        internal void SetTimingsFrom(float timing, float delta)
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                for (int j = 0; j < _tracks[i].Keyframes.Count; j++)
                {
                    if (_tracks[i].Keyframes[j].PreviousTiming > timing)
                    {
                        _tracks[i].Keyframes[j].Timing = _tracks[i].Keyframes[j].PreviousTiming + delta;
                    }
                }
            }
        }
#endif

        internal MetaKeyframeBase GetClosestPreviousKeyframe(MetaKeyframeBase keyframe, int tracksViewPosition = -1)
        {
            MetaKeyframeBase result = null;

            int trackViewPos = tracksViewPosition;
            if (trackViewPos < 0)
            {
                for (int i = 0; i < _tracks.Count; i++)
                {
                    if (_tracks[i].Keyframes.Contains(keyframe))
                    {
                        trackViewPos = _tracks[i].TracksViewPosition;
                        break;
                    }
                }
            }

            if (trackViewPos >= 0)
            {
                TimelineTrack track = GetTrackOnTracksViewPosition(trackViewPos);
                if (track != null)
                {
                    float smallestDelta = -1f;
                    int smallestDeltaIndex = 0;

                    for (int i = 0; i < track.Keyframes.Count; i++)
                    {
                        if (track.Keyframes[i].Timing < keyframe.Timing)
                        {
                            if (smallestDelta < 0f)
                            {
                                smallestDelta = keyframe.Timing - track.Keyframes[i].Timing;
                                smallestDeltaIndex = i;
                            }
                            else if (keyframe.Timing - track.Keyframes[i].Timing < smallestDelta)
                            {
                                smallestDelta = keyframe.Timing - track.Keyframes[i].Timing;
                                smallestDeltaIndex = i;
                            }
                        }
                    }

                    if (smallestDelta >= 0)
                    {
                        result = track.Keyframes[smallestDeltaIndex];
                    }
                }
            }

            return result;
        }
    }
}
