using UnityEngine;

namespace UsefulTools.Runtime
{
    public static class FastMath
    {
        public static float Remap(float inValue, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (outMax - outMin) * ((inValue - inMin) / (inMax - inMin));
        }

        public static float RemapClamped(float inValue, float inMin, float inMax, float outMin, float outMax)
        {
            float remappedValue = Remap(inValue, inMin, inMax, outMin, outMax);
            return Mathf.Clamp(remappedValue, outMin, outMax);
        }

        public static bool LayerInMask(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) == 1 << layer;
        }
    }
}