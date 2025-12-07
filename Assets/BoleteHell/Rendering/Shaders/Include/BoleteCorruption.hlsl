#ifndef BOLETE_CORRUPTION_INCLUDED
#define BOLETE_CORRUPTION_INCLUDED

#include "BoleteWorldParams.hlsl"
#include "BoleteRipples.hlsl"

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

float3    _SpecularColor;
float     _SpecularIntensity;
float     _Shininess;

float     _RimPower;
float     _RimIntensity;
float3    _RimColor;

float     _BaseColorInfluence;

struct CorruptionSample
{
    float2 baseUV;
    float2 warpOffset;
    float2 sampleUV;
};

float2 Corruption_ComputeWarpOffset(float2 baseUV)
{
    return tex2D(_NoiseTex, baseUV * _WarpScale + _Time.y * _WarpSpeed).rg * 2.0 - 1.0;
}

float2 Corruption_GetBaseUV(float2 worldPos, float2 rippleOffset)
{
    worldPos += rippleOffset;
    float2 uv = worldPos * _NoiseScale;
    float offsetX = sin(uv.x + _Time.y * _OffsetSpeed);
    float offsetY = sin(uv.y + _Time.y * _OffsetSpeed);
    return uv + float2(offsetX, offsetY) * 0.1;
}

float2 Corruption_ApplyWarp(float2 baseUV, float2 warpOffset)
{
    return baseUV + warpOffset * _WarpStrength;
}

CorruptionSample Corruption_GetSample(float2 worldPos, float2 rippleOffset)
{
    CorruptionSample result;
    result.baseUV = Corruption_GetBaseUV(worldPos, rippleOffset);
    result.warpOffset = Corruption_ComputeWarpOffset(result.baseUV);
    result.sampleUV = Corruption_ApplyWarp(result.baseUV, result.warpOffset);
    return result;
}

float Corruption_SampleMask(float2 sampleUV)
{
    float corruption = GetCorruption();
    if (corruption < 0.01)
        return 0.0f;

    float n = tex2D(_NoiseTex, sampleUV).r;
    float remapped = smoothstep(0.0, 1.0, n);

    float low = saturate(corruption - _BlendRange);
    float high = saturate(corruption + _BlendRange);

    return 1 - smoothstep(low, high, remapped);
}

float3 Corruption_SampleNormal(float2 sampleUV, float mask, float scale)
{
    const float3 flat = float3(0, 0, 1);
    float3 n = UnpackNormal(tex2D(_NoiseNormalsTex, sampleUV * scale));
    return normalize(lerp(flat, n, mask));
}

float3 Corruption_ComputeAlbedo(float2 baseUV, float2 warpOffset)
{
    float2 offsetA = float2(0.2, 0.4) * _ColorsOffset * _NoiseScale;
    float2 offsetB = float2(0.3, 0.7) * _ColorsOffset * _NoiseScale;
    float2 offsetC = float2(0.9, 0.5) * _ColorsOffset * _NoiseScale;

    float nA = tex2D(_NoiseTex, Corruption_ApplyWarp(baseUV + offsetA, warpOffset)).r;
    float nB = tex2D(_NoiseTex, Corruption_ApplyWarp(baseUV + offsetB, warpOffset)).r;
    float nC = tex2D(_NoiseTex, Corruption_ApplyWarp(baseUV + offsetC, warpOffset)).r;

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

float3 Corruption_ApplyLighting(float3 albedo, float3 normal, float3 baseNormal, float rippleIntensity)
{
    float3 lightDir = GetWorldLightDir();
    float3 diffuse = albedo * saturate(dot(normal, lightDir));

    float3 viewDir = float3(0, 0, 1);
    float3 halfVector = normalize(lightDir + viewDir);
    float specPower = _Shininess * (1.0 + rippleIntensity * 2.0);
    float3 specular = _SpecularColor * _SpecularIntensity * pow(saturate(dot(normal, halfVector)), specPower);
    specular *= 1.0 + rippleIntensity * 1.5;

    float cosTheta = saturate(dot(baseNormal, viewDir));
    float3 rim = _RimColor * pow(saturate(1.0 - cosTheta), _RimPower) * _RimIntensity;
    specular += rim;

    return diffuse + specular;
}

float3 Corruption_ApplyBaseColorInfluence(float3 corruptionResult, float3 baseColor, float mask)
{
    float3 tinted = corruptionResult * baseColor * 2.0;
    float3 blended = lerp(corruptionResult, baseColor, 0.5);
    float3 combined = lerp(tinted, blended, 0.6);
    return lerp(corruptionResult, combined, _BaseColorInfluence * mask);
}

struct CorruptionResult
{
    float3 color;
    float mask;
    float3 normal;
};

CorruptionResult ComputeCorruption(float2 worldPos, float3 baseColor)
{
    CorruptionResult result;
    result.color = baseColor;
    result.mask = 0;
    result.normal = float3(0, 0, 1);
    
    if (GetCorruption() < 0.01)
        return result;
    
    RippleSample ripple = SampleRipple(worldPos);
    CorruptionSample cs = Corruption_GetSample(worldPos, ripple.offset);
    
    float mask = Corruption_SampleMask(cs.sampleUV);
    if (mask < 0.001)
        return result;
    
    float3 albedo = Corruption_ComputeAlbedo(cs.baseUV, cs.warpOffset);
    float3 normal = Corruption_SampleNormal(cs.sampleUV, mask, 1.0f);
    float3 detailNormal = Corruption_SampleNormal(cs.sampleUV, mask, _DetailsScale);
    normal = normalize(normal + detailNormal * _DetailsIntensity);
    
    float3 baseNormal = normal;
    normal = ApplyRippleToNormal(normal, ripple, mask);
    
    float rippleIntensity = ripple.intensity;
    float3 lit = Corruption_ApplyLighting(albedo, normal, baseNormal, rippleIntensity);
    float3 finalCorruption = Corruption_ApplyBaseColorInfluence(lit, baseColor, mask);
    
    result.color = lerp(baseColor, finalCorruption, mask);
    result.mask = mask;
    result.normal = normal;
    
    return result;
}

#endif

