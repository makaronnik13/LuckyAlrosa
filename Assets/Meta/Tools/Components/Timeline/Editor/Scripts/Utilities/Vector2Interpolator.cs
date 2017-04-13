using System;
using UnityEngine;

namespace Meta.Tools.Editor
{
    [Serializable]
    public class Vector2Interpolator
    {
        private const float _smallNumber = 0.00003f;

        public Action<Vector2> _newVector2;

        public RectOffset Bounds;
        public Vector2 CurrentVector2;

        public bool SmoothLerpToTarget = false;
        public float SmoothPositionLerpRatio = 0.5f;
        public float PositionPerSecond = 5.0f;

        [SerializeField]
        private Vector2 _targetPosition;
        [SerializeField]
        private bool _animatingPosition;


        public void SetTargetVector2(Vector2 target)
        {
            _targetPosition = target;

            float sqrMagn = (_targetPosition - CurrentVector2).sqrMagnitude;
            if (sqrMagn > _smallNumber)
            {
                _animatingPosition = true;
            }
            else
            {
                if (_newVector2 != null)
                {
                    _newVector2.Invoke(_targetPosition);
                }
                _animatingPosition = false;
            }
        }

        public void Update(float deltaTime)
        {
            if (_animatingPosition && deltaTime > 0f)
            {
                Vector2 lerpTargetPosition = _targetPosition;
                if (SmoothLerpToTarget)
                {
                    lerpTargetPosition = Vector2.Lerp(CurrentVector2, lerpTargetPosition, SmoothPositionLerpRatio);
                }

                Vector2 newPosition = interpolateTo(CurrentVector2, lerpTargetPosition, deltaTime, PositionPerSecond);

                float sqrMagn = (_targetPosition - newPosition).sqrMagnitude;
                if (sqrMagn <= _smallNumber)
                {
                    if (_newVector2 != null)
                    {
                        _newVector2.Invoke(_targetPosition);
                    }
                    _animatingPosition = false;
                }
                else
                {
                    if (_newVector2 != null)
                    {
                        _newVector2.Invoke(newPosition);
                    }
                }
            }
        }

        private static Vector2 interpolateTo(Vector2 start, Vector2 target, float deltaTime, float speed)
        {
            if (speed <= 0.0f)
            {
                return target;
            }

            Vector2 distance = (target - start);

            if (distance.sqrMagnitude <= Mathf.Epsilon)
            {
                return target;
            }

            Vector2 deltaMove = distance * Mathf.Clamp(deltaTime * speed, 0.0f, 1.0f);

            return start + deltaMove;
        }
    }
}
