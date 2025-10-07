using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    public class ObstaclesSilhouetteData : ContextItem {
        public TextureHandle SilhouetteTex;

        public override void Reset() {
            SilhouetteTex = TextureHandle.nullHandle;
        }
    }
    
    public class ObstaclesSilhouettePass : ScriptableRenderPass
    {
        private readonly RenderingLayerMask _renderingLayerMask;
        
        public ObstaclesSilhouettePass(string renderingLayerMaskName)
        {
            _renderingLayerMask = RenderingLayerMask.GetMask(renderingLayerMaskName);
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
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            
            List<ShaderTagId> shaderTagIds = new() {
                new ShaderTagId("Universal2D"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
            };
            var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, SortingCriteria.CommonOpaque);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all)
            {
                renderingLayerMask = _renderingLayerMask
            };
            var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
            passData.RendererListHandle = renderGraph.CreateRendererList(rendererListParams);
            builder.UseRendererList(passData.RendererListHandle);
            
            // Create silhouette texture with explicit settings to avoid MSAA/filtering issues
            var silhouetteDesc = cameraData.cameraTargetDescriptor;
            silhouetteDesc.msaaSamples = 1; // Disable MSAA - causes false edge detection
            silhouetteDesc.depthBufferBits = 0; // No depth needed
            silhouetteDesc.useMipMap = false;
            silhouetteDesc.autoGenerateMips = false;
            // Use Float16 format for maximum precision - critical for accurate edge detection
            // Float format has better precision than UNorm for intermediate calculations
            silhouetteDesc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
            
            Debug.Log($"Silhouette Pass: Camera={cameraData.camera.name}, Format={silhouetteDesc.graphicsFormat}, Size={silhouetteDesc.width}x{silhouetteDesc.height}");
            
            TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, silhouetteDesc, "SDF Silhouette", true, FilterMode.Point);
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