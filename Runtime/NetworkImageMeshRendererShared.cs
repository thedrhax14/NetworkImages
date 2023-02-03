using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageMeshRendererShared : NetworkImageMeshRenderer
    {
        public override void SetTexture(Texture texture)
        {
            if (target)
            {
                Destroy(target.sharedMaterials[materialIndex].GetTexture(materialPropertyName));
                target.sharedMaterials[materialIndex].SetTexture(materialPropertyName, texture);
                target.sharedMaterials[materialIndex].mainTextureScale = tiling;
                target.sharedMaterials[materialIndex].mainTextureOffset = offset;
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