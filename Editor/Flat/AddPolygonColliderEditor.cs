using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.Flat
{
    [CustomEditor(typeof(AddPolygonCollider))]
    public class AddPolygonColliderEditor : UnityEditor.Editor
    {
        private DestroyMe _destroyMe;
        private AddPolygonCollider _target;

        public override void OnInspectorGUI()
        {
            _target = target as AddPolygonCollider;
            if (_target == null || _destroyMe != null)
            {
                return;
            }

            var polygonCollider = _target.gameObject.GetComponent<PolygonCollider2D>();
            if (polygonCollider == null)
            {
                polygonCollider = _target.gameObject.AddComponent<PolygonCollider2D>();
            }

            AddExtraScripts();

            float edge = 150;
            var points = new[]
            {
                new Vector2(-edge, -edge / 1.41421f),
                new Vector2(0f, edge),
                new Vector2(edge, -edge / 1.41421f)
            };

            polygonCollider.points = points;

            _destroyMe = new GameObject().AddComponent<DestroyMe>();
            _destroyMe.ToDestroy = _target;
            Selection.objects = new[] { _destroyMe.gameObject };
        }

        private void AddExtraScripts()
        {
//			var raycastFilter = _target.gameObject.GetComponent<ColliderRaycastFilter>();
//			if (raycastFilter == null)
//			{
//				_target.gameObject.AddComponent<ColliderRaycastFilter>();
//			}
        }
    }
}