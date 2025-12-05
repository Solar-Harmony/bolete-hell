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
               o.positionCS = TransformObjectToHClip(i.positionOS);
               return o;
           }
 
           float4 frag(Varyings i) : SV_Target
           {
               return float4(1, 1, 1, 1);
           }

           ENDHLSL
       }
   }
}