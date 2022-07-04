Shader "Daggerfall/SDFFont"
{
	Properties
    {
        _MainTex ("Texture", any) = "" {}
        _ScissorRect("Scissor Rectangle", Vector) = (0,1,0,1)   // x=left, y=right, z=bottom, w=top - fullscreen is (0,1,0,1)
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

	SubShader {

		Tags { "ForceSupported" = "True" "RenderType"="Overlay" } 
		
		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off 
		ZWrite Off 
		Fog { Mode Off } 
		ZTest Always
		
		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_local __ _MacOSX

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
            float4 _ScissorRect;
			float4 _Color;
			
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
                if (i.screenPos.x < _ScissorRect.x || i.screenPos.x > _ScissorRect.y ||
                    i.screenPos.y < _ScissorRect.z || i.screenPos.y > _ScissorRect.w)
                    discard;

                float dist = tex2D(_MainTex, i.texcoord).a;
                float smoothing = fwidth(dist);
                float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, dist);
                #ifdef _MacOSX
                    return float4(_Color.rgb, alpha);
                #else
                    return float4(i.color.rgb, alpha);
                #endif

                //return float4(float3(1,1,1), 1 - alpha);  // Glyph visualisation
                //return float4(float3(i.screenPos.x, i.screenPos.y, 0), 1 - alpha);  // screenPos visualisation
			}
			ENDCG 
		}
	} 
	
	Fallback off
}