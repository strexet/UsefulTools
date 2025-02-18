using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UsefulTools.Runtime.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3 XZ(this Vector3 v) => new Vector3(v.x, 0, v.z);
		public static Vector3 XY(this Vector3 v) => new Vector3(v.x, v.y, 0);
		public static Vector3 YZ(this Vector3 v) => new Vector3(0, v.y, v.z);

		/// <summary>
		/// Sets any x y z values of a Vector3
		/// </summary>
		public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) => new(x ?? vector.x, y ?? vector.y, z ?? vector.z);
		public static Vector3 WithX(this Vector3 v, float value) => new Vector3(value, v.y, v.z);
		public static Vector3 WithY(this Vector3 v, float value) => new Vector3(v.x, value, v.z);
		public static Vector3 WithZ(this Vector3 v, float value) => new Vector3(v.x, v.y, value);

		public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal) => v - Vector3.Project(v, planeNormal);

		public static Vector3 DirectionTo(this Vector3 from, Vector3 to) => from.FromTo(to).normalized;
		public static Vector3 FromTo(this Vector3 from, Vector3 to) => to - from;
		
		public static bool IsShorterThan(this Vector3 v, float value) => v.sqrMagnitude < value * value;
		public static bool IsShorterOrEqualThan(this Vector3 v, float value) => v.sqrMagnitude <= value * value;
		public static bool IsLongerThan(this Vector3 v, float value) => v.sqrMagnitude > value * value;
		public static bool IsLongerOrEqualThan(this Vector3 v, float value) => v.sqrMagnitude >= value * value;
		
		/// <summary>
		/// Adds to any x y z values of a Vector3
		/// </summary>
		public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) {
			return new Vector3(vector.x + x, vector.y + y, vector.z + z);
		}
		
		/// <summary>
		/// Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
		/// </summary>
		/// <param name="current">The current Vector3 position</param>
		/// <param name="target">The Vector3 position to compare against</param>
		/// <param name="range">The range value to compare against</param>
		/// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
		public static bool InRangeOf(this Vector3 current, Vector3 target, float range)
		{
			return current.FromTo(target).IsShorterOrEqualThan(range);
		}

        /// <summary>
        /// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
        /// maximum radius values around a central Vector3 point (origin).
        /// </summary>
        /// <param name="origin">The center Vector3 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random Vector3 point within the specified annulus.</returns>
        public static Vector3 RandomPointInRing(this Vector3 origin, float minRadius, float maxRadius) {
            if (minRadius.Approx(0))
            {
                return origin.RandomPointInCircle(maxRadius);
            }
            
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
            // Squaring and then square-rooting radii to ensure uniform distribution within the ring
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
    
            // Converting the 2D direction vector to a 3D position vector
            Vector3 position = new Vector3(direction.x, 0, direction.y) * distance;
            return origin + position;
        }
        
        public static Vector3 RandomPointInCircle(this Vector3 origin, float radius) {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
            // Squaring and then square-rooting radius to ensure uniform distribution within the circle
            float radiusSquared = radius * radius;
            float distance = Mathf.Sqrt(Random.value * radiusSquared);
    
            // Converting the 2D direction vector to a 3D position vector
            Vector3 position = new Vector3(direction.x, 0, direction.y) * distance;
            return origin + position;
        }
        
        public static Vector3 RandomPointInSphere(this Vector3 origin, float radius) {
	        float angle1 = Random.value * Mathf.PI * 2f;
	        float angle2 = Random.value * Mathf.PI * 2f;
	        Vector3 direction = new Vector3(Mathf.Cos(angle1) * Mathf.Sin(angle2), Mathf.Sin(angle1) * Mathf.Sin(angle2), Mathf.Cos(angle2));
	        
	        float distance = Random.value * radius;
	        Vector3 position = distance * direction;
	        return origin + position;
        }
        
        /// <summary>
        /// Divides two Vector3 objects component-wise.
        /// </summary>
        /// <remarks>
        /// For each component in v0 (x, y, z), it is divided by the corresponding component in v1 if the component in v1 is not zero. 
        /// Otherwise, the component in v0 remains unchanged.
        /// </remarks>
        /// <example>
        /// Use 'ComponentDivide' to scale a game object proportionally:
        /// <code>
        /// myObject.transform.localScale = originalScale.ComponentDivide(targetDimensions);
        /// </code>
        /// This scales the object size to fit within the target dimensions while maintaining its original proportions.
        ///</example>
        /// <param name="v0">The Vector3 object that this method extends.</param>
        /// <param name="v1">The Vector3 object by which v0 is divided.</param>
        /// <returns>A new Vector3 object resulting from the component-wise division.</returns>
        public static Vector3 ComponentDivide(this Vector3 v0, Vector3 v1){
	        return new Vector3( 
		        v1.x != 0 ? v0.x / v1.x : v0.x, 
		        v1.y != 0 ? v0.y / v1.y : v0.y, 
		        v1.z != 0 ? v0.z / v1.z : v0.z);  
        }
	}
}