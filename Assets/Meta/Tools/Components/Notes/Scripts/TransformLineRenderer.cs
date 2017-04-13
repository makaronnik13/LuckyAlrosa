using UnityEngine;

namespace Meta
{
    [RequireComponent(typeof(LineRenderer))]
    public class TransformLineRenderer : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _linePoints;

        [SerializeField]
        private bool _smooth;

        [SerializeField]
        private bool _update;

        [SerializeField]
        private bool _useWorldSpace = true;

        [SerializeField]
        [Range(0.01f, 1f)]
        private float _smoothStep = .1f;

        private AnimationCurve3 _animationCurve3;
        private LineRenderer _lineRenderer;
        private int _vertexCount;

        private void Awake()
        {
            Initialize();
        }

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = _useWorldSpace;
            if (_smooth)
            {
                _animationCurve3 = new AnimationCurve3();
                for (int i = 0; i < _linePoints.Length; ++i)
                {
                    _animationCurve3.AddKey(i, GetPosition(i));
                }
                _vertexCount = (int)((_linePoints.Length - 1) / _smoothStep) + 1;
                if (_vertexCount >= 0)
                {
                    _lineRenderer.SetVertexCount(_vertexCount);
                }
            }
            else
            {
                _lineRenderer.SetVertexCount(_linePoints.Length);
            }
        }

        private void LateUpdate()
        {
            if (_update && _smooth)
            {
                for (int i = 0; i < _linePoints.Length; ++i)
                {
                    _animationCurve3.MoveKey(i, _animationCurve3.Time(i), GetPosition(i), i == 0 || i == 1 ? 1 : 0);
                }
                int index = 0;
                for (float time = 0; time < _animationCurve3.LastTime(); time += _smoothStep)
                {
                    if (index < _vertexCount)
                    {
                        _lineRenderer.SetPosition(index, _animationCurve3.Evaluate(time));
                    }
                    index++;
                }
            }
            else if (_update)
            {
                for (int i = 0; i < _linePoints.Length; ++i)
                {
                    _lineRenderer.SetPosition(i, GetPosition(i));
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Initialize();
                LateUpdate();
            }
        }

        private Vector3 GetPosition(int index)
        {
            return _useWorldSpace ? _linePoints[index].position : transform.InverseTransformPoint(_linePoints[index].position);
        }

        public void SetPositions(Transform[] positions)
        {
            _linePoints = positions;
            Initialize();
        }
    }
}