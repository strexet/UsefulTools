using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

    public class RefHelperSaveLoadScriptable : IRefHelperSaveLoad
    {
        private static RefHelperSaveLoadScriptable instance;
        public static IRefHelperSaveLoad Instance => instance ??= new RefHelperSaveLoadScriptable();

        private const string RefHelperScriptableAssetsDirectoryPath = "UsefulTools/Editor/RefHelperData";
        private const string RefHelperScriptableFileName = "RefHelperScriptableData.asset";
        private const string RefHelperScriptablePath = "Assets/"
                                                       + RefHelperScriptableAssetsDirectoryPath
                                                       + "/" + RefHelperScriptableFileName;

        private static readonly string RawDirectoryPath = Path.Combine(
            Application.dataPath,
            RefHelperScriptableAssetsDirectoryPath);

        private static readonly string RawFilePath = Path.Combine(
            Application.dataPath,
            RefHelperScriptableAssetsDirectoryPath,
            RefHelperScriptableFileName);

        private static bool AssetFileExist => File.Exists(RawFilePath);

        private RefHelperScriptableData _scriptableData;

        private RefHelperScriptableData ScriptableData
        {
            get
            {
                if (_scriptableData == null || !AssetFileExist)
                {
                    _scriptableData = LoadRefHelperScriptableData();
                }

                return _scriptableData;
            }
        }

        private RefHelperSaveLoadScriptable() { }

        public void SaveData(RefHelperData data) => ScriptableData.RefHelperData = data;

        public RefHelperData LoadData() => ScriptableData.RefHelperData;

        private static RefHelperScriptableData LoadRefHelperScriptableData()
        {
            var scriptableAsset = AssetDatabase.LoadAssetAtPath<RefHelperScriptableData>(RefHelperScriptablePath);

            if (scriptableAsset == null)
            {
                if (!Directory.Exists(RawDirectoryPath))
                {
                    Directory.CreateDirectory(RawDirectoryPath);
                }

                var scriptableData = ScriptableObject.CreateInstance<RefHelperScriptableData>();
                AssetDatabase.CreateAsset(scriptableData, RefHelperScriptablePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                scriptableAsset = AssetDatabase.LoadAssetAtPath<RefHelperScriptableData>(RefHelperScriptablePath);

                Debug.Log($"<color=yellow>{nameof(RefHelperSaveLoadScriptable)}.{nameof(RefHelperSaveLoadScriptable)}></color> " +
                          $"Created asset at path: {RefHelperScriptablePath}");
            }

            return scriptableAsset;
        }
    }
}