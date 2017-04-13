using System;

namespace Meta.Tools
{
    [Serializable]
    public class Range01FloatValues : IRange01FloatValues, ICloneable
    {
        private float value0 = 0f;
        public float Value0
        {
            get
            {
                return value0;
            }

            set
            {
                if (value > 1f)
                {
                    value0 = 1f;
                }
                else if (value < 0f)
                {
                    value0 = 0f;
                }
                else
                {
                    value0 = value;
                }
            }
        }

        private float value1 = 1f;
        public float Value1
        {
            get
            {
                return value1;
            }

            set
            {
                if (value > 1f)
                {
                    value1 = 1f;
                }
                else if (value < 0f)
                {
                    value1 = 0f;
                }
                else
                {
                    value1 = value;
                }
            }
        }

        public object Clone()
        {
            Range01FloatValues clone = new Range01FloatValues();

            clone.value0 = value0;
            clone.value1 = value1;

            return clone;
        }
    }
}
