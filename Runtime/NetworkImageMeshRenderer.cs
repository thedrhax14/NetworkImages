using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageMeshRenderer : NetworkImage
    {
        [Header("MeshRenderer Settings")]
        public MeshRenderer target;
        public Vector2 tiling = Vector2.one;
        public Vector2 offset = Vector2.zero;
        public string materialPropertyName = "_BaseMap";
        public int materialIndex = 0;

        private void Awake() {
            if (!target) target = GetComponent<MeshRenderer>();
        }

        public override void SetTexture(Texture texture)
        {
            if (target)
            {
                Destroy(target.materials[materialIndex].GetTexture(materialPropertyName));
                target.materials[materialIndex].SetTexture(materialPropertyName, texture);
                target.materials[materialIndex].mainTextureScale = tiling;
                target.materials[materialIndex].mainTextureOffset = offset;
                base.SetTexture(texture);
            }
            else
            {
                Debug.LogError($"[NetworkImageMeshRenderer] {gameObject.name} has no target assigned");
            }
        }

        public virtual Material GetMaterial()
        {
            return target.materials[materialIndex];
        }
    }
}