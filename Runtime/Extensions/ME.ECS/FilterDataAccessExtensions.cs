using System;
using ME.ECS;
using ME.ECS.Collections;

namespace Khutor.Scripts.Utils.Extensions
{
    public static class FilterDataAccessExtensions
    {
        public static int AmountByCondition(this in Filter filter, Func<Entity, bool> condition)
        {
            int result = 0;

            foreach (var houseEntity in filter)
            {
                if (condition(houseEntity))
                    result++;
            }

            return result;
        }

        public static BufferArray<Entity> ArrayByCondition(this in Filter filter, Func<Entity, bool> condition)
        {
            int capacity = 0;
            var array = PoolArray<Entity>.Spawn(0, true);

            foreach (var entity in filter)
            {
                if (!condition(entity))
                    continue;

                ArrayUtils.Resize(capacity, ref array);
                array[capacity] = entity;
                capacity++;
            }

            return array;
        }

        public static bool RandomElement(this in Filter filter, out Entity result)
        {
            if (filter.Count == 0)
            {
                result = Entity.Empty;
                return false;
            }

            var array = filter.ToArray();
            result = array[Worlds.currentWorld.GetRandomRange(0, filter.Count)];
            array.Dispose();

            return !result.IsEmpty() && result.IsAlive();
        }

        public static bool First(this in Filter filter, out Entity result)
        {
            if (filter.Count == 0)
            {
                result = Entity.Empty;
                return false;
            }

            var array = filter.ToArray();
            result = array[0];
            array.Dispose();

            return !result.IsEmpty() && result.IsAlive();
        }
        
        public static bool First(this in Filter filter, out Entity result, Func<Entity, bool> condition)
        {
            if (filter.Count == 0)
            {
                result = Entity.Empty;
                return false;
            }
            
            foreach (var entity in filter)
            {
                if (entity.IsEmpty() || !entity.IsAlive())
                    continue;

                if (condition(entity))
                {
                    result = entity;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static bool RandomElementWithCondition(this in Filter filter, out Entity result, Func<Entity, bool> condition)
        {
            BufferArray<Entity> results = PoolArray<Entity>.Spawn(filter.Count, true);
            int index = 0;
            foreach (Entity entity in filter)
            {
                if (entity.IsEmpty() || !entity.IsAlive())
                    continue;

                if (condition(entity))
                {
                    results.arr[index] = entity;
                    index++;
                }
            }

            var count = index;

            if (count == 0)
            {
                result = Entity.Empty;
                return false;
            }

            var resultIndex = Worlds.currentWorld.GetRandomRange(0, count);
            result = results.arr[resultIndex];

            PoolArray<Entity>.Recycle(results);

            return !result.IsEmpty() && result.IsAlive();
        }
    }
}