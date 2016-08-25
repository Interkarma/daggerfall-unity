Shader "Daggerfall/CreateLookupIndices" {
    Properties
    {
            _MainTex ("Base (RGB)", 2D) = "white" {}
    }
		
	CGINCLUDE

	#include "UnityCG.cginc"               

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	float _GroundLevelHeight;
	float _LowerLevelHeight;     

    struct v2f
    {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
			//float3 worldPos : TEXCOORD2;
			//float4 screenPos : TEXCOORD3;
			//float4 parallaxCorrectedScreenPos : TEXCOORD4;
    };

    v2f vert( appdata_img v )
    {
            v2f o;
			//UNITY_INITIALIZE_OUTPUT(v2f, o);

            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.texcoord.xy;
            o.uv2 = v.texcoord.xy;
            #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                        o.uv2.y = 1-o.uv2.y;
            #endif

			//o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			//o.screenPos = float4(v.texcoord.x, v.texcoord.y, 0.0f, 1.0f);					
						
            return o;
    }
				
    float4 frag(v2f IN) : SV_Target
    {
			float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
            return result;
    }

	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles xbox360 ps3
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
	}	

    Fallback "Diffuse"
}























/*
Shader "Daggerfall/CreateLookupIndices" {
    Properties
    {
            _MainTex ("Base (RGB)", 2D) = "white" {}
    }

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
	
		#pragma target 3.0
		#pragma surface surf Lambert vertex:customvert finalcolor:fcolor
		#pragma glsl


		sampler2D _MainTex;

		struct Input
		{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				//float3 worldPos : TEXCOORD2;
				//float4 screenPos : TEXCOORD3;
				//float4 parallaxCorrectedScreenPos : TEXCOORD4;
		};

		void customvert( inout appdata_full v, out Input o)
		{
				//Input o;
				//UNITY_INITIALIZE_OUTPUT(Input, o);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.uv2 = v.texcoord.xy;	
				//return o;
		}

		void fcolor (Input IN, SurfaceOutput o, inout fixed4 color)
		{
			color = float4(1.0f, 0.0f, 0.0f, 0.5f);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = float3(1.0f, 0.0f, 0.0f);
		}
	
		ENDCG
		
	}	

    FallBack "Diffuse"
}
*/
