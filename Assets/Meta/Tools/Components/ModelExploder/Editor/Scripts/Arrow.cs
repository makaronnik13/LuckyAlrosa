using UnityEngine;
using UnityEditor;
using System;

namespace Meta.Tools
{
    [Serializable]
    public class Arrow
    {
        private Vector3 _start, _end;
        private float _length;
        private Color _color;
        private Quaternion _rotation;
        private bool _selected;
        private int _hash;

        public Arrow(Vector3 start, Vector3 end)
        {
            _start = start;
            _end = end;
            _color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            calkLength();
            calcRotation();
        }

        public Arrow(Vector3 start, Vector3 end, Color color)
        {
            _start = start;
            _end = end;
            _color = color;
            calkLength();
            calcRotation();
        }

        public Vector3 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector3 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Quaternion Rotation
        {
            get { return calcRotation(); }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public int Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

        public Vector3 Center
        {
            get
            {
                Vector3 sum = Vector3.zero;
                sum += _start;
                sum += _end;

                return sum / 2;
            }
        }

        private Quaternion calcRotation()
        {
            _rotation = Quaternion.LookRotation(_end - _start);
            return _rotation;
        }

        private float calkLength()
        {
            _length = (_end - _start).magnitude;

            return _length;
        }

        /// <summary>
        ///     Draw single arrow
        /// </summary>
        public void Draw(int index, float dist)
        {
            _hash = ("arrow" + index).GetHashCode();
            calkLength();
            calcRotation();
            Handles.lighting = true;
            Handles.color = !_selected ? _color : Color.yellow;
            Handles.DrawLine(_start, _end);
            Handles.ConeCap(_hash, _end, _rotation, dist / 25);
        }
    }
}