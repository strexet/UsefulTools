using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class EditorScreenshot : EditorWindow
    {
        private const string NextScreenshotIndexPrefsKey = "EditorScreenshot.nextScreenshotIndex";
        private const string ScreenshotFolderPathPrefsKey = "EditorScreenshot.screenshotFolderPath";
        private const string ScreenshotFilenamePrefixPrefsKey = "EditorScreenshot.screenshotFilenamePrefix";

        private const string DefaultScreenshotFolderPath = "!Screenshots";
        private const string DefaultScreenshotFilenamePrefix = "screenshot_";
        private const int DefaultNextScreenshotIndex = 0;

        private int nextScreenshotIndex;
        private string screenshotFilenamePrefix;
        private string screenshotFolderPath;

        [MenuItem("Tools/UsefulTools/Editor Screenshot/Editor Screenshot Window")]
        private static void Init() => GetOrCreateWindow();

        [MenuItem("Tools/UsefulTools/Editor Screenshot/Take Screenshot _F11")]
        private static void StaticTakeScreenshot() => GetOrCreateWindow().TakeScreenshot();

        private static EditorScreenshot GetOrCreateWindow()
        {
            var editorScreenshot = GetWindow<EditorScreenshot>("Screenshot");

            editorScreenshot.screenshotFolderPath =
                EditorPrefs.GetString(ScreenshotFolderPathPrefsKey, DefaultScreenshotFolderPath);

            editorScreenshot.screenshotFilenamePrefix =
                EditorPrefs.GetString(ScreenshotFilenamePrefixPrefsKey, DefaultScreenshotFilenamePrefix);

            editorScreenshot.nextScreenshotIndex =
                EditorPrefs.GetInt(NextScreenshotIndexPrefsKey, DefaultNextScreenshotIndex);

            return editorScreenshot;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            var savePathLabel =
                new GUIContent("Save path", "Save path for the screenshots in Assets");

            screenshotFolderPath = EditorGUILayout.TextField(savePathLabel, screenshotFolderPath);
            screenshotFilenamePrefix = EditorGUILayout.TextField("Screenshot prefix", screenshotFilenamePrefix);
            nextScreenshotIndex = EditorGUILayout.IntField("Next screenshot index", nextScreenshotIndex);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(ScreenshotFolderPathPrefsKey, screenshotFolderPath);
                EditorPrefs.SetString(ScreenshotFilenamePrefixPrefsKey, screenshotFilenamePrefix);
                EditorPrefs.SetInt(NextScreenshotIndexPrefsKey, nextScreenshotIndex);
            }

            if (GUILayout.Button("Take screenshot"))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            string filePath = GetPath();

            SetFocusToGameView();
            ScreenCapture.CaptureScreenshot(filePath);

            Debug.LogFormat("Screenshot recorded at {0} ({1})", filePath, UnityStats.screenRes);

            ++nextScreenshotIndex;
            EditorPrefs.SetInt(NextScreenshotIndexPrefsKey, nextScreenshotIndex);
        }

        private string GetPath()
        {
            string directoryPath = $"{Application.dataPath}/{screenshotFolderPath}";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = $"{directoryPath}/{screenshotFilenamePrefix}{nextScreenshotIndex:00}.png";
            return filePath;
        }

        private void SetFocusToGameView()
        {
            // Get name of current focused window, which should be "  (UnityEditor.GameView)" if it is a Game view
            string focusedWindowName = focusedWindow.ToString();
            if (!focusedWindowName.Contains("UnityEditor.GameView"))
            {
                // Since no Game view is focused right now, focus on any Game view, or create one if needed
                var gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
                GetWindow(gameViewType);
            }
        }
    }
}