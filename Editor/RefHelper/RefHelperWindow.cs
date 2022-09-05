using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperWindow : EditorWindow
    { 
        private RefHelperData _refHelperData;
        private IRefHelperSaveLoad _saveLoad;
        private Object _nextRef;
        private bool _isSubscribed;

        private GUIStyle _headerTextStyle;
        private Vector2 _scrollPosition;

        private RefHelperData RefHelperData => _refHelperData ??= new RefHelperData();
        private IRefHelperSaveLoad SaveLoad => _saveLoad ??= new RefHelperSaveLoadRaw();
        private List<Object> ReferencedObjects => RefHelperData.ReferencedObjects ??= new List<Object>();
        private List<Object> LastSelectedObjects => RefHelperData.LastSelectedObjects ??= new List<Object>();

        [MenuItem("Tools/UsefulTools/Ref Helper Window")]
        private static void Init()
        {
            var window = GetWindow<RefHelperWindow>("RefHelper");
            window.minSize = new Vector2(250, 250);
            window.Show();
        }

        private void OnEnable()
        {
            SubscribeToSelectionChange();
            _refHelperData = SaveLoad.LoadData();
        }

        private void OnDestroy()
        {
            UnsubscribeFromSelectionChange();
            SaveLoad.SaveData(_refHelperData);
        }

        private void OnGUI()
        {
            InitHeaderStyle();
            Resubscribe();
            UpdateGUI();
            SaveLoad.SaveData(_refHelperData);
        }

        private void UpdateGUI()
        {
            ClearEmptyRefs();

            DrawLastSelectedObjects();

            GUILayout.Space(10);

            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            DrawSavedRefs();

            GUILayout.EndScrollView();
            GUILayout.Space(5);

            DrawClearSaved();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        private void ClearEmptyRefs()
        {
            for (int i = LastSelectedObjects.Count - 1; i >= 0; i--)
            {
                if (LastSelectedObjects[i] == null)
                    LastSelectedObjects.RemoveAt(i);
            }

            for (int j = ReferencedObjects.Count - 1; j >= 0; j--)
            {
                if (ReferencedObjects[j] == null)
                    ReferencedObjects.RemoveAt(j);
            }
        }

        private void DrawLastSelectedObjects()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("History", _headerTextStyle);

            RefHelperData.LastSelectedObjectsMaxCount = EditorGUILayout.DelayedIntField("Max Count", RefHelperData.LastSelectedObjectsMaxCount);
            int maxIndex = LastSelectedObjects.Count - 1;
            
            for (int i = maxIndex; i >= 0; i--)
            {
                var selectedObject = LastSelectedObjects[i];
                DrawObjectToHistory(selectedObject, () => ReferencedObjects.Add(selectedObject));
            }

            GUILayout.EndVertical();
        }

        private static void DrawObjectToHistory(Object selectedObject, Action buttonCallback)
        {
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("+", GUILayout.Width(20)))
                buttonCallback?.Invoke();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(
                string.Empty,
                selectedObject,
                typeof(Object),
                true);
            EditorGUI.EndDisabledGroup();
            
            GUILayout.EndHorizontal();
        }

        private void DrawSavedRefs()
        {
            // GUILayout.BeginHorizontal();
            GUILayout.Label("Saved", _headerTextStyle);
            // GUILayout.Space(25);
            DrawNextSaveRef();
            // GUILayout.EndHorizontal();

            for (int i = 0; i < ReferencedObjects.Count; i++)
            {
                var referencedObject = ReferencedObjects[i];
                int removeAtIndex = i;
                
                if (!DrawSavedObject(referencedObject, () => ReferencedObjects.RemoveAt(removeAtIndex)))
                {
                    return;
                }
            }
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

        private void DrawNextSaveRef()
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
                ReferencedObjects.Add(_nextRef);
                _nextRef = null;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawClearSaved()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear Saved"))
                ReferencedObjects.Clear();

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
                return;

            _isSubscribed = true;
            Selection.selectionChanged += UpdateLastSelectedObjects;
        }

        private void UnsubscribeFromSelectionChange()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = false;
            Selection.selectionChanged -= UpdateLastSelectedObjects;
        }

        private void UpdateLastSelectedObjects()
        {
            if (Selection.count != 1)
                return;

            LastSelectedObjects.Add(Selection.objects[0]);

            while (LastSelectedObjects.Count > RefHelperData.LastSelectedObjectsMaxCount)
                LastSelectedObjects.RemoveAt(0);

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
