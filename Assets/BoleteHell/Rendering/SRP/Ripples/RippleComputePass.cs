using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Rendering.SRP.Ripples
{
    public class RippleTextureData : ContextItem
    {
        public TextureHandle RippleTexture;
        public Vector4 WorldBounds;

        public override void Reset()
        {
            RippleTexture = TextureHandle.nullHandle;
            WorldBounds = Vector4.zero;
        }
    }

    public class RippleComputePass : ScriptableRenderPass
    {
        private const int MaxRipples = 32;

        private readonly ComputeShader _computeShader;
        private readonly int _kernelIndex;
        private readonly int _resolution;
        private readonly Vector4[] _rippleDataCache = new Vector4[MaxRipples];

        private static readonly int _rippleOutputId = Shader.PropertyToID("_RippleOutput");
        private static readonly int _ripplesId = Shader.PropertyToID("_Ripples");
        private static readonly int _timeId = Shader.PropertyToID("_Time");
        private static readonly int _rippleLifetimeId = Shader.PropertyToID("_RippleLifetime");
        private static readonly int _rippleRadiusId = Shader.PropertyToID("_RippleRadius");
        private static readonly int _rippleFrequencyId = Shader.PropertyToID("_RippleFrequency");
        private static readonly int _rippleSpeedId = Shader.PropertyToID("_RippleSpeed");
        private static readonly int _rippleStrengthId = Shader.PropertyToID("_RippleStrength");
        private static readonly int _corruptionId = Shader.PropertyToID("_Corruption");
        private static readonly int _rippleCountId = Shader.PropertyToID("_RippleCount");
        private static readonly int _worldBoundsId = Shader.PropertyToID("_WorldBounds");
        private static readonly int _resolutionId = Shader.PropertyToID("_Resolution");

        private static readonly int _globalRippleTexId = Shader.PropertyToID("_RippleTexture");
        private static readonly int _globalRippleDataId = Shader.PropertyToID("_RippleData");
        private static readonly int _globalRippleCountId = Shader.PropertyToID("_RippleCount");
        private static readonly int _globalRippleTextureBoundsId = Shader.PropertyToID("_RippleTextureBounds");
        private static readonly int _globalRippleLifetimeId = Shader.PropertyToID("_RippleLifetime");

        public float RippleRadius { get; set; } = 5f;
        public float RippleFrequency { get; set; } = 8f;
        public float RippleSpeed { get; set; } = 3f;
        public float RippleStrength { get; set; } = 0.15f;

        private class PassData
        {
            public ComputeShader ComputeShader;
            public int KernelIndex;
            public int Resolution;
            public TextureHandle OutputTexture;
            public BufferHandle RippleBuffer;
            public Vector4 WorldBounds;
            public int RippleCount;
            public float Time;
            public float RippleLifetime;
            public float RippleRadius;
            public float RippleFrequency;
            public float RippleSpeed;
            public float RippleStrength;
            public float Corruption;
            public Vector4[] RippleDataCache;
        }

        public RippleComputePass(ComputeShader computeShader, int resolution = 128)
        {
            _computeShader = computeShader;
            _kernelIndex = _computeShader.FindKernel("CSMain");
            _resolution = resolution;
            
            for (int i = 0; i < MaxRipples; i++)
                _rippleDataCache[i] = new Vector4(0, 0, -100, 0);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_computeShader == null)
                return;

            Vector4 worldBounds = Shader.GetGlobalVector(_globalRippleTextureBoundsId);
            
            if (worldBounds.z - worldBounds.x < 0.001f || worldBounds.w - worldBounds.y < 0.001f)
                return;

            Vector4[] globalRippleData = Shader.GetGlobalVectorArray(_globalRippleDataId);
            int rippleCount = Shader.GetGlobalInt(_globalRippleCountId);

            if (globalRippleData != null)
            {
                int count = Mathf.Min(rippleCount, MaxRipples);
                for (int i = 0; i < count; i++)
                    _rippleDataCache[i] = globalRippleData[i];
                for (int i = count; i < MaxRipples; i++)
                    _rippleDataCache[i] = new Vector4(0, 0, -100, 0);
            }

            var textureDesc = new TextureDesc(_resolution, _resolution)
            {
                colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
                enableRandomWrite = true,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "RippleTexture"
            };
            TextureHandle outputTexture = renderGraph.CreateTexture(textureDesc);

            var bufferDesc = new BufferDesc(MaxRipples, sizeof(float) * 4, GraphicsBuffer.Target.Structured)
            {
                name = "RippleBuffer"
            };
            BufferHandle rippleBuffer = renderGraph.CreateBuffer(bufferDesc);

            using (var builder = renderGraph.AddComputePass<PassData>("RippleCompute", out var passData))
            {
                passData.ComputeShader = _computeShader;
                passData.KernelIndex = _kernelIndex;
                passData.Resolution = _resolution;
                passData.OutputTexture = outputTexture;
                passData.RippleBuffer = rippleBuffer;
                passData.WorldBounds = worldBounds;
                passData.RippleCount = rippleCount;
                passData.Time = Time.time;
                passData.RippleLifetime = Shader.GetGlobalFloat(_globalRippleLifetimeId);
                passData.RippleRadius = RippleRadius;
                passData.RippleFrequency = RippleFrequency;
                passData.RippleSpeed = RippleSpeed;
                passData.RippleStrength = RippleStrength;
                passData.Corruption = Shader.GetGlobalFloat(_corruptionId);
                passData.RippleDataCache = _rippleDataCache;

                builder.UseTexture(outputTexture, AccessFlags.ReadWrite);
                builder.UseBuffer(rippleBuffer, AccessFlags.ReadWrite);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
                {
                    context.cmd.SetBufferData(data.RippleBuffer, data.RippleDataCache);

                    context.cmd.SetComputeTextureParam(data.ComputeShader, data.KernelIndex, _rippleOutputId, data.OutputTexture);
                    context.cmd.SetComputeBufferParam(data.ComputeShader, data.KernelIndex, _ripplesId, data.RippleBuffer);

                    context.cmd.SetComputeFloatParam(data.ComputeShader, _timeId, data.Time);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _rippleLifetimeId, data.RippleLifetime);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _rippleRadiusId, data.RippleRadius);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _rippleFrequencyId, data.RippleFrequency);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _rippleSpeedId, data.RippleSpeed);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _rippleStrengthId, data.RippleStrength);
                    context.cmd.SetComputeFloatParam(data.ComputeShader, _corruptionId, data.Corruption);
                    context.cmd.SetComputeIntParam(data.ComputeShader, _rippleCountId, data.RippleCount);
                    context.cmd.SetComputeVectorParam(data.ComputeShader, _worldBoundsId, data.WorldBounds);
                    context.cmd.SetComputeVectorParam(data.ComputeShader, _resolutionId, new Vector4(data.Resolution, data.Resolution, 0, 0));

                    int threadGroups = Mathf.CeilToInt(data.Resolution / 8f);
                    context.cmd.DispatchCompute(data.ComputeShader, data.KernelIndex, threadGroups, threadGroups, 1);

                    context.cmd.SetGlobalTexture(_globalRippleTexId, data.OutputTexture);
                });
            }

            var outputData = frameData.Create<RippleTextureData>();
            outputData.RippleTexture = outputTexture;
            outputData.WorldBounds = worldBounds;
        }
    }
}
