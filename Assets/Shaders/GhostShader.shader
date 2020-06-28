Shader "Daggerfall/GhostShader" {
	Properties {
        _Color("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _EmissionMap("Emission Map", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.0
		_Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model
		#pragma surface surf Standard alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        half4 _Color;
		sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _EmissionMap;
        half4 _EmissionColor;
        
		struct Input {
			float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_EmissionMap;
		};
        
        half _Cutoff;
		half _Glossiness;
		half _Metallic;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            clip (c.a - _Cutoff);
            
            half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
			o.Albedo = c.rgb - emission; // Emission cancels out other lights
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Emission = emission;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
            // Transparency comes from albedo alpha - allows parts some parts of image to be transparent (e.g. body) and other parts opaque (e.g. eyes)
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
