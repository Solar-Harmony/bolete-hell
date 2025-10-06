using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    public class EdgeDetectionRenderPass : ScriptableRenderPass
    {
        private readonly Material _edgeMaterial;
        private readonly Material _blurMaterial;
        private TextureDesc edgeDetectTex;
        private float _edgeSensitivity;
        private float _blurStrength;
        
        private static readonly int EdgeSensitivityId = Shader.PropertyToID("_EdgeSensitivity");
        private static readonly int BlurStrengthId = Shader.PropertyToID("_BlurStrength");
        private static readonly int ReferenceHeightId = Shader.PropertyToID("_ReferenceHeight");
        
        public EdgeDetectionRenderPass(Material edgeMaterial, Material blurMaterial, float edgeSensitivity, float blurStrength)
        {
            _edgeMaterial = edgeMaterial;
            _blurMaterial = blurMaterial;
            _edgeSensitivity = edgeSensitivity;
            _blurStrength = blurStrength;
        }
        
        private class PassData
        {
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            TextureHandle silhouetteTex = frameData.Get<ObstaclesSilhouetteData>().SilhouetteTex;
            
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            
            if (resourceData.isActiveTargetBackBuffer)
                return;
            
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            
            // Create intermediate textures for edge detection and blur passes
            edgeDetectTex = silhouetteTex.GetDescriptor(renderGraph);
            edgeDetectTex.name = "EdgeDetectionTexture";
            edgeDetectTex.depthBufferBits = 0;
            TextureHandle edgeTex = renderGraph.CreateTexture(edgeDetectTex);
            
            var blurTexDesc = edgeDetectTex;
            blurTexDesc.name = "BlurTempTexture";
            TextureHandle blurTemp = renderGraph.CreateTexture(blurTexDesc);

            UpdateBlurSettings();
            
            if (!srcCamColor.IsValid() || !edgeTex.IsValid())
                return;
            
            // Pass 1: Sobel edge detection
            RenderGraphUtils.BlitMaterialParameters edgePassParams = new(silhouetteTex, edgeTex, _edgeMaterial, 0);
            renderGraph.AddBlitPass(edgePassParams, "sdf edge");
            
            // Pass 2: Horizontal blur
            RenderGraphUtils.BlitMaterialParameters blurHorizontalParams = new(edgeTex, blurTemp, _blurMaterial, 0);
            renderGraph.AddBlitPass(blurHorizontalParams, "sdf blur horizontal");
            
            // Pass 3: Vertical blur (output to camera color)
            RenderGraphUtils.BlitMaterialParameters blurVerticalParams = new(blurTemp, srcCamColor, _blurMaterial, 1);
            renderGraph.AddBlitPass(blurVerticalParams, "sdf blur vertical");
        }
        
        private void UpdateBlurSettings()
        {
            if (_edgeMaterial)
            {
                _edgeMaterial.SetFloat(EdgeSensitivityId, _edgeSensitivity);
            }
           
            if (_blurMaterial)
            {
                _blurMaterial.SetFloat(BlurStrengthId, _blurStrength);
                // Use 1080p as reference resolution for consistent blur appearance
                _blurMaterial.SetFloat(ReferenceHeightId, 1080f);
            }
        }
    }
}