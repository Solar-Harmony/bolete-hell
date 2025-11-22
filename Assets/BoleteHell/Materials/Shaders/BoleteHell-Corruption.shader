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
        
        _CrawlingSpeed ("Crawling Speed", Float) = 0.1
        
        _EdgeWidth ("Rim Edge Width", Range(0,0.2)) = 0.05
        _RimLightDirection ("Rim Light Direction", Vector) = (1,0,0,0)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularPower ("Specular Power", Float) = 10

        _Color2("Color 2", Color) = (1,1,1,1)
        _Color3("Color 3", Color) = (1,1,1,1)
        _MucusScale ("Mucus Scale", Float) = 15.0
        _MucusStrength ("Mucus Strength", Float) = 0.2
        _MucusBumpStrength ("Mucus Bump Strength", Float) = 1.0
        _MucusColor ("Mucus Color", Color) = (0.5, 0.8, 1.0, 1)
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
            float _CrawlingSpeed;
            float _EdgeWidth;
            float4 _RimLightDirection;
            float4 _SpecularColor;
            float _SpecularPower;
            float4 _Color2;
            float4 _Color3;
            float _MucusScale;
            float _MucusStrength;
            float _MucusBumpStrength;
            float4 _MucusColor;


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
            
            float3 RGBtoHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
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
                float crawlPhase = _Time.y * _CrawlingSpeed + fbm(float3(_Time.y * 0.05, 0, 0)) * 2.0;
                float crawlOffsetX = sin(crawlPhase) * 2.0 * _CrawlingSpeed;
                float crawlOffsetY = cos(crawlPhase * 1.5) * 1.0 * _CrawlingSpeed;
                float crawlOffsetZ = sin(crawlPhase * 0.7) * 0.5 * _CrawlingSpeed;
                float3 worldPosCrawled = i.worldPos + float3(crawlOffsetX, crawlOffsetY, crawlOffsetZ);

                float drift = fbm(worldPosCrawled * 0.1 + _Time.y * 0.005) * 10.0 - 5.0;

                float n = fbm(worldPosCrawled * _Scale + float3(0, 0, _Time.y * _AnimationSpeed + drift));

                float eps = 0.01;
                float veins = fbm(worldPosCrawled * _Scale * 8.0 + float3(0, 0, _Time.y * _AnimationSpeed + drift));
                float nx = fbm(worldPosCrawled * _Scale + float3(0, 0, _Time.y * _AnimationSpeed + drift) + float3(eps, 0, 0));
                float ny = fbm(worldPosCrawled * _Scale + float3(0, 0, _Time.y * _AnimationSpeed + drift) + float3(0, eps, 0));
                float corruption = _Corruption;
                float corruptedMask = 1.0 - smoothstep(corruption - _SmoothMin, corruption + _SmoothMax, n);
                corruptedMask = saturate(corruptedMask);
                float2 grad = (float2(nx - n, ny - n)) / eps;
                float bumpStrength = (veins - 0.5) * 0.05;
                grad += bumpStrength * corruptedMask;

                float mucusScale = _MucusScale;
                float mucusNoise = fbm(worldPosCrawled * mucusScale + float3(0, 0, _Time.y * _AnimationSpeed + drift));
                float epsMucus = 0.1;
                float mx = fbm(worldPosCrawled * mucusScale + float3(epsMucus, 0, _Time.y * _AnimationSpeed + drift));
                float my = fbm(worldPosCrawled * mucusScale + float3(0, epsMucus, _Time.y * _AnimationSpeed + drift));
                float2 gradMucus = (float2(mx - mucusNoise, my - mucusNoise)) / epsMucus;
                float mucusVisibility = smoothstep(0.5, 1.0, _Corruption);
                float mucusMaskEarly = smoothstep(0.4, 0.6, mucusNoise) * corruptedMask * mucusVisibility;
                grad += gradMucus * mucusMaskEarly * _MucusBumpStrength;

                float mucusMask = mucusMaskEarly;

                float2 normal = normalize(float2(-grad.y, grad.x));

                float colorBlend = fbm(worldPosCrawled * _Scale * 1.5 + float3(0, 0, _Time.y * _AnimationSpeed + drift));
                float3 color1 = _Color.rgb;
                float3 color2 = _Color2.rgb;
                float3 color3 = _Color3.rgb;
                float3 selectedColor;
                if (colorBlend < 0.33) {
                    float t = colorBlend / 0.33;
                    selectedColor = lerp(color1, color2, t);
                } else if (colorBlend < 0.66) {
                    float t = (colorBlend - 0.33) / 0.33;
                    selectedColor = lerp(color2, color3, t);
                } else {
                    float t = (colorBlend - 0.66) / 0.34;
                    selectedColor = lerp(color3, color1, t);
                }
                float colorNoise = fbm(worldPosCrawled * _Scale * 3.0 + float3(0, 0, _Time.y * _AnimationSpeed + drift)) * 0.6 - 0.3;
                float3 variedCorruptionColor = selectedColor * (1.0 + colorNoise);
                variedCorruptionColor = saturate(variedCorruptionColor);

                float smallHueNoise = fbm(worldPosCrawled * _Scale * 2.0 + float3(0, 0, _Time.y * _AnimationSpeed + drift)) * 0.2 - 0.1;
                float3 hsv = RGBtoHSV(variedCorruptionColor);
                hsv.x += smallHueNoise;
                variedCorruptionColor = HSVtoRGB(hsv);

                float veinStrength = smoothstep(0.45, 0.55, veins);
                float3 tintedBase = _Color.rgb * lerp(1.0, variedCorruptionColor, corruptedMask) * lerp(1.0, 0.8, veinStrength * corruptedMask);

                float rim = 1.0 - smoothstep(0.0, _EdgeWidth, abs(n - corruption));

                float angle = atan2(normal.y, normal.x);
                float2 dir = normalize(_RimLightDirection.xy);
                float lightDirAngle = atan2(dir.y, dir.x);
                float diff = abs(angle - lightDirAngle);
                diff = min(diff, 2 * 3.14159 - diff);
                float light = 1.0 - saturate(diff / 3.14159);

                float factor = 0.5 + light * 1.0;
                float3 rimGlow = tintedBase * lerp(1.0, factor, rim);

                float flow = fbm(worldPosCrawled * _Scale * 4.0 + float3(0, 0, _Time.y * _AnimationSpeed + drift) + float3(1, 0, 0) * _Time.y * 0.1);
                float wetness = smoothstep(0.3, 0.7, flow);
                float3 wetGlow = rimGlow + wetness * 0.1 * _CorruptionColor.rgb * corruptedMask + mucusMask * _Corruption * _MucusStrength * _MucusColor.rgb;

                float3 specularColor = _SpecularColor.rgb * pow(saturate(dot(normal, dir)), _SpecularPower) * 0.25 * (1.0 + veins + mucusMask * 0.5) * corruptedMask;

                return half4(wetGlow + specularColor, _Color.a);
            }

            ENDHLSL
        }
    }
}