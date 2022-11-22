using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageMeshRendererShared : NetworkImageMeshRenderer
    {
        public override void SetTexture(Texture texture)
        {
            Destroy(target.sharedMaterials[materialIndex].GetTexture(materialPropertyName));
            target.sharedMaterials[materialIndex].SetTexture(materialPropertyName, texture);
            target.sharedMaterials[materialIndex].mainTextureScale = tiling;
            target.sharedMaterials[materialIndex].mainTextureOffset = offset;
        }

        public override Material GetMaterial()
        {
            return target.sharedMaterials[materialIndex];
        }
    }
}