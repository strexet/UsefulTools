using System.Collections.Generic;
using UnityEngine;

namespace UsefulTools
{
    [ExecuteAlways]
    public class ObjectsPlacer : MonoBehaviour
    {
        public GameObject _placedPrefab;

        [Space]
        public List<Transform> _placedTransforms;
        public float _distanceBetween;
        public float _fullLength;
        public Vector3 _placeWorldDirection;

        private int _count;

        private void Update()
        {
            _placeWorldDirection.Normalize();

            if (_placedPrefab == null)
            {
                return;
            }

            ValidateDistance();

            if (_fullLength == 0 || _distanceBetween == 0)
            {
                return;
            }

            _count = Mathf.FloorToInt(_fullLength / _distanceBetween);
            _placedTransforms ??= new List<Transform>(_count);

            AddChildObjects();
            UpdateChildObjectsCount();

            if (_count == 0)
            {
                return;
            }

            PlaceChildObjects();
        }

        private void ValidateDistance()
        {
            if (_fullLength < 0)
            {
                _fullLength = 0;
            }

            if (_distanceBetween < 0)
            {
                _distanceBetween = 0;
            }

            if (_distanceBetween > _fullLength)
            {
                _distanceBetween = _fullLength;
            }
        }

        private void AddChildObjects()
        {
            if (_placedTransforms.Count != transform.childCount)
            {
                _placedTransforms.Clear();

                for (int i = 0; i < transform.childCount; i++)
                    _placedTransforms.Add(transform.GetChild(i));
            }
        }

        private void UpdateChildObjectsCount()
        {
            if (_placedTransforms.Count != _count)
            {
                foreach (var t in _placedTransforms)
                    DestroyImmediate(t.gameObject);

                _placedTransforms.Clear();

                for (int i = 0; i < _count; i++)
                {
                    var obj = Instantiate(_placedPrefab, transform);
                    obj.SetActive(true);
                    _placedTransforms.Add(obj.transform);
                }
            }
        }

        private void PlaceChildObjects()
        {
            bool canDivideBy2 = _count % 2 == 0;

            var center = transform.position;

            int signCount = 0;
            int placeCount = 0;
            int startIndex = 0;

            Vector3 singleOffset;
            Vector3 startOffset;

            if (canDivideBy2)
            {
                singleOffset = _distanceBetween * _placeWorldDirection;
                startOffset = 0.5f * _distanceBetween * _placeWorldDirection;
            }
            else
            {
                _placedTransforms[0].position = center;
                placeCount++;
                startIndex++;

                singleOffset = placeCount * _distanceBetween * _placeWorldDirection;
                startOffset = Vector3.zero;
            }

            for (int i = startIndex; i < _placedTransforms.Count; i++)
            {
                var t = _placedTransforms[i];

                int sign = signCount % 2 == 0 ? 1 : -1;
                var offset = sign * (startOffset + placeCount * singleOffset);

                t.position = center + offset;

                if (sign < 0)
                {
                    placeCount++;
                }

                signCount++;
            }
        }
    }
}