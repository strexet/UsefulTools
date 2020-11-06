using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class EditorScreenshot : EditorWindow
    {
        int nextScreenshotIndex;
        string screenshotFilenamePrefix = "screenshot_";
        string screenshotFolderPath = "Screenshots";

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
            var editorScreenshot = GetWindow<EditorScreenshot>("!Screenshot");

            if (EditorPrefs.HasKey("EditorScreenshot.screenshotFolderPath"))
            {
                editorScreenshot.screenshotFolderPath = EditorPrefs.GetString("EditorScreenshot.screenshotFolderPath");
            }

            if (EditorPrefs.HasKey("EditorScreenshot.screenshotFilenamePrefix"))
            {
                editorScreenshot.screenshotFilenamePrefix =
                    EditorPrefs.GetString("EditorScreenshot.screenshotFilenamePrefix");
            }

            if (EditorPrefs.HasKey("EditorScreenshot.nextScreenshotIndex"))
            {
                editorScreenshot.nextScreenshotIndex = EditorPrefs.GetInt("EditorScreenshot.nextScreenshotIndex");
            }

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
                EditorPrefs.SetString("EditorScreenshot.screenshotFolderPath", screenshotFolderPath);
                EditorPrefs.SetString("EditorScreenshot.screenshotFilenamePrefix", screenshotFilenamePrefix);
                EditorPrefs.SetInt("EditorScreenshot.nextScreenshotIndex", nextScreenshotIndex);
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
            EditorPrefs.SetInt("EditorScreenshot.nextScreenshotIndex", nextScreenshotIndex);
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
