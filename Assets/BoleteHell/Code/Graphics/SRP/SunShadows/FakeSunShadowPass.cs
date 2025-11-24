using BoleteHell.Code.Rendering.SDF;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Graphics.SRP
{
    public class FakeSunShadowPass : ScriptableRenderPass
    {
        private readonly Vector3 _sunDirection;
        private readonly float _stepSize;
        private readonly float _intensity;
        private readonly float _softness;
        private readonly Material _passMaterial;
        
        private static readonly int _sunDirectionId = Shader.PropertyToID("_SunDirection");
        private static readonly int _silhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        private static readonly int _stepSizeId = Shader.PropertyToID("_StepSize");
        private static readonly int _intensityId = Shader.PropertyToID("_ShadowIntensity");
        private static readonly int _softnessId = Shader.PropertyToID("_ShadowSoftness");
        
        private class FakeSunShadowPassData
        {
            public TextureHandle SilhouetteTex;
            public Material Material;
            public Vector3 SunDirection;
            public float StepSize;
            public float Intensity;
            public float Softness;
        }
        
        public FakeSunShadowPass(Material passMaterial, Vector3 sunDirection, float stepSize, float intensity, float softness)
        {
            _passMaterial = passMaterial;
            _sunDirection = sunDirection;
            _stepSize = stepSize;
            _intensity = intensity;
            _softness = softness;
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

            using var builder = renderGraph.AddRasterRenderPass<FakeSunShadowPassData>("Fake 2D sun shadow", out var passData);
            builder.UseTexture(silhouetteTex);
            builder.SetRenderAttachment(srcCamColor, 0);
            
            passData.SilhouetteTex = silhouetteTex;
            passData.Material = _passMaterial;
            passData.SunDirection = _sunDirection;
            passData.StepSize = _stepSize;
            passData.Intensity = _intensity;
            passData.Softness = _softness;
            
            builder.SetRenderFunc((FakeSunShadowPassData data, RasterGraphContext context) =>
            {
                data.Material.SetTexture(_silhouetteTexId, data.SilhouetteTex);
                data.Material.SetVector(_sunDirectionId, data.SunDirection);
                data.Material.SetFloat(_stepSizeId, data.StepSize);
                data.Material.SetFloat(_intensityId, data.Intensity);
                data.Material.SetFloat(_softnessId, data.Softness);
                Blitter.BlitTexture(context.cmd, srcCamColor, new Vector4(1, 1, 0, 0), data.Material, 0);
            });
        }
    }
}
