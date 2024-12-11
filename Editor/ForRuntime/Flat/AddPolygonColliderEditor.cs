using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor.Flat
{
    [CustomEditor(typeof(AddPolygonCollider))]
    public class AddPolygonColliderEditor : UnityEditor.Editor
    {
        private DestroyMe _destroyMe;
        private AddPolygonCollider _target;

        private readonly List<Vector2> points = new List<Vector2>();
        private readonly List<Vector2> simplifiedPoints = new List<Vector2>();

        private SpriteRenderer _spriteRenderer;

        public override void OnInspectorGUI()
        {
            _target = target as AddPolygonCollider;
            if (_target == null || _destroyMe != null)
            {
                return;
            }

            _target.SpriteRenderer ??= FindSpriteRenderer(_target.gameObject);
            _spriteRenderer = _target.SpriteRenderer;

            DrawInspector();
        }

        private void DrawInspector()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Add Polygon Collider 2D"))
            {
                AddCollider();
            }
        }

        private void AddCollider()
        {
            SetupPolygonCollider();
            SetupExtraScripts();

            _destroyMe = new GameObject().AddComponent<DestroyMe>();
            _destroyMe.ToDestroy = _target;
            Selection.objects = new[] { _destroyMe.gameObject };
        }

        private void SetupPolygonCollider()
        {
            var polygonCollider = _target.gameObject.GetComponent<PolygonCollider2D>();

            if (polygonCollider == null)
            {
                polygonCollider = _target.gameObject.AddComponent<PolygonCollider2D>();
            }

            SetPath(ref polygonCollider);
        }

        private void SetupExtraScripts()
        {
//			var raycastFilter = _target.gameObject.GetComponent<ColliderRaycastFilter>();
//			if (raycastFilter == null)
//			{
//				_target.gameObject.AddComponent<ColliderRaycastFilter>();
//			}
        }

        private void SetPath(ref PolygonCollider2D polygonCollider)
        {
            var sprite = GetAttachedSprite(_spriteRenderer);

            if (sprite != null)
            {
                SetupPolygonCollider2DFromSprite(ref polygonCollider, sprite);
            }
            else
            {
                Debug.LogWarning("Sprite Renderer not found: setting default path");

                float edge = 150;
                var points = new[]
                {
                    new Vector2(-edge, -edge / 1.41421f),
                    new Vector2(0f, edge),
                    new Vector2(edge, -edge / 1.41421f)
                };

                polygonCollider.points = points;
            }
        }

        private Sprite GetAttachedSprite(SpriteRenderer spriteRenderer) => spriteRenderer != null ? spriteRenderer.sprite : null;

        private void SetupPolygonCollider2DFromSprite(ref PolygonCollider2D polygonCollider2D, Sprite sprite, float tolerance = 0.05f)
        {
            polygonCollider2D.pathCount = sprite.GetPhysicsShapeCount();

            for (var i = 0; i < polygonCollider2D.pathCount; i++)
            {
                sprite.GetPhysicsShape(i, points);
                LineUtility.Simplify(points, tolerance, simplifiedPoints);
                polygonCollider2D.SetPath(i, simplifiedPoints);
            }
        }

        private SpriteRenderer FindSpriteRenderer(GameObject gameObject)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                return spriteRenderer;
            }

            return gameObject.GetComponentInChildren<SpriteRenderer>(true);
        }
    }
}