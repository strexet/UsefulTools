using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ME.ECSEditor;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.Tools
{
    public class UsefulToolsWindow : EditorWindow
    {
        private static readonly string[] Options = { "Prefix", "Suffix" };
        private string additionToName;
        private readonly float bigSpacePixelsCount = 20f;
        private string byComponentName = "";
        private string byObjectName = "";
        private Material commonMaterial;
        private string folderName = "NewFolder";
        private GUIStyle headerTextStyle;
        private string insertionSuffixForName = "";
        private bool isActiveInHierarchy;
        private string namePart = "NamePart";
        private string newName = "NewName";
        private int numberOfInserts;
        private List<GameObject> objectsToInsertRemove;
        private readonly EditorList<GameObject> objectsToInsertRemoveEditorList = new(1);
        private List<Transform> optionalParents;
        private readonly EditorList<Transform> optionalParentsEditorList = new(1);
        private string prefix = "";
        private string replaceFor = "ReplaceFor";
        private bool saveCurrentSelection;
        private Vector2 scrollPosition;
        private int selectedAddition = 2;
        private bool selectInserted = true;
        private readonly float smallSpacePixelsCount = 5f;
        private string suffix = "";
        private int logLineLength = 120;
        private string hierarchyPath = "";
        private int entityId = 0;

        [MenuItem("Tools/UsefulTools/Tools Window", false, 0)]
        private static void Init()
        {
            var window = GetWindow<UsefulToolsWindow>();
            window.minSize = new Vector2(312, 50);
            window.titleContent = new GUIContent("UsefulTools");
            window.Show();
        }

        private void OnGUI()
        {
            InitHeaderStyle();

            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            DrawLogLine();
            DrawSelectEntityWithId();
            DrawInsertRemoveObjectsMenu();
            DrawSelectChildrenParenMenu();
            DrawGetHierarchyPathOfSelectedObject();
            DrawRenameSelectedObjectsMenu();
            DrawSelectGameObjectsMenu();
            DrawPackObjectsMenu();
            DrawSelectObjectsWithCommonMaterialMenu();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawLogLine()
        {
            GUILayout.Label("Log Line", headerTextStyle);
            logLineLength = EditorGUILayout.IntField("Line Length", logLineLength);

            if (GUILayout.Button("Log Line"))
            {
                var line = new StringBuilder();
                for (int i = 0; i < logLineLength; i++)
                    line.Append("-");

                Debug.Log(line);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawSelectEntityWithId()
        {
            GUILayout.Label("Select Entity With Id", headerTextStyle);
            entityId = EditorGUILayout.IntField("Entity ID", entityId);

            if (GUILayout.Button("Select Entity"))
            {
                SelectEntityWithId(entityId);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private static void SelectEntityWithId(int id)
        {
            var world = ME.ECS.Worlds.currentWorld;
            var currentEntity = world.GetEntityById(id);

            if (currentEntity.IsEmpty())
                Debug.LogError($"{nameof(UsefulToolsWindow)}.{nameof(DrawSelectEntityWithId)}> Entity with id{id} is empty!");
            else if (!currentEntity.IsAlive())
                Debug.LogError($"{nameof(UsefulToolsWindow)}.{nameof(DrawSelectEntityWithId)}> Entity with id{id} is not alive!");
            else
                WorldsViewerEditor.SelectEntity(currentEntity);
        }

        private void DrawSelectObjectsWithCommonMaterialMenu()
        {
            GUILayout.Label("Select Objects With Common Material", headerTextStyle);
            GUILayout.Label("Common Material");
            commonMaterial = EditorGUILayout.ObjectField(commonMaterial, typeof(Material), true) as Material;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search for activeInHierarchy:");
            isActiveInHierarchy = EditorGUILayout.Toggle(isActiveInHierarchy);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Select By Material"))
            {
                SelectObjectsByMaterial(commonMaterial, isActiveInHierarchy);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawGetHierarchyPathOfSelectedObject()
        {
            GUILayout.Label("Get Hierarchy Path Of Selected Object", headerTextStyle);

            EditorGUILayout.TextField("Hierarchy Path", hierarchyPath);

            if (GUILayout.Button("Get Path"))
            {
                hierarchyPath = GetHierarchyPathOfSelectedObject();
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawSelectChildrenParenMenu()
        {
            GUILayout.Label("Select relatives of current selected objects", headerTextStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Save current selection:");
            saveCurrentSelection = EditorGUILayout.Toggle(saveCurrentSelection);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Select Parents"))
            {
                SelectChildrenOrParentsOfSelectedObjects(false, saveCurrentSelection);
            }

            if (GUILayout.Button("Select Children"))
            {
                SelectChildrenOrParentsOfSelectedObjects(true, saveCurrentSelection);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawRenameSelectedObjectsMenu()
        {
            GUILayout.Label("Rename Selected Objects", headerTextStyle);
            GUILayout.Label("New Name");
            newName = EditorGUILayout.TextField(newName);
            if (GUILayout.Button("Rename"))
            {
                RenameSelectedObjects(newName);
            }

            GUILayout.Space(smallSpacePixelsCount);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add to name...");
            selectedAddition =
                GUILayout.SelectionGrid(selectedAddition, Options, Options.Length, EditorStyles.radioButton);

            GUILayout.EndHorizontal();
            additionToName = EditorGUILayout.TextField(additionToName);

            if (GUILayout.Button("Add to name..."))
            {
                AddToName(selectedAddition, additionToName);
            }

            GUILayout.Space(smallSpacePixelsCount);

            GUILayout.Label("Replace part of name...", headerTextStyle);
            namePart = EditorGUILayout.TextField("Name Part", namePart);
            replaceFor = EditorGUILayout.TextField("Replace For", replaceFor);
            if (GUILayout.Button("Replace"))
            {
                ReplaceNamePart(namePart, replaceFor);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawSelectGameObjectsMenu()
        {
            GUILayout.Label("Select GameObjects", headerTextStyle);
            GUILayout.Label("Search Root Transforms");
            optionalParentsEditorList.UpdateAndDraw(ref optionalParents);
            GUILayout.Space(smallSpacePixelsCount);
            GUILayout.Label("By Object Name or Name Part");
            byObjectName = EditorGUILayout.TextField(byObjectName);
            GUILayout.Space(smallSpacePixelsCount);
            GUILayout.Label("By Component Name");
            byComponentName = EditorGUILayout.TextField(byComponentName);

            if (GUILayout.Button("Select"))
            {
                SelectGameObjectsByNameAndComponent(byObjectName, byComponentName, optionalParents);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawInsertRemoveObjectsMenu()
        {
            GUILayout.Label("Insert/Remove objects (or prefab)", headerTextStyle);
            GUILayout.Label("Objects to insert/remove");

            objectsToInsertRemoveEditorList.UpdateAndDraw(ref objectsToInsertRemove);

            GUILayout.Space(smallSpacePixelsCount);

            numberOfInserts = EditorGUILayout.IntField("Number of inserts: ", numberOfInserts);
            if (numberOfInserts < 1)
            {
                numberOfInserts = 1;
            }

            insertionSuffixForName = EditorGUILayout.TextField("Insertion suffix: ", insertionSuffixForName);

            selectInserted = EditorGUILayout.Toggle("Select inserted", selectInserted);

            if (GUILayout.Button("Insert object into selected"))
            {
                InsertObjectsIntoSelected(objectsToInsertRemove.ToArray(),
                    numberOfInserts,
                    insertionSuffixForName,
                    selectInserted);
            }

            if (GUILayout.Button("Remove all objects by name from selected"))
            {
                DestroySelectedChildrenByObjectName(objectsToInsertRemove);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void DrawPackObjectsMenu()
        {
            GUILayout.Label("Pack objects", headerTextStyle);

            GUILayout.Label("Directory Name");
            folderName = EditorGUILayout.TextField(folderName);

            if (GUILayout.Button("Group selected gameobjects into directory"))
            {
                PackSelectedObjectsInFolder(folderName);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }

        private void InitHeaderStyle()
        {
            headerTextStyle = new GUIStyle(GUI.skin.label);
            headerTextStyle.fontSize = 14;
            headerTextStyle.stretchWidth = true;
        }

        private static void ReplaceNamePart(string part, string replacement)
        {
            var selectedGameObjects = Selection.gameObjects;

            if (selectedGameObjects.Length == 0)
            {
                ReplaceNamePartForAsset(part, replacement);
                return;
            }

            Undo.RecordObjects(selectedGameObjects, "Replace Part Of The Name");

            foreach (var go in selectedGameObjects)
                go.name = go.name.Replace(part, replacement);
        }

        private static void ReplaceNamePartForAsset(string part, string replacement)
        {
            string[] selectedObjectsGUIDs = Selection.assetGUIDs;

            foreach (string guid in selectedObjectsGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(path);
                if (fileName == null)
                {
                    continue;
                }

                string newFileName = fileName.Replace(part, replacement);
                string error = AssetDatabase.RenameAsset(path, newFileName);

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError(error);
                    Debug.Log($"Old Path = \"{path}\"");
                    Debug.Log($"Old File Name = \"{fileName}\"");
                    Debug.Log($"New File Name = \"{newFileName}\"");
                }
            }
        }

        private void AddToName(int indexOfSelectedAddition, string additionToName)
        {
            var selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selected, "Add to object's Name");

            switch (indexOfSelectedAddition)
            {
                case 0:
                    prefix = additionToName;
                    suffix = string.Empty;
                    break;

                case 1:
                    suffix = additionToName;
                    prefix = string.Empty;
                    break;

                default:
                    return;
            }

            foreach (var selectedObject in selected)
                selectedObject.name = string.Format("{0}{1}{2}", prefix, selectedObject.name, suffix);
        }

        private void SelectObjectsByMaterial(Material mateial, bool activeInHierarchy)
        {
            if (mateial == null)
            {
                return;
            }

            var properObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject))
               .Select(g => g as GameObject)
               .Where(
                    g =>
                    {
                        if (activeInHierarchy && g)
                        {
                            return g.activeInHierarchy;
                        }

                        return true;
                    })
               .Where(
                    g =>
                    {
                        var renderer = g.GetComponent<Renderer>();

                        if (renderer == null)
                        {
                            return false;
                        }

                        foreach (var sharedMaterial in renderer.sharedMaterials)
                        {
                            if (mateial.Equals(sharedMaterial))
                            {
                                return true;
                            }
                        }

                        return false;
                    })
               .ToArray();

            Selection.objects = properObjects.ToArray();
        }

        private string GetHierarchyPathOfSelectedObject()
        {
            var selected = Selection.gameObjects;

            if (selected.Length != 1)
            {
                return "Select one object to get its hierarchy path!";
            }

            var reversedResultList = new List<string>();

            foreach (var selectedObject in selected)
            {
                var current = selectedObject.transform;

                while (current.parent != null)
                {
                    reversedResultList.Add(current.name);
                    current = current.parent;
                }
            }

            if (reversedResultList.Count == 0)
            {
                return string.Empty;
            }

            if (reversedResultList.Count == 1)
            {
                return reversedResultList[0];
            }

            var result = new StringBuilder();

            for (int i = reversedResultList.Count - 1; i > 0; i--)
            {
                result.Append(reversedResultList[i])
                   .Append("/");
            }

            result.Append(reversedResultList[0]);

            return result.ToString();
        }

        private void SelectChildrenOrParentsOfSelectedObjects(bool selectChildrenNotParents, bool saveCurrentSelection)
        {
            var selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                return;
            }

            var nextSelected = new List<GameObject>();

            foreach (var selectedObject in selected)
            {
                int childCount = selectedObject.transform.childCount;
                var parent = selectedObject.transform.parent;

                if ((selectChildrenNotParents && childCount == 0) ||
                    (!selectChildrenNotParents && parent == null))
                {
                    nextSelected.Add(selectedObject);
                    continue;
                }

                if (saveCurrentSelection)
                {
                    nextSelected.Add(selectedObject);
                }

                if (selectChildrenNotParents)
                {
                    for (int i = 0; i < childCount; i++)
                        nextSelected.Add(selectedObject.transform.GetChild(i).gameObject);
                }
                else
                {
                    nextSelected.Add(selectedObject.transform.parent.gameObject);
                }
            }

            Selection.objects = nextSelected.ToArray();
        }

        private void SelectGameObjectsByNameAndComponent(string byObjectName, string byComponentName,
            List<Transform> optionalParents)
        {
            var properObjects = new List<Object>();

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

        private void RenameSelectedObjects(string newName)
        {
            var selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selected, "Rename Objects");

            foreach (var selectedObject in selected)
                selectedObject.name = newName;
        }

        private void PackSelectedObjectsInFolder(string folderName)
        {
            var selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                return;
            }

            var n = new GameObject(folderName);
            n.transform.position = selectedObjects[0].transform.position;
            n.transform.parent = selectedObjects[0].transform.parent;

            Undo.RegisterCreatedObjectUndo(n, "Create Folder GameObject");

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Undo.SetTransformParent(selectedObjects[i].transform, n.transform,
                    "Set folding object's parent to the Folder");
            }

            Selection.objects = new[] { n };
        }

        private void InsertObjectsIntoSelected(GameObject[] clonedObjects, int numberOfActions,
            string insertionSuffix,
            bool selectInserted)
        {
            if (clonedObjects.Length == 0)
            {
                Debug.Log("Objects to insert/remove is empty!");
                return;
            }

            foreach (var clonedObject in clonedObjects)
            {
                if (clonedObject == null)
                {
                    Debug.Log("Some Object to insert/remove is null!");
                    return;
                }
            }

            var selectedObjects = Selection.gameObjects;
            var clones = new List<GameObject>();

            foreach (var selectedObject in selectedObjects)
            {
                clones.AddRange(PrefabAsChildrenPaster.PastePrefabsAsChildren(
                    clonedObjects,
                    selectedObject.transform,
                    numberOfActions));
            }

            foreach (var clone in clones)
            {
                if (insertionSuffix != string.Empty)
                {
                    clone.name = clone.name + insertionSuffix;
                }

                clone.transform.localPosition = Vector3.zero;
            }

            if (selectInserted)
            {
                Selection.objects = clones.ToArray();
            }
        }

        private void DestroySelectedChildrenByObjectName(List<GameObject> objectsWithNameToDestroy)
        {
            if (objectsWithNameToDestroy.Count == 0)
            {
                Debug.Log("Objects to insert/remove is empty!");
                return;
            }

            foreach (var clonedObject in objectsWithNameToDestroy)
            {
                if (clonedObject == null)
                {
                    Debug.Log("Some Object to insert/remove is null!");
                    return;
                }
            }

            foreach (var objectWithNameToDestroy in objectsWithNameToDestroy)
            {
                string destroyName = objectWithNameToDestroy.name;

                var selected = Selection.gameObjects;

                foreach (var obj in selected)
                {
                    var childs = obj.transform.GetComponentsInChildren<Transform>();
                    foreach (var child in childs)
                    {
                        if (child && child.name.Equals(destroyName))
                        {
                            Undo.DestroyObjectImmediate(child.gameObject);
                        }
                    }
                }
            }
        }
    }
}