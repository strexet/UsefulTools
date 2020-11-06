using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public static class ApplySelectedPrefabs
    {
        const int SelectionThresholdForProgressBar = 20;
        static bool showProgressBar;
        static int changedObjectsCount;

        [MenuItem("Tools/UsefulTools/Prefabs/Apply Changes To Selected Prefabs %j", false, 100)]
        static void ApplyPrefabs()
        {
            SearchPrefabConnections(ApplyToSelectedPrefabs);
        }

        [MenuItem("Tools/UsefulTools/Prefabs/Revert Changes Of Selected Prefabs", false, 100)]
        static void ResetPrefabs()
        {
            SearchPrefabConnections(RevertToSelectedPrefabs);
        }

        [MenuItem("Tools/UsefulTools/Prefabs/Apply Changes To Selected Prefabs", true)]
        [MenuItem("Tools/UsefulTools/Prefabs/Revert Changes Of Selected Prefabs", true)]
        static bool IsSceneObjectSelected()
        {
            return Selection.activeTransform != null;
        }

        //Look for connections
        static void SearchPrefabConnections(ChangePrefab changePrefabAction)
        {
            var selectedTransforms = Selection.gameObjects;
            var numberOfTransforms = selectedTransforms.Length;

            showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            changedObjectsCount = 0;

            //Iterate through all the selected gameobjects
            try
            {
                for (var i = 0; i < numberOfTransforms; i++)
                {
                    var go = selectedTransforms[i];
                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Update prefabs",
                            "Updating prefab " + go.name + " (" + i + "/" + numberOfTransforms + ")",
                            i / (float) numberOfTransforms);
                    }

                    IterateThroughObjectTree(changePrefabAction, go);
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                Debug.LogFormat("{0} Prefab(s) updated", changedObjectsCount);
            }
        }

        static void IterateThroughObjectTree(ChangePrefab changePrefabAction, GameObject go)
        {
            var prefabType = PrefabUtility.GetPrefabAssetType(go);

            //Is the selected gameobject a prefab?
            if (prefabType == PrefabAssetType.NotAPrefab
                || prefabType == PrefabAssetType.MissingAsset)
            {
                return;
            }

            var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            if (prefabRoot != null)
            {
                changePrefabAction(prefabRoot);
                changedObjectsCount++;
                return;
            }

            // If not a prefab, go through all children
            var transform = go.transform;
            var children = transform.childCount;
            for (var i = 0; i < children; i++)
            {
                var childGo = transform.GetChild(i).gameObject;
                IterateThroughObjectTree(changePrefabAction, childGo);
            }
        }

        //Apply    
        static void ApplyToSelectedPrefabs(GameObject go)
        {
            var prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);

            if (string.IsNullOrEmpty(prefabAssetPath))
            {
                return;
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabAssetPath, InteractionMode.UserAction);
        }

        //Revert
        static void RevertToSelectedPrefabs(GameObject go)
        {
            PrefabUtility.RevertPrefabInstance(go, InteractionMode.UserAction);
        }

        delegate void ChangePrefab(GameObject go);
    }
}