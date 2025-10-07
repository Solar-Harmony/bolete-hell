Shader "CustomEffects/SDFCombine"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    TEXTURE2D(_SilhouetteTex);
    SAMPLER(sampler_SilhouetteTex);
    float _ReferenceHeight; // Reference resolution height for resolution-independent scaling
    
    float4 CombineToSDF(Varyings input) : SV_Target
    {
        // Sample the JFA distance field with POINT filtering to avoid interpolating seed positions
        // R,G = nearest seed position in pixels, B = distance to nearest edge in pixels
        float4 jfaData = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, input.texcoord);
        float distanceInPixels = jfaData.z;
        
        // Sample the original full-resolution silhouette texture
        float silhouette = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord).r;
        
        // Check if this pixel has valid distance data
        bool hasValidDistance = jfaData.x >= 0.0;
        
        // Normalize distance based on reference resolution for resolution independence
        // Scale the distance by current resolution / reference resolution
        float resolutionScale = _BlitTexture_TexelSize.w / _ReferenceHeight;
        float scaledDistance = distanceInPixels / resolutionScale;
        // Target gradient width of ~20 pixels at reference resolution
        float normalizedDist = saturate(scaledDistance / 20.0);
        
        float sdfValue;
        
        if (!hasValidDistance)
        {
            // No valid distance data - fallback to simple silhouette
            sdfValue = silhouette > 0.5 ? 1.0 : 0.0;
        }
        else if (silhouette > 0.5)
        {
            // Inside the shape: white (1.0) far from edge, grey (0.5) at edge
            sdfValue = lerp(0.5, 1.0, normalizedDist);
        }
        else
        {
            // Outside the shape: grey (0.5) at edge, black (0.0) far away
            sdfValue = lerp(0.5, 0.0, normalizedDist);
        }
        
        return float4(sdfValue, sdfValue, sdfValue, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "CombineToSDF"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment CombineToSDF
            ENDHLSL
        }
    }
}
