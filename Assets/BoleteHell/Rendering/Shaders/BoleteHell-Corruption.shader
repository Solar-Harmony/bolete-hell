Shader "Bolete Hell/World Corruption"
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
        
        _LightDir ("Light Direction", Vector) = (0.1, 0.1, 0.3, -1)
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

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            shared float _Corruption;
            
            sampler2D _RippleTexture;
            float4 _RippleTextureBounds;
            
            float3    _Color;
            float3    _CorruptionColor;
            float3    _CorruptionColorB;
            float3    _CorruptionColorC;
            
            sampler2D _NoiseTex;
            sampler2D _NoiseNormalsTex;
            float     _NoiseScale;
            float     _ColorsOffset;
            float     _DetailsScale;
            float     _DetailsIntensity;
            float     _BlendRange;
            
            float     _OffsetSpeed;
            float     _WarpScale;
            float     _WarpSpeed;
            float     _WarpStrength;
            
            float3    _LightDir;
            float3    _SpecularColor;
            float     _SpecularIntensity;
            float     _Shininess;

            float     _RimPower;
            float     _RimIntensity;
            float3    _RimColor;

            float     _BaseColorInfluence;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }

            void sampleRippleTexture(float2 worldPos, out float2 rippleOffset, out float3 rippleNormal)
            {
                rippleOffset = float2(0, 0);
                rippleNormal = float3(0, 0, 1);

                float corruptionScale = saturate(_Corruption);
                if (corruptionScale < 0.01)
                    return;

                float2 boundsMin = _RippleTextureBounds.xy;
                float2 boundsMax = _RippleTextureBounds.zw;
                float2 boundsSize = boundsMax - boundsMin;

                if (boundsSize.x < 0.001 || boundsSize.y < 0.001)
                    return;

                float2 uv = saturate((worldPos - boundsMin) / boundsSize);
                float4 rippleData = tex2Dlod(_RippleTexture, float4(uv, 0, 0));
                rippleOffset = rippleData.xy;
                float2 encodedNormal = rippleData.zw * 2.0 - 1.0;
                
                const float normalIntensity = 0.3f;
                rippleNormal = normalize(float3(-encodedNormal.x * normalIntensity, -encodedNormal.y * normalIntensity, 1.0));
            }

            float2 computeWarpOffset(float2 baseUV)
            {
                return tex2D(_NoiseTex, baseUV * _WarpScale + _Time.y * _WarpSpeed).rg * 2.0 - 1.0;
            }

            float2 getBaseUV(float2 worldPos, float2 rippleOffset)
            {
                worldPos += rippleOffset;
                float2 uv = worldPos * _NoiseScale;
                float offsetX = sin(uv.x + _Time.y * _OffsetSpeed);
                float offsetY = sin(uv.y + _Time.y * _OffsetSpeed);
                return uv + float2(offsetX, offsetY) * 0.1;
            }

            float2 applyWarp(float2 baseUV, float2 warpOffset)
            {
                return baseUV + warpOffset * _WarpStrength;
            }

            float2 getSampleUV(float2 worldPos, float2 rippleOffset)
            {
                float2 baseUV = getBaseUV(worldPos, rippleOffset);
                float2 warp = computeWarpOffset(baseUV);
                return applyWarp(baseUV, warp);
            }

            float sampleCorruptionMask(float2 sampleUV)
            {
                if (_Corruption < 0.01)
                    return 0.0f;

                float n = tex2D(_NoiseTex, sampleUV).r;
                float remapped = smoothstep(0.0, 1.0, n);

                float low = saturate(_Corruption - _BlendRange);
                float high = saturate(_Corruption + _BlendRange);

                return 1 - smoothstep(low, high, remapped);
            }

            float3 sampleNormal(float2 sampleUV, float noise, float scale)
            {
                const float3 flat = float3(0, 0, 1);
                float3 n = UnpackNormal(tex2D(_NoiseNormalsTex, sampleUV * scale));
                return normalize(lerp(flat, n, noise));
            }

            float3 applyBaseColorInfluence(float3 corruptionResult, float3 baseColor, float mask)
            {
                float3 tinted = corruptionResult * baseColor * 2.0;
                float3 blended = lerp(corruptionResult, baseColor, 0.5);
                float3 combined = lerp(tinted, blended, 0.6);
                return lerp(corruptionResult, combined, _BaseColorInfluence * mask);
            }

            float3 computeAlbedo(float2 baseUV, float2 warpOffset)
            {
                float2 offsetA = float2(0.2, 0.4) * _ColorsOffset * _NoiseScale;
                float2 offsetB = float2(0.3, 0.7) * _ColorsOffset * _NoiseScale;
                float2 offsetC = float2(0.9, 0.5) * _ColorsOffset * _NoiseScale;

                float nA = tex2D(_NoiseTex, applyWarp(baseUV + offsetA, warpOffset)).r;
                float nB = tex2D(_NoiseTex, applyWarp(baseUV + offsetB, warpOffset)).r;
                float nC = tex2D(_NoiseTex, applyWarp(baseUV + offsetC, warpOffset)).r;

                nA = smoothstep(0.2, 0.8, nA);
                nB = smoothstep(0.2, 0.8, nB);
                nC = smoothstep(0.2, 0.8, nC);

                const float sharp = 4.0;
                float eA = exp(nA * sharp);
                float eB = exp(nB * sharp);
                float eC = exp(nC * sharp);

                float sum = eA + eB + eC;
                float wA = eA / sum;
                float wB = eB / sum;
                float wC = eC / sum;

                return _CorruptionColor * wA + _CorruptionColorB * wB + _CorruptionColorC * wC;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                float2 worldPos = i.worldPos.xy;
                
                float2 rippleOffset;
                float3 rippleNormal;
                sampleRippleTexture(worldPos, rippleOffset, rippleNormal);
                
                float2 baseUV = getBaseUV(worldPos, rippleOffset);
                float2 warpOffset = computeWarpOffset(baseUV);
                float2 sampleUV = applyWarp(baseUV, warpOffset);
                
                float mask = sampleCorruptionMask(sampleUV);
                
                if (mask < 0.001)
                    return float4(_Color, 1.0);
                
                float3 albedo = computeAlbedo(baseUV, warpOffset);
                float3 normal = sampleNormal(sampleUV, mask, 1.0f);
                float3 detailNormal = sampleNormal(sampleUV, mask, _DetailsScale);
                normal = normalize(normal + detailNormal * _DetailsIntensity);
                
                float3 baseNormal = normal;
                float3 ripplePerturbation = rippleNormal - float3(0, 0, 1);
                normal = normalize(normal + ripplePerturbation * mask);
                
                float rippleIntensity = length(ripplePerturbation);

                // Lambert diffuse
                float3 lightDir = normalize(_LightDir);
                float3 diffuse = albedo * saturate(dot(normal, lightDir));

                // Blinn-Phong specular with boosted ripple highlights
                float3 viewDir = float3(0,0,1);
                float3 halfVector = normalize(lightDir + viewDir);
                float specPower = _Shininess * (1.0 + rippleIntensity * 2.0);
                float3 specular = _SpecularColor * _SpecularIntensity * pow(saturate(dot(normal, halfVector)), specPower);
                specular *= 1.0 + rippleIntensity * 1.5;

                // faux rim lighting (use base normal to avoid ripple artifacts)
                float cosTheta = saturate(dot(baseNormal, viewDir));
                float3 rim = _RimColor * pow(saturate(1.0 - cosTheta), _RimPower) * _RimIntensity;
                specular += rim;

                float3 corruptionResult = diffuse + specular;
                corruptionResult = applyBaseColorInfluence(corruptionResult, _Color, mask);
                
                return float4(lerp(_Color, corruptionResult, mask), 1.0);
            }

            ENDHLSL
        }
    }
}