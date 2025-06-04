using Dreamteck.Splines;
using UnityEngine;

namespace Utils.SDF
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(SplineComputer))]
    [ExecuteAlways]
    public partial class SDFGenerator : MonoBehaviour
    {
        public int baseResolution = 64;
        public float padding = 4.0f;
        public int blurRadius = 2;
        public bool useSubsampling = true;
        
        [SerializeField]
        private Texture2D sdfTexture;
        
        private static readonly int SDFPropertyId = Shader.PropertyToID("_SDF");

        private MeshRenderer _meshRenderer;

        private void Awake()
        {
#if UNITY_EDITOR
            AutoRefreshOnSplineEdit();
#endif
            ApplyToMaterial();
        }

        private void ApplyToMaterial()
        {
            if (!sdfTexture)
                return;
            
            // TODO: Using MaterialPropertyBlock disables SRP batcher. We could merge all SDF textures
            //       into a giant atlas and modify the shared material to preserve compatibility?
            _meshRenderer = GetComponent<MeshRenderer>();
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(SDFPropertyId, sdfTexture);
            _meshRenderer.SetPropertyBlock(propertyBlock);    
        }
    }
}