using System;
using UnityEditor;
using UnityEngine;

namespace BoleteHell.Utils
{
    /// <summary>
    /// Sets per-object material property.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialProperties : MonoBehaviour
    {
        public Color color;

        private MeshRenderer _meshRenderer;
        
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
        
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            UpdateMaterialProperties();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _meshRenderer ??= GetComponent<MeshRenderer>();
            UpdateMaterialProperties();
        }
#endif

        private void UpdateMaterialProperties()
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorId, color);
            _meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}