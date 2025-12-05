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

        [Header(Interactive Ripple)]
        _RippleRadius ("Ripple Radius", Float) = 5.0
        _RippleFrequency ("Ripple Frequency", Float) = 8.0
        _RippleSpeed ("Ripple Speed", Float) = 3.0
        _RippleStrength ("Ripple Strength", Float) = 0.15
        _RippleLifetime ("Ripple Lifetime", Float) = 1.5
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
            
            #define MAX_RIPPLES 8
            float4 _RippleData[MAX_RIPPLES];
            int _RippleCount;
            float _RippleLifetime;
            
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

            float     _RippleRadius;
            float     _RippleFrequency;
            float     _RippleSpeed;
            float     _RippleStrength;
            float     _BaseColorInfluence;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }

            void computeRippleOffsetAndNormal(float2 worldPos, out float2 totalOffset, out float3 rippleNormal)
            {
                totalOffset = float2(0, 0);
                rippleNormal = float3(0, 0, 1);

                float corruptionScale = saturate(_Corruption);
                if (corruptionScale < 0.01)
                    return;

                float2 normalAccum = float2(0, 0);

                UNITY_UNROLLX(MAX_RIPPLES)
                for (int i = 0; i < MAX_RIPPLES; i++)
                {
                    float4 ripple = _RippleData[i];
                    float2 ripplePos = ripple.xy;
                    float spawnTime = ripple.z;
                    float intensity = ripple.w;
                    
                    float age = _Time.y - spawnTime;
                    if (age < 0 || age > _RippleLifetime)
                        continue;
                    
                    float2 toRipple = worldPos - ripplePos;
                    float dist = length(toRipple);

                    if (dist < 0.001)
                        continue;
                    
                    float scaledRadius = _RippleRadius * corruptionScale;
                    float expandingRadius = scaledRadius * (age / _RippleLifetime);
                    float ringDist = abs(dist - expandingRadius);

                    float ringWidth = 2.0 * corruptionScale;
                    float falloff = exp(-ringDist * ringDist / (ringWidth * ringWidth));
                    
                    float ageNorm = age / _RippleLifetime;
                    float ageFade = 1.0 - ageNorm;
                    float ageIn = saturate(ageNorm * 10.0);
                    ageFade *= ageIn;
                    ageFade = ageFade * ageFade;
                    
                    float weight = falloff * ageFade * intensity;
                    
                    float2 dir = toRipple / dist;
                    float phase = dist * _RippleFrequency - age * _RippleSpeed * 6.0;
                    float wave = sin(phase) + 0.3 * sin(phase * 2.3 + 1.0);
                    
                    totalOffset += dir * wave * weight * _RippleStrength * corruptionScale;

                    float waveDerivative = cos(phase) + 0.3 * 2.3 * cos(phase * 2.3 + 1.0);
                    normalAccum += dir * waveDerivative * weight * corruptionScale;
                }

                const float intensity = 0.3f;
                rippleNormal = normalize(float3(-normalAccum.x * intensity, -normalAccum.y * intensity, 1.0));
            }

            float2 getSampleUV(float2 worldPos)
            {
                float2 rippleOffset;
                float3 rippleNormal;
                computeRippleOffsetAndNormal(worldPos, rippleOffset, rippleNormal);
                worldPos += rippleOffset;

                float2 uv = worldPos * _NoiseScale;

                float offsetX = sin(uv.x + _Time.y * _OffsetSpeed);
                float offsetY = sin(uv.y + _Time.y * _OffsetSpeed);
                uv += float2(offsetX, offsetY) * 0.1; // arbitrary scale

                float2 warp = tex2D(_NoiseTex, uv * _WarpScale + _Time.y * _WarpSpeed).rg * 2.0 - 1.0;
                uv += warp * _WarpStrength;

                return uv;
            }

            float sampleCorruptionMask(float2 p)
            {
                float n = tex2D(_NoiseTex, getSampleUV(p)).r;
                float remapped = smoothstep(0.0, 1.0, n); // help with non uniform noise

                float low = saturate(_Corruption - _BlendRange);
                float high = saturate(_Corruption + _BlendRange);

                if (_Corruption < 0.01)
                    return 0.0f;

                return 1 - smoothstep(low, high, remapped);
            }

            float3 sampleNormal(float2 p, float noise, float scale)
            {
                const float3 flat = float3(0, 0, 1);
                float3 sample = UnpackNormal(tex2D(_NoiseNormalsTex, getSampleUV(p) * scale));
                return normalize(lerp(flat, sample, noise));
            }

            float3 applyBaseColorInfluence(float3 corruptionResult, float3 baseColor, float mask)
            {
                float3 tinted = corruptionResult * baseColor * 2.0;
                float3 blended = lerp(corruptionResult, baseColor, 0.5);
                float3 combined = lerp(tinted, blended, 0.6);
                return lerp(corruptionResult, combined, _BaseColorInfluence * mask);
            }

            float3 computeAlbedo(float2 worldPos)
            {
                float2 offsetA = float2(0.2, 0.4);
                float2 offsetB = float2(0.3, 0.7);
                float2 offsetC = float2(0.9, 0.5);

                float nA = tex2D(_NoiseTex, getSampleUV(worldPos + offsetA * _ColorsOffset)).r;
                float nB = tex2D(_NoiseTex, getSampleUV(worldPos + offsetB * _ColorsOffset)).r;
                float nC = tex2D(_NoiseTex, getSampleUV(worldPos + offsetC * _ColorsOffset)).r;

                nA = smoothstep(0.2, 0.8, nA);
                nB = smoothstep(0.2, 0.8, nB);
                nC = smoothstep(0.2, 0.8, nC);

                // softmax
                const float sharp = 4.0;
                float eA = exp(nA * sharp);
                float eB = exp(nB * sharp);
                float eC = exp(nC * sharp);

                float sum = eA + eB + eC;
                float wA = eA / sum;
                float wB = eB / sum;
                float wC = eC / sum;

                float3 corruptionColor =
                    _CorruptionColor * wA
                  + _CorruptionColorB * wB
                  + _CorruptionColorC * wC;

                return corruptionColor;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                float2 worldPos = i.worldPos.xy;
                float mask = sampleCorruptionMask(worldPos);
                
                if (mask < 0.001)
                    return float4(_Color, 1.0);
                
                float2 rippleOffset;
                float3 rippleNormal;
                computeRippleOffsetAndNormal(worldPos, rippleOffset, rippleNormal);
                
                float3 albedo = computeAlbedo(worldPos);
                float3 normal = sampleNormal(worldPos, mask, 1.0f);
                float3 detailNormal = sampleNormal(worldPos, mask, _DetailsScale);
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