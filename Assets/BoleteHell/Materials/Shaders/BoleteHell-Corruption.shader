Shader "Bolete Hell/World Corruption"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _CorruptionColor("Corruption Color", Color) = (0.18, 0.1, 0.27, 1)
        _CorruptionColor2("Corruption Color 2", Color) = (0.5, 0.2, 0.7, 1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 5.0
        _AnimationSpeed ("Animation Speed", Float) = 0.05
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType"="Plane" }

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
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float     _Corruption;
            
            float4    _Color;
            sampler2D _NoiseTex;
            float     _NoiseScale;
            float3    _CorruptionColor;
            float3    _CorruptionColor2;
            float     _AnimationSpeed;

            // sample noise texture with animated offset
            float noise(float3 p)
            {
                float offsetX = sin(p.x + _Time.y * _AnimationSpeed);
                float offsetY = sin(p.y + _Time.y * _AnimationSpeed) * 1.5f; // arbitrary pour briser symmetrie
                float2 offset = float2(offsetX, offsetY) * 0.1; // arbitrary intensity scale
                float scale = 1.0 + 0.05 * sin(_Time.y * 0.3);
                return tex2D(_NoiseTex, p.xy + scale + offset).r;
            }
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos   = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                float n = noise(i.worldPos * _NoiseScale);
                float corruptedMask = 1.0 - smoothstep(_Corruption - 0.25, _Corruption - 0.25, n);
                float3 color = lerp(_Color, _CorruptionColor, corruptedMask);

                float n2 = noise(i.worldPos * _NoiseScale * 2.0f);

                color += lerp(color, _CorruptionColor2, n2) * corruptedMask;
                
                return float4(color.rgb, 1.0f);
            }

            ENDHLSL
        }
    }
}