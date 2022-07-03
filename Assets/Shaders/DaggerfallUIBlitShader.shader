Shader "Daggerfall/UIBlit"
{
	Properties
    {
        _MainTex ("Texture", any) = "" {}
		_ColorTint ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
    }

	SubShader {

		Tags { "ForceSupported" = "True" "RenderType"="Overlay" } 
		
		Lighting Off
		Cull Off
		ZWrite Off
        ZTest Always
		Fog { Mode Off }

        // Renders texture with normal alpha blending
		Pass {
            Blend Off
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
			float4 _ColorTint;
			
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
                return tex2D(_MainTex, i.texcoord).rgba * _ColorTint;
			}
			ENDCG 
		}
	} 
	
	Fallback off
}