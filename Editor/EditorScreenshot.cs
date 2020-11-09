using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class EditorScreenshot : EditorWindow
    {
        const string NextScreenshotIndexPrefsKey = "EditorScreenshot.nextScreenshotIndex";
        const string ScreenshotFolderPathPrefsKey = "EditorScreenshot.screenshotFolderPath";
        const string ScreenshotFilenamePrefixPrefsKey = "EditorScreenshot.screenshotFilenamePrefix";

        const string DefaultScreenshotFolderPath = "!Screenshots";
        const string DefaultScreenshotFilenamePrefix = "screenshot_";
        const int DefaultNextScreenshotIndex = 0;

        int nextScreenshotIndex;
        string screenshotFilenamePrefix;
        string screenshotFolderPath;

        [MenuItem("Tools/UsefulTools/Editor Screenshot/Editor Screenshot Window")]
        static void Init()
        {
            GetOrCreateWindow();
        }

        [MenuItem("Tools/UsefulTools/Editor Screenshot/Take Screenshot _F11")]
        static void StaticTakeScreenshot()
        {
            GetOrCreateWindow().TakeScreenshot();
        }

        static EditorScreenshot GetOrCreateWindow()
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

        void OnGUI()
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

        void TakeScreenshot()
        {
            var filePath = GetPath();

            SetFocusToGameView();
            ScreenCapture.CaptureScreenshot(filePath);

            Debug.LogFormat("Screenshot recorded at {0} ({1})", filePath, UnityStats.screenRes);

            ++nextScreenshotIndex;
            EditorPrefs.SetInt(NextScreenshotIndexPrefsKey, nextScreenshotIndex);
        }

        string GetPath()
        {
            var directoryPath = $"{Application.dataPath}/{screenshotFolderPath}";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = $"{directoryPath}/{screenshotFilenamePrefix}{nextScreenshotIndex:00}.png";
            return filePath;
        }

        void SetFocusToGameView()
        {
            // Get name of current focused window, which should be "  (UnityEditor.GameView)" if it is a Game view
            var focusedWindowName = focusedWindow.ToString();
            if (!focusedWindowName.Contains("UnityEditor.GameView"))
            {
                // Since no Game view is focused right now, focus on any Game view, or create one if needed
                var gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
                GetWindow(gameViewType);
            }
        }
    }
}
