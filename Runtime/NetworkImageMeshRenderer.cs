using UnityEngine;

namespace com.outrealxr.networkimages.Runtime
{
    public class NetworkImageMeshRenderer : NetworkImage
    {
        public MeshRenderer target;
        public string materialPropertyName = "_BaseMap";
        public int materialIndex = 0;

        public override void SetTexture(Texture texture)
        {
            Destroy(target.materials[materialIndex].GetTexture(materialPropertyName));
            target.materials[materialIndex].SetTexture(materialPropertyName, texture);
        }
    }
}