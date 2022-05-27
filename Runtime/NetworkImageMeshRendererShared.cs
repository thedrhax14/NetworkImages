using UnityEngine;

namespace com.outrealxr.networkimages.Runtime
{
    public class NetworkImageMeshRendererShared : NetworkImageMeshRenderer
    {
        public override void SetTexture(Texture texture)
        {
            Destroy(target.sharedMaterials[materialIndex].GetTexture(materialPropertyName));
            target.sharedMaterials[materialIndex].SetTexture(materialPropertyName, texture);
        }
    }
}