using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.RefHelper
{
    public class RefHelperWindow : EditorWindow
    {
        private const string RefHelperSaveKey = "UsefulTools.RefHelper.Data";

        private RefHelperData _refHelperData;
        private Object _nextRef;
        private bool _isSubscribed;

        private GUIStyle _headerTextStyle;
        private Vector2 _scrollPosition;

        private RefHelperData RefHelperData => _refHelperData ??= new RefHelperData();
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
            LoadData();
        }

        private void OnDestroy()
        {
            UnsubscribeFromSelectionChange();
            SaveData();
        }

        private void OnGUI()
        {
            InitHeaderStyle();
            Resubscribe();
            UpdateGUI();
            SaveData();
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
            for (int i = ReferencedObjects.Count - 1; i >= 0; i--)
            {
                if (ReferencedObjects[i] == null)
                    LastSelectedObjects.RemoveAt(i);
            }

            for (int i = ReferencedObjects.Count - 1; i >= 0; i--)
            {
                if (ReferencedObjects[i] == null)
                    ReferencedObjects.RemoveAt(i);
            }
        }

        private void DrawLastSelectedObjects()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("History", _headerTextStyle);

            RefHelperData.LastSelectedObjectsMaxCount = EditorGUILayout.DelayedIntField("Max Count", RefHelperData.LastSelectedObjectsMaxCount);

            for (int i = LastSelectedObjects.Count - 1; i >= 0; i--)
            {
                var selectedObject = LastSelectedObjects[i];

                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(
                    string.Empty,
                    selectedObject,
                    typeof(Object),
                    true);

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("+", GUILayout.Width(20)))
                    ReferencedObjects.Add(selectedObject);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void DrawSavedRefs()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Saved", _headerTextStyle);
            GUILayout.Space(25);
            DrawNextSaveRef();
            GUILayout.EndHorizontal();

            for (int i = 0; i < ReferencedObjects.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                ReferencedObjects[i] = EditorGUILayout.ObjectField(
                    string.Empty,
                    ReferencedObjects[i],
                    typeof(Object),
                    true);

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    ReferencedObjects.RemoveAt(i);
                    return;
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawNextSaveRef()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Drop Here To Save");

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

        private void SaveData()
        {
            var saveData = new RefHelperSaveData(_refHelperData);
            string dataString = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(RefHelperSaveKey, dataString);
        }

        private void LoadData()
        {
            string dataString = PlayerPrefs.GetString(RefHelperSaveKey);
            var saveData = JsonUtility.FromJson<RefHelperSaveData>(dataString);
            _refHelperData = new RefHelperData(saveData);
        }
    }
}