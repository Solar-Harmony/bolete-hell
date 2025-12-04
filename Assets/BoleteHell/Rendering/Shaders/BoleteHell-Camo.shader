Shader "Bolete Hell/Simple Camo"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _FirstColor("First Color", Color) = (0.29, 0.33, 0.23, 1)
        _SecondColor("Second Color", Color) = (0.34, 0.29, 0.23, 1)
        _Scale("Scale", Float) = 1.0
        [NoScaleOffset] _NoiseTex ("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float3 _FirstColor;
                float3 _SecondColor;
                TEXTURE2D(_NoiseTex);
                SAMPLER(sampler_NoiseTex);
                float _Scale;
            CBUFFER_END
            
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS);
                o.uv = i.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv * _Scale).r;
                float mask = step(0.5f, noise);
                float3 color = lerp(_FirstColor, _SecondColor, mask);
                return float4(color, 1.0f);
            }
            ENDHLSL
        }
    }
}