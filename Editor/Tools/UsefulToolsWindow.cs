using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.Tools
{
    [Serializable]
    public class TransformData
    {
        public Vector3 LocalPosition = Vector3.zero;
        public Vector3 LocalEulerRotation = Vector3.zero;
        public Vector3 LocalScale = Vector3.one;

        // Unity requires a default constructor for serialization
        public TransformData() { }

        public TransformData(Transform transform)
        {
            LocalPosition = transform.localPosition;
            LocalEulerRotation = transform.localEulerAngles;
            LocalScale = transform.localScale;
        }

        public void ApplyTo(Transform transform)
        {
            transform.localPosition = LocalPosition;
            transform.localEulerAngles = LocalEulerRotation ;
            transform.localScale = LocalScale;
        }
    }
    
    public class UsefulToolsWindow : EditorWindow
    {
        private static readonly string[] AddToNameOptions = new string[] { "Prefix", "Suffix", "Counter" };
        private static readonly string[] AddToTagOptions = new string[] { "Prefix", "Suffix" };
        private static readonly string[] TagComponentOptions = new string[] { "GE2", "Lua" };
        private string additionToName;
        private string additionToTag;
        private readonly float bigSpacePixelsCount = 20f;
        private string byComponentName = "";
        private string byObjectName = "";
        private Material commonMaterial;
        private string folderName = "NewFolder";
        private GUIStyle headerTextStyle;
        private string insertionSuffixForName = "";
        private bool isActiveInHierarchy;
        private string namePart = "NamePart";
        private string tagPart = "NamePart";
        private string newName = "NewName";
        private string setTag = "NewName";
        private int numberOfInserts;
        private List<GameObject> objectsToInsertRemove;
        private readonly EditorList<GameObject> objectsToInsertRemoveEditorList = new EditorList<GameObject>(1);
        private List<Transform> optionalParents;
        private readonly EditorList<Transform> optionalParentsEditorList = new EditorList<Transform>(1);
        private string prefix = "";
        private string replaceNamePartFor = "ReplaceFor";
        private string replaceTagPartFor = "ReplaceFor";
        private bool saveCurrentSelection;
        private Vector2 scrollPosition;
        private int selectedNameAdditionIndex = 0;
        private int selectedTagAdditionIndex = 0;
        private int selectedTagComponentIndex = 0;
        private bool selectInserted = true;
        private readonly float smallSpacePixelsCount = 5f;
        private string suffix = "";
        private int logLineLength = 120;
        private string hierarchyPath = "";
        private string childrenCounterNumberFormat = "";
        private Vector3 addToPosition = Vector3.zero;
        private List<Transform> copyFromTransforms;
        private EditorList<Transform> copyFromTransformsEditorList = new EditorList<Transform>(0);
        private List<Transform> pasteToTransforms;
        private EditorList<Transform> pasteToTransformsEditorList = new EditorList<Transform>(0);
        
        private ITagHelper tagHelper = new PlayStudiosTagHelper();

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
            DrawInsertRemoveObjectsMenu();
            DrawSelectChildrenParenMenu();
            DrawGetHierarchyPathOfSelectedObject();
            DrawRenameSelectedObjectsMenu();
            DrawEditTagOfSelectedObjectsMenu();
            DrawSelectGameObjectsMenu();
            DrawPackObjectsMenu();
            DrawSelectObjectsWithCommonMaterialMenu();
            DrawTransformManipulationsMenu();

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
        
        private void DrawTransformManipulationsMenu()
        {
            GUILayout.Label("Update Transform", headerTextStyle);
            
            GUILayout.BeginHorizontal();

            var addToPositionLabel = new GUIContent("Add to current position");
            addToPosition = EditorGUILayout.Vector3Field(addToPositionLabel, addToPosition);
            
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add to position"))
            {
                AddToPositionForSelectedObjects(addToPosition);
            }
            
            GUILayout.Space(smallSpacePixelsCount);
            
            GUILayout.BeginHorizontal();

            var addCounterLabel = new GUIContent("Counter Number Format","Counter Number Format: e.g. #0,000.00; live empty if simple counter is enough.");
            childrenCounterNumberFormat = EditorGUILayout.TextField(addCounterLabel, childrenCounterNumberFormat);
            
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add counter to children"))
            {
                AddCounterToNameForChildrenOfSelectedObjects(childrenCounterNumberFormat);
            }
            
            GUILayout.Space(smallSpacePixelsCount);
            
            GUILayout.Label("Copy/Paste transforms", headerTextStyle);
           
            GUILayout.Label("Copy From Transforms");
            copyFromTransformsEditorList.UpdateAndDraw(ref copyFromTransforms);
            
            if (GUILayout.Button("Add Selection to Copy List"))
            {
                AddSelectedToTransformsList(ref copyFromTransforms);
                copyFromTransformsEditorList.UpdateAndDraw(ref copyFromTransforms);
            }
            
            GUILayout.Space(smallSpacePixelsCount);
            
            GUILayout.Label("Paste To Transforms");
            pasteToTransformsEditorList.UpdateAndDraw(ref pasteToTransforms);
            
            if (GUILayout.Button("Add Selection to Paste List"))
            {
                AddSelectedToTransformsList(ref pasteToTransforms);
                pasteToTransformsEditorList.UpdateAndDraw(ref pasteToTransforms);
            }
            
            GUILayout.Space(smallSpacePixelsCount);
            
            if (GUILayout.Button("Copy & Paste Transforms"))
            {
                CopyPasteTransforms(copyFromTransforms.ToArray(), pasteToTransforms.ToArray());
            }
            
            if (GUILayout.Button("Clear Copy & Paste Lists"))
            {
                copyFromTransforms.Clear();
                pasteToTransforms.Clear();
                
                copyFromTransformsEditorList.UpdateAndDraw(ref copyFromTransforms);
                pasteToTransformsEditorList.UpdateAndDraw(ref pasteToTransforms);
            }
            
            GUILayout.Space(bigSpacePixelsCount);
        }
        
        private void AddToPositionForSelectedObjects(Vector3 addToPosition)
        {
            var selected = Selection.transforms;

            if (selected.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selected, "Move Objects");

            foreach (var selectedTransform in selected)
            {
                var currentPosition = selectedTransform.position;
                var newPosition = currentPosition + addToPosition;
                selectedTransform.position = newPosition;
            }
        }
        
        private void AddCounterToNameForChildrenOfSelectedObjects(string numberFormat)
        {
            var selected = Selection.transforms;

            if (selected.Length == 0)
            {
                Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(AddToName)}></color> "
                    + "No selection");
                return;
            }
            
            foreach (var selectedTransform in selected)
            {
                int additionCounter = 0;
                
                Undo.RecordObjects(selected, "Add counter to object's Name");

                for (var i = 0; i < selectedTransform.childCount; i++)
                {
                    var child = selectedTransform.GetChild(i);
                    
                    string currentName = child.name;
                    string additionCounterString = string.IsNullOrEmpty(numberFormat) 
                        ? additionCounter.ToString() 
                        : additionCounter.ToString(numberFormat);

                    string newName = $"{currentName}{additionCounterString}";
                    child.name = newName;
                    additionCounter++;
                }
            }
        }

        private void AddSelectedToTransformsList(ref List<Transform> transformsList)
        {
            var selectedTransforms = Selection.transforms;

            if (selectedTransforms.Length == 0)
            {
                return;
            }

            bool haveSameParent = true;
            var commonParent = selectedTransforms[0].parent;
            
            foreach (var selectedTransform in selectedTransforms)
            {
                if (commonParent != selectedTransform.parent)
                {
                    haveSameParent = false;
                    break;
                }
            }

            if (haveSameParent)
            {
                for (int i = 0; i < commonParent.childCount; i++)
                {
                    var child = commonParent.GetChild(i);
                    
                    foreach (var selectedTransform in selectedTransforms)
                    {
                        if (child == selectedTransform)
                        {
                            transformsList.Add(child);
                        }
                    }
                }
            }
            else
            {
                foreach (var selectedTransform in selectedTransforms)
                {
                    transformsList.Add(selectedTransform);
                }
            }
        }
        
        private void CopyPasteTransforms(Transform[] copyFromTransforms, Transform[] pasteToTransforms)
        {
            if (copyFromTransforms.Length != pasteToTransforms.Length)
            {
                Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(CopyPasteTransforms)}></color> "
                    + "Number of copied from transforms is not equal to number of pasted to transforms!");
                return;
            }
            
            if (copyFromTransforms.Length == 0)
            {
                Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(CopyPasteTransforms)}></color> "
                    + "Nothing to copy!");
                return;
            }
            
            if (pasteToTransforms.Length == 0)
            {
                Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(CopyPasteTransforms)}></color> "
                    + "Nowhere to paste!");
                return;
            }

            foreach (var t in copyFromTransforms)
            {
                if (t == null)
                {
                    Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(CopyPasteTransforms)}></color> "
                        + "Some of copied transforms are null!");
                    return;
                }
            }
            
            foreach (var t in pasteToTransforms)
            {
                if (t == null)
                {
                    Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(CopyPasteTransforms)}></color> "
                        + "Some of pasted transforms are null!");
                    return;
                }
            }

            for (int i = 0; i < copyFromTransforms.Length; i++)
            {
                var transformData = new TransformData(copyFromTransforms[i]);
                transformData.ApplyTo(pasteToTransforms[i]);
            }
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
            selectedNameAdditionIndex = GUILayout.SelectionGrid(selectedNameAdditionIndex, AddToNameOptions, AddToNameOptions.Length, EditorStyles.radioButton);

            GUILayout.EndHorizontal();
            additionToName = EditorGUILayout.TextField(additionToName);

            if (GUILayout.Button("Add to name..."))
            {
                AddToName(selectedNameAdditionIndex, additionToName);
            }

            GUILayout.Space(smallSpacePixelsCount);

            GUILayout.Label("Replace part of name...", headerTextStyle);
            namePart = EditorGUILayout.TextField("Name Part", namePart);
            replaceNamePartFor = EditorGUILayout.TextField("Replace For", replaceNamePartFor);
            
            if (GUILayout.Button("Replace name part"))
            {
                ReplaceNamePart(namePart, replaceNamePartFor);
            }

            GUILayout.Space(bigSpacePixelsCount);
        }
        
        private void DrawEditTagOfSelectedObjectsMenu()
        {
            GUILayout.Label("Set Tag for Selected Objects", headerTextStyle);
            
            selectedTagComponentIndex = GUILayout.SelectionGrid(selectedTagComponentIndex, TagComponentOptions, TagComponentOptions.Length, EditorStyles.radioButton);
            string selectedTagComponent = TagComponentOptions[selectedTagComponentIndex];
            GUILayout.Space(smallSpacePixelsCount);
            
            GUILayout.Label("New Tag");
            setTag = EditorGUILayout.TextField(setTag);
            if (GUILayout.Button("Set Tag"))
            {
                SetTagForSelectedObjects(setTag, selectedTagComponent);
            }

            GUILayout.Space(smallSpacePixelsCount);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add to tag...");
            selectedTagAdditionIndex = GUILayout.SelectionGrid(selectedTagAdditionIndex, AddToTagOptions, AddToTagOptions.Length, EditorStyles.radioButton);

            GUILayout.EndHorizontal();
            additionToTag = EditorGUILayout.TextField(additionToTag);

            if (GUILayout.Button("Add to tag..."))
            {
                AddToTag(selectedTagAdditionIndex, additionToTag, selectedTagComponent);
            }

            GUILayout.Space(smallSpacePixelsCount);

            GUILayout.Label("Replace part of tag...", headerTextStyle);
            tagPart = EditorGUILayout.TextField("Tag Part", tagPart);
            replaceTagPartFor = EditorGUILayout.TextField("Replace For", replaceTagPartFor);
            
            if (GUILayout.Button("Replace tag part"))
            {
                ReplaceTagPart(tagPart, replaceNamePartFor, selectedTagComponent);
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
        
        private void ReplaceTagPart(string part, string replacement, string selectedComponent)
        {
            var selectedGameObjects = Selection.gameObjects;

            if (selectedGameObjects.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selectedGameObjects, "Replace Part Of The Tag");

            foreach (var selectedObject in selectedGameObjects)
            {
                string currentTag = tagHelper.GetTagStringFromObject(selectedObject, selectedComponent);
                string newTag = currentTag.Replace(part, replacement);
                tagHelper.SetTagStringForObject(newTag, selectedObject, selectedComponent);
            }
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
        
        private void AddToTag(int indexOfSelectedAddition, string additionToTag, string selectedComponent)
        {
            var selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selected, "Add to object's Tag");

            switch (indexOfSelectedAddition)
            {
                case 0:
                    prefix = additionToTag;
                    suffix = string.Empty;
                    break;

                case 1:
                    suffix = additionToTag;
                    prefix = string.Empty;
                    break;
            }

            foreach (var selectedObject in selected)
            {
                string currentTag = tagHelper.GetTagStringFromObject(selectedObject, selectedComponent);
                string newTag = $"{prefix}{currentTag}{suffix}";
                tagHelper.SetTagStringForObject(newTag, selectedObject, selectedComponent);
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
                
                case 2:
                    int additionCounter = 0;

                    var commonParent = selected[0].transform.parent;
                    
                    foreach (var selectedObject in selected)
                    {
                        string currentName = selectedObject.name;
                        string additionCounterString = string.IsNullOrEmpty(additionToName) 
                            ? additionCounter.ToString() 
                            : additionCounter.ToString(additionToName);

                        string newName = $"{currentName}{additionCounterString}";

                        selectedObject.name = newName;
                        
                        additionCounter++;
                    }
                    
                    return;

                default:
                    return;
            }

            foreach (var selectedObject in selected)
            {
                string currentName = selectedObject.name;
                selectedObject.name = $"{prefix}{currentName}{suffix}";
            }
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
        
        private void SetTagForSelectedObjects(string newTag, string selectedComponent)
        {
            var selected = Selection.gameObjects;

            if (selected.Length == 0)
            {
                return;
            }

            Undo.RecordObjects(selected, "Set Tag for Objects");

            foreach (var selectedObject in selected)
            {
                tagHelper.SetTagStringForObject(newTag, selectedObject, selectedComponent);
            }
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