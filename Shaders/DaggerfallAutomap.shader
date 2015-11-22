// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Michael Rauter (a.k.a. Nystul)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
// Contributors:    
// 
// Notes:
//

Shader "Daggerfall/Automap" {
	// Surface Shader for Automap Geometry
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_PlayerPosition("player position", Vector) = (0,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200

		Fog {Mode Off}
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha nofog

		#pragma multi_compile __ RENDER_IN_GRAYSCALE

		half4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		half4 _EmissionColor;
		uniform float4 _PlayerPosition;
		uniform float _SclicingPositionY;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
			o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
			o.Alpha = albedo.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = emission;
			o.Metallic = 0;
			if (IN.worldPos.y > _SclicingPositionY)
			{
				discard;
			}

			float dist = distance(IN.worldPos.y, _SclicingPositionY); //_PlayerPosition.y);
			o.Albedo *= 1.0f - max(0.0f, min(0.6f, dist/20.0f));

#if defined(RENDER_IN_GRAYSCALE)
			half3 color = o.Albedo.rgb;
			o.Albedo = dot(color.rgb, float3(0.3, 0.59, 0.11));		
#endif
		}
		ENDCG
	} 
	FallBack "Standard"
}