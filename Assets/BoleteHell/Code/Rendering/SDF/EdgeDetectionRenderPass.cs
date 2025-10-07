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
        private float _referenceHeight;
        
        private static readonly int JumpStepId = Shader.PropertyToID("_JumpStep");
        private static readonly int SilhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        private static readonly int ReferenceHeightId = Shader.PropertyToID("_ReferenceHeight");
        
        public EdgeDetectionRenderPass(Material copyMaterial, Material jfaMaterial, Material combineMaterial, float edgeSensitivity, float referenceHeight)
        {
            _copyMaterial = copyMaterial;
            _jfaMaterial = jfaMaterial;
            _combineMaterial = combineMaterial;
            _edgeSensitivity = edgeSensitivity;
            _referenceHeight = referenceHeight;
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
            
            // Run JFA at full resolution to avoid banding artifacts from downsampling
            jfaTexDesc = silhouetteTex.GetDescriptor(renderGraph);
            jfaTexDesc.name = "JFATemp1";
            jfaTexDesc.depthBufferBits = 0;
            // Use Float16 format for maximum precision in storing pixel coordinates
            jfaTexDesc.colorFormat = GraphicsFormat.R16G16B16A16_SFloat;
            
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            Debug.Log($"JFA Pass: Camera={cameraData.camera.name}, Full-res JFA Size={jfaTexDesc.width}x{jfaTexDesc.height}, Format={jfaTexDesc.colorFormat}");
            
            TextureHandle jfaTemp1 = renderGraph.CreateTexture(jfaTexDesc);
            
            var jfaTexDesc2 = jfaTexDesc;
            jfaTexDesc2.name = "JFATemp2";
            TextureHandle jfaTemp2 = renderGraph.CreateTexture(jfaTexDesc2);
            
            if (!srcCamColor.IsValid())
                return;
            
            // Pass 1: Initialize JFA directly from full-resolution silhouette (no downsampling)
            RenderGraphUtils.BlitMaterialParameters initParams = new(silhouetteTex, jfaTemp1, _jfaMaterial, 0);
            renderGraph.AddBlitPass(initParams, "jfa init");
            
            // Pass 2: Jump Flood iterations
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
                passData.referenceHeight = _referenceHeight;
                
                builder.UseTexture(silhouetteTex, AccessFlags.Read);
                builder.UseTexture(currentInput, AccessFlags.Read);
                builder.SetRenderAttachment(srcCamColor, 0, AccessFlags.Write);
                
                builder.SetRenderFunc((CombinePassData data, RasterGraphContext context) =>
                {
                    data.material.SetTexture(SilhouetteTexId, data.silhouetteTex);
                    data.material.SetFloat(ReferenceHeightId, data.referenceHeight);
                    Blitter.BlitTexture(context.cmd, data.jfaResult, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
        }
        
        private class CombinePassData
        {
            public TextureHandle silhouetteTex;
            public TextureHandle jfaResult;
            public Material material;
            public float referenceHeight;
        }
    }
}