Shader "Daggerfall/CreateLookupIndexReflectionTexture" {
    Properties
    {
            _MainTex ("Base (RGB)", 2D) = "white" {}
    }
		
	CGINCLUDE

	#include "UnityCG.cginc"               

    sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	float _GroundLevelHeight;
	float _LowerLevelHeight;     

    struct v2f
    {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
			float3 worldPos : TEXCOORD2;
			float4 screenPos : TEXCOORD3;
    };

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

			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.screenPos = float4(v.texcoord.x, v.texcoord.y, 0.0f, 1.0f);					
						
            return o;
    }
				
    float frag(v2f IN) : SV_Target
    {
			//float4 result = float4(1.0f, 0.0f, 0.0f, 0.5f);
			
			float result = 0.0f;
			if (abs(IN.worldPos.y - _LowerLevelHeight) < 0.01f)
			{
				result = 1.0f; //255.0f / 18.0f;
			}
			else if	(	//(abs(IN.worldPos.y - _GroundLevelHeight) < 0.01f)|| // fragment belong to object on current ground level plane
						(IN.worldPos.y < _GroundLevelHeight)|| // fragment is below (use parallax-corrected reflection)
						(IN.worldPos.y - _GroundLevelHeight < 0.32f) // fragment is slightly above (use parallax-corrected reflection) - also valid for current ground level plane
					)
			{
				result = 2.0f;
			}
            return result;
    }

	ENDCG

	SubShader
	{
		ZTest LEqual Cull Back ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles xbox360 ps3
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
	}	

    Fallback "Diffuse"
}
