Shader "Bolete Hell/Basic Unlit"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
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
                float4 extraData  : COLOR;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                half4  extraData   : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END
            
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS);
                o.extraData = i.extraData;
                return o;
            }

            float4 frag(Varyings o) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}