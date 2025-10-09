using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    public class SDFRenderPass : ScriptableRenderPass
    {
        private readonly Material _jfaMaterial;
        private readonly Material _combineMaterial;
        private readonly float _referenceHeight;
        
        private static readonly int JumpStepId = Shader.PropertyToID("_JumpStep");
        private static readonly int SilhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        private static readonly int ReferenceHeightId = Shader.PropertyToID("_ReferenceHeight");
        
        private class CombinePassData
        {
            public TextureHandle SilhouetteTex;
            public TextureHandle JfaResult;
            public Material Material;
            public float ReferenceHeight;
        }
        
        public SDFRenderPass(Material jfaMaterial, Material combineMaterial, float referenceHeight)
        {
            _jfaMaterial = jfaMaterial;
            _combineMaterial = combineMaterial;
            _referenceHeight = referenceHeight;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
                return;
            
            TextureHandle srcCamColor = resourceData.activeColorTexture;
            if (!srcCamColor.IsValid())
                return;
            
            TextureHandle silhouetteTex = frameData.Get<ObstaclesSilhouetteData>().SilhouetteTex;
            if (!silhouetteTex.IsValid())
                return;
            
            TextureHandle jfaTexture = AddJumpFloodingPass(renderGraph, silhouetteTex);
            AddCombinePass(renderGraph, silhouetteTex, jfaTexture, srcCamColor);
        }

        private TextureHandle AddJumpFloodingPass(RenderGraph renderGraph, TextureHandle silhouette)
        {
            TextureDesc bufferADesc = silhouette.GetDescriptor(renderGraph);
            bufferADesc.name = "Screen-space SDF jump flooding temp buffer A";
            bufferADesc.depthBufferBits = 0;
            bufferADesc.colorFormat = GraphicsFormat.R16G16B16A16_SFloat;
            TextureHandle bufferA = renderGraph.CreateTexture(bufferADesc);
            
            TextureDesc bufferBDesc = bufferADesc;
            bufferBDesc.name = "Screen-space SDF jump flooding temp buffer B";
            TextureHandle bufferB = renderGraph.CreateTexture(bufferBDesc);
            
            RenderGraphUtils.BlitMaterialParameters initParams = new(silhouette, bufferA, _jfaMaterial, 0);
            renderGraph.AddBlitPass(initParams, "Screen-space SDF jump flooding init pass");
            
            int maxDimension = Mathf.Max(bufferADesc.width, bufferADesc.height);
            int numIterations = Mathf.CeilToInt(Mathf.Log(maxDimension, 2));
            
            TextureHandle currentInput = bufferA;
            TextureHandle currentOutput = bufferB;

            for (int i = 0; i < numIterations; i++)
            {
                int jumpStep = 1 << (numIterations - 1 - i);
                _jfaMaterial.SetInt(JumpStepId, jumpStep);

                RenderGraphUtils.BlitMaterialParameters jfaParams = new(currentInput, currentOutput, _jfaMaterial, 1);
                renderGraph.AddBlitPass(jfaParams, $"Screen-space SDF jump flooding step {jumpStep}");

                (currentInput, currentOutput) = (currentOutput, currentInput);
            }

            return currentInput;
        }

        private void AddCombinePass(RenderGraph renderGraph, TextureHandle silhouetteTex, TextureHandle currentInput, TextureHandle srcCamColor)
        {
            using var builder = renderGraph.AddRasterRenderPass<CombinePassData>("Screen-space SDF final combine pass", out var passData);
            passData.SilhouetteTex = silhouetteTex;
            passData.JfaResult = currentInput;
            passData.Material = _combineMaterial;
            passData.ReferenceHeight = _referenceHeight;
                
            builder.UseTexture(silhouetteTex);
            builder.UseTexture(currentInput);
            builder.SetRenderAttachment(srcCamColor, 0);
            builder.SetRenderFunc((CombinePassData data, RasterGraphContext context) =>
            {
                data.Material.SetTexture(SilhouetteTexId, data.SilhouetteTex);
                data.Material.SetFloat(ReferenceHeightId, data.ReferenceHeight);
                Blitter.BlitTexture(context.cmd, data.JfaResult, new Vector4(1, 1, 0, 0), data.Material, 0);
            });
        }
    }
}