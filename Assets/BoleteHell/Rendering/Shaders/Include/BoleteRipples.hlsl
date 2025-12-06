#ifndef BOLETE_RIPPLES_INCLUDED
#define BOLETE_RIPPLES_INCLUDED

sampler2D _RippleTexture;
float4 _RippleTextureBounds;

struct RippleSample
{
    float2 offset;
    float3 normal;
    float intensity;
};

RippleSample SampleRipple(float2 worldPos)
{
    RippleSample result;
    result.offset = float2(0, 0);
    result.normal = float3(0, 0, 1);
    result.intensity = 0;

    float2 boundsMin = _RippleTextureBounds.xy;
    float2 boundsMax = _RippleTextureBounds.zw;
    float2 boundsSize = boundsMax - boundsMin;

    if (boundsSize.x < 0.001 || boundsSize.y < 0.001)
        return result;

    float2 uv = saturate((worldPos - boundsMin) / boundsSize);
    float4 rippleData = tex2Dlod(_RippleTexture, float4(uv, 0, 0));
    result.offset = rippleData.xy;
    float2 encodedNormal = rippleData.zw * 2.0 - 1.0;
    
    const float normalIntensity = 0.3f;
    result.normal = normalize(float3(-encodedNormal.x * normalIntensity, -encodedNormal.y * normalIntensity, 1.0));
    result.intensity = length(result.normal.xy);
    
    return result;
}

float3 ApplyRippleToNormal(float3 baseNormal, RippleSample ripple, float mask)
{
    float3 ripplePerturbation = ripple.normal - float3(0, 0, 1);
    return normalize(baseNormal + ripplePerturbation * mask);
}

#endif

