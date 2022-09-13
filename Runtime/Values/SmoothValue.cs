using UnityEngine;

namespace UnlockGames.BA.CameraControl.ExpositionMovement
{
    public class SmoothValue
    {
        public float value { get; private set; }

        public SmoothValue(float startValue) => value = startValue;

        public void SetValueImmediate(float value) => this.value = value;

        public void Update(float nextValue, float speed, float deltaTime) => value = Mathf.Lerp(value, nextValue, speed * deltaTime);
    }
}