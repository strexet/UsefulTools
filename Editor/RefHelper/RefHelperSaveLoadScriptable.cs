using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
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