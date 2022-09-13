using System;
using Random = UnityEngine.Random;

namespace UsefulTools.Runtime.DataStructures.Floats
{
    [Serializable]
    public struct FloatWithSpread : IRandomFloat, IScalable<FloatWithSpread>
    {
        public float value;
        public float spread;

        public float ValueInRange => Random.Range(value - spread, value + spread);
        public float Average => value;

        public FloatWithSpread(float value)
        {
            this.value = value;
            spread = 0;
        }

        public FloatWithSpread(float value, float spread)
        {
            this.value = value;
            this.spread = spread;
        }

        public FloatWithSpread Scale(float scale)
        {
            float valueOut = value * scale;
            float spreadOut = spread * scale;

            var floatWithSpread = new FloatWithSpread(valueOut, spreadOut);

            return floatWithSpread;
        }

        public override string ToString() => $"[{value} (+{spread})]";

        public static implicit operator FloatWithSpread(float value) => new(value);
    }
}