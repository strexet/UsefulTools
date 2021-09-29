using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.PrefabApiTest
{
    public class PrefabApiTest : EditorWindow
    {
        private Object _testingPrefab;
        private Object _currentDisplayingPrefab;
        private PrefabHelper.PrefabProperties _currentPrefabProperties;

        private Object _extraSelectPrefab;
        private string _extraSelectInnerHierarchyPath;

        [MenuItem("Tools/UsefulTools/Prefabs/Prefab API tester")]
        private static void OpenConfig() =>
            GetWindow<PrefabApiTest>(true, "Prefab API tester");

        private void OnGUI()
        {
            _DisplayApiExample();
            _DisplayExtraSelect();
        }

        private void _DisplayExtraSelect()
        {
            GUILayout.Space(40);

            EditorGUILayout.HelpBox(
                "Extra sample\n" +
                "This will select gameobject node under CubePlayer/LeftHand/Gun in PrefabAsset: Assets/Prefabs/CubePlayer.prefab\n" +
                "Which is not possible to select by hand in newer version of unity.\n" +
                "This will demonstrate prefab instance nested under prefab asset.", MessageType.Info);

            _extraSelectPrefab = EditorGUILayout.ObjectField(
                "Select GameObject",
                _extraSelectPrefab,
                typeof(GameObject),
                true
            );

            _extraSelectInnerHierarchyPath = EditorGUILayout.TextField("Inner Hierarchy Path", _extraSelectInnerHierarchyPath);

            if (GUILayout.Button("Select by inner path from prefab"))
            {
                var selectedPrefabProperties = PrefabHelper.GetPrefabProperties(_extraSelectPrefab as GameObject);
                string selectedPrefabPath = selectedPrefabProperties.prefabAssetPath;

                var selectedPrefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(selectedPrefabPath);
                _testingPrefab = selectedPrefabGameObject.transform.Find(_extraSelectInnerHierarchyPath).gameObject;
            }
        }

        private void _DisplayApiExample()
        {
            string helpText =
                "TRYME! Try to drag game object from various places and Display prefab properties.\n" +
                "- From scene\n" +
                "- From asset folder\n" +
                "- From prefab editing stage\n" +
                "- Nested prefab, child of nested prefab, non-prefab\n" +
                "Output below.";

            EditorGUILayout.HelpBox(helpText, MessageType.Info);

            _testingPrefab = EditorGUILayout.ObjectField(
                "Testing GameObject",
                _testingPrefab,
                typeof(GameObject),
                true
            );

            if (_testingPrefab == null)
                _currentDisplayingPrefab = null;

            if (_testingPrefab != _currentDisplayingPrefab || _currentPrefabProperties == null)
            {
                _currentDisplayingPrefab = _testingPrefab;
                _currentPrefabProperties = PrefabHelper.GetPrefabProperties(_testingPrefab as GameObject);
            }

            if (_currentPrefabProperties == null)
                return;

            // Display output
            DisplayReportText();
            DisplayReportObjects();
        }

        private void DisplayReportText()
        {
            var prefabProperties = _currentPrefabProperties;
            var report = new StringBuilder();

            if (prefabProperties.isPartOfPrefabStage)
            {
                report.Append("This is ");
                report.Append(prefabProperties.isPrefabStageRoot
                    ? "root of "
                    : "child of ");

                report.Append("prefabStage");
                report.Append("\n");
            }

            if (prefabProperties.isPartOfPrefabAsset)
            {
                report.Append("This is ");
                report.Append(prefabProperties.isPrefabAssetRoot
                    ? "root of "
                    : "child of ");

                report.Append("prefabAsset");
                report.Append("\n");
            }

            if (prefabProperties.isPartOfPrefabInstance)
            {
                report.Append("This is ");
                report.Append(prefabProperties.isPrefabInstanceRoot
                    ? "root of "
                    : "child of ");

                report.Append("prefabInstance");
                report.Append("\n");
            }

            if (prefabProperties.isPrefabAssetVariant)
            {
                report.Append("This is variant");
                report.Append("\n");
            }

            if (prefabProperties.isSceneObject)
            {
                report.Append("This is sceneObject");
                report.Append("\n");
            }

            report.Append("nearest AssetPath: ");
            report.Append(prefabProperties.prefabAssetPath);
            report.Append("\n");

            if (prefabProperties.prefabAssetRoot != null)
            {
                report.Append("Prefab asset root is: ");
                report.Append(prefabProperties.prefabAssetRoot.name);
                report.Append("\n");
            }

            EditorGUILayout.HelpBox(report.ToString(), MessageType.None);
        }

        private void DisplayReportObjects()
        {
            var prefabProperties = _currentPrefabProperties;

            if (prefabProperties.nearestInstanceRoot != null)
            {
                EditorGUILayout.LabelField("Resolved nearest instance root");
                EditorGUI.indentLevel += 1;
                GUILayout.Label(Helper.GetGameObjectPath(prefabProperties.nearestInstanceRoot));
                GUI.enabled = false;
                EditorGUILayout.ObjectField(prefabProperties.nearestInstanceRoot, typeof(GameObject), true);
                GUI.enabled = true;
                EditorGUI.indentLevel -= 1;
            }

            if (prefabProperties.prefabAssetRoot != null)
            {
                EditorGUILayout.LabelField("Resolved prefab asset root, this object was from this asset:");
                EditorGUI.indentLevel += 1;
                GUILayout.Label(Helper.GetGameObjectPath(prefabProperties.prefabAssetRoot));
                GUI.enabled = false;
                EditorGUILayout.ObjectField(prefabProperties.prefabAssetRoot, typeof(GameObject), false);
                GUI.enabled = true;
                EditorGUI.indentLevel -= 1;
            }

            int level = 1;
            var oneLevelUp = prefabProperties.GetSourcePrefab();
            while (oneLevelUp != null)
            {
                if (level > 10)
                {
                    GUILayout.Label("AND MORE!....");
                    break;
                }

                EditorGUILayout.LabelField($"Sourced from ({level} level up), this object was made from:");
                EditorGUI.indentLevel += 1;
                GUILayout.Label(Helper.GetGameObjectPath(oneLevelUp));
                GUI.enabled = false;
                EditorGUILayout.ObjectField(oneLevelUp, typeof(GameObject), false);
                GUI.enabled = true;
                EditorGUI.indentLevel -= 1;

                // To next
                level++;
                // This is the function behind GetSourcePrefab
                oneLevelUp = PrefabUtility.GetCorrespondingObjectFromSource(oneLevelUp);
            }
        }

        private static class Helper
        {
            /// <summary>
            ///     Get game object path in hierarchy.
            /// </summary>
            public static string GetGameObjectPath(GameObject gameObject)
            {
                var path = new List<string>();
                var iter = gameObject.transform;
                while (iter != null)
                {
                    if (path.Count > 20)
                        break; // Safety measure

                    path.Insert(0, iter.name);
                    iter = iter.parent;
                }

                var scene = gameObject.scene;
                if (scene.isLoaded && !string.IsNullOrEmpty(scene.name))
                    path.Insert(0, $"{scene.name}:");
                else
                    path.Insert(0, "NoScene:");

                return string.Join("/", path.ToArray());
            }
        }
    }
}