using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperData
    {
        public int LastSelectedObjectsMaxCount;
        public List<Object> ReferencedObjects;
        public List<Object> LastSelectedObjects;

        public RefHelperData() =>
            SetupEmpty();

        public RefHelperData(RefHelperSaveData saveData)
        {
            if (saveData == null || saveData.ReferencedPaths == null || saveData.LastSelectedPaths == null)
            {
                SetupEmpty();
                return;
            }

            LastSelectedObjectsMaxCount = saveData.LastSelectedObjectsMaxCount;

            ReferencedObjects = new List<Object>(saveData.ReferencedPaths.Length);
            var referencedObjects = saveData.ReferencedPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);
            ReferencedObjects.AddRange(referencedObjects);

            LastSelectedObjects = new List<Object>(saveData.LastSelectedPaths.Length);
            var lastSelectedObjects = saveData.LastSelectedPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);
            ReferencedObjects.AddRange(lastSelectedObjects);
        }

        private void SetupEmpty()
        {
            LastSelectedObjectsMaxCount = 4;
            ReferencedObjects = new List<Object>();
            LastSelectedObjects = new List<Object>();
        }
    }
}