using UnityEngine;

namespace BoleteHell.Code.Utils
{
    /// <summary>
    /// Sets per-object material property.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialProperties : MonoBehaviour
    {
        public Color color;

        private MaterialPropertyBlock _propertyBlock;
        private MeshRenderer _meshRenderer;
        
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
        
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void Start()
        {
            UpdateMaterialProperties();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _meshRenderer ??= GetComponent<MeshRenderer>();
            _propertyBlock ??= new MaterialPropertyBlock();
    
            UpdateMaterialProperties();
        }
#endif

        private void UpdateMaterialProperties()
        {
            if (_propertyBlock == null)
                return;
            
            _propertyBlock.SetColor(ColorId, color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}