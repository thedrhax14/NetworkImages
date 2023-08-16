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
                if (materialIndex < target.materials.Length)
                {
                    ClearTexture();
                    target.materials[materialIndex].SetTexture(materialPropertyName, texture);
                    target.materials[materialIndex].mainTextureScale = tiling;
                    target.materials[materialIndex].mainTextureOffset = offset;
                }
                else
                {
                    Debug.LogError($"[NetworkImageMeshRenderer] {gameObject.name} less materials then requested index number. It has {target.materials.Length} materials, but tried {materialIndex}");
                }
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

        public override void ClearTexture()
        {
            if (materialIndex < target.materials.Length)
            {
                Destroy(target.materials[materialIndex].GetTexture(materialPropertyName));
            }
            else
            {
                Debug.LogError($"[NetworkImageMeshRenderer] {gameObject.name} less materials then requested index number. It has {target.materials.Length} materials, but tried {materialIndex}");
            }
        }
    }
}