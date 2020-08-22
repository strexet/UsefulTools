using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UsefulTools.Editor {
	public static class EditorCollapseAll {
		const  int WaitThreshold = 6;
		static int wait;
		static int undoIndex;

		//This code is released under the MIT license: https://opensource.org/licenses/MIT
		[MenuItem("GameObject/Collapse All 2nd Method &%#-", priority = 40)]
		static void UnfoldSelection() {
			EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
			var hierarchyWindow = EditorWindow.focusedWindow;
			var expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");

			if ( expandMethodInfo == null ) {
				Debug.Log("Hierarchy must be focused! (e.g. select any object in hierarchy)");
				return;
			}

			foreach ( var root in SceneManager.GetActiveScene().GetRootGameObjects() ) {
				expandMethodInfo.Invoke(hierarchyWindow, new object[] {root.GetInstanceID(), false});
			}
		}

		[MenuItem("Assets/Collapse Folders &#-", priority = 1000)]
		public static void CollapseFolders() {
			var rootDirectories = new DirectoryInfo(Application.dataPath).GetDirectories();
			var rootDirectoriesList = new List<Object>(rootDirectories.Length);
			for ( var i = 0; i < rootDirectories.Length; i++ ) {
				var directoryObj = AssetDatabase.LoadAssetAtPath<Object>("Assets/" + rootDirectories[i].Name);
				if ( directoryObj != null ) {
					rootDirectoriesList.Add(directoryObj);
				}
			}

			if ( rootDirectoriesList.Count > 0 ) {
				Undo.IncrementCurrentGroup();
				Selection.objects = Selection.objects;
				undoIndex = Undo.GetCurrentGroup();

				EditorUtility.FocusProjectWindow();

				Selection.objects = rootDirectoriesList.ToArray();
				EditorApplication.update += CollapseHelper;
			}
		}

		[MenuItem("GameObject/Collapse All &%-", priority = 40)]
		public static void CollapseGameObjects() {
			var rootGameObjects = new List<GameObject>();
			var sceneCount = SceneManager.sceneCount;
			for ( var i = 0; i < sceneCount; i++ ) {
				rootGameObjects.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
			}

			if ( rootGameObjects.Count > 0 ) {
				Undo.IncrementCurrentGroup();
				Selection.objects = Selection.objects;
				undoIndex = Undo.GetCurrentGroup();

				Selection.objects = rootGameObjects.ToArray();
				EditorApplication.update += CollapseHelper;
			}
		}

		static void CollapseHelper() {
			if ( wait < WaitThreshold ) // Increase the 'waitThreshold' if script doesn't always work
			{
				wait++;
			}
			else {
				wait = 0;

				SendCollapseEvent();
			}
		}

		static void SendCollapseEvent() {
			var focusedWindow = EditorWindow.focusedWindow;
			if ( focusedWindow != null ) {
				focusedWindow.SendEvent(new Event {
					keyCode = KeyCode.LeftArrow,
					type = EventType.KeyDown,
					alt = true
				});
			}

			EditorApplication.update -= CollapseHelper;
			Undo.RevertAllDownToGroup(undoIndex);
		}
	}
}