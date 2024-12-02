using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.Tools
{
    public static class PrefabAsChildrenPaster
    {
        public static GameObject[] PastePrefabsAsChildren(GameObject[] clonedObjects, Transform parentTransform,
            int numberOfInserts)
        {
            var clones = new List<GameObject>();

            foreach (var clonedObject in clonedObjects)
            {
                if (IsPrefabFromHierarchy(clonedObject))
                {
                    RevertPrefabInstance(clonedObject);
                    Debug.LogWarning("Reverted Prefab Instance!", clonedObject);
                }

                for (int i = 0; i < numberOfInserts; i++)
                {
                    InstantiatedFrom instantiatedFrom;
                    var clone = InstantiateClone(clonedObject, out instantiatedFrom);

                    if (instantiatedFrom == InstantiatedFrom.PrefabFromAssets
                        || instantiatedFrom == InstantiatedFrom.PrefabFromHierarchy)
                    {
                        RevertAndReparentPrefabInstance(clone, parentTransform);
                    }
                    else
                    {
                        ReparentAndResetTransformOfClone(clone, parentTransform);
                    }

                    clones.Add(clone);

                    Undo.RegisterCreatedObjectUndo(clone, "Insert Cloned Object");
                }
            }

            return clones.ToArray();
        }

        private static bool IsPrefabFromHierarchy(GameObject prefabInstance)
        {
            InstantiatedFrom instantiatedFrom;
            var clone = InstantiateClone(prefabInstance, out instantiatedFrom);
            Object.DestroyImmediate(clone);

            if (instantiatedFrom == InstantiatedFrom.PrefabFromHierarchy)
            {
                return true;
            }

            return false;
        }

        private static void ReparentAndResetTransformOfClone(GameObject clone, Transform parent)
        {
            clone.transform.SetParent(parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.transform.localRotation = Quaternion.identity;
        }

        private static GameObject InstantiateClone(GameObject clonedObject, out InstantiatedFrom instantiated)
        {
            var clone = PrefabUtility.InstantiatePrefab(clonedObject) as GameObject;

            if (clone)
            {
                instantiated = InstantiatedFrom.PrefabFromAssets;
                return clone;
            }

            var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(clonedObject);
            if (prefabParent)
            {
                clone = PrefabUtility.InstantiatePrefab(prefabParent) as GameObject;
                instantiated = InstantiatedFrom.PrefabFromHierarchy;
                return clone;
            }

            clone = Object.Instantiate(clonedObject);
            clone.name = clonedObject.name;
            instantiated = InstantiatedFrom.NotPrefab;
            return clone;
        }

        private static void RevertAndReparentPrefabInstance(GameObject prefabInstance, Transform parent)
        {
            RevertPrefabInstance(prefabInstance);
            prefabInstance.transform.SetParent(parent);
        }

        private static void RevertPrefabInstance(GameObject prefabInstance)
        {
            PrefabUtility.RevertPrefabInstance(prefabInstance, InteractionMode.AutomatedAction);
        }

        private enum InstantiatedFrom
        {
            NotPrefab = 10,
            PrefabFromAssets = 20,
            PrefabFromHierarchy = 30
        }
    }
}