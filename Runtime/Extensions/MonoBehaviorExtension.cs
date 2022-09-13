using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
    public static class MonoBehaviorExtension
    {
        public static bool IsNull(this MonoBehaviour m) => ReferenceEquals(m, null);

        public static bool IsNotNull(this MonoBehaviour m) => !ReferenceEquals(m, null);
    }
}