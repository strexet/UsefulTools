using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UsefulTools.Runtime.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ClipMaskRenderer : MonoBehaviour
    {
        [SerializeField] private CanvasRenderer _targetRenderer;
        [SerializeField] private RectTransform _clipMask;

        private Dictionary<Transform, Canvas> _cachedCanvases;

        private Vector2 _lastPosition;
        private Vector2 _currentPosition;

        private void OnValidate()
        {
            _targetRenderer = GetComponent<CanvasRenderer>();

            var parent = transform.parent;
            while (parent != null && parent.GetComponent<Mask>() == null)
                parent = parent.parent;

            _clipMask = parent?.GetComponent<RectTransform>();
        }

        private void Awake()
        {
            Vector2 position = _clipMask.position;
            _currentPosition = position;
            _lastPosition = position;

            _cachedCanvases = new Dictionary<Transform, Canvas>();
        }

        private void Update()
        {
            _currentPosition = _clipMask.position;

            if (_currentPosition != _lastPosition)
            {
                SetTargetClippingRect();
                _lastPosition = _currentPosition;
            }
        }

        private void OnEnable()
        {
            SetTargetClippingRect();
        }

        private void OnDisable()
        {
            _targetRenderer.DisableRectClipping();
        }

        private void SetTargetClippingRect()
        {
            var rect = _clipMask.rect;
            Vector2 offset = _clipMask.localPosition;

            var parent = _clipMask.parent;

            while (IsRootCanvas(GetCanvas(parent)) == false)
            {
                offset += (Vector2)parent.localPosition;
                parent = parent.parent;
            }

            rect.x += offset.x;
            rect.y += offset.y;

            _targetRenderer.EnableRectClipping(rect);
        }

        private Canvas GetCanvas(Transform t)
        {
            if (_cachedCanvases.TryGetValue(t, out var canvas))
            {
                return canvas;
            }

            canvas = t.gameObject.GetComponent<Canvas>();

            if (canvas == null)
            {
                canvas = null;
            }

            _cachedCanvases.Add(t, canvas);
            return canvas;
        }

        private static bool IsRootCanvas(Canvas canvas) =>
            ReferenceEquals(canvas, null) == false &&
            canvas.isRootCanvas;
    }
}