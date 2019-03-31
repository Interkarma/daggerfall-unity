Shader "Daggerfall/PaperDoll"
{
	Properties
    {
        _MainTex ("Texture", any) = "" {}
        _MaskTex ("Texture", any) = "" {}
        _Cutoff  ("Alpha cutoff", Range(0,1)) = 0.9
    }

	SubShader {

		Tags { "ForceSupported" = "True" "RenderType"="Overlay" } 
		
		Lighting Off
		Cull Off
		ZWrite Off
        ZTest Always
		Fog { Mode Off }

        // First pass renders texture with normal alpha blending
		Pass {
            Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
                float2 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
            uniform float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
                return tex2D(_MainTex, i.texcoord).rgba;
			}
			ENDCG 
		}

        // Second pass deletes masked fragments
        // Mask texture should use alpha 0 for non-masked areas and alpha 1 for masked areas
        // Fragments below alpha cutoff are discarded, everything else is cleared to expose background
        Pass {
            Blend Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MaskTex;
            uniform float4 _MaskTex_ST;
            float _Cutoff;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MaskTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 maskColor = tex2D(_MaskTex, i.texcoord).rgba;

                // Discard fragment if not masked
                if (maskColor.a < _Cutoff)
                    discard;

                return float4(0,0,0,0);
            }
            ENDCG
        }
	} 
	
	Fallback off
}