using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    public class EdgeDetectionRenderPass : ScriptableRenderPass
    {
        private readonly Material _copyMaterial;
        private readonly Material _jfaMaterial;
        private readonly Material _combineMaterial;
        private TextureDesc jfaTexDesc;
        private float _edgeSensitivity;
        
        private static readonly int JumpStepId = Shader.PropertyToID("_JumpStep");
        private static readonly int SilhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        
        public EdgeDetectionRenderPass(Material copyMaterial, Material jfaMaterial, Material combineMaterial, float edgeSensitivity)
        {
            _copyMaterial = copyMaterial;
            _jfaMaterial = jfaMaterial;
            _combineMaterial = combineMaterial;
            _edgeSensitivity = edgeSensitivity;
        }
        
        private class JFAPassData
        {
            public Material material;
            public int jumpStep;
            public TextureHandle input;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            TextureHandle silhouetteTex = frameData.Get<ObstaclesSilhouetteData>().SilhouetteTex;
            
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            
            if (resourceData.isActiveTargetBackBuffer)
                return;
            
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            
            // Create half-resolution textures for JFA
            jfaTexDesc = silhouetteTex.GetDescriptor(renderGraph);
            jfaTexDesc.name = "DownsampledSilhouette";
            jfaTexDesc.depthBufferBits = 0;
            // Downsample to half resolution for performance
            jfaTexDesc.width /= 2;
            jfaTexDesc.height /= 2;
            // Use Float16 format for maximum precision in storing pixel coordinates
            jfaTexDesc.colorFormat = GraphicsFormat.R16G16B16A16_SFloat;
            
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            Debug.Log($"JFA Pass: Camera={cameraData.camera.name}, JFA Size={jfaTexDesc.width}x{jfaTexDesc.height}, Format={jfaTexDesc.colorFormat}");
            
            TextureHandle downsampledSilhouette = renderGraph.CreateTexture(jfaTexDesc);
            
            var jfaTexDesc1 = jfaTexDesc;
            jfaTexDesc1.name = "JFATemp1";
            TextureHandle jfaTemp1 = renderGraph.CreateTexture(jfaTexDesc1);
            
            var jfaTexDesc2 = jfaTexDesc;
            jfaTexDesc2.name = "JFATemp2";
            TextureHandle jfaTemp2 = renderGraph.CreateTexture(jfaTexDesc2);
            
            if (!srcCamColor.IsValid())
                return;
            
            // Pass 1: Downsample silhouette to half resolution for JFA
            RenderGraphUtils.BlitMaterialParameters downsampleParams = new(silhouetteTex, downsampledSilhouette, _copyMaterial, 0);
            renderGraph.AddBlitPass(downsampleParams, "downsample silhouette");
            
            // Pass 2: Initialize JFA from downsampled silhouette (detect boundaries)
            RenderGraphUtils.BlitMaterialParameters initParams = new(downsampledSilhouette, jfaTemp1, _jfaMaterial, 0);
            renderGraph.AddBlitPass(initParams, "jfa init");
            
            // Pass 3: Jump Flood iterations - FIX: capture jump step in pass data
            int maxDimension = Mathf.Max(jfaTexDesc.width, jfaTexDesc.height);
            int numIterations = Mathf.CeilToInt(Mathf.Log(maxDimension, 2));
            
            TextureHandle currentInput = jfaTemp1;
            TextureHandle currentOutput = jfaTemp2;
            
            for (int i = 0; i < numIterations; i++)
            {
                int jumpStep = 1 << (numIterations - 1 - i);
                
                // Create custom pass to set jump step at EXECUTION time, not recording time
                using (var builder = renderGraph.AddRasterRenderPass<JFAPassData>($"jfa step {jumpStep}", out var passData))
                {
                    passData.material = _jfaMaterial;
                    passData.jumpStep = jumpStep;
                    passData.input = currentInput;
                    
                    builder.UseTexture(currentInput, AccessFlags.Read);
                    builder.SetRenderAttachment(currentOutput, 0, AccessFlags.Write);
                    
                    builder.SetRenderFunc((JFAPassData data, RasterGraphContext context) =>
                    {
                        data.material.SetInt(JumpStepId, data.jumpStep);
                        Blitter.BlitTexture(context.cmd, data.input, new Vector4(1, 1, 0, 0), data.material, 1);
                    });
                }
                
                (currentInput, currentOutput) = (currentOutput, currentInput);
            }
            
            // Add final pass with step 1 to fill any gaps
            using (var builder = renderGraph.AddRasterRenderPass<JFAPassData>("jfa step 1 final", out var passData))
            {
                passData.material = _jfaMaterial;
                passData.jumpStep = 1;
                passData.input = currentInput;
                
                builder.UseTexture(currentInput, AccessFlags.Read);
                builder.SetRenderAttachment(currentOutput, 0, AccessFlags.Write);
                
                builder.SetRenderFunc((JFAPassData data, RasterGraphContext context) =>
                {
                    data.material.SetInt(JumpStepId, data.jumpStep);
                    Blitter.BlitTexture(context.cmd, data.input, new Vector4(1, 1, 0, 0), data.material, 1);
                });
            }
            (currentInput, currentOutput) = (currentOutput, currentInput);
            
            // Final pass: Combine JFA result with silhouette to create proper SDF
            using (var builder = renderGraph.AddRasterRenderPass<CombinePassData>("sdf combine", out var passData))
            {
                passData.silhouetteTex = silhouetteTex;
                passData.jfaResult = currentInput;
                passData.material = _combineMaterial;
                
                builder.UseTexture(silhouetteTex, AccessFlags.Read);
                builder.UseTexture(currentInput, AccessFlags.Read);
                builder.SetRenderAttachment(srcCamColor, 0, AccessFlags.Write);
                
                builder.SetRenderFunc((CombinePassData data, RasterGraphContext context) =>
                {
                    data.material.SetTexture(SilhouetteTexId, data.silhouetteTex);
                    Blitter.BlitTexture(context.cmd, data.jfaResult, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
        }
        
        private class CombinePassData
        {
            public TextureHandle silhouetteTex;
            public TextureHandle jfaResult;
            public Material material;
        }
    }
}