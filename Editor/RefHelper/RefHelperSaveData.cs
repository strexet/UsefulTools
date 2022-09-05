using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperSaveDataScriptable : ScriptableObject
    {
        public int LastSelectedObjectsMaxCount;
        public string[] ReferencedPaths;
        public string[] LastSelectedPaths;

        public void Init(RefHelperData data)
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

    public interface IRefHelperSaveLoad
    {
        void SaveData(RefHelperData data);
        RefHelperData LoadData();
    }

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
            return new RefHelperData(saveData);
        }
    }

    public class RefHelperSaveLoadScriptable : IRefHelperSaveLoad
    {
        public void SaveData(RefHelperData data) => throw new NotImplementedException();

        public RefHelperData LoadData() => throw new NotImplementedException();
    }
}
