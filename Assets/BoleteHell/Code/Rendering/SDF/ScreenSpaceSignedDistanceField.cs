using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    [Serializable]
    public class SDFRenderingSettings
    {
        [ValidateInput(nameof(ValidateRenderLayer), "Invalid rendering layer name.")]
        public string renderingLayerMaskName;
        private bool ValidateRenderLayer(string name) => RenderingLayerMask.NameToRenderingLayer(name) != -1;
        
        [Tooltip("Reference resolution height (e.g., 1080) for resolution-independent SDF scaling.")]
        public float referenceHeight = 1080f;
    }
    
    public class ScreenSpaceSignedDistanceField : ScriptableRendererFeature
    {
        [SerializeField]
        private SDFRenderingSettings settings;
        
        private ObstaclesSilhouettePass _silhouettePass;

        private EdgeDetectionRenderPass _edgeDetectionPass;
        private Material _jfaMaterial;
        private Material _combineMaterial;

        public override void Create()
        {
            InitJFAMaterial();
            InitCombineMaterial();
            
            _silhouettePass = new ObstaclesSilhouettePass(settings.renderingLayerMaskName)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };

            _edgeDetectionPass = new EdgeDetectionRenderPass(_jfaMaterial, _combineMaterial, settings.referenceHeight)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }
        
        private void InitJFAMaterial()
        {
            var shader = Shader.Find("BoleteHell/SDFJumpFlood");
            _jfaMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        private void InitCombineMaterial()
        {
            var shader = Shader.Find("BoleteHell/SDFCombine");
            _combineMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_silhouettePass);
            renderer.EnqueuePass(_edgeDetectionPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_jfaMaterial);
            CoreUtils.Destroy(_combineMaterial);
        }
    }
}
