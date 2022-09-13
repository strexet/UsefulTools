using System;
using ME.ECS;
using ME.ECS.Collections;

namespace Khutor.Scripts.Utils.Extensions
{
    public static class BufferArrayExtensions
    {
        public static bool RandomElement<T>(this in BufferArray<T> elements, out T result, Func<T, bool> condition, Func<int, bool> indexCondition = null)
        {
            if (elements.Count == 0)
            {
                result = default;
                return false;
            }

            var array = PoolArray<T>.Spawn(elements.Length);
            var index = 0;

            foreach (T element in elements)
            {
                if (condition(element) && (indexCondition?.Invoke(index) ?? true))
                    array[index++] = element;
            }

            if (index == 0)
            {
                result = default;
                return false;
            }

            result = array[Worlds.currentWorld.GetRandomRange(0, index)];

            array.Dispose();

            return result != null;
        }

        public static int RandomElement<T>(this in BufferArray<T> elements, out T result)
        {
            int length = elements.Length;
            int index = -1;

            if (length == 0)
                result = default;
            else
            {
                index = Worlds.currentWorld.GetRandomRange(0, length);
                result = elements[index];
            }

            return index;
        }
    }
}