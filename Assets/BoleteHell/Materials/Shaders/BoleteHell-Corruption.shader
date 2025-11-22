Shader "Bolete Hell/World Corruption"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _CorruptionColor("Corruption Color", Color) = (0.18, 0.1, 0.27, 1)
        _SmoothMin("Transition Min", Range(0,0.25)) = 0.1
        _SmoothMax("Transition Max", Range(0,0.25)) = 0.15

        _Scale ("Noise Scale", Float) = 5.0
        _Octaves ("Noise Octaves", Integer) = 4
        _Persistence ("Noise Persistence", Float) = 0.5
        
        _AnimationSpeed ("Animation Speed", Float) = 0.05
        
        _RimColor ("Rim Color", Color) = (0.6,0.2,0.8,1)
        _EdgeWidth ("Rim Edge Width", Range(0,0.2)) = 0.05
        _RimLightDirection ("Rim Light Direction", Vector) = (1,0,0,0)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularPower ("Specular Power", Float) = 10
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
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _CorruptionColor;
            float _SmoothMin;
            float _SmoothMax;
            float _Scale;
            int _Octaves;
            float _Persistence;
            float _AnimationSpeed;
            float4 _RimColor;
            float _EdgeWidth;
            float4 _RimLightDirection;
            float4 _SpecularColor;
            float _SpecularPower;


            float _Corruption;

            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);

                float n000 = hash(i + float3(0,0,0));
                float n100 = hash(i + float3(1,0,0));
                float n010 = hash(i + float3(0,1,0));
                float n110 = hash(i + float3(1,1,0));
                float n001 = hash(i + float3(0,0,1));
                float n101 = hash(i + float3(1,0,1));
                float n011 = hash(i + float3(0,1,1));
                float n111 = hash(i + float3(1,1,1));

                float3 u = f*f*(3.0-2.0*f);

                return lerp(
                    lerp(lerp(n000, n100, u.x), lerp(n010, n110, u.x), u.y),
                    lerp(lerp(n001, n101, u.x), lerp(n011, n111, u.x), u.y),
                    u.z);
            }

            // Fractal Brownian Motion (fBm)
            float fbm(float3 p)
            {
                float value = 0.0;
                float amplitude = 1.0;
                float frequency = 1.0;
                float totalAmplitude = 0.0;

                for (int i = 0; i < _Octaves; i++)
                {
                    value += amplitude * (noise(p * frequency) * 2.0 - 1.0); // signed [-1,1]
                    totalAmplitude += amplitude;
                    frequency *= 2.0;
                    amplitude *= _Persistence;
                }

                value = value / totalAmplitude; // normalize
                return saturate(0.5 * (value + 1.0));
            }
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv         = v.uv;
                o.worldPos   = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }
                        
            half4 frag(Varyings i) : SV_Target
            {
                float n = fbm(i.worldPos * _Scale + float3(0, 0, _Time.y * _AnimationSpeed));

                float eps = 0.01;
                float3 offset = float3(0, 0, _Time.y * _AnimationSpeed);
                float veins = fbm(i.worldPos * _Scale * 8.0 + offset);
                float nx = fbm(i.worldPos * _Scale + offset + float3(eps, 0, 0));
                float ny = fbm(i.worldPos * _Scale + offset + float3(0, eps, 0));
                float2 grad = (float2(nx - n, ny - n)) / eps;
                float bumpStrength = (veins - 0.5) * 0.05;
                grad += bumpStrength;
                float2 normal = normalize(float2(-grad.y, grad.x));

                float corruptedMask = 1.0 - smoothstep(_Corruption - _SmoothMin, _Corruption + _SmoothMax, n);
                corruptedMask = saturate(corruptedMask);

                float3 tintedBase = lerp(_Color.rgb, _Color.rgb * _CorruptionColor.rgb, corruptedMask);

                float veinStrength = smoothstep(0.45, 0.55, veins);
                tintedBase *= lerp(1.0, 0.8, veinStrength * corruptedMask);


                float rim = 1.0 - smoothstep(0.0, _EdgeWidth, abs(n - _Corruption));

                float angle = atan2(normal.y, normal.x);
                float2 dir = normalize(_RimLightDirection.xy);
                float lightDirAngle = atan2(dir.y, dir.x);
                float diff = abs(angle - lightDirAngle);
                diff = min(diff, 2 * 3.14159 - diff);
                float light = 1.0 - saturate(diff / 3.14159);

                float factor = 0.5 + light * 1.0;
                float3 rimTarget = tintedBase * factor;

                float3 rimGlow = lerp(tintedBase, rimTarget, rim);

                float flow = fbm(i.worldPos * _Scale * 4.0 + offset * 2.0 + float3(1, 0, 0) * _Time.y * 0.1);
                float wetness = smoothstep(0.3, 0.7, flow);
                float3 wetGlow = rimGlow + wetness * 0.1 * _CorruptionColor.rgb * corruptedMask;

                // Specular highlights
                float specular = pow(saturate(dot(normal, dir)), _SpecularPower) * (0.5 + 0.5 * veins);
                float3 specularColor = _SpecularColor.rgb * specular * 0.5;

                return half4(wetGlow + specularColor, _Color.a);
            }

            ENDHLSL
        }
    }
}