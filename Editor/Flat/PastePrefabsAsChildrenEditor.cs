using UnityEditor;
using UnityEngine;
using UsefulTools.Editor.Tools;

namespace UsefulTools.Editor.Flat
{
    [CustomEditor(typeof(PastePrefabsAsChildren))]
    public class PastePrefabsAsChildrenEditor : UnityEditor.Editor
    {
        PastePrefabsAsChildren _target;

        void OnEnable()
        {
            _target = target as PastePrefabsAsChildren;
            ActiveEditorTracker.sharedTracker.isLocked = true;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Paste prefabs as children and Remove script"))
            {
                var clones = PrefabAsChildrenPaster.PastePrefabsAsChildren(
                    _target.Prefabs,
                    _target.transform,
                    _target.NumberOfInserts);

                foreach (var clone in clones)
                {
                    clone.transform.localPosition = Vector3.zero;
                }

                Undo.DestroyObjectImmediate(_target);
                GUIUtility.ExitGUI();

                ActiveEditorTracker.sharedTracker.isLocked = false;
                Selection.objects = clones;
            }

            DrawDefaultInspector();
        }
    }
}