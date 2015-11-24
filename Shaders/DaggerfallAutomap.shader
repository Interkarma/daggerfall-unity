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



/*
Shader "Daggerfall/Automap"
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
	SubShader 
	{
	
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

    	Pass 
    	{

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#pragma multi_compile __ RENDER_IN_GRAYSCALE
			
			half4 _Color;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _EmissionMap;
			half4 _EmissionColor;
			uniform float4 _PlayerPosition;
			uniform float _SclicingPositionY;
		
			struct v2g 
			{
    			float4  pos : SV_POSITION;				
    			float2  uv : TEXCOORD0;
				float3 worldPos : TEXCOORD5;
			};
			
			struct g2f 
			{
    			float4  pos : SV_POSITION;
    			float2  uv : TEXCOORD0;
    			float3 dist : TEXCOORD1;
				float3 worldPos : TEXCOORD5;
			};

			//v2g vert(appdata_full v)
			void vert(appdata_full v, out v2g OUT)
			{
    			//v2g OUT;
				//UNITY_INITIALIZE_OUTPUT(v2g, OUT);
    			OUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    			OUT.uv = v.texcoord; //the uv's arent used in this shader but are included in case you want to use them
				OUT.worldPos = mul(_Object2World, v.vertex).xyz;
    			//return OUT;
			}
			
			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{
			
				float2 WIN_SCALE = float2(_ScreenParams.x/2.0, _ScreenParams.y/2.0);
				
				//frag position
				float2 p0 = WIN_SCALE * IN[0].pos.xy / IN[0].pos.w;
				float2 p1 = WIN_SCALE * IN[1].pos.xy / IN[1].pos.w;
				float2 p2 = WIN_SCALE * IN[2].pos.xy / IN[2].pos.w;
				
				//barycentric position
				float2 v0 = p2-p1;
				float2 v1 = p2-p0;
				float2 v2 = p1-p0;
				//triangles area
				float area = abs(v1.x*v2.y - v1.y * v2.x);
			
				g2f OUT;
				OUT.pos = IN[0].pos;
				OUT.uv = IN[0].uv;
				OUT.dist = float3(area/length(v0),0,0);
				OUT.worldPos = IN[0].worldPos;
				triStream.Append(OUT);

				OUT.pos = IN[1].pos;
				OUT.uv = IN[1].uv;
				OUT.dist = float3(0,area/length(v1),0);
				OUT.worldPos = IN[1].worldPos;
				triStream.Append(OUT);

				OUT.pos = IN[2].pos;
				OUT.uv = IN[2].uv;
				OUT.dist = float3(0,0,area/length(v2));
				OUT.worldPos = IN[2].worldPos;
				triStream.Append(OUT);
				
				
				
			}
			
			half4 frag(g2f IN) : COLOR
			{
				float4 outColor;
				half4 albedo = tex2D(_MainTex, IN.uv) * _Color;
				half3 emission = tex2D(_EmissionMap, IN.uv).rgb * _EmissionColor;
				outColor.rgb = albedo.rgb - emission; // Emission cancels out other lights
				outColor.a = albedo.a;
				if (IN.worldPos.y > _SclicingPositionY)
				{
					//distance of frag from triangles center
					float d = min(IN.dist.x, min(IN.dist.y, IN.dist.z));
					//fade based on dist from center
 					float I = exp2(-4.0*d*d);
 				
 					//return lerp(_Color, _WireColor, I);				
					if (I<0.1f)
					{
						clip( -1.0 );
						return float4( 1.0, 0.0, 0.0, 1.0 );
					}
					else
					{
						return float4( 0.9, 0.9, 0.7, 1.0 ); // outColor;
					}
				}

				float dist = distance(IN.worldPos.y, _SclicingPositionY);
				outColor.rgb *= 1.0f - max(0.0f, min(0.6f, dist/20.0f));

				// TODO: make render in grayscale work again
//		#if defined(RENDER_IN_GRAYSCALE)
//				half3 color = outColor;
//				outColor = dot(color.rgb, float3(0.3, 0.59, 0.11));		
//		#endif

				return outColor;

			}
			
			ENDCG

    	}
	}
}
*/
