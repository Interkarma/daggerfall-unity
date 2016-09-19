//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

// used as replacement shader to create reflection texture sampling index (which reflection texture to sample from) for every fragment (in r channel of texture),
// metallic amount is stored in g channel of the texture, glossiness amount is stored in b channel of the texture (for possible use in DeferredPlanarReflections shader)
Shader "Daggerfall/ReflectionsMod/CreateLookupReflectionTextureIndex" {
    Properties
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
				
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0
    }
		
	CGINCLUDE

	#include "UnityCG.cginc"
	//#include "UnityStandardInput.cginc" // function MetallicGloss defined there, but also lots of other stuff not needed

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	#ifdef _METALLICGLOSSMAP
		sampler2D _MetallicGlossMap;
	#else	
		half _Metallic;
		half _Glossiness;		
	#endif
	half _GlossMapScale;

	float _GroundLevelHeight;
	float _LowerLevelHeight;

	half2 MetallicGloss(float2 uv)
	{
		half2 mg;
	
	#ifdef _METALLICGLOSSMAP
		#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			mg.r = tex2D(_MetallicGlossMap, uv).r;
			mg.g = tex2D(_MainTex, uv).a;
		#else
			mg = tex2D(_MetallicGlossMap, uv).ra;
		#endif
		mg.g *= _GlossMapScale;
	#else
		mg.r = _Metallic;
		#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			mg.g = tex2D(_MainTex, uv).a * _GlossMapScale;
		#else
			mg.g = _Glossiness;
		#endif
	#endif
		return mg;
	}

    struct v2f
    {
            float4 pos : SV_POSITION;
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

			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.worldNormal = normalize( mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz);
			o.screenPos = float4(v.texcoord.x, v.texcoord.y, 0.0f, 1.0f);					
						
            return o;
    }
				
    half3 frag(v2f IN) : SV_Target
    {		            
			half4 col = tex2D(_MainTex, IN.uv);
			if (col.a < 0.5f)
				discard;

			half3 result = float3(0.0f, 0.0f, 0.0f);
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
			
			half2 mg = MetallicGloss(IN.uv);
			result.gb = mg;

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
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A			
			ENDCG
		}
	}	

    Fallback "None"
}