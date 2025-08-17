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
        private TextureDesc edgeDetectTex;
        private float _edgeSensitivity;
        
        private static readonly int EdgeSensitivityId = Shader.PropertyToID("_EdgeSensitivity");
        
        public EdgeDetectionRenderPass(Material edgeMaterial, float edgeSensitivity)
        {
            _edgeMaterial = edgeMaterial;
            _edgeSensitivity = edgeSensitivity;
        }
        
        private class PassData
        {
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string customPassNameH = "Edge detection pass to approximate SDF (horizontal)";
            const string customPassNameV = "Edge detection pass to approximate SDF (vertical)";

            TextureHandle silhouetteTex = frameData.Get<ObstaclesSilhouetteData>().SilhouetteTex;
            
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            
            if (resourceData.isActiveTargetBackBuffer)
                return;
            
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            edgeDetectTex = silhouetteTex.GetDescriptor(renderGraph);
            edgeDetectTex.name = "EdgeDetectionTexture";
            edgeDetectTex.depthBufferBits = 0;
            var dst = renderGraph.CreateTexture(edgeDetectTex);

            UpdateBlurSettings();
            
            if (!srcCamColor.IsValid() || !dst.IsValid())
                return;
            
            RenderGraphUtils.BlitMaterialParameters edgePassParams = new(silhouetteTex, srcCamColor, _edgeMaterial, 0);
            renderGraph.AddBlitPass(edgePassParams, "sdf edge");
        }
        
        private void UpdateBlurSettings()
        {
            if (!_edgeMaterial) return;
           
            _edgeMaterial.SetFloat(EdgeSensitivityId, _edgeSensitivity);
        }
    }
}