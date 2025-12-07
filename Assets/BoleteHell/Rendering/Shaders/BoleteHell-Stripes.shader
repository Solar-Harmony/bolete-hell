Shader "Bolete Hell/Simple Stripes"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (0,0,0,1)
        _StripeWidth("Stripe Width", Float) = 0.1
        _StripeAngle("Stripe Angle", Range(0,360)) = 0
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
                float2 uv        : TEXCOORD0;
                float4 extraData : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                half4  extraData  : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _Color2;
                float _StripeWidth;
                float _StripeAngle;
            CBUFFER_END
            
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS);
                o.uv = i.uv;
                o.extraData = i.extraData;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float angleRad = radians(_StripeAngle);
                float2 dir = float2(cos(angleRad), sin(angleRad));
                float stripeWidthUV = _StripeWidth / _ScreenParams.x;
                float stripeCoord = dot(uv, dir);
                float stripe = frac(stripeCoord / stripeWidthUV);
                float4 col = lerp(_Color, _Color2, step(0.5, stripe));

                return col;
            }
            ENDHLSL
        }
    }
}