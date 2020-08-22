﻿using UnityEditor;
using UnityEngine;

namespace UsefulTools.Flat.Editor {
	[CustomEditor(typeof(DestroyMe))]
	public class DestroyMeEditor : UnityEditor.Editor {
		static DestroyMe    _target;
		static GameObject[] _selection;

		public override void OnInspectorGUI() {
			if ( _target ) {
				DestroyImmediate(_target.gameObject);
				Selection.objects = _selection;
				return;
			}

			_target = target as DestroyMe;
			_selection = new[] {_target.ToDestroy.gameObject};

			DestroyImmediate(_target.ToDestroy);
		}
	}
}