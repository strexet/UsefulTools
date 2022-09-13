using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperScriptableData : ScriptableObject
    {
        [SerializeField] private RefHelperData _refHelperData;

        public RefHelperData RefHelperData
        {
            get => _refHelperData;
            set
            {
                _refHelperData = value;

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }
    }
}