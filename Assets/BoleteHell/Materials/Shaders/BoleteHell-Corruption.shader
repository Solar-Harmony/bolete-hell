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

            shared float _Corruption;
            
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
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos   = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            float2 getSampleUV(float2 worldPos)
            {
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

                return _CorruptionColor * wA +
                       _CorruptionColorB * wB +
                       _CorruptionColorC * wC;
            }
                        
            float4 frag(Varyings i) : SV_Target
            {
                float2 worldPos = i.worldPos.xy;
                float mask = sampleCorruptionMask(worldPos); // 0 = base color, 1 = corruption material
                
                float3 albedo = computeAlbedo(worldPos);
                float3 normal = sampleNormal(worldPos, mask, 1.0f);
                float3 detailNormal = sampleNormal(worldPos, mask, _DetailsScale);
                normal = normalize(normal + detailNormal * _DetailsIntensity);

                // Lambert diffuse
                float3 lightDir = normalize(_LightDir);
                float3 diffuse = albedo * saturate(dot(normal, lightDir));

                // Blinn-Phong specular
                float3 viewDir = float3(0,0,1);
                float3 halfVector = normalize(lightDir + viewDir);
                float3 specular = _SpecularColor * _SpecularIntensity * pow(saturate(dot(normal, halfVector)), _Shininess);

                // faux rim lighting
                float cosTheta = saturate(dot(normal, viewDir));
                float3 rim = _RimColor * pow(saturate(1.0 - cosTheta), _RimPower) * _RimIntensity;
                specular += rim;

                return float4(lerp(_Color, diffuse + specular, mask), 1.0);
            }

            ENDHLSL
        }
    }
}