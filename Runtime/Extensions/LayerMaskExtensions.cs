using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}