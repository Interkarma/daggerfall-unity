// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Daggerfall/DeferredPlanarReflections" {
    Properties
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
		
	CGINCLUDE

	#include "UnityCG.cginc"
    #include "UnityPBSLighting.cginc"
    #include "UnityStandardBRDF.cginc"
    #include "UnityStandardUtils.cginc"        

    sampler2D _CameraGBufferTexture0;
    sampler2D _CameraGBufferTexture1;
    sampler2D _CameraGBufferTexture2;
    sampler2D _CameraGBufferTexture3;
    sampler2D _CameraReflectionsTexture;
    sampler2D _MainTex;
	
	sampler2D_float _CameraDepthTexture;
	sampler2D _FinalReflectionTexture;

	sampler2D _ReflectionGroundTex;
	sampler2D _ReflectionLowerLevelTex;

	sampler2D _LookupIndicesTex;
	sampler2D _IndexReflectionsTextureTex;

	float4 _MainTex_TexelSize;
    float4 _ProjInfo;
    float4x4 _CameraToWorldMatrix;
	float4x4 _InverseViewProject;

	float _GroundLevelHeight;
	float _LowerLevelHeight;

    struct v2f
    {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
    };

                float3 ReconstructCSPosition(float2 S, float z)
                {
                        float linEyeZ = -LinearEyeDepth(z);
                        return float3(( (( S.xy * _MainTex_TexelSize.zw) ) * _ProjInfo.xy + _ProjInfo.zw) * linEyeZ, linEyeZ);
                }

                /** Read the camera-space position of the point at screen-space pixel ssP */
                float3 GetPosition(float2 ssP)
                {
                        float3 P;

                        P.z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, ssP.xy);

                        // Offset to pixel center
                        P = ReconstructCSPosition(float2(ssP) /*+ float2(0.5, 0.5)*/, P.z);
                        return P;
                }

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
						
            return o;
    }

	float3 getReflectionColor(sampler2D tex, float2 screenUV, float smoothness)
	{		
		half mipmapLevel1 = floor(smoothness);
		half mipmapLevel2 = ceil(smoothness);
		float w = smoothness - mipmapLevel1;
		return((1.0f-w) * tex2Dlod(tex, float4(screenUV, 0.0f, mipmapLevel1)).rgb + w * tex2Dlod(tex, float4(screenUV, 0.0f, mipmapLevel2)).rgb);
	}
				
    float4 fragReflection(v2f IN) : SV_Target
    {
			//float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			//float2 screenUV = IN.parallaxCorrectedScreenPos.xy / IN.parallaxCorrectedScreenPos.w;
			float2 screenUV = IN.uv2.xy;
			float2 parallaxCorrectedScreenUV = tex2D(_LookupIndicesTex, screenUV).xy;

			half3 refl = half3(0.0f, 0.0f, 0.0f);			

			//float roughness = tex2D(_CameraGBufferTexture1, screenUV).a;

			//float roughness = 1.0-tex2D(_CameraGBufferTexture1, screenUV).a;

			float roughness = 1.0f - tex2D(_IndexReflectionsTextureTex, screenUV).b;

			//roughness = pow(roughness, 4.0/3.0);

			float indexReflectionsTextureTex = tex2D(_IndexReflectionsTextureTex, screenUV).r;
			if (indexReflectionsTextureTex == 1.0f)
			{
				refl = getReflectionColor(_ReflectionLowerLevelTex, screenUV, roughness * 8); //refl = tex2Dlod(_ReflectionLowerLevelTex, float4(screenUV, 0.0f, _Smoothness)).rgb;
			}
			else if	(abs(indexReflectionsTextureTex - 0.5f) < 0.01f)
			{
				refl = getReflectionColor(_ReflectionGroundTex, parallaxCorrectedScreenUV.xy, roughness * 8); //refl = tex2Dlod(_ReflectionGroundTex, float4(screenUV, 0.0f, _Smoothness)).rgb;
			}
			else if	(abs(indexReflectionsTextureTex - 0.75f) < 0.01f)
			{
				const half fadeWidth = 0.3f;
				half fadeOutFactX = min(1.0f, max(0.0f, abs(0.5f - parallaxCorrectedScreenUV.x) - (0.5f-fadeWidth)) / fadeWidth);
				half fadeOutFactY = min(1.0f, max(0.0f, abs(0.5f - parallaxCorrectedScreenUV.y) - (0.5f-fadeWidth)) / fadeWidth);

				half fadeOutFact = 0.0f;
				fadeOutFact = max(fadeOutFactX, fadeOutFactY);

				refl = (1.0f-fadeOutFact) * getReflectionColor(_ReflectionGroundTex, parallaxCorrectedScreenUV.xy, roughness * 8); //refl = tex2Dlod(_ReflectionGroundTex, float4(screenUV, 0.0f, _Smoothness)).rgb;
			}
		
				//if (tex2D(_CameraGBufferTexture1, screenUV).r < 0.4f)
				//if (_Metallic == 0.6f) //
				//if (abs(_Metallic-0.5f) < 0.05f)
				refl *= 8.0f * tex2D(_IndexReflectionsTextureTex, screenUV).g;
				//refl *= 2.0f; //_Metallic; //tex2D(_CameraGBufferTexture1, screenUV).r; // float3(0.0,0.0,0.0);


                        float2 tsP = IN.uv2.xy;

                        // View space point being shaded
                        float3 C = GetPosition(tsP);

                        // Final image before this pass
                        float4 gbuffer3 = tex2D(_MainTex, IN.uv);

                        float4 specEmission = float4(0.0,0.0,0.0,0.0);
                        //float3 specColor = tex2D(_CameraGBufferTexture1, tsP).rgb;
						float3 specColor = refl.rgb;

                        //float roughness = 1.0-tex2D(_CameraGBufferTexture1, tsP).a;

                        float4 reflectionTexel = tex2D(_FinalReflectionTexture, tsP);

                        float4 gbuffer0 = tex2D(_CameraGBufferTexture0, tsP);
                        // Let core Unity functions do the dirty work of applying the BRDF
                        float3 baseColor = gbuffer0.rgb;
                        float occlusion = gbuffer0.a;
                        float oneMinusReflectivity;
                        baseColor = EnergyConservationBetweenDiffuseAndSpecular(baseColor, specColor, oneMinusReflectivity);

                        float3 wsNormal = tex2D(_CameraGBufferTexture2, tsP).rgb * 2.0 - 1.0;

                        float3 csEyeVec = normalize(C);
                        float3 eyeVec = mul(_CameraToWorldMatrix, float4(csEyeVec, 0)).xyz;

                        float3 incomingRadiance = reflectionTexel.rgb;

                        UnityLight light;
                        light.color = 0;
                        light.dir = 0;
                        light.ndotl = 0;

                        UnityIndirect ind;
                        ind.diffuse = 0;
                        ind.specular = incomingRadiance;

                        float3 reflResult = UNITY_BRDF_PBS (0, specColor, oneMinusReflectivity, 1-roughness, wsNormal, -eyeVec, light, ind).rgb;
                        float confidence = reflectionTexel.a;

                        specEmission.rgb = tex2D(_CameraReflectionsTexture, tsP).rgb;
                        float3 finalGlossyTerm;

                        // Subtract out Unity's glossy result: (we're just applying the delta)
						/*
                        {
                                gbuffer3 -= specEmission;
                                // We may have blown out our dynamic range by adding then subtracting the reflection probes.
                                // As a half-measure to fix this, simply clamp to zero
                                gbuffer3 = max(gbuffer3, 0);
                                finalGlossyTerm = lerp(specEmission.rgb, reflResult, saturate(confidence));
                        }
						*/
                        {
							finalGlossyTerm = reflResult*saturate(confidence);
                        }
						
						//finalGlossyTerm = reflResult;

                        finalGlossyTerm *= occlusion;

            //float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
			
			//float4 result = float4(refl.r, refl.g, refl.b, 1.0f); //*0.3f;
			//float4 result = float4(tex2D(_CameraGBufferTexture1, screenUV).rgb, 1.0f);
			//float4 result = float4(_Metallic, 0.0f, 0.0f, 1.0f);			
			
			//float4 result = float4(finalGlossyTerm, 0);
			
			
			float4 result = float4(tex2D(_CameraReflectionsTexture, tsP).rgb, 0);
			if ((refl.r > 0.0f) || (refl.g > 0.0f) || (refl.b > 0.0f))
				result = float4(finalGlossyTerm, 0);
			
            return result;
    }

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles xbox360 ps3
			#pragma vertex vert
			#pragma fragment fragReflection
			#pragma target 3.0
			//#pragma multicompile __ _METALLICGLOSSMAP
			#pragma shader_feature _METALLICGLOSSMAP 
			// USE_METALLICGLOSSMAP 			
			ENDCG
		}
	}	

    Fallback "None"
}






/*

Shader "Daggerfall/DeferredPlanarReflections"
{
        Properties
        {
                _MainTex ("Base (RGB)", 2D) = "white" {}
        }
		

        CGINCLUDE
			
                //#include "UnityCG.cginc"
                //#include "UnityPBSLighting.cginc"
                //#include "UnityStandardBRDF.cginc"
                //#include "UnityStandardUtils.cginc"
                //#include "ScreenSpaceRaytrace.cginc"
				
                sampler2D _CameraGBufferTexture0;
                sampler2D _CameraGBufferTexture1;
                sampler2D _CameraGBufferTexture2;
                sampler2D _CameraGBufferTexture3;
                sampler2D _CameraReflectionsTexture;

                sampler2D _MainTex;

				//sampler2D _FinalReflectionTexture;
				//sampler2D _TempTexture;
                //sampler2D _FinalReflectionTexture;
				
				//float4 _MainTex_TexelSize;

                struct v2f
                {
                        float4 pos : SV_POSITION;
                        float2 uv : TEXCOORD0;
                        float2 uv2 : TEXCOORD1;
                };

                v2f vert( appdata_img v )
                {
                        v2f o;

                        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                        o.uv = v.texcoord.xy;
                        o.uv2 = v.texcoord.xy;
						
                        //#if UNITY_UV_STARTS_AT_TOP
                        //        if (_MainTex_TexelSize.y < 0)
                        //                o.uv2.y = 1-o.uv2.y;
                        //#endif
						
                        return o;
                }

				
                float4 fragReflection(v2f i) : SV_Target
                {
                        // Pixel being shaded
                       // float2 tsP = i.uv2.xy;

                       // float roughness = 1.0-tex2D(_CameraGBufferTexture1, tsP).a;

                        float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
						
						//result.r = 1.0;
						//result.g = 1.0;
						//result.b = 0.0;
						//result.a = 0.5;
						
                        return result;
                }


                float4 fragComposite(v2f i) : SV_Target
                {
					
                        // Pixel being shaded
                        float2 tsP = i.uv2.xy;

                        // View space point being shaded
                        float3 C = GetPosition(tsP);

                        // Final image before this pass
                        float4 gbuffer3 = tex2D(_MainTex, i.uv);

                        float4 specEmission = float4(0.0,0.0,0.0,0.0);
                        float3 specColor = tex2D(_CameraGBufferTexture1, tsP).rgb;

                        float roughness = 1.0-tex2D(_CameraGBufferTexture1, tsP).a;

                        float4 reflectionTexel = tex2D(_FinalReflectionTexture, tsP);

                        float4 gbuffer0 = tex2D(_CameraGBufferTexture0, tsP);
                        // Let core Unity functions do the dirty work of applying the BRDF
                        float3 baseColor = gbuffer0.rgb;
                        float occlusion = gbuffer0.a;
                        float oneMinusReflectivity;
                        baseColor = EnergyConservationBetweenDiffuseAndSpecular(baseColor, specColor, oneMinusReflectivity);

                        float3 wsNormal = tex2D(_CameraGBufferTexture2, tsP).rgb * 2.0 - 1.0;

                        float3 csEyeVec = normalize(C);
                        float3 eyeVec = mul(_CameraToWorldMatrix, float4(csEyeVec, 0)).xyz;

                        float3 worldPos =  mul(_CameraToWorldMatrix, float4(C, 1)).xyz;

                        float cos_o = dot(wsNormal, eyeVec);
                        float3 w_mi = -normalize((wsNormal * (2.0 * cos_o)) - eyeVec);


                        float3 incomingRadiance = reflectionTexel.rgb;

                        UnityLight light;
                        light.color = 0;
                        light.dir = 0;
                        light.ndotl = 0;

                        UnityIndirect ind;
                        ind.diffuse = 0;
                        ind.specular = incomingRadiance;

                        float3 ssrResult = UNITY_BRDF_PBS (0, specColor, oneMinusReflectivity, 1-roughness, wsNormal, -eyeVec, light, ind).rgb;
                        float confidence = reflectionTexel.a;

                        specEmission.rgb = tex2D(_CameraReflectionsTexture, tsP).rgb;
                        float3 finalGlossyTerm;

                        // Subtract out Unity's glossy result: (we're just applying the delta)
                        if (_AdditiveReflection == 0)
                        {
                                gbuffer3 -= specEmission;
                                // We may have blown out our dynamic range by adding then subtracting the reflection probes.
                                // As a half-measure to fix this, simply clamp to zero
                                gbuffer3 = max(gbuffer3, 0);
                                finalGlossyTerm = lerp(specEmission.rgb, ssrResult, saturate(confidence));
                        }
                        else
                        {
                                finalGlossyTerm = ssrResult*saturate(confidence);
                        }

                        finalGlossyTerm *= occlusion;

                        // Additively blend the glossy GI result with the output buffer
                        return gbuffer3 + float4(finalGlossyTerm, 0);
						

						float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
						/*
						result.r = 1.0;
						result.g = 0.0;
						result.b = 0.0;
						result.a = 0.5;
						*/
						return result;
                }




        ENDCG

        SubShader
        {
                ZTest Always Cull Off ZWrite Off

                // 0: ReflectionPass
                Pass
                {
                        CGPROGRAM
                                #pragma exclude_renderers gles xbox360 ps3
                                #pragma vertex vert
                                #pragma fragment fragReflection
                                #pragma target 3.0
                        ENDCG
                }
		
                // 1: Composite
                Pass
                {
                        CGPROGRAM
                                #pragma exclude_renderers gles xbox360 ps3
                                #pragma vertex vert
                                #pragma fragment fragComposite
                                #pragma target 3.0
                        ENDCG
                }			

        }

        Fallback "Diffuse"
		
}
*/