using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    public class ObstaclesSilhouetteData : ContextItem 
    {
        public TextureHandle SilhouetteTex;

        public override void Reset() 
        {
            SilhouetteTex = TextureHandle.nullHandle;
        }
    }
    
    public class ObstaclesSilhouettePass : ScriptableRenderPass
    {
        private readonly RenderingLayerMask _renderingLayerMask;
        private readonly Material _overrideMaterial;
        
        public ObstaclesSilhouettePass(string renderingLayerMaskName, Material overrideMaterial)
        {
            _renderingLayerMask = RenderingLayerMask.GetMask(renderingLayerMaskName);
            _overrideMaterial = overrideMaterial;
        }
        
        private class PassData
        {
            public RendererListHandle RendererListHandle;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string customPassName = "Silhouette Pass for SDF Obstacles";

            using var builder = renderGraph.AddRasterRenderPass<PassData>(customPassName, out var passData);
  
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            
            List<ShaderTagId> shaderTagIds = new() {
                new ShaderTagId("Universal2D"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
            };
            var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, SortingCriteria.CommonOpaque);
            drawSettings.overrideMaterial = _overrideMaterial;
            var filteringSettings = new FilteringSettings(RenderQueueRange.all)
            {
                renderingLayerMask = _renderingLayerMask,
            };
            var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
            passData.RendererListHandle = renderGraph.CreateRendererList(rendererListParams);
            builder.UseRendererList(passData.RendererListHandle);
            
            RenderTextureDescriptor silhouetteDesc = cameraData.cameraTargetDescriptor;
            silhouetteDesc.msaaSamples = 1; 
            silhouetteDesc.depthBufferBits = 0;
            silhouetteDesc.useMipMap = false;
            silhouetteDesc.autoGenerateMips = false;
            silhouetteDesc.graphicsFormat = GraphicsFormat.R8_UNorm;
            
            TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, silhouetteDesc, "Obstacles Silhouette", true);
            builder.SetRenderAttachment(destination, 0);
            
            // pass to next pass
            var outputData = frameData.Create<ObstaclesSilhouetteData>();
            outputData.SilhouetteTex = destination;
            
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
        }
        
        private static void ExecutePass(PassData data, RasterGraphContext context)
        {
            context.cmd.ClearRenderTarget(RTClearFlags.Color, Color.clear, 1, 0);
            context.cmd.DrawRendererList(data.RendererListHandle);
        }
    }
}