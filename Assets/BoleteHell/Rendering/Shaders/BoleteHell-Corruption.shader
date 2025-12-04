Shader "Bolete Hell/World Corruption"
{
    Properties
    {
        [Header(Base)]
        [MainColor] [PerRendererData] _Color("Color", Color) = (1,1,1,1)
        _BaseColorInfluence ("Base Color Influence", Range(0,1)) = 0.6
        
        [Header(Corruption Colors)]
        _CorruptionColor("Corruption Color", Color) = (0.18, 0.1, 0.27, 1)
        _CorruptionColorB("Corruption Color B", Color) = (0.08, 0.25, 0.12, 1)
        _CorruptionColorC("Corruption Color C", Color) = (0.35, 0.08, 0.18, 1)
        
        [Header(Noise)]
        [NoScaleOffset] _NoiseTex ("Noise Texture", 2D) = "white" {}
        [NoScaleOffset] _NoiseNormalsTex ("Noise Normal Map", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 5.0
        _ColorsOffset ("Color Noise Separation", Float) = 1.6
        _DetailsScale ("Detail Normal Scale", Float) = 0.3
        _DetailsIntensity ("Detail Normal Intensity", Float) = 1.0
        _BlendRange("Transition Softness", Range(0,0.5)) = 0.15
        
        [Header(Animation)]
        _OffsetSpeed ("Shift Animation Speed", Float) = 0.05
        _WarpScale ("Warp Scale", Float) = 1.0
        _WarpSpeed ("Warp Animation Speed", Float) = 0.05
        _WarpStrength ("Warp Strength", Float) = 0.1
        
        [Header(Lighting)]
        _LightDir ("Light Direction", Vector) = (0.1, 0.1, 0.3, -1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularIntensity ("Specular Intensity", Float) = 1.0
        _Shininess ("Specular Shininess", Float) = 16.0
        
        [Header(Rim)]
        _RimPower ("Rim Power", Range(0.1,8)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0,4)) = 1.0
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        
        [Header(Ripple)]
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
                float3 worldPos   : TEXCOORD0;
            };

            // Global
            shared float _Corruption;
            
            // Ripple data
            #define MAX_RIPPLES 8
            float4 _RippleData[MAX_RIPPLES];
            float  _RippleLifetime;
            float  _RippleRadius;
            float  _RippleFrequency;
            float  _RippleSpeed;
            float  _RippleStrength;
            
            // Base color
            float3 _Color;
            float  _BaseColorInfluence;
            
            // Corruption colors
            float3 _CorruptionColor;
            float3 _CorruptionColorB;
            float3 _CorruptionColorC;
            
            // Noise
            sampler2D _NoiseTex;
            sampler2D _NoiseNormalsTex;
            float     _NoiseScale;
            float     _ColorsOffset;
            float     _DetailsScale;
            float     _DetailsIntensity;
            float     _BlendRange;
            
            // Animation
            float _OffsetSpeed;
            float _WarpScale;
            float _WarpSpeed;
            float _WarpStrength;
            
            // Lighting
            float3 _LightDir;
            float3 _SpecularColor;
            float  _SpecularIntensity;
            float  _Shininess;

            // Rim
            float  _RimPower;
            float  _RimIntensity;
            float3 _RimColor;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS);
                return o;
            }

            float2 computeRippleOffset(float2 worldPos)
            {
                float2 totalOffset = 0;
                float corruptionScale = saturate(_Corruption);
                
                if (corruptionScale < 0.01)
                    return totalOffset;
                
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
                    float ringWidth = 1.5 * corruptionScale;
                    float ringMask = saturate(1.0 - ringDist / ringWidth);
                    
                    float ageFade = pow(1.0 - (age / _RippleLifetime), 0.7);
                    
                    float2 dir = toRipple / dist;
                    float wave = sin(dist * _RippleFrequency - age * _RippleSpeed * 6.0) * 0.3 + 0.7;
                    
                    totalOffset += dir * wave * ringMask * ageFade * intensity * _RippleStrength * corruptionScale;
                }
                
                return totalOffset;
            }

            float2 getSampleUV(float2 worldPos)
            {
                worldPos += computeRippleOffset(worldPos);
                float2 uv = worldPos * _NoiseScale;

                float2 offset = sin(uv + _Time.y * _OffsetSpeed) * 0.1;
                uv += offset;

                float2 warp = tex2D(_NoiseTex, uv * _WarpScale + _Time.y * _WarpSpeed).rg * 2.0 - 1.0;
                uv += warp * _WarpStrength;

                return uv;
            }

            float sampleCorruptionMask(float2 worldPos)
            {
                if (_Corruption < 0.01)
                    return 0;
                    
                float n = tex2D(_NoiseTex, getSampleUV(worldPos)).r;
                float remapped = smoothstep(0.0, 1.0, n);

                float low = saturate(_Corruption - _BlendRange);
                float high = saturate(_Corruption + _BlendRange);

                return 1 - smoothstep(low, high, remapped);
            }

            float3 sampleNormal(float2 worldPos, float intensity, float scale)
            {
                float3 flat = float3(0, 0, 1);
                float3 n = UnpackNormal(tex2D(_NoiseNormalsTex, getSampleUV(worldPos) * scale));
                return normalize(lerp(flat, n, intensity));
            }

            float3 applyBaseColorInfluence(float3 corruption, float3 baseColor, float mask)
            {
                float3 tinted = corruption * baseColor * 2.0;
                float3 blended = lerp(corruption, baseColor, 0.5);
                float3 combined = lerp(tinted, blended, 0.6);
                return lerp(corruption, combined, _BaseColorInfluence * mask);
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

                const float sharpness = 4.0;
                float eA = exp(nA * sharpness); 
                float eB = exp(nB * sharpness);
                float eC = exp(nC * sharpness);
                float sum = eA + eB + eC;

                return _CorruptionColor  * (eA / sum)
                     + _CorruptionColorB * (eB / sum)
                     + _CorruptionColorC * (eC / sum);
            }

            float3 computeLighting(float3 albedo, float3 normal)
            {
                float3 lightDir = normalize(_LightDir);
                float3 viewDir = float3(0, 0, 1);
                float3 halfVec = normalize(lightDir + viewDir);
                
                float3 diffuse = albedo * saturate(dot(normal, lightDir));
                float3 specular = _SpecularColor * _SpecularIntensity * pow(saturate(dot(normal, halfVec)), _Shininess);
                
                float cosTheta = saturate(dot(normal, viewDir));
                float3 rim = _RimColor * pow(1.0 - cosTheta, _RimPower) * _RimIntensity;
                
                return diffuse + specular + rim;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                float2 worldPos = i.worldPos.xy;
                float mask = sampleCorruptionMask(worldPos);
                
                float3 albedo = computeAlbedo(worldPos);
                
                float3 normal = sampleNormal(worldPos, mask, 1.0);
                float3 detailNormal = sampleNormal(worldPos, mask, _DetailsScale);
                normal = normalize(normal + detailNormal * _DetailsIntensity);

                float3 corruption = computeLighting(albedo, normal);
                corruption = applyBaseColorInfluence(corruption, _Color, mask);
                
                return float4(lerp(_Color, corruption, mask), 1.0);
            }

            ENDHLSL
        }
    }
}