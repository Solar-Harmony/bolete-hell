Shader "Bolete Hell/Fake Ambient Occlusion"
{
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
            
            float4 Frag(Varyings input) : SV_Target
            {
                float silhouette = SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord).r;

                float2 dirs[4] = {
                    float2(1, 0),
                    float2(-1, 0),
                    float2(0, 1),
                    float2(0, -1)
                };

                const float intensity = 1.0f;
                const float radius = 10.0f;
                
                float aoAccum = 0.0f;
                for (int k = 0; k < 4; ++k)
                {
                    float2 offset = dirs[k] * radius / _ScreenParams.xy;
                    aoAccum += SAMPLE_TEXTURE2D(_SilhouetteTex, sampler_SilhouetteTex, input.texcoord + offset).r;
                }

                float aoFac = aoAccum / 4.0 * intensity;
                
                return float4(0, 0, 0, aoFac - silhouette);
            }

            ENDHLSL
        }
    }
}
