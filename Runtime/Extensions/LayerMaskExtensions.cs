using UnityEngine;

namespace UsefulTools.Runtime.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool ContainsLayer(this LayerMask mask, int layer) => mask == (mask | (1 << layer));
    }
}