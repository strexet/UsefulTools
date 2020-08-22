using System;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulTools.Performance {
	public class SlowUpdateManager : MonoBehaviour {
		static SlowUpdateManager _instance;
		List<SlowUpdatedTask>    _tasks;

		public static SlowUpdatedTask CreateTask(float intervalSeconds, Action action) {
			if ( _instance == null ) {
				Init();
			}

			var task = new SlowUpdatedTask(intervalSeconds, action, _instance);
			_instance._tasks.Add(task);

			return task;
		}

		public static void Clear() {
			_instance._tasks.Clear();
			Destroy(_instance);
		}

		static void Init() {
			_instance = new GameObject("SlowUpdateManager").AddComponent<SlowUpdateManager>();
			DontDestroyOnLoad(_instance.gameObject);
			_instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			_instance._tasks = new List<SlowUpdatedTask>();
		}

		void Update() {
			for ( int i = 0; i < _tasks.Count; i++ ) {
				_tasks[i].Update();
			}
		}

		public void DestroyTask(SlowUpdatedTask task) {
			_tasks.Remove(task);
		}
	}
}