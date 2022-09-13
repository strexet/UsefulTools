using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperSaveLoadRaw : IRefHelperSaveLoad
    {
        private const string RefHelperSaveKey = "UsefulTools.RefHelper.Data";

        public void SaveData(RefHelperData data)
        {
            var saveData = new RefHelperSaveData(data);
            string dataString = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(RefHelperSaveKey, dataString);
        }

        public RefHelperData LoadData()
        {
            string dataString = PlayerPrefs.GetString(RefHelperSaveKey);
            var saveData = JsonUtility.FromJson<RefHelperSaveData>(dataString);

            return CreateRefHelperData(saveData);
        }

        private static RefHelperData CreateRefHelperData(RefHelperSaveData saveData)
        {
            var refHelperData = new RefHelperData();

            if (saveData == null || saveData.ReferencedPaths == null || saveData.LastSelectedPaths == null)
            {
                return refHelperData;
            }

            refHelperData.LastSelectedObjectsMaxCount = saveData.LastSelectedObjectsMaxCount;

            refHelperData.ReferencedObjects = new List<Object>(saveData.ReferencedPaths.Length);
            var referencedObjects = saveData.ReferencedPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);
            refHelperData.ReferencedObjects.AddRange(referencedObjects);

            refHelperData.LastSelectedObjects = new List<Object>(saveData.LastSelectedPaths.Length);
            var lastSelectedObjects = saveData.LastSelectedPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);
            refHelperData.LastSelectedObjects.AddRange(lastSelectedObjects);

            return refHelperData;
        }
    }
}