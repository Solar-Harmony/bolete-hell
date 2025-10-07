Shader "CustomEffects/SimpleCopy"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float4 SimpleCopy(Varyings input) : SV_Target
    {
        // Simple passthrough - just copy the input texture
        return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "SimpleCopy"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment SimpleCopy
            ENDHLSL
        }
    }
}

