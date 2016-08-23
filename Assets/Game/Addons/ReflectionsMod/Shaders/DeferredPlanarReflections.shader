
Shader "Daggerfall/DeferredPlanarReflections"
{
        Properties
        {
                _MainTex ("Base (RGB)", 2D) = "white" {}
        }


        CGINCLUDE
			/*
                #include "UnityCG.cginc"
                #include "UnityPBSLighting.cginc"
                #include "UnityStandardBRDF.cginc"
                #include "UnityStandardUtils.cginc"
                #include "ScreenSpaceRaytrace.cginc"
				*/
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

						/*
                        #if UNITY_UV_STARTS_AT_TOP
                                if (_MainTex_TexelSize.y < 0)
                                        o.uv2.y = 1-o.uv2.y;
                        #endif
						*/
                        return o;
                }

				
                float4 fragReflection(v2f i) : SV_Target
                {
                        // Pixel being shaded
                       // float2 tsP = i.uv2.xy;

                       // float roughness = 1.0-tex2D(_CameraGBufferTexture1, tsP).a;

                        float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
						/*
						result.r = 1.0;
						result.g = 1.0;
						result.b = 0.0;
						result.a = 0.5;
						*/
                        return result;
                }


                float4 fragComposite(v2f i) : SV_Target
                {
					/*
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
						*/

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
