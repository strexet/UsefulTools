using System;
using Random = UnityEngine.Random;

namespace Tartaria.Codebase.DataStructures.Floats
{
    [Serializable]
    public struct FloatMinMax : IRandomFloat, IScalable<FloatMinMax>
    {
        public float min;
        public float max;

        public float ValueInRange => Random.Range(min, max);
        public float Average => 0.5f * (min + max);

        public FloatMinMax(float value)
        {
            min = value;
            max = value;
        }

        public FloatMinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public FloatMinMax Scale(float scale)
        {
            return new FloatMinMax(min * scale, max * scale);
        }

        public override string ToString()
        {
            return $"[{min}; {max}])";
        }

        public static implicit operator FloatMinMax(float value)
        {
            return new FloatMinMax(value);
        }
    }
}