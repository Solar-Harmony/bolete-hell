Shader "CustomEffects/GaussianBlur"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float _BlurStrength;
    float _ReferenceHeight;

    // Higher quality 17-tap blur with better distribution to reduce banding
    static const float weights[17] = { 
        0.0540540541, 0.0539568346, 0.0536650891, 0.0531811118,
        0.0525086284, 0.0516527082, 0.0506194382, 0.0494158894,
        0.0480501080, 0.0465309447, 0.0448680391, 0.0430717157,
        0.0411529683, 0.0391233445, 0.0369948348, 0.0347798393,
        0.0324911660
    };

    float4 BlurHorizontal(Varyings input) : SV_Target
    {
        float2 texelSize = _BlitTexture_TexelSize.xy;
        
        // Reduced multiplier since we're doing multiple iterations
        float adjustedBlurStrength = _BlurStrength * 1.2;
        
        float3 result = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb * weights[0];
        
        // Use sub-pixel offsets to reduce banding
        for(int i = 1; i < 17; i++)
        {
            float offset = (float)i * adjustedBlurStrength;
            float2 offsetVec = float2(texelSize.x * offset, 0.0);
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + offsetVec).rgb * weights[i];
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - offsetVec).rgb * weights[i];
        }
        
        return float4(result, 1.0);
    }

    float4 BlurVertical(Varyings input) : SV_Target
    {
        float2 texelSize = _BlitTexture_TexelSize.xy;
        
        // Reduced multiplier since we're doing multiple iterations
        float adjustedBlurStrength = _BlurStrength * 1.2;
        
        float3 result = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb * weights[0];
        
        // Use sub-pixel offsets to reduce banding
        for(int i = 1; i < 17; i++)
        {
            float offset = (float)i * adjustedBlurStrength;
            float2 offsetVec = float2(0.0, texelSize.y * offset);
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + offsetVec).rgb * weights[i];
            result += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord - offsetVec).rgb * weights[i];
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
