#ifndef BOLETE_WORLD_PARAMS_INCLUDED
#define BOLETE_WORLD_PARAMS_INCLUDED

float3 _WorldLightDir;
float _Corruption;

float3 GetWorldLightDir()
{
    return normalize(_WorldLightDir);
}

float GetCorruption()
{
    return saturate(_Corruption);
}

#endif

