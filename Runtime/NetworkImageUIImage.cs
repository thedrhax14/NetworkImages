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

        public TextureFormat WebGLTextureFormat = TextureFormat.DXT5Crunched;
        [Tooltip("It is also default format")]
        public TextureFormat WindowsTextureFormat = TextureFormat.RGBA32;
        public TextureFormat iOSTextureFormat = TextureFormat.ASTC_6x6;
        public TextureFormat AndroidTextureFormat = TextureFormat.ETC2_RGBA1;

        private void Awake()
        {
            if (!target) target = GetComponent<UnityEngine.UI.Image>();
        }

        public override void SetTexture(Texture texture)
        {
            if (target.sprite != null) {
                Destroy(target.sprite.texture);
                Destroy(target.sprite);
            }
            Texture2D texture2D = new Texture2D(texture.width, texture.height, GetTextureFormat(), false);
            Graphics.CopyTexture(texture, texture2D);
            target.sprite = Sprite.Create(texture2D, new Rect(rect.x, rect.y, texture.width, texture.height), pivot, pixelsPerUnit);
        }

        TextureFormat GetTextureFormat()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return WindowsTextureFormat;
                case RuntimePlatform.WindowsPlayer:
                    return WindowsTextureFormat;
                case RuntimePlatform.IPhonePlayer:
                    return iOSTextureFormat;
                case RuntimePlatform.Android:
                    return AndroidTextureFormat;
                case RuntimePlatform.WebGLPlayer:
                    return WebGLTextureFormat;
            }
            return WindowsTextureFormat;
        }
    }
}