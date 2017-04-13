using UnityEngine;

namespace Meta.Tools
{
    public class Adapter01 : MonoBehaviour, IInputFloatValue, IRangeFloatValues
    {
        [SerializeField]
        private MonoBehaviour _containingMonoBehaviour;

        private I01InputFloatValue _target;
        public I01InputFloatValue Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                if (value != null)
                {
                    _containingMonoBehaviour = value as MonoBehaviour;
                }

                if (_value0 != _value1 && value != null)
                {
                    _setted = true;
                }
                else
                {
                    _setted = false;
                }
            }
        }

        public float InputValue
        {
            set
            {
                if (_setted)
                {
                    //Debug.Log("Adapter01 setter value: " + value);
                    Target.InputValue = (value - Value0)/(Value1 - Value0);
                }
            }
        }

        bool _setted = false;
        private float _value0 = 0f;
        public float Value0
        {
            get
            {
                return _value0;
            }
            set
            {
                _value0 = value;
                if (_value0 != _value1 && _target != null)
                {
                    _setted = true;
                }
                else
                {
                    _setted = false;
                }
            }
        }

        private float _value1 = 1f;
        public float Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                _value1 = value;
                if (_value0 != _value1 && _target != null)
                {
                    _setted = true;
                }
                else
                {
                    _setted = false;
                }
            }
        }
        
        private void Awake()
        {
            if (_target == null)
            {
                _target = _containingMonoBehaviour as I01InputFloatValue;
            }
            /*Debug.Log("Adapter01. _target = " + _target);
            Debug.Log("Adapter01. _value0 = " + _value0);
            Debug.Log("Adapter01. _value1 = " + _value1);*/
            if (_value0 != _value1 && _target != null)
            {
                _setted = true;
            }
            else
            {
                _setted = false;
            }
        }
    }
}
