using LDOE.Features.Foundation.Physics.Components;
using ME.ECS;
using ME.ECS.Transform;

namespace Khutor.Scripts.Utils.Extensions
{
    public static class EntityExtensions
    {
#if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static float DistanceTo(this in Entity entity, in Entity target)
        {
            return TargetDistance(in entity, in target);
        }
        
#if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static float TargetDistance(in Entity entity, in Entity target)
        {
            return ColliderSize(in entity) + ColliderSize(in target);
        }

#if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static float ColliderSize(in Entity entity)
        {
            if (entity.Has<BoxCollider>())
            {
                return entity.Read<BoxCollider>().size.x / 2;
            }

            if (entity.Has<CircleCollider>())
            {
                return entity.Read<CircleCollider>().radius;
            }

            return 0.5f;
        }
        
        
        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public static void SetParentAndPosition2D(this in Entity child, in Entity root) {

            child.SetParentAndPosition2D(in root, worldPositionStays: true);

        }
        
        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public static void SetParentAndPosition2D(this in Entity child, in Entity root, bool worldPositionStays) {

            if (worldPositionStays == true) {

                var pos = root.GetLocalPosition2D();
                var rot = root.GetLocalRotation2D();
                SetParent_INTERNAL(in child, in root);
                child.SetLocalPosition2D(pos);
                child.SetLocalRotation2D(rot);

            } else {

                SetParent_INTERNAL(in child, in root);

            }

        }
        
        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        private static void SetParent_INTERNAL(in Entity child, in Entity root) {

            if (child == root) return;

            ref var container = ref child.Get<Container>();
            if (root == Entity.Empty) {

                ref var childs = ref container.entity.Get<Childs>();
                child.Remove<Container>();
                childs.childs.Remove(child);
                return;

            }

            if (container.entity == root || root.IsAlive() == false) {

                return;

            }

            if (FindInHierarchy(in child, in root) == true) return;

            if (container.entity.IsAlive() == true) {

                child.SetParent(Entity.Empty);

            }

            container.entity = root;
            ref var rootChilds = ref root.Get<Childs>();
            rootChilds.childs.Add(child);
        }
        
        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        private static bool FindInHierarchy(in Entity child, in Entity root) {

            ref var childChilds = ref child.Get<Childs>(createIfNotExists: false);
            if (childChilds.childs.Contains(root) == true) {

                return true;

            }

            foreach (var cc in childChilds.childs) {

                if (FindInHierarchy(in cc, in root) == true) return true;

            }

            return false;

        }

    }
}