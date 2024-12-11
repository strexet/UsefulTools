using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.Flat
{
    [CustomEditor(typeof(DestroyMe))]
    public class DestroyMeEditor : UnityEditor.Editor
    {
        private static DestroyMe _target;
        private static GameObject[] _selection;

        public override void OnInspectorGUI()
        {
            if (_target)
            {
                DestroyImmediate(_target.gameObject);
                Selection.objects = _selection;
                return;
            }

            _target = target as DestroyMe;
            _selection = new[] { _target.ToDestroy.gameObject };

            DestroyImmediate(_target.ToDestroy);
        }
    }
}