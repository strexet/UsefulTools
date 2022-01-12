using System.Collections.Generic;
using UnityEngine;

namespace UsefulTools
{
    [ExecuteAlways]
    public class ObjectsPlacer : MonoBehaviour
    {
        public GameObject _originalObj;

        [Space]
        public List<Transform> _transforms;
        public float _distanceBetween;
        public float _fullLength;
        public Vector3 _placeWorldDirection;
        public int _count;
        
        private void Update()
        {
            _placeWorldDirection.Normalize();

            if (_originalObj == null)
            {
                return;
            }

            _count = Mathf.FloorToInt(_fullLength / _distanceBetween);
            _transforms ??= new List<Transform>(_count);

            if (_transforms.Count != transform.childCount)
            {
                _transforms.Clear();

                for (var i = 0; i < transform.childCount; i++)
                    _transforms.Add(transform.GetChild(i));
            }

            if (_transforms.Count != _count)
            {
                foreach (var t in _transforms)
                    DestroyImmediate(t.gameObject);

                _transforms.Clear();

                for (var i = 0; i < _count; i++)
                {
                    var obj = Instantiate(_originalObj, transform);
                    obj.SetActive(true);
                    _transforms.Add(obj.transform);
                }
            }

            if (_count == 0)
            {
                return;
            }

            bool canDivideBy2 = _count % 2 == 0;

            var center = transform.position;

            var signCount = 0;
            var placeCount = 0;
            var startIndex = 0;

            Vector3 singleOffset;
            Vector3 startOffset;

            if (canDivideBy2)
            {
                singleOffset = _distanceBetween * _placeWorldDirection;
                startOffset = 0.5f * _distanceBetween * _placeWorldDirection;
            }
            else
            {
                _transforms[0].position = center;
                placeCount++;
                startIndex++;

                singleOffset = placeCount * _distanceBetween * _placeWorldDirection;
                startOffset = Vector3.zero;
            }

            for (int i = startIndex; i < _transforms.Count; i++)
            {
                var t = _transforms[i];
                
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