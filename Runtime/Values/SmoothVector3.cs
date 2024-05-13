using UnityEngine;

namespace UsefulTools.Runtime.Values
{
    public class SmoothVector3
    {
        public Vector3 value { get; private set; }

        public SmoothVector3(Vector3 startValue) => value = startValue;

        public void SetValueImmediate(Vector3 value) => this.value = value;

        public void Update(Vector3 nextValue, float speed, float deltaTime) => value = Vector3.Lerp(value, nextValue, speed * deltaTime);
    }
}