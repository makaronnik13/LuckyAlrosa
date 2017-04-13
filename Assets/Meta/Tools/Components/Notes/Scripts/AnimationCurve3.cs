using UnityEngine;

namespace Meta
{
    [System.Serializable]
    public class AnimationCurve3
    {
        [SerializeField]
        private AnimationCurve _x = new AnimationCurve();

        [SerializeField]
        private AnimationCurve _y = new AnimationCurve();

        [SerializeField]
        private AnimationCurve _z = new AnimationCurve();

        public int Length
        {
            get { return _x.length; }
        }

        public void AddKey(float time, Vector3 vector3, float? inTangent, float? outTangent)
        {
            Keyframe xKey = new Keyframe(time, vector3.x);
            Keyframe yKey = new Keyframe(time, vector3.y);
            Keyframe zKey = new Keyframe(time, vector3.z);

            if (inTangent != null)
            {
                xKey.inTangent = inTangent.Value;
                yKey.inTangent = inTangent.Value;
                zKey.inTangent = inTangent.Value;
            }
            if (outTangent != null)
            {
                xKey.outTangent = outTangent.Value;
                yKey.outTangent = outTangent.Value;
                zKey.outTangent = outTangent.Value;
            }

            _x.AddKey(xKey);
            _y.AddKey(yKey);
            _z.AddKey(zKey);
        }

        public void AddKey(float time, Vector3 vector3)
        {
            _x.AddKey(time, vector3.x);
            _y.AddKey(time, vector3.y);
            _z.AddKey(time, vector3.z);
        }

        public void MoveKey(int index, float time, Vector3 vector3, float smooth)
        {
            _x.MoveKey(index, new Keyframe(time, vector3.x));
            _y.MoveKey(index, new Keyframe(time, vector3.y));
            _z.MoveKey(index, new Keyframe(time, vector3.z));
            _x.SmoothTangents(index, smooth);
            _y.SmoothTangents(index, smooth);
            _z.SmoothTangents(index, smooth);
        }

        public Vector3 Evaluate(float time)
        {
            Vector3 vector3 = new Vector3();
            vector3.x = _x.Evaluate(time);
            vector3.y = _y.Evaluate(time);
            vector3.z = _z.Evaluate(time);
            return vector3;
        }

        public float Time(int index)
        {
            return _x.keys[index].time;
        }

        public void RemoveAfterTime(float time)
        {
            int deleteAferIndex = int.MaxValue;
            Keyframe[] keys = _x.keys;
            for (int i = 0; i < keys.Length; ++i)
            {
                if (keys[i].time > time)
                {
                    deleteAferIndex = i - 1;
                    break;
                }
            }
            for (int i = keys.Length - 1; i > deleteAferIndex; --i)
            {
                _x.RemoveKey(i);
                _y.RemoveKey(i);
                _z.RemoveKey(i);
            }
        }

        public float LastTime()
        {
            return _x.keys[_x.length - 1].time;
        }
    }
}