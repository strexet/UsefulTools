using UnityEngine;

namespace UsefulTools
{
    public class SpriteRendererProxy : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;

        public SpriteRendererProxy(SpriteRenderer spriteRenderer)
        {
            SpriteRenderer = spriteRenderer;
        }
        
        public new bool enabled
        {
            get => SpriteRenderer.enabled;
            set
            {
                Debug.LogError($"SET {gameObject.name} SpriteRenderer.enabled={value} - before={SpriteRenderer.enabled}", this);
                
                SpriteRenderer.enabled = value;
            }
        }

        public Sprite sprite
        {
            get => SpriteRenderer.sprite;
            set => SpriteRenderer.sprite = value;
        }

        public Vector2 size
        {
            get => SpriteRenderer == null ? Vector2.zero : SpriteRenderer.size;
            set => SpriteRenderer.size = value;
        }

        public int sortingOrder
        {
            get => SpriteRenderer.sortingOrder;
            set => SpriteRenderer.sortingOrder = value;
        }
    }
}