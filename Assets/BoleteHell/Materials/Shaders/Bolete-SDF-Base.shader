Shader "Bolete Hell/Sprite SDF Shaded"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        
        [Header(SDF Edge Shading)]
        [Toggle(DEBUG_SDF)] _DebugSDF("Debug - Show Raw SDF", Float) = 0
        _SDFInfluence("SDF Influence", Range(0, 1)) = 1.0
        _EdgeDarken("Edge Darkening", Range(0, 1)) = 0.3
        _EdgeWidth("Edge Width", Range(0.01, 0.2)) = 0.05
        _OutsideDarken("Outside Darkening", Range(0, 1)) = 0.5
        
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 10 // OneMinusSrcAlpha
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 // LEqual
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        // Default blend mode - can still be overridden per-pass if needed
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            
            // Override blend/zwrite/ztest per-pass if properties are set
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local DEBUG_SDF

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 screenPos    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_ScreenSpaceSDF);
            SAMPLER(sampler_ScreenSpaceSDF);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _SDFInfluence;
                half _EdgeDarken;
                half _EdgeWidth;
                half _OutsideDarken;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample the sprite texture
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 baseColor = texColor * input.color * _Color;
                
                // Sample the SDF at this fragment's screen position
                // The SDF tells us the distance from this pixel to the edge of the CURRENT shape being rendered
                // (not distance to other obstacles - each shape samples its own edge distance)
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                half sdfValue = SAMPLE_TEXTURE2D(_ScreenSpaceSDF, sampler_ScreenSpaceSDF, screenUV).r;
                
                #ifdef DEBUG_SDF
                    // Debug mode: visualize raw SDF values
                    // Black = far outside shape, Grey = at shape edge, White = inside shape
                    return half4(sdfValue.xxx, 1.0);
                #endif
                
                // SDF interpretation for the current shape:
                // 0.0 = far outside this shape's silhouette
                // 0.5 = at this shape's edge
                // 1.0 = far inside this shape's silhouette
                
                // Calculate edge proximity (0 = at edge, 1 = far from edge)
                half distFromEdge = abs(sdfValue - 0.5) * 2.0; // Remap [0,1] to distance from 0.5
                half edgeFactor = saturate(distFromEdge / _EdgeWidth);
                
                // Calculate darkening based on whether we're inside or outside
                half isInside = step(0.5, sdfValue); // 1 if inside, 0 if outside
                half outsideFactor = (1.0 - isInside) * _OutsideDarken;
                
                // Combine edge darkening and outside darkening
                half edgeDarkening = (1.0 - edgeFactor) * _EdgeDarken;
                half totalDarkening = max(edgeDarkening, outsideFactor);
                
                // Apply darkening
                half brightness = 1.0 - totalDarkening;
                half3 shadedColor = baseColor.rgb * brightness;
                
                // Blend between original and shaded based on influence
                half3 finalColor = lerp(baseColor.rgb, shadedColor, _SDFInfluence);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local DEBUG_SDF

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 screenPos    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_ScreenSpaceSDF);
            SAMPLER(sampler_ScreenSpaceSDF);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _SDFInfluence;
                half _EdgeDarken;
                half _EdgeWidth;
                half _OutsideDarken;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 baseColor = texColor * input.color * _Color;
                
                // Sample SDF for the current shape's edge distance
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                half sdfValue = SAMPLE_TEXTURE2D(_ScreenSpaceSDF, sampler_ScreenSpaceSDF, screenUV).r;
                
                #ifdef DEBUG_SDF
                    return half4(sdfValue.xxx, 1.0);
                #endif
                
                half distFromEdge = abs(sdfValue - 0.5) * 2.0;
                half edgeFactor = saturate(distFromEdge / _EdgeWidth);
                
                half isInside = step(0.5, sdfValue);
                half outsideFactor = (1.0 - isInside) * _OutsideDarken;
                
                half edgeDarkening = (1.0 - edgeFactor) * _EdgeDarken;
                half totalDarkening = max(edgeDarkening, outsideFactor);
                
                half brightness = 1.0 - totalDarkening;
                half3 shadedColor = baseColor.rgb * brightness;
                half3 finalColor = lerp(baseColor.rgb, shadedColor, _SDFInfluence);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
    }
    
    Fallback "Sprites/Default"
}
