Shader "Bolete Hell/Internal/Fake Ambient Occlusion"
{
    Properties
    {
        _Radius("Radius", Float) = 10.0
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
            float _ReferenceHeight;
            float _Radius;
            
            float4 Frag(Varyings input) : SV_Target
            {
                float center = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord).r;

                // 8 evenly-spaced directions
                float2 dirs[8] = {
                    float2(1,0), float2(-1,0),
                    float2(0,1), float2(0,-1),
                    normalize(float2( 1,  1)),
                    normalize(float2(-1,  1)),
                    normalize(float2( 1, -1)),
                    normalize(float2(-1, -1))
                };

                const int NUM_DIRS  = 8;
                const int NUM_STEPS = 6;    
                const float DECAY   = 0.25; 

                float aoAccum = 0.0f;
                float weightAccum = 0.0f;

                float2 invRes = 1.0 / _ScreenParams.xy;

                for (int d = 0; d < NUM_DIRS; d++)
                {
                    float2 dir = dirs[d];

                    for (int s = 1; s <= NUM_STEPS; s++)
                    {
                        float t = (float)s / NUM_STEPS;
                        float w = exp(-t / DECAY);
                        float2 uv = input.texcoord + dir * (_Radius * t) * invRes;

                        float sampleVal = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, uv).r;

                        aoAccum += sampleVal * w;
                        weightAccum += w;
                    }
                }

                float ao = aoAccum / weightAccum;

                float finalAO = saturate(ao - center);

                return float4(0, 0, 0, finalAO);
            }

            ENDHLSL
        }
    }
}
