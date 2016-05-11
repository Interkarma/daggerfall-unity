// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Michael Rauter (a.k.a. Nystul)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
// Contributors:    
// 
// Notes: original wireframe shader source: http://scrawkblog.com/2013/03/18/wireframe-shader-for-unity/
//

Shader "Daggerfall/AutomapBelowSclicePlane"
{

	// Surface Shader for Automap Geometry
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_PlayerPosition("player position", Vector) = (0,0,0,1)
	}	

	SubShader // fallback shader for target 3.0
	{
	
	    Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }
		//Blend SrcAlpha OneMinusSrcAlpha

    	Pass 
    	{
			//ZWrite On
			//ColorMask 0

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile __ RENDER_IN_GRAYSCALE
			#pragma multi_compile __ AUTOMAP_RENDER_MODE_TRANSPARENT
			
			half4 _Color;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _EmissionMap;
			half4 _EmissionColor;
			uniform float4 _PlayerPosition;
			uniform float _SclicingPositionY;
		
			struct v2f
			{
    			float4  pos : SV_POSITION;				
    			float2  uv : TEXCOORD0;
				float3 worldPos : TEXCOORD5;
			};		

			void vert(appdata_full v, out v2f OUT)
			{
    			OUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    			OUT.uv = v.texcoord;
				OUT.worldPos = mul(_Object2World, v.vertex).xyz;
			}


			half4 frag(v2f IN) : COLOR
			{
				float4 outColor;
				half4 albedo = tex2D(_MainTex, IN.uv) * _Color;
				half3 emission = tex2D(_EmissionMap, IN.uv).rgb * _EmissionColor;
				outColor.rgb = albedo.rgb - emission; // Emission cancels out other lights
				outColor.a = albedo.a;
				if (IN.worldPos.y > _SclicingPositionY)
				{				
					discard;				
				}

				float dist = distance(IN.worldPos.y, _SclicingPositionY);
				outColor.rgb *= 1.0f - max(0.0f, min(0.6f, dist/20.0f));

				#if defined(RENDER_IN_GRAYSCALE)
					half3 color = outColor;
					float grayValue = dot(color.rgb, float3(0.3, 0.59, 0.11));
					outColor.rgb = half3(grayValue, grayValue, grayValue);
				#endif

				return outColor;

			}
			
			ENDCG

    	}
	}
	FallBack "Standard"
}
