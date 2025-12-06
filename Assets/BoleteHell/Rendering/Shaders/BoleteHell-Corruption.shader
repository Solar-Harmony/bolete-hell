﻿Shader "Bolete Hell/World Corruption"
{
    Properties
    {
        [MainColor] [PerRendererData] _Color("Color", Color) = (1,1,1,1)
        _CorruptionColor("Corruption Color", Color) = (0.18, 0.1, 0.27, 1)
        _CorruptionColorB("Corruption Color B", Color) = (0.08, 0.25, 0.12, 1)
        _CorruptionColorC("Corruption Color C", Color) = (0.35, 0.08, 0.18, 1)
        
        [NoScaleOffset] _NoiseTex ("Noise Texture", 2D) = "white" {}
        [NoScaleOffset] _NoiseNormalsTex ("Noise Normal Map", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 5.0
        _ColorsOffset ("Color Noise Separation", Float) = 1.6
        _DetailsScale ("Detail Normal Scale", Float) = 0.3
        _DetailsIntensity ("Detail Normal Intensity", Float) = 1.0
        _BlendRange("Transition Softness", Range(0,0.5)) = 0.15
        
        _OffsetSpeed ("Shift Animation Speed", Float) = 0.05
        _WarpScale ("Warp Scale", Float) = 1.0
        _WarpSpeed ("Warp Animation Speed", Float) = 0.05
        _WarpStrength ("Warp Strength", Float) = 0.1
        
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularIntensity ("Specular Intensity", Float) = 1.0
        _Shininess ("Specular Shininess", Float) = 16.0

        _RimPower ("Rim Power", Range(0.1,8)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0,4)) = 1.0
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        
        _BaseColorInfluence ("Base Color Influence", Range(0,1)) = 0.6
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "PreviewType"="Plane" }
        
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
            #include "Include/BoleteCorruption.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float3 _Color;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                CorruptionResult corruption = ComputeCorruption(i.worldPos.xy, _Color);
                return float4(corruption.color, 1.0);
            }

            ENDHLSL
        }
    }
}