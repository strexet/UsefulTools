using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
    public static class GameObjectExtension
    {
        public static bool IsNull(this GameObject go)
        {
            return ReferenceEquals(go, null);
        }

        public static bool IsNotNull(this GameObject go)
        {
            return !ReferenceEquals(go, null);
        }
    }

    public static class MonoBehaviorExtension
    {
        public static bool IsNull(this MonoBehaviour m)
        {
            return ReferenceEquals(m, null);
        }

        public static bool IsNotNull(this MonoBehaviour m)
        {
            return !ReferenceEquals(m, null);
        }
    }
}