    Shader "CustomEffects/EdgeSDF"
    {
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _EdgeSensitivity;

        float4 EdgeSDF(Varyings input) : SV_Target
        {
            float2 texelSize = _BlitTexture_TexelSize.xy;

            // Sobel kernel offsets
            float2 offsets[9] = {
                float2(-1,  1), float2(0,  1), float2(1,  1),
                float2(-1,  0), float2(0,  0), float2(1,  0),
                float2(-1, -1), float2(0, -1), float2(1, -1)
            };

            // Sobel kernels
            float sobelX[9] = {
                -1, 0, 1,
                -2, 0, 2,
                -1, 0, 1
            };

            float sobelY[9] = {
                 1,  2,  1,
                 0,  0,  0,
                -1, -2, -1
            };

            float3 sample[9];
            for (int i = 0; i < 9; i++)
            {
                float2 uv = input.texcoord + offsets[i] * texelSize;
                sample[i] = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).rgb;
            }

            // Convert to luminance
            float lum[9];
            for (int i = 0; i < 9; i++)
            {
                lum[i] = dot(sample[i], float3(0.299, 0.587, 0.114));
            }

            // Apply Sobel filter
            float gx = 0;
            float gy = 0;
            for (int i = 0; i < 9; i++)
            {
                gx += sobelX[i] * lum[i];
                gy += sobelY[i] * lum[i];
            }

            float edgeStrength = sqrt(gx * gx + gy * gy);
            edgeStrength *= _EdgeSensitivity;

            // Simulate SDF: invert edge strength
            float sdfValue = saturate(1.0 - edgeStrength);

            return float4(sdfValue, sdfValue, sdfValue, 1);
        }

        ENDHLSL

        SubShader
        {
            Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
            LOD 100
            ZWrite Off Cull Off

            Pass
            {
                Name "EdgeSDFPass"
                Blend SrcAlpha OneMinusSrcAlpha

                HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment EdgeSDF
                ENDHLSL
            }
        }
    }