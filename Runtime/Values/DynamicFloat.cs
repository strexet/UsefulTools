using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UsefulTools.Runtime.Values
{
    public class DynamicFloat
    {
        public float Value { get; private set; }
        public bool IsChanging { get; private set; }

        private float _nextValue;

        public DynamicFloat(float startValue) => ChangeTo(startValue);

        public void ChangeTo(float value)
        {
            IsChanging = false;
            Value = value;
            _nextValue = value;
        }

        public void ChangeTo(float value, float duration) => SmoothChange(value, duration).Forget();

        public void StopSmoothChange()
        {
            IsChanging = false;
            _nextValue = Value;
        }

        public void FinishSmoothChange()
        {
            IsChanging = false;
            Value = _nextValue;
        }

        private async UniTaskVoid SmoothChange(float nextValue, float duration)
        {
            IsChanging = true;
            _nextValue = nextValue;

            var timer = 0.0f;
            var startValue = Value;
            var durationInversed = 1 / duration;

            while (IsChanging && timer <= duration)
            {
                timer += Time.deltaTime;

                Value = Mathf.Lerp(startValue, _nextValue, timer * durationInversed);

                await UniTask.NextFrame();
            }

            IsChanging = false;
        }
    }
}