using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageMeshRendererShared : NetworkImageMeshRenderer
    {
        public override void SetTexture(Texture texture)
        {
            if (target)
            {
                if (materialIndex < target.materials.Length)
                {
                    if (loaded) Destroy(target.sharedMaterials[materialIndex].GetTexture(materialPropertyName));
                    target.sharedMaterials[materialIndex].SetTexture(materialPropertyName, texture);
                    target.sharedMaterials[materialIndex].mainTextureScale = tiling;
                    target.sharedMaterials[materialIndex].mainTextureOffset = offset;
                    loaded = true;
                }
                else
                {
                    Debug.LogError($"[NetworkImageMeshRendererShared] {gameObject.name} less materials then requested index number. It has {target.materials.Length} materials, but tried {materialIndex}");
                }
                base.SetTexture(texture);
            }
            else
            {
                Debug.LogError($"[NetworkImageMeshRendererShared] {gameObject.name} has no target assigned");
            }
        }

        public override Material GetMaterial()
        {
            return target.sharedMaterials[materialIndex];
        }
    }
}