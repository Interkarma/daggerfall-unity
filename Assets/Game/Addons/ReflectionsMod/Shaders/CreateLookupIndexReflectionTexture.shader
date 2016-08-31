Shader "Daggerfall/CreateLookupIndexReflectionTexture" {
    Properties
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MetallicGlossMap("Metallic Gloss Map", 2D) = "black" {}	
		_Metallic("metallic amount", Range(0.0, 1.0)) = 0
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
    }
		
	CGINCLUDE

	#include "UnityCG.cginc"               

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	#ifdef _METALLICGLOSSMAP
		sampler2D _MetallicGlossMap;
	#else	
		half _Metallic;
		half _Glossiness;		
	#endif

	float _GroundLevelHeight;
	float _LowerLevelHeight;     

    struct v2f
    {
            float4 pos : SV_POSITION;
			//fixed4 color : COLOR;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
			float3 worldPos : TEXCOORD2;
			float3 worldNormal : TEXCOORD3;
			float4 screenPos : TEXCOORD4;
    };

    v2f vert( appdata_full v )
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

			//o.color = v.color;

			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.worldNormal = normalize( mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);
			o.screenPos = float4(v.texcoord.x, v.texcoord.y, 0.0f, 1.0f);					
						
            return o;
    }
				
    float3 frag(v2f IN) : SV_Target
    {		            
			half4 col = tex2D(_MainTex, IN.uv);
			if (col.a < 0.5f)
				discard;

			float3 result = float3(0.0f, 0.0f, 0.0f);
			float3 vecUp = float3(0.0f,1.0f,0.0f);
			if ( (abs(IN.worldPos.y - _LowerLevelHeight) < 0.001f) && (acos(dot(normalize(IN.worldNormal), vecUp)) < 0.01f) )
			{
				result.r = 1.0f;
			}
			else if	( (abs(IN.worldPos.y - _GroundLevelHeight) < 0.1f) && (acos(dot(normalize(IN.worldNormal), vecUp)) < 0.01f) ) // fragment belong to object on current ground level plane
			{
				result.r = 0.5f;
			}
			else if	(
						(acos(dot(normalize(IN.worldNormal), vecUp)) < 0.01f) &&
						(
						(IN.worldPos.y -_GroundLevelHeight > -0.92f) && // fragment is below (use parallax-corrected reflection)
						(IN.worldPos.y - _GroundLevelHeight < 0.32f) // fragment is slightly above (use parallax-corrected reflection) - also valid for current ground level plane
						)
					)
			{
				result.r = 0.75f;
			}

			#ifdef _METALLICGLOSSMAP
				half4 metallicGloss =  tex2D(_MetallicGlossMap, IN.uv);
				half metallic = metallicGloss.r;
				result.g = metallic;
				//result.b = 0.8f; //1.0f - metallicGloss.a;
			#else
				result.g = _Metallic;
				result.b = _Glossiness;
			#endif
            return result;
    }

	ENDCG

	SubShader
	{
		Pass
		{
			Tags { "Queue"="Geometry" "RenderType"="Opaque" }

			CGPROGRAM
			#pragma exclude_renderers gles xbox360 ps3
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma shader_feature _METALLICGLOSSMAP
			ENDCG
		}
	}	

    Fallback "None"
}





















/*
Shader "Daggerfall/CreateLookupIndexReflectionTexture" {
    Properties
    {
            _MainTex ("Base (RGB)", 2D) = "white" {}
    }
		
	SubShader
	{
		Lighting Off

		CGPROGRAM

		#pragma target 3.0
		#pragma surface surf Lambert //vertex:vert		
		#pragma glsl	

		#include "UnityCG.cginc"               

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		float _GroundLevelHeight;
		float _LowerLevelHeight;  
		
		sampler2D _CameraDepthTexture; 

		struct Input
		{
				float4 pos : SV_POSITION;
				//fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
		};

		Input vert( appdata_full v )
		{
				Input o;
				UNITY_INITIALIZE_OUTPUT(Input, o);

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.uv2 = v.texcoord.xy;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
							o.uv2.y = 1-o.uv2.y;
				#endif

				//o.color = v.color;

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = normalize( mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);
				o.screenPos = float4(v.texcoord.x, v.texcoord.y, 0.0f, 1.0f);					
						
				return o;
		}
				
		void surf(Input IN, inout SurfaceOutput o)
		{
				//float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);

				//if (IN.color.a < 0.5f)
				//{
				//	discard;
				//}
				/*
				float result = 0.0f;
				float3 vecUp = float3(0.0f,1.0f,0.0f);
				if ( (abs(IN.worldPos.y - _LowerLevelHeight) < 0.001f) && (normalize(dot(IN.worldNormal, vecUp)) > 0.0f) )
				{
					result = 1.0f; //255.0f / 18.0f;
				}
				else if	(	//(abs(IN.worldPos.y - _GroundLevelHeight) < 0.1f)|| // fragment belong to object on current ground level plane
							(normalize(dot(IN.worldNormal, vecUp)) == 0.0f)&&
							(
							(IN.worldPos.y < _GroundLevelHeight)|| // fragment is below (use parallax-corrected reflection)
							(IN.worldPos.y - _GroundLevelHeight < 0.32f) // fragment is slightly above (use parallax-corrected reflection) - also valid for current ground level plane
							)
						)
				{
					result = 2.0f;
				}
				o.Albedo = result;
				*/
				o.Albedo = half3(1.0f, 0.0f, 0.0f); //tex2D(_CameraDepthTexture, IN.uv.xy); //half3(1.0f, 0.0f, 0.0f);
		}

		ENDCG
	}	

    Fallback "None"
}

*/