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
            
            var blurTexDesc1 = edgeDetectTex;
            blurTexDesc1.name = "BlurTemp1";
            TextureHandle blurTemp1 = renderGraph.CreateTexture(blurTexDesc1);
            
            var blurTexDesc2 = edgeDetectTex;
            blurTexDesc2.name = "BlurTemp2";
            TextureHandle blurTemp2 = renderGraph.CreateTexture(blurTexDesc2);

            UpdateBlurSettings();
            
            if (!srcCamColor.IsValid() || !edgeTex.IsValid())
                return;
            
            // Pass 1: Sobel edge detection
            RenderGraphUtils.BlitMaterialParameters edgePassParams = new(silhouetteTex, edgeTex, _edgeMaterial, 0);
            renderGraph.AddBlitPass(edgePassParams, "sdf edge");
            
            // Multiple blur iterations for uniform smoothness
            // Iteration 1
            RenderGraphUtils.BlitMaterialParameters blurH1 = new(edgeTex, blurTemp1, _blurMaterial, 0);
            renderGraph.AddBlitPass(blurH1, "sdf blur h1");
            
            RenderGraphUtils.BlitMaterialParameters blurV1 = new(blurTemp1, blurTemp2, _blurMaterial, 1);
            renderGraph.AddBlitPass(blurV1, "sdf blur v1");
            
            // Iteration 2
            RenderGraphUtils.BlitMaterialParameters blurH2 = new(blurTemp2, blurTemp1, _blurMaterial, 0);
            renderGraph.AddBlitPass(blurH2, "sdf blur h2");
            
            RenderGraphUtils.BlitMaterialParameters blurV2 = new(blurTemp1, blurTemp2, _blurMaterial, 1);
            renderGraph.AddBlitPass(blurV2, "sdf blur v2");
            
            // Iteration 3 (final output to camera)
            RenderGraphUtils.BlitMaterialParameters blurH3 = new(blurTemp2, blurTemp1, _blurMaterial, 0);
            renderGraph.AddBlitPass(blurH3, "sdf blur h3");
            
            RenderGraphUtils.BlitMaterialParameters blurV3 = new(blurTemp1, srcCamColor, _blurMaterial, 1);
            renderGraph.AddBlitPass(blurV3, "sdf blur v3");
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