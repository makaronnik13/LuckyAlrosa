using System;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class TimeLineClip
    {
        public bool LoopClip = false;

        public float time;
		public float offset;

        [SerializeField]
        private GameObject _go;
        public GameObject Go
        {
            get
            {
                return _go;
            }

            set
            {
                _go = value;
            }
        }

        public float Speed = 1;

        public MetaKeyframeBase keyframe;

        [SerializeField]
        public AnimationClip _clip;
        public AnimationClip Clip
        {
            get
            {
                return _clip;
            }

            set
            {
                _clip = value;
            }
        }

        public TimeLineClip(GameObject go, AnimationClip clip, MetaKeyframeBase kf)
        {
            _go = go;
            _clip = clip;
            keyframe = kf;
        }


        public void SampleClip(float timing, float speed)
        {
			time = (timing - keyframe.Timing)*speed+offset;
            if (Clip)
            {
                if (LoopClip)
                {
                    time = time % Clip.length;
                }
                Clip.SampleAnimation(Go, time);
            }
        }
    }
}