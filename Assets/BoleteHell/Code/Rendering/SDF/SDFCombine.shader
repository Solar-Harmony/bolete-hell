Shader "CustomEffects/SDFCombine"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    TEXTURE2D(_SilhouetteTex);
    SAMPLER(sampler_SilhouetteTex);
    
    float4 CombineToSDF(Varyings input) : SV_Target
    {
        // Sample the blurred edge texture (from _BlitTexture)
        // This represents edge distance - higher values = closer to edge
        float edgeStrength = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).r;
        
        // Sample the original silhouette texture
        // This is binary: 1 = inside shape, 0 = outside shape
        float silhouette = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord).r;
        
        // Create proper SDF:
        // - Outside shapes (silhouette = 0): black (0)
        // - Inside shapes far from edge (silhouette = 1, low edgeStrength): white (1)
        // - At edges (silhouette = 1, high edgeStrength): mid-grey (0.5)
        
        float sdfValue;
        
        if (silhouette < 0.01)
        {
            // Outside the shape - always black
            sdfValue = 0.0;
        }
        else
        {
            // Inside the shape - interpolate from white (1.0) to mid-grey (0.5) based on edge proximity
            // When edgeStrength is high (at edge), output 0.5
            // When edgeStrength is low (interior), output 1.0
            sdfValue = lerp(1.0, 0.5, saturate(edgeStrength * 2.0));
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
            Name "SDFCombinePass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment CombineToSDF
            ENDHLSL
        }
    }
}
