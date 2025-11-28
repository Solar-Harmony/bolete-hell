using BoleteHell.Rendering.SRP.Silhouette;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Rendering.SRP.FakeAO
{
    public class FakeAOPass : ScriptableRenderPass
    {
        private readonly float _referenceHeight;
        private readonly float _radius;
        private readonly Material _passMaterial;
        
        private static readonly int _referenceHeightId = Shader.PropertyToID("_ReferenceHeight");
        private static readonly int _radiusId = Shader.PropertyToID("_Radius");
        private static readonly int _silhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        
        private class FakeAOPassData
        {
            public TextureHandle SilhouetteTex;
            public Material Material;
            public float ReferenceHeight;
            public float Radius;
        }
        
        public FakeAOPass(Material passMaterial, float referenceHeight, float radius)
        {
            _passMaterial = passMaterial;
            _referenceHeight = referenceHeight;
            _radius = radius;
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

            using var builder = renderGraph.AddRasterRenderPass<FakeAOPassData>("Fake 2D ambient occlusion", out var passData);
            builder.UseTexture(silhouetteTex);
            builder.SetRenderAttachment(srcCamColor, 0);
            
            passData.SilhouetteTex = silhouetteTex;
            passData.Material = _passMaterial;
            passData.ReferenceHeight = _referenceHeight;
            passData.Radius = _radius;
            
            builder.SetRenderFunc((FakeAOPassData data, RasterGraphContext context) =>
            {
                data.Material.SetTexture(_silhouetteTexId, data.SilhouetteTex);
                data.Material.SetFloat(_referenceHeightId, data.ReferenceHeight);
                data.Material.SetFloat(_radiusId, data.Radius);
                Blitter.BlitTexture(context.cmd, srcCamColor, new Vector4(1, 1, 0, 0), data.Material, 0);
            });
        }
    }
}
