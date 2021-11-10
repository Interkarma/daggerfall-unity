// ColorBoost postprocess intended mainly for retro rendering and palettization/posterization to help select brighter colours near camera.
// Works by lerping dark fragments near camera towards unlit diffuse based on radius and intensity settings.
// Already brightly lit fragments will undergo little to no change.
// Ignores emissive fragments not included in gbuffer diffuse texture (RGB 0,0,0)
// Best used with purely diffuse classic textures. Will not yield expected results with PBR textures.
Shader "Daggerfall/PostProcess/ColorBoost"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        TEXTURE2D_SAMPLER2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0);

        float _Radius;
        float _Intensity;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord));
            float4 diffuse = SAMPLE_TEXTURE2D(_CameraGBufferTexture0, sampler_CameraGBufferTexture0, i.texcoord);

            // Ignore emissive fragments (RGB 0,0,0 in diffuse buffer)
            if (length(diffuse.rgb) == 0)
                return color;

            // Lerp towards unlit diffuse colour based on distance and intensity settings
            if (depth < _Radius)
            {
                float distance = 1 - depth / _Radius;
                color.rgb = lerp(color.rgb, diffuse.rgb, distance * _Intensity);
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