using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3 XZ(this Vector3 v) => new Vector3(v.x, 0, v.z);
		public static Vector3 XY(this Vector3 v) => new Vector3(v.x, v.y, 0);
		public static Vector3 YZ(this Vector3 v) => new Vector3(0, v.y, v.z);
		
		public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal) => v - Vector3.Project(v, planeNormal);
		
		public static Vector3 DirectionTo(this Vector3 from, Vector3 to) => (to - from).normalized;
		public static Vector3 UnnormalizedDirectionTo(this Vector3 from, Vector3 to) => to - from;
	}
}