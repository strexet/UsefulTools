using UnityEngine;

namespace UsefulTools.Runtime.Values
{
    public class SmoothFloat
    {
        public float value { get; private set; }

        public SmoothFloat(float startValue) => value = startValue;

        public void SetValueImmediate(float value) => this.value = value;

        public void Update(float nextValue, float speed, float deltaTime) => value = Mathf.Lerp(value, nextValue, speed * deltaTime);
    }
}