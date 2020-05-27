Shader "Daggerfall/DepthProcessShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Cull Off ZWrite On ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            half4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.texcoord);
                //float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord);
                //depth = Linear01Depth(depth);

                return color;
            }

            ENDCG
        }
    }
}
