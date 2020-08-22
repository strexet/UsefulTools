using System;
using UnityEngine;

namespace UsefulTools.Performance {
	public class SlowUpdatedTask {
		readonly Action            _action;
		readonly float             _intervalSeconds;
		readonly SlowUpdateManager _manager;

		static float _currentTime;
		float        _nextUpdateTime;


		public SlowUpdatedTask(float intervalSeconds, Action action, SlowUpdateManager manager) {
			_intervalSeconds = intervalSeconds;
			_action = action;
			_manager = manager;
		}

		public void Update() {
			_currentTime = Time.realtimeSinceStartup;

			if ( _currentTime < _nextUpdateTime ) {
				return;
			}

			_nextUpdateTime = _currentTime + _intervalSeconds;
			_action.Invoke();
		}

		public void Destroy() {
			_manager.DestroyTask(this);
		}
	}
}