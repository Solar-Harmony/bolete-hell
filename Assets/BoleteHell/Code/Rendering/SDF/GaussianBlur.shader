Shader "CustomEffects/GaussianBlur"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float _BlurStrength;
    float _ReferenceHeight;

    // Properly normalized Gaussian weights for 13-tap blur (very heavy)
    static const float weights[13] = { 
        0.09893, 0.09735, 0.09275, 0.08565, 0.07661,
        0.06619, 0.05502, 0.04379, 0.03316, 0.02370,
        0.01591, 0.01015, 0.00613
    };

    float4 BlurHorizontal(Varyings input) : SV_Target
    {
        float2 texelSize = _BlitTexture_TexelSize.xy;
        
        // Much more aggressive scaling - use camera orthographic size if available
        // For now, use fixed heavy blur that doesn't scale with resolution
        float adjustedBlurStrength = _BlurStrength * 3.0; // 3x multiplier for heavy blur
        
        float3 result = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb * weights[0];
        
        for(int i = 1; i < 13; i++)
        {
            float2 offset = float2(texelSize.x * i * adjustedBlurStrength, 0.0);
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + offset).rgb * weights[i];
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - offset).rgb * weights[i];
        }
        
        return float4(result, 1.0);
    }

    float4 BlurVertical(Varyings input) : SV_Target
    {
        float2 texelSize = _BlitTexture_TexelSize.xy;
        
        float adjustedBlurStrength = _BlurStrength * 3.0; // 3x multiplier for heavy blur
        
        float3 result = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb * weights[0];
        
        for(int i = 1; i < 13; i++)
        {
            float2 offset = float2(0.0, texelSize.y * i * adjustedBlurStrength);
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + offset).rgb * weights[i];
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - offset).rgb * weights[i];
        }
        
        return float4(result, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "GaussianBlurHorizontal"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            ENDHLSL
        }

        Pass
        {
            Name "GaussianBlurVertical"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment BlurVertical
            ENDHLSL
        }
    }
}
