// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2016 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

// Shader used by all freestanding billboards including mobiles
// For wilderness foliage shader see DaggerfallBillboardBatch instead
Shader "Daggerfall/Billboard" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _MainTex("Albedo Map", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0,0,0)
    }
    SubShader {
        Tags { "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        LOD 200
        
        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert alphatest:_Cutoff addshadow
        #pragma multi_compile_local __ _NORMALMAP
        #pragma multi_compile_local __ _EMISSION

        half4 _Color;
        sampler2D _MainTex;
        #ifdef _NORMALMAP
            sampler2D _BumpMap;
        #endif
        #ifdef _EMISSION
            sampler2D _EmissionMap;
            half4 _EmissionColor;
        #endif

        struct Input {
            float2 uv_MainTex;
            #ifdef _NORMALMAP
                float2 uv_BumpMap;
            #endif
            #ifdef _EMISSION
                float2 uv_EmissionMap;
            #endif
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            #ifdef _NORMALMAP
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            #endif
            #ifdef _EMISSION
                half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
                o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
                o.Emission = emission;
            #else
                o.Albedo = albedo.rgb;
            #endif
            o.Alpha = albedo.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}