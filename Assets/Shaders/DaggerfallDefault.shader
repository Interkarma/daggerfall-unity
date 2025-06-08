// Project:         Daggerfall Unity
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
        _SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
        _MainTex("Albedo Map", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0,0,0)
        _ParallaxMap("Parallax Map (R)", 2D) = "black" {}
        _Parallax("Parallax Scale", Range (0.005, 0.08)) = 0.05
        _MetallicGlossMap("Metallic Map (R)", 2D) = "black" {}
        _Smoothness("Smoothness", Range (0, 1)) = 0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf BlinnPhong vertex:vert
        #pragma multi_compile_local __ _NORMALMAP
        #pragma multi_compile_local __ _EMISSION
        #pragma multi_compile_local __ _PARALLAXMAP
        #pragma multi_compile_local __ _METALLICGLOSSMAP

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
        #ifdef _METALLICGLOSSMAP
            sampler2D _MetallicGlossMap;
            float _Smoothness;
        #endif

    	struct Input {
            float2 uv_MainTex;
            #ifdef _PARALLAXMAP
                float3 viewDir;
            #endif
    	};

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
        }

    	void surf (Input IN, inout SurfaceOutput o)
    	{
            // Get parallax offset
            #ifdef _PARALLAXMAP
                half height = tex2D(_ParallaxMap, IN.uv_MainTex).r;
                IN.uv_MainTex += ParallaxOffset(height, _Parallax, IN.viewDir);
            #endif

            // Albedo (colour) map
            half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Normal map
            #ifdef _NORMALMAP
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            #endif

            // Emission map
            #ifdef _EMISSION
                half3 emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor;
                o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
                o.Emission = emission;
            #else
                o.Albedo = albedo.rgb;
            #endif

            // Very rough approximation of metallic map using gloss and specular
            #ifdef _METALLICGLOSSMAP
                half4 metallicMap = tex2D(_MetallicGlossMap, IN.uv_MainTex);
                o.Gloss = 1 - metallicMap.r;
                o.Specular = _Smoothness;
            #endif

            // Assign alpha
            o.Alpha = albedo.a;
    	}
    	ENDCG
    } 
	FallBack "Diffuse"
}