// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
// Contributors:    Nystul
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
		_VisitedInThisEntering("indicates if mesh was visited in this entering", Float) = 0.0
	}
	SubShader {
		//Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent"}
		//Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent"}
		Tags { "RenderType"="Opaque"}

		LOD 200

		//// extra pass that renders to depth buffer only (world terrain is semi-transparent) - important for correct blending
		//Pass {
		//	ZWrite On
		//	ColorMask 0	
		//}
		//Blend One OneMinusSrcAlpha		
		//Blend SrcAlpha OneMinusSrcAlpha

		Fog {Mode Off}
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha nofog //noforwardadd

		half4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		half4 _EmissionColor;
		uniform float4 _PlayerPosition;
		uniform float _SclicingPositionY;
		fixed _VisitedInThisEntering;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
			o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
			o.Alpha = albedo.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = emission;
			//o.Metallic = 0;
			//o.Albedo.r +=0.2f;
			if (IN.worldPos.y > _SclicingPositionY)
			{
				//o.Alpha = 0.1f;
				discard;
			}
			//o.Albedo = half3(1.0f, 1.0f, 0.0f);

			float dist = distance(IN.worldPos.y, _SclicingPositionY); //_PlayerPosition.y);
			//o.Alpha = 1.0f - max(0.0f, min(0.1f, dist/100.0f));
			o.Albedo *= 1.0f - max(0.0f, min(0.6f, dist/20.0f));

			if (_VisitedInThisEntering == 0.0f)
			{
				half3 color = o.Albedo.rgb;
				o.Albedo = dot(color.rgb, float3(0.3, 0.59, 0.11));
			}
		}
		ENDCG
	} 
	FallBack "Standard"
}