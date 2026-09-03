Shader "Bolete Hell/Internal/Silhouette"
{
   SubShader
   {
       Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
       ZWrite Off Cull Off ZTest Always
       Pass
       {
           HLSLPROGRAM
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

           #pragma vertex vert
           #pragma fragment frag

           CBUFFER_START(UnityPerMaterial)
           float4x4 _WorldToBufferClip;
           CBUFFER_END

           struct Attributes
           {
               float3 positionOS   : POSITION;
           };

           struct Varyings
           {
               float4  positionCS  : SV_POSITION;
           };

           Varyings vert(Attributes i)
           {
               Varyings o;
               float3 world = TransformObjectToWorld(i.positionOS);
               o.positionCS = mul(_WorldToBufferClip, float4(world, 1.0));
               return o;
           }
 
           float4 frag(Varyings i) : SV_Target
           {
               float height01 = saturate(asfloat(unity_RendererUserValue));
               return float4(height01, 0, 0, 1);
           }

           ENDHLSL
       }
   }
}