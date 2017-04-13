
using System;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public class RangeFloatValues : IRangeFloatValues
    {
        [SerializeField]
        private float value0 = 0f;
        public float Value0
        {
            get
            {
                return value0;
            }

            set
            {
                value0 = value;
            }
        }

        [SerializeField]
        private float value1 = 1f;
        public float Value1
        {
            get
            {
                return value1;
            }

            set
            {
                value1 = value;
            }
        }
    }
    [Serializable]
    public class ScriptableRangeFloatValues : ScriptableObject, IRangeFloatValues
    {
        [SerializeField]
        private float value0 = 0f;
        public float Value0
        {
            get
            {
                return value0;
            }

            set
            {
                value0 = value;
            }
        }

        [SerializeField]
        private float value1 = 1f;
        public float Value1
        {
            get
            {
                return value1;
            }

            set
            {
                value1 = value;
            }
        }
    }
}
