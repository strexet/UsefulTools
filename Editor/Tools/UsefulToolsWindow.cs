using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor {
	public class UsefulToolsWindow : EditorWindow {
		private static readonly string[]       Options = {"Prefix", "Suffix"};
		private string                         _additionToName;
		private readonly float                 _bigSpacePixelsCount = 20f;
		private string                         _byComponentName     = "";
		private string                         _byObjectName        = "";
		private Material                       _commonMaterial;
		private string                         _folderName = "NewFolder";
		private GUIStyle                       _headerTextStyle;
		private string                         _insertionSuffixForName = "";
		private bool                           _isActiveInHierarchy;
		private string                         _namePart        = "NamePart";
		private string                         _newName         = "NewName";
		private int _numberOfInserts;
		private List<GameObject>               _objectsToInsertRemove;
		private readonly EditorList<GameObject> _objectsToInsertRemoveEditorList = new EditorList<GameObject>(1);
		private List<Transform>               _optionalParents;
		private readonly EditorList<Transform> _optionalParentsEditorList = new EditorList<Transform>(1);
		private string                         _prefix     = "";
		private string                         _replaceFor = "ReplaceFor";
		private bool                           _saveCurrentSelection;
		private Vector2                        _scrollPosition;
		private int                            _selectedAddition      = 2;
		private bool                           _selectInserted        = true;
		private readonly float                 _smallSpacePixelsCount = 5f;
		private string                         _suffix                = "";

		[MenuItem("Tools/UsefulTools/Tools Window", false, 0)]
		private static void Init() {
			var window = GetWindow<UsefulToolsWindow>();
			window.minSize = new Vector2(312, 50);
			window.titleContent = new GUIContent("UsefulTools");
			window.Show();
		}

		private void OnGUI() {
			InitHeaderStyle();

			GUILayout.BeginVertical();
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

			DrawInsertRemoveObjectsMenu();
			DrawSelectChildrenParenMenu();
			DrawRenameSelectedObjectsMenu();
			DrawSelectGameObjectsMenu();
			DrawPackObjectsMenu();
			DrawSelectObjectsWithCommonMaterialMenu();

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		private void DrawSelectObjectsWithCommonMaterialMenu() {
			GUILayout.Label("Select Objects With Common Material", _headerTextStyle);
			GUILayout.Label("Common Material");
			_commonMaterial = EditorGUILayout.ObjectField(_commonMaterial, typeof(Material), true) as Material;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Search for activeInHierarchy:");
			_isActiveInHierarchy = EditorGUILayout.Toggle(_isActiveInHierarchy);
			GUILayout.EndHorizontal();
			if ( GUILayout.Button("Select By Material") ) {
				SelectObjectsByMaterial(_commonMaterial, _isActiveInHierarchy);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void DrawSelectChildrenParenMenu() {
			GUILayout.Label("Select relatives of current selected objects", _headerTextStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Save current selection:");
			_saveCurrentSelection = EditorGUILayout.Toggle(_saveCurrentSelection);
			GUILayout.EndHorizontal();
			if ( GUILayout.Button("Select Parents") ) {
				SelectChildrenOrParentsOfSelectedObjects(false, _saveCurrentSelection);
			}

			if ( GUILayout.Button("Select Children") ) {
				SelectChildrenOrParentsOfSelectedObjects(true, _saveCurrentSelection);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void DrawRenameSelectedObjectsMenu() {
			GUILayout.Label("Rename Selected Objects", _headerTextStyle);
			GUILayout.Label("New Name");
			_newName = EditorGUILayout.TextField(_newName);
			if ( GUILayout.Button("Rename") ) {
				RenameSelectedObjects(_newName);
			}

			GUILayout.Space(_smallSpacePixelsCount);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Add to name...");
			_selectedAddition =
				GUILayout.SelectionGrid(_selectedAddition, Options, Options.Length, EditorStyles.radioButton);
			GUILayout.EndHorizontal();
			_additionToName = EditorGUILayout.TextField(_additionToName);


			if ( GUILayout.Button("Add to name...") ) {
				AddToName(_selectedAddition, _additionToName);
			}

			GUILayout.Space(_smallSpacePixelsCount);

			GUILayout.Label("Replace part of name...", _headerTextStyle);
			_namePart = EditorGUILayout.TextField("Name Part", _namePart);
			_replaceFor = EditorGUILayout.TextField("Replace For", _replaceFor);
			if ( GUILayout.Button("Replace") ) {
				ReplaceNamePart(_namePart, _replaceFor);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void DrawSelectGameObjectsMenu() {
			GUILayout.Label("Select GameObjects", _headerTextStyle);
			GUILayout.Label("Search Root Transforms");
			_optionalParentsEditorList.UpdateAndDraw(ref _optionalParents);
			GUILayout.Space(_smallSpacePixelsCount);
			GUILayout.Label("By Object Name or Name Part");
			_byObjectName = EditorGUILayout.TextField(_byObjectName);
			GUILayout.Space(_smallSpacePixelsCount);
			GUILayout.Label("By Component Name");
			_byComponentName = EditorGUILayout.TextField(_byComponentName);

			if ( GUILayout.Button("Select") ) {
				SelectGameObjectsByNameAndComponent(_byObjectName, _byComponentName, _optionalParents);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void DrawInsertRemoveObjectsMenu() {
			GUILayout.Label("Insert/Remove objects (or prefab)", _headerTextStyle);
			GUILayout.Label("Objects to insert/remove");

			_objectsToInsertRemoveEditorList.UpdateAndDraw(ref _objectsToInsertRemove);
			
			GUILayout.Space(_smallSpacePixelsCount);

			_numberOfInserts = EditorGUILayout.IntField("Number of inserts: ", _numberOfInserts);
			if ( _numberOfInserts < 1 ) {
				_numberOfInserts = 1;
			}

			_insertionSuffixForName = EditorGUILayout.TextField("Insertion suffix: ", _insertionSuffixForName);

			_selectInserted = EditorGUILayout.Toggle("Select inserted", _selectInserted);

			if ( GUILayout.Button("Insert object into selected") ) {
				InsertObjectsIntoSelected(_objectsToInsertRemove.ToArray(),
					_numberOfInserts,
					_insertionSuffixForName,
					_selectInserted);
			}

			if ( GUILayout.Button("Remove all objects by name from selected") ) {
				DestroySelectedChildrenByObjectName(_objectsToInsertRemove);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void DrawPackObjectsMenu() {
			GUILayout.Label("Pack objects", _headerTextStyle);

			GUILayout.Label("Directory Name");
			_folderName = EditorGUILayout.TextField(_folderName);

			if ( GUILayout.Button("Group selected gameobjects into directory") ) {
				PackSelectedObjectsInFolder(_folderName);
			}

			GUILayout.Space(_bigSpacePixelsCount);
		}

		private void InitHeaderStyle() {
			_headerTextStyle = new GUIStyle(GUI.skin.label);
			_headerTextStyle.fontSize = 14;
			_headerTextStyle.stretchWidth = true;
		}

		private static void ReplaceNamePart(string part, string replacement) {
			var selectedGameObjects = Selection.gameObjects;

			if ( selectedGameObjects.Length == 0 )
			{
				ReplaceNamePartForAsset(part, replacement);
				return;
			}

			Undo.RecordObjects(selectedGameObjects, "Replace Part Of The Name");

			foreach ( var go in selectedGameObjects ) {
				go.name = go.name.Replace(part, replacement);
			}
		}

		private static void ReplaceNamePartForAsset(string part, string replacement)
		{
			var selectedObjectsGUIDs = Selection.assetGUIDs;

			foreach (string guid in selectedObjectsGUIDs)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var fileName = Path.GetFileName(path);
				if (fileName == null)
				{
					continue;
				}

				var newFileName = fileName.Replace(part, replacement);
				var error = AssetDatabase.RenameAsset(path, newFileName);

				if (!string.IsNullOrEmpty(error))
				{
					Debug.LogError(error);
					Debug.Log($"Old Path = \"{path}\"");
					Debug.Log($"Old File Name = \"{fileName}\"");
					Debug.Log($"New File Name = \"{newFileName}\"");
				}
			}
		}

		private void AddToName(int indexOfSelectedAddition, string additionToName) {
			var selected = Selection.gameObjects;

			if ( selected.Length == 0 ) {
				return;
			}

			Undo.RecordObjects(selected, "Add to object's Name");

			switch ( indexOfSelectedAddition ) {
				case 0:
					_prefix = additionToName;
					_suffix = string.Empty;
					break;
				case 1:
					_suffix = additionToName;
					_prefix = string.Empty;
					break;
				default:
					return;
			}

			foreach ( var selectedObject in selected ) {
				selectedObject.name = string.Format("{0}{1}{2}", _prefix, selectedObject.name, _suffix);
			}
		}

		private void SelectObjectsByMaterial(Material mateial, bool activeInHierarchy) {
			if ( mateial == null ) {
				return;
			}

			var properObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject))
				.Select(g => g as GameObject)
				.Where(
					g => {
						if ( activeInHierarchy && g ) {
							return g.activeInHierarchy;
						}

						return true;
					})
				.Where(
					g => {
						var renderer = g.GetComponent<Renderer>();

						if ( renderer == null ) {
							return false;
						}

						foreach ( var sharedMaterial in renderer.sharedMaterials ) {
							if ( mateial.Equals(sharedMaterial) ) {
								return true;
							}
						}

						return false;
					})
				.ToArray();

			Selection.objects = properObjects.ToArray();
		}

		private void
			SelectChildrenOrParentsOfSelectedObjects(bool selectChildrenNotParents, bool saveCurrentSelection) {
			var selected = Selection.gameObjects;

			if ( selected.Length == 0 ) {
				return;
			}

			var nextSelected = new List<GameObject>();

			foreach ( var selectedObject in selected ) {
				var childCount = selectedObject.transform.childCount;
				var parent = selectedObject.transform.parent;

				if ( selectChildrenNotParents && childCount == 0 ||
				     !selectChildrenNotParents && parent == null ) {
					nextSelected.Add(selectedObject);
					continue;
				}

				if ( saveCurrentSelection ) {
					nextSelected.Add(selectedObject);
				}

				if ( selectChildrenNotParents ) {
					for ( var i = 0; i < childCount; i++ ) {
						nextSelected.Add(selectedObject.transform.GetChild(i).gameObject);
					}
				}
				else {
					nextSelected.Add(selectedObject.transform.parent.gameObject);
				}
			}

			Selection.objects = nextSelected.ToArray();
		}

		private void SelectGameObjectsByNameAndComponent(string byObjectName, string byComponentName,
			List<Transform> optionalParents) {
			List<Object> properObjects = new List<Object>();

			foreach (var optionalParent in optionalParents)
			{
				properObjects.AddRange(Resources.FindObjectsOfTypeAll(typeof(GameObject))
				                                    .Select(g => g as GameObject)
				                                    .Where(
					                                    g =>
					                                    {
						                                    if (optionalParent == null)
						                                    {
							                                    return true;
						                                    }

						                                    var parent = g.transform.parent;

						                                    while (parent != null)
						                                    {
							                                    if (parent == optionalParent)
							                                    {
								                                    return true;
							                                    }

							                                    parent = parent.parent;
						                                    }

						                                    return false;
					                                    })
				                                    .Where(
					                                    g =>
					                                    {
						                                    if (string.Empty == byObjectName
						                                        && string.Empty != byComponentName)
						                                    {
							                                    return true;
						                                    }

						                                    return g.name.Contains(byObjectName);
					                                    })
				                                    .Where(
					                                    g =>
					                                    {
						                                    if (string.Empty == byComponentName
						                                        && string.Empty != byObjectName)
						                                    {
							                                    return true;
						                                    }

						                                    return g.GetComponent(byComponentName) != null;
					                                    }));
			}

			Selection.objects = properObjects.ToArray();
		}

		private void RenameSelectedObjects(string newName) {
			var selected = Selection.gameObjects;

			if ( selected.Length == 0 ) {
				return;
			}

			Undo.RecordObjects(selected, "Rename Objects");

			foreach ( var selectedObject in selected ) {
				selectedObject.name = newName;
			}
		}

		private void PackSelectedObjectsInFolder(string folderName) {
			var selectedObjects = Selection.gameObjects;

			if ( selectedObjects.Length == 0 ) {
				return;
			}

			var n = new GameObject(folderName);
			n.transform.position = selectedObjects[0].transform.position;
			n.transform.parent = selectedObjects[0].transform.parent;

			Undo.RegisterCreatedObjectUndo(n, "Create Folder GameObject");

			for ( var i = 0; i < selectedObjects.Length; i++ ) {
				Undo.SetTransformParent(selectedObjects[i].transform, n.transform,
					"Set folding object's parent to the Folder");
			}

			Selection.objects = new[] {n};
		}

		private void InsertObjectsIntoSelected(GameObject[] clonedObjects, int numberOfActions,
			string insertionSuffix,
			bool selectInserted) {
			if ( clonedObjects.Length == 0 ) {
				Debug.Log("Objects to insert/remove is empty!");
				return;
			}

			foreach ( var clonedObject in clonedObjects ) {
				if ( clonedObject == null ) {
					Debug.Log("Some Object to insert/remove is null!");
					return;
				}
			}

			var selectedObjects = Selection.gameObjects;
			var clones = new List<GameObject>();

			foreach ( var selectedObject in selectedObjects ) {
				clones.AddRange(PrefabAsChildrenPaster.PastePrefabsAsChildren(
					clonedObjects,
					selectedObject.transform,
					numberOfActions));
			}

			foreach ( var clone in clones ) {
				if ( insertionSuffix != string.Empty ) {
					clone.name = clone.name + insertionSuffix;
				}

				clone.transform.localPosition = Vector3.zero;
			}

			if ( selectInserted ) {
				Selection.objects = clones.ToArray();
			}
		}

		private void DestroySelectedChildrenByObjectName(List<GameObject> objectsWithNameToDestroy) {
			if ( objectsWithNameToDestroy.Count == 0 ) {
				Debug.Log("Objects to insert/remove is empty!");
				return;
			}

			foreach ( var clonedObject in objectsWithNameToDestroy ) {
				if ( clonedObject == null ) {
					Debug.Log("Some Object to insert/remove is null!");
					return;
				}
			}

			foreach ( var objectWithNameToDestroy in objectsWithNameToDestroy ) {
				var destroyName = objectWithNameToDestroy.name;

				var selected = Selection.gameObjects;

				foreach ( var obj in selected ) {
					var childs = obj.transform.GetComponentsInChildren<Transform>();
					foreach ( var child in childs ) {
						if ( child && child.name.Equals(destroyName) ) {
							Undo.DestroyObjectImmediate(child.gameObject);
						}
					}
				}
			}
		}
	}
}