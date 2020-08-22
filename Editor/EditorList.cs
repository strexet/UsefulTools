using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    [Serializable]
    public class EditorList<T> where T : UnityEngine.Object
    {
        private int _count;
        private readonly List<T> _list;

        public int Count => _count;

        public EditorList(int listCapacity) {
            _list = new List<T>(listCapacity);
            _count = listCapacity;

            for ( var i = 0; i < listCapacity; i++ ) {
                AddEmpty();
            }
        }

        public void AddEmpty() {
            _list.Add(default(T));
        }

        public void UpdateAndDraw(ref List<T> _objectsToInsertRemove) {
            _objectsToInsertRemove = _list;

            var newCount = -1;

            if ( GUILayout.Button("Reset") ) {
                newCount = 1;
                _objectsToInsertRemove.Clear();
                _objectsToInsertRemove.Add(default);
                _count = 1;
            }

            GUILayout.BeginHorizontal();

            if ( newCount < 0 ) {
                newCount = Mathf.Max(
                    0,
                    EditorGUILayout.DelayedIntField("Count:", _objectsToInsertRemove.Count));
            }


            if ( GUILayout.Button("+", GUILayout.Width(20)) ) {
                newCount++;
            }

            GUILayout.EndHorizontal();

            while ( newCount < _objectsToInsertRemove.Count ) {
                _objectsToInsertRemove.RemoveAt(_objectsToInsertRemove.Count - 1);
            }

            while ( newCount > _objectsToInsertRemove.Count ) {
                var objectToAdd = default(T);
                var count = _objectsToInsertRemove.Count;

                if ( count > 0 ) {
                    objectToAdd = _objectsToInsertRemove[count - 1];
                }

                _objectsToInsertRemove.Add(objectToAdd);
            }

            for ( var i = 0; i < _objectsToInsertRemove.Count; i++ ) {
                _objectsToInsertRemove[i] = EditorGUILayout.ObjectField(
                    $"Element {i}:",
                    _objectsToInsertRemove[i],
                    typeof(T),
                    true) as T;
            }
			
			

        }
		
		
    }
}