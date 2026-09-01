using BoleteHell.Rendering.SRP.Silhouette;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Rendering.SRP.SunShadows
{
    public class FakeSunShadowPass : ScriptableRenderPass
    {
        private readonly SunShadowSettings _settings;
        private readonly Material _passMaterial;

        private static readonly int _sunDirectionId = Shader.PropertyToID("_SunDirection");
        private static readonly int _silhouetteTexId = Shader.PropertyToID("_SilhouetteTex");
        private static readonly int _maxStepsId = Shader.PropertyToID("_MaxSteps");
        private static readonly int _intensityId = Shader.PropertyToID("_ShadowIntensity");
        private static readonly int _softnessId = Shader.PropertyToID("_ShadowSoftness");
        private static readonly int _bufferOrigin = Shader.PropertyToID("_BufferOrigin");
        private static readonly int _bufferInvSize = Shader.PropertyToID("_BufferInvSize");
        private static readonly int _camCenter = Shader.PropertyToID("_CamCenter");
        private static readonly int _camSize = Shader.PropertyToID("_CamSize");
        private static readonly int _maxLengthId = Shader.PropertyToID("_ShadowMaxLength");

        private class FakeSunShadowPassData
        {
            public TextureHandle SilhouetteTex;
            public Material Material;
            public Vector3 SunDirection;
            public int MaxSteps;
            public float Intensity;
            public float Softness;
            public Vector2 CamCenter;
            public Vector2 CamSize;
            public Vector2 BufferOrigin;
            public Vector2 BufferInvSize;
            public float MaxLength;
        }

        public FakeSunShadowPass(Material passMaterial, SunShadowSettings settings)
        {
            _passMaterial = passMaterial;
            _settings = settings;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
                return;

            TextureHandle srcCamColor = resourceData.activeColorTexture;
            if (!srcCamColor.IsValid())
                return;

            var heightField = frameData.Get<ObstaclesSilhouetteData>();
            TextureHandle silhouetteTex = heightField.SilhouetteTex;
            if (!silhouetteTex.IsValid())
                return;
            
            Camera camera = frameData.Get<UniversalCameraData>().camera;

            using var builder = renderGraph.AddRasterRenderPass<FakeSunShadowPassData>("Fake 2D sun shadow", out var passData);
            builder.UseTexture(silhouetteTex);
            builder.SetRenderAttachment(srcCamColor, 0);

            passData.SilhouetteTex = silhouetteTex;
            passData.Material = _passMaterial;
            passData.SunDirection = _settings.SunDirection;
            passData.MaxSteps = _settings.MaxSteps;
            passData.Intensity = _settings.Intensity;
            passData.Softness = _settings.Softness;
            passData.BufferOrigin = heightField.BufferOrigin;
            passData.BufferInvSize = Vector2.one / heightField.BufferSize;
            passData.CamCenter = camera.transform.position;
            passData.MaxLength = _settings.MaxLength;
            float camWidth = camera.orthographicSize * camera.aspect * 2;
            float camHeight = camera.orthographicSize * 2;
            passData.CamSize = new Vector2(camWidth, camHeight);

            builder.SetRenderFunc((FakeSunShadowPassData data, RasterGraphContext context) =>
            {
                data.Material.SetTexture(_silhouetteTexId, data.SilhouetteTex);
                data.Material.SetVector(_sunDirectionId, data.SunDirection);
                data.Material.SetVector(_bufferOrigin, data.BufferOrigin);
                data.Material.SetVector(_bufferInvSize, data.BufferInvSize);
                data.Material.SetVector(_camCenter, data.CamCenter);
                data.Material.SetVector(_camSize, data.CamSize);
                data.Material.SetInteger(_maxStepsId, data.MaxSteps);
                data.Material.SetFloat(_intensityId, data.Intensity);
                data.Material.SetFloat(_softnessId, data.Softness);
                data.Material.SetFloat(_maxLengthId, data.MaxLength);
                Blitter.BlitTexture(context.cmd, srcCamColor, new Vector4(1, 1, 0, 0), data.Material, 0);
            });
        }
    }
}
