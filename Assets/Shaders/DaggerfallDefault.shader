// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

// Shader used by unmodded game or mods not wanting to use Standard shader or PBR workflow
// Mods requiring a full PBR workflow or more features should use Standard or a custom shader
Shader "Daggerfall/Default" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo Map", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0,0,0)
        _ParallaxMap ("Heightmap (R)", 2D) = "black" {}
        _Parallax ("Height", Range (0.005, 0.08)) = 0.04
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert
        #pragma multi_compile_local __ _NORMALMAP
        #pragma multi_compile_local __ _EMISSION
        #pragma multi_compile_local __ _PARALLAXMAP

        half4 _Color;
        sampler2D _MainTex;
        #ifdef _NORMALMAP
    	    sampler2D _BumpMap;
        #endif
        #ifdef _EMISSION
            sampler2D _EmissionMap;
            half4 _EmissionColor;
        #endif
        #ifdef _PARALLAXMAP
            sampler2D _ParallaxMap;
            float _Parallax;
        #endif

    	struct Input {
            float2 uv_MainTex;
            #ifdef _NORMALMAP
                float2 uv_BumpMap;
            #endif
            #ifdef _EMISSION
                float2 uv_EmissionMap;
            #endif
            #ifdef _PARALLAXMAP
                float3 viewDir;
                float2 uv_ParallaxMap;
                float _Parallax;
            #endif
    	};

    	void surf (Input IN, inout SurfaceOutput o)
    	{
            // Adjust input UVs for Parallax (height) map offsets - uses R channel for height data
            #ifdef _PARALLAXMAP
                half h = tex2D(_ParallaxMap, IN.uv_ParallaxMap).r;
                float2 offset = ParallaxOffset(h, _Parallax, IN.viewDir);
                IN.uv_MainTex += offset;
                #ifdef _NORMALMAP
                    IN.uv_BumpMap += offset;
                #endif
                #ifdef _EMISSION
                    IN.uv_EmissionMap += offset;
                #endif
            #endif

            // Albedo (colour) map
            half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Normal map
            #ifdef _NORMALMAP
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            #endif

            // Emission map
            #ifdef _EMISSION
                half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
                o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
                o.Emission = emission;
            #else
                o.Albedo = albedo.rgb;
            #endif

            // Assign alpha
            o.Alpha = albedo.a;
    	}
    	ENDCG
    } 
	FallBack "Diffuse"
}