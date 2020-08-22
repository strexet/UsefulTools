using System;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor {
	public class EditorScreenshot : EditorWindow {
		int    _nextScreenshotIndex;
		string _screenshotFilenamePrefix = "screenshot_";
		string _screenshotFolderPath     = "Screenshots";

		[MenuItem("Tools/UsefulTools/Editor Screenshot/Editor Screenshot Window")]
		static void Init() {
			GetOrCreateWindow();
		}

		[MenuItem("Tools/UsefulTools/Editor Screenshot/Take Screenshot _F11")]
		static void StaticTakeScreenshot() {
			GetOrCreateWindow().TakeScreenshot();
		}

		static EditorScreenshot GetOrCreateWindow() {
			var editorScreenshot = GetWindow<EditorScreenshot>("Screenshot");

			if ( EditorPrefs.HasKey("EditorScreenshot.screenshotFolderPath") ) {
				editorScreenshot._screenshotFolderPath = EditorPrefs.GetString("EditorScreenshot.screenshotFolderPath");
			}

			if ( EditorPrefs.HasKey("EditorScreenshot.screenshotFilenamePrefix") ) {
				editorScreenshot._screenshotFilenamePrefix =
					EditorPrefs.GetString("EditorScreenshot.screenshotFilenamePrefix");
			}

			if ( EditorPrefs.HasKey("EditorScreenshot.nextScreenshotIndex") ) {
				editorScreenshot._nextScreenshotIndex = EditorPrefs.GetInt("EditorScreenshot.nextScreenshotIndex");
			}

			return editorScreenshot;
		}

		void OnGUI() {
			EditorGUI.BeginChangeCheck();

			var savePathLabel =
				new GUIContent("Save path", "Save path of the screenshots, relative from the project root");
			_screenshotFolderPath = EditorGUILayout.TextField(savePathLabel, _screenshotFolderPath);
			_screenshotFilenamePrefix = EditorGUILayout.TextField("Screenshot prefix", _screenshotFilenamePrefix);
			_nextScreenshotIndex = EditorGUILayout.IntField("Next screenshot index", _nextScreenshotIndex);

			if ( EditorGUI.EndChangeCheck() ) {
				EditorPrefs.SetString("EditorScreenshot.screenshotFolderPath", _screenshotFolderPath);
				EditorPrefs.SetString("EditorScreenshot.screenshotFilenamePrefix", _screenshotFilenamePrefix);
				EditorPrefs.SetInt("EditorScreenshot.nextScreenshotIndex", _nextScreenshotIndex);
			}

			if ( GUILayout.Button("Take screenshot") ) {
				TakeScreenshot();
			}
		}

		void TakeScreenshot() {
			// get name of current focused window, which should be "  (UnityEditor.GameView)" if it is a Game view
			var focusedWindowName = focusedWindow.ToString();
			if ( !focusedWindowName.Contains("UnityEditor.GameView") ) {
				// since no Game view is focused right now, focus on any Game view, or create one if needed
				var gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
				GetWindow(gameViewType);
			}

			// Tried getting the last focused window, but does not always work (even for focused window!)
			// Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
			// EditorWindow lastFocusedGameView = (EditorWindow) gameViewType.GetField("s_LastFocusedGameView", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			// if (lastFocusedGameView != null) {
			// 	lastFocusedGameView.Focus();
			// } else {
			// 	// no Game view created since editor launch, create one
			// 	Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
			// 	EditorWindow.GetWindow(gameViewType);
			// }

//		string path = string.Format("{0}/{1}{2:00}.png", screenshotFolderPath, screenshotFilenamePrefix, nextScreenshotIndex);
			var path = string.Format("{0}/{1}{2:00}.png", "", _screenshotFilenamePrefix, _nextScreenshotIndex);

			ScreenCapture.CaptureScreenshot(path);

			Debug.LogFormat("Screenshot recorded at {0} ({1})", path, UnityStats.screenRes);

			++_nextScreenshotIndex;
			EditorPrefs.SetInt("EditorScreenshot.nextScreenshotIndex", _nextScreenshotIndex);
		}
	}
}