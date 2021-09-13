Shader "Daggerfall/RetroPalettization"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Lut ("Texture", 3D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
       Lighting Off 
	   Cull Off ZWrite On ZTest Always
       Fog { Mode Off } 

		Pass
		{
			CGPROGRAM
           #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile __ EXCLUDE_SKY

			
			#include "UnityCG.cginc"
            
			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}
			
			sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler3D _Lut;
            
	        fixed4 frag (v2f i) : SV_Target
	        {
                // Explore color space!
                // return fixed4(GammaToLinearSpace(tex3D(_Lut, half3(i.texcoord, frac(_Time.x)))), 1.0);

                fixed4 color = tex2D(_MainTex, i.texcoord);
#ifdef EXCLUDE_SKY
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord);
                depth = Linear01Depth(depth);
              
                // Sky untouched
                if (depth == 1)
                  return color;
#endif
                
                return fixed4(GammaToLinearSpace(tex3D(_Lut, LinearToGammaSpace(color.rgb)).rgb), color.a);
            }
			ENDCG
		}
	}
}
