using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public static class PrefsEditor
    {
        [MenuItem("Tools/UsefulTools/Prefs/Clear Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log($"<color=yellow>{nameof(PrefsEditor)}.{nameof(ClearPrefs)}> Cleared Prefs!</color>");
        }
    }
}