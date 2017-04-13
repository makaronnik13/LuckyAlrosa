using System;
using UnityEngine;

namespace Meta.Tools
{
    public class Tweener : MonoBehaviour, IInputFloatValue, IRangeFloatValues
    {
        public enum LoopTypes
        {
            None,
            Regular,
            Once,
            PingPong
        }

        [SerializeField]
        private MonoBehaviour _containingMonoBehaviour;
        public IInputFloatValue _target;
        public IInputFloatValue Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                //Debug.Log("_target setted");
                if (value != null)
                {
                    //Debug.Log("_target != null");
                    _containingMonoBehaviour = value as MonoBehaviour;
                }
                else
                {
                    //Debug.Log("_target == null");
                }
            }
        }

        public float InputValue
        {
            set
            {
                //throw new NotImplementedException();
            }
        }

        public float Value0
        {
            get
            {
                return From;
            }

            set
            {
                From = value;
            }
        }

        public float Value1
        {
            get
            {
                return To;
            }

            set
            {
                To = value;
            }
        }

        void Awake()
        {
            if (_target == null)
            {
                _target = _containingMonoBehaviour as IInputFloatValue;
            }
        }

        public bool AutoStart = true;
        public float From = 0f;
        public float To = 1f;
        public float Time = 3f;
        public LeanTweenType EaseType = LeanTweenType.easeInOutCubic;
        public float Delay = 1f;
        public LoopTypes LoopType = LoopTypes.PingPong;
        public int LoopsCount = 0;

        void Start()
        {
            if (_target != null && AutoStart)
            {
                LTDescr descr = LeanTween.value(gameObject, From, To, Time).setEase(EaseType).setDelay(Delay).setOnStart(() =>
                {
                    if (_target is IRangeFloatValues)
                    {
                        (_target as IRangeFloatValues).Value0 = From;
                        (_target as IRangeFloatValues).Value1 = To;
                    }
                }).setOnUpdate((float val) =>
                {
                    _target.InputValue = val;
                });

                switch (LoopType)
                {
                    case LoopTypes.Once:
                        descr.setLoopOnce();
                        break;
                    case LoopTypes.PingPong:
                        descr.setLoopPingPong();
                        break;
                    case LoopTypes.Regular:
                        descr.setLoopCount(LoopsCount);
                        break;
                }
            }
        }
    }
}
