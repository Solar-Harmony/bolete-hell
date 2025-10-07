Shader "CustomEffects/JumpFloodSDF"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    int _JumpStep;
    TEXTURE2D(_SilhouetteTex);
    SAMPLER(sampler_SilhouetteTex);

    // Initialize: Store seed positions in RG channels, distance in B channel
    float4 InitJFA(Varyings input) : SV_Target
    {
        float2 texelSize = _BlitTexture_TexelSize.xy;
        
        // Sample center and neighbors to detect silhouette boundaries
        float center = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord).r;
        
        // Check 4-connected neighbors to detect edge
        float left   = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord + float2(-texelSize.x, 0)).r;
        float right  = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord + float2(texelSize.x, 0)).r;
        float top    = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord + float2(0, texelSize.y)).r;
        float bottom = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord + float2(0, -texelSize.y)).r;
        
        // If any neighbor has different value, this is a boundary pixel
        bool isBoundary = (center != left) || (center != right) || (center != top) || (center != bottom);
        
        if (isBoundary)
        {
            // Store screen-space pixel coordinates in RG, distance 0 in B
            float2 pixelPos = input.texcoord * _BlitTexture_TexelSize.zw;
            return float4(pixelPos.xy, 0.0, 1.0);
        }
        else
        {
            // Not a boundary - initialize with invalid position
            return float4(-1.0, -1.0, 999999.0, 0.0);
        }
    }

    // Jump Flood Algorithm pass
    float4 JumpFloodPass(Varyings input) : SV_Target
    {
        float2 pixelPos = input.texcoord * _BlitTexture_TexelSize.zw;
        float2 bestSeed = float2(-1, -1);
        float bestDist = 999999.0;
        
        // Sample in a 3x3 grid with current jump step
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                float2 offset = float2(x, y) * _JumpStep;
                float2 samplePixel = pixelPos + offset;
                float2 sampleUV = samplePixel * _BlitTexture_TexelSize.xy;
                
                // Sample the current best seed at this location
                float4 sample = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, sampleUV);
                float2 seedPos = sample.xy;
                
                // If this sample has a valid seed
                if (seedPos.x >= 0.0)
                {
                    // Calculate distance in pixel space
                    float2 diff = pixelPos - seedPos;
                    float dist = length(diff);
                    
                    // Update if this is closer
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestSeed = seedPos;
                    }
                }
            }
        }
        
        return float4(bestSeed.xy, bestDist, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        // Pass 0: Initialize JFA
        Pass
        {
            Name "InitJFA"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment InitJFA
            ENDHLSL
        }

        // Pass 1: Jump Flood iteration
        Pass
        {
            Name "JumpFloodPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment JumpFloodPass
            ENDHLSL
        }
    }
}
