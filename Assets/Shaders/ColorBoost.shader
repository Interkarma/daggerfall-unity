// ColorBoost postprocess intended mainly for retro rendering and palettization/posterization to help select brighter colours near camera.
// Works by lerping dark fragments near camera towards unlit diffuse based on radius and intensity settings.
// Already brightly lit fragments will undergo little to no change.
// Ignores emissive fragments not included in gbuffer diffuse texture (RGB 0,0,0).
// Best used with purely diffuse classic textures. May not yield good results with PBR textures depending on intensity of effect.
// Can optionally darken dungeon environments towards rapid light falloff.
Shader "Daggerfall/PostProcess/ColorBoost"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        TEXTURE2D_SAMPLER2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0);

        float _Radius;
        float _Intensity;
        float _DungeonFalloffIntensity;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord));
            float4 diffuse = SAMPLE_TEXTURE2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0, i.texcoord);

            // Dungeon falloff fades everything to near black, including lights
            // Radius fixed at 90 units with variable strength
            // _DungeonFalloffIntensity is always 0 when player not in dungeon
            if (depth < 90)
            {
                float falloff = saturate(depth / 90);
                color.rgb = lerp(color.rgb, float3(0.001,0.001,0.001), falloff * _DungeonFalloffIntensity);
            }

            // Ignore emissive fragments (RGB 0,0,0 in diffuse buffer) to prevent overbrightening emissive lights and objects
            if (length(diffuse.rgb) == 0)
                return color;

            // Increase brightness of local colour based on distance and intensity settings
            if (depth < _Radius)
            {
                float falloff = 1 - saturate(depth / _Radius);
                color.rgb += color.rgb * _Intensity * falloff;
            }

            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}