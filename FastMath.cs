using UnityEngine;

namespace UsefulTools {
	public static class FastMath {
		public static float Remap(float inValue, float inMin, float inMax, float outMin, float outMax) {
			return outMin + (outMax - outMin) * ((inValue - inMin) / (inMax - inMin));
		}
		
		public static bool LayerInMask(this LayerMask mask, int layer) {
			return (mask.value & (1 << layer)) == 1 << layer;
		}
	}
}