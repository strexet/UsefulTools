using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public static class PrefsEditor
    {
        [MenuItem("Tools/UsefulTools/Prefs/Clear")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}