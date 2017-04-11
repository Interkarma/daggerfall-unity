//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

Shader "Daggerfall/ReflectionsMod2/FloorMaterialWithReflections" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "white" {}
		[HideInInspector] _ReflectionGroundTex("Reflection Texture Ground Reflection", 2D) = "black" {}
		[HideInInspector] _ReflectionLowerLevelTex("Reflection Texture for lower level Ground Reflection", 2D) = "black" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_MetallicGlossMap("Metallic Gloss Map", 2D) = "black" {}	
		_Metallic("metallic amount", Range(0.0, 1.0)) = 0
		_Smoothness("smoothness amount", Range(0.0, 1.0)) = 0
		[HideInInspector] _GroundLevelHeight("ground level height", Float) = 0.0
		[HideInInspector] _LowerLevelHeight("lower level height", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0 		
		#pragma surface surf Standard vertex:customvert
		#pragma glsl
		
		#pragma multi_compile __ _METALLICGLOSSMAP

		half4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		sampler2D _ReflectionGroundTex;
		sampler2D _ReflectionLowerLevelTex;
		half4 _EmissionColor;

		float _GroundLevelHeight;
		float _LowerLevelHeight;

		#if defined (_METALLICGLOSSMAP)
			sampler2D _MetallicGlossMap;
		#else	
			half _Metallic;
			half _Smoothness;		
		#endif

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			float3 worldPos;
			float4 screenPos;
			float4 parallaxCorrectedScreenPos : TEXCOORD1;
		};
		
		void customvert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
				
			float4 posWorldSpace = mul(unity_ObjectToWorld, v.vertex);
			if ((abs(posWorldSpace.y - _GroundLevelHeight) < 0.01f) || (abs(posWorldSpace.y - _LowerLevelHeight) < 0.01f))
			{
				o.parallaxCorrectedScreenPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, posWorldSpace));
			}
			else
			{
				// parallax-correct reflection position
				if (posWorldSpace.y > _GroundLevelHeight+0.01f)
					o.parallaxCorrectedScreenPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, posWorldSpace-float4(0.0f, posWorldSpace.y - _GroundLevelHeight + 0.25f, 0.0f, 0.0f)));
				else if (posWorldSpace.y < _GroundLevelHeight-0.01f)
					o.parallaxCorrectedScreenPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, posWorldSpace-float4(0.0f, posWorldSpace.y - _GroundLevelHeight - 0.14f, 0.0f, 0.0f)));				
				else
					o.parallaxCorrectedScreenPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, posWorldSpace-float4(0.0f, posWorldSpace.y - _GroundLevelHeight, 0.0f, 0.0f)));
			}
		}

		float3 getReflectionColor(sampler2D tex, float2 screenUV, float smoothness)
		{		
			half mipmapLevel1 = floor(smoothness);
			half mipmapLevel2 = ceil(smoothness);
			float w = smoothness - mipmapLevel1;
			return((1.0f-w) * tex2Dlod(tex, float4(screenUV, 0.0f, mipmapLevel1)).rgb + w * tex2Dlod(tex, float4(screenUV, 0.0f, mipmapLevel2)).rgb);
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor.rgb;
			
			//float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			float2 screenUV = IN.parallaxCorrectedScreenPos.xy / IN.parallaxCorrectedScreenPos.w;

			half3 refl;

			const half fadeWidth = 0.3f;
			half fadeOutFactX = min(1.0f, max(0.0f, abs(0.5f - screenUV.x) - (0.5f-fadeWidth)) / fadeWidth);
			half fadeOutFactY = min(1.0f, max(0.0f, abs(0.5f - screenUV.y) - (0.5f-fadeWidth)) / fadeWidth);

			half fadeOutFact = 0.0f;
			if ((abs(IN.worldPos.y - _GroundLevelHeight) > 0.01f) && (abs(IN.worldPos.y - _LowerLevelHeight) > 0.01f))
				fadeOutFact = max(fadeOutFactX, fadeOutFactY);

			#if defined (_METALLICGLOSSMAP)
				half4 metallicGloss =  tex2D(_MetallicGlossMap, IN.uv_MainTex);
				half metallic = metallicGloss.r * (1.0f-fadeOutFact);
				half smoothness = (1.0f - metallicGloss.a) * 8.0f;
			#else
				half metallic = _Metallic * (1.0f-fadeOutFact);
				half smoothness = (1.0f-_Smoothness) * 8.0f;
			#endif

			//if ((screenUV.x>0.0f)&&(screenUV.x<1.0f)&&(screenUV.y>0.0f)&&(screenUV.y<1.0f))
			{
				if (abs(IN.worldPos.y - _LowerLevelHeight) < 0.01f)
				{
					refl = getReflectionColor(_ReflectionLowerLevelTex, screenUV, smoothness); //refl = tex2Dlod(_ReflectionLowerLevelTex, float4(screenUV, 0.0f, _Smoothness)).rgb;
					albedo.rgb += metallic * refl; // + (1.0f - metallic) * albedo.rgb;
					albedo.rgb *= (1.0f/(1.0f + metallic));
				}
				else if	(	//(abs(IN.worldPos.y - _GroundLevelHeight) < 0.01f)|| // fragment belong to object on current ground level plane
							(IN.worldPos.y < _GroundLevelHeight)|| // fragment is below (use parallax-corrected reflection)
							(IN.worldPos.y - _GroundLevelHeight < 0.32f) // fragment is slightly above (use parallax-corrected reflection) - also valid for current ground level plane
						)
				{
					refl = getReflectionColor(_ReflectionGroundTex, screenUV, smoothness); //refl = tex2Dlod(_ReflectionGroundTex, float4(screenUV, 0.0f, _Smoothness)).rgb;
					albedo.rgb += metallic * refl; // + (1.0f - metallic) * albedo.rgb;
					albedo.rgb *= (1.0f/(1.0f + metallic));
				}
				else
				{
					albedo.rgb += metallic * 0.2f; // correction of x/z planes without reflection texture applied - 0.2 seems to work ok for most situations
					albedo.rgb *= (1.0f/(1.0f + metallic));
				}
			}

			o.Albedo = albedo.rgb; // - emission; // Emission cancels out other lights
			o.Alpha = albedo.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = emission;		
			o.Metallic = metallic;
		}
		ENDCG
	} 
	FallBack "Standard"
}
