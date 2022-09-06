using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

#else
using UnityEngine.Experimental.UIElements;
#endif

namespace UsefulTools.Editor
{
    [InitializeOnLoad]
    public static class SceneShortcutUtil
    {
        private static ScriptableObject _toolbar;
        private static string[] _scenePaths;
        private static string[] _sceneNames;

        static SceneShortcutUtil()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (_toolbar == null)
            {
                var editorAssembly = typeof(UnityEditor.Editor).Assembly;

                var toolbars = Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
                _toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (_toolbar != null)
                {
#if UNITY_2020_1_OR_NEWER
                    var windowBackendPropertyInfo =
                        editorAssembly.GetType("UnityEditor.GUIView").GetProperty("windowBackend",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    object windowBackend = windowBackendPropertyInfo.GetValue(_toolbar);
                    var visualTreePropertyInfo =
                        windowBackend.GetType().GetProperty("visualTree", BindingFlags.Public | BindingFlags.Instance);

                    var visualTree = (VisualElement)visualTreePropertyInfo.GetValue(windowBackend);
#else
					PropertyInfo visualTreePropertyInfo = editorAssembly.GetType("UnityEditor.GUIView")
					   .GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

					VisualElement visualTree = (VisualElement)visualTreePropertyInfo.GetValue(_toolbar, null);
#endif

                    var container = (IMGUIContainer)visualTree[0];

                    var onGUIHandlerFieldInfo = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    var handler = (Action)onGUIHandlerFieldInfo.GetValue(container);
                    handler -= OnGUI;
                    handler += OnGUI;
                    onGUIHandlerFieldInfo.SetValue(container, handler);
                }
            }

            if (_scenePaths == null || _scenePaths.Length != EditorBuildSettings.scenes.Length)
            {
                var scenePaths = new List<string>();
                var sceneNames = new List<string>();

                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.path == null || scene.path.StartsWith("Assets") == false)
                    {
                        continue;
                    }

                    string scenePath = Application.dataPath + scene.path.Substring(6);

                    scenePaths.Add(scenePath);
                    sceneNames.Add(Path.GetFileNameWithoutExtension(scenePath));
                }

                _scenePaths = scenePaths.ToArray();
                _sceneNames = sceneNames.ToArray();

                Debug.Log($"[DEBUG]<color=green>{nameof(SceneShortcutUtil)}.{nameof(Update)}></color> " +
                          $"Got scene paths and names: {_sceneNames.Length} names and {_scenePaths.Length} paths");
            }
        }

        private static void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                var rect = new Rect(0, 0, Screen.width, Screen.height)
                {
                    xMin = EditorGUIUtility.currentViewWidth * 0.5f + 100.0f,
                    xMax = EditorGUIUtility.currentViewWidth - 350.0f,
                    y = 8.0f
                };

                using (new GUILayout.AreaScope(rect))
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    int sceneIndex = -1;

                    for (int i = 0; i < _sceneNames.Length; ++i)
                    {
                        if (sceneName == _sceneNames[i])
                        {
                            sceneIndex = i;
                            break;
                        }
                    }

                    int newSceneIndex = EditorGUILayout.Popup(sceneIndex, _sceneNames, GUILayout.Width(200.0f));
                    if (newSceneIndex != sceneIndex)
                    {
                        EditorSceneManager.OpenScene(_scenePaths[newSceneIndex], OpenSceneMode.Single);
                    }
                }
            }
        }
    }
}
