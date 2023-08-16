using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageUIImage : NetworkImage
    {
        [Header("UIImage Settings")]
        public UnityEngine.UI.Image target;
        public Vector2 rect = Vector2.zero;
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public float pixelsPerUnit = 100f;

        private void Awake()
        {
            if (!target) target = GetComponent<UnityEngine.UI.Image>();
        }

        public override void SetTexture(Texture texture)
        {
            if (target)
            {
                ClearTexture();
                target.sprite = Sprite.Create(texture as Texture2D, new Rect(rect.x, rect.y, texture.width, texture.height), pivot, pixelsPerUnit);
                base.SetTexture(texture);
            }
            else
            {
                Debug.LogError($"[NetworkImageUIImage] {gameObject.name} has no target assigned");
            }
        }

        public override void ClearTexture()
        {
            if (target.sprite != null)
            {
                Destroy(target.sprite.texture);
                Destroy(target.sprite);
            }
        }
    }
}