using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperWindow : EditorWindow
    {
        private Object _nextRef;
        private bool _isSubscribed;

        private GUIStyle _headerTextStyle;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/UsefulTools/Ref Helper Window")]
        private static void Init()
        {
            Debug.Log($"[DEBUG]<color=white>{nameof(RefHelperWindow)}.{nameof(Init)}></color> " +
                      "");

            var window = GetWindow<RefHelperWindow>("RefHelper");
            window.minSize = new Vector2(250, 250);
            window.Show();
        }

        private void OnEnable() => SubscribeToSelectionChange();

        private void OnDestroy() => UnsubscribeFromSelectionChange();

        private void OnGUI()
        {
            InitHeaderStyle();
            Resubscribe();
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            using var refWrapper = new RefHelperDataWrapper();
            var data = refWrapper.Data;

            ClearEmptyRefs(ref data);

            DrawLastSelectedObjects(ref data);

            GUILayout.Space(10);

            DrawSavedRefs(ref data);

            GUILayout.Space(5);

            DrawClearSaved(ref data);

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        private void ClearEmptyRefs(ref RefHelperData data)
        {
            var lastSelectedObjects = data.LastSelectedObjects;
            var referencedObjects = data.ReferencedObjects;

            for (int i = lastSelectedObjects.Count - 1; i >= 0; i--)
            {
                if (lastSelectedObjects[i] == null)
                {
                    lastSelectedObjects.RemoveAt(i);
                }
            }

            for (int j = referencedObjects.Count - 1; j >= 0; j--)
            {
                if (referencedObjects[j] == null)
                {
                    referencedObjects.RemoveAt(j);
                }
            }
        }

        private void DrawLastSelectedObjects(ref RefHelperData data)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("History", _headerTextStyle);

            data.LastSelectedObjectsMaxCount =
                EditorGUILayout.DelayedIntField("Max Count", data.LastSelectedObjectsMaxCount);

            int maxIndex = data.LastSelectedObjects.Count - 1;

            for (int i = maxIndex; i >= 0; i--)
            {
                var selectedObject = data.LastSelectedObjects[i];

                void ButtonCallback()
                {
                    using var refWrapper = new RefHelperDataWrapper();
                    var refData = refWrapper.Data;
                    refData.ReferencedObjects.Add(selectedObject);
                }

                DrawObjectToHistory(selectedObject, ButtonCallback);
            }

            GUILayout.EndVertical();
        }

        private static void DrawObjectToHistory(Object selectedObject, Action buttonCallback)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                buttonCallback?.Invoke();
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(
                string.Empty,
                selectedObject,
                typeof(Object),
                true);

            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
        }

        private void DrawSavedRefs(ref RefHelperData data)
        {
            // GUILayout.BeginHorizontal();
            GUILayout.Label("Saved", _headerTextStyle);
            // GUILayout.Space(25);
            DrawNextSaveRef(ref data);
            // GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            var referencedObjects = data.ReferencedObjects;

            for (int i = 0; i < referencedObjects.Count; i++)
            {
                var referencedObject = referencedObjects[i];
                int removeAtIndex = i;

                void ButtonCallback()
                {
                    using var refWrapper = new RefHelperDataWrapper();
                    var refData = refWrapper.Data;
                    refData.ReferencedObjects.RemoveAt(removeAtIndex);
                }

                if (!DrawSavedObject(referencedObject, ButtonCallback))
                {
                    return;
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawNextSaveRef(ref RefHelperData data)
        {
            GUILayout.BeginHorizontal();

            // GUILayout.FlexibleSpace();
            GUILayout.Label("Drop To Save");

            _nextRef = EditorGUILayout.ObjectField(
                _nextRef,
                typeof(Object),
                true
            );

            if (_nextRef != null)
            {
                data.ReferencedObjects.Add(_nextRef);
                _nextRef = null;
            }

            GUILayout.EndHorizontal();
        }

        private static bool DrawSavedObject(Object savedObject, Action buttonCallback)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                buttonCallback?.Invoke();

                return false;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(
                string.Empty,
                savedObject,
                typeof(Object),
                true);

            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();

            return true;
        }

        private void DrawClearSaved(ref RefHelperData data)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear Saved"))
            {
                data.ReferencedObjects.Clear();
            }

            GUILayout.EndHorizontal();
        }

        private void Resubscribe()
        {
            UnsubscribeFromSelectionChange();
            SubscribeToSelectionChange();
        }

        private void SubscribeToSelectionChange()
        {
            if (_isSubscribed)
            {
                return;
            }

            _isSubscribed = true;
            Selection.selectionChanged += UpdateLastSelectedObjects;
        }

        private void UnsubscribeFromSelectionChange()
        {
            if (!_isSubscribed)
            {
                return;
            }

            _isSubscribed = false;
            Selection.selectionChanged -= UpdateLastSelectedObjects;
        }

        private void UpdateLastSelectedObjects()
        {
            using var refWrapper = new RefHelperDataWrapper();
            var data = refWrapper.Data;

            if (Selection.count != 1)
            {
                return;
            }

            var lastSelectedObjects = data.LastSelectedObjects;
            lastSelectedObjects.Add(Selection.objects[0]);

            while (lastSelectedObjects.Count > data.LastSelectedObjectsMaxCount)
                lastSelectedObjects.RemoveAt(0);

            Repaint();
        }

        private void InitHeaderStyle() =>
            _headerTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                stretchWidth = true,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(3, 0, 0, 1)
            };
    }
}