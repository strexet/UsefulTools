using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.Tools
{
    [Serializable]
    public class EditorList<T> where T : Object
    {
        readonly List<T> list;
        int count;

        public int Count => count;

        public EditorList(int listCapacity)
        {
            list = new List<T>(listCapacity);
            count = listCapacity;

            for (var i = 0; i < listCapacity; i++)
            {
                AddEmpty();
            }
        }

        public void AddEmpty()
        {
            list.Add(default);
        }

        public void UpdateAndDraw(ref List<T> objectsToInsertRemove)
        {
            objectsToInsertRemove = list;

            var newCount = -1;

            if (GUILayout.Button("Reset"))
            {
                newCount = 1;
                objectsToInsertRemove.Clear();
                objectsToInsertRemove.Add(default(T));
                count = 1;
            }

            GUILayout.BeginHorizontal();

            if (newCount < 0)
            {
                newCount = Mathf.Max(
                    0,
                    EditorGUILayout.DelayedIntField("Count:", objectsToInsertRemove.Count));
            }

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                newCount++;
            }

            GUILayout.EndHorizontal();

            while (newCount < objectsToInsertRemove.Count)
            {
                objectsToInsertRemove.RemoveAt(objectsToInsertRemove.Count - 1);
            }

            while (newCount > objectsToInsertRemove.Count)
            {
                var objectToAdd = default(T);
                var count = objectsToInsertRemove.Count;

                if (count > 0)
                {
                    objectToAdd = objectsToInsertRemove[count - 1];
                }

                objectsToInsertRemove.Add(objectToAdd);
            }

            for (var i = 0; i < objectsToInsertRemove.Count; i++)
            {
                objectsToInsertRemove[i] = EditorGUILayout.ObjectField(
                    $"Element {i}:",
                    objectsToInsertRemove[i],
                    typeof(T),
                    true) as T;
            }
        }
    }
}