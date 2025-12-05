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
            
            float4 Frag(Varyings input) : SV_Target
            {
                float2 sunDirClip = mul((float3x3)UNITY_MATRIX_VP, float4(_SunDirection, 0)).xy;
                float2 sunDirUV = sunDirClip * 0.5f;

                const int steps = 16;

                float transmittance = 1.0f;
                for (int i = 1; i <= steps; ++i)
                {
                    float dither = frac(sin(dot(input.texcoord * 1000 + i * 10, float2(12.9898, 78.233))) * 43758.5453) - 0.5;
                    float stepPos = i + dither;
                    float2 offset = sunDirUV * stepPos * _StepSize;
                    float2 sampleUV = input.texcoord + offset;
                    float silhouette = 0;
                    if (sampleUV.x >= 0 && sampleUV.x <= 1 && sampleUV.y >= 0 && sampleUV.y <= 1)
                        silhouette = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, sampleUV).r;
                    float density = silhouette * _ShadowSoftness * _StepSize;
                    transmittance *= exp(-density);
                }

                float shadow = (1 - transmittance) * _ShadowIntensity;

                float currentSil = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord).r;
                if (currentSil > 0.5f)
                    shadow = 0;

                return float4(0, 0, 0, shadow);
            }

            ENDHLSL
        }
    }
}
