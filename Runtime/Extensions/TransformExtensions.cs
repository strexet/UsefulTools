using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Runtime.Extensions {
    [Serializable]
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public TransformData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        public TransformData LerpTo(TransformData other, float percentage)
        {
            var betweenPosition = Vector3.Lerp(position, other.position, percentage);
            var betweenRotation = Quaternion.Lerp(rotation, other.rotation, percentage);

            return new TransformData(betweenPosition, betweenRotation);
        }

        public override string ToString() => $"P: {position}; R: {rotation.eulerAngles};";
    }
    
    public static class TransformExtensions {
        public static TransformData GetData(this Transform transform) => new TransformData(transform);

        public static void SetData(this Transform transform, TransformData transformData)
        {
            transform.position = transformData.position;
            transform.rotation = transformData.rotation;
        }

        public static void CopyDataFrom(this Transform target, Transform source)
        {
            var data = source.GetData();
            target.SetData(data);
        }

        public static void CopyDataTo(this Transform source, Transform target) => target.CopyDataFrom(source);
        
        
        /// <summary>
        /// Check if the transform is within a certain distance and optionally within a certain angle (FOV) from the target transform.
        /// </summary>
        /// <param name="source">The transform to check.</param>
        /// <param name="target">The target transform to compare the distance and optional angle with.</param>
        /// <param name="maxDistance">The maximum distance allowed between the two transforms.</param>
        /// <param name="maxAngle">The maximum allowed angle between the transform's forward vector and the direction to the target (default is 360).</param>
        /// <returns>True if the transform is within range and angle (if provided) of the target, false otherwise.</returns>
        public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle = 360f) {
            Vector3 directionToTarget = source.position.FromTo(target.position).WithY(0);
            return directionToTarget.IsShorterOrEqualThan(maxDistance) && Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
        }
        
        /// <summary>
        /// Retrieves all the children of a given Transform.
        /// </summary>
        /// <remarks>
        /// This method can be used with LINQ to perform operations on all child Transforms. For example,
        /// you could use it to find all children with a specific tag, to disable all children, etc.
        /// Transform implements IEnumerable and the GetEnumerator method which returns an IEnumerator of all its children.
        /// </remarks>
        /// <param name="parent">The Transform to retrieve children from.</param>
        /// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>    
        public static IEnumerable<Transform> Children(this Transform parent) {
            foreach (Transform child in parent) {
                yield return child;
            }
        }

        /// <summary>
        /// Resets transform's position, scale and rotation
        /// </summary>
        /// <param name="transform">Transform to use</param>
        public static void ResetToLocal(this Transform transform) {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        public static void ResetToWorld(this Transform transform)
        {
            var parent = transform.parent;
            transform.SetParent(null);
            transform.ResetToLocal();
            transform.SetParent(parent);
        }
        
        /// <summary>
        /// Destroys all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be destroyed.</param>
        public static void DestroyChildren(this Transform parent) {
            parent.ForEveryChild(child => Object.Destroy(child.gameObject));
        }

        /// <summary>
        /// Immediately destroys all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be immediately destroyed.</param>
        public static void DestroyChildrenImmediate(this Transform parent) {
            parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
        }

        /// <summary>
        /// Enables all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be enabled.</param>
        public static void EnableChildren(this Transform parent) {
            parent.ForEveryChild(child => child.gameObject.SetActive(true));
        }

        /// <summary>
        /// Disables all child game objects of the given transform.
        /// </summary>
        /// <param name="parent">The Transform whose child game objects are to be disabled.</param>
        public static void DisableChildren(this Transform parent) {
            parent.ForEveryChild(child => child.gameObject.SetActive(false));
        }

        /// <summary>
        /// Executes a specified action for each child of a given transform.
        /// </summary>
        /// <param name="parent">The parent transform.</param>
        /// <param name="action">The action to be performed on each child.</param>
        /// <remarks>
        /// This method iterates over all child transforms in reverse order and executes a given action on them.
        /// The action is a delegate that takes a Transform as parameter.
        /// </remarks>
        public static void ForEveryChild(this Transform parent, System.Action<Transform> action) {
            for (var i = parent.childCount - 1; i >= 0; i--) {
                action(parent.GetChild(i));
            }
        }

        [Obsolete("Renamed to ForEveryChild")]
        static void PerformActionOnChildren(this Transform parent, System.Action<Transform> action) {
            parent.ForEveryChild(action);
        }
    }
}
