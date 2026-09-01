Shader "Bolete Hell/Internal/Fake Sun Shadow"
{
    Properties
    {
        _StepSize("Step Size", Float) = 0.01
        _ShadowIntensity("Shadow Intensity", Float) = 0.5
        _ShadowSoftness("Shadow Softness", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_SilhouetteTex);
            SAMPLER(sampler_SilhouetteTex);
            float3 _SunDirection;
            float _StepSize;
            float _ShadowIntensity;
            float _ShadowSoftness;
            float2 _BufferOrigin;
            float2 _BufferInvSize;
            float2 _CamCenter;
            float2 _CamSize;
            int _MaxSteps;
            float _ShadowMaxLength;
            
            float3 dither(float2 uv, int i)
            {
                return frac(sin(dot(uv * 1000 + i * 10, float2(12.9898, 78.233))) * 43758.5453) - 0.5;
            }
            
            bool InMask(float2 uv) { return uv.x >= 0.0 && uv.x <= 1.0 && uv.y >= 0.0 && uv.y <= 1.0; }
            
            float4 Frag(Varyings input) : SV_Target
            {
                float2 p0 = _CamCenter + (input.texcoord - 0.5) * _CamSize;   // screen pixel -> world
                float2 uv0 = (p0 - _BufferOrigin) * _BufferInvSize;          // world -> mask UV
                float h0 = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, uv0).r; // receiver height01 (0 = ground)
                float marchLen = (1.0 - h0) * _ShadowMaxLength;             // ray only needs to rise to 1.0
                float2 duv = _SunDirection * marchLen * _BufferInvSize;
                float transmittance = 1.0;
                for (int i = 1; i <= _MaxSteps; ++i) {
                    float u  = (i + dither(input.texcoord, i)) / _MaxSteps;                    // existing hash dither in [-0.5, 0.5]
                    float2 suv = uv0 + duv * u;
                    float rayH01 = h0 + (1.0 - h0) * u;                    // ray height ramps receiver-height -> 1.0
                    float h01 = InMask(suv) ? SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, suv).r : 0;
                    float edge = saturate((h01 - rayH01) / _ShadowSoftness + 0.5);
                    const float stepDensity = 0.15f;  
                    transmittance *= exp(-edge * stepDensity);
                }
                float shadow = (1.0 - transmittance) * _ShadowIntensity;
                return float4(0, 0, 0, shadow);                             // NO self-exclusion line
            }

            ENDHLSL
        }
    }
}
