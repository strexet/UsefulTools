using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperSaveData
    {
        public int LastSelectedObjectsMaxCount;
        public string[] ReferencedPaths;
        public string[] LastSelectedPaths;

        public RefHelperSaveData(RefHelperData data)
        {
            LastSelectedObjectsMaxCount = data.LastSelectedObjectsMaxCount;
            ReferencedPaths = GetPathsArray(data.ReferencedObjects);
            LastSelectedPaths = GetPathsArray(data.LastSelectedObjects);
        }

        private static string[] GetPathsArray(IEnumerable<Object> objects) =>
            objects.Select(AssetDatabase.GetAssetPath)
               .Where(path => !string.IsNullOrEmpty(path))
               .ToArray();
    }
}