using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
    public static class GameObjectExtension
    {
        public static bool IsNull(this GameObject go) => ReferenceEquals(go, null);

        public static bool IsNotNull(this GameObject go) => !ReferenceEquals(go, null);
    }
}